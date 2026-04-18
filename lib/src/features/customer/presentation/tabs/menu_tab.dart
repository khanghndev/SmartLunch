import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../widgets/meal_cards.dart';

class MenuTab extends StatelessWidget {
  final double bottomInset;
  final bool showBack;

  const MenuTab({super.key, required this.bottomInset, this.showBack = false});

  @override
  Widget build(BuildContext context) {
    final meals = suggestedMeals;

    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFFF5F7FB), Color(0xFFE8EFF5)],
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
        ),
      ),
      child: SafeArea(
        child: CustomScrollView(
          physics: const BouncingScrollPhysics(),
          slivers: [
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 10, 16, 0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _MenuHeader(showBack: showBack),
                    const SizedBox(height: 10),
                    const _SearchBar(),
                    const SizedBox(height: 8),
                    const _FilterChips(),
                  ],
                ),
              ),
            ),
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(20, 24, 20, 0),
                child: const _MenuSectionTitle(
                  title: 'Combo & ưu đãi',
                  subtitle: 'Giảm giá, giao nhanh và đủ dinh dưỡng',
                ),
              ),
            ),
            SliverToBoxAdapter(
              child: SizedBox(
                height: 170,
                child: Padding(
                  padding: const EdgeInsets.only(left: 20, top: 12),
                  child: _ComboRail(),
                ),
              ),
            ),
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(20, 24, 20, 8),
                child: const _MenuSectionTitle(
                  title: 'Món nổi bật',
                  subtitle: 'Được đặt nhiều tuần này',
                ),
              ),
            ),
            SliverPadding(
              padding: EdgeInsets.fromLTRB(20, 0, 20, 24 + bottomInset),
              sliver: SliverList(
                delegate: SliverChildBuilderDelegate((context, index) {
                  final meal = meals[index];
                  return Padding(
                    padding: EdgeInsets.only(
                      bottom: index == meals.length - 1 ? 0 : 12,
                    ),
                    child: MiniMealCard(meal: meal, compact: true),
                  );
                }, childCount: meals.length),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

String _formatPriceShort(int price) {
  if (price >= 1000) {
    final value = price / 1000;
    return value % 1 == 0
        ? '${value.toStringAsFixed(0)}k'
        : '${value.toStringAsFixed(1)}k';
  }
  return price.toString();
}

class _MenuHeader extends StatelessWidget {
  final bool showBack;

  const _MenuHeader({required this.showBack});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            if (showBack)
              Container(
                margin: const EdgeInsets.only(right: 10),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.06),
                      blurRadius: 10,
                      offset: const Offset(0, 6),
                    ),
                  ],
                ),
                child: IconButton(
                  icon: const Icon(Icons.arrow_back_rounded),
                  color: AppColors.ink,
                  onPressed: () => Navigator.of(context).pop(),
                ),
              ),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Thực đơn hôm nay',
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.w800,
                      color: AppColors.ink,
                      letterSpacing: -0.2,
                      fontSize: 24,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Chọn nhanh, giao trước 11:30, menu healthy và chỉn chu.',
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: AppColors.ink.withOpacity(0.6),
                      fontSize: 14,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            Container(
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(12),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.03),
                    blurRadius: 10,
                    offset: const Offset(0, 6),
                  ),
                ],
              ),
              child: IconButton(
                icon: const Icon(Icons.notifications_outlined),
                color: AppColors.ink.withOpacity(0.75),
                onPressed: () {},
              ),
            ),
          ],
        ),
        const SizedBox(height: 10),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: const [
            _StatusPill(
              label: 'Miễn phí ship nội bộ',
              icon: Icons.delivery_dining_rounded,
              color: AppColors.customer,
            ),
            _StatusPill(
              label: 'Eat clean - ít dầu mỡ',
              icon: Icons.eco_rounded,
              color: AppColors.customerAlt,
            ),
            _StatusPill(
              label: 'Ưu đãi đến 25%',
              icon: Icons.local_offer_rounded,
              color: AppColors.customer,
            ),
          ],
        ),
      ],
    );
  }
}

class _StatusPill extends StatelessWidget {
  final String label;
  final IconData icon;
  final Color color;

  const _StatusPill({
    required this.label,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: color.withOpacity(0.08),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: color.withOpacity(0.16)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16, color: color.withOpacity(0.9)),
          const SizedBox(width: 6),
          Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.85),
              fontWeight: FontWeight.w600,
              fontSize: 13.5,
            ),
          ),
        ],
      ),
    );
  }
}

class _SearchBar extends StatefulWidget {
  const _SearchBar();

  @override
  State<_SearchBar> createState() => _SearchBarState();
}

class _SearchBarState extends State<_SearchBar> {
  late final FocusNode _focusNode;
  bool _activated = false;

  @override
  void initState() {
    super.initState();
    _focusNode = FocusNode();
  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  void _activate() {
    if (_activated) return;
    setState(() => _activated = true);
    Future.microtask(() => _focusNode.requestFocus());
  }

  @override
  Widget build(BuildContext context) {
    return TextField(
      focusNode: _focusNode,
      autofocus: false,
      readOnly: !_activated,
      onTap: _activate,
      textAlignVertical: TextAlignVertical.center,
      style: TextStyle(
        color: AppColors.ink.withOpacity(0.9),
        fontWeight: FontWeight.w600,
        fontSize: 14,
        height: 1.2,
      ),
      decoration: InputDecoration(
        hintText: 'Tìm món, combo, eat clean...',
        hintStyle: TextStyle(
          color: AppColors.ink.withOpacity(0.5),
          fontWeight: FontWeight.w600,
          fontSize: 13.5,
        ),
        prefixIcon: Icon(Icons.search, color: AppColors.ink.withOpacity(0.65)),

        filled: true,
        fillColor: Colors.white.withOpacity(0.95),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16),
          borderSide: BorderSide(color: AppColors.ink.withOpacity(0.08)),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16),
          borderSide: BorderSide(
            color: AppColors.customer.withOpacity(0.6),
            width: 1.5,
          ),
        ),
        contentPadding: const EdgeInsets.symmetric(vertical: 14, horizontal: 0),
      ),
    );
  }
}

class _FilterChips extends StatelessWidget {
  const _FilterChips();

  @override
  Widget build(BuildContext context) {
    final filters = [
      'Tất cả',
      'Healthy',
      'Giảm giá',
      'Nhiều protein',
      'Đồ chay',
      'Low carb',
    ];

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      physics: const BouncingScrollPhysics(),
      child: Row(
        children:
            filters
                .map(
                  (label) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 12,
                        vertical: 7,
                      ),
                      decoration: BoxDecoration(
                        color: Colors.white.withOpacity(0.9),
                        borderRadius: BorderRadius.circular(14),
                        border: Border.all(
                          color: AppColors.ink.withOpacity(0.08),
                        ),
                      ),
                      child: Text(
                        label,
                        style: const TextStyle(
                          fontWeight: FontWeight.w600,
                          color: AppColors.ink,
                          fontSize: 13,
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

class _ComboRail extends StatelessWidget {
  final combos = const [
    _ComboData(
      title: 'Combo Healthy',
      subtitle: 'Eat clean - ít dầu mỡ',
      badge: '-15%',
      gradient: [AppColors.customer, AppColors.customerAlt],
      icon: Icons.eco_rounded,
    ),
    _ComboData(
      title: 'Combo Nhanh',
      subtitle: 'Mang đi trong 10 phút',
      badge: 'NEW',
      gradient: [AppColors.customerAlt, AppColors.customerAlt],
      icon: Icons.flash_on_rounded,
    ),
    _ComboData(
      title: 'Combo Premium',
      subtitle: 'Hải sản, bò cao cấp',
      badge: '-20%',
      gradient: [Color(0xFF6A11CB), Color(0xFF2575FC)],
      icon: Icons.workspace_premium_rounded,
    ),
  ];

  const _ComboRail();

  @override
  Widget build(BuildContext context) {
    return ListView.separated(
      scrollDirection: Axis.horizontal,
      physics: const BouncingScrollPhysics(),
      itemBuilder: (context, i) {
        final combo = combos[i];
        return _ComboCard(combo: combo);
      },
      separatorBuilder: (_, __) => const SizedBox(width: 12),
      itemCount: combos.length,
    );
  }
}

class _ComboCard extends StatelessWidget {
  final _ComboData combo;

  const _ComboCard({required this.combo});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 230,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: combo.gradient,
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: combo.gradient.first.withOpacity(0.25),
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
                padding: const EdgeInsets.symmetric(
                  horizontal: 10,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.16),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  combo.badge,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                  ),
                ),
              ),
              const Spacer(),
              Icon(combo.icon, color: Colors.white),
            ],
          ),
          const Spacer(),
          Text(
            combo.title,
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.w900,
              fontSize: 18,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            combo.subtitle,
            style: TextStyle(
              color: Colors.white.withOpacity(0.9),
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }
}

class _ComboData {
  final String title;
  final String subtitle;
  final String badge;
  final List<Color> gradient;
  final IconData icon;

  const _ComboData({
    required this.title,
    required this.subtitle,
    required this.badge,
    required this.gradient,
    required this.icon,
  });
}

class _MenuSectionTitle extends StatelessWidget {
  final String title;
  final String subtitle;

  const _MenuSectionTitle({required this.title, required this.subtitle});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.w900,
            color: AppColors.ink,
          ),
        ),
        const SizedBox(height: 6),
        Text(
          subtitle,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: AppColors.ink.withOpacity(0.6),
          ),
        ),
      ],
    );
  }
}
