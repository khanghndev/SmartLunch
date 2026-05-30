import 'bulk_order_models.dart';

/// Draft response cho hợp đồng theo kỳ (Period-Based) — `/organization/meal-contract-order/contract`.
class PrepareMealPeriodDraftModel {
  final String draftId;
  final int contractId;
  final String? contractNumber;
  final String? contractFileUrl;
  final String startDate;
  final String endDate;
  final List<String> excludedDates;
  final int serviceDays;
  final int mealsPerDay;
  final double mealUnitPrice;
  final double totalAmount;
  final OrganizationMealDeliveryModel? delivery;

  const PrepareMealPeriodDraftModel({
    required this.draftId,
    required this.contractId,
    this.contractNumber,
    this.contractFileUrl,
    required this.startDate,
    required this.endDate,
    this.excludedDates = const [],
    this.serviceDays = 0,
    this.mealsPerDay = 0,
    this.mealUnitPrice = 0,
    this.totalAmount = 0,
    this.delivery,
  });

  factory PrepareMealPeriodDraftModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final excluded = (data['excludedDates'] as List? ?? [])
        .map((e) => e?.toString() ?? '')
        .where((s) => s.isNotEmpty)
        .toList();
    final delivery = data['delivery'] as Map<String, dynamic>?;
    return PrepareMealPeriodDraftModel(
      draftId: data['draftId']?.toString() ?? '',
      contractId: data['contractId'] as int? ?? 0,
      contractNumber: data['contractNumber']?.toString(),
      contractFileUrl: data['contractFileUrl']?.toString(),
      startDate: data['startDate']?.toString() ?? '',
      endDate: data['endDate']?.toString() ?? '',
      excludedDates: excluded,
      serviceDays: data['serviceDays'] as int? ?? 0,
      mealsPerDay: data['mealsPerDay'] as int? ?? 0,
      mealUnitPrice: (data['mealUnitPrice'] as num?)?.toDouble() ?? 0,
      totalAmount: (data['totalAmount'] as num?)?.toDouble() ?? 0,
      delivery: delivery == null
          ? null
          : OrganizationMealDeliveryModel(
              recipientName: delivery['recipientName']?.toString() ?? '',
              recipientPhone: delivery['recipientPhone']?.toString() ?? '',
              recipientEmail: delivery['recipientEmail']?.toString() ?? '',
              deliveryAddress: delivery['deliveryAddress']?.toString() ?? '',
              deliveryWardDistrict: delivery['deliveryWardDistrict']?.toString(),
              deliveryNotes: delivery['deliveryNotes']?.toString(),
              preferredDeliveryTime: delivery['preferredDeliveryTime']?.toString(),
            ),
    );
  }
}

