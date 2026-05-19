/// Quy tắc chọn ngày phục vụ — khớp web `OrganizationMealOrderDateRules.cs`.
class OrgMealOrderDateRules {
  static DateTime dateOnly(DateTime d) => DateTime(d.year, d.month, d.day);

  static DateTime todayLocal() {
    final now = DateTime.now();
    return dateOnly(now);
  }

  /// Chuẩn hóa ngày từ API (tránh lệch múi giờ khi so sánh).
  static DateTime? parseApiDate(dynamic raw) {
    if (raw == null) return null;
    if (raw is DateTime) return dateOnly(raw);
    final s = raw.toString();
    if (s.length < 10) return null;
    final parsed = DateTime.tryParse(s.substring(0, 10));
    return parsed != null ? dateOnly(parsed) : null;
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
    return '${p[2]}/${p[1]}/${p[0]}';
  }
}
