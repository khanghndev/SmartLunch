import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../auth/data/auth_storage.dart';
import '../../data/models/customer_review_models.dart';
import '../../data/org_repository.dart';
import '../widgets/organization_ui.dart';

/// Đánh giá suất ăn — khớp web `Customer/Reviews` + `CustomerController.Reviews`.
class OrgReviewsPage extends StatefulWidget {
  final bool embeddedInModuleShell;

  const OrgReviewsPage({
    super.key,
    this.embeddedInModuleShell = false,
  });

  @override
  State<OrgReviewsPage> createState() => _OrgReviewsPageState();
}

class _OrgReviewsPageState extends State<OrgReviewsPage> {
  List<PublicReviewModel> _reviews = [];
  ReviewMeContextModel? _context;
  bool _contextLoaded = false;
  bool _isLoggedIn = false;
  double _avgRating = 0;
  int _totalCount = 0;
  bool _loading = true;
  String? _publicLoadError;

  int? _selectedOrderId;
  int _rating = 5;
  final _commentCtrl = TextEditingController();
  bool _submitting = false;
  String? _submitError;
  bool _submitSuccess = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void dispose() {
    _commentCtrl.dispose();
    super.dispose();
  }

  /// Web: public reviews luôn tải; `/me` chỉ khi có token, lỗi không chặn trang.
  Future<void> _load() async {
    setState(() {
      _loading = true;
      _publicLoadError = null;
      _submitSuccess = false;
      _submitError = null;
    });

    PublicReviewsPageModel? pub;
    String? pubError;
    try {
      pub = await OrgRepository.instance.getPublicReviews();
    } catch (e) {
      pubError = orgApiError(e);
    }

    ReviewMeContextModel? ctx;
    final session = await const AuthStorage().readSession();
    final loggedIn = session != null;
    if (loggedIn) {
      try {
        ctx = await OrgRepository.instance.getReviewMeContext();
      } catch (_) {
        // Khớp web: log warning, vẫn hiển thị danh sách công khai.
      }
    }

    if (!mounted) return;
    setState(() {
      _reviews = pub?.reviews ?? [];
      _avgRating = pub?.averageRating ?? 0;
      _totalCount = pub?.totalCount ?? 0;
      _publicLoadError = pubError;
      _context = ctx;
      _contextLoaded = loggedIn;
      _isLoggedIn = loggedIn;
      _loading = false;
      // Không tự chọn đơn — web mặc định "-- Chọn đơn --".
      if (ctx?.canSubmitReview != true) {
        _selectedOrderId = null;
      }
    });
  }

  Future<void> _submit() async {
    final orderId = _selectedOrderId;
    final comment = _commentCtrl.text.trim();
    if (orderId == null) {
      setState(() => _submitError = 'Vui lòng chọn đơn hàng');
      return;
    }
    if (comment.isEmpty) {
      setState(() => _submitError = 'Vui lòng nhập nội dung đánh giá');
      return;
    }
    setState(() {
      _submitting = true;
      _submitError = null;
      _submitSuccess = false;
    });
    try {
      await OrgRepository.instance.submitCustomerReview(
        orderId: orderId,
        rating: _rating,
        comment: comment,
      );
      if (!mounted) return;
      setState(() {
        _submitting = false;
        _submitSuccess = true;
        _commentCtrl.clear();
        _rating = 5;
        _selectedOrderId = null;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: const Text('Cảm ơn bạn! Đánh giá đã được gửi.'),
          behavior: SnackBarBehavior.floating,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        ),
      );
      await Future<void>.delayed(const Duration(milliseconds: 1500));
      if (mounted) await _load();
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _submitting = false;
        _submitError = orgApiError(e);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đánh giá suất ăn',
      onRefresh: _load,
      embeddedInModuleShell: widget.embeddedInModuleShell,
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tải đánh giá…')
          : ModuleListView(
              padding: orgListPadding(context),
              children: [
                const OrgPageIntro(
                  title: 'Tiếng nói đối tác',
                  description:
                      'Bữa ăn chất lượng là phúc lợi giữ chân nhân tài. Xem ý kiến đối tác và gửi đánh giá về suất ăn, giao hàng, dịch vụ.',
                  icon: Icons.star_rounded,
                ),
                const SizedBox(height: 14),
                Row(
                  children: [
                    Expanded(
                      child: OrgStatTile(
                        label: 'Điểm trung bình',
                        value: _avgRating > 0
                            ? '${_avgRating.toStringAsFixed(1)}/5.0'
                            : '—',
                        icon: Icons.star_rounded,
                        color: kOrgRole.primary,
                      ),
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: OrgStatTile(
                        label: 'Tổng đánh giá',
                        value: '$_totalCount',
                        icon: Icons.forum_outlined,
                        color: kOrgRole.primaryAlt,
                      ),
                    ),
                  ],
                ),
                if (_publicLoadError != null) ...[
                  const SizedBox(height: 12),
                  OrgInfoBanner(
                    message: _publicLoadError!,
                    icon: Icons.error_outline_rounded,
                    color: AppDesignSystem.danger,
                  ),
                ],
                const SizedBox(height: 14),
                _buildSubmitSection(),
                const SizedBox(height: 8),
                const OrgSectionHeader(title: 'Họ nói gì về HuitMeal?'),
                const SizedBox(height: 8),
                if (_reviews.isEmpty)
                  const OrgCard(
                    child: OrgEmptyList(
                      message:
                          'Chưa có đánh giá nào từ đối tác doanh nghiệp.',
                    ),
                  )
                else
                  ..._reviews.map(_reviewCard),
                const SizedBox(height: 24),
              ],
            ),
    );
  }

  Widget _buildSubmitSection() {
    // Khớp web/BE: chỉ đơn đã giao hoặc confirmed+paid, chưa đánh giá.
    final orders = _context?.reviewableOrders ?? const <ReviewableOrderModel>[];
    final canSubmit = _context?.canSubmitReview == true && orders.isNotEmpty;

    if (canSubmit) {
      return OrgCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              'Gửi đánh giá của đơn vị bạn',
              style: AppDesignSystem.sectionTitle(),
            ),
            const SizedBox(height: 6),
            Text(
              'Chỉ áp dụng cho khách hàng doanh nghiệp đã đặt suất ăn và đơn đủ điều kiện (đã giao hoặc đã thanh toán).',
              style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray500),
            ),
            const SizedBox(height: 16),
            Text('Chọn đơn hàng', style: AppDesignSystem.label().copyWith(fontSize: 13)),
            const SizedBox(height: 8),
            DropdownButtonFormField<int?>(
              isExpanded: true,
              value: _selectedOrderId,
              decoration: InputDecoration(
                filled: true,
                fillColor: AppDesignSystem.gray50,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(color: AppDesignSystem.gray200, width: 1.5),
                ),
                contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
              ),
              hint: Text(
                '-- Chọn đơn --',
                style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray500),
                overflow: TextOverflow.ellipsis,
              ),
              items: orders
                  .map(
                    (o) => DropdownMenuItem<int?>(
                      value: o.orderId,
                      child: Text(
                        o.displayLabel,
                        style: AppDesignSystem.body(size: 13),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  )
                  .toList(),
              selectedItemBuilder: (context) => orders
                  .map(
                    (o) => Align(
                      alignment: Alignment.centerLeft,
                      child: Text(
                        o.displayLabelShort,
                        style: AppDesignSystem.body(size: 13),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  )
                  .toList(),
              onChanged: (v) => setState(() {
                _selectedOrderId = v;
                _submitError = null;
              }),
            ),
            const SizedBox(height: 16),
            Text('Số sao', style: AppDesignSystem.label().copyWith(fontSize: 13)),
            const SizedBox(height: 8),
            Row(
              children: List.generate(5, (i) {
                final star = i + 1;
                return IconButton(
                  padding: EdgeInsets.zero,
                  constraints: const BoxConstraints(minWidth: 40, minHeight: 40),
                  onPressed: () => setState(() => _rating = star),
                  icon: Icon(
                    star <= _rating ? Icons.star_rounded : Icons.star_border_rounded,
                    color: star <= _rating
                        ? AppDesignSystem.warning
                        : AppDesignSystem.gray400,
                    size: 32,
                  ),
                );
              }),
            ),
            const SizedBox(height: 12),
            Text('Nội dung', style: AppDesignSystem.label().copyWith(fontSize: 13)),
            const SizedBox(height: 8),
            TextField(
              controller: _commentCtrl,
              maxLines: 4,
              decoration: InputDecoration(
                hintText:
                    'Chia sẻ trải nghiệm về chất lượng suất ăn, giao hàng, dịch vụ...',
                filled: true,
                fillColor: AppDesignSystem.gray50,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(color: AppDesignSystem.gray200, width: 1.5),
                ),
              ),
            ),
            if (_submitError != null) ...[
              const SizedBox(height: 10),
              Text(
                _submitError!,
                style: AppDesignSystem.body(size: 13, color: AppDesignSystem.danger),
              ),
            ],
            if (_submitSuccess) ...[
              const SizedBox(height: 10),
              Text(
                'Cảm ơn bạn! Đánh giá đã được gửi.',
                style: AppDesignSystem.body(size: 13, color: AppDesignSystem.success),
              ),
            ],
            const SizedBox(height: 16),
            OrgPrimaryButton(
              label: 'Gửi đánh giá',
              icon: Icons.send_rounded,
              loading: _submitting,
              onPressed: _submit,
            ),
          ],
        ),
      );
    }

    final defaultMsg =
        'Bạn có thể xem đánh giá từ các đối tác doanh nghiệp bên dưới. Chỉ tài khoản doanh nghiệp đã đặt suất ăn mới được gửi đánh giá.';
    final message = _context?.message ?? defaultMsg;

    return OrgCard(
      child: Column(
        children: [
          Icon(Icons.info_outline_rounded, size: 40, color: AppDesignSystem.gray400),
          const SizedBox(height: 12),
          Text(
            message,
            textAlign: TextAlign.center,
            style: AppDesignSystem.body(size: 14, color: AppDesignSystem.gray700),
          ),
          if (!_isLoggedIn) ...[
            const SizedBox(height: 16),
            TextButton(
              onPressed: () {
                Navigator.of(context).pushNamedAndRemoveUntil(
                  AppRoutes.login,
                  (route) => false,
                );
              },
              child: Text(
                'Đăng nhập tài khoản doanh nghiệp',
                style: AppDesignSystem.label(color: kOrgRole.primary),
              ),
            ),
          ] else if (_contextLoaded && _context == null) ...[
            const SizedBox(height: 12),
            Text(
              'Không tải được thông tin đơn đánh giá. Kéo xuống để thử lại.',
              textAlign: TextAlign.center,
              style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
            ),
          ],
        ],
      ),
    );
  }

  Widget _reviewCard(PublicReviewModel r) {
    final initial = r.authorName.trim().isNotEmpty
        ? r.authorName.trim()[0].toUpperCase()
        : 'K';
    final dateStr =
        r.createdAt != null ? formatReviewDate(r.createdAt!) : '';

    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: OrgCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                ...List.generate(
                  5,
                  (i) => Icon(
                    i < r.rating ? Icons.star_rounded : Icons.star_border_rounded,
                    size: 18,
                    color: i < r.rating
                        ? AppDesignSystem.warning
                        : AppDesignSystem.gray400,
                  ),
                ),
                const SizedBox(width: 8),
                Text(
                  '${r.rating}',
                  style: AppDesignSystem.label().copyWith(fontSize: 12),
                ),
              ],
            ),
            if (r.comment.isNotEmpty) ...[
              const SizedBox(height: 10),
              Text(
                '"${r.comment}"',
                style: AppDesignSystem.body(size: 14).copyWith(
                  fontStyle: FontStyle.italic,
                  height: 1.45,
                ),
              ),
            ],
            if (r.managerReply != null && r.managerReply!.trim().isNotEmpty) ...[
              const SizedBox(height: 12),
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: AppDesignSystem.info.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: AppDesignSystem.info.withValues(alpha: 0.2)),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Phản hồi từ HuitMeal',
                      style: AppDesignSystem.label(color: AppDesignSystem.info)
                          .copyWith(fontSize: 11),
                    ),
                    const SizedBox(height: 4),
                    Text(r.managerReply!, style: AppDesignSystem.body(size: 13)),
                  ],
                ),
              ),
            ],
            const SizedBox(height: 14),
            Row(
              children: [
                CircleAvatar(
                  radius: 22,
                  backgroundColor: kOrgRole.primary,
                  child: Text(
                    initial,
                    style: AppDesignSystem.label(color: Colors.white)
                        .copyWith(fontSize: 16),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(r.authorName, style: AppDesignSystem.label()),
                      const SizedBox(height: 2),
                      Text(
                        [
                          if (r.organizationName.isNotEmpty) r.organizationName,
                          if (dateStr.isNotEmpty) dateStr,
                        ].join(' · '),
                        style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
