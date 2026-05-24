/// Quy tắc chọn ngày phục vụ — khớp web `OrganizationMealOrderDateRules.cs`.
class OrgMealOrderDateRules {
  static DateTime dateOnly(DateTime d) => DateTime(d.year, d.month, d.day);

  static DateTime todayLocal() {
    final now = DateTime.now();
    return dateOnly(now);
  }

  /// Chuẩn hóa ngày từ API (tránh lệch múi giờ; bỏ qua DateOnly mặc định `0001-01-01`).
  static DateTime? parseApiDate(dynamic raw) {
    if (raw == null) return null;
    if (raw is DateTime) {
      final d = dateOnly(raw);
      return _isPlausibleServiceYear(d.year) ? d : null;
    }
    final s = raw.toString().trim();
    if (s.length < 8) return null;
    final parsed = DateTime.tryParse(s.length >= 10 ? s.substring(0, 10) : s);
    if (parsed == null) return null;
    final d = dateOnly(parsed);
    return _isPlausibleServiceYear(d.year) ? d : null;
  }

  static bool _isPlausibleServiceYear(int year) => year >= 2000 && year <= 2100;

  /// Min/max từ API; nếu BE chưa gửi cửa sổ hợp lệ thì dùng quy tắc local (khớp web).
  static DateTime resolveMinDate(DateTime? apiMin, DateTime today) {
    final localMin = minimumServiceDate(today);
    if (apiMin == null) return localMin;
    final api = dateOnly(apiMin);
    return api.isBefore(localMin) ? localMin : api;
  }

  static DateTime resolveMaxDate(
    DateTime? apiMax,
    DateTime today,
    DateTime min,
  ) {
    final localMax = maximumServiceDate(today);
    if (apiMax == null) return localMax.isBefore(min) ? min.add(const Duration(days: 90)) : localMax;
    final api = dateOnly(apiMax);
    if (api.isBefore(min)) {
      return min.add(const Duration(days: 90));
    }
    return api;
  }

  static bool isAllowed(DateTime date, DateTime min, DateTime max) {
    final d = dateOnly(date);
    return !d.isBefore(dateOnly(min)) && !d.isAfter(dateOnly(max));
  }

  static bool isToday(DateTime date) {
    final t = todayLocal();
    final d = dateOnly(date);
    return d.year == t.year && d.month == t.month && d.day == t.day;
  }

  static DateTime clamp(DateTime value, DateTime min, DateTime max) {
    final v = dateOnly(value);
    final a = dateOnly(min);
    final b = dateOnly(max);
    if (v.isBefore(a)) return a;
    if (v.isAfter(b)) return b;
    return v;
  }

  static DateTime shiftDay(DateTime current, int delta, DateTime min, DateTime max) =>
      clamp(current.add(Duration(days: delta)), min, max);

  static DateTime mondayOfNextCalendarWeek(DateTime today) {
    final daysFromMonday = today.weekday == DateTime.sunday
        ? 6
        : today.weekday - DateTime.monday;
    final mondayThisWeek = today.subtract(Duration(days: daysFromMonday));
    return mondayThisWeek.add(const Duration(days: 7));
  }

  static DateTime minimumServiceDate(DateTime today) {
    final threeDaysAhead = today.add(const Duration(days: 3));
    final nextMonday = mondayOfNextCalendarWeek(today);
    var min = threeDaysAhead.isAfter(nextMonday) ? threeDaysAhead : nextMonday;

    if (today.weekday == DateTime.saturday) {
      final tuesdayNextWeek = nextMonday.add(const Duration(days: 1));
      if (min.isBefore(tuesdayNextWeek)) min = tuesdayNextWeek;
    }
    return min;
  }

  static DateTime defaultServiceDate(DateTime today) {
    final min = minimumServiceDate(today);
    var preferred = mondayOfNextCalendarWeek(today);
    if (today.weekday == DateTime.saturday) {
      preferred = preferred.add(const Duration(days: 1));
    }
    return !preferred.isBefore(min) ? preferred : min;
  }

  static DateTime maximumServiceDate(DateTime today, {int horizonDays = 90}) =>
      minimumServiceDate(today).add(Duration(days: horizonDays));

  static String ruleHint(DateTime today) {
    final min = minimumServiceDate(today);
    final minStr = '${min.day.toString().padLeft(2, '0')}/${min.month.toString().padLeft(2, '0')}/${min.year}';
    if (today.weekday == DateTime.saturday) {
      return 'Đặt trước tối thiểu 3 ngày. Hôm nay là Thứ 7 — ngày phục vụ sớm nhất: $minStr (Thứ 3 tuần sau trở đi).';
    }
    return 'Đặt trước tối thiểu 3 ngày. Ngày phục vụ sớm nhất: $minStr (Thứ 2 tuần sau hoặc sau đó).';
  }

  static String toIsoDate(DateTime d) =>
      '${d.year}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';

  static String formatDisplay(String iso) {
    final p = iso.split('-');
    if (p.length != 3) return iso;
    final y = int.tryParse(p[0]) ?? 0;
    if (!_isPlausibleServiceYear(y)) return iso;
    return '${p[2]}/${p[1]}/${p[0]}';
  }

  static String formatDisplayDate(DateTime d) {
    final x = dateOnly(d);
    return '${x.day.toString().padLeft(2, '0')}/${x.month.toString().padLeft(2, '0')}/${x.year}';
  }

  /// Hiển thị gọn trên tab ngày (tránh overflow).
  static String formatDisplayShort(String iso) {
    final p = iso.split('-');
    if (p.length != 3) return formatDisplay(iso);
    final y = int.tryParse(p[0]) ?? 0;
    if (!_isPlausibleServiceYear(y)) return formatDisplay(iso);
    return '${p[2]}/${p[1]}';
  }

  static String formatDisplayShortDate(DateTime d) {
    final x = dateOnly(d);
    return '${x.day.toString().padLeft(2, '0')}/${x.month.toString().padLeft(2, '0')}';
  }

  static String weekdayShort(DateTime d) {
    const labels = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];
    return labels[d.weekday - 1];
  }

  /// Các Thứ 2 trong khoảng cho phép — nút gợi ý nhanh.
  static List<DateTime> mondaysInRange(DateTime min, DateTime max, {int limit = 8}) {
    var cursor = dateOnly(min);
    final end = dateOnly(max);
    while (cursor.weekday != DateTime.monday && !cursor.isAfter(end)) {
      cursor = cursor.add(const Duration(days: 1));
    }
    final out = <DateTime>[];
    while (!cursor.isAfter(end) && out.length < limit) {
      if (cursor.weekday == DateTime.monday) out.add(cursor);
      cursor = cursor.add(const Duration(days: 7));
    }
    return out;
  }
}
