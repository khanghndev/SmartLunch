import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class ReportsPage extends StatelessWidget {
  const ReportsPage({super.key});

  @override
  Widget build(BuildContext context) {
    final months = const [
      _MonthData(label: 'Tháng 3', selected: true),
      _MonthData(label: 'Tháng 2', selected: false),
      _MonthData(label: 'Tháng 1', selected: false),
    ];

    final reports = const [
      _ReportCardData(
        title: 'Tổng hợp chi phí',
        desc: 'Theo phòng ban và ca làm',
        size: '420 KB · PDF',
        color: AppColors.org,
      ),
      _ReportCardData(
        title: 'Biên bản đối soát',
        desc: 'Ký với SmartLunch',
        size: '180 KB · PDF',
        color: AppColors.org,
      ),
      _ReportCardData(
        title: 'Bảng điểm chất lượng',
        desc: 'Đánh giá & phản hồi nhân viên',
        size: '320 KB · XLSX',
        color: AppColors.orgAlt,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Báo cáo'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.history_rounded)),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            _MonthChips(months: months),
            const SizedBox(height: 14),
            _ReportCards(reports: reports),
            const SizedBox(height: 14),
            const _DeliverySummary(),
            const SizedBox(height: 14),
            const _NoteCard(),
          ],
        ),
      ),
    );
  }
}

class _MonthData {
  final String label;
  final bool selected;

  const _MonthData({required this.label, required this.selected});
}

class _MonthChips extends StatelessWidget {
  final List<_MonthData> months;

  const _MonthChips({required this.months});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        children:
            months
                .map(
                  (m) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: ChoiceChip(
                      label: Text(m.label),
                      selected: m.selected,
                      onSelected: (_) {},
                      selectedColor: AppColors.org.withOpacity(0.14),
                      labelStyle: TextStyle(
                        color:
                            m.selected
                                ? AppColors.org
                                : AppColors.ink.withOpacity(0.75),
                        fontWeight: FontWeight.w800,
                      ),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                        side: BorderSide(
                          color:
                              m.selected
                                  ? AppColors.org.withOpacity(0.45)
                                  : AppColors.ink.withOpacity(0.08),
                        ),
                      ),
                    ),
                  ),
                )
                .toList(),
      ),
    );
  }
}

class _ReportCardData {
  final String title;
  final String desc;
  final String size;
  final Color color;

  const _ReportCardData({
    required this.title,
    required this.desc,
    required this.size,
    required this.color,
  });
}

class _ReportCards extends StatelessWidget {
  final List<_ReportCardData> reports;

  const _ReportCards({required this.reports});

  @override
  Widget build(BuildContext context) {
    return Column(
      children:
          reports
              .map(
                (r) => Padding(
                  padding: const EdgeInsets.only(bottom: 10),
                  child: Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(14),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.04),
                          blurRadius: 12,
                          offset: const Offset(0, 8),
                        ),
                      ],
                    ),
                    child: Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: r.color.withOpacity(0.12),
                            borderRadius: BorderRadius.circular(12),
                          ),
                          child: Icon(
                            Icons.description_rounded,
                            color: r.color,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                r.title,
                                style: const TextStyle(
                                  color: AppColors.ink,
                                  fontWeight: FontWeight.w900,
                                  fontSize: 16,
                                ),
                              ),
                              const SizedBox(height: 4),
                              Text(
                                r.desc,
                                style: TextStyle(
                                  color: AppColors.ink.withOpacity(0.65),
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 6),
                              Text(
                                r.size,
                                style: TextStyle(
                                  color: AppColors.ink.withOpacity(0.55),
                                  fontWeight: FontWeight.w600,
                                  fontSize: 12.5,
                                ),
                              ),
                            ],
                          ),
                        ),
                        Column(
                          children: [
                            OutlinedButton.icon(
                              onPressed: () {},
                              style: OutlinedButton.styleFrom(
                                foregroundColor: r.color,
                                side: BorderSide(
                                  color: r.color.withOpacity(0.4),
                                ),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
                                ),
                              ),
                              icon: const Icon(
                                Icons.download_rounded,
                                size: 18,
                              ),
                              label: const Text('Tải'),
                            ),
                            const SizedBox(height: 6),
                            TextButton(
                              onPressed: () {},
                              child: const Text('Gửi email'),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),
              )
              .toList(),
    );
  }
}

class _DeliverySummary extends StatelessWidget {
  const _DeliverySummary();

  @override
  Widget build(BuildContext context) {
    final rows = const [
      _SummaryRow(label: 'Tổng suất giao', value: '1.240'),
      _SummaryRow(label: 'Số ca đúng giờ', value: '96%'),
      _SummaryRow(label: 'Suất hoàn tiền', value: '4'),
      _SummaryRow(label: 'Điểm trung bình', value: '4.7/5'),
    ];

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.assessment_outlined, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Tóm tắt giao nhận',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          ...rows.map(
            (r) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 6),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      r.label,
                      style: TextStyle(
                        color: AppColors.ink.withOpacity(0.7),
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  Text(
                    r.value,
                    style: const TextStyle(
                      color: AppColors.ink,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _SummaryRow {
  final String label;
  final String value;

  const _SummaryRow({required this.label, required this.value});
}

class _NoteCard extends StatelessWidget {
  const _NoteCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFEEF4FF),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.org.withOpacity(0.16)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: const BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
            ),
            child: const Icon(Icons.info_outline, color: AppColors.org),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Ghi chú',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w900,
                    color: AppColors.org,
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  'Báo cáo được cập nhật tự động lúc 18:00 mỗi ngày. Nếu cần ký xác nhận, vui lòng phản hồi trong vòng 24h.',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.75),
                    fontWeight: FontWeight.w600,
                    height: 1.3,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
