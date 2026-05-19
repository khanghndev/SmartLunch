import 'package:flutter/material.dart';

import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/constants/app_colors.dart';
import '../widgets/manager_ui.dart';

enum _StatsRange { week, month, quarter, year }

class ManagerStatisticsPage extends StatefulWidget {
  const ManagerStatisticsPage({super.key});

  @override
  State<ManagerStatisticsPage> createState() => _ManagerStatisticsPageState();
}

class _ManagerStatisticsPageState extends State<ManagerStatisticsPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  _StatsRange _range = _StatsRange.month;
  int _viewIndex = 0; // 0=ngày, 1=ca, 2=bộ phận

  List<MealStatisticItemModel> _meals = [];
  List<DetailedMealItemModel> _details = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _load();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  (DateTime, DateTime) _dateRange() {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    switch (_range) {
      case _StatsRange.week:
        return (today.subtract(const Duration(days: 6)), today);
      case _StatsRange.month:
        return (DateTime(today.year, today.month, 1), today);
      case _StatsRange.quarter:
        final qMonth = ((today.month - 1) ~/ 3) * 3 + 1;
        return (DateTime(today.year, qMonth, 1), today);
      case _StatsRange.year:
        return (DateTime(today.year, 1, 1), today);
    }
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final (start, end) = _dateRange();
      final bundle = await fetchMealStatsBundle(
        startDate: start,
        endDate: end,
      );
      if (!mounted) return;
      setState(() {
        _meals = bundle.meals;
        _details = bundle.details;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = apiErrorMessage(e);
        _loading = false;
      });
    }
  }

  Map<String, double> _chartData() {
    switch (_viewIndex) {
      case 1:
        return _meals.mealsBySlot().map((k, v) => MapEntry(k, v.toDouble()));
      case 2:
        return _meals.mealsByOrganization().map((k, v) => MapEntry(k, v.toDouble()));
      default:
        final byDay = _meals.mealsByDay();
        final sorted = byDay.keys.toList()..sort();
        return {
          for (final d in sorted)
            '${d.day}/${d.month}': (byDay[d] ?? 0).toDouble(),
        };
    }
  }

  @override
  Widget build(BuildContext context) {
    final sumMeals = _meals.sumMeals;
    final sumAmount = _meals.sumAmount;

    return ManagerPageShell(
      title: 'Thống kê suất ăn',
      onRefresh: _load,
      body: _loading
          ? const ManagerLoadingBody()
          : _error != null
              ? ManagerErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  physics: const AlwaysScrollableScrollPhysics(),
                  padding: const EdgeInsets.fromLTRB(16, 16, 16, 32),
                  children: [
                    ManagerPeriodChips(
                      labels: const ['7 ngày', 'Tháng', 'Quý', 'Năm'],
                      selected: _range.index,
                      onSelected: (i) {
                        setState(() => _range = _StatsRange.values[i]);
                        _load();
                      },
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: ManagerStatTile(
                            label: 'Tổng suất ăn',
                            value: '$sumMeals',
                            icon: Icons.restaurant_rounded,
                            color: AppColors.info,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: ManagerStatTile(
                            label: 'Doanh thu',
                            value: formatVnd(sumAmount, compact: true),
                            icon: Icons.payments_rounded,
                            color: AppColors.success,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    TabBar(
                      controller: _tabController,
                      labelColor: managerAccent,
                      unselectedLabelColor: Colors.grey.shade600,
                      indicatorColor: managerAccent,
                      tabs: const [
                        Tab(text: 'Tổng quan'),
                        Tab(text: 'Món chi tiết'),
                      ],
                    ),
                    const SizedBox(height: 12),
                    SizedBox(
                      height: 520,
                      child: TabBarView(
                        controller: _tabController,
                        children: [
                          SingleChildScrollView(child: _overviewTab()),
                          SingleChildScrollView(child: _detailsTab()),
                        ],
                      ),
                    ),
                  ],
                ),
    );
  }

  Widget _overviewTab() {
    return Column(
      key: const ValueKey('overview'),
      children: [
        ManagerGlassCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Phân bổ suất ăn',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w800,
                    ),
              ),
              const SizedBox(height: 12),
              ManagerPeriodChips(
                labels: const ['Theo ngày', 'Theo ca', 'Bộ phận'],
                selected: _viewIndex,
                onSelected: (i) => setState(() => _viewIndex = i),
              ),
              const SizedBox(height: 16),
              ManagerBarChart(data: _chartData()),
            ],
          ),
        ),
        const SizedBox(height: 12),
        ManagerGlassCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Doanh thu theo ngày',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w800,
                    ),
              ),
              const SizedBox(height: 12),
              ManagerBarChart(
                data: _meals.amountByDay().map((k, v) {
                  return MapEntry('${k.day}/${k.month}', v);
                }),
                barColor: AppColors.success,
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _detailsTab() {
    final top = _details.topDishes(limit: 10);
    if (top.isEmpty) {
      return const ManagerGlassCard(child: ManagerEmptyList(message: 'Chưa có món chi tiết'));
    }
    return ManagerGlassCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Top món bán chạy',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w800,
                ),
          ),
          const SizedBox(height: 8),
          ...top.map((d) {
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Row(
                children: [
                  CircleAvatar(
                    radius: 18,
                    backgroundColor: managerAccent.withValues(alpha: 0.15),
                    child: Icon(Icons.ramen_dining_rounded, color: managerAccent, size: 18),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(d.name, style: const TextStyle(fontWeight: FontWeight.w600)),
                        Text(
                          '${d.orders} suất',
                          style: TextStyle(color: Colors.grey.shade600, fontSize: 12),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            );
          }),
        ],
      ),
    );
  }
}
