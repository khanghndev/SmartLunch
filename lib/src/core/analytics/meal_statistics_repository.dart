import '../config/api_paths.dart';
import '../config/app_env.dart';
import '../network/api_client.dart';
import '../network/api_exception.dart';

class MealStatisticItemModel {
  final DateTime date;
  final String mealSlot;
  final int? organizationId;
  final String organizationName;
  final int totalMeals;
  final double totalAmount;

  MealStatisticItemModel({
    required this.date,
    required this.mealSlot,
    this.organizationId,
    required this.organizationName,
    required this.totalMeals,
    required this.totalAmount,
  });

  factory MealStatisticItemModel.fromJson(Map<String, dynamic> json) {
    final dateRaw = json['date'];
    DateTime date;
    if (dateRaw is String) {
      date = DateTime.tryParse(dateRaw) ?? DateTime.now();
    } else {
      date = DateTime.now();
    }
    return MealStatisticItemModel(
      date: DateTime(date.year, date.month, date.day),
      mealSlot: json['mealSlot'] as String? ?? '',
      organizationId: json['organizationId'] as int?,
      organizationName: json['organizationName'] as String? ?? '',
      totalMeals: json['totalMeals'] as int? ?? 0,
      totalAmount: (json['totalAmount'] is num)
          ? (json['totalAmount'] as num).toDouble()
          : double.tryParse('${json['totalAmount']}') ?? 0,
    );
  }
}

class DetailedMealItemModel {
  final DateTime date;
  final String mealSlot;
  final String dishName;
  final int quantity;
  final double totalAmount;

  DetailedMealItemModel({
    required this.date,
    required this.mealSlot,
    required this.dishName,
    required this.quantity,
    required this.totalAmount,
  });

  factory DetailedMealItemModel.fromJson(Map<String, dynamic> json) {
    final dateRaw = json['date'];
    DateTime date;
    if (dateRaw is String) {
      date = DateTime.tryParse(dateRaw) ?? DateTime.now();
    } else {
      date = DateTime.now();
    }
    return DetailedMealItemModel(
      date: DateTime(date.year, date.month, date.day),
      mealSlot: json['mealSlot'] as String? ?? '',
      dishName: json['dishName'] as String? ?? '',
      quantity: json['quantity'] as int? ?? 0,
      totalAmount: (json['totalAmount'] is num)
          ? (json['totalAmount'] as num).toDouble()
          : double.tryParse('${json['totalAmount']}') ?? 0,
    );
  }
}

/// Khoảng thời gian [start, end] (cả hai inclusive theo ngày lịch local).
(DateTime start, DateTime end) mealStatsDateRangeFor({
  required DateTime now,
  required int daysBackInclusive,
}) {
  final today = DateTime(now.year, now.month, now.day);
  final start = today.subtract(Duration(days: daysBackInclusive));
  return (start, today);
}

String formatVndCompact(double amount) {
  if (amount >= 1e9) {
    return '${(amount / 1e9).toStringAsFixed(1)} tỷ';
  }
  if (amount >= 1e6) {
    return '${(amount / 1e6).toStringAsFixed(1)} tr';
  }
  if (amount >= 1e3) {
    return '${(amount / 1e3).toStringAsFixed(0)}k';
  }
  return '${amount.toStringAsFixed(0)}đ';
}

class MealStatisticsRepository {
  MealStatisticsRepository._({ApiClient? client})
      : _client = client ?? ApiClient(baseUrl: AppEnv.apiBaseUrl);

  static final MealStatisticsRepository instance = MealStatisticsRepository._();

  final ApiClient _client;

  Future<List<MealStatisticItemModel>> getMealStatistics({
    required DateTime startDate,
    required DateTime endDate,
    int? organizationId,
  }) async {
    final q = <String, dynamic>{
      'startDate': _dateQuery(startDate),
      'endDate': _dateQuery(endDate),
    };
    if (organizationId != null) {
      q['organizationId'] = organizationId;
    }
    final response = await _client.get(
      ApiPaths.orderStatisticsMealCount,
      queryParameters: q,
    );
    final data = response['data'] as Map<String, dynamic>?;
    final list = data?['data'] as List<dynamic>? ?? [];
    return list
        .map((e) => MealStatisticItemModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<List<DetailedMealItemModel>> getDetailedMealStatistics({
    required DateTime startDate,
    required DateTime endDate,
    int? organizationId,
  }) async {
    final q = <String, dynamic>{
      'startDate': _dateQuery(startDate),
      'endDate': _dateQuery(endDate),
    };
    if (organizationId != null) {
      q['organizationId'] = organizationId;
    }
    final response = await _client.get(
      ApiPaths.orderStatisticsDetails,
      queryParameters: q,
    );
    final data = response['data'] as Map<String, dynamic>?;
    final list = data?['data'] as List<dynamic>? ?? [];
    return list
        .map((e) => DetailedMealItemModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  String _dateQuery(DateTime d) {
    final x = DateTime(d.year, d.month, d.day);
    return '${x.year}-${x.month.toString().padLeft(2, '0')}-${x.day.toString().padLeft(2, '0')}';
  }
}

extension MealStatsAggregates on List<MealStatisticItemModel> {
  int get sumMeals => fold(0, (a, b) => a + b.totalMeals);

  double get sumAmount => fold<double>(0, (a, b) => a + b.totalAmount);

  Map<DateTime, int> mealsByDay() {
    final m = <DateTime, int>{};
    for (final e in this) {
      final k = DateTime(e.date.year, e.date.month, e.date.day);
      m[k] = (m[k] ?? 0) + e.totalMeals;
    }
    return m;
  }

  Map<DateTime, double> amountByDay() {
    final m = <DateTime, double>{};
    for (final e in this) {
      final k = DateTime(e.date.year, e.date.month, e.date.day);
      m[k] = (m[k] ?? 0) + e.totalAmount;
    }
    return m;
  }

  Map<String, int> mealsBySlot() {
    final m = <String, int>{};
    for (final e in this) {
      final key = e.mealSlot.trim().isEmpty ? 'Khác' : e.mealSlot;
      m[key] = (m[key] ?? 0) + e.totalMeals;
    }
    return m;
  }

  Map<String, int> mealsByOrganization() {
    final m = <String, int>{};
    for (final e in this) {
      final key = e.organizationName.trim().isEmpty ? 'Không xác định' : e.organizationName;
      m[key] = (m[key] ?? 0) + e.totalMeals;
    }
    return m;
  }
}

extension DetailedAggregates on List<DetailedMealItemModel> {
  List<({String name, int orders, double rating})> topDishes({int limit = 8}) {
    final m = <String, int>{};
    for (final e in this) {
      if (e.dishName.isEmpty) continue;
      m[e.dishName] = (m[e.dishName] ?? 0) + e.quantity;
    }
    final sorted = m.entries.toList()..sort((a, b) => b.value.compareTo(a.value));
    return sorted
        .take(limit)
        .map((e) => (name: e.key, orders: e.value, rating: 0.0))
        .toList();
  }
}

/// Gọi song song meal + detailed; bọc lỗi API thành message ngắn.
Future<({List<MealStatisticItemModel> meals, List<DetailedMealItemModel> details})>
    fetchMealStatsBundle({
  required DateTime startDate,
  required DateTime endDate,
  int? organizationId,
}) async {
  try {
    final repo = MealStatisticsRepository.instance;
    final meals = await repo.getMealStatistics(
      startDate: startDate,
      endDate: endDate,
      organizationId: organizationId,
    );
    final details = await repo.getDetailedMealStatistics(
      startDate: startDate,
      endDate: endDate,
      organizationId: organizationId,
    );
    return (meals: meals, details: details);
  } on ApiException {
    rethrow;
  } catch (e) {
    throw ApiException('Không tải được thống kê suất ăn: $e', statusCode: 500);
  }
}
