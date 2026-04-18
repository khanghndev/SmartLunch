import 'package:flutter/foundation.dart';

class AppEnv {
  static const String _defaultBaseUrl =
      String.fromEnvironment('SMARTLUNCH_API', defaultValue: 'http://localhost:5001');

  static String get apiBaseUrl {
    if (kIsWeb) {
      return _defaultBaseUrl;
    }

    if (!kIsWeb &&
        defaultTargetPlatform == TargetPlatform.android &&
        _defaultBaseUrl.contains('localhost')) {
      return _defaultBaseUrl.replaceFirst('localhost', '10.0.2.2');
    }

    return _defaultBaseUrl;
  }
}
