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
          ? const OrgLoadingBody(message: 'Đang tải thống kê đơn vị…')
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: orgListPadding(context),
                  children: [
                    const OrgPageIntro(
                      title: 'Thống kê suất ăn đơn vị',
                      description:
                          'Theo dõi số suất và giá trị theo ngày phục vụ, lọc theo kỳ 7 / 30 / 90 ngày.',
                      icon: Icons.analytics_rounded,
                    ),
                    const SizedBox(height: 14),
                    OrgPeriodChips(
                      labels: _ranges,
                      selected: _rangeIndex,
                      onSelected: (i) {
                        setState(() => _rangeIndex = i);
                        _load();
                      },
                    ),
                    const SizedBox(height: 14),
                    Row(
                      children: [
                        Expanded(
                          child: OrgStatTile(
                            label: 'Tổng suất',
                            value: '$_totalMeals',
                            icon: Icons.restaurant_rounded,
                            color: orgAccent,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: OrgStatTile(
                            label: 'Giá trị',
                            value: formatOrgVnd(_totalAmount),
                            icon: Icons.payments_outlined,
                            color: AppDesignSystem.success,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 18),
                    const OrgSectionHeader(
                      title: 'Chi tiết theo ngày / ca',
                      subtitle: 'Dữ liệu lọc theo đơn vị đăng nhập',
                    ),
                    const SizedBox(height: 10),
                    if (_items.isEmpty)
                      const OrgEmptyList(message: 'Chưa có dữ liệu thống kê trong kỳ đã chọn')
                    else
                      ..._items.map(_detailRow),
                  ],
                ),
    );
  }

  Widget _detailRow(MealStatisticItemModel item) {
    final d = item.date;
    final label =
        '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
    final slot = item.mealSlot.isNotEmpty ? item.mealSlot : 'Tất cả ca';

    return OrgDataRow(
      icon: Icons.calendar_today_rounded,
      iconColor: orgAccent,
      title: label,
      subtitle: slot,
      trailing: formatOrgVnd(item.totalAmount),
      badge: OrgStatusBadge(
        label: '${item.totalMeals} suất',
        color: orgAccent,
      ),
    );
  }
}
