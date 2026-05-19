import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../widgets/organization_ui.dart';

/// Báo cáo tóm tắt đơn vị (xuất text — mobile không xuất file Excel/PDF).
class OrgReportsPage extends StatefulWidget {
  const OrgReportsPage({super.key});

  @override
  State<OrgReportsPage> createState() => _OrgReportsPageState();
}

class _OrgReportsPageState extends State<OrgReportsPage> {
  bool _loading = true;
  String? _error;
  String _reportText = '';

  @override
  void initState() {
    super.initState();
    _generate();
  }

  Future<void> _generate() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final profile = await ProfileRepository.instance.getProfile();
      final orgId = profile.unit?.id;
      final orgName = profile.unit?.name ?? profile.displayName;
      final range = mealStatsDateRangeFor(now: DateTime.now(), daysBackInclusive: 29);
      final stats = await MealStatisticsRepository.instance.getMealStatistics(
        startDate: range.$1,
        endDate: range.$2,
        organizationId: orgId,
      );
      final details = await MealStatisticsRepository.instance.getDetailedMealStatistics(
        startDate: range.$1,
        endDate: range.$2,
        organizationId: orgId,
      );

      final totalMeals = stats.fold(0, (s, i) => s + i.totalMeals);
      final totalAmount = stats.fold(0.0, (s, i) => s + i.totalAmount);
      final buf = StringBuffer()
        ..writeln('BÁO CÁO SUẤT ĂN ĐƠN VỊ')
        ..writeln('Đơn vị: $orgName')
        ..writeln('Kỳ: ${range.$1.day}/${range.$1.month}/${range.$1.year} – ${range.$2.day}/${range.$2.month}/${range.$2.year}')
        ..writeln('')
        ..writeln('Tổng suất: $totalMeals')
        ..writeln('Tổng giá trị: ${formatOrgVnd(totalAmount)}')
        ..writeln('')
        ..writeln('--- Chi tiết món ---');
      for (final d in details.take(50)) {
        buf.writeln(
          '${d.date.day}/${d.date.month}: ${d.dishName} (${d.mealSlot}) x${d.quantity} — ${formatOrgVnd(d.totalAmount)}',
        );
      }
      if (details.length > 50) {
        buf.writeln('... và ${details.length - 50} dòng khác');
      }

      if (!mounted) return;
      setState(() {
        _reportText = buf.toString();
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

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Báo cáo',
      onRefresh: _generate,
      body: _loading
          ? const OrgLoadingBody()
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _generate)
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    OrgCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('Báo cáo 30 ngày gần nhất', style: AppDesignSystem.sectionTitle()),
                          const SizedBox(height: 8),
                          Text(
                            'Sao chép nội dung để gửi email hoặc lưu ghi chú. Xuất Excel/PDF thực hiện trên web Manager.',
                            style: AppDesignSystem.body(size: 12),
                          ),
                          const SizedBox(height: 12),
                          SelectableText(
                            _reportText,
                            style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray900),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 12),
                    SizedBox(
                      width: double.infinity,
                      child: FilledButton.icon(
                        onPressed: _reportText.isEmpty
                            ? null
                            : () {
                                Clipboard.setData(ClipboardData(text: _reportText));
                                ScaffoldMessenger.of(context).showSnackBar(
                                  const SnackBar(content: Text('Đã sao chép báo cáo')),
                                );
                              },
                        icon: const Icon(Icons.copy_rounded),
                        label: const Text('Sao chép báo cáo'),
                        style: FilledButton.styleFrom(backgroundColor: orgAccent),
                      ),
                    ),
                  ],
                ),
    );
  }
}
