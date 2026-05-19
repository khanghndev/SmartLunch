import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_order_date_rules.dart';
import '../widgets/organization_ui.dart';

/// Bước 3 — Xác nhận nháp & đặt cọc (web OrganizationMealOrder/Review).
class OrgMealOrderReviewPage extends StatefulWidget {
  const OrgMealOrderReviewPage({super.key, required this.draft});

  final PrepareMealDraftModel draft;

  @override
  State<OrgMealOrderReviewPage> createState() => _OrgMealOrderReviewPageState();
}

class _OrgMealOrderReviewPageState extends State<OrgMealOrderReviewPage> {
  static const _depositOptions = [20, 25, 30, 35, 50];
  int _depositPercent = 30;
  bool _submitting = false;

  PrepareMealDraftModel get draft => widget.draft;

  Map<String, List<MealDraftLineSummaryModel>> get _linesByDate {
    final map = <String, List<MealDraftLineSummaryModel>>{};
    for (final l in draft.lines) {
      map.putIfAbsent(l.serviceDate, () => []).add(l);
    }
    return map;
  }

  Future<void> _checkout() async {
    setState(() => _submitting = true);
    try {
      final result = await OrgRepository.instance.checkoutMealOrder(
        draftId: draft.draftId,
        depositPercent: _depositPercent,
      );
      if (!mounted) return;

      var message =
          'Đã tạo đơn hàng #${result.orderId}. Vui lòng ký phụ lục và thanh toán cọc.';
      String? payUrl = result.checkoutUrl;

      if (payUrl == null || payUrl.isEmpty) {
        final pay = await OrgRepository.instance.initiateMealPayment(
          orderId: result.orderId,
        );
        payUrl = pay.checkoutUrl;
      }

      await showDialog<void>(
        context: context,
        builder: (ctx) => AlertDialog(
          title: const Text('Đặt hàng thành công'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(message),
              const SizedBox(height: 8),
              Text(
                'Cọc $_depositPercent%: ${formatOrgVnd(result.depositAmountVnd.toDouble())}',
                style: AppDesignSystem.body(size: 13),
              ),
              if (payUrl != null && payUrl.isNotEmpty) ...[
                const SizedBox(height: 12),
                SelectableText(payUrl, style: const TextStyle(fontSize: 12)),
              ],
            ],
          ),
          actions: [
            if (payUrl != null && payUrl.isNotEmpty)
              TextButton(
                onPressed: () {
                  Clipboard.setData(ClipboardData(text: payUrl!));
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text('Đã sao chép link thanh toán')),
                  );
                },
                child: const Text('Sao chép link'),
              ),
            FilledButton(
              onPressed: () {
                Navigator.of(ctx).pop();
                Navigator.of(context).pushNamedAndRemoveUntil(
                  AppRoutes.orgContractSettlement,
                  (r) => r.settings.name == AppRoutes.orgHome || r.isFirst,
                );
              },
              child: const Text('Xem hợp đồng'),
            ),
          ],
        ),
      );
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(e.toString()), backgroundColor: AppDesignSystem.danger),
        );
      }
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Xác nhận nháp',
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (draft.organizationName.isNotEmpty)
            Text(
              draft.organizationName,
              style: AppDesignSystem.body(color: orgAccent),
            ),
          const SizedBox(height: 8),
          Row(
            children: [
              _metricCard('Giá/suất', formatOrgVnd(draft.pricePerPortion)),
              const SizedBox(width: 8),
              _metricCard('Suất chính', '${draft.totalMainQuantity}'),
            ],
          ),
          const SizedBox(height: 8),
          _metricCard('Tổng thanh toán', formatOrgVnd(draft.totalAmount), wide: true),
          if (draft.discountAmount > 0)
            Padding(
              padding: const EdgeInsets.only(top: 8),
              child: Text(
                'Giảm ${formatOrgVnd(draft.discountAmount)}'
                '${draft.appliedPromotionName != null ? ' · ${draft.appliedPromotionName}' : ''}',
                style: AppDesignSystem.body(size: 12, color: AppDesignSystem.success),
              ),
            ),
          if (draft.persistenceNotice != null) ...[
            const SizedBox(height: 12),
            OrgCard(
              padding: const EdgeInsets.all(12),
              child: Text(draft.persistenceNotice!, style: AppDesignSystem.body(size: 12)),
            ),
          ],
          const SizedBox(height: 16),
          Text('Chi tiết thực đơn', style: AppDesignSystem.sectionTitle()),
          const SizedBox(height: 8),
          ..._linesByDate.entries.map((e) {
            return OrgCard(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    OrgMealOrderDateRules.formatDisplay(e.key),
                    style: AppDesignSystem.label(),
                  ),
                  const SizedBox(height: 8),
                  ...e.value.map(
                    (l) => Padding(
                      padding: const EdgeInsets.only(bottom: 4),
                      child: Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: orgAccent.withValues(alpha: 0.1),
                              borderRadius: BorderRadius.circular(6),
                            ),
                            child: Text(
                              l.slotLabel,
                              style: AppDesignSystem.body(size: 10, color: orgAccent),
                            ),
                          ),
                          const SizedBox(width: 8),
                          Expanded(child: Text(l.dishName)),
                          Text('×${l.quantity}', style: AppDesignSystem.label()),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            );
          }),
          const SizedBox(height: 16),
          Text('Tỷ lệ đặt cọc', style: AppDesignSystem.sectionTitle()),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: _depositOptions.map((pct) {
              final deposit = (draft.totalAmount * pct / 100).round();
              final selected = _depositPercent == pct;
              return ChoiceChip(
                label: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text('$pct%', style: const TextStyle(fontWeight: FontWeight.w800)),
                    Text(formatOrgVnd(deposit.toDouble()), style: const TextStyle(fontSize: 10)),
                  ],
                ),
                selected: selected,
                onSelected: (_) => setState(() => _depositPercent = pct),
                selectedColor: orgAccent.withValues(alpha: 0.2),
              );
            }).toList(),
          ),
          const SizedBox(height: 24),
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: _submitting ? null : _checkout,
              style: FilledButton.styleFrom(
                backgroundColor: orgAccent,
                padding: const EdgeInsets.symmetric(vertical: 14),
              ),
              child: Text(_submitting ? 'Đang xử lý…' : 'Đặt hàng — xác nhận'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _metricCard(String label, String value, {bool wide = false}) {
    return Expanded(
      flex: wide ? 1 : 1,
      child: OrgCard(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(label, style: AppDesignSystem.body(size: 11)),
            Text(value, style: AppDesignSystem.sectionTitle()),
          ],
        ),
      ),
    );
  }
}
