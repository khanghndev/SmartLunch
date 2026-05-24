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
  String _orgName = '';
  int _totalMeals = 0;
  double _totalAmount = 0;

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
        ..writeln(
          'Kỳ: ${range.$1.day}/${range.$1.month}/${range.$1.year} – '
          '${range.$2.day}/${range.$2.month}/${range.$2.year}',
        )
        ..writeln('')
        ..writeln('Tổng suất: $totalMeals')
        ..writeln('Tổng giá trị: ${formatOrgVnd(totalAmount)}')
        ..writeln('')
        ..writeln('--- Chi tiết món ---');
      for (final d in details.take(50)) {
        buf.writeln(
          '${d.date.day}/${d.date.month}: ${d.dishName} (${d.mealSlot}) '
          'x${d.quantity} — ${formatOrgVnd(d.totalAmount)}',
        );
      }
      if (details.length > 50) {
        buf.writeln('... và ${details.length - 50} dòng khác');
      }

      if (!mounted) return;
      setState(() {
        _orgName = orgName;
        _totalMeals = totalMeals;
        _totalAmount = totalAmount;
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

  void _copyReport() {
    if (_reportText.isEmpty) return;
    Clipboard.setData(ClipboardData(text: _reportText));
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Đã sao chép báo cáo vào bộ nhớ tạm')),
    );
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Báo cáo',
      onRefresh: _generate,
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tạo báo cáo 30 ngày…')
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _generate)
              : ModuleListView(
                  padding: orgListPadding(context),
                  children: [
                    const OrgPageIntro(
                      title: 'Báo cáo suất ăn đơn vị',
                      description:
                          'Tóm tắt 30 ngày gần nhất. Sao chép nội dung để gửi email; xuất Excel/PDF trên web Manager.',
                      icon: Icons.summarize_rounded,
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
                    const SizedBox(height: 16),
                    OrgCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          OrgSectionHeader(
                            title: 'Nội dung báo cáo',
                            subtitle: _orgName.isNotEmpty ? _orgName : null,
                          ),
                          const SizedBox(height: 12),
                          const OrgInfoBanner(
                            message:
                                'Đây là bản tóm tắt văn bản. Để xuất file chính thức, dùng màn Báo cáo trên web quản trị.',
                            icon: Icons.info_outline_rounded,
                          ),
                          const SizedBox(height: 14),
                          Container(
                            width: double.infinity,
                            padding: const EdgeInsets.all(12),
                            decoration: BoxDecoration(
                              color: AppDesignSystem.gray50,
                              borderRadius: BorderRadius.circular(12),
                              border: Border.all(color: AppDesignSystem.gray200),
                            ),
                            child: SelectableText(
                              _reportText,
                              style: AppDesignSystem.body(
                                size: 13,
                                color: AppDesignSystem.gray900,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 14),
                    OrgPrimaryButton(
                      label: 'Sao chép báo cáo',
                      icon: Icons.copy_rounded,
                      onPressed: _reportText.isEmpty ? null : _copyReport,
                    ),
                  ],
                ),
    );
  }
}
