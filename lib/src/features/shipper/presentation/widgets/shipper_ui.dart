import 'package:flutter/material.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../data/models/shipper_delivery_models.dart';

export '../../../../core/widgets/module_page_shell.dart';

const RolePalette kShipperRole = RolePalette.shipper;

Color get shipperAccent => kShipperRole.primary;

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

class ShipperStatusChip extends StatelessWidget {
  final String status;

  const ShipperStatusChip({super.key, required this.status});

  @override
  Widget build(BuildContext context) {
    final color = shipperStatusColor(status);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        shipperDeliveryStatusLabelVi(status),
        style: AppDesignSystem.body(size: 11, color: color)
            .copyWith(fontWeight: FontWeight.w700),
      ),
    );
  }
}

class ShipperDeliveryTile extends StatelessWidget {
  final ShipperDeliveryListItemModel item;
  final VoidCallback? onTap;

  const ShipperDeliveryTile({super.key, required this.item, this.onTap});

  @override
  Widget build(BuildContext context) {
    return ShipperCard(
      onTap: onTap,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: kShipperRole.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(Icons.local_shipping_outlined, color: kShipperRole.primary),
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
                        'Đơn #${item.orderId}',
                        style: AppDesignSystem.sectionTitle(),
                      ),
                    ),
                    ShipperStatusChip(status: item.deliveryStatus),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  item.deliveryAddress,
                  style: AppDesignSystem.body(size: 13),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
                const SizedBox(height: 6),
                Row(
                  children: [
                    Icon(Icons.schedule, size: 14, color: AppDesignSystem.gray400),
                    const SizedBox(width: 4),
                    Text(
                      formatShipperDateTime(item.scheduledDateUtc),
                      style: AppDesignSystem.body(size: 12),
                    ),
                    const Spacer(),
                    Text(
                      '${item.mealCount} suất',
                      style: AppDesignSystem.label(color: kShipperRole.primary),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class ShipperLoadingBody extends StatelessWidget {
  const ShipperLoadingBody({super.key});

  @override
  Widget build(BuildContext context) =>
      ModuleLoadingBody(color: kShipperRole.primary);
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
