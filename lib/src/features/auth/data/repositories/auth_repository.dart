import '../../../../core/config/app_env.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exception.dart';
import '../../auth_types.dart';
import '../auth_storage.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_models.dart';

class AuthRepository {
  AuthRepository._({AuthRemoteDataSource? remote, AuthStorage? storage})
      : _remote = remote ?? AuthRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl)),
        _storage = storage ?? const AuthStorage();

  static final AuthRepository instance = AuthRepository._();

  final AuthRemoteDataSource _remote;
  final AuthStorage _storage;

  static const String _mockEmail = 'khang@gmail.com';
  static const String _mockPassword = '123456';
  static const List<String> _mockUserRoles = [
    'User',
    'Student',
    'Admin',
    'SuperAdmin',
    'Organization',
    'Org',
  ];
  static const List<String> _mockCourierRoles = [
    'Instructor',
    'Courier',
    'Shipper',
    'Delivery',
  ];

  Future<AuthSession> login({
    required String identifier,
    required String password,
    AuthLoginMode mode = AuthLoginMode.user,
  }) async {
    final trimmedIdentifier = identifier.trim();
    if (_isMockCredential(trimmedIdentifier, password)) {
      final session = _buildMockSession(mode);
      await _storage.saveSession(session);
      return session;
    }

    throw ApiException('Email hoặc mật khẩu không đúng.');
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

  Future<AuthSession?> readSavedSession() => _storage.readSession();

  Future<void> clearSession() => _storage.clear();

  bool _isMockCredential(String identifier, String password) {
    return identifier.toLowerCase() == _mockEmail && password == _mockPassword;
  }

  AuthSession _buildMockSession(AuthLoginMode mode) {
    final now = DateTime.now();
    return AuthSession(
      userId: 'mock-user',
      username: 'Khang',
      email: _mockEmail,
      accessToken: 'mock-access-token',
      refreshToken: 'mock-refresh-token',
      refreshTokenExpiresAt: now.add(const Duration(days: 30)),
      roles: _mockRolesForMode(mode),
    );
  }

  List<String> _mockRolesForMode(AuthLoginMode mode) {
    switch (mode) {
      case AuthLoginMode.auto:
        return _mockCourierRoles;
      case AuthLoginMode.admin:
      case AuthLoginMode.user:
      default:
        return _mockUserRoles;
    }
  }
}
