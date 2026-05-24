import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart' as http;

import '../../features/auth/data/auth_storage.dart';
import '../../features/auth/data/models/auth_models.dart';
import '../config/api_paths.dart';

/// Gọi `POST /api/v1/Auth/refresh-token` khi access token hết hạn (401).
///
/// Nhiều request 401 đồng thời chỉ refresh một lần (shared [Completer]).
class TokenRefreshCoordinator {
  TokenRefreshCoordinator._();

  static final TokenRefreshCoordinator instance = TokenRefreshCoordinator._();

  Completer<bool>? _inFlight;

  /// Trả `true` nếu đã lưu access/refresh token mới.
  Future<bool> tryRefresh({
    required Uri baseUri,
    required http.Client client,
  }) async {
    if (_inFlight != null) {
      return _inFlight!.future;
    }

    final completer = Completer<bool>();
    _inFlight = completer;

    try {
      final ok = await _refreshOnce(baseUri: baseUri, client: client);
      completer.complete(ok);
      return ok;
    } catch (_) {
      completer.complete(false);
      return false;
    } finally {
      _inFlight = null;
    }
  }

  Future<bool> _refreshOnce({
    required Uri baseUri,
    required http.Client client,
  }) async {
    final storage = const AuthStorage();
    final session = await storage.readSession();
    if (session == null ||
        session.accessToken.isEmpty ||
        session.refreshToken.isEmpty) {
      return false;
    }

    final uri = baseUri.resolve(ApiPaths.authRefreshToken);
    final response = await client.post(
      uri,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ${session.accessToken}',
      },
      body: jsonEncode({'refreshToken': session.refreshToken}),
    );

    if (response.statusCode != 200 || response.body.isEmpty) {
      return false;
    }

    final dynamic decoded;
    try {
      decoded = jsonDecode(response.body);
    } catch (_) {
      return false;
    }

    if (decoded is! Map<String, dynamic>) return false;
    if (decoded['success'] != true) return false;

    final data = decoded['data'];
    if (data is! Map<String, dynamic>) return false;

    final tokens = RefreshTokenData.fromJson(data);
    if (tokens.accessToken.isEmpty || tokens.refreshToken.isEmpty) {
      return false;
    }

    await storage.saveSession(
      session.copyWith(
        accessToken: tokens.accessToken,
        refreshToken: tokens.refreshToken,
        refreshTokenExpiresAt: tokens.refreshTokenExpiresAt,
      ),
    );

    return true;
  }
}
