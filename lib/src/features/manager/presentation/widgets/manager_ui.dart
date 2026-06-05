import 'package:flutter/material.dart';

import '../../../../core/analytics/meal_statistics_repository.dart'
    show formatVndCompact;
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../../../core/widgets/role_tab_shell.dart' show moduleTabListPadding;

export '../../../../core/widgets/module_page_shell.dart';
export '../../../../core/widgets/module_scroll.dart';
export '../../../../core/widgets/role_tab_shell.dart';

EdgeInsets managerListPadding(BuildContext context) =>
    moduleTabListPadding(context);

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
  final bool embeddedInModuleShell;

  const ManagerPageShell({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.floatingAction,
    this.onRefresh,
    this.embeddedInModuleShell = false,
  });

  @override
  Widget build(BuildContext context) => ModulePageShell(
        title: title,
        body: body,
        role: kManagerRole,
        actions: actions,
        floatingAction: floatingAction,
        onRefresh: onRefresh,
        embeddedInModuleShell: embeddedInModuleShell,
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

/// Banner dashboard — tổng quan vận hành tháng.
class ManagerWelcomeBanner extends StatelessWidget {
  final String managerName;
  final int month;

  const ManagerWelcomeBanner({
    super.key,
    required this.managerName,
    required this.month,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(20, 14, 20, 0),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: kManagerRole.gradient,
        ),
        boxShadow: [
          BoxShadow(
            color: kManagerRole.primary.withValues(alpha: 0.28),
            blurRadius: 18,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Stack(
          children: [
            Positioned(
              right: -16,
              bottom: -16,
              child: Icon(
                Icons.insights_rounded,
                size: 110,
                color: Colors.white.withValues(alpha: 0.1),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(18, 18, 18, 18),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.white.withValues(alpha: 0.2),
                      borderRadius: BorderRadius.circular(999),
                      border: Border.all(color: Colors.white24),
                    ),
                    child: Text(
                      'Tháng $month · Quản trị HUITMeal',
                      style: AppDesignSystem.body(size: 11, color: Colors.white)
                          .copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  const SizedBox(height: 10),
                  Text(
                    'Xin chào, $managerName',
                    style: AppDesignSystem.title(size: 22, color: Colors.white),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    'Theo dõi suất ăn, dòng tiền, đối soát và phản hồi trên một màn hình.',
                    style: AppDesignSystem.body(size: 13, color: Colors.white.withValues(alpha: 0.92)),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// KPI nổi bật — lợi nhuận / chỉ số chính.
class ManagerHeroKpiCard extends StatelessWidget {
  final String label;
  final String value;
  final String? hint;
  final IconData icon;
  final Color accent;
  final bool outerMargin;

  const ManagerHeroKpiCard({
    super.key,
    required this.label,
    required this.value,
    this.hint,
    required this.icon,
    required this.accent,
    this.outerMargin = true,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: outerMargin ? const EdgeInsets.fromLTRB(20, 12, 20, 0) : EdgeInsets.zero,
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: accent.withValues(alpha: 0.25)),
        boxShadow: [
          BoxShadow(
            color: accent.withValues(alpha: 0.12),
            blurRadius: 16,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [accent.withValues(alpha: 0.15), accent.withValues(alpha: 0.05)],
              ),
              borderRadius: BorderRadius.circular(14),
            ),
            child: Icon(icon, color: accent, size: 30),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500)),
                const SizedBox(height: 4),
                Text(value, style: AppDesignSystem.title(size: 24, color: AppDesignSystem.gray900)),
                if (hint != null) ...[
                  const SizedBox(height: 4),
                  Text(hint!, style: AppDesignSystem.body(size: 12, color: accent)),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Thu — Chi — Lợi nhuận (màn thu chi).
class ManagerFinanceStrip extends StatelessWidget {
  final String income;
  final String expense;
  final String profit;

  const ManagerFinanceStrip({
    super.key,
    required this.income,
    required this.expense,
    required this.profit,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 4),
      child: Row(
        children: [
          Expanded(
            child: _OpsMini(
              icon: Icons.south_west_rounded,
              title: 'Tổng thu',
              value: income,
              color: AppDesignSystem.success,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _OpsMini(
              icon: Icons.north_east_rounded,
              title: 'Tổng chi',
              value: expense,
              color: AppDesignSystem.danger,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _OpsMini(
              icon: Icons.trending_up_rounded,
              title: 'Lợi nhuận',
              value: profit,
              color: kManagerRole.primary,
            ),
          ),
        ],
      ),
    );
  }
}

/// Ba chỉ số nhanh dưới banner.
class ManagerOpsStrip extends StatelessWidget {
  final String mealsLabel;
  final String complaintsLabel;
  final String reconciliationLabel;

  const ManagerOpsStrip({
    super.key,
    required this.mealsLabel,
    required this.complaintsLabel,
    required this.reconciliationLabel,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 12, 20, 0),
      child: Row(
        children: [
          Expanded(
            child: _OpsMini(
              icon: Icons.restaurant_rounded,
              title: 'Suất ăn',
              value: mealsLabel,
              color: AppDesignSystem.info,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _OpsMini(
              icon: Icons.feedback_rounded,
              title: 'Khiếu nại',
              value: complaintsLabel,
              color: AppDesignSystem.warning,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _OpsMini(
              icon: Icons.receipt_long_rounded,
              title: 'Đối soát',
              value: reconciliationLabel,
              color: kManagerRole.primary,
            ),
          ),
        ],
      ),
    );
  }
}

class _OpsMini extends StatelessWidget {
  final IconData icon;
  final String title;
  final String value;
  final Color color;

  const _OpsMini({
    required this.icon,
    required this.title,
    required this.value,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 10),
      decoration: AppDesignSystem.card(radius: 14),
      child: Column(
        children: [
          Icon(icon, size: 20, color: color),
          const SizedBox(height: 6),
          Text(
            value,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: AppDesignSystem.label().copyWith(fontSize: 12),
          ),
          Text(
            title,
            style: AppDesignSystem.body(size: 9, color: AppDesignSystem.gray500),
          ),
        ],
      ),
    );
  }
}

/// Card bọc biểu đồ — tiêu đề + nội dung sinh động.
class ManagerChartCard extends StatelessWidget {
  final String title;
  final String? subtitle;
  final Widget child;
  final Color accent;
  final Widget? trailing;

  const ManagerChartCard({
    super.key,
    required this.title,
    this.subtitle,
    required this.child,
    this.accent = const Color(0xFFD97706),
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return ManagerGlassCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Container(
            padding: const EdgeInsets.fromLTRB(16, 14, 16, 12),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [accent.withValues(alpha: 0.12), Colors.transparent],
              ),
              borderRadius: const BorderRadius.vertical(top: Radius.circular(16)),
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: accent.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(Icons.bar_chart_rounded, color: accent, size: 20),
                ),
                const SizedBox(width: 12),
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
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 0, 12, 16),
            child: child,
          ),
        ],
      ),
    );
  }
}

class ManagerSkeletonBox extends StatelessWidget {
  final double height;
  final double? width;
  final double radius;

  const ManagerSkeletonBox({
    super.key,
    required this.height,
    this.width,
    this.radius = 12,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: AppDesignSystem.gray100,
        borderRadius: BorderRadius.circular(radius),
      ),
    );
  }
}

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
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: AppDesignSystem.gray100),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: kManagerRole.primary.withValues(alpha: 0.1),
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
            decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg).copyWith(
              border: Border.all(color: accent.withValues(alpha: 0.15)),
            ),
            child: IntrinsicHeight(
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Container(
                    width: 5,
                    decoration: BoxDecoration(
                      color: accent,
                      borderRadius: const BorderRadius.horizontal(left: Radius.circular(16)),
                    ),
                  ),
                  Expanded(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.all(12),
                            decoration: BoxDecoration(
                              gradient: LinearGradient(
                                colors: [
                                  accent.withValues(alpha: 0.18),
                                  accent.withValues(alpha: 0.06),
                                ],
                              ),
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
                          Icon(Icons.arrow_forward_rounded, color: accent, size: 22),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
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
