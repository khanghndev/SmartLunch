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
        gradient: LinearGradient(
          colors: [
            kShipperRole.primary.withValues(alpha: 0.12),
            kShipperRole.primaryAlt.withValues(alpha: 0.06),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: kShipperRole.primary.withValues(alpha: 0.2)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
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
    return ShipperDataRow(
      icon: sequence != null ? Icons.pin_drop_rounded : Icons.local_shipping_outlined,
      iconColor: shipperAccent,
      title: 'Đơn #${item.orderId}',
      subtitle: '${item.deliveryAddress}\n${formatShipperDateTime(item.scheduledDateUtc)}',
      trailing: '${item.mealCount} suất',
      badge: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (sequence != null) ...[
            CircleAvatar(
              radius: 12,
              backgroundColor: shipperAccent,
              child: Text(
                '$sequence',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 11,
                  fontWeight: FontWeight.w800,
                ),
              ),
            ),
            const SizedBox(width: 6),
          ],
          ShipperStatusBadge(status: item.deliveryStatus),
        ],
      ),
      onTap: onTap,
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
