import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';

class CourierNotificationsPage extends StatelessWidget {
  const CourierNotificationsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final items = [
      const _Notification(
        title: 'Đơn mới: #DL-2304',
        subtitle: 'Ca chiều · KTX Zone C · Nhận trước 13:00',
        time: '5 phút trước',
        type: NotificationType.assignment,
        unread: true,
      ),
      const _Notification(
        title: 'Sự cố tuyến đường',
        subtitle: 'Đường Nguyễn Huệ kẹt xe, gợi ý tuyến thay thế.',
        time: '20 phút trước',
        type: NotificationType.issue,
        unread: true,
      ),
      const _Notification(
        title: 'Nhắc chụp Minh chứng',
        subtitle: 'Đơn #DL-2301 còn thiếu ảnh giao hàng.',
        time: 'Hôm nay',
        type: NotificationType.proof,
      ),
      const _Notification(
        title: 'Đánh giá từ khách',
        subtitle: 'Ca sáng hôm nay: 5 sao, phản hồi tốt.',
        time: 'Hôm qua',
        type: NotificationType.feedback,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Thông báo tài xế'),
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
            Container(
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
                children:
                    items
                        .map(
                          (n) => Padding(
                            padding: const EdgeInsets.symmetric(vertical: 8),
                            child: _NotificationTile(notification: n),
                          ),
                        )
                        .toList(),
              ),
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
      const _Filter('Tất cả', Icons.inbox_rounded, true),
      const _Filter('Đơn mới', Icons.list_alt_rounded, false),
      const _Filter('Sự cố', Icons.warning_amber_rounded, false),
      const _Filter('Minh chứng', Icons.camera_alt_outlined, false),
    ];
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children:
            filters
                .map(
                  (f) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: FilterChip(
                      label: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Icon(f.icon, size: 16),
                          const SizedBox(width: 6),
                          Text(f.label),
                        ],
                      ),
                      selected: f.selected,
                      onSelected: (_) {},
                      selectedColor: AppColors.courier.withOpacity(0.12),
                      checkmarkColor: AppColors.courier,
                      backgroundColor: Colors.white,
                      labelStyle: TextStyle(
                        color:
                            f.selected
                                ? AppColors.courier
                                : AppColors.ink.withOpacity(0.75),
                        fontWeight: FontWeight.w700,
                      ),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                        side: BorderSide(
                          color:
                              f.selected
                                  ? AppColors.courier.withOpacity(0.4)
                                  : AppColors.ink.withOpacity(0.08),
                        ),
                      ),
                    ),
                  ),
                )
                .toList(),
      ),
    );
  }
}

class _Filter {
  final String label;
  final IconData icon;
  final bool selected;

  const _Filter(this.label, this.icon, this.selected);
}

enum NotificationType { assignment, issue, proof, feedback }

class _Notification {
  final String title;
  final String subtitle;
  final String time;
  final NotificationType type;
  final bool unread;

  const _Notification({
    required this.title,
    required this.subtitle,
    required this.time,
    required this.type,
    this.unread = false,
  });
}

class _NotificationTile extends StatelessWidget {
  final _Notification notification;

  const _NotificationTile({required this.notification});

  Color _color() {
    switch (notification.type) {
      case NotificationType.assignment:
        return AppColors.courier;
      case NotificationType.issue:
        return const Color(0xFFF4A261);
      case NotificationType.proof:
        return const Color(0xFF1F3C88);
      case NotificationType.feedback:
        return const Color(0xFF2BAE66);
    }
  }

  IconData _icon() {
    switch (notification.type) {
      case NotificationType.assignment:
        return Icons.list_alt_rounded;
      case NotificationType.issue:
        return Icons.warning_amber_rounded;
      case NotificationType.proof:
        return Icons.camera_alt_outlined;
      case NotificationType.feedback:
        return Icons.star_rounded;
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _color();
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: notification.unread ? color.withOpacity(0.05) : Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color:
              notification.unread
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
            child: Icon(_icon(), color: color, size: 20),
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
                        style: const TextStyle(
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
                        padding: const EdgeInsets.symmetric(
                          horizontal: 8,
                          vertical: 4,
                        ),
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
                            horizontal: 10,
                            vertical: 6,
                          ),
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
