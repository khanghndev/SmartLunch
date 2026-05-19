import 'package:flutter/material.dart';

import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../widgets/organization_ui.dart';

/// Thống kê suất ăn đơn vị — `GET .../Order/statistics/meal-count`.
class OrgStatisticsPage extends StatefulWidget {
  const OrgStatisticsPage({super.key});

  @override
  State<OrgStatisticsPage> createState() => _OrgStatisticsPageState();
}

class _OrgStatisticsPageState extends State<OrgStatisticsPage> {
  static const _ranges = ['7 ngày', '30 ngày', '90 ngày'];
  static const _daysBack = [6, 29, 89];

  int _rangeIndex = 0;
  List<MealStatisticItemModel> _items = [];
  bool _loading = true;
  String? _error;
  int? _orgId;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final profile = await ProfileRepository.instance.getProfile();
      _orgId = profile.unit?.id;
      final now = DateTime.now();
      final range = mealStatsDateRangeFor(
        now: now,
        daysBackInclusive: _daysBack[_rangeIndex],
      );
      final items = await MealStatisticsRepository.instance.getMealStatistics(
        startDate: range.$1,
        endDate: range.$2,
        organizationId: _orgId,
      );
      if (!mounted) return;
      setState(() {
        _items = items;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  int get _totalMeals => _items.fold(0, (s, i) => s + i.totalMeals);

  double get _totalAmount => _items.fold(0.0, (s, i) => s + i.totalAmount);

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Thống kê suất ăn',
      onRefresh: _load,
      body: _loading
          ? const OrgLoadingBody()
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    ModulePeriodChips(
                      labels: _ranges,
                      selected: _rangeIndex,
                      onSelected: (i) {
                        setState(() => _rangeIndex = i);
                        _load();
                      },
                      role: kOrgRole,
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: ModuleStatTile(
                            label: 'Tổng suất',
                            value: '$_totalMeals',
                            icon: Icons.restaurant_rounded,
                            color: orgAccent,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: ModuleStatTile(
                            label: 'Giá trị',
                            value: formatOrgVnd(_totalAmount),
                            icon: Icons.payments_outlined,
                            color: AppDesignSystem.success,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    Text('Chi tiết theo ngày / ca', style: AppDesignSystem.sectionTitle()),
                    const SizedBox(height: 8),
                    if (_items.isEmpty)
                      const ModuleEmptyList(message: 'Chưa có dữ liệu thống kê')
                    else
                      ..._items.map((item) {
                        final d = item.date;
                        final label =
                            '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
                        return Padding(
                          padding: const EdgeInsets.only(bottom: 8),
                          child: OrgCard(
                            padding: const EdgeInsets.all(14),
                            child: Row(
                              children: [
                                Expanded(
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: [
                                      Text(label, style: AppDesignSystem.label()),
                                      Text(
                                        item.mealSlot.isNotEmpty
                                            ? item.mealSlot
                                            : 'Tất cả ca',
                                        style: AppDesignSystem.body(size: 12),
                                      ),
                                    ],
                                  ),
                                ),
                                Column(
                                  crossAxisAlignment: CrossAxisAlignment.end,
                                  children: [
                                    Text(
                                      '${item.totalMeals} suất',
                                      style: AppDesignSystem.sectionTitle(color: orgAccent),
                                    ),
                                    Text(
                                      formatOrgVnd(item.totalAmount),
                                      style: AppDesignSystem.body(size: 12),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ),
                        );
                      }),
                  ],
                ),
    );
  }
}
