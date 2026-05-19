import 'package:flutter/material.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';

export '../../../../core/widgets/module_page_shell.dart';

String orgApiError(Object e) {
  if (e is ApiException) return e.message;
  return e.toString();
}

const RolePalette kOrgRole = RolePalette.organization;

Color get orgAccent => kOrgRole.primary;

String formatOrgVnd(double amount) {
  final s = amount.toStringAsFixed(0).replaceAllMapped(
        RegExp(r'(\d)(?=(\d{3})+(?!\d))'),
        (m) => '${m[1]}.',
      );
  return '$s đ';
}

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

class OrgLoadingBody extends StatelessWidget {
  const OrgLoadingBody({super.key});

  @override
  Widget build(BuildContext context) =>
      ModuleLoadingBody(color: kOrgRole.primary);
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

class OrgCard extends StatelessWidget {
  final Widget child;
  final EdgeInsetsGeometry? padding;

  const OrgCard({super.key, required this.child, this.padding});

  @override
  Widget build(BuildContext context) {
    return ModuleCard(padding: padding ?? const EdgeInsets.all(20), child: child);
  }
}
