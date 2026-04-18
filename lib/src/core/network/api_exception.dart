class ApiException implements Exception {
  final String message;
  final int? statusCode;
  final List<String> errors;

  ApiException(this.message, {this.statusCode, List<String>? errors})
      : errors = errors ?? const [];

  @override
  String toString() => 'ApiException(statusCode: $statusCode, message: $message)';
}
