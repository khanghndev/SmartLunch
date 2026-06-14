import '../../../core/config/app_env.dart';
import '../../../core/network/api_client.dart';
import 'dart:typed_data';

import 'datasources/org_remote_datasource.dart';
import 'models/bulk_order_models.dart';
import 'models/contract_models.dart';
import 'models/customer_review_models.dart';
import 'models/org_meal_contract_models.dart';
import 'models/org_meal_period_weekly_models.dart';
import '../utils/org_meal_period_promotion_builder.dart';
import '../utils/org_meal_promotion_builder.dart';
import 'models/org_order_models.dart';

class OrgRepository {
  OrgRepository._({OrgRemoteDataSource? remote})
      : _remote =
            remote ?? OrgRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl));

  static final OrgRepository instance = OrgRepository._();

  final OrgRemoteDataSource _remote;

  Future<DishCategoriesResponseModel> getDishCategories() =>
      _remote.getDishCategories();

  Future<DishesByCategoryResponseModel> getDishesByCategory(
    int categoryId, {
    int page = 1,
    int pageSize = 50,
  }) =>
      _remote.getDishesByCategory(categoryId, page: page, pageSize: pageSize);

  Future<ListEligiblePromotionsModel> listEligibleMealPromotions({
    required int organizationId,
    required double price,
    required List<MealDayDraftModel> mealDays,
  }) {
    final body = OrgMealPromotionBuilder.toPreviewRequest(
      organizationId: organizationId,
      price: price,
      mealDays: mealDays,
    );
    return _remote.listEligiblePromotions(body);
  }

  Future<PreviewPromotionModel> previewMealPromotion({
    required int organizationId,
    required double price,
    required List<MealDayDraftModel> mealDays,
    required String promotionCode,
  }) {
    final body = OrgMealPromotionBuilder.toPreviewRequest(
      organizationId: organizationId,
      price: price,
      mealDays: mealDays,
      promotionCode: promotionCode,
    );
    return _remote.previewPromotion(body);
  }

  Future<PrepareMealDraftModel> prepareMealContract({
    required int organizationId,
    required double price,
    required List<MealDayDraftModel> mealDays,
    required OrganizationMealDeliveryModel delivery,
    String? promotionCode,
    int? promotionId,
    String organizationName = '',
  }) =>
      _remote.prepareMealContract(
        organizationId: organizationId,
        price: price,
        mealDays: mealDays,
        delivery: delivery,
        promotionCode: promotionCode,
        promotionId: promotionId,
        organizationName: organizationName,
      );

  Future<CheckoutMealResultModel> checkoutMealOrder({
    required String draftId,
    required int depositPercent,
  }) =>
      _remote.checkoutMealOrder(
        draftId: draftId,
        depositPercent: depositPercent,
      );

  Future<Uint8List> getOrderAnnexPreviewPdf(int orderId) =>
      _remote.getOrderAnnexPreviewPdf(orderId);

  Future<SignOrderAnnexResultModel> signOrderAnnex({
    required int orderId,
    required String digitalSignature,
  }) =>
      _remote.signOrderAnnex(
        orderId: orderId,
        digitalSignature: digitalSignature,
      );

  Future<InitiateMealPaymentModel> initiateMealPayment({
    required int orderId,
    required String returnUrl,
    required String cancelUrl,
  }) =>
      _remote.initiateMealPayment(
        orderId: orderId,
        returnUrl: returnUrl,
        cancelUrl: cancelUrl,
      );

  Future<ContractListModel> getContracts({int page = 1, int pageSize = 20}) =>
      _remote.getContracts(page: page, pageSize: pageSize);

  Future<ContractModel?> getContractDetail(int id) => _remote.getContractDetail(id);

  Future<bool> signContract({
    required int contractId,
    required String digitalSignature,
    String? signatureImageUrl,
  }) =>
      _remote.signContract(
        contractId: contractId,
        digitalSignature: digitalSignature,
        signatureImageUrl: signatureImageUrl,
      );

  Future<ContractPaymentListModel> getContractPayments(int contractId) =>
      _remote.getContractPayments(contractId);

  // ────────────────────────────────────────────────────────────────────────────
  // Period-Based meal contract order
  // ────────────────────────────────────────────────────────────────────────────

  Future<List<MealPortionPriceOptionModel>> getMealPortionPrices() =>
      _remote.getMealPortionPrices();

  Future<ListEligiblePromotionsModel> listEligiblePeriodPromotions({
    required int organizationId,
    required String startDate,
    required String endDate,
    required List<String> excludedDates,
    required int mealsPerDay,
    required double mealUnitPrice,
    Map<String, int>? dailyMealOverrides,
  }) {
    final body = OrgMealPeriodPromotionBuilder.toPreviewRequest(
      organizationId: organizationId,
      startDate: startDate,
      endDate: endDate,
      excludedDates: excludedDates,
      mealsPerDay: mealsPerDay,
      mealUnitPrice: mealUnitPrice,
      dailyMealOverrides: dailyMealOverrides,
    );
    return _remote.listEligiblePromotions(body);
  }

  Future<PreviewPromotionModel> previewPeriodPromotion({
    required int organizationId,
    required String startDate,
    required String endDate,
    required List<String> excludedDates,
    required int mealsPerDay,
    required double mealUnitPrice,
    required String promotionCode,
    Map<String, int>? dailyMealOverrides,
  }) {
    final body = OrgMealPeriodPromotionBuilder.toPreviewRequest(
      organizationId: organizationId,
      startDate: startDate,
      endDate: endDate,
      excludedDates: excludedDates,
      mealsPerDay: mealsPerDay,
      mealUnitPrice: mealUnitPrice,
      dailyMealOverrides: dailyMealOverrides,
      promotionCode: promotionCode,
    );
    return _remote.previewPromotion(body);
  }

  Future<DishesByCategoryResponseModel> getMealContractMainDishes({
    int page = 1,
    int pageSize = 50,
    String? search,
  }) =>
      _remote.getMealContractMainDishes(page: page, pageSize: pageSize, search: search);

  Future<PrepareMealPeriodDraftModel> prepareMealPeriodContract({
    required int organizationId,
    required String startDate,
    required String endDate,
    required List<String> excludedDates,
    required int mealsPerDay,
    required double mealUnitPrice,
    required OrganizationMealDeliveryModel delivery,
    List<Map<String, dynamic>> dailyMealPortions = const [],
    int? dishValueId,
    String? promotionCode,
    int? promotionId,
  }) =>
      _remote.prepareMealPeriodContract(
        organizationId: organizationId,
        startDate: startDate,
        endDate: endDate,
        excludedDates: excludedDates,
        mealsPerDay: mealsPerDay,
        mealUnitPrice: mealUnitPrice,
        delivery: delivery,
        dailyMealPortions: dailyMealPortions,
        dishValueId: dishValueId,
        promotionCode: promotionCode,
        promotionId: promotionId,
      );

  Future<CheckoutMealResultModel> checkoutMealPeriodContract({
    required String draftId,
    required int depositPercent,
  }) =>
      _remote.checkoutMealPeriodContract(
        draftId: draftId,
        depositPercent: depositPercent,
      );

  Future<InitiateMealPaymentModel> initiateMealPeriodPayment({
    required int orderId,
    required String returnUrl,
    required String cancelUrl,
  }) =>
      _remote.initiateMealPeriodPayment(
        orderId: orderId,
        returnUrl: returnUrl,
        cancelUrl: cancelUrl,
      );

  Future<SubmitWeeklyMealsResultModel> submitMealPeriodWeeklyMeals({
    required int contractId,
    required String weekStart,
    required List<Map<String, dynamic>> mealDays,
  }) async {
    final res = await _remote.submitMealPeriodWeeklyMeals(
      contractId: contractId,
      weekStart: weekStart,
      mealDays: mealDays,
    );
    return SubmitWeeklyMealsResultModel.fromJson(res);
  }

  Future<PublicReviewsPageModel> getPublicReviews({
    int page = 1,
    int pageSize = 50,
  }) =>
      _remote.getPublicReviews(page: page, pageSize: pageSize);

  Future<ReviewMeContextModel> getReviewMeContext() =>
      _remote.getReviewMeContext();

  Future<void> submitCustomerReview({
    required int orderId,
    required int rating,
    required String comment,
  }) =>
      _remote.createCustomerReview(
        orderId: orderId,
        rating: rating,
        comment: comment,
      );

  Future<OrgOrderListModel> getOrders({
    int page = 1,
    int pageSize = 20,
    String? status,
    String? paymentStatus,
    String? search,
  }) =>
      _remote.getOrders(
        page: page,
        pageSize: pageSize,
        status: status,
        paymentStatus: paymentStatus,
        search: search,
      );
}
