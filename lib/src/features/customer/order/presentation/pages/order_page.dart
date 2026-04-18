import 'package:flutter/material.dart';

import '../../../../../app/app_routes.dart';
import '../../../../../core/constants/app_colors.dart';

class OrderPage extends StatelessWidget {
  final bool embedded;
  final double bottomInset;

  const OrderPage({super.key, this.embedded = false, this.bottomInset = 0});

  @override
  Widget build(BuildContext context) {
    final upcomingOrders = [
      _OrderData(
        title: 'Đặt bữa nhanh',
        subtitle: 'Giao hôm nay · 11:00 - 11:30',
        price: '55.000đ',
        status: 'Đang xử lý',
        tag: 'Healthy',
        color: AppColors.customer,
      ),
      _OrderData(
        title: 'Combo văn phòng',
        subtitle: 'Giao ngày mai · 10:30 - 11:00',
        price: '120.000đ',
        status: 'Đã xác nhận',
        tag: 'Combo 2 phần',
        color: AppColors.customerAlt,
      ),
    ];

    final historyOrders = [
      _OrderData(
        title: 'Phở bò truyền thống',
        subtitle: 'Đã giao · 3 ngày trước',
        price: '55.000đ',
        status: 'Hoàn tất',
        tag: 'Đánh giá',
        color: AppColors.customer,
      ),
      _OrderData(
        title: 'Cơm gà nướng mật ong',
        subtitle: 'Đã giao · 1 tuần trước',
        price: '45.000đ',
        status: 'Hoàn tất',
        tag: 'Thêm vào giỏ',
        color: AppColors.customerAlt,
      ),
    ];

    final content = SingleChildScrollView(
      physics: const BouncingScrollPhysics(),
      padding: EdgeInsets.fromLTRB(20, embedded ? 6 : 18, 20, 28 + bottomInset),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const _HighlightCard(),
          const SizedBox(height: 16),
          const _QuickActions(),
          const SizedBox(height: 22),
          const _SectionHeader(
            title: 'Đơn sắp tới',
            subtitle: 'Các đơn đã đặt và chờ giao',
          ),
          const SizedBox(height: 10),
          ...upcomingOrders.map(
            (order) => Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: _OrderTile(order: order),
            ),
          ),
          const SizedBox(height: 10),
          const SizedBox(height: 14),
          const _SectionHeader(
            title: 'Lịch sử nhanh',
            subtitle: 'Các đơn giao gần đây',
          ),
          const SizedBox(height: 10),
          ...historyOrders.map(
            (order) => Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: _OrderTile(order: order, dense: true),
            ),
          ),
        ],
      ),
    );

    if (embedded) {
      return content;
    }

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Đơn hàng'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: content,
    );
  }
}

class _HighlightCard extends StatelessWidget {
  const _HighlightCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.customer, AppColors.customerAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.customer.withOpacity(0.25),
            blurRadius: 16,
            offset: const Offset(0, 10),
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
                  color: Colors.white.withOpacity(0.2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  Icons.bento_rounded,
                  color: Colors.white,
                  size: 22,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Đặt bữa nhanh hôm nay',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'Chốt đơn trước 10:00, giao 11:00 - 11:30',
                      style: TextStyle(color: Colors.white.withOpacity(0.9)),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              _Pill(
                label: 'Miễn phí giao nội bộ',
                color: Colors.white.withOpacity(0.2),
                textColor: Colors.white,
                icon: Icons.delivery_dining_rounded,
              ),
              const SizedBox(width: 8),
              _Pill(
                label: 'Ăn healthy',
                color: Colors.white.withOpacity(0.2),
                textColor: Colors.white,
                icon: Icons.eco_rounded,
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              Expanded(
                child: ElevatedButton(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.white,
                    foregroundColor: AppColors.customer,
                    elevation: 0,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  onPressed:
                      () => Navigator.of(
                        context,
                      ).pushNamed(AppRoutes.customerOrder),
                  child: const Text(
                    'Đặt bữa nhanh',
                    style: TextStyle(fontWeight: FontWeight.w800),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              OutlinedButton(
                style: OutlinedButton.styleFrom(
                  foregroundColor: Colors.white,
                  side: BorderSide(color: Colors.white.withOpacity(0.7)),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 13,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                onPressed:
                    () =>
                        Navigator.of(context).pushNamed(AppRoutes.customerMenu),
                child: const Text('Chọn món'),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _QuickActions extends StatelessWidget {
  const _QuickActions();

  @override
  Widget build(BuildContext context) {
    final actions = [
      _ActionData(
        label: 'Lịch sử',
        icon: Icons.history_rounded,
        color: AppColors.customerAlt,
        onTap: () => Navigator.of(context).pushNamed(AppRoutes.customerHistory),
      ),
      _ActionData(
        label: 'Ưu đãi',
        icon: Icons.local_offer_rounded,
        color: AppColors.customer,
        onTap: () {},
      ),
    ];

    return Row(
      children:
          actions
              .asMap()
              .entries
              .map(
                (entry) => Expanded(
                  child: Padding(
                    padding: EdgeInsets.only(
                      right: entry.key == actions.length - 1 ? 0 : 10,
                    ),
                    child: _QuickActionCard(data: entry.value),
                  ),
                ),
              )
              .toList(),
    );
  }
}

class _QuickActionCard extends StatelessWidget {
  final _ActionData data;

  const _QuickActionCard({required this.data});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(14),
      elevation: 2,
      shadowColor: Colors.black.withOpacity(0.05),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: data.onTap,
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 10),
          child: Column(
            children: [
              CircleAvatar(
                radius: 20,
                backgroundColor: data.color.withOpacity(0.12),
                child: Icon(data.icon, color: data.color, size: 20),
              ),
              const SizedBox(height: 8),
              Text(
                data.label,
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.8),
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _OrderTile extends StatelessWidget {
  final _OrderData order;
  final bool dense;

  const _OrderTile({required this.order, this.dense = false});

  @override
  Widget build(BuildContext context) {
    final padding = dense ? 12.0 : 14.0;
    return InkWell(
      borderRadius: BorderRadius.circular(14),
      onTap:
          () => Navigator.of(context).pushNamed(
            AppRoutes.customerOrderDetail,
            arguments: {
              'id': '#SL-202409-00${order.title.hashCode.abs() % 90}',
              'title': order.title,
              'status': order.status,
              'statusColor': order.color,
              'time': order.subtitle,
              'address': 'Tòa nhà ABC, 123 Lê Lợi, Q.1, TP.HCM',
              'contact': 'Nguyễn An (0909 123 456)',
              'note': 'Giao tại quầy lễ tân, gọi trước 5 phút.',
              'items': [
                {'name': order.title, 'qty': 1, 'price': 55000},
                {'name': 'Nước ép cam', 'qty': 1, 'price': 25000},
              ],
              'shipping': 12000,
              'discount': 15000,
              'subtotal': 80000,
              'total': 77000,
              'payment': 'Ví SmartLunch · Đã thanh toán',
              'timeline':
                  order.status == 'Đang giao'
                      ? 3
                      : order.status == 'Đã xác nhận'
                      ? 2
                      : 4,
            },
          ),
      child: Container(
        padding: EdgeInsets.all(padding),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: AppColors.ink.withOpacity(0.04)),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 12,
              offset: const Offset(0, 8),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: dense ? 44 : 52,
              height: dense ? 44 : 52,
              decoration: BoxDecoration(
                color: order.color.withOpacity(0.12),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(
                Icons.receipt_long_rounded,
                color: order.color,
                size: dense ? 20 : 22,
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    order.title,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w800,
                      color: AppColors.ink,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    order.subtitle,
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.6),
                      fontWeight: FontWeight.w600,
                      fontSize: 13,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      _Pill(
                        label: order.status,
                        color: order.color.withOpacity(0.12),
                        textColor: order.color,
                        icon: Icons.check_circle_rounded,
                      ),
                      _Pill(
                        label: order.tag,
                        color: AppColors.ink.withOpacity(0.04),
                        textColor: AppColors.ink.withOpacity(0.75),
                        icon: Icons.local_fire_department_rounded,
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(width: 10),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Text(
                  order.price,
                  style: TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w900,
                    fontSize: dense ? 14 : 15,
                  ),
                ),
                const SizedBox(height: 6),
                TextButton(
                  onPressed:
                      () => Navigator.of(context).pushNamed(
                        AppRoutes.customerOrderDetail,
                        arguments: {
                          'id':
                              '#SL-202409-00${order.title.hashCode.abs() % 90}',
                          'title': order.title,
                          'status': order.status,
                          'statusColor': order.color,
                          'time': order.subtitle,
                          'address': 'Tòa nhà ABC, 123 Lê Lợi, Q.1, TP.HCM',
                          'contact': 'Nguyễn An (0909 123 456)',
                          'note': 'Giao tại quầy lễ tân, gọi trước 5 phút.',
                          'items': [
                            {'name': order.title, 'qty': 1, 'price': 55000},
                            {'name': 'Nước ép cam', 'qty': 1, 'price': 25000},
                          ],
                          'shipping': 12000,
                          'discount': 15000,
                          'subtotal': 80000,
                          'total': 77000,
                          'payment': 'Ví SmartLunch · Đã thanh toán',
                          'timeline':
                              order.status == 'Đang giao'
                                  ? 3
                                  : order.status == 'Đã xác nhận'
                                  ? 2
                                  : 4,
                        },
                      ),
                  style: TextButton.styleFrom(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 10,
                      vertical: 8,
                    ),
                    foregroundColor: order.color,
                  ),
                  child: const Text('Chi tiết'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _Pill extends StatelessWidget {
  final String label;
  final Color color;
  final Color textColor;
  final IconData icon;

  const _Pill({
    required this.label,
    required this.color,
    required this.textColor,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: textColor),
          const SizedBox(width: 6),
          Text(
            label,
            style: TextStyle(
              color: textColor,
              fontWeight: FontWeight.w700,
              fontSize: 12,
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  final String title;
  final String subtitle;

  const _SectionHeader({required this.title, required this.subtitle});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.w900,
            color: AppColors.ink,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          subtitle,
          style: TextStyle(
            color: AppColors.ink.withOpacity(0.6),
            fontWeight: FontWeight.w600,
          ),
        ),
      ],
    );
  }
}

class _ActionData {
  final String label;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  const _ActionData({
    required this.label,
    required this.icon,
    required this.color,
    required this.onTap,
  });
}

class _OrderData {
  final String title;
  final String subtitle;
  final String price;
  final String status;
  final String tag;
  final Color color;

  const _OrderData({
    required this.title,
    required this.subtitle,
    required this.price,
    required this.status,
    required this.tag,
    required this.color,
  });
}
