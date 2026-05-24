import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/models/org_order_annex_sign_args.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_order_date_rules.dart';
import '../widgets/organization_ui.dart';

/// Xác nhận nháp & đặt hàng — khớp web `OrganizationMealOrder/Review.cshtml`.
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
  String? _error;

  PrepareMealDraftModel get draft => widget.draft;

  Map<String, List<MealDraftLineSummaryModel>> get _linesByDate {
    final map = <String, List<MealDraftLineSummaryModel>>{};
    for (final l in draft.lines) {
      map.putIfAbsent(l.serviceDate, () => []).add(l);
    }
    return map;
  }

  int _dayMainQty(List<MealDraftLineSummaryModel> lines) {
    return lines
        .where((l) => l.slot.toLowerCase() == 'main')
        .fold(0, (s, l) => s + l.quantity);
  }

  Future<void> _openContractPdf(String url) async {
    final uri = Uri.tryParse(url);
    if (uri == null) return;
    if (!await launchUrl(uri, mode: LaunchMode.externalApplication)) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không mở được liên kết PDF hợp đồng.'),
          backgroundColor: AppDesignSystem.danger,
        ),
      );
    }
  }

  Future<void> _checkout() async {
    if (draft.totalMainQuantity <= 0) {
      setState(() => _error = 'Đơn chưa có suất món chính. Vui lòng sửa thực đơn.');
      return;
    }
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final result = await OrgRepository.instance.checkoutMealOrder(
        draftId: draft.draftId,
        depositPercent: _depositPercent,
      );
      if (!mounted) return;

      final deposit = result.depositAmountVnd > 0
          ? result.depositAmountVnd
          : (draft.totalAmount * _depositPercent / 100).round();

      await Navigator.of(context).pushReplacementNamed(
        AppRoutes.orgOrderAnnexSign,
        arguments: OrgOrderAnnexSignArgs(
          orderId: result.orderId,
          depositPercent: _depositPercent,
          depositAmountVnd: deposit,
          totalAmount: draft.totalAmount,
          organizationName: draft.organizationName,
          totalMainQuantity: draft.totalMainQuantity,
        ),
      );
    } catch (e) {
      if (mounted) {
        setState(() => _error = orgApiError(e));
      }
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Xác nhận nháp',
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          OrgPageIntro(
            title: 'Xác nhận đặt suất',
            description: draft.organizationName.isNotEmpty
                ? '${draft.organizationName} — kiểm tra thực đơn và chọn tỷ lệ đặt cọc.'
                : 'Kiểm tra thực đơn và chọn tỷ lệ đặt cọc trước khi ký phụ lục.',
            icon: Icons.fact_check_rounded,
          ),
          if (_error != null) ...[
            const SizedBox(height: 12),
            OrgInfoBanner(
              message: _error!,
              icon: Icons.error_outline,
              color: AppDesignSystem.danger,
            ),
          ],
          if (draft.contractId > 0) ...[
            const SizedBox(height: 14),
            OrgCard(
              padding: const EdgeInsets.all(14),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Icon(Icons.file_download_done_rounded,
                          color: AppDesignSystem.success, size: 22),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          'Hợp đồng đã lưu vào hệ thống',
                          style: AppDesignSystem.label().copyWith(
                            color: AppDesignSystem.success,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Mã HĐ: ${draft.contractNumber ?? '#${draft.contractId}'}',
                    style: AppDesignSystem.body(size: 13),
                  ),
                  if (draft.persistenceNotice != null) ...[
                    const SizedBox(height: 6),
                    Text(
                      draft.persistenceNotice!,
                      style: AppDesignSystem.body(size: 11),
                    ),
                  ],
                  if (draft.contractFileUrl != null &&
                      draft.contractFileUrl!.isNotEmpty) ...[
                    const SizedBox(height: 12),
                    OutlinedButton.icon(
                      onPressed: () => _openContractPdf(draft.contractFileUrl!),
                      icon: const Icon(Icons.picture_as_pdf_outlined, size: 18),
                      label: const Text('Xem PDF hợp đồng (nháp)'),
                    ),
                  ],
                ],
              ),
            ),
          ] else if (draft.persistenceNotice != null) ...[
            const SizedBox(height: 14),
            OrgInfoBanner(
              message: draft.persistenceNotice!,
              icon: Icons.info_outline,
              color: AppDesignSystem.warning,
            ),
          ],
          if (draft.delivery != null) ...[
            const SizedBox(height: 14),
            _buildDeliveryCard(draft.delivery!),
          ],
          const SizedBox(height: 14),
          Row(
            children: [
              Expanded(
                child: OrgStatTile(
                  label: 'Giá/suất',
                  value: formatOrgVnd(draft.pricePerPortion),
                  icon: Icons.payments_outlined,
                  color: orgAccent,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: OrgStatTile(
                  label: 'Suất chính',
                  value: '${draft.totalMainQuantity}',
                  icon: Icons.rice_bowl_outlined,
                  color: AppDesignSystem.success,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          OrgStatTile(
            label: 'Tổng thanh toán',
            value: formatOrgVnd(draft.totalAmount),
            icon: Icons.receipt_long_rounded,
            color: orgAccent,
          ),
          if (draft.subtotalAmount != null &&
              draft.subtotalAmount! > draft.totalAmount) ...[
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Tạm tính', style: AppDesignSystem.body(size: 13)),
                Text(
                  formatOrgVnd(draft.subtotalAmount!),
                  style: AppDesignSystem.label(),
                ),
              ],
            ),
          ],
          if (draft.discountAmount > 0) ...[
            const SizedBox(height: 10),
            OrgInfoBanner(
              message:
                  'Giảm ${formatOrgVnd(draft.discountAmount)}'
                  '${draft.appliedPromotionName != null ? ' · ${draft.appliedPromotionName}' : ''}'
                  '${draft.promotionCode != null ? ' (${draft.promotionCode})' : ''}',
              icon: Icons.local_offer_outlined,
              color: AppDesignSystem.success,
            ),
          ],
          const SizedBox(height: 18),
          const OrgSectionHeader(title: 'Chi tiết thực đơn'),
          const SizedBox(height: 10),
          ..._linesByDate.entries.map((e) => _dayCard(e.key, e.value)),
          const SizedBox(height: 18),
          OrgCard(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const OrgSectionHeader(
                  title: 'Tỷ lệ đặt cọc',
                  subtitle:
                      'Chọn % cọc (20–50%). Sau khi đặt hàng bạn sẽ ký phụ lục, rồi thanh toán cọc PayOS.',
                ),
                const SizedBox(height: 12),
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
                          Text(
                            '$pct%',
                            style: const TextStyle(fontWeight: FontWeight.w800),
                          ),
                          Text(
                            formatOrgVnd(deposit.toDouble()),
                            style: const TextStyle(fontSize: 10),
                          ),
                        ],
                      ),
                      selected: selected,
                      onSelected: (_) => setState(() => _depositPercent = pct),
                      selectedColor: orgAccent.withValues(alpha: 0.2),
                      labelStyle: TextStyle(
                        color: selected ? orgAccent : AppDesignSystem.gray700,
                      ),
                    );
                  }).toList(),
                ),
                const SizedBox(height: 20),
                OrgPrimaryButton(
                  label: _submitting
                      ? 'Đang tạo đơn & gửi email…'
                      : 'Đặt hàng — chuyển ký phụ lục',
                  icon: Icons.draw_rounded,
                  loading: _submitting,
                  onPressed: _submitting ? null : _checkout,
                ),
                const SizedBox(height: 8),
                Text(
                  'Bằng việc xác nhận, bạn đồng ý với điều khoản cung cấp suất ăn của HuitMeal.',
                  style: AppDesignSystem.body(size: 11),
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDeliveryCard(OrganizationMealDeliverySummaryModel d) {
    return OrgCard(
      padding: const EdgeInsets.all(14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Giao hàng & người nhận',
            style: AppDesignSystem.label().copyWith(color: AppDesignSystem.success),
          ),
          const SizedBox(height: 12),
          _deliveryRow('Người nhận', d.recipientName),
          _deliveryRow('Số điện thoại', d.recipientPhone),
          _deliveryRow('Email thông báo', d.recipientEmail),
          _deliveryRow('Địa chỉ giao', d.displayAddress),
          if (d.preferredDeliveryTime != null &&
              d.preferredDeliveryTime!.isNotEmpty)
            _deliveryRow('Giờ giao', d.preferredDeliveryTime!),
          if (d.deliveryNotes != null && d.deliveryNotes!.isNotEmpty)
            _deliveryRow('Ghi chú', d.deliveryNotes!),
        ],
      ),
    );
  }

  Widget _deliveryRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label.toUpperCase(),
            style: AppDesignSystem.body(size: 10, color: AppDesignSystem.gray500),
          ),
          const SizedBox(height: 2),
          Text(value, style: AppDesignSystem.body(size: 13)),
        ],
      ),
    );
  }

  Widget _dayCard(String iso, List<MealDraftLineSummaryModel> lines) {
    final mainQty = _dayMainQty(lines);
    final dayAmount = (draft.pricePerPortion * mainQty).round();

    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: OrgCard(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              OrgMealOrderDateRules.formatDisplay(iso),
              style: AppDesignSystem.label(),
            ),
            const SizedBox(height: 8),
            ...lines.map(
              (l) => Padding(
                padding: const EdgeInsets.only(bottom: 6),
                child: Row(
                  children: [
                    OrgStatusBadge(label: l.slotLabel, color: orgAccent),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(l.dishName, style: AppDesignSystem.body(size: 13)),
                    ),
                    Text('×${l.quantity}', style: AppDesignSystem.label()),
                  ],
                ),
              ),
            ),
            if (mainQty > 0) ...[
              const SizedBox(height: 8),
              Text(
                'Thành tiền ngày: ${formatOrgVnd(dayAmount.toDouble())} '
                '($mainQty suất × ${formatOrgVnd(draft.pricePerPortion)})',
                style: AppDesignSystem.body(size: 11).copyWith(
                  color: orgAccent,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
