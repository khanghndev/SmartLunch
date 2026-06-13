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

// ─── Chi tiết món công khai (GET .../organization/meal-order/dish/{id}) ───

class CustomerDishImageModel {
  final int id;
  final String url;
  final String role;
  final int sortOrder;

  const CustomerDishImageModel({
    required this.id,
    required this.url,
    this.role = 'gallery',
    this.sortOrder = 0,
  });

  factory CustomerDishImageModel.fromJson(Map<String, dynamic> json) {
    return CustomerDishImageModel(
      id: json['id'] as int? ?? 0,
      url: json['url']?.toString() ?? '',
      role: json['role']?.toString() ?? 'gallery',
      sortOrder: json['sortOrder'] as int? ?? 0,
    );
  }
}

class CustomerDishIngredientQuotaModel {
  final int id;
  final int ingredientId;
  final String ingredientName;
  final int dishValueId;
  final double dishValueAmount;
  final String? dishValueLabel;
  final double quantity;
  final String? unit;

  const CustomerDishIngredientQuotaModel({
    required this.id,
    required this.ingredientId,
    required this.ingredientName,
    required this.dishValueId,
    required this.dishValueAmount,
    this.dishValueLabel,
    required this.quantity,
    this.unit,
  });

  factory CustomerDishIngredientQuotaModel.fromJson(Map<String, dynamic> json) {
    return CustomerDishIngredientQuotaModel(
      id: json['id'] as int? ?? 0,
      ingredientId: json['ingredientId'] as int? ?? 0,
      ingredientName: json['ingredientName']?.toString() ?? '',
      dishValueId: json['dishValueId'] as int? ?? 0,
      dishValueAmount: _toDouble(json['dishValueAmount']) ?? 0,
      dishValueLabel: json['dishValueLabel']?.toString(),
      quantity: _toDouble(json['quantity']) ?? 0,
      unit: json['unit']?.toString(),
    );
  }

  String get quantityLabel {
    final q = quantity;
    final formatted = q == q.roundToDouble()
        ? q.toStringAsFixed(0)
        : q.toStringAsFixed(1);
    final u = unit?.trim();
    if (u != null && u.isNotEmpty) return '$formatted $u';
    return formatted;
  }
}

class CustomerDishPriceTierModel {
  final String? label;
  final double amount;
  final int sortOrder;
  final double portionWeightGrams;
  final List<CustomerDishIngredientQuotaModel> ingredientQuotas;

  const CustomerDishPriceTierModel({
    this.label,
    required this.amount,
    this.sortOrder = 0,
    required this.portionWeightGrams,
    this.ingredientQuotas = const [],
  });

  factory CustomerDishPriceTierModel.fromJson(Map<String, dynamic> json) {
    final dv = json['dishValue'] as Map<String, dynamic>? ?? {};
    final quotas = json['ingredientQuotas'] as List? ?? [];
    return CustomerDishPriceTierModel(
      label: dv['label']?.toString(),
      amount: _toDouble(dv['amount']) ?? 0,
      sortOrder: dv['sortOrder'] as int? ?? 0,
      portionWeightGrams: _toDouble(json['portionWeightGrams']) ?? 0,
      ingredientQuotas: quotas
          .whereType<Map<String, dynamic>>()
          .map(CustomerDishIngredientQuotaModel.fromJson)
          .toList(),
    );
  }

  String get tierTitle {
    final l = label?.trim();
    if (l != null && l.isNotEmpty) return l;
    if (amount > 0) return '${amount.toStringAsFixed(0)}đ/suất';
    return 'Mức giá suất ăn';
  }
}

class CustomerDishDetailModel {
  final int id;
  final String? code;
  final String name;
  final String? nameEnglish;
  final String? description;
  final String? primarySlotKey;
  final List<String> slotCategoryCodes;
  final String? cookingMethod;
  final double price;
  final String? dietaryLabel;
  final String? imageUrl;
  final List<CustomerDishImageModel> images;
  final double? calories;
  final double? protein;
  final double? fat;
  final double? carbs;
  final bool isActive;
  final List<CustomerDishIngredientQuotaModel> ingredientQuotas;
  final List<CustomerDishPriceTierModel> priceTiers;

  const CustomerDishDetailModel({
    required this.id,
    this.code,
    required this.name,
    this.nameEnglish,
    this.description,
    this.primarySlotKey,
    this.slotCategoryCodes = const [],
    this.cookingMethod,
    this.price = 0,
    this.dietaryLabel,
    this.imageUrl,
    this.images = const [],
    this.calories,
    this.protein,
    this.fat,
    this.carbs,
    this.isActive = true,
    this.ingredientQuotas = const [],
    this.priceTiers = const [],
  });

  factory CustomerDishDetailModel.fromJson(Map<String, dynamic> json) {
    final dish = json['dish'] as Map<String, dynamic>? ?? json;
    final slots = dish['dishSlotCategoryCodes'] as List? ?? [];
    final imgs = dish['images'] as List? ?? [];
    final flatQuotas = json['ingredientQuotas'] as List? ?? [];
    final tiers = json['priceTiers'] as List? ?? [];

    return CustomerDishDetailModel(
      id: dish['id'] as int? ?? 0,
      code: dish['code']?.toString(),
      name: dish['name']?.toString() ?? '',
      nameEnglish: dish['nameEnglish']?.toString(),
      description: dish['description']?.toString(),
      primarySlotKey: dish['primarySlotKey']?.toString(),
      slotCategoryCodes: slots.map((e) => e.toString()).toList(),
      cookingMethod: dish['cookingMethod']?.toString(),
      price: _toDouble(dish['price']) ?? 0,
      dietaryLabel: dish['dietaryLabel']?.toString(),
      imageUrl: dish['imageUrl']?.toString(),
      images: imgs
          .whereType<Map<String, dynamic>>()
          .map(CustomerDishImageModel.fromJson)
          .where((e) => e.url.isNotEmpty)
          .toList(),
      calories: _toDouble(dish['calories']),
      protein: _toDouble(dish['protein']),
      fat: _toDouble(dish['fat']),
      carbs: _toDouble(dish['carbs']),
      isActive: dish['isActive'] as bool? ?? true,
      ingredientQuotas: flatQuotas
          .whereType<Map<String, dynamic>>()
          .map(CustomerDishIngredientQuotaModel.fromJson)
          .toList(),
      priceTiers: tiers
          .whereType<Map<String, dynamic>>()
          .map(CustomerDishPriceTierModel.fromJson)
          .toList(),
    );
  }

  String? get heroImageUrl {
    if (imageUrl != null && imageUrl!.trim().isNotEmpty) return imageUrl;
    if (images.isEmpty) return null;
    final cover = images.where((i) => i.role.toLowerCase() == 'cover').toList();
    if (cover.isNotEmpty) return cover.first.url;
    return images.first.url;
  }

  bool get hasNutrition =>
      calories != null || protein != null || fat != null || carbs != null;

  bool get hasIngredients =>
      priceTiers.any((t) => t.ingredientQuotas.isNotEmpty) ||
      ingredientQuotas.isNotEmpty;

  String get cookingMethodLabel => cookingMethodLabelOf(cookingMethod);

  String get slotCategoryLabel => slotCategoryLabelOf(primarySlotKey);

  static String cookingMethodLabelOf(String? key) {
    if (key == null || key.trim().isEmpty) return '';
    const map = {
      'fried': 'Chiên',
      'stewed': 'Kho / nấu',
      'boiled': 'Luộc',
      'steamed': 'Hấp',
      'grilled': 'Nướng',
      'stir_fried': 'Xào',
      'stir-fried': 'Xào',
      'raw': 'Ăn sống',
      'baked': 'Nướng lò',
      'braised': 'Om',
      'soup': 'Nấu canh',
    };
    return map[key.toLowerCase()] ?? key.replaceAll('_', ' ');
  }

  static String slotCategoryLabelOf(String? key) {
    if (key == null || key.trim().isEmpty) return '';
    const map = {
      'main': 'Món chính',
      'soup': 'Canh',
      'vegetable': 'Rau',
      'side': 'Món phụ',
      'dessert': 'Tráng miệng',
      'drink': 'Đồ uống',
      'staple': 'Cơm / tinh bột',
    };
    return map[key.toLowerCase()] ?? key;
  }
}

double? _toDouble(dynamic value) {
  if (value == null) return null;
  if (value is num) return value.toDouble();
  return double.tryParse(value.toString());
}
