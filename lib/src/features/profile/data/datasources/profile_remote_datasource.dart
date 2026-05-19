import '../../../../core/config/api_paths.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exception.dart';
import '../models/user_profile_model.dart';

class ProfileRemoteDataSource {
  final ApiClient _apiClient;

  ProfileRemoteDataSource(this._apiClient);

  Future<UserProfileModel> getProfile() async {
    try {
      final response = await _apiClient.get(
        ApiPaths.authProfile,
        queryParameters: {},
      );
      final data = response['data'] as Map<String, dynamic>?;
      if (data == null) {
        throw ApiException('Profile data is empty', statusCode: 200);
      }
      return UserProfileModel.fromJson(data);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Failed to fetch profile: $e', statusCode: 500);
    }
  }
}
