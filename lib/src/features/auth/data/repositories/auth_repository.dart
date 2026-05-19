import '../../../../core/config/app_env.dart';
import '../../../../core/network/api_client.dart';
import '../../../profile/data/profile_repository.dart';
import '../auth_types.dart';
import '../auth_storage.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_models.dart';
import '../../../../core/utils/jwt_utils.dart';

class AuthRepository {
  AuthRepository._({AuthRemoteDataSource? remote, AuthStorage? storage})
    : _remote =
          remote ?? AuthRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl)),
      _storage = storage ?? const AuthStorage();

  static final AuthRepository instance = AuthRepository._();

  final AuthRemoteDataSource _remote;
  final AuthStorage _storage;


  Future<AuthSession> login({
    required String identifier,
    required String password,
    AuthLoginMode mode = AuthLoginMode.user,
  }) async {
    final trimmedIdentifier = identifier.trim();

    try {
      final loginData = await _remote.loginUser(
        email: trimmedIdentifier,
        password: password,
      );

      final roles = JwtUtils.extractRoles(loginData.accessToken);

      final session = AuthSession(
        userId: loginData.userId,
        username: loginData.username,
        email: loginData.email,
        accessToken: loginData.accessToken,
        refreshToken: loginData.refreshToken,
        refreshTokenExpiresAt: loginData.refreshTokenExpiresAt,
        roles: roles,
      );

      await _storage.saveSession(session);
      return await _enrichSessionRolesFromProfile(session);
    } catch (e) {
      rethrow;
    }
  }

  Future<LoginData> loginWithFirebase({required String idToken}) async {
    return _remote.loginWithFirebase(idToken: idToken);
  }

  Future<RegisterData> register({
    required String email,
    required String password,
    required String confirmPassword,
  }) async {
    return _remote.register(
      email: email.trim(),
      password: password,
      confirmPassword: confirmPassword,
    );
  }

  Future<void> resetPassword({
    required String email,
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    return _remote.resetPassword(
      email: email.trim(),
      currentPassword: currentPassword,
      newPassword: newPassword,
      confirmPassword: confirmPassword,
    );
  }

  Future<AuthSession?> readSavedSession() => _storage.readSession();

  Future<void> clearSession() => _storage.clear();



  /// Sau khi lưu token, gọi `GET /api/v1/Auth/profile` để lấy `roles` chuẩn từ BE (UserProfileResponse).
  /// Nếu lỗi hoặc roles rỗng, giữ roles suy ra từ JWT.
  Future<AuthSession> _enrichSessionRolesFromProfile(
    AuthSession session,
  ) async {
    try {
      final profile = await ProfileRepository.instance.getProfile();
      if (profile.roles.isEmpty) {
        return session;
      }
      final updated = session.copyWith(roles: profile.roles);
      await _storage.saveSession(updated);
      return updated;
    } catch (_) {
      return session;
    }
  }
}
