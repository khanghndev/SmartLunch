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
}
