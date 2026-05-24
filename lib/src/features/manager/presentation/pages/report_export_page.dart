import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../core/analytics/meal_statistics_repository.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/finance_models.dart';
import '../../data/models/review_complaint_models.dart';
import '../../data/repositories/manager_repository.dart';
import '../widgets/manager_ui.dart';

class ReportExportPage extends StatefulWidget {
  const ReportExportPage({super.key});

  @override
  State<ReportExportPage> createState() => _ReportExportPageState();
}

class _ReportExportPageState extends State<ReportExportPage> {
  String _reportType = 'cashflow';
  int _rangeIndex = 1;
  String _format = 'excel';
  bool _exporting = false;
  bool _argsRead = false;

  static const _ranges = ['7 ngày', '30 ngày', '90 ngày'];
  static const _rangeDays = [7, 30, 90];

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (!_argsRead) {
      _reportType = ModalRoute.of(context)?.settings.arguments as String? ?? 'cashflow';
      _argsRead = true;
    }
  }

  (DateTime, DateTime) _range() {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final days = _rangeDays[_rangeIndex];
    return (today.subtract(Duration(days: days - 1)), today);
  }

  String _reportTitle() {
    switch (_reportType) {
      case 'meals':
        return 'Thống kê suất ăn';
      case 'reconciliation':
        return 'Đối soát thanh toán';
      case 'feedback':
        return 'Phản hồi khách hàng';
      default:
        return 'Báo cáo thu chi';
    }
  }

  Future<String> _buildReportContent() async {
    final (start, end) = _range();
    final buffer = StringBuffer();
    buffer.writeln('HUITMeal — $_reportTitle');
    buffer.writeln('Kỳ: ${managerDateQuery(start)} → ${managerDateQuery(end)}');
    buffer.writeln('Định dạng: ${_format.toUpperCase()}');
    buffer.writeln('---');

    switch (_reportType) {
      case 'meals':
        final bundle = await fetchMealStatsBundle(startDate: start, endDate: end);
        buffer.writeln('Tổng suất: ${bundle.meals.sumMeals}');
        buffer.writeln('Doanh thu: ${formatVnd(bundle.meals.sumAmount)}');
        for (final e in bundle.meals) {
          buffer.writeln(
            '${managerDateQuery(e.date)} | ${e.mealSlot} | ${e.organizationName} | ${e.totalMeals} suất | ${formatVnd(e.totalAmount)}',
          );
        }
        break;
      case 'reconciliation':
        final recon = await ManagerRepository.instance.getPaymentReconciliation(
          startDate: start,
          endDate: end,
          onlyMismatches: false,
        );
        final recv = await ManagerRepository.instance.getOrganizationReceivables();
        buffer.writeln('Đơn lệch: ${recon.mismatchCount}/${recon.orderCount}');
        for (final line in recon.items.take(50)) {
          buffer.writeln(
            'ĐH ${line.id} | ${line.orgName} | ${line.status.label} | ${formatVnd(line.difference)}',
          );
        }
        buffer.writeln('Công nợ ĐV: ${formatVnd(recv.grandTotal)}');
        break;
      case 'feedback':
        final reviews = await ManagerRepository.instance.getReviews(pageSize: 50);
        final complaints = await ManagerRepository.instance.getComplaints(pageSize: 50);
        buffer.writeln(
          'Đánh giá: ${reviews.totalCount} (TB ${reviews.averageRating.toStringAsFixed(1)}★)',
        );
        for (final r in reviews.items) {
          buffer.writeln('★${r.stars} ${r.customerName}: ${r.comment}');
        }
        buffer.writeln('Khiếu nại: ${complaints.totalCount}');
        for (final c in complaints.items) {
          buffer.writeln('[${c.status.label}] ${c.title}: ${c.description}');
        }
        break;
      default:
        final cash = await ManagerRepository.instance.getCashFlowSummary(
          startDate: start,
          endDate: end,
          granularity: 'Day',
        );
        buffer.writeln('Thu: ${formatVnd(cash.totalIncome)}');
        buffer.writeln('Chi: ${formatVnd(cash.totalExpense)}');
        buffer.writeln('Ròng: ${formatVnd(cash.totalProfit)}');
        for (final b in cash.items) {
          buffer.writeln('${b.periodKey} | Thu ${formatVnd(b.income)} | Chi ${formatVnd(b.expense)}');
        }
    }
    return buffer.toString();
  }

  Future<void> _export() async {
    setState(() => _exporting = true);
    try {
      final content = await _buildReportContent();
      await Clipboard.setData(ClipboardData(text: content));
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Đã tạo báo cáo ${_format.toUpperCase()} và sao chép nội dung. '
            'Dán vào Excel hoặc trình soạn PDF để lưu file.',
            style: const TextStyle(fontSize: 13),
          ),
          behavior: SnackBarBehavior.floating,
          backgroundColor: managerAccent,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(apiErrorMessage(e)),
          backgroundColor: AppDesignSystem.danger,
          behavior: SnackBarBehavior.floating,
        ),
      );
    } finally {
      if (mounted) setState(() => _exporting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return ManagerPageShell(
      title: 'Xuất: $_reportTitle',
      body: ModuleListView(
        padding: managerListPadding(context),
        children: [
          ManagerPageIntro(
            title: _reportTitle(),
            description: 'Chọn kỳ và định dạng, sau đó nhấn xuất để sao chép dữ liệu báo cáo.',
            icon: Icons.description_outlined,
          ),
          const SizedBox(height: 14),
          ManagerGlassCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const ManagerSectionHeader(
                  title: 'Khoảng thời gian',
                  subtitle: 'Dữ liệu lấy theo số ngày gần nhất',
                ),
                const SizedBox(height: 12),
                ManagerPeriodChips(
                  labels: _ranges,
                  selected: _rangeIndex,
                  onSelected: (i) => setState(() => _rangeIndex = i),
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          ManagerGlassCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const ManagerSectionHeader(title: 'Định dạng xuất'),
                const SizedBox(height: 4),
                _FormatTile(
                  title: 'Excel / CSV',
                  subtitle: 'Bảng dữ liệu — dán vào Excel',
                  value: 'excel',
                  groupValue: _format,
                  onSelect: (v) => setState(() => _format = v),
                ),
                _FormatTile(
                  title: 'PDF (văn bản)',
                  subtitle: 'Nội dung có cấu trúc — dán vào Word/PDF',
                  value: 'pdf',
                  groupValue: _format,
                  onSelect: (v) => setState(() => _format = v),
                ),
              ],
            ),
          ),
          const SizedBox(height: 24),
          ManagerPrimaryButton(
            label: _exporting ? 'Đang tạo báo cáo…' : 'Xuất báo cáo',
            icon: Icons.download_rounded,
            loading: _exporting,
            onPressed: _export,
          ),
        ],
      ),
    );
  }
}

class _FormatTile extends StatelessWidget {
  final String title;
  final String subtitle;
  final String value;
  final String groupValue;
  final ValueChanged<String> onSelect;

  const _FormatTile({
    required this.title,
    required this.subtitle,
    required this.value,
    required this.groupValue,
    required this.onSelect,
  });

  @override
  Widget build(BuildContext context) {
    final selected = value == groupValue;
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: () => onSelect(value),
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 8),
          child: Row(
            children: [
              Icon(
                selected ? Icons.radio_button_checked : Icons.radio_button_off,
                color: selected ? managerAccent : AppDesignSystem.gray400,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(title, style: AppDesignSystem.label()),
                    Text(subtitle, style: AppDesignSystem.body(size: 12)),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
