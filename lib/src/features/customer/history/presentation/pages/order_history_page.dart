import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class OrderHistoryPage extends StatelessWidget {
  const OrderHistoryPage({super.key});

  @override
  Widget build(BuildContext context) {
    final orders = [
      const _Order(
        code: '#SL1024',
        date: 'Hôm nay · 11:40',
        items: 'Cơm gà sốt chanh, Trà đào',
        total: '120.000đ',
        status: OrderStatus.delivered,
      ),
      const _Order(
        code: '#SL1023',
        date: 'Hôm qua · 12:05',
        items: 'Bún bò Huế, Sữa chua',
        total: '98.000đ',
        status: OrderStatus.completed,
      ),
      const _Order(
        code: '#SL1022',
        date: 'Thứ 6 · 11:50',
        items: 'Mì Ý bò bằm',
        total: '75.000đ',
        status: OrderStatus.refunded,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Lịch sử đặt hàng'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            onPressed: () {},
            icon: const Icon(Icons.filter_list_rounded),
          ),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            const _SummaryRow(),
            const SizedBox(height: 14),
            _DateChips(),
            const SizedBox(height: 14),
            ...orders.map(
              (order) => Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: _OrderTile(order: order),
              ),
            ),
            TextButton.icon(
              onPressed: () {},
              icon: const Icon(Icons.history_rounded),
              label: const Text('Xem thêm đơn cũ'),
            ),
          ],
        ),
      ),
    );
  }
}

class _SummaryRow extends StatelessWidget {
  const _SummaryRow();

  @override
  Widget build(BuildContext context) {
    final items = [
      _SummaryCard(
        label: 'Đơn tuần này',
        value: '12',
        icon: Icons.receipt_long_rounded,
        color: AppColors.customer,
      ),
      _SummaryCard(
        label: 'Chi tiêu',
        value: '1.240.000đ',
        icon: Icons.payments_rounded,
        color: AppColors.customer,
      ),
      _SummaryCard(
        label: 'Điểm thưởng',
        value: '2.480',
        icon: Icons.bolt_rounded,
        color: AppColors.customerAlt,
      ),
    ];

    return LayoutBuilder(
      builder: (context, constraints) {
        if (constraints.maxWidth < 380) {
          return Column(
            children:
                items
                    .map(
                      (i) => Padding(
                        padding: const EdgeInsets.only(bottom: 8),
                        child: i,
                      ),
                    )
                    .toList(),
          );
        }
        return Row(
          children: [
            for (var i = 0; i < items.length; i++) ...[
              Expanded(child: items[i]),
              if (i != items.length - 1) const SizedBox(width: 10),
            ],
          ],
        );
      },
    );
  }
}

class _SummaryCard extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final Color color;

  const _SummaryCard({
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
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
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: color.withOpacity(0.12),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: color),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.7),
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  value,
                  style: const TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w900,
                    fontSize: 16,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _DateChips extends StatelessWidget {
  final chips = const [
    _ChipData('Tuần này', true),
    _ChipData('Tháng này', false),
    _ChipData('3 tháng', false),
  ];

  _DateChips({super.key});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children:
            chips
                .map(
                  (c) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: ChoiceChip(
                      label: Text(c.label),
                      selected: c.selected,
                      onSelected: (_) {},
                      selectedColor: AppColors.customer.withOpacity(0.12),
                      labelStyle: TextStyle(
                        color:
                            c.selected
                                ? AppColors.customer
                                : AppColors.ink.withOpacity(0.75),
                        fontWeight: FontWeight.w800,
                      ),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                        side: BorderSide(
                          color:
                              c.selected
                                  ? AppColors.customer.withOpacity(0.4)
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

class _ChipData {
  final String label;
  final bool selected;

  const _ChipData(this.label, this.selected);
}

enum OrderStatus { delivered, completed, refunded }

class _Order {
  final String code;
  final String date;
  final String items;
  final String total;
  final OrderStatus status;

  const _Order({
    required this.code,
    required this.date,
    required this.items,
    required this.total,
    required this.status,
  });
}

class _OrderTile extends StatelessWidget {
  final _Order order;

  const _OrderTile({required this.order});

  Color _statusColor() {
    switch (order.status) {
      case OrderStatus.delivered:
        return AppColors.customerAlt;
      case OrderStatus.completed:
        return AppColors.customer;
      case OrderStatus.refunded:
        return AppColors.customer;
    }
  }

  String _statusLabel() {
    switch (order.status) {
      case OrderStatus.delivered:
        return 'Đã giao';
      case OrderStatus.completed:
        return 'Hoàn tất';
      case OrderStatus.refunded:
        return 'Đã hoàn tiền';
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _statusColor();
    return Container(
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
                  color: color.withOpacity(0.12),
                  shape: BoxShape.circle,
                ),
                child: Icon(Icons.shopping_bag_rounded, color: color),
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
                            order.code,
                            style: TextStyle(
                              color: AppColors.ink,
                              fontWeight: FontWeight.w900,
                              fontSize: 16,
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
                      order.date,
                      style: TextStyle(
                        color: AppColors.ink.withOpacity(0.6),
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          Text(
            order.items,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.75),
              fontWeight: FontWeight.w700,
            ),
          ),
          const SizedBox(height: 6),
          Row(
            children: [
              Text(
                'Tổng cộng',
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.65),
                  fontWeight: FontWeight.w700,
                ),
              ),
              const Spacer(),
              Text(
                order.total,
                style: const TextStyle(
                  color: AppColors.ink,
                  fontWeight: FontWeight.w900,
                  fontSize: 16,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          Row(
            children: [
              OutlinedButton.icon(
                onPressed: () {},
                icon: const Icon(Icons.receipt_long_rounded, size: 18),
                label: const Text('Chi tiết'),
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppColors.customer,
                  side: BorderSide(color: AppColors.customer.withOpacity(0.4)),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              TextButton(onPressed: () {}, child: const Text('Đặt lại')),
            ],
          ),
        ],
      ),
    );
  }
}
