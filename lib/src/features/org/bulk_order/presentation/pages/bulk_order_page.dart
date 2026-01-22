import 'package:flutter/material.dart';

import '../../../../../app/app_routes.dart';
import '../../../../../core/constants/app_colors.dart';

class BulkOrderPage extends StatelessWidget {
  const BulkOrderPage({super.key});

  @override
  Widget build(BuildContext context) {
    final upcoming = [
      const _BulkOrder(
        title: 'Ca sáng · Thứ 2',
        date: '08:30 · 25/03',
        pax: 45,
        menu: 'Cơm gà xé, Canh bí, Trà tắc',
        status: OrderStatus.confirmed,
      ),
      const _BulkOrder(
        title: 'Ca trưa · Thứ 3',
        date: '11:30 · 26/03',
        pax: 62,
        menu: 'Bún bò, Rau luộc, Nước ép',
        status: OrderStatus.draft,
      ),
      const _BulkOrder(
        title: 'Ca tối · Thứ 4',
        date: '18:00 · 27/03',
        pax: 30,
        menu: 'Cơm sườn, Canh chua, Nước suối',
        status: OrderStatus.delivering,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Đặt số lượng lớn'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.orgStaff),
            icon: const Icon(Icons.groups_rounded),
            tooltip: 'Danh sách nhân viên',
          ),
          IconButton(onPressed: () {}, icon: const Icon(Icons.more_vert)),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.org,
        foregroundColor: Colors.white,
        onPressed: () {},
        icon: const Icon(Icons.add_rounded),
        label: const Text('Tạo đơn mới'),
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 48),
        child: Column(
          children: [
            const _HeaderCard(),
            const SizedBox(height: 14),
            const _ShiftSelector(),
            const SizedBox(height: 14),
            const _PlanSummary(),
            const SizedBox(height: 14),
            _UpcomingList(orders: upcoming),
          ],
        ),
      ),
    );
  }
}

class _HeaderCard extends StatelessWidget {
  const _HeaderCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.org, AppColors.orgAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.org.withOpacity(0.24),
            blurRadius: 18,
            offset: const Offset(0, 12),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.18),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.event_note, color: Colors.white),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  'Kế hoạch tuần này',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                    letterSpacing: -0.2,
                  ),
                ),
              ),
              TextButton(
                style: TextButton.styleFrom(foregroundColor: Colors.white),
                onPressed: () {},
                child: const Text('Tải thực đơn'),
              ),
            ],
          ),
          const SizedBox(height: 14),
          LayoutBuilder(
            builder: (context, constraints) {
              final isTight = constraints.maxWidth < 360;
              final children = const [
                _HeaderStat(
                  label: 'Tổng suất ăn',
                  value: '186',
                  icon: Icons.groups_rounded,
                ),
              ];
              if (isTight) {
                return Column(
                  children:
                      children
                          .map(
                            (c) => Padding(
                              padding: const EdgeInsets.only(bottom: 8),
                              child: c,
                            ),
                          )
                          .toList(),
                );
              }
              return Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children:
                    children
                        .map(
                          (c) => Expanded(
                            child: Padding(
                              padding: const EdgeInsets.only(right: 8),
                              child: c,
                            ),
                          ),
                        )
                        .toList(),
              );
            },
          ),
          const SizedBox(height: 12),
          ElevatedButton.icon(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.white,
              foregroundColor: AppColors.org,
              padding: const EdgeInsets.symmetric(vertical: 12),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onPressed: () {},
            icon: const Icon(Icons.add_task_rounded),
            label: const Text(
              'Lên đơn theo ca',
              style: TextStyle(fontWeight: FontWeight.w800),
            ),
          ),
        ],
      ),
    );
  }
}

class _HeaderStat extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;

  const _HeaderStat({
    required this.label,
    required this.value,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return ConstrainedBox(
      constraints: const BoxConstraints(minWidth: 110),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        decoration: BoxDecoration(
          color: Colors.white.withOpacity(0.18),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: Colors.white.withOpacity(0.3)),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.16),
                shape: BoxShape.circle,
              ),
              child: Icon(icon, color: Colors.white, size: 18),
            ),
            const SizedBox(width: 10),
            Flexible(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    value,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.w900,
                      fontSize: 16,
                    ),
                  ),
                  Text(
                    label,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(
                      color: Colors.white.withOpacity(0.9),
                      fontWeight: FontWeight.w600,
                      height: 1.1,
                    ),
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

class _ShiftSelector extends StatelessWidget {
  const _ShiftSelector();

  @override
  Widget build(BuildContext context) {
    final chips = ['Ca sáng', 'Ca trưa', 'Ca tối', 'Theo phòng ban'];
    return Container(
      padding: const EdgeInsets.all(12),
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
          Row(
            children: [
              const Icon(Icons.schedule_rounded, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Chọn ca & phạm vi',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Tùy chỉnh')),
            ],
          ),
          const SizedBox(height: 10),
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            padding: const EdgeInsets.only(bottom: 4),
            child: Row(
              children:
                  chips
                      .map(
                        (chip) => Padding(
                          padding: const EdgeInsets.only(right: 8),
                          child: FilterChip(
                            label: Text(chip),
                            selected: chip == 'Ca trưa',
                            onSelected: (_) {},
                            selectedColor: AppColors.org.withOpacity(0.12),
                            checkmarkColor: AppColors.org,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                              side: BorderSide(
                                color:
                                    chip == 'Ca trưa'
                                        ? AppColors.org.withOpacity(0.5)
                                        : AppColors.ink.withOpacity(0.08),
                              ),
                            ),
                            labelStyle: TextStyle(
                              color:
                                  chip == 'Ca trưa'
                                      ? AppColors.org
                                      : AppColors.ink.withOpacity(0.75),
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                        ),
                      )
                      .toList(),
            ),
          ),
          const SizedBox(height: 12),
          LayoutBuilder(
            builder: (context, constraints) {
              final isTight = constraints.maxWidth < 360;
              if (isTight) {
                return Column(
                  children: const [
                    _DateField(
                      label: 'Ngày bắt đầu',
                      value: '25/03/2024',
                      icon: Icons.calendar_today_rounded,
                    ),
                    SizedBox(height: 10),
                    _DateField(
                      label: 'Đến',
                      value: '29/03/2024',
                      icon: Icons.event_available_rounded,
                    ),
                  ],
                );
              }
              return Row(
                children: const [
                  Expanded(
                    child: _DateField(
                      label: 'Ngày bắt đầu',
                      value: '25/03/2024',
                      icon: Icons.calendar_today_rounded,
                    ),
                  ),
                  SizedBox(width: 10),
                  Expanded(
                    child: _DateField(
                      label: 'Đến',
                      value: '29/03/2024',
                      icon: Icons.event_available_rounded,
                    ),
                  ),
                ],
              );
            },
          ),
        ],
      ),
    );
  }
}

class _DateField extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;

  const _DateField({
    required this.label,
    required this.value,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFFF8F9FB),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.ink.withOpacity(0.06)),
      ),
      child: Row(
        children: [
          Icon(icon, color: AppColors.org),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w700,
                    fontSize: 12.5,
                  ),
                ),
                Text(
                  value,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w900,
                  ),
                ),
              ],
            ),
          ),
          const Icon(Icons.keyboard_arrow_down_rounded, color: Colors.black38),
        ],
      ),
    );
  }
}

class _PlanSummary extends StatelessWidget {
  const _PlanSummary();

  @override
  Widget build(BuildContext context) {
    final cards = [
      _PlanCard(
        color: AppColors.org,
        title: 'Theo ca',
        value: '3 ca',
        subtitle: 'Sáng, trưa, tối',
        icon: Icons.access_time_rounded,
      ),
      _PlanCard(
        color: const Color(0xFF1F3C88),
        title: 'Theo phòng ban',
        value: '5 nhóm',
        subtitle: 'Kế toán, CSKH, IT, Vận hành, Sales',
        icon: Icons.account_tree_rounded,
      ),
      _PlanCard(
        color: const Color(0xFFF4A261),
        title: 'Ngân sách',
        value: '12.5tr',
        subtitle: 'Trong tuần này',
        icon: Icons.payments_rounded,
      ),
    ];

    return SizedBox(
      height: 140,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        physics: const BouncingScrollPhysics(),
        itemBuilder: (context, index) => cards[index],
        separatorBuilder: (_, __) => const SizedBox(width: 10),
        itemCount: cards.length,
      ),
    );
  }
}

class _PlanCard extends StatelessWidget {
  final Color color;
  final String title;
  final String value;
  final String subtitle;
  final IconData icon;

  const _PlanCard({
    required this.color,
    required this.title,
    required this.value,
    required this.subtitle,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return ConstrainedBox(
      constraints: const BoxConstraints(minWidth: 170),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: color.withOpacity(0.12)),
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
            CircleAvatar(
              radius: 18,
              backgroundColor: color.withOpacity(0.12),
              child: Icon(icon, color: color, size: 18),
            ),
            const Spacer(),
            Text(
              title,
              style: TextStyle(
                color: AppColors.ink.withOpacity(0.7),
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              value,
              style: TextStyle(
                color: color,
                fontWeight: FontWeight.w900,
                fontSize: 16,
              ),
            ),
            const SizedBox(height: 2),
            Text(
              subtitle,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                color: AppColors.ink.withOpacity(0.6),
                fontWeight: FontWeight.w600,
                height: 1.2,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

enum OrderStatus { draft, confirmed, delivering }

class _BulkOrder {
  final String title;
  final String date;
  final int pax;
  final String menu;
  final OrderStatus status;

  const _BulkOrder({
    required this.title,
    required this.date,
    required this.pax,
    required this.menu,
    required this.status,
  });
}

class _UpcomingList extends StatelessWidget {
  final List<_BulkOrder> orders;

  const _UpcomingList({required this.orders});

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
          Row(
            children: [
              const Icon(Icons.upcoming_rounded, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Lịch giao sắp tới',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Xuất Excel')),
            ],
          ),
          const SizedBox(height: 8),
          ...orders.map(
            (order) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: _UpcomingTile(order: order),
            ),
          ),
        ],
      ),
    );
  }
}

class _UpcomingTile extends StatelessWidget {
  final _BulkOrder order;

  const _UpcomingTile({required this.order});

  Color _statusColor() {
    switch (order.status) {
      case OrderStatus.confirmed:
        return const Color(0xFF2BAE66);
      case OrderStatus.delivering:
        return const Color(0xFFF4A261);
      case OrderStatus.draft:
      default:
        return AppColors.org;
    }
  }

  String _statusLabel() {
    switch (order.status) {
      case OrderStatus.confirmed:
        return 'Đã chốt';
      case OrderStatus.delivering:
        return 'Đang giao';
      case OrderStatus.draft:
        return 'Bản nháp';
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _statusColor();
    return InkWell(
      borderRadius: BorderRadius.circular(12),
      onTap: () {},
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: AppColors.ink.withOpacity(0.01),
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: AppColors.ink.withOpacity(0.05)),
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: color.withOpacity(0.12),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(Icons.event_available_rounded, color: color),
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
                          order.title,
                          style: TextStyle(
                            color: AppColors.ink,
                            fontWeight: FontWeight.w900,
                          ),
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 8,
                          vertical: 4,
                        ),
                        decoration: BoxDecoration(
                          color: color.withOpacity(0.14),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Text(
                          _statusLabel(),
                          style: TextStyle(
                            color: color,
                            fontWeight: FontWeight.w800,
                            fontSize: 12.5,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Text(
                    '${order.date} · ${order.pax} suất',
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.65),
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    order.menu,
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.75),
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            Icon(
              Icons.chevron_right_rounded,
              color: AppColors.ink.withOpacity(0.4),
            ),
          ],
        ),
      ),
    );
  }
}
