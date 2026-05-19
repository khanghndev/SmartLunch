import '../../../../core/config/app_env.dart';
import '../../../../core/network/api_client.dart';
import '../datasources/manager_remote_datasource.dart';
import '../models/finance_models.dart';
import '../models/review_complaint_models.dart';

class ManagerRepository {
  ManagerRepository._({ManagerRemoteDataSource? remote})
      : _remote = remote ??
            ManagerRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl));

  static final ManagerRepository instance = ManagerRepository._();

  final ManagerRemoteDataSource _remote;

  Future<CashFlowSummaryModel> getCashFlowSummary({
    required DateTime startDate,
    required DateTime endDate,
    String granularity = 'Day',
  }) =>
      _remote.getCashFlowSummary(
        startDate: startDate,
        endDate: endDate,
        granularity: granularity,
      );

  Future<PaymentReconciliationModel> getPaymentReconciliation({
    required DateTime startDate,
    required DateTime endDate,
    bool onlyMismatches = true,
  }) =>
      _remote.getPaymentReconciliation(
        startDate: startDate,
        endDate: endDate,
        onlyMismatches: onlyMismatches,
      );

  Future<OrganizationReceivablesModel> getOrganizationReceivables({
    int? organizationId,
  }) =>
      _remote.getOrganizationReceivables(organizationId: organizationId);

  Future<SupplierPayablesModel> getSupplierPayables({int? partnerId}) =>
      _remote.getSupplierPayables(partnerId: partnerId);

  Future<FinancePaymentHistoryModel> getPaymentHistory({
    required DateTime startDate,
    required DateTime endDate,
    int page = 1,
    int pageSize = 30,
  }) =>
      _remote.getPaymentHistory(
        startDate: startDate,
        endDate: endDate,
        page: page,
        pageSize: pageSize,
      );

  Future<ReviewPageModel> getReviews({
    int page = 1,
    int pageSize = 20,
    String? searchTerm,
  }) =>
      _remote.getReviews(page: page, pageSize: pageSize, searchTerm: searchTerm);

  Future<ComplaintPageModel> getComplaints({
    int page = 1,
    int pageSize = 20,
  }) =>
      _remote.getComplaints(page: page, pageSize: pageSize);
}

String managerDateQuery(DateTime d) {
  final x = DateTime(d.year, d.month, d.day);
  return '${x.year}-${x.month.toString().padLeft(2, '0')}-${x.day.toString().padLeft(2, '0')}';
}

/// Map UI period → backend CashflowGranularity enum name.
String cashflowGranularityFor(String period) {
  switch (period) {
    case 'weekly':
      return 'Week';
    case 'monthly':
      return 'Month';
    default:
      return 'Day';
  }
}
