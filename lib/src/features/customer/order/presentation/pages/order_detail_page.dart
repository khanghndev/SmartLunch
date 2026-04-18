import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class OrderDetailPage extends StatelessWidget {
  final dynamic data;

  const OrderDetailPage({super.key, this.data});

  Map<String, dynamic> get _order {
    final map = data;
    if (map is Map<String, dynamic>) {
      return map;
    }
    return {
      'id': '#SL-202409-235',
      'title': 'Combo văn phòng',
      'status': 'Đang giao',
      'statusColor': AppColors.customer,
      'time': 'Giao trước 11:30 hôm nay',
      'address': 'Tòa nhà ABC, 123 Lê Lợi, Q.1, TP.HCM',
      'contact': 'Nguyễn An (0909 123 456)',
      'note': 'Giao tại quầy lễ tân, gọi trước 5 phút.',
      'items': [
        {'name': 'Cơm gà nướng mật ong', 'qty': 1, 'price': 45000},
        {'name': 'Salad Hy Lạp', 'qty': 1, 'price': 32000},
        {'name': 'Nước ép cam', 'qty': 1, 'price': 25000},
      ],
      'shipping': 12000,
      'discount': 15000,
      'subtotal': 102000,
      'total': 99000,
      'payment': 'Ví SmartLunch · Đã thanh toán',
      'timeline': 3, // 1-4
    };
  }

  @override
  Widget build(BuildContext context) {
    final order = _order;
    final timelineStep = (order['timeline'] as int?) ?? 3;
    final statusColor =
        order['statusColor'] is Color
            ? order['statusColor'] as Color
            : AppColors.customer;

    return Scaffold(
      backgroundColor: const Color(0xFFF6F7F9),
      appBar: AppBar(
        title: const Text('Chi tiết đơn hàng'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(16, 16, 16, 24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _Header(
              id: order['id']?.toString() ?? '',
              title: order['title']?.toString() ?? '',
              status: order['status']?.toString() ?? '',
              statusColor: statusColor,
              time: order['time']?.toString() ?? '',
            ),
            const SizedBox(height: 14),
            _Timeline(currentStep: timelineStep, accent: statusColor),
            const SizedBox(height: 14),
            _SectionCard(
              title: 'Món đã đặt',
              icon: Icons.receipt_long_rounded,
              accent: AppColors.customer,
              child: Column(
                children: [
                  ..._buildItems(order),
                  const Divider(height: 20),
                  _PriceRow(
                    label: 'Tạm tính',
                    value: _formatMoney(order['subtotal']),
                  ),
                  const SizedBox(height: 6),
                  _PriceRow(
                    label: 'Giảm giá',
                    value: '-${_formatMoney(order['discount'])}',
                    highlight: true,
                  ),
                  const SizedBox(height: 6),
                  _PriceRow(
                    label: 'Phí giao',
                    value: _formatMoney(order['shipping']),
                  ),
                  const SizedBox(height: 10),
                  _PriceRow(
                    label: 'Tổng cộng',
                    value: _formatMoney(order['total']),
                    bold: true,
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            _SectionCard(
              title: 'Giao hàng',
              icon: Icons.delivery_dining_rounded,
              accent: AppColors.customerAlt,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _InfoRow(
                    icon: Icons.pin_drop_rounded,
                    label: 'Địa chỉ',
                    value: order['address']?.toString() ?? '',
                  ),
                  const SizedBox(height: 8),
                  _InfoRow(
                    icon: Icons.person_rounded,
                    label: 'Liên hệ',
                    value: order['contact']?.toString() ?? '',
                  ),
                  const SizedBox(height: 8),
                  _InfoRow(
                    icon: Icons.edit_note_rounded,
                    label: 'Ghi chú',
                    value: order['note']?.toString() ?? 'Không có',
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      _Chip(
                        'Ưu tiên trước 11:30',
                        color: AppColors.customerAlt,
                      ),
                      const SizedBox(width: 8),
                      _Chip('Đã thanh toán', color: AppColors.customer),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            _SectionCard(
              title: 'Thanh toán',
              icon: Icons.payments_rounded,
              accent: AppColors.customerAlt,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _InfoRow(
                    icon: Icons.account_balance_wallet_rounded,
                    label: 'Phương thức',
                    value: order['payment']?.toString() ?? 'Chưa thanh toán',
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(
                        child: OutlinedButton.icon(
                          onPressed: () {},
                          icon: const Icon(Icons.headset_mic_rounded),
                          label: const Text('Liên hệ hỗ trợ'),
                          style: OutlinedButton.styleFrom(
                            foregroundColor: AppColors.customerAlt,
                            side: const BorderSide(
                              color: AppColors.customerAlt,
                            ),
                            padding: const EdgeInsets.symmetric(vertical: 12),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: DecoratedBox(
                          decoration: BoxDecoration(
                            gradient: const LinearGradient(
                              colors: [
                                AppColors.customer,
                                AppColors.customerAlt,
                              ],
                              begin: Alignment.topLeft,
                              end: Alignment.bottomRight,
                            ),
                            borderRadius: BorderRadius.circular(12),
                            boxShadow: [
                              BoxShadow(
                                color: AppColors.customer.withOpacity(0.28),
                                blurRadius: 16,
                                offset: const Offset(0, 8),
                              ),
                            ],
                          ),
                          child: ElevatedButton.icon(
                            onPressed: () {},
                            icon: const Icon(
                              Icons.local_shipping_rounded,
                              color: Colors.white,
                            ),
                            label: const Text(
                              'Theo dõi giao',
                              style: TextStyle(fontWeight: FontWeight.w800),
                            ),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: Colors.transparent,
                              shadowColor: Colors.transparent,
                              foregroundColor: Colors.white,
                              padding: const EdgeInsets.symmetric(vertical: 12),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(12),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () {},
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.ink.withOpacity(0.85),
                      side: BorderSide(color: AppColors.ink.withOpacity(0.18)),
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(14),
                      ),
                    ),
                    child: const Text(
                      'Mua lại đơn này',
                      style: TextStyle(fontWeight: FontWeight.w800),
                    ),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      color: AppColors.customer.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(14),
                      border: Border.all(
                        color: AppColors.customer.withOpacity(0.4),
                      ),
                    ),
                    child: ElevatedButton(
                      onPressed: () {},
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.transparent,
                        shadowColor: Colors.transparent,
                        foregroundColor: AppColors.customer,
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(14),
                        ),
                      ),
                      child: const Text(
                        'Chỉnh sửa & đặt lại',
                        style: TextStyle(fontWeight: FontWeight.w800),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  List<Widget> _buildItems(Map<String, dynamic> order) {
    final items = order['items'];
    if (items is List) {
      return items
          .map(
            (item) => _ItemRow(
              name: item['name']?.toString() ?? '',
              qty: item['qty'] is num ? (item['qty'] as num).toInt() : 1,
              priceText: _formatMoney(
                item['price'] is num ? (item['price'] as num).toInt() : 0,
              ),
            ),
          )
          .toList();
    }
    return [];
  }

  String _formatMoney(dynamic value) {
    if (value is num) {
      final text = value.toStringAsFixed(0);
      return '${_addSeparator(text)}đ';
    }
    return value?.toString() ?? '0đ';
  }

  String _addSeparator(String value) {
    final buffer = StringBuffer();
    for (var i = 0; i < value.length; i++) {
      final positionFromEnd = value.length - i;
      buffer.write(value[i]);
      if (positionFromEnd > 1 && positionFromEnd % 3 == 1) {
        buffer.write('.');
      }
    }
    return buffer.toString();
  }
}

class _Header extends StatelessWidget {
  final String id;
  final String title;
  final String status;
  final String time;
  final Color statusColor;

  const _Header({
    required this.id,
    required this.title,
    required this.status,
    required this.time,
    required this.statusColor,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [statusColor, statusColor.withOpacity(0.85)],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: statusColor.withOpacity(0.28),
            blurRadius: 20,
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
                  color: Colors.white.withOpacity(0.16),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  Icons.receipt_long_rounded,
                  color: Colors.white,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      id,
                      style: const TextStyle(
                        color: Colors.white70,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      title,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                        fontWeight: FontWeight.w900,
                      ),
                    ),
                  ],
                ),
              ),
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 8,
                ),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(999),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.radio_button_checked_rounded,
                      size: 14,
                      color: statusColor,
                    ),
                    const SizedBox(width: 6),
                    Text(
                      status,
                      style: TextStyle(
                        color: statusColor,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Icon(
                Icons.schedule_rounded,
                size: 18,
                color: Colors.white.withOpacity(0.9),
              ),
              const SizedBox(width: 6),
              Text(
                time,
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _Timeline extends StatelessWidget {
  final int currentStep;
  final Color accent;

  const _Timeline({required this.currentStep, required this.accent});

  @override
  Widget build(BuildContext context) {
    const steps = ['Đã xác nhận', 'Đang chuẩn bị', 'Đang giao', 'Hoàn tất'];
    const indicatorSize = 26.0;
    const lineThickness = 3.0;
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.ink.withOpacity(0.05)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        children: List.generate(steps.length, (index) {
          final active = index + 1 <= currentStep;
          final passed = index + 1 < currentStep;
          return Expanded(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Row(
                  children: [
                    Expanded(
                      child:
                          index == 0
                              ? const SizedBox.shrink()
                              : Container(
                                height: lineThickness,
                                color:
                                    passed
                                        ? accent
                                        : AppColors.ink.withOpacity(0.08),
                              ),
                    ),
                    Container(
                      width: indicatorSize,
                      height: indicatorSize,
                      decoration: BoxDecoration(
                        color: active ? accent : Colors.white,
                        borderRadius: BorderRadius.circular(indicatorSize),
                        border: Border.all(
                          color:
                              active ? accent : AppColors.ink.withOpacity(0.14),
                          width: 2,
                        ),
                      ),
                      child: Icon(
                        active ? Icons.check_rounded : Icons.circle_outlined,
                        size: 14,
                        color:
                            active
                                ? Colors.white
                                : AppColors.ink.withOpacity(0.55),
                      ),
                    ),
                    Expanded(
                      child:
                          index == steps.length - 1
                              ? const SizedBox.shrink()
                              : Container(
                                height: lineThickness,
                                color:
                                    active
                                        ? accent
                                        : AppColors.ink.withOpacity(0.08),
                              ),
                    ),
                  ],
                ),
                const SizedBox(height: 10),
                Text(
                  steps[index],
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    color: active ? accent : AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w800,
                    fontSize: 9,
                  ),
                ),
              ],
            ),
          );
        }),
      ),
    );
  }
}

class _SectionCard extends StatelessWidget {
  final String title;
  final IconData icon;
  final Widget child;
  final Color accent;

  const _SectionCard({
    required this.title,
    required this.icon,
    required this.child,
    required this.accent,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.ink.withOpacity(0.04)),
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
                  color: accent.withOpacity(0.14),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, color: accent),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  title,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w900,
                    color: AppColors.ink,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          child,
        ],
      ),
    );
  }
}

class _ItemRow extends StatelessWidget {
  final String name;
  final int qty;
  final String priceText;

  const _ItemRow({
    required this.name,
    required this.qty,
    required this.priceText,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  name,
                  style: Theme.of(
                    context,
                  ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w800),
                ),
                const SizedBox(height: 4),
                Text(
                  'x$qty',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
          Text(
            priceText,
            style: TextStyle(color: AppColors.ink, fontWeight: FontWeight.w800),
          ),
        ],
      ),
    );
  }
}

class _PriceRow extends StatelessWidget {
  final String label;
  final String value;
  final bool bold;
  final bool highlight;

  const _PriceRow({
    required this.label,
    required this.value,
    this.bold = false,
    this.highlight = false,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Text(
          label,
          style: TextStyle(
            color: AppColors.ink.withOpacity(0.7),
            fontWeight: FontWeight.w700,
          ),
        ),
        const Spacer(),
        Text(
          value,
          style: TextStyle(
            color: highlight ? Colors.redAccent : AppColors.ink,
            fontWeight: bold ? FontWeight.w900 : FontWeight.w800,
          ),
        ),
      ],
    );
  }
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _InfoRow({
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: AppColors.ink.withOpacity(0.04),
            borderRadius: BorderRadius.circular(10),
          ),
          child: Icon(icon, size: 18, color: AppColors.ink.withOpacity(0.7)),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: const TextStyle(
                  fontWeight: FontWeight.w800,
                  fontSize: 13,
                ),
              ),
              const SizedBox(height: 4),
              Text(
                value,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.7),
                  height: 1.35,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _Chip extends StatelessWidget {
  final String label;
  final Color? color;

  const _Chip(this.label, {this.color});

  @override
  Widget build(BuildContext context) {
    final baseColor = color ?? AppColors.customer;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: baseColor.withOpacity(0.08),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: baseColor.withOpacity(0.2)),
      ),
      child: Text(
        label,
        style: TextStyle(
          color: baseColor,
          fontWeight: FontWeight.w800,
          fontSize: 12,
        ),
      ),
    );
  }
}
