import '../../utils/org_meal_order_date_rules.dart';

/// Models đặt suất Organization — khớp BE `/api/v1/organization/meal-order/...`.

class DishCategoriesResponseModel {
  final List<DishCategoryModel> categories;
  final DateTime? allowedFirstServiceDate;
  final DateTime? allowedLastServiceDate;

  const DishCategoriesResponseModel({
    this.categories = const [],
    this.allowedFirstServiceDate,
    this.allowedLastServiceDate,
  });

  factory DishCategoriesResponseModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['categories'] as List? ?? [];
    return DishCategoriesResponseModel(
      categories: list
          .whereType<Map<String, dynamic>>()
          .map(DishCategoryModel.fromJson)
          .toList(),
      allowedFirstServiceDate:
          OrgMealOrderDateRules.parseApiDate(data['allowedFirstServiceDate']),
      allowedLastServiceDate:
          OrgMealOrderDateRules.parseApiDate(data['allowedLastServiceDate']),
    );
  }
}

class DishCategoryModel {
  final int id;
  final String name;
  final String slotKey;
  final int sortOrder;

  const DishCategoryModel({
    required this.id,
    required this.name,
    this.slotKey = '',
    this.sortOrder = 0,
  });

  factory DishCategoryModel.fromJson(Map<String, dynamic> json) {
    return DishCategoryModel(
      id: json['id'] is int ? json['id'] as int : int.tryParse('${json['id']}') ?? 0,
      name: json['name']?.toString() ?? '',
      slotKey: json['slotKey']?.toString() ?? '',
      sortOrder: json['sortOrder'] is int
          ? json['sortOrder'] as int
          : int.tryParse('${json['sortOrder']}') ?? 0,
    );
  }
}

class OrganizationDishModel {
  final int id;
  final String name;
  final double price;
  final String? imageUrl;

  const OrganizationDishModel({
    required this.id,
    required this.name,
    this.price = 0,
    this.imageUrl,
  });

  factory OrganizationDishModel.fromJson(Map<String, dynamic> json) {
    return OrganizationDishModel(
      id: json['id'] is int ? json['id'] as int : int.tryParse('${json['id']}') ?? 0,
      name: json['name']?.toString() ?? '',
      price: (json['price'] as num?)?.toDouble() ?? 0,
      imageUrl: json['imageUrl']?.toString(),
    );
  }
}

class DishesByCategoryResponseModel {
  final List<OrganizationDishModel> dishes;
  final int totalCount;

  const DishesByCategoryResponseModel({
    this.dishes = const [],
    this.totalCount = 0,
  });

  factory DishesByCategoryResponseModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['dishes'] as List? ?? [];
    return DishesByCategoryResponseModel(
      dishes: list
          .whereType<Map<String, dynamic>>()
          .map(OrganizationDishModel.fromJson)
          .toList(),
      totalCount: data['totalCount'] as int? ?? list.length,
    );
  }
}

class MealLineDraftModel {
  final int dishId;
  final String dishName;
  int quantity;

  MealLineDraftModel({
    required this.dishId,
    required this.dishName,
    this.quantity = 1,
  });

  Map<String, dynamic> toJson() => {'dishId': dishId, 'quantity': quantity};
}

class MealDayDraftModel {
  final String serviceDate;
  final Map<String, List<MealLineDraftModel>> mealPlan;

  MealDayDraftModel({
    required this.serviceDate,
    Map<String, List<MealLineDraftModel>>? mealPlan,
  }) : mealPlan = mealPlan ??
            {
              'main': [],
              'side': [],
              'soup': [],
            };

  List<MealLineDraftModel> linesFor(String slot) => mealPlan[slot] ?? [];

  int slotTotal(String slot) =>
      linesFor(slot).fold(0, (s, l) => s + (l.quantity < 1 ? 1 : l.quantity));
}

class PrepareMealDraftModel {
  final String draftId;
  final double pricePerPortion;
  final int totalMainQuantity;
  final double totalAmount;
  final double discountAmount;
  final String? appliedPromotionName;
  final String? persistenceNotice;
  final List<MealDraftLineSummaryModel> lines;
  final String organizationName;

  const PrepareMealDraftModel({
    required this.draftId,
    required this.pricePerPortion,
    required this.totalMainQuantity,
    required this.totalAmount,
    this.discountAmount = 0,
    this.appliedPromotionName,
    this.persistenceNotice,
    this.lines = const [],
    this.organizationName = '',
  });

  factory PrepareMealDraftModel.fromJson(
    Map<String, dynamic> json, {
    String organizationName = '',
  }) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final rawLines = data['lines'] as List? ?? [];
    return PrepareMealDraftModel(
      draftId: data['draftId']?.toString() ?? '',
      pricePerPortion: _num(data['pricePerPortion']),
      totalMainQuantity: data['totalMainQuantity'] as int? ?? 0,
      totalAmount: _num(data['totalAmount']),
      discountAmount: _num(data['discountAmount']),
      appliedPromotionName: data['appliedPromotionName']?.toString(),
      persistenceNotice: data['persistenceNotice']?.toString(),
      organizationName: organizationName,
      lines: rawLines
          .whereType<Map<String, dynamic>>()
          .map(MealDraftLineSummaryModel.fromJson)
          .toList(),
    );
  }
}

class MealDraftLineSummaryModel {
  final String serviceDate;
  final String slot;
  final String dishName;
  final int quantity;

  const MealDraftLineSummaryModel({
    required this.serviceDate,
    required this.slot,
    required this.dishName,
    required this.quantity,
  });

  factory MealDraftLineSummaryModel.fromJson(Map<String, dynamic> json) {
    return MealDraftLineSummaryModel(
      serviceDate: json['serviceDate']?.toString() ?? '',
      slot: json['slot']?.toString() ?? '',
      dishName: json['dishName']?.toString() ?? '',
      quantity: json['quantity'] as int? ?? 0,
    );
  }

  String get slotLabel {
    switch (slot.toLowerCase()) {
      case 'main':
        return 'Món chính';
      case 'side':
        return 'Món phụ';
      case 'soup':
        return 'Canh';
      default:
        return slot;
    }
  }
}

class CheckoutMealResultModel {
  final int orderId;
  final int depositPercent;
  final int depositAmountVnd;
  final String? checkoutUrl;
  final String? payOsMessage;

  const CheckoutMealResultModel({
    required this.orderId,
    required this.depositPercent,
    required this.depositAmountVnd,
    this.checkoutUrl,
    this.payOsMessage,
  });

  factory CheckoutMealResultModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final orderWrap = data['order'] as Map<String, dynamic>?;
    final order = orderWrap?['order'] as Map<String, dynamic>? ?? orderWrap;
    final orderId = order?['id'] as int? ?? order?['orderId'] as int? ?? 0;
    return CheckoutMealResultModel(
      orderId: orderId,
      depositPercent: data['depositPercent'] as int? ?? 0,
      depositAmountVnd: data['depositAmountVnd'] as int? ?? 0,
      checkoutUrl: data['checkoutUrl']?.toString(),
      payOsMessage: data['payOsMessage']?.toString(),
    );
  }
}

class InitiateMealPaymentModel {
  final int orderId;
  final int depositAmountVnd;
  final String? checkoutUrl;
  final String? payOsMessage;

  const InitiateMealPaymentModel({
    required this.orderId,
    required this.depositAmountVnd,
    this.checkoutUrl,
    this.payOsMessage,
  });

  factory InitiateMealPaymentModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    return InitiateMealPaymentModel(
      orderId: data['orderId'] as int? ?? 0,
      depositAmountVnd: data['depositAmountVnd'] as int? ?? 0,
      checkoutUrl: data['checkoutUrl']?.toString(),
      payOsMessage: data['payOsMessage']?.toString(),
    );
  }
}

DateTime? _parseDate(dynamic raw) {
  if (raw == null) return null;
  if (raw is String && raw.length >= 10) {
    return DateTime.tryParse(raw.substring(0, 10));
  }
  return null;
}

double _num(dynamic v) {
  if (v is num) return v.toDouble();
  return double.tryParse('$v') ?? 0;
}
