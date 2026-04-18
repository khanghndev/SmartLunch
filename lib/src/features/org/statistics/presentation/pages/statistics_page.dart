import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class StatisticsPage extends StatelessWidget {
  const StatisticsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final bars = [
      _BarData(label: 'T2', value: 62),
      _BarData(label: 'T3', value: 74),
      _BarData(label: 'T4', value: 58),
      _BarData(label: 'T5', value: 92),
      _BarData(label: 'T6', value: 80),
    ];

    final dept = const [
      _DeptData(name: 'Kế toán', total: 42, diff: 4),
      _DeptData(name: 'CSKH', total: 38, diff: -6),
      _DeptData(name: 'IT', total: 55, diff: 8),
      _DeptData(name: 'Vận hành', total: 30, diff: -2),
    ];

    final bestMeals = const [
      _MealData(name: 'Cơm gà xé', orders: 120, rating: 4.8),
      _MealData(name: 'Bún bò Huế', orders: 108, rating: 4.7),
      _MealData(name: 'Cơm sườn', orders: 96, rating: 4.5),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Thống kê'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            onPressed: () {},
            icon: const Icon(Icons.download_rounded),
          ),
          IconButton(onPressed: () {}, icon: const Icon(Icons.more_vert)),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            const _SummaryCards(),
            const SizedBox(height: 14),
            _BarChartCard(bars: bars),
            const SizedBox(height: 14),
            _DeptTable(data: dept),
            const SizedBox(height: 14),
            _BestMeals(meals: bestMeals),
          ],
        ),
      ),
    );
  }
}

class _SummaryCards extends StatelessWidget {
  const _SummaryCards();

  @override
  Widget build(BuildContext context) {
    final cards = [
      _SummaryCard(
        label: 'Suất ăn tuần này',
        value: '366',
        icon: Icons.receipt_long_rounded,
        color: AppColors.org,
      ),
      _SummaryCard(
        label: 'Tỉ lệ đúng giờ',
        value: '97%',
        icon: Icons.access_time_filled_rounded,
        color: AppColors.org,
      ),
      _SummaryCard(
        label: 'Đánh giá trung bình',
        value: '4.7',
        icon: Icons.star_rounded,
        color: AppColors.orgAlt,
      ),
    ];

    return LayoutBuilder(
      builder: (context, constraints) {
        if (constraints.maxWidth < 380) {
          return Column(
            children:
                cards
                    .map(
                      (c) => Padding(
                        padding: const EdgeInsets.only(bottom: 8),
                        child: c,
                      ),
                    )
                    .toList(),
          );
        }
        return Row(
          children: [
            for (var i = 0; i < cards.length; i++) ...[
              Expanded(child: cards[i]),
              if (i != cards.length - 1) const SizedBox(width: 10),
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
          const SizedBox(width: 12),
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
                  style: TextStyle(
                    color: color,
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

class _BarData {
  final String label;
  final double value;

  const _BarData({required this.label, required this.value});
}

class _BarChartCard extends StatelessWidget {
  final List<_BarData> bars;

  const _BarChartCard({required this.bars});

  @override
  Widget build(BuildContext context) {
    final maxValue = bars
        .map((b) => b.value)
        .fold<double>(0, (p, c) => c > p ? c : p);
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
        children: [
          Row(
            children: [
              const Icon(Icons.bar_chart_rounded, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Suất ăn theo ngày',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Theo ca')),
            ],
          ),
          const SizedBox(height: 12),
          SizedBox(
            height: 180,
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.end,
              children:
                  bars
                      .map(
                        (bar) => Expanded(
                          child: Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 6),
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.end,
                              children: [
                                AnimatedContainer(
                                  duration: const Duration(milliseconds: 400),
                                  height: (bar.value / maxValue) * 120 + 20,
                                  decoration: BoxDecoration(
                                    gradient: const LinearGradient(
                                      colors: [AppColors.org, AppColors.orgAlt],
                                      begin: Alignment.bottomCenter,
                                      end: Alignment.topCenter,
                                    ),
                                    borderRadius: BorderRadius.circular(10),
                                    boxShadow: [
                                      BoxShadow(
                                        color: AppColors.org.withOpacity(0.2),
                                        blurRadius: 10,
                                        offset: const Offset(0, 6),
                                      ),
                                    ],
                                  ),
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  '${bar.value.toInt()}',
                                  style: TextStyle(
                                    color: AppColors.ink.withOpacity(0.75),
                                    fontWeight: FontWeight.w800,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  bar.label,
                                  style: TextStyle(
                                    color: AppColors.ink.withOpacity(0.6),
                                    fontWeight: FontWeight.w700,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      )
                      .toList(),
            ),
          ),
        ],
      ),
    );
  }
}

class _DeptData {
  final String name;
  final int total;
  final int diff;

  const _DeptData({
    required this.name,
    required this.total,
    required this.diff,
  });
}

class _DeptTable extends StatelessWidget {
  final List<_DeptData> data;

  const _DeptTable({required this.data});

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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.account_tree_rounded, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Theo phòng ban',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Xuất CSV')),
            ],
          ),
          const SizedBox(height: 10),
          ...data.map(
            (row) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 6),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(10),
                    decoration: BoxDecoration(
                      color: AppColors.org.withOpacity(0.08),
                      shape: BoxShape.circle,
                    ),
                    child: const Icon(Icons.business, color: AppColors.org),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Text(
                      row.name,
                      style: const TextStyle(
                        color: AppColors.ink,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                  ),
                  Text(
                    '${row.total} suất',
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.75),
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const SizedBox(width: 10),
                  _DiffBadge(diff: row.diff),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _DiffBadge extends StatelessWidget {
  final int diff;

  const _DiffBadge({required this.diff});

  @override
  Widget build(BuildContext context) {
    final color = diff >= 0 ? AppColors.org : Colors.redAccent;
    final sign = diff >= 0 ? '+' : '';
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.12),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        '$sign$diff',
        style: TextStyle(
          color: color,
          fontWeight: FontWeight.w800,
          fontSize: 12.5,
        ),
      ),
    );
  }
}

class _MealData {
  final String name;
  final int orders;
  final double rating;

  const _MealData({
    required this.name,
    required this.orders,
    required this.rating,
  });
}

class _BestMeals extends StatelessWidget {
  final List<_MealData> meals;

  const _BestMeals({required this.meals});

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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.thumb_up_alt_outlined, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Món được yêu thích',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Xem thêm')),
            ],
          ),
          const SizedBox(height: 10),
          ...meals.map(
            (meal) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Row(
                children: [
                  Container(
                    width: 50,
                    height: 50,
                    decoration: BoxDecoration(
                      color: AppColors.org.withOpacity(0.08),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Icon(
                      Icons.fastfood_rounded,
                      color: AppColors.org,
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
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          '${meal.orders} suất',
                          style: TextStyle(
                            color: AppColors.ink.withOpacity(0.65),
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                  Row(
                    children: [
                      const Icon(
                        Icons.star_rounded,
                        size: 18,
                        color: Color(0xFFE8B73B),
                      ),
                      const SizedBox(width: 4),
                      Text(
                        meal.rating.toStringAsFixed(1),
                        style: TextStyle(
                          color: AppColors.ink.withOpacity(0.8),
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
