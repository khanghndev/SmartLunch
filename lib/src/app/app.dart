import 'package:flutter/material.dart';

import 'app_config.dart';
import 'app_routes.dart';
import 'router.dart';
import 'theme/app_theme.dart';

class SmartLunchApp extends StatelessWidget {
  final AppConfig config;

  const SmartLunchApp({super.key, required this.config});

  @override
  Widget build(BuildContext context) {
    final router = AppRouter(config);
    return MaterialApp(
      title: config.appName,
      theme: AppTheme.light(),
      debugShowCheckedModeBanner: false,
      onGenerateRoute: router.onGenerateRoute,
      initialRoute: AppRoutes.initialFor(config.flavor),
    );
  }
}
