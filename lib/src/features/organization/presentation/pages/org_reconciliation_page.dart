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
      final paymentBundles = await Future.wait(
        contracts.items.map((c) async {
          try {
            final payments =
                await OrgRepository.instance.getContractPayments(c.id);
            return (contract: c, payments: payments.items);
          } catch (e) {
            debugPrint('Payments contract ${c.id}: $e');
            return (contract: c, payments: <ContractPaymentModel>[]);
          }
        }),
      );
      for (final bundle in paymentBundles) {
        for (final p in bundle.payments) {
          _rows.add(_PaymentRow(contract: bundle.contract, payment: p));
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

  Color _statusColor(PaymentStatus status) {
    switch (status) {
      case PaymentStatus.success:
        return AppDesignSystem.success;
      case PaymentStatus.failed:
        return AppDesignSystem.danger;
      default:
        return AppDesignSystem.warning;
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đối soát thanh toán',
      onRefresh: _load,
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tải thanh toán hợp đồng…')
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: orgListPadding(context),
                  children: [
                    const OrgPageIntro(
                      title: 'Đối soát thanh toán',
                      description:
                          'Tổng hợp các kỳ thanh toán PayOS gắn hợp đồng B2B của đơn vị.',
                      icon: Icons.receipt_long_rounded,
                    ),
                    const SizedBox(height: 14),
                    Row(
                      children: [
                        Expanded(
                          child: OrgStatTile(
                            label: 'Đã thanh toán',
                            value: formatOrgVnd(_paidAmount),
                            icon: Icons.check_circle_outline,
                            color: AppDesignSystem.success,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: OrgStatTile(
                            label: 'Còn phải thu',
                            value: formatOrgVnd(_pendingAmount),
                            icon: Icons.schedule_rounded,
                            color: AppDesignSystem.warning,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 18),
                    OrgSectionHeader(
                      title: 'Lịch sử thanh toán',
                      subtitle: _contractCount > 0
                          ? '$_contractCount hợp đồng · ${_rows.length} kỳ'
                          : 'Chưa có hợp đồng',
                    ),
                    const SizedBox(height: 10),
                    if (_rows.isEmpty)
                      OrgEmptyList(
                        message: _contractCount == 0
                            ? 'Chưa có hợp đồng. Kiểm tra tài khoản đơn vị hoặc liên hệ quản trị.'
                            : 'Chưa có đơn/thanh toán gắn hợp đồng trong kỳ.',
                      )
                    else
                      ..._rows.map(_paymentRow),
                  ],
                ),
    );
  }

  Widget _paymentRow(_PaymentRow row) {
    final p = row.payment;
    final c = row.contract;
    final color = _statusColor(p.status);

    return OrgDataRow(
      icon: Icons.description_outlined,
      iconColor: orgAccent,
      title: c.code,
      subtitle: '${p.description}\n${p.date}',
      trailing: formatOrgVnd(p.amount),
      trailingColor: orgAccent,
      badge: OrgStatusBadge(label: p.status.label, color: color),
    );
  }
}

class _PaymentRow {
  final ContractModel contract;
  final ContractPaymentModel payment;

  _PaymentRow({required this.contract, required this.payment});
}
