import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/org_meal_contract_models.dart';
import '../../data/models/org_order_annex_sign_args.dart';
import '../../data/org_repository.dart';
import '../widgets/organization_ui.dart';

class OrgMealPeriodContractReviewPage extends StatefulWidget {
  const OrgMealPeriodContractReviewPage({super.key, required this.draft});

  final PrepareMealPeriodDraftModel draft;

  @override
  State<OrgMealPeriodContractReviewPage> createState() =>
      _OrgMealPeriodContractReviewPageState();
}

class _OrgMealPeriodContractReviewPageState
    extends State<OrgMealPeriodContractReviewPage> {
  int _depositPercent = 30;
  bool _submitting = false;
  String? _error;

  PrepareMealPeriodDraftModel get d => widget.draft;

  Future<void> _checkout() async {
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final result = await OrgRepository.instance.checkoutMealPeriodContract(
        draftId: d.draftId,
        depositPercent: _depositPercent,
      );

      if (!mounted) return;
      if (result.orderId <= 0) {
        setState(() => _error = 'Không tạo được đơn đặt cọc.');
        return;
      }

      await Navigator.of(context).pushNamed(
        AppRoutes.orgOrderAnnexSign,
        arguments: OrgOrderAnnexSignArgs(
          orderId: result.orderId,
          depositPercent: _depositPercent,
          depositAmountVnd: result.depositAmountVnd,
          totalAmount: d.totalAmount,
          organizationName: '',
          totalMainQuantity: d.mealsPerDay,
          fromCheckout: true,
          usePeriodContractPay: true,
        ),
      );
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = orgApiError(e));
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Rà soát hợp đồng',
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          OrgPageIntro(
            title: 'Bản nháp hợp đồng theo kỳ',
            description: 'HĐ #${d.contractId} · ${d.startDate} → ${d.endDate}',
            icon: Icons.fact_check_outlined,
          ),
          const SizedBox(height: 12),
          if (_error != null) ...[
            OrgInfoBanner(
              message: _error!,
              icon: Icons.error_outline,
              color: AppDesignSystem.danger,
            ),
            const SizedBox(height: 10),
          ],
          OrgCard(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const OrgSectionHeader(
                  title: 'Tóm tắt',
                  subtitle: 'Tổng tiền được tính theo số ngày phục vụ và số suất/ngày',
                ),
                const SizedBox(height: 10),
                OrgStatTile(
                  label: 'Số ngày phục vụ',
                  value: '${d.serviceDays} ngày',
                  icon: Icons.calendar_month_outlined,
                  color: orgAccent,
                ),
                const SizedBox(height: 8),
                OrgStatTile(
                  label: 'Số suất/ngày',
                  value: '${d.mealsPerDay} suất',
                  icon: Icons.people_alt_outlined,
                  color: AppDesignSystem.info,
                ),
                const SizedBox(height: 8),
                OrgStatTile(
                  label: 'Giá/suất',
                  value: formatOrgVnd(d.mealUnitPrice),
                  icon: Icons.local_offer_outlined,
                  color: RolePalette.organization.primaryAlt,
                ),
                const SizedBox(height: 8),
                OrgStatTile(
                  label: 'Tổng hợp đồng',
                  value: formatOrgVnd(d.totalAmount),
                  icon: Icons.receipt_long_outlined,
                  color: AppDesignSystem.success,
                ),
                if (d.excludedDates.isNotEmpty) ...[
                  const SizedBox(height: 10),
                  Text('Ngày không cung cấp:', style: AppDesignSystem.label()),
                  const SizedBox(height: 6),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: d.excludedDates
                        .map((x) => Chip(label: Text(x)))
                        .toList(),
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(height: 12),
          OrgCard(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const OrgSectionHeader(
                  title: 'Đặt cọc',
                  subtitle: 'Chọn % đặt cọc trước khi ký phụ lục và thanh toán',
                ),
                const SizedBox(height: 10),
                Wrap(
                  spacing: 10,
                  runSpacing: 10,
                  children: [20, 25, 30, 35, 50]
                      .map(
                        (p) => ChoiceChip(
                          label: Text('$p%'),
                          selected: _depositPercent == p,
                          onSelected: (_) => setState(() => _depositPercent = p),
                        ),
                      )
                      .toList(),
                ),
                const SizedBox(height: 10),
                OrgInfoBanner(
                  message:
                      'Sau khi checkout, bạn sẽ ký phụ lục đặt hàng và thanh toán cọc qua PayOS.',
                  icon: Icons.info_outline,
                  color: AppDesignSystem.info,
                ),
              ],
            ),
          ),
          const SizedBox(height: 14),
          FilledButton.icon(
            onPressed: _submitting ? null : _checkout,
            icon: _submitting
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.shopping_cart_checkout_outlined),
            label: Text(_submitting ? 'Đang checkout…' : 'Checkout & ký phụ lục'),
            style: FilledButton.styleFrom(
              backgroundColor: orgAccent,
              padding: const EdgeInsets.symmetric(vertical: 14),
            ),
          ),
          const SizedBox(height: 10),
          OutlinedButton(
            onPressed: () => Navigator.of(context).pushNamed(AppRoutes.orgContractSettlement),
            style: OutlinedButton.styleFrom(foregroundColor: orgAccent),
            child: const Text('Xem danh sách hợp đồng'),
          ),
        ],
      ),
    );
  }
}

