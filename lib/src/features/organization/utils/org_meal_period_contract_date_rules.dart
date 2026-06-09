/// Quy tắc HĐ theo kỳ — khớp web `OrganizationMealContractDateRules.cs`.
abstract final class OrgMealPeriodContractDateRules {
  static const int minStartLeadDays = 3;

  static DateTime dateOnly(DateTime d) => DateTime(d.year, d.month, d.day);

  static DateTime todayLocal() => dateOnly(DateTime.now());

  static DateTime getMinPeriodStart(DateTime today) =>
      dateOnly(today).add(const Duration(days: minStartLeadDays));

  static DateTime getDefaultPeriodStart(DateTime today) =>
      getMinPeriodStart(today);

  static DateTime getMaxPeriodEnd(DateTime start) {
    final s = dateOnly(start);
    final nextMonth = DateTime(s.year, s.month + 1, s.day);
    return dateOnly(nextMonth).subtract(const Duration(days: 1));
  }

  static DateTime getDefaultPeriodEnd(DateTime start) => getMaxPeriodEnd(start);

  static String getRuleHint(DateTime today) {
    final minStart = getMinPeriodStart(today);
    final sampleEnd = getMaxPeriodEnd(minStart);
    return 'Ngày bắt đầu phải từ ${formatDisplay(minStart)} trở đi '
        '(đặt trước ít nhất $minStartLeadDays ngày để chuẩn bị suất ăn). '
        'Thời hạn tối đa 1 tháng (vd. bắt đầu ${formatDisplay(minStart)} '
        '→ kết thúc tối đa ${formatDisplay(sampleEnd)}).';
  }

  static int countServiceDays(
    DateTime start,
    DateTime end,
    Iterable<String> excludedIso,
  ) {
    final s = dateOnly(start);
    final e = dateOnly(end);
    if (e.isBefore(s)) return 0;
    final excluded = excludedInRange(s, e, excludedIso).length;
    final inclusive = e.difference(s).inDays + 1;
    return (inclusive - excluded).clamp(0, 1 << 30);
  }

  static int countTotalMeals({
    required DateTime start,
    required DateTime end,
    required Iterable<String> excludedIso,
    required int defaultMealsPerDay,
    Map<String, int>? dailyOverridesByIso,
  }) {
    if (defaultMealsPerDay < 1) return 0;
    var total = 0;
    for (final date in enumerateServiceDates(start, end, excludedIso)) {
      final iso = toIsoDate(date);
      final custom = dailyOverridesByIso?[iso];
      total += (custom != null && custom > 0) ? custom : defaultMealsPerDay;
    }
    return total;
  }

  static double computeTotal({
    required DateTime start,
    required DateTime end,
    required Iterable<String> excludedIso,
    required int mealsPerDay,
    required double mealUnitPrice,
    Map<String, int>? dailyOverridesByIso,
  }) {
    final totalMeals = countTotalMeals(
      start: start,
      end: end,
      excludedIso: excludedIso,
      defaultMealsPerDay: mealsPerDay,
      dailyOverridesByIso: dailyOverridesByIso,
    );
    if (totalMeals < 1 || mealUnitPrice <= 0) return 0;
    return (totalMeals * mealUnitPrice).roundToDouble();
  }

  static Iterable<DateTime> enumerateServiceDates(
    DateTime start,
    DateTime end,
    Iterable<String> excludedIso,
  ) sync* {
    final s = dateOnly(start);
    final e = dateOnly(end);
    if (e.isBefore(s)) return;
    final excluded = excludedInRange(s, e, excludedIso).toSet();
    var d = s;
    while (!d.isAfter(e)) {
      if (!excluded.contains(toIsoDate(d))) yield d;
      d = d.add(const Duration(days: 1));
    }
  }

  static List<String> excludedInRange(
    DateTime start,
    DateTime end,
    Iterable<String> excludedIso,
  ) {
    final s = dateOnly(start);
    final e = dateOnly(end);
    return excludedIso
        .map((iso) => DateTime.tryParse(iso))
        .whereType<DateTime>()
        .map(dateOnly)
        .where((d) => !d.isBefore(s) && !d.isAfter(e))
        .map(toIsoDate)
        .toSet()
        .toList()
      ..sort();
  }

  static bool isInRange(DateTime date, DateTime start, DateTime end) {
    final d = dateOnly(date);
    final s = dateOnly(start);
    final e = dateOnly(end);
    return !d.isBefore(s) && !d.isAfter(e);
  }

  static String toIsoDate(DateTime d) {
    final x = dateOnly(d);
    return '${x.year.toString().padLeft(4, '0')}-'
        '${x.month.toString().padLeft(2, '0')}-'
        '${x.day.toString().padLeft(2, '0')}';
  }

  static String formatDisplay(DateTime d) {
    final x = dateOnly(d);
    return '${x.day.toString().padLeft(2, '0')}/'
        '${x.month.toString().padLeft(2, '0')}/'
        '${x.year}';
  }

  static String weekdayShort(DateTime d) {
    const labels = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];
    return labels[d.weekday - 1];
  }

  static String weekendNote(DateTime d) {
    if (d.weekday == DateTime.saturday) return 'Thứ Bảy';
    if (d.weekday == DateTime.sunday) return 'Chủ nhật';
    return '';
  }
}
