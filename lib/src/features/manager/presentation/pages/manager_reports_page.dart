import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../widgets/manager_ui.dart';

class ManagerReportsPage extends StatelessWidget {
  const ManagerReportsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final reports = [
      _ReportOption(
        title: 'Báo cáo thu chi',
        subtitle: 'Xuất Excel / PDF theo kỳ',
        icon: Icons.account_balance_wallet_rounded,
        color: AppColors.success,
        type: 'cashflow',
      ),
      _ReportOption(
        title: 'Thống kê suất ăn',
        subtitle: 'Theo ngày, ca, bộ phận',
        icon: Icons.restaurant_rounded,
        color: AppColors.info,
        type: 'meals',
      ),
      _ReportOption(
        title: 'Đối soát thanh toán',
        subtitle: 'Danh sách lệch & công nợ',
        icon: Icons.receipt_long_rounded,
        color: AppColors.warning,
        type: 'reconciliation',
      ),
      _ReportOption(
        title: 'Phản hồi khách hàng',
        subtitle: 'Đánh giá & khiếu nại',
        icon: Icons.feedback_rounded,
        color: managerAccent,
        type: 'feedback',
      ),
    ];

    return ManagerPageShell(
      title: 'Xuất báo cáo',
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          ManagerGlassCard(
            child: Row(
              children: [
                Icon(Icons.file_download_rounded, color: managerAccent, size: 32),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    'Chọn loại báo cáo và định dạng xuất (Excel hoặc PDF). Dữ liệu lấy từ hệ thống theo khoảng thời gian bạn chọn.',
                    style: TextStyle(color: Colors.grey.shade700, height: 1.4),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          ...reports.map((r) => _ReportCard(option: r)),
        ],
      ),
    );
  }
}

class _ReportOption {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color color;
  final String type;

  const _ReportOption({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.color,
    required this.type,
  });
}

class _ReportCard extends StatelessWidget {
  final _ReportOption option;

  const _ReportCard({required this.option});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: ManagerGlassCard(
        child: InkWell(
          borderRadius: BorderRadius.circular(20),
          onTap: () {
            Navigator.of(context).pushNamed(
              AppRoutes.managerReportExport,
              arguments: option.type,
            );
          },
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 4),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: option.color.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(14),
                  ),
                  child: Icon(option.icon, color: option.color),
                ),
                const SizedBox(width: 14),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        option.title,
                        style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 16),
                      ),
                      Text(
                        option.subtitle,
                        style: TextStyle(color: Colors.grey.shade600, fontSize: 13),
                      ),
                    ],
                  ),
                ),
                Icon(Icons.chevron_right_rounded, color: Colors.grey.shade400),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
