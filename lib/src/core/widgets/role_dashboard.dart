import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';
import 'role_tab_shell.dart';

class DashboardTopAction {
  final IconData icon;
  final String tooltip;
  final String? route;
  final VoidCallback? onTap;

  const DashboardTopAction({
    required this.icon,
    required this.tooltip,
    this.route,
    this.onTap,
  });
}

class DashboardLeadingAction {
  final IconData icon;
  final String tooltip;
  final VoidCallback onTap;

  const DashboardLeadingAction({
    required this.icon,
    required this.tooltip,
    required this.onTap,
  });
}

class DashboardQuickAction {
  final String label;
  final IconData icon;
  final String route;

  const DashboardQuickAction({
    required this.label,
    required this.icon,
    required this.route,
  });
}

class DashboardFeature {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color color;
  final String route;
  /// Gợi ý số liệu từ BE (vd. "3 hợp đồng đang hiệu lực").
  final String? preview;

  const DashboardFeature({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.color,
    required this.route,
    this.preview,
  });
}

/// Mục trong danh sách "Hoạt động gần đây".
class DashboardRecentItem {
  final String title;
  final String subtitle;
  final String? trailing;
  final IconData icon;
  final Color? iconColor;
  final String? route;
  final Object? routeArguments;

  const DashboardRecentItem({
    required this.title,
    required this.subtitle,
    this.trailing,
    required this.icon,
    this.iconColor,
    this.route,
    this.routeArguments,
  });
}

class RoleDashboardPage extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final Color accentAlt;
  final DashboardLeadingAction? leadingAction;
  final double bottomInset;
  final List<DashboardTopAction> topActions;
  final List<DashboardQuickAction> quickActions;
  final List<DashboardFeature> features;
  final List<Widget>? summaryMetrics;
  final List<DashboardRecentItem>? recentItems;
  final bool isLoading;
  final String? errorMessage;
  final Future<void> Function()? onRefresh;

  const RoleDashboardPage({
    super.key,
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.accentAlt,
    this.leadingAction,
    this.bottomInset = 0,
    required this.topActions,
    required this.quickActions,
    required this.features,
    this.summaryMetrics,
    this.recentItems,
    this.isLoading = false,
    this.errorMessage,
    this.onRefresh,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: RoleDashboardBody(
        title: title,
        subtitle: subtitle,
        icon: icon,
        accent: accent,
        accentAlt: accentAlt,
        leadingAction: leadingAction,
        bottomInset: bottomInset,
        topActions: topActions,
        quickActions: quickActions,
        features: features,
        summaryMetrics: summaryMetrics,
        recentItems: recentItems,
        isLoading: isLoading,
        errorMessage: errorMessage,
        onRefresh: onRefresh,
      ),
    );
  }
}

class RoleDashboardBody extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final Color accentAlt;
  final DashboardLeadingAction? leadingAction;
  final double bottomInset;
  final List<DashboardTopAction> topActions;
  final List<DashboardQuickAction> quickActions;
  final List<DashboardFeature> features;
  final List<Widget>? summaryMetrics;
  final List<DashboardRecentItem>? recentItems;
  final bool isLoading;
  final String? errorMessage;
  final Future<void> Function()? onRefresh;
  /// `false` khi dùng [RoleModuleHeader] bên ngoài (tab shell đồng bộ Customer).
  final bool showInternalHeader;

  const RoleDashboardBody({
    super.key,
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.accentAlt,
    this.leadingAction,
    this.bottomInset = 0,
    required this.topActions,
    required this.quickActions,
    required this.features,
    this.summaryMetrics,
    this.recentItems,
    this.isLoading = false,
    this.errorMessage,
    this.onRefresh,
    this.showInternalHeader = true,
  });

  @override
  Widget build(BuildContext context) {
    final tabInset = RoleTabScope.maybeOf(context)?.bottomInset ?? bottomInset;
    final keyboardInset = MediaQuery.viewInsetsOf(context).bottom;
    final scrollBottom = tabInset + keyboardInset + 24;

    final scrollView = CustomScrollView(
      physics: const AlwaysScrollableScrollPhysics(
        parent: BouncingScrollPhysics(),
      ),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      slivers: [
        if (showInternalHeader)
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 16, 20, 16),
              child: _DashboardHeader(
                title: title,
                subtitle: subtitle,
                icon: icon,
                accent: accent,
                accentAlt: accentAlt,
                leadingAction: leadingAction,
                topActions: topActions,
                quickActions: quickActions,
              ),
            ),
          ),
        if (errorMessage != null && errorMessage!.isNotEmpty)
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 0, 20, 12),
              child: _DashboardErrorBanner(
                message: errorMessage!,
                onRetry: onRefresh,
              ),
            ),
          ),
        if (summaryMetrics != null && summaryMetrics!.isNotEmpty)
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.only(bottom: 20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 20),
                    child: Row(
                      children: [
                        Text(
                          'Số liệu tổng quan',
                          style: AppDesignSystem.sectionTitle(),
                        ),
                        if (isLoading) ...[
                          const SizedBox(width: 8),
                          SizedBox(
                            width: 16,
                            height: 16,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: accent,
                            ),
                          ),
                        ],
                      ],
                    ),
                  ),
                  const SizedBox(height: 12),
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 20),
                    child: _MetricsGrid(
                      metrics: summaryMetrics!,
                      isLoading: isLoading,
                      accent: accent,
                    ),
                  ),
                ],
              ),
            ),
          ),
        if (recentItems != null && recentItems!.isNotEmpty)
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.only(bottom: 20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 20),
                    child: Text(
                      'Hoạt động gần đây',
                      style: AppDesignSystem.sectionTitle(),
                    ),
                  ),
                  const SizedBox(height: 10),
                  ...recentItems!.map(
                    (item) => Padding(
                      padding: const EdgeInsets.fromLTRB(20, 0, 20, 8),
                      child: _RecentItemTile(item: item, accent: accent),
                    ),
                  ),
                ],
              ),
            ),
          ),
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.fromLTRB(20, 0, 20, 8),
            child: Text(
              'Truy cập module',
              style: AppDesignSystem.sectionTitle(),
            ),
          ),
        ),
        SliverPadding(
          padding: EdgeInsets.fromLTRB(20, 0, 20, scrollBottom),
          sliver: SliverList(
            delegate: SliverChildBuilderDelegate(
              (context, index) => Padding(
                padding: const EdgeInsets.only(bottom: 10),
                child: _FeatureNavTile(feature: features[index]),
              ),
              childCount: features.length,
            ),
          ),
        ),
      ],
    );

    return ColoredBox(
      color: AppDesignSystem.gray50,
      child: onRefresh != null
          ? RefreshIndicator(
              onRefresh: onRefresh!,
              color: accent,
              child: scrollView,
            )
          : scrollView,
    );
  }
}

class _MetricsGrid extends StatelessWidget {
  final List<Widget> metrics;
  final bool isLoading;
  final Color accent;

  const _MetricsGrid({
    required this.metrics,
    required this.isLoading,
    required this.accent,
  });

  @override
  Widget build(BuildContext context) {
    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        crossAxisSpacing: 12,
        mainAxisSpacing: 12,
        childAspectRatio: 1.35,
      ),
      itemCount: metrics.length,
      itemBuilder: (context, index) {
        if (isLoading) {
          return _MetricSkeleton(accent: accent);
        }
        return metrics[index];
      },
    );
  }
}

class _MetricSkeleton extends StatelessWidget {
  final Color accent;

  const _MetricSkeleton({required this.accent});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusXl),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const SizedBox(height: 16),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Container(
              width: 28,
              height: 28,
              decoration: BoxDecoration(
                color: accent.withValues(alpha: 0.12),
                shape: BoxShape.circle,
              ),
            ),
          ),
          const Spacer(),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Container(
              height: 14,
              width: 72,
              decoration: BoxDecoration(
                color: AppDesignSystem.gray100,
                borderRadius: BorderRadius.circular(6),
              ),
            ),
          ),
          const SizedBox(height: 8),
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
            child: Container(
              height: 10,
              width: 100,
              decoration: BoxDecoration(
                color: AppDesignSystem.gray100,
                borderRadius: BorderRadius.circular(6),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _DashboardErrorBanner extends StatelessWidget {
  final String message;
  final Future<void> Function()? onRetry;

  const _DashboardErrorBanner({required this.message, this.onRetry});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppDesignSystem.danger.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppDesignSystem.danger.withValues(alpha: 0.2)),
      ),
      child: Row(
        children: [
          const Icon(Icons.error_outline, color: AppDesignSystem.danger, size: 20),
          const SizedBox(width: 10),
          Expanded(child: Text(message, style: AppDesignSystem.body(size: 13, color: AppDesignSystem.danger))),
          if (onRetry != null)
            TextButton(
              onPressed: () => onRetry!(),
              child: const Text('Thử lại'),
            ),
        ],
      ),
    );
  }
}

class _RecentItemTile extends StatelessWidget {
  final DashboardRecentItem item;
  final Color accent;

  const _RecentItemTile({required this.item, required this.accent});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(14),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: item.route != null
            ? () => Navigator.of(context).pushNamed(
                  item.route!,
                  arguments: item.routeArguments,
                )
            : null,
        child: Container(
          padding: const EdgeInsets.all(14),
          decoration: AppDesignSystem.card(radius: 14),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: (item.iconColor ?? accent).withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(item.icon, color: item.iconColor ?? accent, size: 20),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(item.title, style: AppDesignSystem.label()),
                    const SizedBox(height: 2),
                    Text(item.subtitle, style: AppDesignSystem.body(size: 12)),
                  ],
                ),
              ),
              if (item.trailing != null)
                Text(
                  item.trailing!,
                  style: AppDesignSystem.body(size: 12, color: accent).copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              if (item.route != null) ...[
                const SizedBox(width: 4),
                Icon(Icons.chevron_right, color: accent.withValues(alpha: 0.6), size: 20),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

class _FeatureNavTile extends StatelessWidget {
  final DashboardFeature feature;

  const _FeatureNavTile({required this.feature});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: () => Navigator.of(context).pushNamed(feature.route),
        borderRadius: BorderRadius.circular(16),
        child: Ink(
          decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: feature.color.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Icon(feature.icon, color: feature.color, size: 22),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(feature.title, style: AppDesignSystem.sectionTitle()),
                      const SizedBox(height: 2),
                      Text(feature.subtitle, style: AppDesignSystem.body(size: 12)),
                      if (feature.preview != null) ...[
                        const SizedBox(height: 4),
                        Text(
                          feature.preview!,
                          style: AppDesignSystem.body(size: 11, color: feature.color).copyWith(
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
                Icon(Icons.arrow_forward_ios_rounded, size: 16, color: feature.color),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class DashboardMetricCard extends StatelessWidget {
  final String title;
  final String value;
  final IconData icon;
  final Color color;
  final String? trend;
  final bool? trendPositive;

  const DashboardMetricCard({
    super.key,
    required this.title,
    required this.value,
    required this.icon,
    required this.color,
    this.trend,
    this.trendPositive,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusXl),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(6),
                decoration: BoxDecoration(
                  color: color.withValues(alpha: 0.1),
                  shape: BoxShape.circle,
                ),
                child: Icon(icon, color: color, size: 16),
              ),
              const Spacer(),
              if (trend != null)
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                  decoration: BoxDecoration(
                    color: (trendPositive ?? true)
                        ? Colors.green.shade50
                        : Colors.red.shade50,
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    trend!,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(
                      color: (trendPositive ?? true)
                          ? Colors.green.shade700
                          : Colors.red.shade700,
                      fontSize: 9,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
            ],
          ),
          const Spacer(),
          Text(
            value,
            style: AppDesignSystem.title(size: 18, color: AppDesignSystem.gray900),
          ),
          const SizedBox(height: 2),
          Text(
            title,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: AppDesignSystem.body(size: 11),
          ),
        ],
      ),
    );
  }
}

class _DashboardHeader extends StatelessWidget {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final Color accentAlt;
  final DashboardLeadingAction? leadingAction;
  final List<DashboardTopAction> topActions;
  final List<DashboardQuickAction> quickActions;

  const _DashboardHeader({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.accentAlt,
    this.leadingAction,
    required this.topActions,
    required this.quickActions,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [accent, accentAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusXl),
        boxShadow: [
          BoxShadow(
            color: accent.withValues(alpha: 0.25),
            blurRadius: 16,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              if (leadingAction != null)
                IconButton(
                  tooltip: leadingAction?.tooltip,
                  onPressed: leadingAction?.onTap,
                  icon: Icon(leadingAction?.icon, color: Colors.white),
                ),
              if (leadingAction != null) const SizedBox(width: 4),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.18),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: Icon(icon, color: Colors.white),
              ),
              const SizedBox(width: 12),
              Text(
                'HUITMeal',
                style: AppDesignSystem.font.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.w900,
                  fontSize: 18,
                ),
              ),
              const Spacer(),
              ...topActions.map(
                (action) => IconButton(
                  tooltip: action.tooltip,
                  onPressed: () {
                    final customTap = action.onTap;
                    if (customTap != null) {
                      customTap();
                      return;
                    }
                    final route = action.route;
                    if (route == null) {
                      return;
                    }
                    Navigator.of(context).pushNamed(route);
                  },
                  icon: Icon(action.icon, color: Colors.white),
                ),
              ),
            ],
          ),
          const SizedBox(height: 18),
          Text(
            title,
            style: AppDesignSystem.title(size: 22, color: Colors.white),
          ),
          const SizedBox(height: 8),
          Text(
            subtitle,
            style: AppDesignSystem.body(color: Colors.white.withValues(alpha: 0.9)),
          ),
          if (quickActions.isNotEmpty) const SizedBox(height: 16),
          if (quickActions.isNotEmpty)
            Wrap(
              spacing: 12,
              runSpacing: 8,
              children:
                  quickActions
                      .map(
                        (action) => FilledButton.tonalIcon(
                          style: FilledButton.styleFrom(
                            backgroundColor: Colors.white,
                            foregroundColor: accent,
                          ),
                          onPressed:
                              () =>
                                  Navigator.of(context).pushNamed(action.route),
                          icon: Icon(action.icon),
                          label: Text(action.label),
                        ),
                      )
                      .toList(),
            ),
        ],
      ),
    );
  }
}

