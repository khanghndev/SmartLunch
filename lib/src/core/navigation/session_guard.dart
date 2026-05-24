import 'package:flutter/material.dart';

import '../../app/app_routes.dart';
import '../../features/auth/data/repositories/auth_repository.dart';

/// Điều hướng toàn app khi phiên hết hạn (HTTP 401).
class SessionGuard {
  SessionGuard._();

  static final SessionGuard instance = SessionGuard._();

  final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

  bool _isHandling = false;

  /// Xóa session và đưa user về màn đăng nhập (chỉ chạy một lần nếu nhiều request 401).
  Future<void> handleUnauthorized() async {
    if (_isHandling) return;
    _isHandling = true;
    try {
      await AuthRepository.instance.clearSession();

      final nav = navigatorKey.currentState;
      if (nav == null) return;

      nav.pushNamedAndRemoveUntil(AppRoutes.login, (route) => false);
    } finally {
      _isHandling = false;
    }
  }
}
