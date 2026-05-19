import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';

/// AppBar + scaffold chuẩn cho mọi module (style Auth).
class ModulePageShell extends StatelessWidget {
  final String title;
  final Widget body;
  final RolePalette role;
  final List<Widget>? actions;
  final Widget? floatingAction;
  final Future<void> Function()? onRefresh;

  const ModulePageShell({
    super.key,
    required this.title,
    required this.body,
    required this.role,
    this.actions,
    this.floatingAction,
    this.onRefresh,
  });

  @override
  Widget build(BuildContext context) {
    final content = onRefresh != null
        ? RefreshIndicator(color: role.primary, onRefresh: onRefresh!, child: body)
        : body;

    return Scaffold(
      backgroundColor: AppDesignSystem.gray50,
      appBar: AppBar(
        elevation: 0,
        flexibleSpace: Container(
          decoration: BoxDecoration(gradient: AppDesignSystem.headerGradient(role)),
        ),
        foregroundColor: Colors.white,
        title: Text(
          title,
          style: AppDesignSystem.font.copyWith(
            fontWeight: FontWeight.w800,
            fontSize: 18,
          ),
        ),
        actions: actions,
      ),
      floatingActionButton: floatingAction,
      body: content,
    );
  }
}

/// Thẻ nội dung — khớp form card Auth.
class ModuleCard extends StatelessWidget {
  final Widget child;
  final EdgeInsetsGeometry padding;

  const ModuleCard({
    super.key,
    required this.child,
    this.padding = const EdgeInsets.all(20),
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: padding,
      decoration: AppDesignSystem.card(),
      child: child,
    );
  }
}

class ModuleStatTile extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final Color color;

  const ModuleStatTile({
    super.key,
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return ModuleCard(
      padding: const EdgeInsets.all(14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
            ),
            child: Icon(icon, color: color, size: 22),
          ),
          const SizedBox(height: 10),
          Text(value, style: AppDesignSystem.title(size: 22, color: AppDesignSystem.gray900)),
          const SizedBox(height: 2),
          Text(label, style: AppDesignSystem.body(size: 12)),
        ],
      ),
    );
  }
}

class ModulePeriodChips extends StatelessWidget {
  final List<String> labels;
  final int selected;
  final ValueChanged<int> onSelected;
  final RolePalette role;

  const ModulePeriodChips({
    super.key,
    required this.labels,
    required this.selected,
    required this.onSelected,
    required this.role,
  });

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children: List.generate(labels.length, (i) {
          final active = i == selected;
          return Padding(
            padding: const EdgeInsets.only(right: 8),
            child: ChoiceChip(
              label: Text(labels[i]),
              selected: active,
              onSelected: (_) => onSelected(i),
              selectedColor: role.primary.withValues(alpha: 0.2),
              labelStyle: AppDesignSystem.body(
                color: active ? role.link : AppDesignSystem.gray500,
              ).copyWith(fontWeight: active ? FontWeight.w700 : FontWeight.w500),
              side: BorderSide(
                color: active ? role.primary : AppDesignSystem.gray200,
              ),
              backgroundColor: Colors.white,
            ),
          );
        }),
      ),
    );
  }
}

class ModuleLoadingBody extends StatelessWidget {
  final Color? color;

  const ModuleLoadingBody({super.key, this.color});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(48),
        child: CircularProgressIndicator(color: color ?? RolePalette.brand.primary),
      ),
    );
  }
}

class ModuleErrorBody extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;
  final RolePalette role;

  const ModuleErrorBody({
    super.key,
    required this.message,
    required this.onRetry,
    required this.role,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: ModuleCard(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.cloud_off_rounded, size: 48, color: AppDesignSystem.gray400),
              const SizedBox(height: 12),
              Text(message, textAlign: TextAlign.center, style: AppDesignSystem.body()),
              const SizedBox(height: 16),
              FilledButton.icon(
                onPressed: onRetry,
                icon: const Icon(Icons.refresh_rounded),
                label: const Text('Thử lại'),
                style: FilledButton.styleFrom(
                  backgroundColor: role.primary,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class ModuleEmptyList extends StatelessWidget {
  final String message;
  final IconData icon;

  const ModuleEmptyList({
    super.key,
    this.message = 'Không có dữ liệu',
    this.icon = Icons.inbox_rounded,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 32),
      child: Column(
        children: [
          Icon(icon, size: 56, color: AppDesignSystem.gray200),
          const SizedBox(height: 12),
          Text(message, style: AppDesignSystem.body()),
        ],
      ),
    );
  }
}
