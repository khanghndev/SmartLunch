import 'package:flutter/material.dart';

import '../core/widgets/feature_placeholder.dart';

class NotFoundPage extends StatelessWidget {
  final String routeName;

  const NotFoundPage({super.key, required this.routeName});

  @override
  Widget build(BuildContext context) {
    return FeaturePlaceholder(
      title: 'Not found',
      description: 'Route not found: $routeName',
    );
  }
}
