import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';
import '../../data/models/finance_models.dart';
import '../../data/repositories/manager_repository.dart';
import '../widgets/manager_ui.dart';

class ManagerReconciliationPage extends StatefulWidget {
  const ManagerReconciliationPage({super.key});

  @override
  State<ManagerReconciliationPage> createState() => _ManagerReconciliationPageState();
}

class _ManagerReconciliationPageState extends State<ManagerReconciliationPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabs;
  PaymentReconciliationModel? _reconciliation;
  OrganizationReceivablesModel? _receivables;
  SupplierPayablesModel? _payables;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _tabs = TabController(length: 3, vsync: this);
    _load();
  }

  @override
  void dispose() {
    _tabs.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final now = DateTime.now();
      final start = now.subtract(const Duration(days: 30));
      final results = await Future.wait([
        ManagerRepository.instance.getPaymentReconciliation(
          startDate: start,
          endDate: now,
          onlyMismatches: false,
        ),
        ManagerRepository.instance.getOrganizationReceivables(),
        ManagerRepository.instance.getSupplierPayables(),
      ]);
      if (!mounted) return;
      setState(() {
        _reconciliation = results[0] as PaymentReconciliationModel;
        _receivables = results[1] as OrganizationReceivablesModel;
        _payables = results[2] as SupplierPayablesModel;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = apiErrorMessage(e);
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return ManagerPageShell(
      title: 'Đối soát thanh toán',
      onRefresh: _load,
      body: _loading
          ? const ManagerLoadingBody()
          : _error != null
              ? ManagerErrorBody(message: _error!, onRetry: _load)
              : Column(
                  children: [
                    Material(
                      color: Colors.white,
                      child: TabBar(
                        controller: _tabs,
                        labelColor: managerAccent,
                        indicatorColor: managerAccent,
                        tabs: const [
                          Tab(text: 'Đối soát đơn'),
                          Tab(text: 'Công nợ ĐV'),
                          Tab(text: 'Công nợ NCC'),
                        ],
                      ),
                    ),
                    Expanded(
                      child: TabBarView(
                        controller: _tabs,
                        children: [
                          _reconciliationList(),
                          _receivableList(),
                          _payableList(),
                        ],
                      ),
                    ),
                  ],
                ),
    );
  }

  Widget _reconciliationList() {
    final items = _reconciliation?.items ?? [];
    final mismatch = _reconciliation?.mismatchCount ?? 0;
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Row(
          children: [
            Expanded(
              child: ManagerStatTile(
                label: 'Tổng đơn',
                value: '${_reconciliation?.orderCount ?? 0}',
                icon: Icons.shopping_bag_rounded,
                color: AppColors.info,
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: ManagerStatTile(
                label: 'Lệch đối soát',
                value: '$mismatch',
                icon: Icons.warning_amber_rounded,
                color: AppColors.warning,
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (items.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList())
        else
          ...items.map(_reconTile),
      ],
    );
  }

  Widget _reconTile(ReconciliationItemModel item) {
    final statusColor = switch (item.status) {
      ReconciliationStatus.matched => AppColors.success,
      ReconciliationStatus.disputed => AppColors.danger,
      _ => AppColors.warning,
    };
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: ManagerGlassCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    item.orgName.isNotEmpty ? item.orgName : 'Đơn #${item.id}',
                    style: const TextStyle(fontWeight: FontWeight.w700),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: statusColor.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    item.status.label,
                    style: TextStyle(color: statusColor, fontSize: 11, fontWeight: FontWeight.w600),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 6),
            Text(
              'Đơn: ${formatVnd(item.amount, compact: true)} · Đã thu: ${formatVnd(item.paidAmount, compact: true)}',
              style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
            ),
            if (item.difference.abs() > 0.01)
              Text(
                'Chênh lệch: ${formatVnd(item.difference, compact: true)}',
                style: TextStyle(fontSize: 12, color: AppColors.danger, fontWeight: FontWeight.w600),
              ),
            if (item.issue != null && item.issue!.isNotEmpty)
              Padding(
                padding: const EdgeInsets.only(top: 4),
                child: Text(item.issue!, style: TextStyle(fontSize: 11, color: Colors.grey.shade500)),
              ),
          ],
        ),
      ),
    );
  }

  Widget _receivableList() {
    final lines = _receivables?.lines ?? [];
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        ManagerStatTile(
          label: 'Tổng công nợ đơn vị',
          value: formatVnd(_receivables?.grandTotal ?? 0, compact: true),
          icon: Icons.business_rounded,
          color: managerAccent,
        ),
        const SizedBox(height: 12),
        if (lines.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList())
        else
          ...lines.map((o) => _debtTile(
                title: o.organizationName,
                subtitle: '${o.orderCount} đơn · Đã thu ${formatVnd(o.totalPaid, compact: true)}',
                amount: o.totalReceivable,
              )),
      ],
    );
  }

  Widget _payableList() {
    final lines = _payables?.lines ?? [];
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        ManagerStatTile(
          label: 'Tổng công nợ NCC',
          value: formatVnd(_payables?.grandTotal ?? 0, compact: true),
          icon: Icons.local_shipping_rounded,
          color: AppColors.danger,
        ),
        const SizedBox(height: 12),
        if (lines.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList())
        else
          ...lines.map((p) => _debtTile(
                title: p.partnerName,
                subtitle: 'Hợp đồng ${formatVnd(p.totalContractValue, compact: true)}',
                amount: p.outstanding,
              )),
      ],
    );
  }

  Widget _debtTile({
    required String title,
    required String subtitle,
    required double amount,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: ManagerGlassCard(
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontWeight: FontWeight.w700)),
                  Text(subtitle, style: TextStyle(fontSize: 12, color: Colors.grey.shade600)),
                ],
              ),
            ),
            Text(
              formatVnd(amount, compact: true),
              style: TextStyle(fontWeight: FontWeight.w800, color: AppColors.danger),
            ),
          ],
        ),
      ),
    );
  }
}
