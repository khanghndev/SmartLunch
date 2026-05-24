import '../../utils/org_meal_order_date_rules.dart';

/// Models đặt suất Organization — khớp BE `/api/v1/organization/meal-order/...`.

/// Thông tin giao hàng — `OrganizationMealDeliveryRequest` (BE).
class OrganizationMealDeliveryModel {
  final String recipientName;
  final String recipientPhone;
  final String recipientEmail;
  final String deliveryAddress;
  final String? deliveryWardDistrict;
  final String? deliveryNotes;
  final String? preferredDeliveryTime;

  const OrganizationMealDeliveryModel({
    required this.recipientName,
    required this.recipientPhone,
    required this.recipientEmail,
    required this.deliveryAddress,
    this.deliveryWardDistrict,
    this.deliveryNotes,
    this.preferredDeliveryTime,
  });

  Map<String, dynamic> toJson() => {
        'recipientName': recipientName.trim(),
        'recipientPhone': recipientPhone.trim(),
        'recipientEmail': recipientEmail.trim(),
        'deliveryAddress': deliveryAddress.trim(),
        if (deliveryWardDistrict != null && deliveryWardDistrict!.trim().isNotEmpty)
          'deliveryWardDistrict': deliveryWardDistrict!.trim(),
        if (deliveryNotes != null && deliveryNotes!.trim().isNotEmpty)
          'deliveryNotes': deliveryNotes!.trim(),
        if (preferredDeliveryTime != null &&
            preferredDeliveryTime!.trim().isNotEmpty)
          'preferredDeliveryTime': preferredDeliveryTime!.trim(),
      };

  /// Kiểm tra trước khi gọi API — thông báo tiếng Việt.
  static String? validate({
    required String recipientName,
    required String recipientPhone,
    required String recipientEmail,
    required String deliveryAddress,
    String? preferredDeliveryTime,
  }) {
    final name = recipientName.trim();
    if (name.length < 2) {
      return 'Tên người nhận phải có ít nhất 2 ký tự.';
    }

    final phone = _normalizePhone(recipientPhone);
    if (!RegExp(r'^(\+?84|0)[0-9]{8,10}$').hasMatch(phone)) {
      return 'Số điện thoại người nhận không hợp lệ (VD: 0901234567).';
    }

    final email = recipientEmail.trim();
    if (!RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$').hasMatch(email)) {
      return 'Email người nhận không hợp lệ.';
    }

    if (deliveryAddress.trim().length < 10) {
      return 'Địa chỉ giao phải có ít nhất 10 ký tự.';
    }

    final time = preferredDeliveryTime?.trim();
    if (time != null && time.isNotEmpty) {
      if (!RegExp(r'^([01]?\d|2[0-3]):[0-5]\d$').hasMatch(time)) {
        return 'Giờ giao mong muốn phải theo định dạng HH:mm (VD: 11:30).';
      }
    }

    return null;
  }

  static String _normalizePhone(String raw) {
    var p = raw.trim().replaceAll(' ', '').replaceAll('-', '');
    if (p.startsWith('+84')) {
      p = '0${p.substring(3)}';
    }
    return p;
  }
}

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

class EligiblePromotionModel {
  final int promotionId;
  final String promotionCode;
  final String promotionName;
  final String? description;
  final double discountAmount;
  final double totalAfter;
  final bool isRecommended;

  const EligiblePromotionModel({
    required this.promotionId,
    this.promotionCode = '',
    this.promotionName = '',
    this.description,
    this.discountAmount = 0,
    this.totalAfter = 0,
    this.isRecommended = false,
  });

  factory EligiblePromotionModel.fromJson(Map<String, dynamic> json) {
    return EligiblePromotionModel(
      promotionId: json['promotionId'] is int
          ? json['promotionId'] as int
          : int.tryParse('${json['promotionId']}') ?? 0,
      promotionCode: json['promotionCode']?.toString() ?? '',
      promotionName: json['promotionName']?.toString() ?? '',
      description: json['description']?.toString(),
      discountAmount: _num(json['discountAmount']),
      totalAfter: _num(json['totalAfter']),
      isRecommended: json['isRecommended'] as bool? ?? false,
    );
  }
}

class ListEligiblePromotionsModel {
  final double subtotal;
  final List<EligiblePromotionModel> items;
  final String? message;

  const ListEligiblePromotionsModel({
    this.subtotal = 0,
    this.items = const [],
    this.message,
  });

  factory ListEligiblePromotionsModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['items'] as List? ?? [];
    return ListEligiblePromotionsModel(
      subtotal: _num(data['subtotal']),
      message: data['message']?.toString(),
      items: list
          .whereType<Map<String, dynamic>>()
          .map(EligiblePromotionModel.fromJson)
          .toList(),
    );
  }
}

class PreviewPromotionModel {
  final double subtotal;
  final double discountAmount;
  final double totalAfter;
  final bool applied;
  final int? promotionId;
  final String? promotionCode;
  final String? promotionName;
  final String? message;

  const PreviewPromotionModel({
    this.subtotal = 0,
    this.discountAmount = 0,
    this.totalAfter = 0,
    this.applied = false,
    this.promotionId,
    this.promotionCode,
    this.promotionName,
    this.message,
  });

  factory PreviewPromotionModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    return PreviewPromotionModel(
      subtotal: _num(data['subtotal']),
      discountAmount: _num(data['discountAmount']),
      totalAfter: _num(data['totalAfter']),
      applied: data['applied'] as bool? ?? false,
      promotionId: data['promotionId'] as int?,
      promotionCode: data['promotionCode']?.toString(),
      promotionName: data['promotionName']?.toString(),
      message: data['message']?.toString(),
    );
  }
}

/// Tóm tắt giao hàng từ nháp — `OrganizationMealDeliverySummaryDto` (BE).
class OrganizationMealDeliverySummaryModel {
  final String recipientName;
  final String recipientPhone;
  final String recipientEmail;
  final String deliveryAddress;
  final String? deliveryWardDistrict;
  final String? deliveryNotes;
  final String? preferredDeliveryTime;
  final String fullAddress;

  const OrganizationMealDeliverySummaryModel({
    this.recipientName = '',
    this.recipientPhone = '',
    this.recipientEmail = '',
    this.deliveryAddress = '',
    this.deliveryWardDistrict,
    this.deliveryNotes,
    this.preferredDeliveryTime,
    this.fullAddress = '',
  });

  factory OrganizationMealDeliverySummaryModel.fromJson(
    Map<String, dynamic> json,
  ) {
    return OrganizationMealDeliverySummaryModel(
      recipientName: json['recipientName']?.toString() ?? '',
      recipientPhone: json['recipientPhone']?.toString() ?? '',
      recipientEmail: json['recipientEmail']?.toString() ?? '',
      deliveryAddress: json['deliveryAddress']?.toString() ?? '',
      deliveryWardDistrict: json['deliveryWardDistrict']?.toString(),
      deliveryNotes: json['deliveryNotes']?.toString(),
      preferredDeliveryTime: json['preferredDeliveryTime']?.toString(),
      fullAddress: json['fullAddress']?.toString() ?? '',
    );
  }

  String get displayAddress {
    if (fullAddress.trim().isNotEmpty) return fullAddress.trim();
    final ward = deliveryWardDistrict?.trim();
    if (ward != null && ward.isNotEmpty) {
      return '$deliveryAddress, $ward';
    }
    return deliveryAddress;
  }
}

class PrepareMealDraftModel {
  final String draftId;
  final int contractId;
  final String? contractNumber;
  final String? contractFileUrl;
  final double pricePerPortion;
  final int totalMainQuantity;
  final double totalAmount;
  final double? subtotalAmount;
  final double discountAmount;
  final int? appliedPromotionId;
  final String? appliedPromotionName;
  final String? promotionCode;
  final String? persistenceNotice;
  final OrganizationMealDeliverySummaryModel? delivery;
  final List<MealDraftLineSummaryModel> lines;
  final String organizationName;

  const PrepareMealDraftModel({
    required this.draftId,
    this.contractId = 0,
    this.contractNumber,
    this.contractFileUrl,
    required this.pricePerPortion,
    required this.totalMainQuantity,
    required this.totalAmount,
    this.subtotalAmount,
    this.discountAmount = 0,
    this.appliedPromotionId,
    this.appliedPromotionName,
    this.promotionCode,
    this.persistenceNotice,
    this.delivery,
    this.lines = const [],
    this.organizationName = '',
  });

  factory PrepareMealDraftModel.fromJson(
    Map<String, dynamic> json, {
    String organizationName = '',
  }) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final rawLines = data['lines'] as List? ?? [];
    final deliveryRaw = data['delivery'];
    return PrepareMealDraftModel(
      draftId: data['draftId']?.toString() ?? '',
      contractId: data['contractId'] is int
          ? data['contractId'] as int
          : int.tryParse('${data['contractId']}') ?? 0,
      contractNumber: data['contractNumber']?.toString(),
      contractFileUrl: data['contractFileUrl']?.toString(),
      pricePerPortion: _num(data['pricePerPortion']),
      totalMainQuantity: data['totalMainQuantity'] as int? ?? 0,
      totalAmount: _num(data['totalAmount']),
      subtotalAmount: data['subtotalAmount'] != null
          ? _num(data['subtotalAmount'])
          : null,
      discountAmount: _num(data['discountAmount']),
      appliedPromotionId: data['appliedPromotionId'] as int?,
      appliedPromotionName: data['appliedPromotionName']?.toString(),
      promotionCode: data['promotionCode']?.toString(),
      persistenceNotice: data['persistenceNotice']?.toString(),
      delivery: deliveryRaw is Map<String, dynamic>
          ? OrganizationMealDeliverySummaryModel.fromJson(deliveryRaw)
          : null,
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

class SignOrderAnnexResultModel {
  final int orderId;
  final String? annexPdfUrl;

  const SignOrderAnnexResultModel({
    required this.orderId,
    this.annexPdfUrl,
  });

  factory SignOrderAnnexResultModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final order = data['order'] as Map<String, dynamic>? ?? data;
    return SignOrderAnnexResultModel(
      orderId: order['id'] as int? ?? order['orderId'] as int? ?? 0,
      annexPdfUrl: order['annexPdfUrl']?.toString(),
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

  bool get hasCheckoutUrl =>
      checkoutUrl != null && checkoutUrl!.trim().isNotEmpty;

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
