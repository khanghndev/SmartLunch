/// Nhãn hiển thị / lọc UI (khớp tab trong `DeliveryListPage`).
String shipperDeliveryStatusLabelVi(String apiStatus) {
  switch (apiStatus.toLowerCase().trim()) {
    case 'pending':
    case 'assigned':
      return 'Chờ nhận';
    case 'received':
      return 'Chờ lấy';
    case 'in_transit':
      return 'Đang giao';
    case 'completed':
      return 'Hoàn tất';
    case 'failed':
      return 'Giao thất bại';
    case 'rejected':
      return 'Từ chối';
    default:
      return apiStatus;
  }
}

class ShipperDeliveryListItemModel {
  final int deliveryId;
  final int orderId;
  final String deliveryAddress;
  final String deliveryStatus;
  final DateTime scheduledDateUtc;
  final int mealCount;

  ShipperDeliveryListItemModel({
    required this.deliveryId,
    required this.orderId,
    required this.deliveryAddress,
    required this.deliveryStatus,
    required this.scheduledDateUtc,
    required this.mealCount,
  });

  factory ShipperDeliveryListItemModel.fromJson(Map<String, dynamic> json) {
    return ShipperDeliveryListItemModel(
      deliveryId: json['deliveryId'] as int? ?? 0,
      orderId: json['orderId'] as int? ?? 0,
      deliveryAddress: json['deliveryAddress'] as String? ?? '',
      deliveryStatus: json['deliveryStatus'] as String? ?? '',
      scheduledDateUtc: json['scheduledDateUtc'] != null
          ? DateTime.parse(json['scheduledDateUtc'] as String)
          : DateTime.now().toUtc(),
      mealCount: json['mealCount'] as int? ?? 0,
    );
  }
}

class ShipperDeliveryDetailModel {
  final int deliveryId;
  final int orderId;
  final String deliveryAddress;
  final String deliveryStatus;
  final DateTime scheduledDateUtc;
  final int mealCount;
  final DateTime? deliveredAtUtc;
  final String? proofImageUrl;
  final DateTime? proofCapturedAtUtc;
  final String? notes;
  final String? recipientConfirmedName;
  final DateTime? recipientConfirmedAtUtc;
  final String? recipientSignatureUrl;
  /// BE: đơn `in_transit` — cần chữ ký người nhận khi POST /proof.
  final bool requiresRecipientSignature;

  ShipperDeliveryDetailModel({
    required this.deliveryId,
    required this.orderId,
    required this.deliveryAddress,
    required this.deliveryStatus,
    required this.scheduledDateUtc,
    required this.mealCount,
    this.deliveredAtUtc,
    this.proofImageUrl,
    this.proofCapturedAtUtc,
    this.notes,
    this.recipientConfirmedName,
    this.recipientConfirmedAtUtc,
    this.recipientSignatureUrl,
    this.requiresRecipientSignature = false,
  });

  factory ShipperDeliveryDetailModel.fromJson(Map<String, dynamic> json) {
    return ShipperDeliveryDetailModel(
      deliveryId: json['deliveryId'] as int? ?? 0,
      orderId: json['orderId'] as int? ?? 0,
      deliveryAddress: json['deliveryAddress'] as String? ?? '',
      deliveryStatus: json['deliveryStatus'] as String? ?? '',
      scheduledDateUtc: json['scheduledDateUtc'] != null
          ? DateTime.parse(json['scheduledDateUtc'] as String)
          : DateTime.now().toUtc(),
      mealCount: json['mealCount'] as int? ?? 0,
      deliveredAtUtc: json['deliveredAtUtc'] != null
          ? DateTime.tryParse(json['deliveredAtUtc'] as String)
          : null,
      proofImageUrl: json['proofImageUrl'] as String?,
      proofCapturedAtUtc: json['proofCapturedAtUtc'] != null
          ? DateTime.tryParse(json['proofCapturedAtUtc'] as String)
          : null,
      notes: json['notes'] as String?,
      recipientConfirmedName: json['recipientConfirmedName'] as String?,
      recipientConfirmedAtUtc: json['recipientConfirmedAtUtc'] != null
          ? DateTime.tryParse(json['recipientConfirmedAtUtc'] as String)
          : null,
      recipientSignatureUrl: json['recipientSignatureUrl'] as String?,
      requiresRecipientSignature: json['requiresRecipientSignature'] as bool? ??
          json['requiresDeliveryOtp'] as bool? ??
          false,
    );
  }
}

class ShipperDeliveriesPageModel {
  final List<ShipperDeliveryListItemModel> data;
  final int totalCount;
  final int page;
  final int pageSize;

  ShipperDeliveriesPageModel({
    required this.data,
    required this.totalCount,
    required this.page,
    required this.pageSize,
  });

  factory ShipperDeliveriesPageModel.fromJson(Map<String, dynamic> json) {
    final list = json['data'] as List<dynamic>? ?? [];
    return ShipperDeliveriesPageModel(
      data: list
          .map((e) => ShipperDeliveryListItemModel.fromJson(e as Map<String, dynamic>))
          .toList(),
      totalCount: json['totalCount'] as int? ?? 0,
      page: json['page'] as int? ?? 1,
      pageSize: json['pageSize'] as int? ?? 10,
    );
  }
}

class ShipperRouteStopInput {
  final int? deliveryId;
  final String label;
  final double latitude;
  final double longitude;

  const ShipperRouteStopInput({
    this.deliveryId,
    required this.label,
    required this.latitude,
    required this.longitude,
  });

  Map<String, dynamic> toJson() => {
        if (deliveryId != null) 'deliveryId': deliveryId,
        'label': label,
        'latitude': latitude,
        'longitude': longitude,
      };
}

class ShipperRouteStopModel {
  final int sequence;
  final int? deliveryId;
  final String label;
  final double latitude;
  final double longitude;

  ShipperRouteStopModel({
    required this.sequence,
    this.deliveryId,
    required this.label,
    required this.latitude,
    required this.longitude,
  });

  factory ShipperRouteStopModel.fromJson(Map<String, dynamic> json) {
    return ShipperRouteStopModel(
      sequence: json['sequence'] as int? ?? 0,
      deliveryId: json['deliveryId'] as int?,
      label: json['label'] as String? ?? 'Điểm giao',
      latitude: (json['latitude'] as num?)?.toDouble() ?? 0,
      longitude: (json['longitude'] as num?)?.toDouble() ?? 0,
    );
  }
}

class ShipperRouteOptimizeResponseModel {
  final List<ShipperRouteStopModel> stops;
  final double approxTotalDistanceKm;

  ShipperRouteOptimizeResponseModel({
    required this.stops,
    required this.approxTotalDistanceKm,
  });

  factory ShipperRouteOptimizeResponseModel.fromJson(Map<String, dynamic> json) {
    final list = json['stops'] as List<dynamic>? ?? [];
    return ShipperRouteOptimizeResponseModel(
      stops: list
          .map((e) => ShipperRouteStopModel.fromJson(e as Map<String, dynamic>))
          .toList(),
      approxTotalDistanceKm:
          (json['approxTotalDistanceKm'] as num?)?.toDouble() ?? 0,
    );
  }
}
