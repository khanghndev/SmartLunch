import 'dart:convert';

import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'models/auth_models.dart';

class AuthStorage {
  static const _accessTokenKey = 'auth_access_token';
  static const _refreshTokenKey = 'auth_refresh_token';
  static const _refreshTokenExpiresAtKey = 'auth_refresh_expires_at';
  static const _userIdKey = 'auth_user_id';
  static const _emailKey = 'auth_email';
  static const _usernameKey = 'auth_username';
  static const _rolesKey = 'auth_roles';

  /// Cache RAM — tránh đọc Secure Storage 6+ lần mỗi HTTP request (gây chậm rõ trên mobile).
  static AuthSession? _memoryCache;

  const AuthStorage();

  FlutterSecureStorage get _storage => const FlutterSecureStorage();

  Future<void> saveSession(AuthSession session) async {
    _memoryCache = session;
    await _storage.write(key: _accessTokenKey, value: session.accessToken);
    await _storage.write(key: _refreshTokenKey, value: session.refreshToken);
    await _storage.write(
      key: _refreshTokenExpiresAtKey,
      value: session.refreshTokenExpiresAt.toIso8601String(),
    );
    await _storage.write(key: _userIdKey, value: session.userId);
    await _storage.write(key: _emailKey, value: session.email);
    await _storage.write(key: _usernameKey, value: session.username);
    await _storage.write(key: _rolesKey, value: jsonEncode(session.roles));
  }

  Future<AuthSession?> readSession() async {
    if (_memoryCache != null) {
      return _memoryCache;
    }

    final accessToken = await _storage.read(key: _accessTokenKey);
    final refreshToken = await _storage.read(key: _refreshTokenKey);
    if (accessToken == null || refreshToken == null) {
      return null;
    }

    final refreshExpiresAtRaw =
        await _storage.read(key: _refreshTokenExpiresAtKey);
    final userId = await _storage.read(key: _userIdKey) ?? '';
    final email = await _storage.read(key: _emailKey) ?? '';
    final username = await _storage.read(key: _usernameKey) ?? '';
    final rolesRaw = await _storage.read(key: _rolesKey);

    final roles = <String>[];
    if (rolesRaw != null && rolesRaw.isNotEmpty) {
      final decoded = jsonDecode(rolesRaw);
      if (decoded is List) {
        roles.addAll(decoded.map((e) => e.toString()));
      }
    }

    final session = AuthSession(
      userId: userId,
      username: username,
      email: email,
      accessToken: accessToken,
      refreshToken: refreshToken,
      refreshTokenExpiresAt: DateTime.tryParse(refreshExpiresAtRaw ?? '') ??
          DateTime.now(),
      roles: roles,
    );
    _memoryCache = session;
    return session;
  }

  Future<void> clear() async {
    _memoryCache = null;
    await _storage.deleteAll();
  }
}
