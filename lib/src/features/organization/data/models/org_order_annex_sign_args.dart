/// Tham số màn ký phụ lục sau checkout — khớp web `Profile/Contracts` + `SignOrderAnnex`.
class OrgOrderAnnexSignArgs {
  final int orderId;
  final int depositPercent;
  final int depositAmountVnd;
  final double totalAmount;
  final String organizationName;
  final int totalMainQuantity;
  /// Banner sau checkout — khớp web `TempData["OrderSuccess"]` trên `Profile/Contracts`.
  final bool fromCheckout;

  const OrgOrderAnnexSignArgs({
    required this.orderId,
    required this.depositPercent,
    required this.depositAmountVnd,
    required this.totalAmount,
    this.organizationName = '',
    this.totalMainQuantity = 0,
    this.fromCheckout = true,
  });
}
