import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';

class NotificationsPage extends StatelessWidget {
  const NotificationsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final today = [
      const _AppNotification(
        title: 'Đơn #SL1024 đã giao',
        subtitle: 'Shipper đã giao đến lễ tân. Nhấn xem chi tiết.',
        time: '3 phút trước',
        type: NotificationType.success,
        unread: true,
      ),
      const _AppNotification(
        title: 'Ưu đãi mới: -25%',
        subtitle: 'Giảm giá cho combo trưa trong 3 ngày tới.',
        time: '1 giờ trước',
        type: NotificationType.promo,
        unread: true,
      ),
    ];

    final earlier = [
      const _AppNotification(
        title: 'Đơn #SL0990 đang chuẩn bị',
        subtitle: 'Nhà bếp đã nhận, dự kiến giao 11:45.',
        time: 'Hôm qua',
        type: NotificationType.info,
      ),
      const _AppNotification(
        title: 'Nhắc đánh giá bữa trưa',
        subtitle: 'Chia sẻ cảm nhận để nhận thêm điểm thưởng.',
        time: 'Hôm qua',
        type: NotificationType.reminder,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Thông báo'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            onPressed: () {},
            icon: const Icon(Icons.settings_outlined),
          ),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            const _FilterRow(),
            const SizedBox(height: 14),
            _Section(title: 'Hôm nay', items: today),
            const SizedBox(height: 14),
            _Section(title: 'Trước đó', items: earlier),
            const SizedBox(height: 12),
            TextButton.icon(
              onPressed: () {},
              icon: const Icon(Icons.history_rounded),
              label: const Text('Xem tất cả thông báo'),
            ),
          ],
        ),
      ),
    );
  }
}

class _FilterRow extends StatelessWidget {
  const _FilterRow();

  @override
  Widget build(BuildContext context) {
    final filters = [
      _FilterChipData(label: 'Tất cả', icon: Icons.inbox_rounded, selected: true),
      _FilterChipData(
          label: 'Đơn hàng', icon: Icons.receipt_long_rounded, selected: false),
      _FilterChipData(label: 'Ưu đãi', icon: Icons.local_offer_rounded, selected: false),
    ];
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: filters
            .map((f) => Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: _FilterChip(data: f),
                ))
            .toList(),
      ),
    );
  }
}

class _FilterChipData {
  final String label;
  final IconData icon;
  final bool selected;

  const _FilterChipData({
    required this.label,
    required this.icon,
    required this.selected,
  });
}

class _FilterChip extends StatelessWidget {
  final _FilterChipData data;

  const _FilterChip({required this.data});

  @override
  Widget build(BuildContext context) {
    return FilterChip(
      label: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(data.icon, size: 16),
          const SizedBox(width: 6),
          Text(data.label),
        ],
      ),
      selected: data.selected,
      onSelected: (_) {},
      selectedColor: AppColors.customer.withOpacity(0.12),
      checkmarkColor: AppColors.customer,
      backgroundColor: Colors.white,
      labelStyle: TextStyle(
        color: data.selected ? AppColors.customer : AppColors.ink.withOpacity(0.75),
        fontWeight: FontWeight.w700,
      ),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(
          color:
              data.selected ? AppColors.customer.withOpacity(0.4) : AppColors.ink.withOpacity(0.08),
        ),
      ),
    );
  }
}

class _Section extends StatelessWidget {
  final String title;
  final List<_AppNotification> items;

  const _Section({required this.title, required this.items});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
          ),
          const SizedBox(height: 8),
          ...items.map(
            (item) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: _NotificationTile(notification: item),
            ),
          ),
        ],
      ),
    );
  }
}

enum NotificationType { success, promo, info, reminder }

class _AppNotification {
  final String title;
  final String subtitle;
  final String time;
  final NotificationType type;
  final bool unread;

  const _AppNotification({
    required this.title,
    required this.subtitle,
    required this.time,
    required this.type,
    this.unread = false,
  });
}

class _NotificationTile extends StatelessWidget {
  final _AppNotification notification;

  const _NotificationTile({required this.notification});

  Color _colorForType() {
    switch (notification.type) {
      case NotificationType.success:
        return const Color(0xFF45A17E);
      case NotificationType.promo:
        return const Color(0xFFE07A24);
      case NotificationType.info:
        return AppColors.customer;
      case NotificationType.reminder:
        return const Color(0xFF1F3C88);
    }
  }

  IconData _iconForType() {
    switch (notification.type) {
      case NotificationType.success:
        return Icons.check_circle_rounded;
      case NotificationType.promo:
        return Icons.local_offer_rounded;
      case NotificationType.info:
        return Icons.receipt_long_rounded;
      case NotificationType.reminder:
        return Icons.notifications_active_rounded;
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _colorForType();
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: notification.unread ? color.withOpacity(0.04) : Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: notification.unread
              ? color.withOpacity(0.35)
              : AppColors.ink.withOpacity(0.05),
        ),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.04),
                  blurRadius: 8,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: Icon(_iconForType(), color: color, size: 20),
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
                        notification.title,
                        style: TextStyle(
                          color: AppColors.ink,
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                    ),
                    const SizedBox(width: 6),
                    Text(
                      notification.time,
                      style: TextStyle(
                        color: AppColors.ink.withOpacity(0.55),
                        fontWeight: FontWeight.w600,
                        fontSize: 12.5,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  notification.subtitle,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.7),
                    fontWeight: FontWeight.w600,
                    height: 1.3,
                  ),
                ),
                if (notification.unread) ...[
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Container(
                        padding:
                            const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                        decoration: BoxDecoration(
                          color: color.withOpacity(0.14),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: const Text(
                          'Chưa đọc',
                          style: TextStyle(
                            color: AppColors.ink,
                            fontWeight: FontWeight.w800,
                            fontSize: 12,
                          ),
                        ),
                      ),
                      const Spacer(),
                      TextButton(
                        onPressed: () {},
                        style: TextButton.styleFrom(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 6),
                          minimumSize: Size.zero,
                          tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        ),
                        child: const Text('Đánh dấu đã đọc'),
                      ),
                    ],
                  ),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}
