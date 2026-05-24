/// Đánh giá suất ăn doanh nghiệp — `/api/v1/customer-reviews/*` (khớp web Reviews).
class PublicReviewModel {
  final int id;
  final int rating;
  final String comment;
  final DateTime? createdAt;
  final String authorName;
  final String organizationName;
  final String? orderCode;
  final String? managerReply;
  final DateTime? repliedAt;

  const PublicReviewModel({
    required this.id,
    required this.rating,
    required this.comment,
    this.createdAt,
    required this.authorName,
    required this.organizationName,
    this.orderCode,
    this.managerReply,
    this.repliedAt,
  });

  factory PublicReviewModel.fromJson(Map<String, dynamic> json) {
    return PublicReviewModel(
      id: json['id'] as int? ?? 0,
      rating: json['rating'] as int? ?? 0,
      comment: json['comment']?.toString() ?? '',
      createdAt: _parseDate(json['createdAt']),
      authorName: json['authorName']?.toString() ?? 'Khách hàng',
      organizationName: json['organizationName']?.toString() ?? '',
      orderCode: json['orderCode']?.toString(),
      managerReply: json['managerReply']?.toString(),
      repliedAt: _parseDate(json['repliedAt']),
    );
  }
}

class PublicReviewsPageModel {
  final List<PublicReviewModel> reviews;
  final double averageRating;
  final int totalCount;

  const PublicReviewsPageModel({
    this.reviews = const [],
    this.averageRating = 0,
    this.totalCount = 0,
  });

  factory PublicReviewsPageModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['reviews'] as List? ?? [];
    return PublicReviewsPageModel(
      reviews: list
          .whereType<Map<String, dynamic>>()
          .map(PublicReviewModel.fromJson)
          .toList(),
      averageRating: (data['averageRating'] as num?)?.toDouble() ?? 0,
      totalCount: data['totalCount'] as int? ?? list.length,
    );
  }
}

class ReviewableOrderModel {
  final int orderId;
  final String? orderCode;
  final String? invoiceCode;
  final DateTime? scheduledDate;
  final String status;
  final String? organizationName;
  final bool alreadyReviewed;

  const ReviewableOrderModel({
    required this.orderId,
    this.orderCode,
    this.invoiceCode,
    this.scheduledDate,
    required this.status,
    this.organizationName,
    this.alreadyReviewed = false,
  });

  /// Nhãn ngắn cho dropdown (tránh overflow trên mobile).
  String get displayLabelShort {
    final code = invoiceCode ?? orderCode ?? 'Đơn #$orderId';
    final date =
        scheduledDate != null ? formatReviewDate(scheduledDate!) : '';
    return date.isNotEmpty ? '$code · $date' : code;
  }

  /// Khớp web `Reviews.cshtml`: `Mã — Tên đơn vị (dd/MM/yyyy)`.
  String get displayLabel {
    final code = invoiceCode ?? orderCode ?? 'Đơn #$orderId';
    final org = organizationName?.trim() ?? '';
    final date =
        scheduledDate != null ? formatReviewDate(scheduledDate!) : '';
    if (org.isNotEmpty && date.isNotEmpty) {
      return '$code — $org ($date)';
    }
    if (org.isNotEmpty) return '$code — $org';
    if (date.isNotEmpty) return '$code ($date)';
    return code;
  }

  factory ReviewableOrderModel.fromJson(Map<String, dynamic> json) {
    return ReviewableOrderModel(
      orderId: json['orderId'] as int? ?? 0,
      orderCode: json['orderCode']?.toString(),
      invoiceCode: json['invoiceCode']?.toString(),
      scheduledDate: _parseDate(json['scheduledDate']),
      status: json['status']?.toString() ?? '',
      organizationName: json['organizationName']?.toString(),
      alreadyReviewed: json['alreadyReviewed'] as bool? ?? false,
    );
  }
}

class ReviewMeContextModel {
  final bool canSubmitReview;
  final bool isEnterpriseMember;
  final String? message;
  final List<ReviewableOrderModel> reviewableOrders;

  const ReviewMeContextModel({
    this.canSubmitReview = false,
    this.isEnterpriseMember = false,
    this.message,
    this.reviewableOrders = const [],
  });

  factory ReviewMeContextModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['reviewableOrders'] as List? ?? [];
    return ReviewMeContextModel(
      canSubmitReview: data['canSubmitReview'] as bool? ?? false,
      isEnterpriseMember: data['isEnterpriseMember'] as bool? ?? false,
      message: data['message']?.toString(),
      reviewableOrders: list
          .whereType<Map<String, dynamic>>()
          .map(ReviewableOrderModel.fromJson)
          .where((o) => o.orderId > 0)
          .toList(),
    );
  }
}

DateTime? _parseDate(dynamic raw) {
  if (raw == null) return null;
  if (raw is DateTime) return raw;
  return DateTime.tryParse(raw.toString());
}

String formatReviewDate(DateTime dt) {
  final d = dt.toLocal();
  final dd = d.day.toString().padLeft(2, '0');
  final mm = d.month.toString().padLeft(2, '0');
  return '$dd/$mm/${d.year}';
}
