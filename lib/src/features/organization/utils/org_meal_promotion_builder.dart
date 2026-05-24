import '../data/models/bulk_order_models.dart';

/// Khớp web `OrganizationMealPromotionPreviewBuilder.cs` — preview KM kênh b2b_org.
class OrgMealPromotionBuilder {
  static Map<String, dynamic> toPreviewRequest({
    required int organizationId,
    required double price,
    required List<MealDayDraftModel> mealDays,
    String? promotionCode,
    int? promotionId,
  }) {
    final lines = <Map<String, dynamic>>[];
    var totalQty = 0;

    for (final day in mealDays) {
      for (final line in day.linesFor('main')) {
        final qty = line.quantity < 1 ? 1 : line.quantity;
        totalQty += qty;
        lines.add({
          'dishId': line.dishId,
          'quantity': qty,
          'lineTotal': _roundMoney(price * qty),
        });
      }
    }

    return {
      'channel': 'b2b_org',
      'organizationId': organizationId,
      if (promotionCode != null && promotionCode.isNotEmpty)
        'promotionCode': promotionCode,
      if (promotionId != null) 'promotionId': promotionId,
      'subtotal': _roundMoney(price * totalQty),
      'totalQuantity': totalQty,
      'lines': lines,
    };
  }

  static double _roundMoney(double value) =>
      (value * 100).roundToDouble() / 100;
}
