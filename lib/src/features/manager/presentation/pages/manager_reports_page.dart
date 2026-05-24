import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../widgets/manager_ui.dart';

class ManagerReportsPage extends StatelessWidget {
  const ManagerReportsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final reports = [
      (
        title: 'Báo cáo thu chi',
        subtitle: 'Thu, chi, lợi nhuận ròng theo kỳ',
        icon: Icons.account_balance_wallet_rounded,
        color: AppDesignSystem.success,
        type: 'cashflow',
      ),
      (
        title: 'Thống kê suất ăn',
        subtitle: 'Suất ăn theo ngày, ca, đơn vị',
        icon: Icons.restaurant_rounded,
        color: AppDesignSystem.info,
        type: 'meals',
      ),
      (
        title: 'Đối soát thanh toán',
        subtitle: 'Lệch đơn và công nợ',
        icon: Icons.receipt_long_rounded,
        color: AppDesignSystem.warning,
        type: 'reconciliation',
      ),
      (
        title: 'Phản hồi khách hàng',
        subtitle: 'Đánh giá & khiếu nại',
        icon: Icons.feedback_rounded,
        color: managerAccent,
        type: 'feedback',
      ),
    ];

    return ManagerPageShell(
      title: 'Xuất báo cáo',
      body: ModuleListView(
        padding: managerListPadding(context),
        children: [
          const ManagerPageIntro(
            title: 'Trung tâm xuất báo cáo',
            description:
                'Chọn loại báo cáo, khoảng thời gian và định dạng Excel hoặc PDF. Dữ liệu lấy trực tiếp từ hệ thống.',
            icon: Icons.file_download_rounded,
          ),
          const SizedBox(height: 16),
          const ManagerSectionHeader(
            title: 'Loại báo cáo',
            subtitle: 'Nhấn để cấu hình và xuất',
          ),
          const SizedBox(height: 10),
          ...reports.map(
            (r) => ManagerReportNavCard(
              title: r.title,
              subtitle: r.subtitle,
              icon: r.icon,
              accent: r.color,
              onTap: () => Navigator.of(context).pushNamed(
                AppRoutes.managerReportExport,
                arguments: r.type,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
