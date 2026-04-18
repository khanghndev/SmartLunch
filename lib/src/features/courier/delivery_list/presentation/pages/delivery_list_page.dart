import 'package:flutter/material.dart';

import '../../../../../app/app_routes.dart';
import '../../../../../core/constants/app_colors.dart';

class DeliveryListPage extends StatefulWidget {
  const DeliveryListPage({super.key});

  @override
  State<DeliveryListPage> createState() => _DeliveryListPageState();
}

class _DeliveryListPageState extends State<DeliveryListPage> {
  int _selectedFilter = 0;

  @override
  Widget build(BuildContext context) {
    final deliveries = [
      const _Delivery(
        title: 'Ca sáng · Văn phòng Q1',
        code: '#DL-2301',
        time: '08:30 · 25/03',
        stops: 4,
        eta: '35 phút',
        status: 'Đang giao',
        color: AppColors.info,
      ),
      const _Delivery(
        title: 'Ca trưa · KTX Zone B',
        code: '#DL-2302',
        time: '11:30 · 25/03',
        stops: 6,
        eta: '1 giờ 10 phút',
        status: 'Chờ lấy',
        color: AppColors.courier,
      ),
      const _Delivery(
        title: 'Ca tối · Kho Q7',
        code: '#DL-2303',
        time: '18:00 · 25/03',
        stops: 3,
        eta: 'Đã xong',
        status: 'Hoàn tất',
        color: AppColors.success,
      ),
    ];

    final filters = ['Tất cả', 'Đang giao', 'Chờ lấy', 'Hoàn tất'];

    return Scaffold(
      backgroundColor: AppColors.surface,
      appBar: AppBar(
        title: const Text('Danh sách giao hàng'),
        actions: [
          IconButton(
            onPressed: () {},
            icon: const Icon(Icons.tune_rounded),
            tooltip: 'Bộ lọc',
          ),
        ],
      ),
      body: CustomScrollView(
        physics: const BouncingScrollPhysics(),
        slivers: [
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 8, 20, 10),
              child: _SummaryStrip(deliveries: deliveries),
            ),
          ),
          SliverToBoxAdapter(
            child: SizedBox(
              height: 44,
              child: ListView.separated(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                scrollDirection: Axis.horizontal,
                itemBuilder: (context, index) {
                  final selected = index == _selectedFilter;
                  return ChoiceChip(
                    label: Text(filters[index]),
                    selected: selected,
                    onSelected: (_) => setState(() => _selectedFilter = index),
                    selectedColor: AppColors.tint(AppColors.courier, 0.16),
                    side: BorderSide(
                      color:
                          selected
                              ? AppColors.courier.withOpacity(0.35)
                              : AppColors.border,
                    ),
                    labelStyle: Theme.of(
                      context,
                    ).textTheme.labelLarge?.copyWith(
                      fontWeight: FontWeight.w700,
                      color: selected ? AppColors.courier : AppColors.inkSoft,
                    ),
                  );
                },
                separatorBuilder: (_, __) => const SizedBox(width: 8),
                itemCount: filters.length,
              ),
            ),
          ),
          SliverPadding(
            padding: const EdgeInsets.fromLTRB(20, 14, 20, 30),
            sliver: SliverList.separated(
              itemBuilder:
                  (context, index) =>
                      _DeliveryCard(delivery: deliveries[index]),
              separatorBuilder: (_, __) => const SizedBox(height: 12),
              itemCount: deliveries.length,
            ),
          ),
        ],
      ),
    );
  }
}

class _SummaryStrip extends StatelessWidget {
  final List<_Delivery> deliveries;

  const _SummaryStrip({required this.deliveries});

  @override
  Widget build(BuildContext context) {
    final inProgress = deliveries.where((d) => d.status == 'Đang giao').length;
    final waiting = deliveries.where((d) => d.status == 'Chờ lấy').length;
    final done = deliveries.where((d) => d.status == 'Hoàn tất').length;

    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.courier, AppColors.courierAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.courier.withOpacity(0.28),
            blurRadius: 18,
            offset: const Offset(0, 10),
            spreadRadius: -6,
          ),
        ],
      ),
      child: Row(
        children: [
          _SummaryItem(label: 'Đang giao', value: '$inProgress'),
          _SummaryDivider(),
          _SummaryItem(label: 'Chờ lấy', value: '$waiting'),
          _SummaryDivider(),
          _SummaryItem(label: 'Hoàn tất', value: '$done'),
        ],
      ),
    );
  }
}

class _SummaryItem extends StatelessWidget {
  final String label;
  final String value;

  const _SummaryItem({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Column(
        children: [
          Text(
            value,
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w800,
            ),
          ),
          Text(
            label,
            style: Theme.of(context).textTheme.labelMedium?.copyWith(
              color: Colors.white.withOpacity(0.9),
            ),
          ),
        ],
      ),
    );
  }
}

class _SummaryDivider extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      width: 1,
      height: 32,
      color: Colors.white.withOpacity(0.28),
    );
  }
}

class _Delivery {
  final String title;
  final String code;
  final String time;
  final int stops;
  final String eta;
  final String status;
  final Color color;

  const _Delivery({
    required this.title,
    required this.code,
    required this.time,
    required this.stops,
    required this.eta,
    required this.status,
    required this.color,
  });
}

class _DeliveryCard extends StatelessWidget {
  final _Delivery delivery;

  const _DeliveryCard({required this.delivery});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      borderRadius: BorderRadius.circular(18),
      onTap:
          () =>
              Navigator.of(context).pushNamed(AppRoutes.courierDeliveryDetail),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: AppColors.border),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow.withOpacity(0.42),
              blurRadius: 18,
              offset: const Offset(0, 8),
              spreadRadius: -8,
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  width: 44,
                  height: 44,
                  decoration: BoxDecoration(
                    color: delivery.color.withOpacity(0.14),
                    borderRadius: BorderRadius.circular(13),
                  ),
                  child: Icon(
                    Icons.local_shipping_rounded,
                    color: delivery.color,
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        delivery.title,
                        style: Theme.of(context).textTheme.titleMedium
                            ?.copyWith(fontWeight: FontWeight.w800),
                      ),
                      const SizedBox(height: 3),
                      Text(
                        '${delivery.code} · ${delivery.time}',
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: AppColors.inkSoft,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ),
                _StatusChip(delivery: delivery),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                _InfoPill(
                  icon: Icons.pin_drop_outlined,
                  text: '${delivery.stops} điểm dừng',
                ),
                const SizedBox(width: 8),
                _InfoPill(
                  icon: Icons.timer_outlined,
                  text: 'ETA ${delivery.eta}',
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _StatusChip extends StatelessWidget {
  final _Delivery delivery;

  const _StatusChip({required this.delivery});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: delivery.color.withOpacity(0.12),
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        delivery.status,
        style: TextStyle(
          color: delivery.color,
          fontWeight: FontWeight.w700,
          fontSize: 12.5,
        ),
      ),
    );
  }
}

class _InfoPill extends StatelessWidget {
  final IconData icon;
  final String text;

  const _InfoPill({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
        decoration: BoxDecoration(
          color: AppColors.surfaceStrong.withOpacity(0.75),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          children: [
            Icon(icon, size: 16, color: AppColors.inkSoft),
            const SizedBox(width: 6),
            Expanded(
              child: Text(
                text,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: AppColors.inkSoft,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
