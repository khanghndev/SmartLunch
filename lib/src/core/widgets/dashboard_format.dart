/// Định dạng số liệu hiển thị trên dashboard.
String formatDashboardVnd(double amount, {bool compact = true}) {
  if (compact) {
    if (amount >= 1e9) return '${(amount / 1e9).toStringAsFixed(1)} tỷ';
    if (amount >= 1e6) return '${(amount / 1e6).toStringAsFixed(1)}Mđ';
    if (amount >= 1e3) return '${(amount / 1e3).toStringAsFixed(0)}Kđ';
    return '${amount.toStringAsFixed(0)}đ';
  }
  return '${amount.toStringAsFixed(0)} đ';
}

String formatDashboardPercent(double value) {
  if (value.isNaN || value.isInfinite) return '—';
  return '${value.toStringAsFixed(1)}%';
}
