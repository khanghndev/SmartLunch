import '../../../../core/config/api_paths.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exception.dart';
import '../models/customer_menu_models.dart';

class CustomerMenuRemoteDataSource {
  final ApiClient _apiClient;

  CustomerMenuRemoteDataSource(this._apiClient);

  /// Ngày hiệu lực theo lịch local (`yyyy-MM-dd`) — backend lọc `StartDate`–`EndDate` khi có binding query.
  static String effectiveDateQuery(DateTime date) {
    final d = DateTime(date.year, date.month, date.day);
    return '${d.year}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';
  }

  /// `GET /api/v1/master-data/WeeklyMenu` — danh sách thực đơn theo kỳ (phân trang).
  Future<List<CustomerWeeklyMenuSummaryModel>> getWeeklyMenuList({
    required DateTime effectiveDate,
    int page = 1,
    int pageSize = 30,
    int? customerTypeId,
  }) async {
    try {
      final listParams = <String, dynamic>{
        'page': page,
        'pageSize': pageSize,
        'effectiveDate': effectiveDateQuery(effectiveDate),
      };
      if (customerTypeId != null) {
        listParams['customerTypeId'] = customerTypeId;
      }

      final listResp = await _apiClient.get(
        ApiPaths.weeklyMenuList(),
        queryParameters: listParams,
      );

      final root = listResp['data'];
      final List<dynamic> items;
      if (root is List<dynamic>) {
        items = root;
      } else if (root is Map<String, dynamic>) {
        items = root['data'] as List<dynamic>? ?? [];
      } else {
        items = const [];
      }

      return items
          .whereType<Map<String, dynamic>>()
          .map(CustomerWeeklyMenuSummaryModel.fromJson)
          .where((e) => e.id > 0)
          .toList();
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Không tải được danh sách thực đơn: $e', statusCode: 500);
    }
  }

  /// `GET /api/v1/master-data/WeeklyMenu/{id}/detail` — lịch món theo tuần.
  Future<CustomerWeeklyMenuModel?> getWeeklyMenuDetail(int id) async {
    if (id <= 0) return null;
    try {
      final detailResp = await _apiClient.get(
        ApiPaths.weeklyMenuDetail(id),
        queryParameters: const {},
      );

      final detailData = detailResp['data'] as Map<String, dynamic>?;
      if (detailData == null) {
        return null;
      }

      return CustomerWeeklyMenuModel.fromWeeklyMenuDetailApi(detailData);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Không tải được chi tiết thực đơn: $e', statusCode: 500);
    }
  }

  /// `GET /api/v1/organization/meal-order/dish/{id}` — chi tiết món + định mức NL.
  Future<CustomerDishDetailModel> getPublicDishDetail(int dishId) async {
    if (dishId <= 0) {
      throw ApiException('Mã món không hợp lệ', statusCode: 400);
    }
    try {
      final response = await _apiClient.get(
        ApiPaths.orgMealOrderPublicDish(dishId),
        queryParameters: const {},
      );
      final data = response['data'] as Map<String, dynamic>?;
      if (data == null) {
        throw ApiException('Không có dữ liệu món ăn', statusCode: 200);
      }
      final detail = CustomerDishDetailModel.fromJson(data);
      if (detail.id <= 0) {
        throw ApiException('Không tìm thấy món ăn', statusCode: 404);
      }
      return detail;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Không tải được chi tiết món: $e', statusCode: 500);
    }
  }
}
