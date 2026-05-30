import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/org_order_annex_sign_args.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_pay_deposit.dart';
import '../../utils/org_meal_period_pay_deposit.dart';
import 'org_annex_pdf_page.dart';
import '../widgets/org_signature_pad.dart';
import '../widgets/organization_ui.dart';

/// Ký phụ lục đặt hàng — sau checkout, trước thanh toán PayOS (web `Profile/Contracts`).
class OrgOrderAnnexSignPage extends StatefulWidget {
  const OrgOrderAnnexSignPage({super.key, required this.args});

  final OrgOrderAnnexSignArgs args;

  @override
  State<OrgOrderAnnexSignPage> createState() => _OrgOrderAnnexSignPageState();
}

class _OrgOrderAnnexSignPageState extends State<OrgOrderAnnexSignPage> {
  int _step = 1;
  bool _submitting = false;
  bool _hasSig = false;
  String? _error;
  String? _signedAnnexPdfUrl;
  bool _loadingPdf = false;
  final _sigKey = GlobalKey<OrgSignaturePadState>();

  OrgOrderAnnexSignArgs get a => widget.args;

  Future<void> _submitSign() async {
    final dataUrl = await _sigKey.currentState?.exportDataUrl();
    if (dataUrl == null || dataUrl.length < 32) {
      setState(() => _error = 'Vui lòng ký tên trên khung bên dưới.');
      return;
    }
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final result = await OrgRepository.instance.signOrderAnnex(
        orderId: a.orderId,
        digitalSignature: dataUrl,
      );
      if (!mounted) return;
      setState(() {
        _signedAnnexPdfUrl = result.annexPdfUrl;
        _step = 3;
        _submitting = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _submitting = false;
      });
    }
  }

  Future<void> _openAnnexPreview({required bool signed}) async {
    setState(() {
      _loadingPdf = true;
      _error = null;
    });
    try {
      if (signed &&
          _signedAnnexPdfUrl != null &&
          _signedAnnexPdfUrl!.isNotEmpty) {
        if (!mounted) return;
        await Navigator.of(context).push<void>(
          MaterialPageRoute(
            builder: (_) => OrgAnnexPdfPage(
              title: 'Phụ lục đã ký',
              pdfUrl: _signedAnnexPdfUrl,
              subtitle:
                  'Đơn #${a.orderId} — bản PDF đã lưu sau khi ký (giống web).',
            ),
          ),
        );
      } else {
        final bytes =
            await OrgRepository.instance.getOrderAnnexPreviewPdf(a.orderId);
        if (!mounted) return;
        await Navigator.of(context).push<void>(
          MaterialPageRoute(
            builder: (_) => OrgAnnexPdfPage(
              title: signed ? 'Phụ lục đã ký' : 'Xem trước phụ lục',
              pdfBytes: bytes,
              subtitle: signed
                  ? 'Đơn #${a.orderId} — bản đã ký'
                  : 'Đơn #${a.orderId} — cùng bố cục bản ký chính thức (trước khi ký).',
            ),
          ),
        );
      }
    } catch (e) {
      if (mounted) setState(() => _error = orgApiError(e));
    } finally {
      if (mounted) setState(() => _loadingPdf = false);
    }
  }

  Future<void> _openPayOs() async {
    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      if (a.usePeriodContractPay) {
        await OrgMealPeriodPayDeposit.launch(
          context: context,
          orderId: a.orderId,
          depositAmountHint: a.depositAmountVnd.toDouble(),
        );
      } else {
        await OrgMealPayDeposit.launch(
          context: context,
          orderId: a.orderId,
          depositAmountHint: a.depositAmountVnd.toDouble(),
        );
      }
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Ký phụ lục đặt hàng',
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          OrgPageIntro(
            title: 'Cổng ký số HuitMeal',
            description: a.organizationName.isNotEmpty
                ? '${a.organizationName} — Đơn #${a.orderId}'
                : 'Đơn #${a.orderId} — ký phụ lục trước khi thanh toán cọc.',
            icon: Icons.draw_rounded,
          ),
          if (a.fromCheckout) ...[
            const SizedBox(height: 12),
            OrgInfoBanner(
              message:
                  'Đã tạo đơn hàng #${a.orderId}. Vui lòng ký phụ lục đặt hàng, '
                  'sau đó thanh toán đặt cọc qua PayOS bên dưới '
                  '(giống cổng ký trên web).',
              icon: Icons.check_circle_outline,
              color: const Color(0xFF0284C7),
            ),
          ],
          const SizedBox(height: 12),
          _buildStepIndicator(),
          const SizedBox(height: 16),
          if (_error != null) ...[
            OrgInfoBanner(
              message: _error!,
              icon: Icons.error_outline,
              color: AppDesignSystem.danger,
            ),
            const SizedBox(height: 12),
          ],
          if (_step == 1) _buildReviewStep(),
          if (_step == 2) _buildSignStep(),
          if (_step == 3) _buildDoneStep(),
        ],
      ),
    );
  }

  Widget _buildStepIndicator() {
    const labels = ['Rà soát', 'Ký tên', 'Thanh toán'];
    return Row(
      children: List.generate(3, (i) {
        final n = i + 1;
        final active = _step == n;
        final done = _step > n;
        return Expanded(
          child: Column(
            children: [
              CircleAvatar(
                radius: 14,
                backgroundColor: done
                    ? AppDesignSystem.success
                    : (active ? orgAccent : AppDesignSystem.gray200),
                child: done
                    ? const Icon(Icons.check, size: 16, color: Colors.white)
                    : Text(
                        '$n',
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w800,
                          color: active ? Colors.white : AppDesignSystem.gray500,
                        ),
                      ),
              ),
              const SizedBox(height: 4),
              Text(
                labels[i],
                style: AppDesignSystem.body(size: 10).copyWith(
                  fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                  color: active ? orgAccent : AppDesignSystem.gray500,
                ),
              ),
            ],
          ),
        );
      }),
    );
  }

  Widget _buildReviewStep() {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const OrgSectionHeader(
            title: 'Bước 1 — Rà soát phụ lục',
            subtitle: 'Xác nhận tổng tiền và tỷ lệ cọc trước khi ký',
          ),
          const SizedBox(height: 12),
          OrgStatTile(
            label: 'Tổng thanh toán',
            value: formatOrgVnd(a.totalAmount),
            icon: Icons.receipt_long_outlined,
            color: orgAccent,
          ),
          const SizedBox(height: 8),
          OrgStatTile(
            label: 'Đặt cọc ${a.depositPercent}%',
            value: formatOrgVnd(a.depositAmountVnd.toDouble()),
            icon: Icons.account_balance_wallet_outlined,
            color: AppDesignSystem.success,
          ),
          if (a.totalMainQuantity > 0) ...[
            const SizedBox(height: 8),
            Text(
              'Tổng ${a.totalMainQuantity} suất món chính',
              style: AppDesignSystem.body(size: 12),
            ),
          ],
          const SizedBox(height: 8),
          OrgInfoBanner(
            message:
                'Sau khi ký, hệ thống tạo PDF phụ lục và lưu trên cloud. '
                'Trạng đơn chuyển chờ thanh toán — bạn thanh toán cọc qua PayOS.',
            icon: Icons.info_outline,
            color: AppDesignSystem.info,
          ),
          const SizedBox(height: 12),
          OutlinedButton.icon(
            onPressed: _loadingPdf ? null : () => _openAnnexPreview(signed: false),
            icon: _loadingPdf
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.picture_as_pdf_outlined),
            label: Text(_loadingPdf ? 'Đang tải PDF…' : 'Xem trước PDF phụ lục'),
            style: OutlinedButton.styleFrom(
              foregroundColor: orgAccent,
              padding: const EdgeInsets.symmetric(vertical: 12),
            ),
          ),
          const SizedBox(height: 12),
          OrgPrimaryButton(
            label: 'Tiếp tục — ký tên',
            icon: Icons.arrow_forward_rounded,
            onPressed: () => setState(() {
              _step = 2;
              _error = null;
            }),
          ),
        ],
      ),
    );
  }

  Widget _buildSignStep() {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const OrgSectionHeader(
            title: 'Bước 2 — Ký xác nhận phụ lục',
            subtitle: 'Ký bằng ngón tay hoặc bút trên màn hình',
          ),
          const SizedBox(height: 8),
          OutlinedButton.icon(
            onPressed: _loadingPdf ? null : () => _openAnnexPreview(signed: false),
            icon: const Icon(Icons.visibility_outlined, size: 18),
            label: const Text('Xem lại bản xem trước PDF'),
          ),
          const SizedBox(height: 12),
          OrgSignaturePad(
            key: _sigKey,
            onSignatureChanged: (ok) => setState(() => _hasSig = ok),
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: _submitting
                      ? null
                      : () => setState(() {
                            _step = 1;
                            _error = null;
                          }),
                  child: const Text('Quay lại'),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                flex: 2,
                child: FilledButton.icon(
                  onPressed: _submitting || !_hasSig ? null : _submitSign,
                  icon: _submitting
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: Colors.white,
                          ),
                        )
                      : const Icon(Icons.draw_rounded),
                  label: Text(_submitting ? 'Đang lưu…' : 'Hoàn tất ký'),
                  style: FilledButton.styleFrom(
                    backgroundColor: orgAccent,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildDoneStep() {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          Icon(Icons.verified_rounded, size: 48, color: AppDesignSystem.success),
          const SizedBox(height: 12),
          Text(
            'Đã ký phụ lục thành công',
            style: AppDesignSystem.sectionTitle(color: AppDesignSystem.success),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            'Đơn #${a.orderId} đang chờ thanh toán cọc ${a.depositPercent}% '
            '(${formatOrgVnd(a.depositAmountVnd.toDouble())}).',
            style: AppDesignSystem.body(size: 13),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 16),
          OutlinedButton.icon(
            onPressed: _loadingPdf ? null : () => _openAnnexPreview(signed: true),
            icon: const Icon(Icons.picture_as_pdf_outlined),
            label: Text(
              _loadingPdf
                  ? 'Đang tải PDF…'
                  : (_signedAnnexPdfUrl != null && _signedAnnexPdfUrl!.isNotEmpty
                      ? 'Xem / tải PDF phụ lục đã ký'
                      : 'Xem PDF phụ lục đã ký'),
            ),
            style: OutlinedButton.styleFrom(
              foregroundColor: orgAccent,
              padding: const EdgeInsets.symmetric(vertical: 12),
            ),
          ),
          const SizedBox(height: 12),
          OrgPrimaryButton(
            label: _submitting ? 'Đang mở PayOS…' : 'Thanh toán cọc — PayOS',
            icon: Icons.payment_rounded,
            loading: _submitting,
            onPressed: _submitting ? null : _openPayOs,
          ),
          const SizedBox(height: 10),
          TextButton(
            onPressed: () => Navigator.of(context).pushNamedAndRemoveUntil(
              AppRoutes.orgContractSettlement,
              (r) => r.settings.name == AppRoutes.orgHome || r.isFirst,
            ),
            child: const Text('Thanh toán sau — xem hợp đồng'),
          ),
        ],
      ),
    );
  }
}
