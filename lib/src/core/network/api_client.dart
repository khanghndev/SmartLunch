import 'dart:async';
import 'dart:convert';
import 'dart:typed_data';

import 'package:http/http.dart' as http;
import 'package:http_parser/http_parser.dart';

import '../../features/auth/data/auth_storage.dart';
import '../auth/token_refresh_coordinator.dart';
import '../config/api_paths.dart';
import '../navigation/session_guard.dart';
import 'api_exception.dart';

Uri _uriWithQuery(Uri base, Map<String, dynamic> queryParameters) {
  if (queryParameters.isEmpty) {
    return base;
  }
  final merged = Map<String, String>.from(base.queryParameters);
  for (final e in queryParameters.entries) {
    final v = e.value;
    if (v == null) continue;
    final s = v is String ? v : v.toString();
    if (s.isEmpty) continue;
    merged[e.key] = s;
  }
  return base.replace(queryParameters: merged);
}

/// HTTP client gọi SmartLunch API.
///
/// Phản hồi JSON kiểu `BaseApiResponse`: `success`, `status`, `message`, `timestamp`, `data`;
/// lỗi có thể kèm `errors` / `error` / `errorDetails` (xử lý trong [_parseResponse]).
///
/// **401:** thử `POST` [ApiPaths.authRefreshToken]; nếu có JWT mới → lưu và retry request;
/// nếu không → [SessionGuard] về `/login`.
class ApiClient {
  ApiClient({required String baseUrl, http.Client? client})
      : _baseUri = Uri.parse(baseUrl),
        _client = client ?? http.Client();

  final Uri _baseUri;
  final http.Client _client;

  Map<String, String> get _ngrokHeaders {
    final headers = <String, String>{};
    final host = _baseUri.host.toLowerCase();
    if (host.contains('ngrok')) {
      headers['ngrok-skip-browser-warning'] = 'true';
    }
    return headers;
  }

  Map<String, String> get _defaultHeaders {
    return {
      'Content-Type': 'application/json',
      ..._ngrokHeaders,
    };
  }

  Future<Map<String, String>> _getAuthHeaders() async {
    final session = await const AuthStorage().readSession();
    if (session != null) {
      return {'Authorization': 'Bearer ${session.accessToken}'};
    }
    return {};
  }

  Future<Map<String, dynamic>> get(
    String path, {
    Map<String, String>? headers,
    Map<String, dynamic> queryParameters = const {},
  }) async {
    return _execute(
      path: path,
      send: () async {
        final resolved = _baseUri.resolve(path);
        final uri = _uriWithQuery(resolved, queryParameters);
        final authHeaders = await _getAuthHeaders();
        return _client.get(
          uri,
          headers: {..._defaultHeaders, ...authHeaders, ...?headers},
        );
      },
    );
  }

  /// Multipart POST (không set `Content-Type` thủ công — client tự thêm boundary).
  Future<Map<String, dynamic>> postMultipart(
    String path, {
    Map<String, String> fields = const {},
    required List<http.MultipartFile> files,
    Map<String, String>? headers,
  }) async {
    return _execute(
      path: path,
      send: () async {
        final uri = _baseUri.resolve(path);
        final request = http.MultipartRequest('POST', uri);
        final authHeaders = await _getAuthHeaders();
        request.headers.addAll({
          ..._ngrokHeaders,
          ...authHeaders,
          ...?headers,
        });
        request.fields.addAll(fields);
        request.files.addAll(files);
        final streamed = await _client.send(request);
        return http.Response.fromStream(streamed);
      },
    );
  }

  /// Tiện ích: tạo [MultipartFile] từ bytes với field name chuẩn upload ảnh.
  static http.MultipartFile multipartImageBytes({
    required String fieldName,
    required List<int> bytes,
    required String filename,
    required String contentType,
  }) {
    return http.MultipartFile.fromBytes(
      fieldName,
      bytes,
      filename: filename,
      contentType: MediaType.parse(contentType),
    );
  }

  /// GET trả về bytes (PDF, file…) — không parse JSON.
  Future<Uint8List> getBytes(
    String path, {
    Map<String, String>? headers,
    Map<String, dynamic> queryParameters = const {},
  }) async {
    return _executeBytes(
      path: path,
      send: () async {
        final resolved = _baseUri.resolve(path);
        final uri = _uriWithQuery(resolved, queryParameters);
        final authHeaders = await _getAuthHeaders();
        return _client.get(
          uri,
          headers: {
            ..._ngrokHeaders,
            ...authHeaders,
            ...?headers,
          },
        );
      },
    );
  }

  Future<Map<String, dynamic>> post(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    return _execute(
      path: path,
      send: () async {
        final uri = _baseUri.resolve(path);
        final authHeaders = await _getAuthHeaders();
        return _client.post(
          uri,
          headers: {..._defaultHeaders, ...authHeaders, ...?headers},
          body: jsonEncode(body ?? const {}),
        );
      },
    );
  }

  Future<Map<String, dynamic>> patch(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    return _execute(
      path: path,
      send: () async {
        final uri = _baseUri.resolve(path);
        final authHeaders = await _getAuthHeaders();
        return _client.patch(
          uri,
          headers: {..._defaultHeaders, ...authHeaders, ...?headers},
          body: jsonEncode(body ?? const {}),
        );
      },
    );
  }

  Future<Map<String, dynamic>> put(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    return _execute(
      path: path,
      send: () async {
        final uri = _baseUri.resolve(path);
        final authHeaders = await _getAuthHeaders();
        return _client.put(
          uri,
          headers: {..._defaultHeaders, ...authHeaders, ...?headers},
          body: jsonEncode(body ?? const {}),
        );
      },
    );
  }

  Future<Map<String, dynamic>> delete(
    String path, {
    Map<String, String>? headers,
  }) async {
    return _execute(
      path: path,
      send: () async {
        final uri = _baseUri.resolve(path);
        final authHeaders = await _getAuthHeaders();
        return _client.delete(
          uri,
          headers: {..._defaultHeaders, ...authHeaders, ...?headers},
        );
      },
    );
  }

  Future<Uint8List> _executeBytes({
    required String path,
    required Future<http.Response> Function() send,
  }) async {
    var response = await send();

    if (_shouldAttemptRefresh(response.statusCode, path)) {
      final refreshed = await TokenRefreshCoordinator.instance.tryRefresh(
        baseUri: _baseUri,
        client: _client,
      );
      if (refreshed) {
        response = await send();
      }
    }

    if (response.statusCode == 401 &&
        !_isPublicAuthPath(path) &&
        !_isRefreshTokenPath(path)) {
      unawaited(SessionGuard.instance.handleUnauthorized());
      throw ApiUnauthorizedException(
        'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
      );
    }

    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException(
        'Không tải được tệp (${response.statusCode})',
        statusCode: response.statusCode,
      );
    }

    return response.bodyBytes;
  }

  Future<Map<String, dynamic>> _execute({
    required String path,
    required Future<http.Response> Function() send,
  }) async {
    var response = await send();

    if (_shouldAttemptRefresh(response.statusCode, path)) {
      final refreshed = await TokenRefreshCoordinator.instance.tryRefresh(
        baseUri: _baseUri,
        client: _client,
      );
      if (refreshed) {
        response = await send();
      }
    }

    return _parseResponse(response, requestPath: path);
  }

  static String _normalizePath(String path) => path.toLowerCase().split('?').first;

  /// Endpoint đăng nhập/đăng ký — 401 là sai mật khẩu, không refresh/đá session.
  static bool _isPublicAuthPath(String path) {
    final p = _normalizePath(path);
    const public = {
      '/api/v1/auth/login',
      '/api/v1/auth/login-admin',
      '/api/v1/auth/firebase-login',
      '/api/v1/auth/register',
      '/api/v1/auth/reset-password',
    };
    return public.contains(p);
  }

  static bool _isRefreshTokenPath(String path) {
    return _normalizePath(path) == _normalizePath(ApiPaths.authRefreshToken);
  }

  static bool _shouldAttemptRefresh(int statusCode, String path) {
    if (statusCode != 401) return false;
    if (_isPublicAuthPath(path)) return false;
    if (_isRefreshTokenPath(path)) return false;
    return true;
  }

  Map<String, dynamic> _parseResponse(
    http.Response response, {
    required String requestPath,
  }) {
    if (response.statusCode == 401 &&
        !_isPublicAuthPath(requestPath) &&
        !_isRefreshTokenPath(requestPath)) {
      unawaited(SessionGuard.instance.handleUnauthorized());
      throw ApiUnauthorizedException(
        'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
      );
    }

    if (response.body.isEmpty) {
      throw ApiException('Máy chủ không trả về dữ liệu', statusCode: response.statusCode);
    }

    dynamic decoded;
    try {
      decoded = jsonDecode(response.body);
    } catch (_) {
      throw ApiException('Không thể đọc phản hồi từ máy chủ', statusCode: response.statusCode);
    }
    if (decoded is! Map<String, dynamic>) {
      throw ApiException('Định dạng phản hồi không hợp lệ', statusCode: response.statusCode);
    }

    final success = decoded['success'];
    final statusCode = response.statusCode;
    final isError = statusCode < 200 || statusCode >= 300 || success == false;

    if (isError) {
      final message = _extractMessage(decoded);
      final errors = _extractErrors(decoded);

      if (statusCode == 401 &&
          !_isPublicAuthPath(requestPath) &&
          !_isRefreshTokenPath(requestPath)) {
        unawaited(SessionGuard.instance.handleUnauthorized());
        throw ApiUnauthorizedException(message, errors: errors);
      }

      throw ApiException(message, statusCode: statusCode, errors: errors);
    }

    return decoded;
  }

  String _extractMessage(Map<String, dynamic> decoded) {
    return decoded['message']?.toString().trim().isNotEmpty == true
        ? decoded['message'].toString()
        : decoded['error']?.toString() ?? 'Yêu cầu thất bại';
  }

  List<String> _extractErrors(Map<String, dynamic> decoded) {
    final errors = decoded['errors'];
    if (errors is List) {
      return errors.map((e) => e.toString()).toList();
    }
    final errorDetail = decoded['errorDetails'];
    if (errorDetail != null) {
      return [errorDetail.toString()];
    }
    return const [];
  }
}
