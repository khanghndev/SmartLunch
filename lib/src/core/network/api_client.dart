import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:http_parser/http_parser.dart';

import '../../features/auth/data/auth_storage.dart';
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
class ApiClient {
  ApiClient({required String baseUrl, http.Client? client})
      : _baseUri = Uri.parse(baseUrl),
        _client = client ?? http.Client();

  final Uri _baseUri;
  final http.Client _client;

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
    final resolved = _baseUri.resolve(path);
    final uri = _uriWithQuery(resolved, queryParameters);
    final authHeaders = await _getAuthHeaders();
    final response = await _client.get(
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...authHeaders,
        ...?headers,
      },
    );

    return _parseResponse(response);
  }

  /// Multipart POST (không set `Content-Type` thủ công — client tự thêm boundary).
  Future<Map<String, dynamic>> postMultipart(
    String path, {
    Map<String, String> fields = const {},
    required List<http.MultipartFile> files,
    Map<String, String>? headers,
  }) async {
    final uri = _baseUri.resolve(path);
    final request = http.MultipartRequest('POST', uri);
    final authHeaders = await _getAuthHeaders();
    request.headers.addAll({...authHeaders, ...?headers});
    request.fields.addAll(fields);
    request.files.addAll(files);
    final streamed = await _client.send(request);
    final response = await http.Response.fromStream(streamed);
    return _parseResponse(response);
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

  Future<Map<String, dynamic>> post(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    final uri = _baseUri.resolve(path);
    final authHeaders = await _getAuthHeaders();
    final response = await _client.post(
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...authHeaders,
        ...?headers,
      },
      body: jsonEncode(body ?? const {}),
    );

    return _parseResponse(response);
  }

  Future<Map<String, dynamic>> patch(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    final uri = _baseUri.resolve(path);
    final authHeaders = await _getAuthHeaders();
    final response = await _client.patch(
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...authHeaders,
        ...?headers,
      },
      body: jsonEncode(body ?? const {}),
    );

    return _parseResponse(response);
  }

  Future<Map<String, dynamic>> put(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    final uri = _baseUri.resolve(path);
    final authHeaders = await _getAuthHeaders();
    final response = await _client.put(
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...authHeaders,
        ...?headers,
      },
      body: jsonEncode(body ?? const {}),
    );

    return _parseResponse(response);
  }

  Future<Map<String, dynamic>> delete(
    String path, {
    Map<String, String>? headers,
  }) async {
    final uri = _baseUri.resolve(path);
    final authHeaders = await _getAuthHeaders();
    final response = await _client.delete(
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...authHeaders,
        ...?headers,
      },
    );

    return _parseResponse(response);
  }

  Map<String, dynamic> _parseResponse(http.Response response) {
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
    if (response.statusCode < 200 || response.statusCode >= 300 || success == false) {
      throw ApiException(
        _extractMessage(decoded),
        statusCode: response.statusCode,
        errors: _extractErrors(decoded),
      );
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
