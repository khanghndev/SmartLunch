import 'package:flutter/material.dart';

import '../../../../../app/app_routes.dart';
import '../../../../../core/constants/app_colors.dart';

class DeliveryDetailPage extends StatelessWidget {
  const DeliveryDetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    final timeline = const [
      _Step(label: 'Nhận đơn', time: '08:10', done: true),
      _Step(label: 'Đang giao', time: '08:40', done: true),
      _Step(label: 'Hoàn tất', time: '--', done: false),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Chi tiết giao hàng'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.courierProof),
            icon: const Icon(Icons.camera_alt_outlined),
          ),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            _HeaderCard(timeline: timeline),
            const SizedBox(height: 14),
            const _InfoCard(),
            const SizedBox(height: 14),
            const _ActionsCard(),
          ],
        ),
      ),
    );
  }
}

class _HeaderCard extends StatelessWidget {
  final List<_Step> timeline;

  const _HeaderCard({required this.timeline});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.courier, AppColors.courierAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.courier.withOpacity(0.24),
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
                child: const Icon(Icons.local_shipping, color: Colors.white),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Ca sáng · Văn phòng Q1',
                      style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.w900,
                        fontSize: 18,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '#DL-2301 · 08:30 · 4 điểm dừng',
                      style: TextStyle(
                        color: Colors.white.withOpacity(0.9),
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: timeline.map((s) => _StepWidget(step: s)).toList(),
          ),
        ],
      ),
    );
  }
}

class _Step {
  final String label;
  final String time;
  final bool done;

  const _Step({required this.label, required this.time, required this.done});
}

class _StepWidget extends StatelessWidget {
  final _Step step;

  const _StepWidget({required this.step});

  @override
  Widget build(BuildContext context) {
    final color = step.done ? const Color(0xFF2BAE66) : Colors.white;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: color.withOpacity(0.18),
            shape: BoxShape.circle,
            border: Border.all(color: color.withOpacity(0.4)),
          ),
          child: Icon(
            step.done ? Icons.check_rounded : Icons.radio_button_unchecked,
            color: color,
          ),
        ),
        const SizedBox(height: 6),
        Text(
          step.label,
          style: const TextStyle(
            color: Colors.white,
            fontWeight: FontWeight.w700,
          ),
        ),
        Text(
          step.time,
          style: TextStyle(
            color: Colors.white.withOpacity(0.85),
            fontWeight: FontWeight.w600,
          ),
        ),
      ],
    );
  }
}

class _InfoCard extends StatelessWidget {
  const _InfoCard();

  @override
  Widget build(BuildContext context) {
    final rows = const [
      _InfoRow(
        label: 'Địa điểm giao',
        value: '10 Nguyễn Trãi, Q1, Tầng 12 - Lễ tân',
      ),
      _InfoRow(label: 'Liên hệ', value: 'Chị Lan · 0901 234 567'),
      _InfoRow(
        label: 'Ghi chú',
        value: 'Giao trước 12:00, gọi lễ tân nếu không vào được.',
      ),
    ];

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
        children:
            rows
                .map(
                  (r) => Padding(
                    padding: const EdgeInsets.symmetric(vertical: 6),
                    child: r,
                  ),
                )
                .toList(),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;

  const _InfoRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: AppColors.courier.withOpacity(0.12),
            shape: BoxShape.circle,
          ),
          child: const Icon(Icons.info_outline, color: AppColors.courier),
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
                  fontWeight: FontWeight.w800,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _ActionsCard extends StatelessWidget {
  const _ActionsCard();

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
      child: Wrap(
        spacing: 10,
        runSpacing: 10,
        children: [
          _ActionButton(
            label: 'Bắt đầu giao',
            icon: Icons.play_circle_fill_rounded,
            color: AppColors.courier,
            onTap: () {},
          ),
          _ActionButton(
            label: 'Mở bản đồ',
            icon: Icons.navigation_rounded,
            color: const Color(0xFF1F3C88),
            onTap:
                () =>
                    Navigator.of(context).pushNamed(AppRoutes.courierRouteMap),
          ),
          _ActionButton(
            label: 'Chụp Minh chứng',
            icon: Icons.camera_alt_outlined,
            color: const Color(0xFFF4A261),
            onTap:
                () => Navigator.of(context).pushNamed(AppRoutes.courierProof),
          ),
        ],
      ),
    );
  }
}

class _ActionButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  const _ActionButton({
    required this.label,
    required this.icon,
    required this.color,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return OutlinedButton.icon(
      onPressed: onTap,
      style: OutlinedButton.styleFrom(
        foregroundColor: color,
        side: BorderSide(color: color.withOpacity(0.4)),
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
      icon: Icon(icon),
      label: Text(label, style: const TextStyle(fontWeight: FontWeight.w800)),
    );
  }
}
