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

  Future<UserProfileModel> updateProfile({
    required String firstName,
    required String lastName,
    required String phoneNumber,
    String? address,
  }) async {
    try {
      final response = await _apiClient.put(
        ApiPaths.authProfile,
        body: {
          'firstName': firstName,
          'lastName': lastName,
          'phoneNumber': phoneNumber,
          'address': address,
        },
      );
      final data = response['data'] as Map<String, dynamic>?;
      if (data == null) {
        throw ApiException('Response data is empty', statusCode: 200);
      }
      return UserProfileModel.fromJson(data);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Failed to update profile: $e', statusCode: 500);
    }
  }

  Future<String> uploadAvatar({
    required List<int> bytes,
    required String filename,
    required String contentType,
  }) async {
    try {
      final multipartFile = ApiClient.multipartImageBytes(
        fieldName: 'file',
        bytes: bytes,
        filename: filename,
        contentType: contentType,
      );
      final response = await _apiClient.postMultipart(
        '${ApiPaths.authProfile}/avatar',
        files: [multipartFile],
      );
      final data = response['data'] as Map<String, dynamic>?;
      final url = data?['url']?.toString();
      if (url == null) {
        throw ApiException('Upload response missing image URL', statusCode: 200);
      }
      return url;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Failed to upload avatar: $e', statusCode: 500);
    }
  }
}
