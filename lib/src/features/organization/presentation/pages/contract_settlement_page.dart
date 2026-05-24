import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/contract_models.dart';
import '../../data/models/org_order_annex_sign_args.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_pay_deposit.dart';
import '../widgets/org_signature_pad.dart';
import '../widgets/organization_ui.dart';

/// Hợp đồng & thanh toán đơn vị B2B — khớp web `Profile/Contracts` + `Orders` (PayOS cọc).
class ContractSettlementPage extends StatefulWidget {
  const ContractSettlementPage({super.key});

  @override
  State<ContractSettlementPage> createState() => _ContractSettlementPageState();
}

class _ContractSettlementPageState extends State<ContractSettlementPage> {
  bool _loading = true;
  String? _error;
  List<ContractModel> _contracts = [];
  ContractModel? _selected;
  List<ContractPaymentModel> _payments = [];
  bool _loadingPayments = false;
  int? _payingOrderId;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final res = await OrgRepository.instance.getContracts(pageSize: 50);
      if (!mounted) return;
      setState(() {
        _contracts = res.items;
        _loading = false;
        if (_contracts.isNotEmpty && _selected == null) {
          _selected = _contracts.first;
          _loadPayments(_selected!.id);
        }
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  Future<void> _loadPayments(int contractId) async {
    setState(() => _loadingPayments = true);
    try {
      final res = await OrgRepository.instance.getContractPayments(contractId);
      if (!mounted) return;
      setState(() {
        _payments = res.items;
        _loadingPayments = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _loadingPayments = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Lỗi tải thanh toán: ${orgApiError(e)}'),
          backgroundColor: AppDesignSystem.danger,
        ),
      );
    }
  }

  Future<void> _signContract(ContractModel c) async {
    final sigKey = GlobalKey<OrgSignaturePadState>();
    final signed = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (ctx) {
        return Padding(
          padding: EdgeInsets.only(
            left: 20,
            right: 20,
            top: 20,
            bottom: MediaQuery.of(ctx).viewInsets.bottom + 24,
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text('Ký hợp đồng ${c.code}', style: AppDesignSystem.sectionTitle()),
              const SizedBox(height: 8),
              Text(
                'Ký bằng ngón tay hoặc bút trong khung bên dưới.',
                style: AppDesignSystem.body(size: 12),
              ),
              const SizedBox(height: 12),
              OrgSignaturePad(key: sigKey),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => Navigator.of(ctx).pop(false),
                      child: const Text('Hủy'),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: FilledButton(
                      onPressed: () async {
                        final dataUrl =
                            await sigKey.currentState?.exportDataUrl();
                        if (dataUrl == null || dataUrl.length < 32) {
                          if (!ctx.mounted) return;
                          ScaffoldMessenger.of(ctx).showSnackBar(
                            const SnackBar(
                              content: Text('Vui lòng ký trong khung trước.'),
                            ),
                          );
                          return;
                        }
                        try {
                          await OrgRepository.instance.signContract(
                            contractId: c.id,
                            digitalSignature: dataUrl,
                          );
                          if (!ctx.mounted) return;
                          Navigator.of(ctx).pop(true);
                          if (!mounted) return;
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text('Đã ký hợp đồng ${c.code}'),
                              backgroundColor: AppDesignSystem.success,
                            ),
                          );
                        } catch (e) {
                          if (!ctx.mounted) return;
                          ScaffoldMessenger.of(ctx).showSnackBar(
                            SnackBar(
                              content: Text(orgApiError(e)),
                              backgroundColor: AppDesignSystem.danger,
                            ),
                          );
                        }
                      },
                      style: FilledButton.styleFrom(backgroundColor: orgAccent),
                      child: const Text('Xác nhận ký'),
                    ),
                  ),
                ],
              ),
            ],
          ),
        );
      },
    );
    if (signed == true && mounted) _load();
  }

  Future<void> _payDeposit(ContractPaymentModel p) async {
    final orderId = p.orderId;
    if (orderId == null || orderId <= 0) return;
    setState(() => _payingOrderId = orderId);
    try {
      await OrgMealPayDeposit.launch(
        context: context,
        orderId: orderId,
        depositAmountHint: p.amount,
      );
      if (mounted) _loadPayments(_selected!.id);
    } finally {
      if (mounted) setState(() => _payingOrderId = null);
    }
  }

  void _openAnnexSign(ContractPaymentModel p) {
    final orderId = p.orderId;
    if (orderId == null || orderId <= 0) return;
    Navigator.of(context).pushNamed(
      AppRoutes.orgOrderAnnexSign,
      arguments: OrgOrderAnnexSignArgs(
        orderId: orderId,
        depositPercent: 30,
        depositAmountVnd: p.amount.round(),
        totalAmount: p.amount,
        fromCheckout: false,
      ),
    );
  }

  Color _contractStatusColor(ContractStatus status) {
    switch (status) {
      case ContractStatus.active:
      case ContractStatus.signed:
        return AppDesignSystem.success;
      case ContractStatus.draft:
        return AppDesignSystem.warning;
      default:
        return AppDesignSystem.gray500;
    }
  }

  Color _paymentBadgeColor(ContractPaymentModel p) {
    final raw = p.orderPaymentStatus?.toLowerCase();
    if (raw == 'awaiting_payment') return AppDesignSystem.warning;
    if (raw == 'deposit_paid' || raw == 'partial' || raw == 'paid') {
      return AppDesignSystem.success;
    }
    if (raw == 'unpaid') return AppDesignSystem.gray500;
    return _paymentStatusColor(p.status);
  }

  Color _paymentStatusColor(PaymentStatus status) {
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
      title: 'Hợp đồng & Thanh toán',
      onRefresh: _load,
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tải hợp đồng…')
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: orgListPadding(context),
                  children: [
                    const OrgPageIntro(
                      title: 'Hợp đồng & thanh toán B2B',
                      description:
                          'Chọn hợp đồng để xem đơn gắn HĐ. Ký phụ lục hoặc thanh toán cọc qua PayOS '
                          '(giống lịch sử đơn trên web).',
                      icon: Icons.handshake_rounded,
                    ),
                    const SizedBox(height: 14),
                    OrgSectionHeader(
                      title: 'Danh sách hợp đồng',
                      subtitle: '${_contracts.length} hợp đồng',
                    ),
                    const SizedBox(height: 10),
                    if (_contracts.isEmpty)
                      const OrgEmptyList(
                        message: 'Chưa có hợp đồng cho đơn vị này',
                      )
                    else
                      ..._contracts.map(_contractTile),
                    if (_selected != null) ...[
                      const SizedBox(height: 20),
                      OrgSectionHeader(
                        title: 'Thanh toán',
                        subtitle: _selected!.code,
                      ),
                      const SizedBox(height: 10),
                      if (_loadingPayments)
                        const Padding(
                          padding: EdgeInsets.symmetric(vertical: 24),
                          child: OrgLoadingBody(
                            message: 'Đang tải kỳ thanh toán…',
                          ),
                        )
                      else if (_payments.isEmpty)
                        const OrgEmptyList(
                          message: 'Chưa có kỳ thanh toán cho hợp đồng này',
                        )
                      else
                        ..._payments.map(_paymentTile),
                    ],
                  ],
                ),
    );
  }

  Widget _contractTile(ContractModel c) {
    final selected = _selected?.id == c.id;
    final statusColor = _contractStatusColor(c.status);

    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: () {
            setState(() => _selected = c);
            _loadPayments(c.id);
          },
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
          child: Ink(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
              border: Border.all(
                color: selected ? orgAccent : AppDesignSystem.gray200,
                width: selected ? 2 : 1,
              ),
              color:
                  selected ? orgAccent.withValues(alpha: 0.04) : Colors.white,
              boxShadow: selected
                  ? [
                      BoxShadow(
                        color: orgAccent.withValues(alpha: 0.12),
                        blurRadius: 8,
                        offset: const Offset(0, 2),
                      ),
                    ]
                  : null,
            ),
            padding: const EdgeInsets.all(14),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: Text(c.code, style: AppDesignSystem.sectionTitle()),
                    ),
                    OrgStatusBadge(label: c.status.label, color: statusColor),
                  ],
                ),
                const SizedBox(height: 6),
                Text(c.type, style: AppDesignSystem.body(size: 12)),
                const SizedBox(height: 4),
                Text(
                  formatOrgVnd(c.value),
                  style: AppDesignSystem.label(color: orgAccent),
                ),
                if (c.status == ContractStatus.draft ||
                    c.status == ContractStatus.active) ...[
                  const SizedBox(height: 10),
                  Align(
                    alignment: Alignment.centerRight,
                    child: TextButton.icon(
                      onPressed: () => _signContract(c),
                      icon: Icon(Icons.draw_outlined, size: 18, color: orgAccent),
                      label: Text(
                        'Ký hợp đồng',
                        style: TextStyle(color: orgAccent),
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _paymentTile(ContractPaymentModel p) {
    final statusColor = _paymentBadgeColor(p);
    final orderId = p.orderId;
    final paying = orderId != null && _payingOrderId == orderId;

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
                Icon(Icons.payment_rounded, color: statusColor, size: 28),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(p.description, style: AppDesignSystem.label()),
                      if (p.date.isNotEmpty) ...[
                        const SizedBox(height: 4),
                        Text(p.date, style: AppDesignSystem.body(size: 12)),
                      ],
                      const SizedBox(height: 6),
                      OrgStatusBadge(
                        label: p.paymentStatusLabel,
                        color: statusColor,
                      ),
                    ],
                  ),
                ),
                Text(
                  formatOrgVnd(p.amount),
                  style: AppDesignSystem.label(color: orgAccent),
                ),
              ],
            ),
            if (p.isB2bOrder) ...[
              const SizedBox(height: 12),
              if (p.canPayDeposit)
                SizedBox(
                  width: double.infinity,
                  child: FilledButton.icon(
                    onPressed: paying ? null : () => _payDeposit(p),
                    icon: paying
                        ? const SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Icon(Icons.credit_card_rounded, size: 20),
                    label: Text(paying ? 'Đang mở PayOS…' : 'Thanh toán đặt cọc'),
                    style: FilledButton.styleFrom(
                      backgroundColor: const Color(0xFFF97316),
                      padding: const EdgeInsets.symmetric(vertical: 12),
                    ),
                  ),
                )
              else if (p.needsAnnexSign)
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton.icon(
                    onPressed: () => _openAnnexSign(p),
                    icon: const Icon(Icons.draw_rounded, size: 18),
                    label: const Text('Ký phụ lục trước'),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: const Color(0xFF7C3AED),
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
}
