import '../../../../core/config/api_paths.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exception.dart';
import '../models/finance_models.dart';
import '../models/review_complaint_models.dart';

class ManagerRemoteDataSource {
  const ManagerRemoteDataSource(this._client);

  final ApiClient _client;

  Future<CashFlowSummaryModel> getCashFlowSummary({
    required DateTime startDate,
    required DateTime endDate,
    String granularity = 'Day',
  }) async {
    try {
      final response = await _client.get(
        ApiPaths.managerCashFlowSummary,
        queryParameters: {
          'from': _dateStr(startDate),
          'to': _dateStr(endDate),
          'granularity': granularity,
        },
      );
      return CashFlowSummaryModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải tổng quan thu chi: $e', statusCode: 500);
    }
  }

  Future<PaymentReconciliationModel> getPaymentReconciliation({
    required DateTime startDate,
    required DateTime endDate,
    bool onlyMismatches = true,
    bool includeCancelledOrders = false,
  }) async {
    try {
      final response = await _client.get(
        ApiPaths.managerPaymentReconciliation,
        queryParameters: {
          'from': _dateStr(startDate),
          'to': _dateStr(endDate),
          'onlyMismatches': onlyMismatches,
          'includeCancelledOrders': includeCancelledOrders,
        },
      );
      return PaymentReconciliationModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải đối soát thanh toán: $e', statusCode: 500);
    }
  }

  Future<OrganizationReceivablesModel> getOrganizationReceivables({
    int? organizationId,
  }) async {
    try {
      final q = <String, dynamic>{};
      if (organizationId != null) q['organizationId'] = organizationId;
      final response = await _client.get(
        ApiPaths.managerOrgReceivables,
        queryParameters: q,
      );
      return OrganizationReceivablesModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải công nợ tổ chức: $e', statusCode: 500);
    }
  }

  Future<SupplierPayablesModel> getSupplierPayables({int? partnerId}) async {
    try {
      final q = <String, dynamic>{};
      if (partnerId != null) q['partnerId'] = partnerId;
      final response = await _client.get(
        ApiPaths.managerSupplierPayables,
        queryParameters: q,
      );
      return SupplierPayablesModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải công nợ nhà cung cấp: $e', statusCode: 500);
    }
  }

  Future<FinancePaymentHistoryModel> getPaymentHistory({
    required DateTime startDate,
    required DateTime endDate,
    String scope = 'All',
    int page = 1,
    int pageSize = 30,
  }) async {
    try {
      final response = await _client.get(
        ApiPaths.managerPaymentHistory,
        queryParameters: {
          'from': _dateStr(startDate),
          'to': _dateStr(endDate),
          'scope': scope,
          'page': page,
          'pageSize': pageSize,
        },
      );
      return FinancePaymentHistoryModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải lịch sử thanh toán: $e', statusCode: 500);
    }
  }

  Future<ReviewPageModel> getReviews({
    int page = 1,
    int pageSize = 20,
    String? searchTerm,
  }) async {
    try {
      final q = <String, dynamic>{
        'page': page,
        'pageSize': pageSize,
      };
      if (searchTerm != null && searchTerm.trim().isNotEmpty) {
        q['searchTerm'] = searchTerm.trim();
      }
      final response = await _client.get(
        ApiPaths.managerReviews,
        queryParameters: q,
      );
      return ReviewPageModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải danh sách đánh giá: $e', statusCode: 500);
    }
  }

  Future<ComplaintPageModel> getComplaints({
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final response = await _client.get(
        ApiPaths.managerComplaints,
        queryParameters: {'page': page, 'pageSize': pageSize},
      );
      return ComplaintPageModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải danh sách khiếu nại: $e', statusCode: 500);
    }
  }

  static String _dateStr(DateTime d) {
    final x = DateTime(d.year, d.month, d.day);
    return '${x.year}-${x.month.toString().padLeft(2, '0')}-${x.day.toString().padLeft(2, '0')}';
  }
}
