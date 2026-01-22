import 'package:flutter/material.dart';

import '../../../../../app/app_routes.dart';
import '../../../../../core/constants/app_colors.dart';

class DeliveryListPage extends StatelessWidget {
  const DeliveryListPage({super.key});

  @override
  Widget build(BuildContext context) {
    final deliveries = [
      const _Delivery(
        title: 'Ca sáng · Văn phòng Q1',
        code: '#DL-2301',
        time: '08:30 · 25/03',
        stops: 4,
        status: 'Đang giao',
        color: Color(0xFFF4A261),
      ),
      const _Delivery(
        title: 'Ca trưa · KTX Zone B',
        code: '#DL-2302',
        time: '11:30 · 25/03',
        stops: 6,
        status: 'Chờ lấy',
        color: AppColors.courier,
      ),
      const _Delivery(
        title: 'Ca tối · Kho Q7',
        code: '#DL-2303',
        time: '18:00 · 25/03',
        stops: 3,
        status: 'Hoàn tất',
        color: Color(0xFF2BAE66),
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Danh sách giao hàng'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children:
              deliveries
                  .map(
                    (d) => Padding(
                      padding: const EdgeInsets.only(bottom: 10),
                      child: _DeliveryCard(delivery: d),
                    ),
                  )
                  .toList(),
        ),
      ),
    );
  }
}

class _Delivery {
  final String title;
  final String code;
  final String time;
  final int stops;
  final String status;
  final Color color;

  const _Delivery({
    required this.title,
    required this.code,
    required this.time,
    required this.stops,
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
      borderRadius: BorderRadius.circular(14),
      onTap:
          () =>
              Navigator.of(context).pushNamed(AppRoutes.courierDeliveryDetail),
      child: Container(
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
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: delivery.color.withOpacity(0.12),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.delivery_dining_rounded,
                    color: AppColors.courier,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        delivery.title,
                        style: const TextStyle(
                          color: AppColors.ink,
                          fontWeight: FontWeight.w900,
                          fontSize: 16,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        '${delivery.code} · ${delivery.time}',
                        style: TextStyle(
                          color: AppColors.ink.withOpacity(0.6),
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 8,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: delivery.color.withOpacity(0.14),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    delivery.status,
                    style: TextStyle(
                      color: delivery.color,
                      fontWeight: FontWeight.w800,
                      fontSize: 12.5,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Icon(
                  Icons.pin_drop_outlined,
                  size: 18,
                  color: AppColors.ink,
                ),
                const SizedBox(width: 6),
                Text(
                  '${delivery.stops} điểm dừng',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.75),
                    fontWeight: FontWeight.w700,
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
