import 'package:flutter/foundation.dart';

/// Biến môi trường build-time và URL API.
///
/// - `--dart-define=SMARTLUNCH_API=https://host:port` — gốc HTTP **không** chứa `/api/v1`
///   (client nối path đầy đủ, xem `core/config/api_paths.dart`).
/// - Android emulator: `localhost` được thay bằng `10.0.2.2` khi cần.
class AppEnv {
  /// Phiên bản API REST (khớp backend `ApiVersion`).
  static const String apiVersion = 'v1';

  static const String _defaultBaseUrl = String.fromEnvironment(
    'SMARTLUNCH_API',
    defaultValue: 'https://localhost:5001',
  );

  static String get apiBaseUrl {
    return _defaultBaseUrl;
  }

  /// Gốc web (PayOS return/cancel) — `--dart-define=SMARTLUNCH_WEB=https://host`.
  static const String _webBaseUrl = String.fromEnvironment(
    'SMARTLUNCH_WEB',
    defaultValue: 'https://localhost:5000',
  );

  static String get webBaseUrl => _webBaseUrl;
}
