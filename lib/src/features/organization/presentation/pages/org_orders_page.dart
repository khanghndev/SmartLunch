import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/org_order_models.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_pay_deposit.dart';
import '../widgets/organization_ui.dart';

class OrgOrdersPage extends StatefulWidget {
  final bool embeddedInModuleShell;

  const OrgOrdersPage({
    super.key,
    this.embeddedInModuleShell = false,
  });

  @override
  State<OrgOrdersPage> createState() => _OrgOrdersPageState();
}

class _OrgOrdersPageState extends State<OrgOrdersPage> {
  bool _loading = true;
  String? _error;
  List<OrgOrderModel> _orders = [];
  int _page = 1;
  bool _hasMore = true;
  bool _loadingMore = false;

  @override
  void initState() {
    super.initState();
    _loadInitial();
  }

  Future<void> _loadInitial() async {
    if (!mounted) return;
    setState(() {
      _loading = true;
      _error = null;
      _page = 1;
      _orders.clear();
      _hasMore = true;
    });

    try {
      final res = await OrgRepository.instance.getOrders(page: _page, pageSize: 20);
      if (!mounted) return;
      
      // Lọc bỏ các đơn con (Weekly Fulfillment) theo logic web
      final filteredItems = res.items.where((o) => !o.isWeeklyFulfillmentOrder).toList();
      
      setState(() {
        _orders = filteredItems;
        _loading = false;
        _hasMore = res.items.length >= 20;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  Future<void> _loadMore() async {
    if (_loadingMore || !_hasMore || _loading) return;
    
    setState(() => _loadingMore = true);
    try {
      _page++;
      final res = await OrgRepository.instance.getOrders(page: _page, pageSize: 20);
      if (!mounted) return;
      
      final filteredItems = res.items.where((o) => !o.isWeeklyFulfillmentOrder).toList();

      setState(() {
        _orders.addAll(filteredItems);
        _loadingMore = false;
        _hasMore = res.items.length >= 20;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _loadingMore = false;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Lỗi tải thêm đơn: ${orgApiError(e)}'),
          backgroundColor: AppDesignSystem.danger,
        ),
      );
    }
  }

  Future<void> _payDeposit(OrgOrderModel order) async {
    try {
      await OrgMealPayDeposit.launch(
        context: context,
        orderId: order.id,
        depositAmountHint: order.totalAmount, // Gợi ý full hoặc 30%, PayOS backend tự tính chuẩn
      );
      if (mounted) {
        _loadInitial();
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Không mở được thanh toán: $e'),
          backgroundColor: AppDesignSystem.danger,
        ),
      );
    }
  }

  Color _statusColor(String status) {
    switch (status.toLowerCase()) {
      case 'draft':
        return AppDesignSystem.warning;
      case 'pending':
      case 'placed':
      case 'preparing':
        return AppDesignSystem.info;
      case 'delivering':
      case 'out_for_delivery':
        return const Color(0xFF7C3AED); // Tím
      case 'confirmed':
      case 'delivered':
      case 'completed':
        return AppDesignSystem.success;
      case 'cancelled':
        return AppDesignSystem.danger;
      default:
        return AppDesignSystem.gray500;
    }
  }

  Color _paymentStatusColor(String paymentStatus) {
    switch (paymentStatus.toLowerCase()) {
      case 'unpaid':
      case 'awaiting_payment':
        return AppDesignSystem.danger;
      case 'deposit_paid':
      case 'partial':
      case 'paid':
        return AppDesignSystem.success;
      case 'refunded':
        return AppDesignSystem.warning;
      default:
        return AppDesignSystem.gray500;
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Lịch sử đặt món',
      onRefresh: _loadInitial,
      embeddedInModuleShell: widget.embeddedInModuleShell,
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tải danh sách đơn hàng…')
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _loadInitial)
              : NotificationListener<ScrollNotification>(
                  onNotification: (ScrollNotification scrollInfo) {
                    if (!_loadingMore && _hasMore && scrollInfo.metrics.pixels >= scrollInfo.metrics.maxScrollExtent - 200) {
                      _loadMore();
                    }
                    return false;
                  },
                  child: ModuleListView(
                    padding: orgListPadding(context),
                  children: [
                    const OrgPageIntro(
                      title: 'Lịch sử đơn hàng',
                      description: 'Danh sách toàn bộ các đơn hàng của đơn vị. Hỗ trợ thanh toán trực tiếp đối với các đơn đang chờ.',
                      icon: Icons.receipt_long_rounded,
                    ),
                    const SizedBox(height: 14),
                    OrgSectionHeader(
                      title: 'Danh sách đơn hàng',
                      subtitle: 'Đang hiển thị ${_orders.length} đơn',
                    ),
                    const SizedBox(height: 10),
                    if (_orders.isEmpty)
                      const OrgEmptyList(
                        message: 'Chưa có đơn hàng nào',
                      )
                    else
                      ..._orders.map(_orderTile),
                    if (_loadingMore)
                      const Padding(
                        padding: EdgeInsets.all(20.0),
                        child: Center(
                          child: CircularProgressIndicator(strokeWidth: 2),
                        ),
                      ),
                    ],
                  ),
                ),
    );
  }

  Widget _orderTile(OrgOrderModel order) {
    final bool canPayDeposit = 
        (order.paymentStatus.toLowerCase() == 'unpaid' || order.paymentStatus.toLowerCase() == 'awaiting_payment') && 
        (order.status.toLowerCase() != 'cancelled' && order.status.toLowerCase() != 'draft');

    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: OrgCard(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        order.invoiceCode ?? 'Đơn #${order.id}',
                        style: AppDesignSystem.sectionTitle(),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        order.typeLabel,
                        style: AppDesignSystem.body(size: 12),
                      ),
                      if (order.orderDate != null) ...[
                        const SizedBox(height: 4),
                        Text(
                          'Ngày đặt: ${_formatDate(order.orderDate!)}',
                          style: AppDesignSystem.body(size: 12),
                        ),
                      ],
                      if (order.scheduledDate != null) ...[
                        const SizedBox(height: 4),
                        Text(
                          'Ngày giao: ${_formatDate(order.scheduledDate!)}',
                          style: AppDesignSystem.body(size: 12),
                        ),
                      ],
                    ],
                  ),
                ),
                Text(
                  formatOrgVnd(order.totalAmount),
                  style: AppDesignSystem.label(color: orgAccent),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                OrgStatusBadge(
                  label: order.displayStatus,
                  color: _statusColor(order.status),
                ),
                const SizedBox(width: 8),
                OrgStatusBadge(
                  label: order.displayPaymentStatus,
                  color: _paymentStatusColor(order.paymentStatus),
                ),
              ],
            ),
            if (canPayDeposit) ...[
              const SizedBox(height: 12),
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: () => _payDeposit(order),
                  icon: const Icon(Icons.credit_card_rounded, size: 20),
                  label: const Text('Thanh toán đơn hàng'),
                  style: FilledButton.styleFrom(
                    backgroundColor: const Color(0xFFF97316),
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  String _formatDate(DateTime dt) {
    final d = dt.day.toString().padLeft(2, '0');
    final m = dt.month.toString().padLeft(2, '0');
    final y = dt.year;
    return '$d/$m/$y';
  }
}
