import '../../../core/config/app_env.dart';
import '../../../core/network/api_client.dart';
import 'dart:typed_data';

import 'datasources/org_remote_datasource.dart';
import 'models/bulk_order_models.dart';
import 'models/contract_models.dart';
import '../utils/org_meal_promotion_builder.dart';

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
}
