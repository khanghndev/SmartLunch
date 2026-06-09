import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';

import '../core/navigation/session_guard.dart';
import '../core/theme/app_design_system.dart';
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
      navigatorKey: SessionGuard.instance.navigatorKey,
      title: config.appName,
      theme: AppTheme.light(config.flavor),
      debugShowCheckedModeBanner: false,
      locale: const Locale('vi', 'VN'),
      supportedLocales: const [
        Locale('vi', 'VN'),
        Locale('en', 'US'),
      ],
      localizationsDelegates: const [
        GlobalMaterialLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
      ],
      onGenerateRoute: router.onGenerateRoute,
      initialRoute: AppRoutes.initialFor(config.flavor),
      builder: (context, child) {
        final mq = MediaQuery.of(context);
        final clampedScaler = mq.textScaler.clamp(
          minScaleFactor: 0.9,
          maxScaleFactor: 1.2,
        );
        return MediaQuery(
          data: mq.copyWith(textScaler: clampedScaler),
          child: DefaultTextStyle(
            style: AppDesignSystem.body(color: AppDesignSystem.gray900),
            child: child ?? const SizedBox.shrink(),
          ),
        );
      },
    );
  }
}
