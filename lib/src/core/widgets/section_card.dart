import 'package:flutter/material.dart';

import '../theme/app_design_system.dart';

class SectionCard extends StatelessWidget {
  final String title;
  final List<Widget> children;

  const SectionCard({super.key, required this.title, required this.children});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: AppDesignSystem.card(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: AppDesignSystem.sectionTitle()),
          const SizedBox(height: 10),
          ...children,
        ],
      ),
    );
  }
}
