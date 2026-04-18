import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';

class MealData {
  final String name;
  final String description;
  final int price;
  final double rating;
  final String image;
  final String category;
  final int calories;
  final int protein;
  final int carb;
  final int fat;
  final String prepTime;
  final List<String> tags;
  final String badge;
  final List<String> ingredients;
  final List<Color> gradient;

  const MealData({
    required this.name,
    required this.description,
    required this.price,
    required this.rating,
    required this.image,
    required this.category,
    this.calories = 520,
    this.protein = 32,
    this.carb = 55,
    this.fat = 18,
    this.prepTime = '20-25 phút',
    this.tags = const [],
    this.badge = '',
    this.ingredients = const [],
    this.gradient = const [AppColors.customer, AppColors.customerAlt],
  });
}

const suggestedMeals = [
  MealData(
    name: 'Cơm gà nướng mật ong',
    description:
        'Đùi gà nướng lò, cơm gạo lứt và salad xanh với sốt mật ong tỏi.',
    price: 45000,
    rating: 4.8,
    image: 'meal1',
    category: 'Best seller',
    calories: 560,
    protein: 38,
    carb: 54,
    fat: 17,
    prepTime: '18-22 phút',
    tags: ['Ít dầu mỡ', 'Cao protein'],
    badge: '-15%',
    ingredients: [
      'Đùi gà nướng sốt mật ong',
      'Cơm gạo lứt',
      'Salad dưa leo - cà chua',
      'Sốt mè rang',
    ],
    gradient: [AppColors.customer, Color(0xFF2AA59B)],
  ),
  MealData(
    name: 'Phở bò truyền thống',
    description: 'Nước dùng đậm đà ninh xương 12h, bò tái chín vừa.',
    price: 55000,
    rating: 4.9,
    image: 'meal2',
    category: 'Signature',
    calories: 480,
    protein: 35,
    carb: 58,
    fat: 12,
    prepTime: '12-15 phút',
    tags: ['Ít béo', 'Nước dùng ngọt xương'],
    badge: 'Chef pick',
    ingredients: [
      'Bánh phở tươi',
      'Nạm - gầu - bò tái',
      'Hành lá, rau thơm',
      'Nước dùng hầm xương 12h',
    ],
    gradient: [AppColors.customerAlt, AppColors.customerAlt],
  ),
  MealData(
    name: 'Salad gà nướng',
    description: 'Xà lách, gà nướng than hoa, sốt yogurt ít béo.',
    price: 40000,
    rating: 4.7,
    image: 'meal3',
    category: 'Eat clean',
    calories: 360,
    protein: 30,
    carb: 28,
    fat: 12,
    prepTime: '10-12 phút',
    tags: ['Eat clean', 'Nhiều rau'],
    badge: 'NEW',
    ingredients: [
      'Ức gà nướng',
      'Xà lách, cà chua bi',
      'Hạt quinoa',
      'Sốt yogurt chanh dây',
    ],
    gradient: [Color(0xFF34A0A4), Color(0xFF88D4D1)],
  ),
  MealData(
    name: 'Bún chả Hà Nội',
    description: 'Chả thịt than hoa, nước chấm chua ngọt, bún tươi mỗi sáng.',
    price: 50000,
    rating: 4.8,
    image: 'meal4',
    category: 'Món truyền thống',
    calories: 520,
    protein: 32,
    carb: 60,
    fat: 16,
    prepTime: '15-18 phút',
    tags: ['Nướng than hoa', 'Đủ no cho trưa'],
    ingredients: [
      'Chả thịt nướng',
      'Bún tươi',
      'Rau sống',
      'Nước mắm chua ngọt',
    ],
    gradient: [AppColors.customer, AppColors.customerAlt],
  ),
  MealData(
    name: 'Cá hồi áp chảo sốt miso',
    description: 'Cá hồi Nauy áp chảo, rau củ nướng, sốt miso ngọt nhẹ.',
    price: 85000,
    rating: 4.95,
    image: 'meal5',
    category: 'Premium',
    calories: 610,
    protein: 42,
    carb: 48,
    fat: 24,
    prepTime: '16-20 phút',
    tags: ['Omega-3', 'Ít tinh bột'],
    badge: 'Premium',
    ingredients: [
      'Phi lê cá hồi Nauy',
      'Khoai lang nướng',
      'Bông cải xanh hấp',
      'Sốt miso mật ong',
    ],
    gradient: [Color(0xFF6A11CB), Color(0xFF2575FC)],
  ),
  MealData(
    name: 'Bò lúc lắc sốt tiêu đen',
    description: 'Thăn bò lúc lắc, khoai tây quế, rau củ áp chảo.',
    price: 78000,
    rating: 4.9,
    image: 'meal6',
    category: 'Năng lượng',
    calories: 670,
    protein: 46,
    carb: 62,
    fat: 22,
    prepTime: '14-18 phút',
    tags: ['Cao protein', 'No lâu'],
    ingredients: [
      'Thăn bò lúc lắc',
      'Khoai tây quế',
      'Ớt chuông, hành tây',
      'Sốt tiêu đen tươi',
    ],
    gradient: [Color(0xFF0E7490), Color(0xFF38BDF8)],
  ),
];

class MiniMealCard extends StatelessWidget {
  final MealData meal;
  final bool compact;

  const MiniMealCard({super.key, required this.meal, this.compact = false});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        borderRadius: BorderRadius.circular(18),
        onTap:
            () => Navigator.of(
              context,
            ).pushNamed(AppRoutes.customerMealDetail, arguments: meal),
        child: Ink(
          padding: const EdgeInsets.all(14),
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
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _MealThumbnail(meal: meal),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Text(
                            meal.name,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: Theme.of(
                              context,
                            ).textTheme.titleMedium?.copyWith(
                              fontWeight: FontWeight.w800,
                              color: AppColors.ink,
                              letterSpacing: -0.2,
                              fontSize: compact ? 15 : null,
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Text(
                          '${_formatPriceShort(meal.price)}đ',
                          style: const TextStyle(
                            color: AppColors.customer,
                            fontWeight: FontWeight.w900,
                            fontSize: 14,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 6),
                    if (meal.tags.isNotEmpty)
                      Padding(
                        padding: const EdgeInsets.only(bottom: 4),
                        child: Wrap(
                          spacing: 6,
                          runSpacing: 6,
                          children:
                              meal.tags
                                  .take(compact ? 1 : 2)
                                  .map((t) => _TagChip(t, dense: compact))
                                  .toList(),
                        ),
                      ),
                    Text(
                      meal.description,
                      maxLines: compact ? 1 : 2,
                      overflow: TextOverflow.ellipsis,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: AppColors.ink.withOpacity(0.6),
                        height: 1.4,
                        fontSize: compact ? 12.5 : null,
                      ),
                    ),
                    const SizedBox(height: 10),
                    Wrap(
                      spacing: 10,
                      runSpacing: 6,
                      crossAxisAlignment: WrapCrossAlignment.center,
                      children: [
                        _Stat(
                          icon: Icons.star_rounded,
                          label: meal.rating.toStringAsFixed(1),
                        ),
                        _Stat(icon: Icons.timer_outlined, label: meal.prepTime),
                        _Stat(
                          icon: Icons.local_fire_department_rounded,
                          label: '${meal.calories} kcal',
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 12),
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Container(
                    width: compact ? 40 : 44,
                    height: compact ? 40 : 44,
                    decoration: BoxDecoration(
                      color: AppColors.customer.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: IconButton(
                      onPressed:
                          () => Navigator.of(context).pushNamed(
                            AppRoutes.customerMealDetail,
                            arguments: meal,
                          ),
                      icon: const Icon(
                        Icons.add_rounded,
                        color: AppColors.customer,
                      ),
                    ),
                  ),
                  const SizedBox(height: 10),
                  _TagChip(meal.category, dense: true),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _MealThumbnail extends StatelessWidget {
  final MealData meal;

  const _MealThumbnail({required this.meal});

  @override
  Widget build(BuildContext context) {
    return Stack(
      clipBehavior: Clip.none,
      children: [
        Container(
          width: 54,
          height: 54,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: meal.gradient,
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(16),
            boxShadow: [
              BoxShadow(
                color: meal.gradient.first.withOpacity(0.35),
                blurRadius: 12,
                offset: const Offset(0, 8),
                spreadRadius: -3,
              ),
            ],
          ),
          child: const Icon(Icons.ramen_dining_rounded, color: Colors.white),
        ),
        if (meal.badge.isNotEmpty)
          Positioned(
            top: -6,
            right: -6,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
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
              child: Text(
                meal.badge,
                style: const TextStyle(
                  color: AppColors.customer,
                  fontWeight: FontWeight.w800,
                  fontSize: 10,
                ),
              ),
            ),
          ),
      ],
    );
  }
}

class _TagChip extends StatelessWidget {
  final String label;
  final bool dense;

  const _TagChip(this.label, {this.dense = false});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.symmetric(
        horizontal: dense ? 8 : 10,
        vertical: dense ? 5 : 6,
      ),
      decoration: BoxDecoration(
        color: AppColors.customer.withOpacity(0.08),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Text(
        label,
        style: const TextStyle(
          fontSize: 11,
          fontWeight: FontWeight.w800,
          color: AppColors.customer,
        ),
      ),
    );
  }
}

class _Dot extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 8),
      width: 5,
      height: 5,
      decoration: BoxDecoration(
        color: Colors.grey[400],
        shape: BoxShape.circle,
      ),
    );
  }
}

class _Stat extends StatelessWidget {
  final IconData icon;
  final String label;

  const _Stat({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 16, color: Colors.grey[700]),
        const SizedBox(width: 4),
        Text(
          label,
          style: TextStyle(
            color: Colors.grey[800],
            fontWeight: FontWeight.w700,
            fontSize: 12.5,
          ),
        ),
      ],
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
