import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';

class QuickActionsSection extends StatelessWidget {
  const QuickActionsSection({super.key});

  @override
  Widget build(BuildContext context) {
    final actions = [
      _ActionData(
        icon: Icons.menu_book_rounded,
        label: 'Thực đơn',
        description: 'Xem món hôm nay',
        gradient: const [AppColors.customer, AppColors.customerAlt],
        route: AppRoutes.customerMenu,
      ),
      _ActionData(
        icon: Icons.shopping_bag_rounded,
        label: 'Đặt nhanh',
        description: 'Chốt bữa trưa',
        gradient: const [Color(0xFF1F3C88), Color(0xFF5C7DC7)],
        route: AppRoutes.customerOrder,
      ),
      _ActionData(
        icon: Icons.payments_rounded,
        label: 'Thanh toán',
        description: 'Ví, thẻ & hóa đơn',
        gradient: const [Color(0xFF2A9C9B), Color(0xFF0F6B6F)],
        route: AppRoutes.customerPayment,
      ),
    ];

    return Row(
      children: [
        for (int i = 0; i < actions.length; i++) ...[
          Expanded(child: _QuickActionCard(data: actions[i])),
          if (i != actions.length - 1) const SizedBox(width: 12),
        ],
      ],
    );
  }
}

class _QuickActionCard extends StatelessWidget {
  final _ActionData data;

  const _QuickActionCard({required this.data});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: () => Navigator.of(context).pushNamed(data.route),
        borderRadius: BorderRadius.circular(18),
        child: Container(
          padding: const EdgeInsets.all(16),
          constraints: const BoxConstraints(minHeight: 140),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: data.gradient,
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(18),
            boxShadow: [
              BoxShadow(
                color: data.gradient.first.withOpacity(0.35),
                blurRadius: 18,
                offset: const Offset(0, 10),
                spreadRadius: -5,
              ),
            ],
          ),
          child: Stack(
            children: [
              Positioned(
                top: -18,
                right: -18,
                child: Container(
                  width: 70,
                  height: 70,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: Colors.white.withOpacity(0.14),
                  ),
                ),
              ),
              Positioned(
                bottom: -22,
                left: -12,
                child: Container(
                  width: 80,
                  height: 80,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: Colors.white.withOpacity(0.08),
                  ),
                ),
              ),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: Colors.white.withOpacity(0.24),
                      borderRadius: BorderRadius.circular(14),
                    ),
                    child: Icon(
                      data.icon,
                      color: Colors.white,
                      size: 24,
                    ),
                  ),
                  const SizedBox(height: 14),
                  Text(
                    data.label,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.w800,
                        ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    data.description,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Colors.white.withOpacity(0.9),
                        ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _ActionData {
  final IconData icon;
  final String label;
  final String description;
  final List<Color> gradient;
  final String route;

  const _ActionData({
    required this.icon,
    required this.label,
    required this.description,
    required this.gradient,
    required this.route,
  });
}
