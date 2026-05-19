/// Models cho Review và Complaint.
/// Khớp endpoint `/api/v1/master-data/Review` và `/api/v1/master-data/Complaint`.

// ─── Review ───────────────────────────────────────────────────────────────────

class ReviewModel {
  final int id;
  final String customerName;
  final String dishName;
  final int stars;          // 1–5
  final String comment;
  final String createdAt;

  const ReviewModel({
    required this.id,
    required this.customerName,
    required this.dishName,
    required this.stars,
    required this.comment,
    required this.createdAt,
  });

  factory ReviewModel.fromJson(Map<String, dynamic> json) {
    final userId = json['userId'] as int?;
    return ReviewModel(
      id: json['id'] as int? ?? 0,
      customerName: json['customerName']?.toString() ??
          json['userName']?.toString() ??
          (userId != null ? 'Khách #$userId' : 'Ẩn danh'),
      dishName: json['dishName']?.toString() ??
          json['mealName']?.toString() ??
          (json['dishId'] != null ? 'Món #${json['dishId']}' : ''),
      stars: json['stars'] as int? ?? json['rating'] as int? ?? 0,
      comment: json['comment']?.toString() ?? json['content']?.toString() ?? '',
      createdAt: json['createdAt']?.toString() ?? json['date']?.toString() ?? '',
    );
  }
}

class ReviewPageModel {
  final List<ReviewModel> items;
  final int totalCount;
  final int page;
  final int pageSize;
  final double averageRating;

  const ReviewPageModel({
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
    this.averageRating = 0,
  });

  factory ReviewPageModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['data'] as List? ?? data['items'] as List? ?? [];
    final items = list
        .whereType<Map<String, dynamic>>()
        .map(ReviewModel.fromJson)
        .toList();
    final avg = (data['averageRating'] as num?)?.toDouble() ??
        (items.isEmpty
            ? 0.0
            : items.map((r) => r.stars).fold(0, (a, b) => a + b) /
                items.length);
    return ReviewPageModel(
      items: items,
      totalCount: data['totalCount'] as int? ?? items.length,
      page: data['page'] as int? ?? 1,
      pageSize: data['pageSize'] as int? ?? 20,
      averageRating: avg.toDouble(),
    );
  }
}

// ─── Complaint ────────────────────────────────────────────────────────────────

enum ComplaintStatus { open, processing, resolved }

extension ComplaintStatusX on ComplaintStatus {
  String get label {
    switch (this) {
      case ComplaintStatus.open: return 'Mới';
      case ComplaintStatus.processing: return 'Đang xử lý';
      case ComplaintStatus.resolved: return 'Đã giải quyết';
    }
  }

  static ComplaintStatus fromString(String? s) {
    switch (s?.toLowerCase()) {
      case 'processing': return ComplaintStatus.processing;
      case 'resolved': return ComplaintStatus.resolved;
      default: return ComplaintStatus.open;
    }
  }
}

class ComplaintModel {
  final int id;
  final String title;
  final String description;
  final String organizationName;
  final String createdAt;
  final ComplaintStatus status;

  const ComplaintModel({
    required this.id,
    required this.title,
    required this.description,
    required this.organizationName,
    required this.createdAt,
    required this.status,
  });

  factory ComplaintModel.fromJson(Map<String, dynamic> json) => ComplaintModel(
        id: json['id'] as int? ?? 0,
        title: json['title']?.toString() ?? json['subject']?.toString() ?? '',
        description: json['description']?.toString() ??
            json['content']?.toString() ?? '',
        organizationName:
            json['organizationName']?.toString() ?? json['orgName']?.toString() ?? '',
        createdAt:
            json['createdAt']?.toString() ?? json['date']?.toString() ?? '',
        status: ComplaintStatusX.fromString(json['status']?.toString()),
      );
}

class ComplaintPageModel {
  final List<ComplaintModel> items;
  final int totalCount;
  final int page;
  final int pageSize;

  const ComplaintPageModel({
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  factory ComplaintPageModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['data'] as List? ?? data['items'] as List? ?? [];
    final items = list
        .whereType<Map<String, dynamic>>()
        .map(ComplaintModel.fromJson)
        .toList();
    return ComplaintPageModel(
      items: items,
      totalCount: data['totalCount'] as int? ?? items.length,
      page: data['page'] as int? ?? 1,
      pageSize: data['pageSize'] as int? ?? 20,
    );
  }
}
