import 'package:flutter/material.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../../../core/widgets/module_scroll.dart';

export '../../../../core/widgets/module_page_shell.dart';
export '../../../../core/widgets/module_scroll.dart';

String orgApiError(Object e) {
  if (e is ApiException) {
    final m = e.message;
    if (m.contains('RecipientName')) {
      return 'Tên người nhận phải có ít nhất 2 ký tự.';
    }
    if (m.contains('RecipientPhone')) {
      return 'Số điện thoại người nhận không hợp lệ.';
    }
    if (m.contains('RecipientEmail')) {
      return 'Email người nhận không hợp lệ.';
    }
    if (m.contains('DeliveryAddress')) {
      return 'Địa chỉ giao phải có ít nhất 10 ký tự.';
    }
    if (m.contains('PreferredDeliveryTime')) {
      return 'Giờ giao mong muốn phải theo định dạng HH:mm.';
    }
    if (m.contains('returnUrl') && m.contains('cancelUrl')) {
      return 'Thiếu URL quay về PayOS. Liên hệ quản trị hoặc cấu hình SMARTLUNCH_WEB.';
    }
    return m;
  }
  return e.toString();
}

const RolePalette kOrgRole = RolePalette.organization;

Color get orgAccent => kOrgRole.primary;

EdgeInsets orgListPadding(BuildContext context) =>
    moduleListPadding(context, bottomBarInset: 0);

String formatOrgVnd(double amount) {
  final s = amount.toStringAsFixed(0).replaceAllMapped(
        RegExp(r'(\d)(?=(\d{3})+(?!\d))'),
        (m) => '${m[1]}.',
      );
  return '$s đ';
}

// ─── Shell ───────────────────────────────────────────────────────────────────

class OrgPageShell extends StatelessWidget {
  final String title;
  final Widget body;
  final List<Widget>? actions;
  final Future<void> Function()? onRefresh;

  const OrgPageShell({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.onRefresh,
  });

  @override
  Widget build(BuildContext context) {
    return ModulePageShell(
      title: title,
      role: kOrgRole,
      body: body,
      actions: actions,
      onRefresh: onRefresh,
    );
  }
}

// ─── Module con — UI chuẩn Organization ────────────────────────────────────

class OrgPageIntro extends StatelessWidget {
  final String title;
  final String description;
  final IconData icon;

  const OrgPageIntro({
    super.key,
    required this.title,
    required this.description,
    this.icon = Icons.business_rounded,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            kOrgRole.primary.withValues(alpha: 0.12),
            kOrgRole.primaryAlt.withValues(alpha: 0.06),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: kOrgRole.primary.withValues(alpha: 0.2)),
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
            child: Icon(icon, color: kOrgRole.primary, size: 26),
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

class OrgSectionHeader extends StatelessWidget {
  final String title;
  final String? subtitle;
  final Widget? trailing;

  const OrgSectionHeader({
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

class OrgTabBar extends StatelessWidget {
  final TabController controller;
  final List<String> tabs;

  const OrgTabBar({super.key, required this.controller, required this.tabs});

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
          labelColor: kOrgRole.primary,
          unselectedLabelColor: AppDesignSystem.gray500,
          indicatorColor: kOrgRole.primary,
          indicatorWeight: 3,
          labelStyle: AppDesignSystem.label(color: kOrgRole.link),
          unselectedLabelStyle: AppDesignSystem.body(size: 13),
          dividerColor: Colors.transparent,
          tabs: tabs.map((t) => Tab(text: t)).toList(),
        ),
      ),
    );
  }
}

class OrgTabbedBody extends StatelessWidget {
  final TabController controller;
  final List<String> tabLabels;
  final List<Widget> children;
  final Widget? top;

  const OrgTabbedBody({
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
        OrgTabBar(controller: controller, tabs: tabLabels),
        Expanded(
          child: TabBarView(controller: controller, children: children),
        ),
      ],
    );
  }
}

class OrgStatusBadge extends StatelessWidget {
  final String label;
  final Color color;

  const OrgStatusBadge({super.key, required this.label, required this.color});

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

class OrgDataRow extends StatelessWidget {
  final IconData icon;
  final Color iconColor;
  final String title;
  final String? subtitle;
  final String? trailing;
  final Color? trailingColor;
  final Widget? badge;
  final VoidCallback? onTap;

  const OrgDataRow({
    super.key,
    required this.icon,
    required this.iconColor,
    required this.title,
    this.subtitle,
    this.trailing,
    this.trailingColor,
    this.badge,
    this.onTap,
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
        ),
      ),
    );
  }
}

class OrgInfoBanner extends StatelessWidget {
  final String message;
  final IconData icon;
  final Color color;

  const OrgInfoBanner({
    super.key,
    required this.message,
    this.icon = Icons.info_outline_rounded,
    this.color = AppDesignSystem.warning,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withValues(alpha: 0.25)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: color, size: 22),
          const SizedBox(width: 10),
          Expanded(
            child: Text(message, style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700)),
          ),
        ],
      ),
    );
  }
}

class OrgPrimaryButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final bool loading;
  final VoidCallback? onPressed;

  const OrgPrimaryButton({
    super.key,
    required this.label,
    this.icon = Icons.arrow_forward_rounded,
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
          : Icon(icon, size: 20),
      label: Text(label, style: AppDesignSystem.label(color: Colors.white)),
      style: FilledButton.styleFrom(
        backgroundColor: kOrgRole.primary,
        disabledBackgroundColor: kOrgRole.primary.withValues(alpha: 0.5),
        minimumSize: const Size.fromHeight(48),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
    );
  }
}

class OrgCard extends StatelessWidget {
  final Widget child;
  final EdgeInsetsGeometry? padding;

  const OrgCard({super.key, required this.child, this.padding});

  @override
  Widget build(BuildContext context) {
    return ModuleCard(padding: padding ?? const EdgeInsets.all(20), child: child);
  }
}

class OrgLoadingBody extends StatelessWidget {
  final String? message;

  const OrgLoadingBody({super.key, this.message});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(40),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(color: kOrgRole.primary),
            if (message != null) ...[
              const SizedBox(height: 16),
              Text(message!, style: AppDesignSystem.body(), textAlign: TextAlign.center),
            ],
          ],
        ),
      ),
    );
  }
}

class OrgErrorBody extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const OrgErrorBody({super.key, required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) => ModuleErrorBody(
        message: message,
        onRetry: onRetry,
        role: kOrgRole,
      );
}

class OrgPeriodChips extends StatelessWidget {
  final List<String> labels;
  final int selected;
  final ValueChanged<int> onSelected;

  const OrgPeriodChips({
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
        role: kOrgRole,
      );
}

class OrgStatTile extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final Color color;

  const OrgStatTile({
    super.key,
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) => ModuleStatTile(
        label: label,
        value: value,
        icon: icon,
        color: color,
      );
}

typedef OrgEmptyList = ModuleEmptyList;

class OrgDetailField extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const OrgDetailField({
    super.key,
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: orgAccent.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(icon, size: 20, color: orgAccent),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500)),
                Text(value, style: AppDesignSystem.label()),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
