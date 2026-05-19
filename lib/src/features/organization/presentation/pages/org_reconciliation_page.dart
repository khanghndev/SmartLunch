import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/contract_models.dart';
import '../../data/org_repository.dart';
import '../widgets/organization_ui.dart';

/// Đối soát thanh toán theo hợp đồng đơn vị.
class OrgReconciliationPage extends StatefulWidget {
  const OrgReconciliationPage({super.key});

  @override
  State<OrgReconciliationPage> createState() => _OrgReconciliationPageState();
}

class _OrgReconciliationPageState extends State<OrgReconciliationPage> {
  bool _loading = true;
  String? _error;
  int _contractCount = 0;
  final List<_PaymentRow> _rows = [];

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
      _rows.clear();
    });
    try {
      final contracts = await OrgRepository.instance.getContracts(pageSize: 50);
      _contractCount = contracts.items.length;
      if (contracts.items.isEmpty) {
        if (!mounted) return;
        setState(() => _loading = false);
        return;
      }
      for (final c in contracts.items) {
        try {
          final payments = await OrgRepository.instance.getContractPayments(c.id);
          for (final p in payments.items) {
            _rows.add(_PaymentRow(contract: c, payment: p));
          }
        } catch (e) {
          debugPrint('Payments contract ${c.id}: $e');
        }
      }
      _rows.sort((a, b) => b.payment.date.compareTo(a.payment.date));
      if (!mounted) return;
      setState(() => _loading = false);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  double get _pendingAmount => _rows
      .where((r) => r.payment.status == PaymentStatus.pending)
      .fold(0.0, (s, r) => s + r.payment.amount);

  double get _paidAmount => _rows
      .where((r) => r.payment.status == PaymentStatus.success)
      .fold(0.0, (s, r) => s + r.payment.amount);

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đối soát thanh toán',
      onRefresh: _load,
      body: _loading
          ? const OrgLoadingBody()
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: ModuleStatTile(
                            label: 'Đã thanh toán',
                            value: formatOrgVnd(_paidAmount),
                            icon: Icons.check_circle_outline,
                            color: AppDesignSystem.success,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: ModuleStatTile(
                            label: 'Còn phải thu',
                            value: formatOrgVnd(_pendingAmount),
                            icon: Icons.schedule_rounded,
                            color: AppDesignSystem.warning,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    if (_rows.isEmpty)
                      ModuleEmptyList(
                        message: _contractCount == 0
                            ? 'Chưa có hợp đồng. Kiểm tra user_organizations hoặc đăng nhập đúng tài khoản đơn vị.'
                            : 'Chưa có đơn/thanh toán gắn hợp đồng trong kỳ.',
                      )
                    else
                      ..._rows.map((row) => _paymentTile(row)),
                  ],
                ),
    );
  }

  Widget _paymentTile(_PaymentRow row) {
    final p = row.payment;
    final c = row.contract;
    Color statusColor;
    switch (p.status) {
      case PaymentStatus.success:
        statusColor = AppDesignSystem.success;
        break;
      case PaymentStatus.failed:
        statusColor = AppDesignSystem.danger;
        break;
      default:
        statusColor = AppDesignSystem.warning;
    }

    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: OrgCard(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(c.code, style: AppDesignSystem.label()),
                ),
                Text(
                  p.status.label,
                  style: AppDesignSystem.body(size: 11, color: statusColor),
                ),
              ],
            ),
            const SizedBox(height: 4),
            Text(p.description, style: AppDesignSystem.body(size: 13)),
            const SizedBox(height: 4),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(p.date, style: AppDesignSystem.body(size: 12)),
                Text(
                  formatOrgVnd(p.amount),
                  style: AppDesignSystem.sectionTitle(color: orgAccent),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _PaymentRow {
  final ContractModel contract;
  final ContractPaymentModel payment;

  _PaymentRow({required this.contract, required this.payment});
}
