import 'package:flutter/material.dart';

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

  const DashboardFeature({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.color,
    required this.route,
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
  });

  @override
  Widget build(BuildContext context) {
    final width = MediaQuery.of(context).size.width;
    final crossAxisCount =
        width >= 900
            ? 3
            : width >= 600
            ? 2
            : 1;
    final childAspectRatio = width >= 600 ? 1.4 : 1.3;

    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFFF5F6F8), Color(0xFFEFF4F5)],
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
        ),
      ),
      child: SafeArea(
        child: CustomScrollView(
          slivers: [
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
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
            SliverPadding(
              padding: EdgeInsets.fromLTRB(20, 0, 20, 24 + bottomInset),
              sliver: SliverGrid(
                delegate: SliverChildBuilderDelegate(
                  (context, index) => _FeatureCard(feature: features[index]),
                  childCount: features.length,
                ),
                gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: crossAxisCount,
                  crossAxisSpacing: 16,
                  mainAxisSpacing: 16,
                  childAspectRatio: childAspectRatio,
                ),
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
        borderRadius: BorderRadius.circular(26),
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
                  color: Colors.white.withOpacity(0.18),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: Icon(icon, color: Colors.white),
              ),
              const SizedBox(width: 12),
              Text(
                'SmartLunch',
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
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
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w700,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            subtitle,
            style: Theme.of(context).textTheme.bodyLarge?.copyWith(
              color: Colors.white.withOpacity(0.9),
            ),
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

class _FeatureCard extends StatelessWidget {
  final DashboardFeature feature;

  const _FeatureCard({required this.feature});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: () => Navigator.of(context).pushNamed(feature.route),
      borderRadius: BorderRadius.circular(20),
      child: Ink(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.06),
              blurRadius: 18,
              offset: const Offset(0, 8),
            ),
          ],
        ),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: feature.color.withOpacity(0.15),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: Icon(feature.icon, color: feature.color),
              ),
              const SizedBox(height: 14),
              Text(
                feature.title,
                style: Theme.of(
                  context,
                ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
              ),
              const SizedBox(height: 6),
              Text(
                feature.subtitle,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.black.withOpacity(0.6),
                ),
              ),
              const Spacer(),
              Align(
                alignment: Alignment.bottomRight,
                child: Icon(Icons.arrow_forward, color: feature.color),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
