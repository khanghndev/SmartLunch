/// Nhãn & quy tắc thanh toán cọc — khớp web `OrderPaymentStatusDisplay.cs`.
class OrgOrderPaymentStatus {
  static String label(String? status) {
    switch (status?.trim().toLowerCase()) {
      case 'unpaid':
        return 'Chưa thanh toán';
      case 'awaiting_payment':
        return 'Đang chờ thanh toán';
      case 'deposit_paid':
      case 'partial':
        return 'Đã đặt cọc';
      case 'paid':
        return 'Đã thanh toán đủ';
      default:
        return status?.isNotEmpty == true ? status! : '—';
    }
  }

  static bool canPayDeposit(String? status, {String? annexPdfUrl}) {
    final s = status?.trim().toLowerCase();
    if (s == 'awaiting_payment') return true;
    if (s == 'unpaid' &&
        annexPdfUrl != null &&
        annexPdfUrl.trim().isNotEmpty) {
      return true;
    }
    return false;
  }

  /// Chưa ký phụ lục — web hiện link «Ký phụ lục» trên Orders.
  static bool needsAnnexSign(String? status, {String? annexPdfUrl}) {
    final s = status?.trim().toLowerCase();
    return s == 'unpaid' &&
        (annexPdfUrl == null || annexPdfUrl.trim().isEmpty);
  }
}
