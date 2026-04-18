import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';

class OrgNotificationsPage extends StatelessWidget {
  const OrgNotificationsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final items = [
      const _OrgNotification(
        title: 'Đối soát kỳ 01-15/03 đã phát hành',
        subtitle: 'Kiểm tra và xác nhận trước 05/04.',
        time: '2 giờ trước',
        type: NotificationType.finance,
        unread: true,
      ),
      const _OrgNotification(
        title: 'Cập nhật kế hoạch ca trưa Thứ 4',
        subtitle: 'Số suất tăng thêm 12, vui lòng duyệt.',
        time: 'Hôm nay',
        type: NotificationType.order,
        unread: true,
      ),
      const _OrgNotification(
        title: 'Báo cáo tháng 2 đã sẵn sàng',
        subtitle: 'Tải PDF hoặc gửi email cho kế toán.',
        time: 'Hôm qua',
        type: NotificationType.report,
      ),
      const _OrgNotification(
        title: 'Phản hồi chất lượng từ phòng IT',
        subtitle: 'Điểm hài lòng 4.6/5 · 32 suất',
        time: 'Hôm qua',
        type: NotificationType.feedback,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Thông báo doanh nghiệp'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgReports),
            icon: const Icon(Icons.description_outlined),
            tooltip: 'Báo cáo',
          ),
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
                          (item) => Padding(
                            padding: const EdgeInsets.symmetric(vertical: 8),
                            child: _NotificationTile(notification: item),
                          ),
                        )
                        .toList(),
              ),
            ),
            const SizedBox(height: 12),
            TextButton.icon(
              onPressed: () {},
              icon: const Icon(Icons.history_rounded),
              label: const Text('Xem thông báo cũ'),
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
      const _Filter('Đối soát', Icons.payments_rounded, false),
      const _Filter('Đơn hàng', Icons.event_note_rounded, false),
      const _Filter('Báo cáo', Icons.description_outlined, false),
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
                      selectedColor: AppColors.org.withOpacity(0.12),
                      checkmarkColor: AppColors.org,
                      backgroundColor: Colors.white,
                      labelStyle: TextStyle(
                        color:
                            f.selected
                                ? AppColors.org
                                : AppColors.ink.withOpacity(0.75),
                        fontWeight: FontWeight.w700,
                      ),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                        side: BorderSide(
                          color:
                              f.selected
                                  ? AppColors.org.withOpacity(0.4)
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

enum NotificationType { finance, order, report, feedback }

class _OrgNotification {
  final String title;
  final String subtitle;
  final String time;
  final NotificationType type;
  final bool unread;

  const _OrgNotification({
    required this.title,
    required this.subtitle,
    required this.time,
    required this.type,
    this.unread = false,
  });
}

class _NotificationTile extends StatelessWidget {
  final _OrgNotification notification;

  const _NotificationTile({required this.notification});

  Color _color() {
    switch (notification.type) {
      case NotificationType.finance:
        return AppColors.org;
      case NotificationType.order:
        return AppColors.org;
      case NotificationType.report:
        return AppColors.org;
      case NotificationType.feedback:
        return AppColors.orgAlt;
    }
  }

  IconData _icon() {
    switch (notification.type) {
      case NotificationType.finance:
        return Icons.payments_rounded;
      case NotificationType.order:
        return Icons.event_note_rounded;
      case NotificationType.report:
        return Icons.description_outlined;
      case NotificationType.feedback:
        return Icons.thumb_up_alt_outlined;
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
