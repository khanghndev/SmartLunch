import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/finance_models.dart';
import '../../data/repositories/manager_repository.dart';
import '../widgets/manager_ui.dart';

class ManagerReconciliationPage extends StatefulWidget {
  final bool embeddedInModuleShell;

  const ManagerReconciliationPage({
    super.key,
    this.embeddedInModuleShell = false,
  });

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

  Widget _introHeader() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
      child: const ManagerPageIntro(
        title: 'Đối soát thanh toán',
        description:
            'Kiểm tra lệch đơn, công nợ đơn vị B2B và nhà cung cấp trong 30 ngày gần nhất.',
        icon: Icons.receipt_long_rounded,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return ManagerPageShell(
      title: 'Đối soát thanh toán',
      onRefresh: _load,
      embeddedInModuleShell: widget.embeddedInModuleShell,
      body: _loading
          ? const ManagerLoadingBody()
          : _error != null
              ? ManagerErrorBody(message: _error!, onRetry: _load)
              : ManagerTabbedBody(
                  controller: _tabs,
                  tabLabels: const ['Đối soát đơn', 'Công nợ ĐV', 'Công nợ NCC'],
                  top: _introHeader(),
                  children: [
                    _reconciliationList(),
                    _receivableList(),
                    _payableList(),
                  ],
                ),
    );
  }

  Widget _reconciliationList() {
    final items = _reconciliation?.items ?? [];
    final mismatch = _reconciliation?.mismatchCount ?? 0;
    return ListView(
      padding: managerListPadding(context).copyWith(top: 12),
      children: [
        Row(
          children: [
            Expanded(
              child: ManagerStatTile(
                label: 'Tổng đơn',
                value: '${_reconciliation?.orderCount ?? 0}',
                icon: Icons.shopping_bag_rounded,
                color: AppDesignSystem.info,
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: ManagerStatTile(
                label: 'Lệch đối soát',
                value: '$mismatch',
                icon: Icons.warning_amber_rounded,
                color: AppDesignSystem.warning,
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (items.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList(message: 'Không có đơn đối soát'))
        else
          ...items.map(_reconTile),
      ],
    );
  }

  Widget _reconTile(ReconciliationItemModel item) {
    final statusColor = switch (item.status) {
      ReconciliationStatus.matched => AppDesignSystem.success,
      ReconciliationStatus.disputed => AppDesignSystem.danger,
      _ => AppDesignSystem.warning,
    };
    final subtitle = StringBuffer()
      ..write('Đơn: ${formatVnd(item.amount, compact: true)}')
      ..write(' · Đã thu: ${formatVnd(item.paidAmount, compact: true)}');
    if (item.difference.abs() > 0.01) {
      subtitle.write(' · Chênh: ${formatVnd(item.difference, compact: true)}');
    }
    if (item.issue != null && item.issue!.isNotEmpty) {
      subtitle.write('\n${item.issue!}');
    }

    return ManagerDataRow(
      icon: Icons.receipt_long_outlined,
      iconColor: statusColor,
      title: item.orgName.isNotEmpty ? item.orgName : 'Đơn #${item.id}',
      subtitle: subtitle.toString(),
      badge: ManagerStatusBadge(label: item.status.label, color: statusColor),
    );
  }

  Widget _receivableList() {
    final lines = _receivables?.lines ?? [];
    return ListView(
      padding: managerListPadding(context).copyWith(top: 12),
      children: [
        ManagerStatTile(
          label: 'Tổng công nợ đơn vị',
          value: formatVnd(_receivables?.grandTotal ?? 0, compact: true),
          icon: Icons.business_rounded,
          color: managerAccent,
        ),
        const SizedBox(height: 12),
        if (lines.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList(message: 'Không có công nợ đơn vị'))
        else
          ...lines.map(
            (o) => ManagerDataRow(
              icon: Icons.apartment_rounded,
              iconColor: managerAccent,
              title: o.organizationName,
              subtitle:
                  '${o.orderCount} đơn · Đã thu ${formatVnd(o.totalPaid, compact: true)}',
              trailing: formatVnd(o.totalReceivable, compact: true),
              trailingColor: AppDesignSystem.danger,
            ),
          ),
      ],
    );
  }

  Widget _payableList() {
    final lines = _payables?.lines ?? [];
    return ListView(
      padding: managerListPadding(context).copyWith(top: 12),
      children: [
        ManagerStatTile(
          label: 'Tổng công nợ NCC',
          value: formatVnd(_payables?.grandTotal ?? 0, compact: true),
          icon: Icons.local_shipping_rounded,
          color: AppDesignSystem.danger,
        ),
        const SizedBox(height: 12),
        if (lines.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList(message: 'Không có công nợ NCC'))
        else
          ...lines.map(
            (p) => ManagerDataRow(
              icon: Icons.handshake_rounded,
              iconColor: AppDesignSystem.info,
              title: p.partnerName,
              subtitle: 'Hợp đồng ${formatVnd(p.totalContractValue, compact: true)}',
              trailing: formatVnd(p.outstanding, compact: true),
              trailingColor: AppDesignSystem.danger,
            ),
          ),
      ],
    );
  }
}
