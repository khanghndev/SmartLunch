import 'org_meal_period_contract_date_rules.dart';

/// Khớp web `OrganizationMealPeriodPromotionPreviewBuilder.cs`.
class OrgMealPeriodPromotionBuilder {
  static Map<String, dynamic> toPreviewRequest({
    required int organizationId,
    required String startDate,
    required String endDate,
    required List<String> excludedDates,
    required int mealsPerDay,
    required double mealUnitPrice,
    Map<String, int>? dailyMealOverrides,
    String? promotionCode,
    int? promotionId,
  }) {
    final start = DateTime.tryParse(startDate);
    final end = DateTime.tryParse(endDate);
    if (start == null || end == null) {
      return {
        'channel': 'b2b_org',
        'organizationId': organizationId,
      };
    }

    final meals = mealsPerDay < 1 ? 1 : mealsPerDay;
    final unitPrice = double.parse(mealUnitPrice.toStringAsFixed(2));
    final totalQty = OrgMealPeriodContractDateRules.countTotalMeals(
      start: start,
      end: end,
      excludedIso: excludedDates,
      defaultMealsPerDay: meals,
      dailyOverridesByIso: dailyMealOverrides,
    );
    final subtotal =
        double.parse((totalQty * unitPrice).toStringAsFixed(2));

    return {
      'channel': 'b2b_org',
      'organizationId': organizationId,
      if (promotionCode != null && promotionCode.isNotEmpty)
        'promotionCode': promotionCode,
      if (promotionId != null) 'promotionId': promotionId,
      'subtotal': subtotal,
      'totalQuantity': totalQty,
      'lines': <Map<String, dynamic>>[],
    };
  }
}
