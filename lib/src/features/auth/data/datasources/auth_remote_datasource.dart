import '../../../../core/network/api_client.dart';
import '../models/auth_models.dart';

class AuthRemoteDataSource {
  AuthRemoteDataSource(this._client);

  final ApiClient _client;

  Future<LoginData> loginUser({
    required String email,
    required String password,
  }) async {
    final response = await _client.post(
      '/api/v1/auth/login',
      body: {
        'email': email,
        'password': password,
      },
    );

    return LoginData.fromJson(_extractData(response));
  }

  Future<LoginData> loginAdmin({
    required String username,
    required String password,
  }) async {
    final response = await _client.post(
      '/api/v1/auth/login-admin',
      body: {
        'username': username,
        'password': password,
      },
    );

    return LoginData.fromJson(_extractData(response));
  }

  Future<LoginData> loginWithFirebase({required String idToken}) async {
    final response = await _client.post(
      '/api/v1/auth/firebase-login',
      body: {
        'idToken': idToken,
      },
    );

    return LoginData.fromJson(_extractData(response));
  }

  Future<RegisterData> register({
    required String email,
    required String password,
    required String confirmPassword,
  }) async {
    final response = await _client.post(
      '/api/v1/auth/register',
      body: {
        'email': email,
        'password': password,
        'confirmPassword': confirmPassword,
      },
    );

    return RegisterData.fromJson(_extractData(response));
  }

  Future<void> resetPassword({
    required String email,
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _client.put(
      '/api/v1/auth/reset-password',
      body: {
        'email': email,
        'currentPassword': currentPassword,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  Map<String, dynamic> _extractData(Map<String, dynamic> response) {
    final data = response['data'];
    if (data is Map<String, dynamic>) {
      return data;
    }
    return <String, dynamic>{};
  }
}
