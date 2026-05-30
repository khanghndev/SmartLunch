import 'dart:typed_data';

import '../../../../core/config/api_paths.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exception.dart';
import '../../utils/org_meal_order_request_builder.dart';
import '../models/bulk_order_models.dart';
import '../models/contract_models.dart';
import '../models/customer_review_models.dart';
import '../models/org_meal_contract_models.dart';

/// Remote datasource Organization — meal-order + contracts.
class OrgRemoteDataSource {
  const OrgRemoteDataSource(this._client);

  final ApiClient _client;

  static const _prefix = '/api/v1';

  Future<DishCategoriesResponseModel> getDishCategories() async {
    try {
      final response = await _client.get(
        '$_prefix/organization/meal-order/dish-category',
        queryParameters: {},
      );
      return DishCategoriesResponseModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải danh mục món: $e', statusCode: 500);
    }
  }

  Future<DishesByCategoryResponseModel> getDishesByCategory(
    int categoryId, {
    int page = 1,
    int pageSize = 50,
  }) async {
    try {
      final response = await _client.get(
        '$_prefix/organization/meal-order/dish/category',
        queryParameters: {
          'categoryId': categoryId,
          'page': page,
          'pageSize': pageSize,
        },
      );
      return DishesByCategoryResponseModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải danh sách món: $e', statusCode: 500);
    }
  }

  Future<ListEligiblePromotionsModel> listEligiblePromotions(
    Map<String, dynamic> previewBody,
  ) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-order/promotions/eligible',
        body: previewBody,
      );
      return ListEligiblePromotionsModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải mã khuyến mãi: $e', statusCode: 500);
    }
  }

  Future<PreviewPromotionModel> previewPromotion(
    Map<String, dynamic> previewBody,
  ) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-order/promotions/preview',
        body: previewBody,
      );
      return PreviewPromotionModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi xem trước khuyến mãi: $e', statusCode: 500);
    }
  }

  Future<PrepareMealDraftModel> prepareMealContract({
    required int organizationId,
    required double price,
    required List<MealDayDraftModel> mealDays,
    required OrganizationMealDeliveryModel delivery,
    String? promotionCode,
    int? promotionId,
    String organizationName = '',
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-order/contract',
        body: OrgMealOrderRequestBuilder.contractBody(
          organizationId: organizationId,
          price: price,
          mealDays: mealDays,
          delivery: delivery,
          promotionCode: promotionCode,
          promotionId: promotionId,
        ),
      );
      return PrepareMealDraftModel.fromJson(
        response,
        organizationName: organizationName,
      );
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tạo nháp hợp đồng: $e', statusCode: 500);
    }
  }

  Future<CheckoutMealResultModel> checkoutMealOrder({
    required String draftId,
    required int depositPercent,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-order/checkout',
        body: {
          'draftId': draftId,
          'depositPercent': depositPercent,
        },
      );
      return CheckoutMealResultModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi xác nhận đặt hàng: $e', statusCode: 500);
    }
  }

  Future<Uint8List> getOrderAnnexPreviewPdf(int orderId) async {
    try {
      return await _client.getBytes(
        '$_prefix/master-data/Order/$orderId/annex-preview',
      );
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải PDF phụ lục: $e', statusCode: 500);
    }
  }

  Future<SignOrderAnnexResultModel> signOrderAnnex({
    required int orderId,
    required String digitalSignature,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/master-data/Order/$orderId/sign-annex',
        body: {'digitalSignature': digitalSignature},
      );
      return SignOrderAnnexResultModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi ký phụ lục đơn: $e', statusCode: 500);
    }
  }

  Future<InitiateMealPaymentModel> initiateMealPayment({
    required int orderId,
    required String returnUrl,
    required String cancelUrl,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-order/pay',
        body: {
          'orderId': orderId,
          'returnUrl': returnUrl,
          'cancelUrl': cancelUrl,
        },
      );
      return InitiateMealPaymentModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tạo link thanh toán: $e', statusCode: 500);
    }
  }

  Future<ContractListModel> getContracts({
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final response = await _client.get(
        '$_prefix/company/contracts',
        queryParameters: {'page': page, 'pageSize': pageSize},
      );
      return ContractListModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải danh sách hợp đồng: $e', statusCode: 500);
    }
  }

  Future<ContractModel?> getContractDetail(int id) async {
    try {
      final response = await _client.get(
        '$_prefix/company/contracts/$id',
        queryParameters: {},
      );
      final data = response['data'] as Map<String, dynamic>?;
      if (data == null) return null;
      final contract = data['contract'] as Map<String, dynamic>?;
      if (contract != null) return ContractModel.fromJson(contract);
      return ContractModel.fromJson(data);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải chi tiết hợp đồng: $e', statusCode: 500);
    }
  }

  Future<bool> signContract({
    required int contractId,
    required String digitalSignature,
    String? signatureImageUrl,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/company/contracts/$contractId/sign',
        body: {
          'digitalSignature': digitalSignature,
          if (signatureImageUrl != null) 'signatureImageUrl': signatureImageUrl,
        },
      );
      return response['success'] as bool? ?? true;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi ký hợp đồng: $e', statusCode: 500);
    }
  }

  Future<ContractPaymentListModel> getContractPayments(int contractId) async {
    try {
      // Ưu tiên endpoint Company (role Organization); fallback Finance nếu có quyền.
      Map<String, dynamic> response;
      try {
        response = await _client.get(
          '$_prefix/company/contracts/$contractId/payments',
          queryParameters: {},
        );
      } on ApiException catch (e) {
        if (e.statusCode != 404) rethrow;
        response = await _client.get(
          '$_prefix/finance/contracts/$contractId/payments',
          queryParameters: {},
        );
      }
      return ContractPaymentListModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải lịch sử thanh toán: $e', statusCode: 500);
    }
  }

  Future<PublicReviewsPageModel> getPublicReviews({
    int page = 1,
    int pageSize = 50,
  }) async {
    try {
      final response = await _client.get(
        ApiPaths.customerReviewsPublic,
        queryParameters: {'page': page, 'pageSize': pageSize},
      );
      return PublicReviewsPageModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải đánh giá công khai: $e', statusCode: 500);
    }
  }

  Future<ReviewMeContextModel> getReviewMeContext() async {
    try {
      final response = await _client.get(ApiPaths.customerReviewsMe);
      return ReviewMeContextModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải ngữ cảnh đánh giá: $e', statusCode: 500);
    }
  }

  Future<void> createCustomerReview({
    required int orderId,
    required int rating,
    required String comment,
  }) async {
    try {
      await _client.post(
        ApiPaths.customerReviews,
        body: {
          'orderId': orderId,
          'rating': rating,
          'comment': comment.trim(),
        },
      );
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi gửi đánh giá: $e', statusCode: 500);
    }
  }

  // ────────────────────────────────────────────────────────────────────────────
  // Period-Based meal contract order (meal-contract-order)
  // ────────────────────────────────────────────────────────────────────────────

  Future<DishesByCategoryResponseModel> getMealContractMainDishes({
    int page = 1,
    int pageSize = 50,
    String? search,
  }) async {
    try {
      final response = await _client.get(
        '$_prefix/organization/meal-contract-order/dish/main',
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
          if (search != null && search.trim().isNotEmpty) 'search': search.trim(),
        },
      );
      return DishesByCategoryResponseModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải món chính: $e', statusCode: 500);
    }
  }

  Future<PrepareMealPeriodDraftModel> prepareMealPeriodContract({
    required int organizationId,
    required String startDate,
    required String endDate,
    required List<String> excludedDates,
    required int mealsPerDay,
    required double mealUnitPrice,
    required OrganizationMealDeliveryModel delivery,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-contract-order/contract',
        body: {
          'organizationId': organizationId,
          'startDate': startDate,
          'endDate': endDate,
          'excludedDates': excludedDates,
          'mealsPerDay': mealsPerDay,
          'mealUnitPrice': mealUnitPrice,
          'delivery': delivery.toJson(),
        },
      );
      return PrepareMealPeriodDraftModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tạo nháp hợp đồng theo kỳ: $e', statusCode: 500);
    }
  }

  Future<CheckoutMealResultModel> checkoutMealPeriodContract({
    required String draftId,
    required int depositPercent,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-contract-order/checkout',
        body: {
          'draftId': draftId,
          'depositPercent': depositPercent,
        },
      );
      return CheckoutMealResultModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi xác nhận hợp đồng theo kỳ: $e', statusCode: 500);
    }
  }

  Future<InitiateMealPaymentModel> initiateMealPeriodPayment({
    required int orderId,
    required String returnUrl,
    required String cancelUrl,
  }) async {
    try {
      final response = await _client.post(
        '$_prefix/organization/meal-contract-order/pay',
        body: {
          'orderId': orderId,
          'returnUrl': returnUrl,
          'cancelUrl': cancelUrl,
        },
      );
      return InitiateMealPaymentModel.fromJson(response);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tạo link thanh toán (HĐ theo kỳ): $e', statusCode: 500);
    }
  }

  Future<Map<String, dynamic>> submitMealPeriodWeeklyMeals({
    required int contractId,
    required String weekStart,
    required List<Map<String, dynamic>> mealDays,
  }) async {
    try {
      return await _client.post(
        '$_prefix/organization/meal-contract-order/$contractId/weekly-meals',
        body: {
          'weekStart': weekStart,
          'mealDays': mealDays,
        },
      );
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi gửi chọn món theo tuần: $e', statusCode: 500);
    }
  }
}
