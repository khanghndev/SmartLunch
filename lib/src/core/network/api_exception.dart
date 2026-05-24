class ApiException implements Exception {
  final String message;
  final int? statusCode;
  final List<String> errors;

  ApiException(this.message, {this.statusCode, List<String>? errors})
      : errors = errors ?? const [];

  bool get isUnauthorized => statusCode == 401;

  @override
  String toString() => 'ApiException(statusCode: $statusCode, message: $message)';
}

/// Phiên đăng nhập không hợp lệ — app sẽ chuyển về login.
class ApiUnauthorizedException extends ApiException {
  ApiUnauthorizedException(String message, {List<String>? errors})
      : super(message, statusCode: 401, errors: errors);
}
