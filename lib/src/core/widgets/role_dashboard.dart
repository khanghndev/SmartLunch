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
  /// Nội dung cuộn phía trên KPI (banner module, v.v.).
  final List<Widget>? prefixWidgets;

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
    this.prefixWidgets,
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
        if (prefixWidgets != null)
          for (final w in prefixWidgets!)
            SliverToBoxAdapter(child: w),
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
              padding: const EdgeInsets.only(bottom: 16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(20, 2, 20, 10),
                    child: _DashboardSectionHeader(
                      title: 'Số liệu tổng quan',
                      accent: accent,
                      isLoading: isLoading,
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 16),
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
              padding: const EdgeInsets.only(bottom: 16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(20, 0, 20, 12),
                    child: _DashboardSectionHeader(
                      title: 'Hoạt động gần đây',
                      accent: accent,
                      trailing: '${recentItems!.length} mục',
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    child: Container(
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(20),
                        boxShadow: [
                          BoxShadow(
                            color: accent.withValues(alpha: 0.08),
                            blurRadius: 20,
                            offset: const Offset(0, 8),
                          ),
                          BoxShadow(
                            color: Colors.black.withValues(alpha: 0.04),
                            blurRadius: 8,
                            offset: const Offset(0, 2),
                          ),
                        ],
                      ),
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(20),
                        child: Column(
                          children: [
                            for (var i = 0; i < recentItems!.length; i++) ...[
                              if (i > 0)
                                Padding(
                                  padding: const EdgeInsets.only(left: 64),
                                  child: Divider(
                                    height: 1,
                                    thickness: 0.5,
                                    color: AppDesignSystem.gray100,
                                  ),
                                ),
                              _RecentItemTile(
                                item: recentItems![i],
                                accent: accent,
                                compact: true,
                              ),
                            ],
                          ],
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.fromLTRB(20, 0, 20, 12),
            child: _DashboardSectionHeader(
              title: 'Truy cập nhanh',
              accent: accent,
              trailing: '${features.length} module',
            ),
          ),
        ),
        SliverPadding(
          padding: EdgeInsets.fromLTRB(16, 0, 16, scrollBottom),
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
      color: const Color(0xFFF5F7FA),
      child: onRefresh != null
          ? RefreshIndicator(
              onRefresh: onRefresh!,
              color: accent,
              strokeWidth: 2.5,
              displacement: 40,
              child: scrollView,
            )
          : scrollView,
    );
  }
}

class _DashboardSectionHeader extends StatelessWidget {
  final String title;
  final Color accent;
  final String? trailing;
  final bool isLoading;

  const _DashboardSectionHeader({
    required this.title,
    required this.accent,
    this.trailing,
    this.isLoading = false,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 3,
          height: 22,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [accent, accent.withValues(alpha: 0.3)],
            ),
            borderRadius: BorderRadius.circular(99),
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Text(
            title,
            style: AppDesignSystem.font.copyWith(
              fontSize: 15,
              fontWeight: FontWeight.w800,
              color: AppDesignSystem.gray900,
              letterSpacing: -0.2,
            ),
          ),
        ),
        if (isLoading)
          SizedBox(
            width: 14,
            height: 14,
            child: CircularProgressIndicator(
              strokeWidth: 2,
              color: accent,
            ),
          )
        else if (trailing != null)
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  accent.withValues(alpha: 0.12),
                  accent.withValues(alpha: 0.06),
                ],
              ),
              borderRadius: BorderRadius.circular(999),
              border: Border.all(
                color: accent.withValues(alpha: 0.15),
                width: 0.5,
              ),
            ),
            child: Text(
              trailing!,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: AppDesignSystem.font.copyWith(
                fontSize: 11,
                fontWeight: FontWeight.w700,
                color: accent,
              ),
            ),
          ),
      ],
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

  static double cellHeight(BuildContext context) {
    final textScale = MediaQuery.textScalerOf(context).scale(1.0).clamp(1.0, 1.3);
    final compact = MediaQuery.sizeOf(context).width < 380;
    return (compact ? 132.0 : 142.0) * textScale;
  }

  @override
  Widget build(BuildContext context) {
    const crossSpacing = 10.0;
    final mainAxisExtent = cellHeight(context);

    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        crossAxisSpacing: crossSpacing,
        mainAxisSpacing: crossSpacing,
        mainAxisExtent: mainAxisExtent,
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
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: AppDesignSystem.card(radius: 16),
      child: Row(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: accent.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(11),
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Container(
                  height: 18,
                  width: 64,
                  decoration: BoxDecoration(
                    color: AppDesignSystem.gray100,
                    borderRadius: BorderRadius.circular(6),
                  ),
                ),
                const SizedBox(height: 5),
                Container(
                  height: 10,
                  width: 48,
                  decoration: BoxDecoration(
                    color: AppDesignSystem.gray100,
                    borderRadius: BorderRadius.circular(6),
                  ),
                ),
              ],
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
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppDesignSystem.danger.withValues(alpha: 0.06),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppDesignSystem.danger.withValues(alpha: 0.18)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: AppDesignSystem.danger.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: const Icon(Icons.error_outline_rounded, color: AppDesignSystem.danger, size: 20),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              message,
              style: AppDesignSystem.body(size: 13, color: AppDesignSystem.danger),
            ),
          ),
          if (onRetry != null)
            TextButton(
              onPressed: () => onRetry!(),
              style: TextButton.styleFrom(
                foregroundColor: AppDesignSystem.danger,
                textStyle: AppDesignSystem.label(color: AppDesignSystem.danger),
              ),
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
  final bool compact;

  const _RecentItemTile({
    required this.item,
    required this.accent,
    this.compact = false,
  });

  @override
  Widget build(BuildContext context) {
    final iconColor = item.iconColor ?? accent;

    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: item.route != null
            ? () => Navigator.of(context).pushNamed(
                  item.route!,
                  arguments: item.routeArguments,
                )
            : null,
        child: Padding(
          padding: EdgeInsets.symmetric(
            horizontal: compact ? 12 : 14,
            vertical: compact ? 11 : 12,
          ),
          child: compact
              ? _buildContent(iconColor)
              : DecoratedBox(
                  decoration: AppDesignSystem.card(radius: 16),
                  child: Padding(
                    padding: const EdgeInsets.all(14),
                    child: _buildContent(iconColor),
                  ),
                ),
        ),
      ),
    );
  }

  Widget _buildContent(Color iconColor) {
    return Row(
      children: [
        Container(
          width: 42,
          height: 42,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [
                iconColor.withValues(alpha: 0.16),
                iconColor.withValues(alpha: 0.06),
              ],
            ),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Icon(item.icon, color: iconColor, size: 20),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                item.title,
                style: AppDesignSystem.label().copyWith(fontSize: 13.5),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 3),
              Text(
                item.subtitle,
                style: AppDesignSystem.body(size: 12),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ),
        ),
        if (item.trailing != null) ...[
          const SizedBox(width: 8),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                item.trailing!,
                style: AppDesignSystem.label(color: accent).copyWith(fontSize: 13),
              ),
            ],
          ),
        ],
        if (item.route != null) ...[
          const SizedBox(width: 4),
          Icon(
            Icons.chevron_right_rounded,
            color: AppDesignSystem.gray400,
            size: 22,
          ),
        ],
      ],
    );
  }
}

class _FeatureNavTile extends StatefulWidget {
  final DashboardFeature feature;

  const _FeatureNavTile({required this.feature});

  @override
  State<_FeatureNavTile> createState() => _FeatureNavTileState();
}

class _FeatureNavTileState extends State<_FeatureNavTile> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final feature = widget.feature;
    return GestureDetector(
      onTapDown: (_) => setState(() => _pressed = true),
      onTapUp: (_) {
        setState(() => _pressed = false);
        Navigator.of(context).pushNamed(feature.route);
      },
      onTapCancel: () => setState(() => _pressed = false),
      child: AnimatedScale(
        scale: _pressed ? 0.97 : 1.0,
        duration: const Duration(milliseconds: 120),
        curve: Curves.easeOut,
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            border: Border.all(
              color: feature.color.withValues(alpha: 0.12),
              width: 1,
            ),
            boxShadow: [
              BoxShadow(
                color: feature.color.withValues(alpha: 0.08),
                blurRadius: 16,
                offset: const Offset(0, 6),
              ),
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.03),
                blurRadius: 6,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 13),
            child: Row(
              children: [
                Container(
                  width: 50,
                  height: 50,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                      colors: [
                        feature.color.withValues(alpha: 0.22),
                        feature.color.withValues(alpha: 0.08),
                      ],
                    ),
                    borderRadius: BorderRadius.circular(15),
                    border: Border.all(
                      color: feature.color.withValues(alpha: 0.15),
                      width: 0.5,
                    ),
                  ),
                  child: Icon(feature.icon, color: feature.color, size: 24),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        feature.title,
                        style: AppDesignSystem.font.copyWith(
                          fontSize: 14.5,
                          fontWeight: FontWeight.w800,
                          color: AppDesignSystem.gray900,
                          letterSpacing: -0.2,
                        ),
                      ),
                      const SizedBox(height: 3),
                      Text(
                        feature.subtitle,
                        style: AppDesignSystem.font.copyWith(
                          fontSize: 12,
                          fontWeight: FontWeight.w500,
                          color: AppDesignSystem.gray500,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      if (feature.preview != null) ...[
                        const SizedBox(height: 6),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                          decoration: BoxDecoration(
                            color: feature.color.withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Text(
                            feature.preview!,
                            style: AppDesignSystem.font.copyWith(
                              fontSize: 11,
                              fontWeight: FontWeight.w700,
                              color: feature.color,
                            ),
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
                const SizedBox(width: 8),
                Container(
                  width: 34,
                  height: 34,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                      colors: [
                        feature.color.withValues(alpha: 0.15),
                        feature.color.withValues(alpha: 0.06),
                      ],
                    ),
                    borderRadius: BorderRadius.circular(11),
                  ),
                  child: Icon(
                    Icons.arrow_forward_ios_rounded,
                    size: 14,
                    color: feature.color,
                  ),
                ),
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
    final positive = trendPositive ?? true;
    final trendColor = positive ? AppDesignSystem.success : AppDesignSystem.danger;
    final trendBg = positive
        ? const Color(0xFF16A34A).withValues(alpha: 0.1)
        : const Color(0xFFDC2626).withValues(alpha: 0.1);
    final trendIcon = positive ? Icons.trending_up_rounded : Icons.trending_down_rounded;

    return Container(
      width: double.infinity,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: color.withValues(alpha: 0.12),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 6,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Stack(
          children: [
            // Decorative circles top-right
            Positioned(
              top: -18,
              right: -18,
              child: Container(
                width: 80,
                height: 80,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: color.withValues(alpha: 0.07),
                ),
              ),
            ),
            Positioned(
              top: -6,
              right: -6,
              child: Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: color.withValues(alpha: 0.10),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(13),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Container(
                        width: 38,
                        height: 38,
                        decoration: BoxDecoration(
                          gradient: LinearGradient(
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                            colors: [
                              color.withValues(alpha: 0.22),
                              color.withValues(alpha: 0.08),
                            ],
                          ),
                          borderRadius: BorderRadius.circular(11),
                          border: Border.all(
                            color: color.withValues(alpha: 0.2),
                            width: 0.5,
                          ),
                        ),
                        child: Icon(icon, color: color, size: 19),
                      ),
                      if (trend != null)
                        Flexible(
                          child: Container(
                            margin: const EdgeInsets.only(left: 6),
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 3),
                            decoration: BoxDecoration(
                              color: trendBg,
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(trendIcon, size: 10, color: trendColor),
                                const SizedBox(width: 3),
                                Flexible(
                                  child: Text(
                                    trend!,
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                    style: AppDesignSystem.font.copyWith(
                                      fontSize: 9,
                                      fontWeight: FontWeight.w700,
                                      color: trendColor,
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  FittedBox(
                    fit: BoxFit.scaleDown,
                    alignment: Alignment.centerLeft,
                    child: Text(
                      value,
                      style: AppDesignSystem.font.copyWith(
                        fontSize: 23,
                        fontWeight: FontWeight.w800,
                        color: AppDesignSystem.gray900,
                        letterSpacing: -0.8,
                        height: 1.1,
                      ),
                      maxLines: 1,
                    ),
                  ),
                  const SizedBox(height: 3),
                  Text(
                    title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: AppDesignSystem.font.copyWith(
                      fontSize: 11,
                      fontWeight: FontWeight.w600,
                      color: AppDesignSystem.gray400,
                      letterSpacing: 0.1,
                    ),
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
            color: accent.withValues(alpha: 0.3),
            blurRadius: 20,
            offset: const Offset(0, 10),
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
