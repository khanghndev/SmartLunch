import 'package:flutter/material.dart';

import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/theme/app_design_system.dart';
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
  int _viewIndex = 0;

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
      final bundle = await fetchMealStatsBundle(startDate: start, endDate: end);
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

  Widget _kpiHeader() {
    final sumMeals = _meals.sumMeals;
    final sumAmount = _meals.sumAmount;
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const ManagerPageIntro(
            title: 'Thống kê suất ăn',
            description:
                'Theo dõi số suất, doanh thu và món bán chạy theo ngày, ca phục vụ hoặc đơn vị.',
            icon: Icons.analytics_rounded,
          ),
          const SizedBox(height: 14),
          ManagerPeriodChips(
            labels: const ['7 ngày', 'Tháng', 'Quý', 'Năm'],
            selected: _range.index,
            onSelected: (i) {
              setState(() => _range = _StatsRange.values[i]);
              _load();
            },
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: ManagerStatTile(
                  label: 'Tổng suất ăn',
                  value: '$sumMeals',
                  icon: Icons.restaurant_rounded,
                  color: AppDesignSystem.info,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: ManagerStatTile(
                  label: 'Doanh thu',
                  value: formatVnd(sumAmount, compact: true),
                  icon: Icons.payments_rounded,
                  color: AppDesignSystem.success,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return ManagerPageShell(
      title: 'Thống kê suất ăn',
      onRefresh: _load,
      body: _loading
          ? const ManagerLoadingBody()
          : _error != null
              ? ManagerErrorBody(message: _error!, onRetry: _load)
              : ManagerTabbedBody(
                  controller: _tabController,
                  tabLabels: const ['Tổng quan', 'Món chi tiết'],
                  top: _kpiHeader(),
                  children: [
                    ListView(
                      padding: managerListPadding(context).copyWith(top: 12),
                      children: [_overviewTab()],
                    ),
                    ListView(
                      padding: managerListPadding(context).copyWith(top: 12),
                      children: [_detailsTab()],
                    ),
                  ],
                ),
    );
  }

  Widget _overviewTab() {
    return Column(
      children: [
        ManagerGlassCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const ManagerSectionHeader(
                title: 'Phân bổ suất ăn',
                subtitle: 'Chọn cách nhóm dữ liệu',
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
              const ManagerSectionHeader(
                title: 'Doanh thu theo ngày',
                subtitle: 'Tổng tiền theo từng ngày trong kỳ',
              ),
              const SizedBox(height: 16),
              ManagerBarChart(
                data: _meals.amountByDay().map(
                  (k, v) => MapEntry('${k.day}/${k.month}', v),
                ),
                barColor: AppDesignSystem.success,
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
      return const ManagerGlassCard(
        child: ManagerEmptyList(
          message: 'Chưa có dữ liệu món chi tiết trong kỳ',
          icon: Icons.ramen_dining_outlined,
        ),
      );
    }
    return ManagerGlassCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const ManagerSectionHeader(
            title: 'Top món bán chạy',
            subtitle: 'Tối đa 10 món theo số suất',
          ),
          const SizedBox(height: 8),
          ...top.asMap().entries.map((entry) {
            final i = entry.key;
            final d = entry.value;
            return ManagerDataRow(
              icon: Icons.ramen_dining_rounded,
              iconColor: managerAccent,
              title: d.name,
              subtitle: '${d.orders} suất đã phục vụ',
              trailing: '#${i + 1}',
              trailingColor: AppDesignSystem.gray500,
            );
          }),
        ],
      ),
    );
  }
}
