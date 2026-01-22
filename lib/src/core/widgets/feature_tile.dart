import 'package:flutter/material.dart';

class FeatureTile extends StatelessWidget {
  final String label;
  final String route;
  final IconData icon;

  const FeatureTile({
    super.key,
    required this.label,
    required this.route,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon),
      title: Text(label),
      trailing: const Icon(Icons.chevron_right),
      onTap: () => Navigator.of(context).pushNamed(route),
    );
  }
}
