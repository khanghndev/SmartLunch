import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';
import '../../../presentation/widgets/meal_cards.dart';

class MealDetailPage extends StatelessWidget {
  final MealData meal;

  const MealDetailPage({super.key, required this.meal});

  @override
  Widget build(BuildContext context) {
    final bottom = MediaQuery.of(context).padding.bottom;
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Chi tiết món ăn'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: Stack(
        children: [
          SingleChildScrollView(
            physics: const BouncingScrollPhysics(),
            padding: EdgeInsets.fromLTRB(20, 0, 20, 120 + bottom),
            child: Column(
              children: [
                const SizedBox(height: 12),
                _MealHero(meal: meal),
                const SizedBox(height: 20),
                _MealTitleSection(meal: meal),
                const SizedBox(height: 12),
                _HeroStatPanel(meal: meal),
                const SizedBox(height: 20),
                _PriceHeader(meal: meal),
                const SizedBox(height: 18),
                _MacroRow(meal: meal),
                const SizedBox(height: 20),
                _SectionCard(
                  title: 'Điểm nổi bật',
                  child: Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children:
                        meal.tags
                            .map(
                              (tag) => Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 12,
                                  vertical: 8,
                                ),
                                decoration: BoxDecoration(
                                  color: AppColors.customer.withOpacity(0.08),
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                child: Text(
                                  tag,
                                  style: const TextStyle(
                                    color: AppColors.customer,
                                    fontWeight: FontWeight.w800,
                                  ),
                                ),
                              ),
                            )
                            .toList(),
                  ),
                ),
                const SizedBox(height: 14),
                _SectionCard(
                  title: 'Thành phần chính',
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children:
                        meal.ingredients
                            .map(
                              (item) => Padding(
                                padding: const EdgeInsets.only(bottom: 8),
                                child: Row(
                                  children: [
                                    Container(
                                      width: 22,
                                      height: 22,
                                      decoration: BoxDecoration(
                                        color: AppColors.customer.withOpacity(
                                          0.12,
                                        ),
                                        shape: BoxShape.circle,
                                      ),
                                      child: const Icon(
                                        Icons.check_rounded,
                                        color: AppColors.customer,
                                        size: 14,
                                      ),
                                    ),
                                    const SizedBox(width: 10),
                                    Expanded(
                                      child: Text(
                                        item,
                                        style: TextStyle(
                                          color: AppColors.ink.withOpacity(0.8),
                                          fontWeight: FontWeight.w700,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            )
                            .toList(),
                  ),
                ),
                const SizedBox(height: 14),
                _SectionCard(
                  title: 'Khẩu phần & giao hàng',
                  child: Column(
                    children: const [
                      _InfoRow(
                        label: 'Khẩu phần',
                        value: '500g - đủ no cho bữa trưa',
                        icon: Icons.restaurant_rounded,
                      ),
                      SizedBox(height: 10),
                      _InfoRow(
                        label: 'Thời gian giao',
                        value: '10:30 - 11:30 (miễn phí nội bộ)',
                        icon: Icons.delivery_dining_rounded,
                      ),
                      SizedBox(height: 10),
                      _InfoRow(
                        label: 'Bảo quản',
                        value: 'Ngon nhất trong 2h, hâm nóng 1 phút',
                        icon: Icons.heat_pump_rounded,
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 14),
                _SectionCard(
                  title: 'Gợi ý ăn kèm',
                  child: Wrap(
                    spacing: 10,
                    runSpacing: 10,
                    children: const [
                      _SuggestChip(label: 'Nước ép cam', price: '25k'),
                      _SuggestChip(label: 'Súp bí đỏ', price: '18k'),
                      _SuggestChip(label: 'Salad mini', price: '22k'),
                    ],
                  ),
                ),
              ],
            ),
          ),
          _BottomBar(meal: meal, bottom: bottom),
        ],
      ),
    );
  }
}

class _MealHero extends StatefulWidget {
  final MealData meal;

  const _MealHero({required this.meal});

  @override
  State<_MealHero> createState() => _MealHeroState();
}

class _MealHeroState extends State<_MealHero> {
  final PageController _controller = PageController(viewportFraction: 0.96);
  int _index = 0;

  List<String> get _gallery => [
    widget.meal.image,
    '${widget.meal.image}_2',
    '${widget.meal.image}_3',
  ];

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final gallery = _gallery;
    return Stack(
      clipBehavior: Clip.none,
      children: [
        SizedBox(
          height: 360,
          child: PageView.builder(
            controller: _controller,
            physics: const BouncingScrollPhysics(),
            itemCount: gallery.length,
            onPageChanged: (i) => setState(() => _index = i),
            itemBuilder: (context, i) {
              final imageKey = gallery[i];
              return Padding(
                padding: EdgeInsets.only(
                  left: i == 0 ? 0 : 10,
                  right: i == gallery.length - 1 ? 0 : 10,
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(22),
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            colors: widget.meal.gradient,
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                          ),
                        ),
                      ),
                      Positioned.fill(
                        child: Image.asset(
                          'assets/images/$imageKey.png',
                          fit: BoxFit.cover,
                          errorBuilder: (_, __, ___) => const SizedBox(),
                        ),
                      ),
                      Container(
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            colors: [
                              Colors.black.withOpacity(0.12),
                              Colors.black.withOpacity(0.42),
                            ],
                          ),
                        ),
                      ),
                      const SizedBox.shrink(),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
        Positioned(
          bottom: 14,
          left: 0,
          right: 0,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: List.generate(
              gallery.length,
              (i) => AnimatedContainer(
                duration: const Duration(milliseconds: 200),
                height: 6,
                width: _index == i ? 16 : 8,
                margin: const EdgeInsets.symmetric(horizontal: 4),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(_index == i ? 0.9 : 0.55),
                  borderRadius: BorderRadius.circular(999),
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _MealTitleSection extends StatelessWidget {
  final MealData meal;

  const _MealTitleSection({required this.meal});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Wrap(
          spacing: 8,
          runSpacing: 6,
          children: [
            _Pill(label: meal.category),
            if (meal.badge.isNotEmpty) _Pill(label: meal.badge),
          ],
        ),
        const SizedBox(height: 10),
        Text(
          meal.name,
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.w900,
            color: AppColors.ink,
            letterSpacing: -0.15,
            height: 1.05,
            fontSize: 22,
          ),
        ),
        const SizedBox(height: 6),
        Text(
          meal.description,
          maxLines: 3,
          overflow: TextOverflow.ellipsis,
          style: TextStyle(
            color: AppColors.ink.withOpacity(0.75),
            height: 1.35,
            fontWeight: FontWeight.w600,
            fontSize: 13,
          ),
        ),
      ],
    );
  }
}

class _HeroStatPanel extends StatelessWidget {
  final MealData meal;

  const _HeroStatPanel({required this.meal});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 14,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Row(
        children: [
          Expanded(
            child: _HeroStat(
              icon: Icons.star_rounded,
              label: meal.rating.toStringAsFixed(1),
              caption: 'Đánh giá',
              color: Colors.amber[700]!,
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: _HeroStat(
              icon: Icons.timer_outlined,
              label: meal.prepTime,
              caption: 'Chuẩn bị',
              color: AppColors.customer,
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: _HeroStat(
              icon: Icons.local_fire_department_rounded,
              label: '${meal.calories} kcal',
              caption: 'Năng lượng',
              color: AppColors.customer,
            ),
          ),
        ],
      ),
    );
  }
}

class _HeroStat extends StatelessWidget {
  final IconData icon;
  final String label;
  final String caption;
  final Color color;

  const _HeroStat({
    required this.icon,
    required this.label,
    required this.caption,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          padding: const EdgeInsets.all(7),
          decoration: BoxDecoration(
            color: color.withOpacity(0.12),
            shape: BoxShape.circle,
          ),
          child: Icon(icon, color: color, size: 15),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w800,
                  color: AppColors.ink,
                  fontSize: 14,
                ),
              ),
              Text(
                caption,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.6),
                  fontWeight: FontWeight.w600,
                  fontSize: 11,
                  height: 1.15,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _PriceHeader extends StatelessWidget {
  final MealData meal;

  const _PriceHeader({required this.meal});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.03),
            blurRadius: 14,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
              decoration: BoxDecoration(
                color: AppColors.customer.withOpacity(0.06),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppColors.customer.withOpacity(0.18)),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '${_formatPriceFull(meal.price)}đ',
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.w900,
                      color: AppColors.customer,
                      letterSpacing: -0.2,
                      fontSize: 22,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Đã gồm VAT & phí đóng gói',
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.65),
                      fontWeight: FontWeight.w600,
                      fontSize: 13,
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(width: 12),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              _Pill(label: meal.category),
              if (meal.badge.isNotEmpty) ...[
                const SizedBox(height: 6),
                _Pill(label: meal.badge),
              ],
            ],
          ),
        ],
      ),
    );
  }
}

class _MacroRow extends StatelessWidget {
  final MealData meal;

  const _MacroRow({required this.meal});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: _MacroCard(
            label: 'Calo',
            value: '${meal.calories}kl',
            color: AppColors.customer,
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: _MacroCard(
            label: 'Protein',
            value: '${meal.protein}g',
            color: AppColors.customer,
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: _MacroCard(
            label: 'Carb',
            value: '${meal.carb}g',
            color: AppColors.customerAlt,
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: _MacroCard(
            label: 'Fat',
            value: '${meal.fat}g',
            color: const Color(0xFF6A11CB),
          ),
        ),
      ],
    );
  }
}

class _MacroCard extends StatelessWidget {
  final String label;
  final String value;
  final Color color;

  const _MacroCard({
    required this.label,
    required this.value,
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
            color: Colors.black.withOpacity(0.03),
            blurRadius: 12,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.65),
              fontWeight: FontWeight.w700,
              fontSize: 13,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            value,
            style: TextStyle(
              color: color,
              fontWeight: FontWeight.w800,
              fontSize: 15,
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionCard extends StatelessWidget {
  final String title;
  final Widget child;

  const _SectionCard({required this.title, required this.child});

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
            color: Colors.black.withOpacity(0.025),
            blurRadius: 10,
            offset: const Offset(0, 7),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.w900,
              color: AppColors.ink,
            ),
          ),
          const SizedBox(height: 10),
          child,
        ],
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;

  const _InfoRow({
    required this.label,
    required this.value,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: AppColors.customer.withOpacity(0.1),
            shape: BoxShape.circle,
          ),
          child: Icon(icon, color: AppColors.customer, size: 15),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.78),
                  fontWeight: FontWeight.w700,
                  fontSize: 13.5,
                ),
              ),
              Text(
                value,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.62),
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _SuggestChip extends StatelessWidget {
  final String label;
  final String price;

  const _SuggestChip({required this.label, required this.price});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      decoration: BoxDecoration(
        color: AppColors.customer.withOpacity(0.08),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            label,
            style: const TextStyle(
              color: AppColors.ink,
              fontWeight: FontWeight.w700,
              fontSize: 13,
            ),
          ),
          const SizedBox(width: 8),
          Text(
            price,
            style: const TextStyle(
              color: AppColors.customer,
              fontWeight: FontWeight.w800,
              fontSize: 13,
            ),
          ),
        ],
      ),
    );
  }
}

class _BottomBar extends StatelessWidget {
  final MealData meal;
  final double bottom;

  const _BottomBar({required this.meal, required this.bottom});

  @override
  Widget build(BuildContext context) {
    return Positioned(
      left: 0,
      right: 0,
      bottom: 0,
      child: Container(
        padding: EdgeInsets.fromLTRB(20, 10, 20, 10 + bottom),
        decoration: BoxDecoration(
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.06),
              blurRadius: 18,
              offset: const Offset(0, -6),
            ),
          ],
        ),
        child: Row(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Tổng',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w700,
                  ),
                ),
                Text(
                  '${_formatPriceFull(meal.price)}đ',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    fontWeight: FontWeight.w900,
                    color: AppColors.ink,
                  ),
                ),
              ],
            ),
            const SizedBox(width: 14),
            Expanded(
              child: ElevatedButton(
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.customer,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 13),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  elevation: 0,
                ),
                onPressed: () {},
                child: const Text(
                  'Thêm vào giỏ',
                  style: TextStyle(fontWeight: FontWeight.w800, fontSize: 15),
                ),
              ),
            ),
            const SizedBox(width: 10),
            Container(
              decoration: BoxDecoration(
                color: AppColors.customer.withOpacity(0.08),
                borderRadius: BorderRadius.circular(11),
                border: Border.all(color: AppColors.customer.withOpacity(0.25)),
              ),
              child: IconButton(
                icon: const Icon(
                  Icons.calendar_month_rounded,
                  color: AppColors.customer,
                ),
                onPressed: () {},
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _Pill extends StatelessWidget {
  final String label;

  const _Pill({required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: AppColors.customer.withOpacity(0.08),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: AppColors.customer.withOpacity(0.2)),
      ),
      child: Text(
        label,
        style: const TextStyle(
          color: AppColors.customer,
          fontWeight: FontWeight.w700,
          fontSize: 12,
        ),
      ),
    );
  }
}

String _formatPriceFull(int price) {
  final value = price.toString();
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
