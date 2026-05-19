import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../../data/models/contract_models.dart';
import '../../data/org_repository.dart';
import '../widgets/organization_ui.dart';

/// Hợp đồng & thanh toán đơn vị B2B.
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
      setState(() {
        _contracts = res.items;
        _loading = false;
        if (_contracts.isNotEmpty && _selected == null) {
          _selected = _contracts.first;
          _loadPayments(_selected!.id);
        }
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  Future<void> _loadPayments(int contractId) async {
    setState(() => _loadingPayments = true);
    try {
      final res = await OrgRepository.instance.getContractPayments(contractId);
      setState(() {
        _payments = res.items;
        _loadingPayments = false;
      });
    } catch (e) {
      setState(() => _loadingPayments = false);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Lỗi tải thanh toán: $e')),
        );
      }
    }
  }

  Future<void> _signContract(ContractModel c) async {
    try {
      final profile = await ProfileRepository.instance.getProfile();
      final ok = await OrgRepository.instance.signContract(
        contractId: c.id,
        digitalSignature: profile.displayName,
      );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(ok ? 'Đã ký hợp đồng ${c.code}' : 'Ký hợp đồng thất bại'),
        ),
      );
      _load();
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(e.toString())),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Hợp đồng & Thanh toán',
      onRefresh: _load,
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(child: Text(_error!, textAlign: TextAlign.center))
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    Text(
                      '${_contracts.length} hợp đồng',
                      style: AppDesignSystem.body(),
                    ),
                    const SizedBox(height: 12),
                    ..._contracts.map((c) => _contractTile(c)),
                    if (_selected != null) ...[
                      const SizedBox(height: 20),
                      Text('Thanh toán — ${_selected!.code}', style: AppDesignSystem.sectionTitle()),
                      const SizedBox(height: 8),
                      if (_loadingPayments)
                        const Center(child: Padding(
                          padding: EdgeInsets.all(24),
                          child: CircularProgressIndicator(),
                        ))
                      else if (_payments.isEmpty)
                        Text('Chưa có kỳ thanh toán.', style: AppDesignSystem.body())
                      else
                        ..._payments.map(_paymentTile),
                    ],
                  ],
                ),
    );
  }

  Widget _contractTile(ContractModel c) {
    final selected = _selected?.id == c.id;
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: OrgCard(
        padding: const EdgeInsets.all(14),
        child: InkWell(
          onTap: () {
            setState(() => _selected = c);
            _loadPayments(c.id);
          },
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(c.code, style: AppDesignSystem.sectionTitle()),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: (selected ? orgAccent : AppDesignSystem.gray400)
                          .withValues(alpha: 0.15),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Text(
                      c.status.label,
                      style: AppDesignSystem.body(
                        size: 11,
                        color: selected ? orgAccent : AppDesignSystem.gray500,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 4),
              Text(c.type, style: AppDesignSystem.body(size: 12)),
              const SizedBox(height: 4),
              Text(
                formatOrgVnd(c.value),
                style: AppDesignSystem.label(color: orgAccent),
              ),
              if (c.status == ContractStatus.draft || c.status == ContractStatus.active) ...[
                const SizedBox(height: 10),
                Align(
                  alignment: Alignment.centerRight,
                  child: TextButton.icon(
                    onPressed: () => _signContract(c),
                    icon: const Icon(Icons.draw_outlined, size: 18),
                    label: const Text('Ký hợp đồng'),
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _paymentTile(ContractPaymentModel p) {
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
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(p.description, style: AppDesignSystem.label()),
                  Text(p.date, style: AppDesignSystem.body(size: 11)),
                ],
              ),
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Text(
                  formatOrgVnd(p.amount),
                  style: AppDesignSystem.sectionTitle().copyWith(fontSize: 14),
                ),
                Text(p.status.label, style: AppDesignSystem.body(size: 11, color: statusColor)),
                if (p.payosUrl != null && p.payosUrl!.isNotEmpty)
                  TextButton(
                    onPressed: () {
                      Clipboard.setData(ClipboardData(text: p.payosUrl!));
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Đã sao chép link PayOS')),
                      );
                    },
                    child: const Text('Link PayOS'),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
