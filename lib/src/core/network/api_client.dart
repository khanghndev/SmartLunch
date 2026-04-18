import 'dart:convert';

import 'package:http/http.dart' as http;

import 'api_exception.dart';

class ApiClient {
  ApiClient({required String baseUrl, http.Client? client})
      : _baseUri = Uri.parse(baseUrl),
        _client = client ?? http.Client();

  final Uri _baseUri;
  final http.Client _client;

  Future<Map<String, dynamic>> post(
    String path, {
    Map<String, dynamic>? body,
    Map<String, String>? headers,
  }) async {
    final uri = _baseUri.resolve(path);
    final response = await _client.post(
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...?headers,
      },
      body: jsonEncode(body ?? const {}),
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
