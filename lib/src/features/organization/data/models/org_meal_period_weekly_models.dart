class SubmitWeeklyMealsResultModel {
  final int contractId;
  final int orderId;
  final String weekStart;
  final int itemCount;

  const SubmitWeeklyMealsResultModel({
    required this.contractId,
    required this.orderId,
    required this.weekStart,
    required this.itemCount,
  });

  factory SubmitWeeklyMealsResultModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    return SubmitWeeklyMealsResultModel(
      contractId: data['contractId'] as int? ?? 0,
      orderId: data['orderId'] as int? ?? 0,
      weekStart: data['weekStart']?.toString() ?? '',
      itemCount: data['itemCount'] as int? ?? 0,
    );
  }
}

