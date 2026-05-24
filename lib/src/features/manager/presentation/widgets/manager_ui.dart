import 'package:flutter/material.dart';

import '../../../../core/analytics/meal_statistics_repository.dart'
    show formatVndCompact;
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../../../core/widgets/module_scroll.dart';

export '../../../../core/widgets/module_page_shell.dart';
export '../../../../core/widgets/module_scroll.dart';
export '../../../../core/widgets/role_tab_shell.dart';

EdgeInsets managerListPadding(BuildContext context) =>
    moduleListPadding(context, bottomBarInset: 0);

/// Manager dùng palette amber — đồng bộ design system.
const RolePalette kManagerRole = RolePalette.manager;

Color get managerAccent => kManagerRole.primary;

LinearGradient get managerGradient => LinearGradient(
      colors: kManagerRole.gradient,
      begin: Alignment.topLeft,
      end: Alignment.bottomRight,
    );

String formatVnd(double amount, {bool compact = false}) {
  if (compact) return '${formatVndCompact(amount)} đ';
  final neg = amount < 0;
  final v = amount.abs();
  final s = v.toStringAsFixed(0).replaceAllMapped(
        RegExp(r'(\d)(?=(\d{3})+(?!\d))'),
        (m) => '${m[1]}.',
      );
  return '${neg ? '-' : ''}$s đ';
}

String formatShortDate(String raw) {
  if (raw.isEmpty) return '—';
  final d = DateTime.tryParse(raw);
  if (d == null) return raw.length > 16 ? raw.substring(0, 16) : raw;
  return '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
}

String apiErrorMessage(Object e) {
  if (e is ApiException) return e.message;
  return e.toString();
}

class ManagerPageShell extends StatelessWidget {
  final String title;
  final Widget body;
  final List<Widget>? actions;
  final Widget? floatingAction;
  final Future<void> Function()? onRefresh;

  const ManagerPageShell({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.floatingAction,
    this.onRefresh,
  });

  @override
  Widget build(BuildContext context) => ModulePageShell(
        title: title,
        body: body,
        role: kManagerRole,
        actions: actions,
        floatingAction: floatingAction,
        onRefresh: onRefresh,
      );
}

class ManagerGlassCard extends ModuleCard {
  const ManagerGlassCard({super.key, required super.child, super.padding});
}

class ManagerStatTile extends ModuleStatTile {
  const ManagerStatTile({
    super.key,
    required super.label,
    required super.value,
    required super.icon,
    required super.color,
  });
}

class ManagerPeriodChips extends StatelessWidget {
  final List<String> labels;
  final int selected;
  final ValueChanged<int> onSelected;

  const ManagerPeriodChips({
    super.key,
    required this.labels,
    required this.selected,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) => ModulePeriodChips(
        labels: labels,
        selected: selected,
        onSelected: onSelected,
        role: kManagerRole,
      );
}

class ManagerLoadingBody extends ModuleLoadingBody {
  const ManagerLoadingBody({super.key}) : super(color: const Color(0xFFD97706));
}

class ManagerErrorBody extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const ManagerErrorBody({super.key, required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) => ModuleErrorBody(
        message: message,
        onRetry: onRetry,
        role: kManagerRole,
      );
}

typedef ManagerEmptyList = ModuleEmptyList;

// ─── Báo cáo / module con — UI chuẩn Manager ─────────────────────────────────

/// Banner giới thiệu đầu màn báo cáo.
class ManagerPageIntro extends StatelessWidget {
  final String title;
  final String description;
  final IconData icon;

  const ManagerPageIntro({
    super.key,
    required this.title,
    required this.description,
    this.icon = Icons.insights_rounded,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            kManagerRole.primary.withValues(alpha: 0.12),
            kManagerRole.primaryAlt.withValues(alpha: 0.06),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: kManagerRole.primary.withValues(alpha: 0.2)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icon, color: kManagerRole.primary, size: 26),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: AppDesignSystem.sectionTitle()),
                const SizedBox(height: 4),
                Text(
                  description,
                  style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Tiêu đề section trong card / danh sách.
class ManagerSectionHeader extends StatelessWidget {
  final String title;
  final String? subtitle;
  final Widget? trailing;

  const ManagerSectionHeader({
    super.key,
    required this.title,
    this.subtitle,
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: AppDesignSystem.sectionTitle()),
              if (subtitle != null) ...[
                const SizedBox(height: 2),
                Text(subtitle!, style: AppDesignSystem.body(size: 12)),
              ],
            ],
          ),
        ),
        if (trailing != null) trailing!,
      ],
    );
  }
}

/// Tab bar bo góc — dùng trong màn có TabBarView.
class ManagerTabBar extends StatelessWidget {
  final TabController controller;
  final List<String> tabs;

  const ManagerTabBar({
    super.key,
    required this.controller,
    required this.tabs,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
      child: DecoratedBox(
        decoration: AppDesignSystem.card(radius: 14),
        child: TabBar(
          controller: controller,
          isScrollable: tabs.length > 3,
          tabAlignment: tabs.length > 3 ? TabAlignment.start : TabAlignment.fill,
          labelColor: kManagerRole.primary,
          unselectedLabelColor: AppDesignSystem.gray500,
          indicatorColor: kManagerRole.primary,
          indicatorWeight: 3,
          labelStyle: AppDesignSystem.label(color: kManagerRole.link),
          unselectedLabelStyle: AppDesignSystem.body(size: 13),
          dividerColor: Colors.transparent,
          tabs: tabs.map((t) => Tab(text: t)).toList(),
        ),
      ),
    );
  }
}

/// Layout tab: header tùy chọn + tab bar + nội dung.
class ManagerTabbedBody extends StatelessWidget {
  final TabController controller;
  final List<String> tabLabels;
  final List<Widget> children;
  final Widget? top;

  const ManagerTabbedBody({
    super.key,
    required this.controller,
    required this.tabLabels,
    required this.children,
    this.top,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (top != null) top!,
        ManagerTabBar(controller: controller, tabs: tabLabels),
        Expanded(
          child: TabBarView(
            controller: controller,
            children: children,
          ),
        ),
      ],
    );
  }
}

class ManagerStatusBadge extends StatelessWidget {
  final String label;
  final Color color;

  const ManagerStatusBadge({super.key, required this.label, required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: color.withValues(alpha: 0.35)),
      ),
      child: Text(
        label,
        style: AppDesignSystem.body(size: 11, color: color).copyWith(fontWeight: FontWeight.w700),
      ),
    );
  }
}

class ManagerSearchField extends StatelessWidget {
  final TextEditingController controller;
  final String hint;
  final VoidCallback? onSubmitted;

  const ManagerSearchField({
    super.key,
    required this.controller,
    required this.hint,
    this.onSubmitted,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
      child: TextField(
        controller: controller,
        onSubmitted: onSubmitted != null ? (_) => onSubmitted!() : null,
        style: AppDesignSystem.body(color: AppDesignSystem.gray900),
        decoration: InputDecoration(
          hintText: hint,
          hintStyle: AppDesignSystem.body(color: AppDesignSystem.gray400),
          prefixIcon: Icon(Icons.search_rounded, color: kManagerRole.primary, size: 22),
          filled: true,
          fillColor: Colors.white,
          contentPadding: const EdgeInsets.symmetric(vertical: 14),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(14),
            borderSide: const BorderSide(color: AppDesignSystem.gray200),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(14),
            borderSide: const BorderSide(color: AppDesignSystem.gray200),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(14),
            borderSide: BorderSide(color: kManagerRole.primary, width: 1.5),
          ),
        ),
      ),
    );
  }
}

/// Dòng dữ liệu trong báo cáo (giao dịch, đối soát, đánh giá…).
class ManagerDataRow extends StatelessWidget {
  final IconData icon;
  final Color iconColor;
  final String title;
  final String? subtitle;
  final String? trailing;
  final Color? trailingColor;
  final Widget? badge;

  const ManagerDataRow({
    super.key,
    required this.icon,
    required this.iconColor,
    required this.title,
    this.subtitle,
    this.trailing,
    this.trailingColor,
    this.badge,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: ManagerGlassCard(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: iconColor.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(icon, color: iconColor, size: 22),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          title,
                          style: AppDesignSystem.label(),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      if (badge != null) ...[const SizedBox(width: 8), badge!],
                    ],
                  ),
                  if (subtitle != null) ...[
                    const SizedBox(height: 4),
                    Text(
                      subtitle!,
                      style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                    ),
                  ],
                ],
              ),
            ),
            if (trailing != null) ...[
              const SizedBox(width: 8),
              Text(
                trailing!,
                style: AppDesignSystem.label(
                  color: trailingColor ?? AppDesignSystem.gray900,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

/// Card điều hướng loại báo cáo (màn Xuất báo cáo).
class ManagerReportNavCard extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final VoidCallback onTap;

  const ManagerReportNavCard({
    super.key,
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
          child: Ink(
            decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg),
            padding: const EdgeInsets.all(16),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: accent.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(14),
                  ),
                  child: Icon(icon, color: accent, size: 26),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(title, style: AppDesignSystem.sectionTitle()),
                      const SizedBox(height: 2),
                      Text(subtitle, style: AppDesignSystem.body(size: 13)),
                    ],
                  ),
                ),
                Icon(Icons.chevron_right_rounded, color: AppDesignSystem.gray400),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class ManagerPrimaryButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final bool loading;
  final VoidCallback? onPressed;

  const ManagerPrimaryButton({
    super.key,
    required this.label,
    required this.icon,
    this.loading = false,
    this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return FilledButton.icon(
      onPressed: loading ? null : onPressed,
      icon: loading
          ? const SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
            )
          : Icon(icon),
      label: Text(label, style: AppDesignSystem.label(color: Colors.white)),
      style: FilledButton.styleFrom(
        backgroundColor: kManagerRole.primary,
        disabledBackgroundColor: kManagerRole.primary.withValues(alpha: 0.5),
        minimumSize: const Size.fromHeight(52),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
    );
  }
}

String _barChartAxisLabel(String key) {
  final k = key.trim();
  if (k.isEmpty) return '—';
  final iso = RegExp(r'^(\d{4})-(\d{2})-(\d{2})');
  final m = iso.firstMatch(k);
  if (m != null) {
    return '${m.group(3)}/${m.group(2)}';
  }
  if (k.length <= 10) return k;
  return '${k.substring(0, 9)}…';
}

/// Biểu đồ cột — cuộn ngang khi nhiều cột, tránh overflow nhãn.
class ManagerBarChart extends StatelessWidget {
  final Map<String, double> data;
  final Color barColor;
  final double height;
  final String emptyMessage;
  final double minBarWidth;

  const ManagerBarChart({
    super.key,
    required this.data,
    this.barColor = const Color(0xFFD97706),
    this.height = 172,
    this.emptyMessage = 'Chưa có dữ liệu trong kỳ',
    this.minBarWidth = 52,
  });

  static const _valueBand = 16.0;
  static const _labelBand = 14.0;
  static const _gap = 4.0;

  Widget _barColumn({
    required String label,
    required double value,
    required double ratio,
    required double width,
    required double chartHeight,
  }) {
    final valueStyle = AppDesignSystem.body(size: 10, color: AppDesignSystem.gray700)
        .copyWith(height: 1.0);
    final labelStyle = AppDesignSystem.body(size: 9, color: AppDesignSystem.gray500)
        .copyWith(height: 1.0);

    return SizedBox(
      width: width,
      height: chartHeight,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 3),
        child: Column(
          children: [
            SizedBox(
              height: _valueBand,
              child: Center(
                child: value > 0
                    ? Text(
                        formatVndCompact(value),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: valueStyle,
                        textAlign: TextAlign.center,
                      )
                    : const SizedBox.shrink(),
              ),
            ),
            const SizedBox(height: _gap),
            Expanded(
              child: Align(
                alignment: Alignment.bottomCenter,
                child: FractionallySizedBox(
                  heightFactor: ratio.clamp(0.06, 1.0),
                  widthFactor: 1,
                  alignment: Alignment.bottomCenter,
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [barColor.withValues(alpha: 0.45), barColor],
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                      ),
                      borderRadius: BorderRadius.circular(8),
                    ),
                  ),
                ),
              ),
            ),
            const SizedBox(height: _gap),
            SizedBox(
              height: _labelBand,
              child: Center(
                child: Text(
                  label,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: labelStyle,
                  textAlign: TextAlign.center,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (data.isEmpty) {
      return SizedBox(
        height: height,
        child: Center(child: ModuleEmptyList(message: emptyMessage, icon: Icons.bar_chart_rounded)),
      );
    }
    final entries = data.entries.toList();
    final maxVal = entries.map((e) => e.value).reduce((a, b) => a > b ? a : b);
    final safeMax = maxVal <= 0 ? 1.0 : maxVal;

    return SizedBox(
      height: height,
      child: LayoutBuilder(
        builder: (context, constraints) {
          final totalWidth = entries.length * minBarWidth;
          final scrollable = totalWidth > constraints.maxWidth - 1;

          final bars = entries.map((e) {
            final ratio = e.value / safeMax;
            final label = _barChartAxisLabel(e.key);
            final w = scrollable ? minBarWidth : constraints.maxWidth / entries.length;
            return _barColumn(
              label: label,
              value: e.value,
              ratio: ratio,
              width: w,
              chartHeight: height,
            );
          }).toList();

          if (scrollable) {
            return SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              physics: const BouncingScrollPhysics(),
              child: SizedBox(
                width: totalWidth,
                height: height,
                child: Row(children: bars),
              ),
            );
          }

          return Row(children: bars);
        },
      ),
    );
  }
}
