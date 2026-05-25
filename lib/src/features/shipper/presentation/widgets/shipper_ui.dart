import 'dart:typed_data';

import 'package:flutter/material.dart';

import '../../../../core/config/app_env.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../../../core/widgets/module_scroll.dart';
import '../../data/models/shipper_delivery_models.dart';

export '../../../../core/widgets/module_page_shell.dart';
export '../../../../core/widgets/module_scroll.dart' show ModuleListView;

const RolePalette kShipperRole = RolePalette.shipper;

Color get shipperAccent => kShipperRole.primary;

EdgeInsets shipperListPadding(BuildContext context) =>
    moduleListPadding(context, bottomBarInset: 0);

String shipperApiError(Object e) {
  if (e is ApiException) return e.message;
  return e.toString();
}

String formatShipperDateTime(DateTime utc) {
  final d = utc.toLocal();
  final dd = d.day.toString().padLeft(2, '0');
  final mm = d.month.toString().padLeft(2, '0');
  final hh = d.hour.toString().padLeft(2, '0');
  final mi = d.minute.toString().padLeft(2, '0');
  return '$dd/$mm/${d.year} $hh:$mi';
}

String formatShipperDate(DateTime utc) {
  final d = utc.toLocal();
  return '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
}

/// URL ảnh PoD từ BE (đường dẫn tương đối hoặc URL đầy đủ).
String? resolveShipperMediaUrl(String? url) {
  final raw = url?.trim();
  if (raw == null || raw.isEmpty) return null;
  if (raw.startsWith('http://') || raw.startsWith('https://')) return raw;
  final base = AppEnv.apiBaseUrl.replaceAll(RegExp(r'/+$'), '');
  final path = raw.startsWith('/') ? raw : '/$raw';
  return '$base$path';
}

Color shipperStatusColor(String status) {
  switch (status.toLowerCase()) {
    case 'completed':
      return AppDesignSystem.success;
    case 'in_transit':
      return kShipperRole.primary;
    case 'received':
      return AppDesignSystem.warning;
    case 'failed':
    case 'rejected':
      return AppDesignSystem.danger;
    default:
      return AppDesignSystem.gray500;
  }
}

// ─── Shell ───────────────────────────────────────────────────────────────────

class ShipperPageShell extends StatelessWidget {
  final String title;
  final Widget body;
  final List<Widget>? actions;
  final Future<void> Function()? onRefresh;

  const ShipperPageShell({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.onRefresh,
  });

  @override
  Widget build(BuildContext context) => ModulePageShell(
        title: title,
        role: kShipperRole,
        body: body,
        actions: actions,
        onRefresh: onRefresh,
      );
}

// ─── Module con — UI chuẩn Shipper ───────────────────────────────────────────

/// Banner dashboard — tổng quan ca giao hôm nay.
class ShipperWelcomeBanner extends StatelessWidget {
  final String shipperName;

  const ShipperWelcomeBanner({super.key, required this.shipperName});

  @override
  Widget build(BuildContext context) {
    final now = DateTime.now();
    final dateLabel =
        '${now.day.toString().padLeft(2, '0')}/${now.month.toString().padLeft(2, '0')}/${now.year}';

    return Container(
      margin: const EdgeInsets.fromLTRB(20, 14, 20, 0),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: kShipperRole.gradient,
        ),
        boxShadow: [
          BoxShadow(
            color: kShipperRole.primary.withValues(alpha: 0.28),
            blurRadius: 18,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Stack(
          children: [
            Positioned(
              right: -16,
              bottom: -16,
              child: Icon(
                Icons.local_shipping_rounded,
                size: 110,
                color: Colors.white.withValues(alpha: 0.12),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(18),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.white.withValues(alpha: 0.2),
                      borderRadius: BorderRadius.circular(999),
                      border: Border.all(color: Colors.white24),
                    ),
                    child: Text(
                      'Ca giao · $dateLabel',
                      style: AppDesignSystem.body(size: 11, color: Colors.white)
                          .copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  const SizedBox(height: 10),
                  Text(
                    'Xin chào, $shipperName',
                    style: AppDesignSystem.title(size: 22, color: Colors.white),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    'Theo dõi đơn, tối ưu tuyến OSM và cập nhật trạng thái giao — tất cả trên một màn hình.',
                    style: AppDesignSystem.body(size: 13, color: Colors.white.withValues(alpha: 0.92)),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// KPI nổi bật — tiến độ hoàn thành hôm nay.
class ShipperHeroKpiCard extends StatelessWidget {
  final String label;
  final String value;
  final String? hint;
  final double progressPercent;
  final bool outerMargin;

  const ShipperHeroKpiCard({
    super.key,
    required this.label,
    required this.value,
    this.hint,
    this.progressPercent = 0,
    this.outerMargin = true,
  });

  @override
  Widget build(BuildContext context) {
    final progress = (progressPercent / 100).clamp(0.0, 1.0);

    return Container(
      margin: outerMargin ? const EdgeInsets.fromLTRB(20, 12, 20, 0) : EdgeInsets.zero,
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: kShipperRole.primary.withValues(alpha: 0.2)),
        boxShadow: [
          BoxShadow(
            color: kShipperRole.primary.withValues(alpha: 0.1),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        children: [
          SizedBox(
            width: 64,
            height: 64,
            child: Stack(
              fit: StackFit.expand,
              children: [
                CircularProgressIndicator(
                  value: progress > 0 ? progress : null,
                  strokeWidth: 6,
                  backgroundColor: AppDesignSystem.gray100,
                  color: AppDesignSystem.success,
                ),
                Center(
                  child: Text(
                    progress > 0 ? '${progressPercent.round()}%' : '—',
                    style: AppDesignSystem.label(color: kShipperRole.primary).copyWith(fontSize: 13),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500)),
                const SizedBox(height: 4),
                Text(value, style: AppDesignSystem.title(size: 22, color: AppDesignSystem.gray900)),
                if (hint != null) ...[
                  const SizedBox(height: 4),
                  Text(hint!, style: AppDesignSystem.body(size: 12, color: kShipperRole.link)),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Ba chỉ số nhanh: chờ · đang giao · hoàn thành.
class ShipperOpsStrip extends StatelessWidget {
  final String pendingLabel;
  final String inTransitLabel;
  final String completedLabel;
  final bool outerMargin;

  const ShipperOpsStrip({
    super.key,
    required this.pendingLabel,
    required this.inTransitLabel,
    required this.completedLabel,
    this.outerMargin = true,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: outerMargin
          ? const EdgeInsets.fromLTRB(20, 12, 20, 0)
          : const EdgeInsets.only(top: 4),
      child: Row(
        children: [
          Expanded(
            child: _OpsMini(
              icon: Icons.hourglass_top_rounded,
              title: 'Chờ xử lý',
              value: pendingLabel,
              color: AppDesignSystem.warning,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _OpsMini(
              icon: Icons.local_shipping_rounded,
              title: 'Đang giao',
              value: inTransitLabel,
              color: kShipperRole.primary,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _OpsMini(
              icon: Icons.check_circle_rounded,
              title: 'Hoàn thành',
              value: completedLabel,
              color: AppDesignSystem.success,
            ),
          ),
        ],
      ),
    );
  }
}

class _OpsMini extends StatelessWidget {
  final IconData icon;
  final String title;
  final String value;
  final Color color;

  const _OpsMini({
    required this.icon,
    required this.title,
    required this.value,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 10),
      decoration: AppDesignSystem.card(radius: 14),
      child: Column(
        children: [
          Icon(icon, size: 20, color: color),
          const SizedBox(height: 6),
          Text(
            value,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: AppDesignSystem.label().copyWith(fontSize: 12),
          ),
          Text(title, style: AppDesignSystem.body(size: 9, color: AppDesignSystem.gray500)),
        ],
      ),
    );
  }
}

/// Card nội dung có header gradient (bản đồ, danh sách…).
class ShipperContentCard extends StatelessWidget {
  final String title;
  final String? subtitle;
  final Widget child;
  final Color accent;
  final IconData headerIcon;
  final Widget? trailing;

  const ShipperContentCard({
    super.key,
    required this.title,
    this.subtitle,
    required this.child,
    this.accent = const Color(0xFF2563EB),
    this.headerIcon = Icons.route_rounded,
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return ShipperCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Container(
            padding: const EdgeInsets.fromLTRB(16, 14, 16, 12),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [accent.withValues(alpha: 0.12), Colors.transparent],
              ),
              borderRadius: const BorderRadius.vertical(top: Radius.circular(16)),
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: accent.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(headerIcon, color: accent, size: 20),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(title, style: AppDesignSystem.sectionTitle()),
                      if (subtitle != null) ...[
                        const SizedBox(height: 2),
                        Text(subtitle!, style: AppDesignSystem.body(size: 12)),
                      ],
                    ],
                  ),
                ),
                if (trailing != null) trailing!,
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 0, 12, 16),
            child: child,
          ),
        ],
      ),
    );
  }
}

/// Thanh tiến trình trạng thái đơn giao (chi tiết đơn).
class ShipperDeliveryProgressBar extends StatelessWidget {
  final String status;

  const ShipperDeliveryProgressBar({super.key, required this.status});

  static const _steps = [
    _ProgressStep('Chờ nhận', Icons.inbox_outlined, {'pending', 'assigned'}),
    _ProgressStep('Đã nhận', Icons.inventory_2_outlined, {'received'}),
    _ProgressStep('Đang giao', Icons.local_shipping_outlined, {'in_transit'}),
    _ProgressStep('Hoàn tất', Icons.check_circle_outline, {'completed'}),
  ];

  int _activeIndex(String s) {
    final lower = s.toLowerCase();
    if (lower == 'failed' || lower == 'rejected') return 2;
    for (var i = 0; i < _steps.length; i++) {
      if (_steps[i].statuses.contains(lower)) return i;
    }
    return 0;
  }

  @override
  Widget build(BuildContext context) {
    final lower = status.toLowerCase();
    final failed = lower == 'failed' || lower == 'rejected';
    final active = _activeIndex(status);
    final accent = failed ? AppDesignSystem.danger : shipperAccent;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: AppDesignSystem.card(radius: 16).copyWith(
        border: Border.all(color: accent.withValues(alpha: 0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Icon(
                failed ? Icons.error_outline_rounded : Icons.timeline_rounded,
                size: 20,
                color: accent,
              ),
              const SizedBox(width: 8),
              Text('Tiến trình giao', style: AppDesignSystem.title(size: 15)),
              const Spacer(),
              ShipperStatusBadge(status: status),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: List.generate(_steps.length * 2 - 1, (i) {
              if (i.isOdd) {
                final seg = (i ~/ 2);
                final done = !failed && seg < active;
                return Expanded(
                  child: Container(
                    height: 3,
                    margin: const EdgeInsets.only(bottom: 22),
                    decoration: BoxDecoration(
                      color: done ? accent : AppDesignSystem.gray200,
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                );
              }
              final stepIndex = i ~/ 2;
              final step = _steps[stepIndex];
              final isActive = !failed && stepIndex == active;
              final isDone = !failed && stepIndex < active;
              final stepColor = failed && stepIndex == active
                  ? AppDesignSystem.danger
                  : (isActive || isDone ? accent : AppDesignSystem.gray400);

              return Expanded(
                child: Column(
                  children: [
                    Container(
                      width: 32,
                      height: 32,
                      decoration: BoxDecoration(
                        color: (isActive || isDone)
                            ? stepColor.withValues(alpha: 0.15)
                            : AppDesignSystem.gray100,
                        shape: BoxShape.circle,
                        border: Border.all(
                          color: isActive ? stepColor : AppDesignSystem.gray200,
                          width: isActive ? 2 : 1,
                        ),
                      ),
                      child: Icon(step.icon, size: 16, color: stepColor),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      step.label,
                      textAlign: TextAlign.center,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: AppDesignSystem.body(
                        size: 9,
                        color: isActive ? stepColor : AppDesignSystem.gray500,
                      ).copyWith(fontWeight: isActive ? FontWeight.w700 : FontWeight.w500),
                    ),
                  ],
                ),
              );
            }),
          ),
          if (failed) ...[
            const SizedBox(height: 10),
            ShipperInfoBanner(
              message: lower == 'rejected'
                  ? 'Đơn đã bị từ chối — không thể tiếp tục giao.'
                  : 'Giao hàng thất bại — xem ghi chú bên dưới.',
              icon: Icons.warning_amber_rounded,
              color: AppDesignSystem.danger,
            ),
          ],
        ],
      ),
    );
  }
}

class _ProgressStep {
  final String label;
  final IconData icon;
  final Set<String> statuses;

  const _ProgressStep(this.label, this.icon, this.statuses);
}

/// Địa chỉ giao nổi bật + nút Maps / sao chép.
class ShipperAddressHighlightCard extends StatelessWidget {
  final String address;
  final VoidCallback onOpenMaps;
  final VoidCallback onCopy;

  const ShipperAddressHighlightCard({
    super.key,
    required this.address,
    required this.onOpenMaps,
    required this.onCopy,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            kShipperRole.primary.withValues(alpha: 0.12),
            Colors.white,
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: kShipperRole.primary.withValues(alpha: 0.22)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(Icons.place_rounded, color: kShipperRole.primary, size: 22),
              const SizedBox(width: 8),
              Text('Điểm giao hàng', style: AppDesignSystem.title(size: 15)),
            ],
          ),
          const SizedBox(height: 10),
          Text(
            address,
            style: AppDesignSystem.label().copyWith(height: 1.35),
          ),
          const SizedBox(height: 14),
          ShipperPrimaryButton(
            label: 'Chỉ đường Google Maps',
            icon: Icons.navigation_rounded,
            onPressed: onOpenMaps,
          ),
          const SizedBox(height: 8),
          ShipperOutlineButton(
            label: 'Sao chép địa chỉ',
            icon: Icons.copy_outlined,
            onPressed: onCopy,
          ),
        ],
      ),
    );
  }
}

class ShipperPageIntro extends StatelessWidget {
  final String title;
  final String description;
  final IconData icon;

  const ShipperPageIntro({
    super.key,
    required this.title,
    required this.description,
    this.icon = Icons.local_shipping_rounded,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: AppDesignSystem.gray100),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: kShipperRole.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icon, color: kShipperRole.primary, size: 26),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: AppDesignSystem.sectionTitle()),
                const SizedBox(height: 4),
                Text(
                  description,
                  style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class ShipperSectionHeader extends StatelessWidget {
  final String title;
  final String? subtitle;
  final Widget? trailing;

  const ShipperSectionHeader({
    super.key,
    required this.title,
    this.subtitle,
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: AppDesignSystem.sectionTitle()),
              if (subtitle != null) ...[
                const SizedBox(height: 2),
                Text(subtitle!, style: AppDesignSystem.body(size: 12)),
              ],
            ],
          ),
        ),
        if (trailing != null) trailing!,
      ],
    );
  }
}

/// Ảnh minh chứng giao hàng (PoD) từ URL API.
class ShipperProofImage extends StatelessWidget {
  final String? imageUrl;
  final double height;

  const ShipperProofImage({super.key, this.imageUrl, this.height = 200});

  @override
  Widget build(BuildContext context) {
    final url = resolveShipperMediaUrl(imageUrl);
    if (url == null) {
      return SizedBox(
        height: height,
        child: ColoredBox(
          color: AppDesignSystem.gray100,
          child: Center(
            child: Icon(Icons.image_not_supported_outlined,
                color: AppDesignSystem.gray400, size: 40),
          ),
        ),
      );
    }
    return ClipRRect(
      borderRadius: BorderRadius.circular(12),
      child: SizedBox(
        height: height,
        width: double.infinity,
        child: Image.network(
          url,
          fit: BoxFit.cover,
          errorBuilder: (_, __, ___) => ColoredBox(
            color: AppDesignSystem.gray100,
            child: Icon(Icons.broken_image_outlined,
                color: AppDesignSystem.gray400, size: 36),
          ),
        ),
      ),
    );
  }
}

class ShipperStatusBadge extends StatelessWidget {
  final String status;

  const ShipperStatusBadge({super.key, required this.status});

  @override
  Widget build(BuildContext context) {
    final color = shipperStatusColor(status);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: color.withValues(alpha: 0.35)),
      ),
      child: Text(
        shipperDeliveryStatusLabelVi(status),
        style: AppDesignSystem.body(size: 11, color: color)
            .copyWith(fontWeight: FontWeight.w700),
      ),
    );
  }
}

/// @deprecated Dùng [ShipperStatusBadge].
typedef ShipperStatusChip = ShipperStatusBadge;

class ShipperDataRow extends StatelessWidget {
  final IconData icon;
  final Color iconColor;
  final String title;
  final String? subtitle;
  final String? trailing;
  final Widget? badge;
  final VoidCallback? onTap;

  const ShipperDataRow({
    super.key,
    required this.icon,
    required this.iconColor,
    required this.title,
    this.subtitle,
    this.trailing,
    this.badge,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
          child: Ink(
            decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg),
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: iconColor.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Icon(icon, color: iconColor, size: 22),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Expanded(
                            child: Text(
                              title,
                              style: AppDesignSystem.label(),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                          if (badge != null) ...[const SizedBox(width: 8), badge!],
                        ],
                      ),
                      if (subtitle != null) ...[
                        const SizedBox(height: 4),
                        Text(
                          subtitle!,
                          style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                          maxLines: 3,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ],
                  ),
                ),
                if (trailing != null) ...[
                  const SizedBox(width: 8),
                  Text(trailing!, style: AppDesignSystem.label(color: shipperAccent)),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class ShipperDetailField extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;
  final Widget? trailing;

  const ShipperDetailField({
    super.key,
    required this.icon,
    required this.label,
    required this.value,
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: shipperAccent.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(icon, size: 20, color: shipperAccent),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500)),
                Text(value, style: AppDesignSystem.label()),
              ],
            ),
          ),
          if (trailing != null) trailing!,
        ],
      ),
    );
  }
}

class ShipperInfoBanner extends StatelessWidget {
  final String message;
  final IconData icon;
  final Color color;

  const ShipperInfoBanner({
    super.key,
    required this.message,
    this.icon = Icons.info_outline_rounded,
    this.color = AppDesignSystem.warning,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withValues(alpha: 0.25)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: color, size: 22),
          const SizedBox(width: 10),
          Expanded(
            child: Text(message, style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700)),
          ),
        ],
      ),
    );
  }
}

class ShipperPrimaryButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final bool loading;
  final VoidCallback? onPressed;
  final Color? backgroundColor;

  const ShipperPrimaryButton({
    super.key,
    required this.label,
    this.icon = Icons.arrow_forward_rounded,
    this.loading = false,
    this.onPressed,
    this.backgroundColor,
  });

  @override
  Widget build(BuildContext context) {
    return FilledButton.icon(
      onPressed: loading ? null : onPressed,
      icon: loading
          ? const SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
            )
          : Icon(icon, size: 20),
      label: Text(label, style: AppDesignSystem.label(color: Colors.white)),
      style: FilledButton.styleFrom(
        backgroundColor: backgroundColor ?? kShipperRole.primary,
        disabledBackgroundColor: kShipperRole.primary.withValues(alpha: 0.5),
        minimumSize: const Size.fromHeight(48),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
    );
  }
}

class ShipperOutlineButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback? onPressed;
  final Color? color;

  const ShipperOutlineButton({
    super.key,
    required this.label,
    required this.icon,
    this.onPressed,
    this.color,
  });

  @override
  Widget build(BuildContext context) {
    final c = color ?? AppDesignSystem.danger;
    return OutlinedButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, color: c, size: 20),
      label: Text(label, style: TextStyle(color: c, fontWeight: FontWeight.w600)),
      style: OutlinedButton.styleFrom(
        minimumSize: const Size.fromHeight(48),
        side: BorderSide(color: c),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
    );
  }
}

class ShipperStatTile extends ModuleStatTile {
  const ShipperStatTile({
    super.key,
    required super.label,
    required super.value,
    required super.icon,
    required super.color,
  });
}

class ShipperPeriodChips extends StatelessWidget {
  final List<String> labels;
  final int selected;
  final ValueChanged<int> onSelected;

  const ShipperPeriodChips({
    super.key,
    required this.labels,
    required this.selected,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) => ModulePeriodChips(
        labels: labels,
        selected: selected,
        onSelected: onSelected,
        role: kShipperRole,
      );
}

class ShipperCard extends StatelessWidget {
  final Widget child;
  final EdgeInsetsGeometry? padding;
  final VoidCallback? onTap;

  const ShipperCard({
    super.key,
    required this.child,
    this.padding,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final card = ModuleCard(
      padding: padding ?? const EdgeInsets.all(16),
      child: child,
    );
    if (onTap == null) return card;
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        child: card,
      ),
    );
  }
}

class ShipperDeliveryTile extends StatelessWidget {
  final ShipperDeliveryListItemModel item;
  final VoidCallback? onTap;
  final int? sequence;

  const ShipperDeliveryTile({
    super.key,
    required this.item,
    this.onTap,
    this.sequence,
  });

  @override
  Widget build(BuildContext context) {
    final statusColor = shipperStatusColor(item.deliveryStatus);

    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
          child: Ink(
            decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg).copyWith(
              border: Border.all(color: statusColor.withValues(alpha: 0.2)),
            ),
            child: IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Container(
                    width: 5,
                    decoration: BoxDecoration(
                      color: statusColor,
                      borderRadius: const BorderRadius.horizontal(
                        left: Radius.circular(16),
                      ),
                    ),
                  ),
                  Expanded(
                    child: Padding(
                      padding: const EdgeInsets.fromLTRB(14, 14, 14, 14),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              if (sequence != null) ...[
                                CircleAvatar(
                                  radius: 13,
                                  backgroundColor: shipperAccent,
                                  child: Text(
                                    '$sequence',
                                    style: AppDesignSystem.label(color: Colors.white)
                                        .copyWith(fontSize: 11),
                                  ),
                                ),
                                const SizedBox(width: 8),
                              ],
                              Expanded(
                                child: Text(
                                  'Đơn #${item.orderId}',
                                  style: AppDesignSystem.sectionTitle(),
                                ),
                              ),
                              ShipperStatusBadge(status: item.deliveryStatus),
                            ],
                          ),
                          const SizedBox(height: 10),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Icon(Icons.location_on_outlined,
                                  size: 18, color: statusColor),
                              const SizedBox(width: 6),
                              Expanded(
                                child: Text(
                                  item.deliveryAddress,
                                  maxLines: 2,
                                  overflow: TextOverflow.ellipsis,
                                  style: AppDesignSystem.body(size: 13),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 10),
                          Wrap(
                            spacing: 8,
                            runSpacing: 6,
                            children: [
                              _MetaChip(
                                icon: Icons.schedule_rounded,
                                label: formatShipperDateTime(item.scheduledDateUtc),
                              ),
                              _MetaChip(
                                icon: Icons.restaurant_rounded,
                                label: '${item.mealCount} suất',
                                color: shipperAccent,
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          Row(
                            children: [
                              Text(
                                'Xem chi tiết',
                                style: AppDesignSystem.label(color: kShipperRole.link)
                                    .copyWith(fontSize: 12),
                              ),
                              Icon(Icons.arrow_forward_rounded,
                                  size: 16, color: kShipperRole.link),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _MetaChip extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color? color;

  const _MetaChip({required this.icon, required this.label, this.color});

  @override
  Widget build(BuildContext context) {
    final c = color ?? AppDesignSystem.gray700;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: c.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 13, color: c),
          const SizedBox(width: 4),
          Text(label, style: AppDesignSystem.body(size: 11, color: c)),
        ],
      ),
    );
  }
}

/// Header đơn — mã đơn + trạng thái nổi bật (chi tiết giao).
class ShipperOrderHeaderCard extends StatelessWidget {
  final int orderId;
  final String status;
  final String? subtitle;

  const ShipperOrderHeaderCard({
    super.key,
    required this.orderId,
    required this.status,
    this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    final color = shipperStatusColor(status);
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [color.withValues(alpha: 0.14), Colors.white],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: color.withValues(alpha: 0.28)),
        boxShadow: [
          BoxShadow(
            color: color.withValues(alpha: 0.1),
            blurRadius: 14,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.15),
              borderRadius: BorderRadius.circular(14),
            ),
            child: Icon(Icons.local_shipping_rounded, color: color, size: 32),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Đơn giao', style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500)),
                Text('#$orderId', style: AppDesignSystem.title(size: 26)),
                if (subtitle != null) ...[
                  const SizedBox(height: 4),
                  Text(subtitle!, style: AppDesignSystem.body(size: 13)),
                ],
              ],
            ),
          ),
          ShipperStatusBadge(status: status),
        ],
      ),
    );
  }
}

/// Chọn ngày giao (lịch trình).
class ShipperDatePickerCard extends StatelessWidget {
  final DateTime date;
  final int orderCount;
  final VoidCallback onTap;

  const ShipperDatePickerCard({
    super.key,
    required this.date,
    required this.orderCount,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Ink(
          padding: const EdgeInsets.all(16),
          decoration: AppDesignSystem.card(radius: 16).copyWith(
            border: Border.all(color: kShipperRole.primary.withValues(alpha: 0.22)),
          ),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [
                      kShipperRole.primary.withValues(alpha: 0.15),
                      kShipperRole.primaryAlt.withValues(alpha: 0.08),
                    ],
                  ),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(Icons.calendar_month_rounded, color: kShipperRole.primary, size: 28),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Ngày phục vụ', style: AppDesignSystem.body(size: 12)),
                    Text(formatShipperDate(date), style: AppDesignSystem.sectionTitle()),
                    const SizedBox(height: 4),
                    Text(
                      'Chạm để đổi ngày · $orderCount đơn',
                      style: AppDesignSystem.body(size: 12, color: kShipperRole.link),
                    ),
                  ],
                ),
              ),
              Icon(Icons.chevron_right_rounded, color: kShipperRole.primary),
            ],
          ),
        ),
      ),
    );
  }
}

/// Khung chụp / chọn ảnh PoD.
class ShipperPhotoCaptureCard extends StatelessWidget {
  final Uint8List? imageBytes;
  final VoidCallback onCamera;
  final VoidCallback onGallery;
  final bool enabled;

  const ShipperPhotoCaptureCard({
    super.key,
    this.imageBytes,
    required this.onCamera,
    required this.onGallery,
    this.enabled = true,
  });

  @override
  Widget build(BuildContext context) {
    return ShipperContentCard(
      title: 'Ảnh minh chứng',
      subtitle: 'Chụp tại điểm giao hoặc chọn từ thư viện',
      accent: kShipperRole.primary,
      child: Column(
        children: [
          AspectRatio(
            aspectRatio: 4 / 3,
            child: DecoratedBox(
              decoration: BoxDecoration(
                color: AppDesignSystem.gray50,
                borderRadius: BorderRadius.circular(14),
                border: Border.all(
                  color: imageBytes != null
                      ? AppDesignSystem.success.withValues(alpha: 0.4)
                      : AppDesignSystem.gray200,
                  width: imageBytes != null ? 2 : 1,
                ),
              ),
              child: imageBytes == null
                  ? Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.add_a_photo_rounded,
                            size: 52, color: kShipperRole.primary.withValues(alpha: 0.5)),
                        const SizedBox(height: 10),
                        Text('Chưa có ảnh PoD', style: AppDesignSystem.body()),
                        Text(
                          'Bắt buộc trước khi hoàn tất',
                          style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                        ),
                      ],
                    )
                  : ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Image.memory(imageBytes!, fit: BoxFit.cover),
                    ),
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: ShipperOutlineButton(
                  label: 'Chụp ảnh',
                  icon: Icons.camera_alt_rounded,
                  onPressed: enabled ? onCamera : null,
                  color: kShipperRole.primary,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: ShipperOutlineButton(
                  label: 'Thư viện',
                  icon: Icons.photo_library_rounded,
                  onPressed: enabled ? onGallery : null,
                  color: kShipperRole.primary,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

/// Tab bar con trong màn Shipper (Danh sách / Bản đồ).
class ShipperSubTabBar extends StatelessWidget implements PreferredSizeWidget {
  final TabController controller;
  final List<String> tabs;

  const ShipperSubTabBar({
    super.key,
    required this.controller,
    required this.tabs,
  });

  @override
  Size get preferredSize => const Size.fromHeight(48);

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: AppDesignSystem.card(radius: 14),
      child: TabBar(
        controller: controller,
        labelColor: kShipperRole.primary,
        unselectedLabelColor: AppDesignSystem.gray500,
        indicatorColor: kShipperRole.primary,
        indicatorWeight: 3,
        dividerColor: Colors.transparent,
        labelStyle: AppDesignSystem.label(color: kShipperRole.link),
        unselectedLabelStyle: AppDesignSystem.body(size: 13),
        tabs: tabs.map((t) => Tab(text: t)).toList(),
      ),
    );
  }
}

/// Hai nút phụ — bản đồ / danh sách.
class ShipperShortcutRow extends StatelessWidget {
  final String leftLabel;
  final IconData leftIcon;
  final VoidCallback onLeft;
  final String rightLabel;
  final IconData rightIcon;
  final VoidCallback onRight;

  const ShipperShortcutRow({
    super.key,
    required this.leftLabel,
    required this.leftIcon,
    required this.onLeft,
    required this.rightLabel,
    required this.rightIcon,
    required this.onRight,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: ShipperOutlineButton(
            label: leftLabel,
            icon: leftIcon,
            onPressed: onLeft,
            color: kShipperRole.primary,
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: ShipperOutlineButton(
            label: rightLabel,
            icon: rightIcon,
            onPressed: onRight,
            color: AppDesignSystem.success,
          ),
        ),
      ],
    );
  }
}

/// Empty state trong card — thông báo / placeholder.
class ShipperPlaceholderCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String message;
  final String? actionLabel;
  final VoidCallback? onAction;

  const ShipperPlaceholderCard({
    super.key,
    required this.icon,
    required this.title,
    required this.message,
    this.actionLabel,
    this.onAction,
  });

  @override
  Widget build(BuildContext context) {
    return ShipperCard(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            padding: const EdgeInsets.all(18),
            decoration: BoxDecoration(
              color: kShipperRole.primary.withValues(alpha: 0.08),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, size: 44, color: kShipperRole.primary.withValues(alpha: 0.75)),
          ),
          const SizedBox(height: 16),
          Text(title, style: AppDesignSystem.sectionTitle(), textAlign: TextAlign.center),
          const SizedBox(height: 8),
          Text(
            message,
            textAlign: TextAlign.center,
            style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray500),
          ),
          if (actionLabel != null && onAction != null) ...[
            const SizedBox(height: 20),
            ShipperPrimaryButton(label: actionLabel!, icon: Icons.arrow_forward_rounded, onPressed: onAction),
          ],
        ],
      ),
    );
  }
}

class ShipperLoadingBody extends StatelessWidget {
  final String? message;

  const ShipperLoadingBody({super.key, this.message});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(40),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(color: kShipperRole.primary),
            if (message != null) ...[
              const SizedBox(height: 16),
              Text(message!, style: AppDesignSystem.body(), textAlign: TextAlign.center),
            ],
          ],
        ),
      ),
    );
  }
}

class ShipperErrorBody extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const ShipperErrorBody({super.key, required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) => ModuleErrorBody(
        message: message,
        onRetry: onRetry,
        role: kShipperRole,
      );
}

typedef ShipperEmptyList = ModuleEmptyList;
