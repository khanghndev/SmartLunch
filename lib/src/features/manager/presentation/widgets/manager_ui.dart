import 'package:flutter/material.dart';

import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';

export '../../../../core/widgets/module_page_shell.dart';

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

/// Biểu đồ cột đơn giản.
class ManagerBarChart extends StatelessWidget {
  final Map<String, double> data;
  final Color barColor;
  final double height;

  const ManagerBarChart({
    super.key,
    required this.data,
    this.barColor = const Color(0xFFD97706),
    this.height = 140,
  });

  @override
  Widget build(BuildContext context) {
    if (data.isEmpty) {
      return SizedBox(
        height: height,
        child: Center(child: ModuleEmptyList(message: 'Chưa có dữ liệu')),
      );
    }
    final entries = data.entries.toList();
    final maxVal = entries.map((e) => e.value).reduce((a, b) => a > b ? a : b);
    final safeMax = maxVal <= 0 ? 1.0 : maxVal;

    return SizedBox(
      height: height,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: entries.map((e) {
          final ratio = e.value / safeMax;
          return Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 3),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  if (e.value > 0)
                    Text(
                      formatVndCompact(e.value),
                      style: AppDesignSystem.body(size: 9),
                    ),
                  const SizedBox(height: 4),
                  AnimatedContainer(
                    duration: const Duration(milliseconds: 400),
                    height: 80 * ratio.clamp(0.05, 1.0),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [barColor.withValues(alpha: 0.5), barColor],
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                      ),
                      borderRadius: BorderRadius.circular(AppDesignSystem.radiusSm),
                    ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    e.key.length > 6 ? e.key.substring(0, 6) : e.key,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: AppDesignSystem.body(size: 9),
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            ),
          );
        }).toList(),
      ),
    );
  }
}
