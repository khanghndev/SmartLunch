import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class RatingPage extends StatelessWidget {
  const RatingPage({super.key});

  @override
  Widget build(BuildContext context) {
    final meals = [
      const _MealRating(
        name: 'Cơm gà sốt chanh',
        date: 'Đã giao 11:40',
        tags: ['Ngon', 'Đúng nhiệt độ', 'Đóng gói đẹp'],
        image: 'assets/images/meals/meal1.png',
      ),
      const _MealRating(
        name: 'Bún bò Huế',
        date: 'Hôm qua · 12:05',
        tags: ['Vừa miệng', 'Đầy đặn'],
        image: 'assets/images/meals/meal2.png',
      ),
      const _MealRating(
        name: 'Mì Ý bò bằm',
        date: 'Thứ 6 · 11:50',
        tags: ['Sốt ngon', 'Bánh mì giòn'],
        image: 'assets/images/meals/meal3.png',
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Đánh giá món ăn'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.history_rounded)),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            const _RatingSummary(),
            const SizedBox(height: 14),
            ...meals.map(
              (meal) => Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: _MealCard(meal: meal),
              ),
            ),
            TextButton.icon(
              onPressed: () {},
              icon: const Icon(Icons.star_border_rounded),
              label: const Text('Xem đánh giá đã gửi'),
            ),
          ],
        ),
      ),
    );
  }
}

class _RatingSummary extends StatelessWidget {
  const _RatingSummary();

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
            color: AppColors.customer.withOpacity(0.22),
            blurRadius: 14,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            width: 68,
            height: 68,
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.15),
              shape: BoxShape.circle,
              border: Border.all(color: Colors.white.withOpacity(0.3)),
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: const [
                Text(
                  '4.9',
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                    fontSize: 24,
                  ),
                ),
                SizedBox(height: 2),
                Icon(Icons.star_rounded, color: Colors.white, size: 18),
              ],
            ),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Mức độ hài lòng',
                  style: TextStyle(
                    color: Colors.white.withOpacity(0.9),
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(height: 6),
                Wrap(
                  spacing: 6,
                  runSpacing: 6,
                  children: const [
                    _Chip(text: 'Giao nhanh'),
                    _Chip(text: 'Đồ ăn ngon'),
                    _Chip(text: 'Đóng gói sạch'),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _MealRating {
  final String name;
  final String date;
  final List<String> tags;
  final String image;

  const _MealRating({
    required this.name,
    required this.date,
    required this.tags,
    required this.image,
  });
}

class _MealCard extends StatelessWidget {
  final _MealRating meal;

  const _MealCard({required this.meal});

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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              ClipRRect(
                borderRadius: BorderRadius.circular(12),
                child: Container(
                  width: 68,
                  height: 68,
                  color: AppColors.customer.withOpacity(0.08),
                  child: Image.asset(
                    meal.image,
                    fit: BoxFit.cover,
                    errorBuilder:
                        (_, __, ___) => const Icon(
                          Icons.fastfood_rounded,
                          color: AppColors.customer,
                        ),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      meal.name,
                      style: const TextStyle(
                        color: AppColors.ink,
                        fontWeight: FontWeight.w900,
                        fontSize: 16,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      meal.date,
                      style: TextStyle(
                        color: AppColors.ink.withOpacity(0.6),
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
              IconButton(
                onPressed: () {},
                icon: const Icon(Icons.more_horiz_rounded),
              ),
            ],
          ),
          const SizedBox(height: 10),
          _StarRow(onChange: (_) {}),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: meal.tags.map((t) => _Tag(text: t)).toList(),
          ),
          const SizedBox(height: 10),
          TextFormField(
            maxLines: 2,
            decoration: InputDecoration(
              hintText: 'Viết thêm cảm nhận về món ăn...',
              filled: true,
              fillColor: const Color(0xFFF8F9FB),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: AppColors.ink.withOpacity(0.08)),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: const BorderSide(
                  color: AppColors.customer,
                  width: 1.4,
                ),
              ),
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 12,
                vertical: 12,
              ),
            ),
          ),
          const SizedBox(height: 10),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.customer,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(vertical: 12),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                elevation: 0,
              ),
              onPressed: () {},
              child: const Text(
                'Gửi đánh giá',
                style: TextStyle(fontWeight: FontWeight.w800),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _Tag extends StatelessWidget {
  final String text;

  const _Tag({required this.text});

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
        text,
        style: TextStyle(
          color: AppColors.ink.withOpacity(0.8),
          fontWeight: FontWeight.w700,
        ),
      ),
    );
  }
}

class _StarRow extends StatefulWidget {
  final ValueChanged<int> onChange;

  const _StarRow({required this.onChange});

  @override
  State<_StarRow> createState() => _StarRowState();
}

class _StarRowState extends State<_StarRow> {
  int value = 5;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: List.generate(5, (index) {
        final filled = index < value;
        return IconButton(
          padding: EdgeInsets.zero,
          constraints: const BoxConstraints(),
          icon: Icon(
            filled ? Icons.star_rounded : Icons.star_border_rounded,
            color:
                filled
                    ? const Color(0xFFE8B73B)
                    : AppColors.ink.withOpacity(0.3),
            size: 28,
          ),
          onPressed: () {
            setState(() => value = index + 1);
            widget.onChange(value);
          },
        );
      }),
    );
  }
}

class _Chip extends StatelessWidget {
  final String text;

  const _Chip({required this.text});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.18),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: Colors.white.withOpacity(0.25)),
      ),
      child: Text(
        text,
        style: const TextStyle(
          color: Colors.white,
          fontWeight: FontWeight.w700,
          fontSize: 12,
        ),
      ),
    );
  }
}
