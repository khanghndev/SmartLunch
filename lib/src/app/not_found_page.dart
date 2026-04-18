import 'package:flutter/material.dart';

import '../core/constants/app_colors.dart';
import '../core/widgets/app_scaffold.dart';

class NotFoundPage extends StatelessWidget {
  final String routeName;

  const NotFoundPage({super.key, required this.routeName});

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Không tìm thấy trang',
      child: Center(
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Container(
            width: 560,
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(24),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 72,
                  height: 72,
                  decoration: BoxDecoration(
                    color: AppColors.tint(AppColors.warning, 0.14),
                    borderRadius: BorderRadius.circular(22),
                  ),
                  child: const Icon(
                    Icons.travel_explore_rounded,
                    size: 36,
                    color: AppColors.warning,
                  ),
                ),
                const SizedBox(height: 14),
                Text(
                  'Route không hợp lệ',
                  style: Theme.of(
                    context,
                  ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w800),
                ),
                const SizedBox(height: 8),
                Text(
                  routeName,
                  style: Theme.of(
                    context,
                  ).textTheme.bodyMedium?.copyWith(color: AppColors.inkSoft),
                  textAlign: TextAlign.center,
                ),
                const SizedBox(height: 18),
                FilledButton.icon(
                  onPressed: () => Navigator.of(context).maybePop(),
                  icon: const Icon(Icons.arrow_back_rounded),
                  label: const Text('Quay lại'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
