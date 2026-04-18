import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';
import '../../../presentation/tabs/menu_tab.dart';

class MenuPage extends StatelessWidget {
  const MenuPage({super.key});

  @override
  Widget build(BuildContext context) {
    final bottomInset = MediaQuery.of(context).padding.bottom + 12;
    return Scaffold(
      appBar: AppBar(
        title: const Text('Thực đơn'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: MenuTab(bottomInset: bottomInset, showBack: false),
    );
  }
}
