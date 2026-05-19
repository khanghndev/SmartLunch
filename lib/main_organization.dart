import 'package:flutter/material.dart';

import 'src/app/app.dart';
import 'src/app/app_config.dart';

void main() {
  runApp(
    const SmartLunchApp(
      config: AppConfig(
        flavor: AppFlavor.organization,
        appName: 'HUITMeal Organization',
      ),
    ),
  );
}
