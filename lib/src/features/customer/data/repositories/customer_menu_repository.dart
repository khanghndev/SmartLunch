import '../../../../core/config/app_env.dart';
import '../../../../core/network/api_client.dart';
import '../datasources/customer_menu_remote_datasource.dart';
import '../models/customer_menu_models.dart';

class CustomerMenuRepository {
  CustomerMenuRepository._({CustomerMenuRemoteDataSource? remote})
      : _remote = remote ?? CustomerMenuRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl));

  static final CustomerMenuRepository instance = CustomerMenuRepository._();

  final CustomerMenuRemoteDataSource _remote;

  /// Danh sách thực đơn: `GET .../master-data/WeeklyMenu`.
  Future<List<CustomerWeeklyMenuSummaryModel>> listWeeklyMenus({
    DateTime? effectiveDate,
    int? customerTypeId,
    int page = 1,
    int pageSize = 30,
  }) {
    return _remote.getWeeklyMenuList(
      effectiveDate: effectiveDate ?? DateTime.now(),
      customerTypeId: customerTypeId,
      page: page,
      pageSize: pageSize,
    );
  }

  /// Chi tiết một kỳ: `GET .../master-data/WeeklyMenu/{id}/detail`.
  Future<CustomerWeeklyMenuModel?> getWeeklyMenuDetail(int weeklyMenuId) {
    return _remote.getWeeklyMenuDetail(weeklyMenuId);
  }
}
