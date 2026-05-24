import '../data/models/bulk_order_models.dart';

/// Body `POST /organization/meal-order/contract` — khớp BE `PrepareOrganizationMealContractRequest`.
abstract final class OrgMealOrderRequestBuilder {
  static Map<String, dynamic> contractBody({
    required int organizationId,
    required double price,
    required List<MealDayDraftModel> mealDays,
    required OrganizationMealDeliveryModel delivery,
    String? promotionCode,
    int? promotionId,
  }) {
    return {
      'organizationId': organizationId,
      'price': price,
      if (promotionCode != null && promotionCode.isNotEmpty)
        'promotionCode': promotionCode,
      if (promotionId != null) 'promotionId': promotionId,
      'delivery': delivery.toJson(),
      'mealDays': mealDays.map((d) {
        return {
          'serviceDate': d.serviceDate,
          'mealPlan': {
            'main': d.linesFor('main').map((l) => l.toJson()).toList(),
            'side': d.linesFor('side').map((l) => l.toJson()).toList(),
            'soup': d.linesFor('soup').map((l) => l.toJson()).toList(),
          },
        };
      }).toList(),
    };
  }
}
