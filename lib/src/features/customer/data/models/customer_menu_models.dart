/// Một dòng trong `GET /api/v1/master-data/WeeklyMenu` (phân trang).
class CustomerWeeklyMenuSummaryModel {
  final int id;
  final DateTime startDate;
  final DateTime endDate;
  final String? description;
  final String? imageUrl;

  const CustomerWeeklyMenuSummaryModel({
    required this.id,
    required this.startDate,
    required this.endDate,
    this.description,
    this.imageUrl,
  });

  factory CustomerWeeklyMenuSummaryModel.fromJson(
    Map<String, dynamic> json,
  ) {
    final idRaw = json['id'];
    final id = idRaw is int ? idRaw : int.tryParse('$idRaw') ?? 0;

    DateTime parseDate(String key) {
      final v = json[key];
      if (v is String) {
        return DateTime.tryParse(v) ?? DateTime.now();
      }
      return DateTime.now();
    }

    return CustomerWeeklyMenuSummaryModel(
      id: id,
      startDate: parseDate('startDate'),
      endDate: parseDate('endDate'),
      description: json['description'] as String?,
      imageUrl: json['imageUrl'] as String?,
    );
  }
}

class CustomerDishModel {
  final int id;
  final String name;
  final String? description;
  final String? category;
  final num price;
  final String? dietaryLabel;
  final String? imageUrl;
  final num? calories;

  CustomerDishModel({
    required this.id,
    required this.name,
    this.description,
    this.category,
    required this.price,
    this.dietaryLabel,
    this.imageUrl,
    this.calories,
  });

  factory CustomerDishModel.fromJson(Map<String, dynamic> json) {
    return CustomerDishModel(
      id: json['id'] as int? ?? 0,
      name: json['name'] as String? ?? '',
      description: json['description'] as String?,
      category: json['category'] as String?,
      price: json['price'] as num? ?? 0,
      dietaryLabel: json['dietaryLabel'] as String?,
      imageUrl: json['imageUrl'] as String?,
      calories: json['calories'] as num?,
    );
  }
}

class CustomerMenuScheduleModel {
  final int id;
  final DateTime date;
  final String mealSlot;
  final CustomerDishModel dish;

  CustomerMenuScheduleModel({
    required this.id,
    required this.date,
    required this.mealSlot,
    required this.dish,
  });

  factory CustomerMenuScheduleModel.fromJson(Map<String, dynamic> json) {
    return CustomerMenuScheduleModel(
      id: json['id'] as int? ?? 0,
      date: json['date'] != null
          ? DateTime.parse(json['date'] as String)
          : DateTime.now(),
      mealSlot: json['mealSlot'] as String? ?? '',
      dish: CustomerDishModel.fromJson(json['dish'] as Map<String, dynamic>? ?? {}),
    );
  }
}

class CustomerWeeklyMenuModel {
  final int id;
  final DateTime startDate;
  final DateTime endDate;
  final String? description;
  final List<CustomerMenuScheduleModel> schedules;

  CustomerWeeklyMenuModel({
    required this.id,
    required this.startDate,
    required this.endDate,
    this.description,
    required this.schedules,
  });

  factory CustomerWeeklyMenuModel.fromJson(Map<String, dynamic> json) {
    final schedulesJson = json['schedules'] as List<dynamic>? ?? [];
    return CustomerWeeklyMenuModel(
      id: json['id'] as int? ?? 0,
      startDate: json['startDate'] != null
          ? DateTime.parse(json['startDate'] as String)
          : DateTime.now(),
      endDate: json['endDate'] != null
          ? DateTime.parse(json['endDate'] as String)
          : DateTime.now(),
      description: json['description'] as String?,
      schedules: schedulesJson
          .map((e) => CustomerMenuScheduleModel.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }

  /// Payload `data` từ `GET /api/v1/master-data/WeeklyMenu/{id}/detail`
  /// (`weeklyMenu` + `schedules` cùng cấp).
  factory CustomerWeeklyMenuModel.fromWeeklyMenuDetailApi(
    Map<String, dynamic> json,
  ) {
    final wm = json['weeklyMenu'] as Map<String, dynamic>? ?? {};
    final schedulesJson = json['schedules'] as List<dynamic>? ?? [];

    return CustomerWeeklyMenuModel(
      id: wm['id'] as int? ?? 0,
      startDate: wm['startDate'] != null
          ? DateTime.parse(wm['startDate'] as String)
          : DateTime.now(),
      endDate: wm['endDate'] != null
          ? DateTime.parse(wm['endDate'] as String)
          : DateTime.now(),
      description: wm['description'] as String?,
      schedules: schedulesJson
          .map((e) => CustomerMenuScheduleModel.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}
