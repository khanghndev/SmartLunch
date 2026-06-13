import '../../../core/config/app_env.dart';
import '../../../core/network/api_client.dart';
import 'datasources/profile_remote_datasource.dart';
import 'models/user_profile_model.dart';

class ProfileRepository {
  ProfileRepository._({ProfileRemoteDataSource? remote})
      : _remote = remote ??
            ProfileRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl));

  static final ProfileRepository instance = ProfileRepository._();

  final ProfileRemoteDataSource _remote;

  Future<UserProfileModel> getProfile() => _remote.getProfile();

  Future<UserProfileModel> updateProfile({
    required String firstName,
    required String lastName,
    required String phoneNumber,
    String? address,
  }) =>
      _remote.updateProfile(
        firstName: firstName,
        lastName: lastName,
        phoneNumber: phoneNumber,
        address: address,
      );

  Future<String> uploadAvatar({
    required List<int> bytes,
    required String filename,
    required String contentType,
  }) =>
      _remote.uploadAvatar(
        bytes: bytes,
        filename: filename,
        contentType: contentType,
      );
}
