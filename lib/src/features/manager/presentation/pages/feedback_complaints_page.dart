import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/review_complaint_models.dart';
import '../../data/repositories/manager_repository.dart';
import '../widgets/manager_ui.dart';

class FeedbackComplaintsPage extends StatefulWidget {
  const FeedbackComplaintsPage({super.key});

  @override
  State<FeedbackComplaintsPage> createState() => _FeedbackComplaintsPageState();
}

class _FeedbackComplaintsPageState extends State<FeedbackComplaintsPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabs;
  final _searchCtrl = TextEditingController();

  List<ReviewModel> _reviews = [];
  List<ComplaintModel> _complaints = [];
  double _avgRating = 0;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _tabs = TabController(length: 2, vsync: this);
    _load();
    _searchCtrl.addListener(() {
      if (_tabs.index == 0) _load();
    });
  }

  @override
  void dispose() {
    _tabs.dispose();
    _searchCtrl.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final reviews = await ManagerRepository.instance.getReviews(
        searchTerm: _searchCtrl.text.trim().isEmpty ? null : _searchCtrl.text.trim(),
      );
      final complaints = await ManagerRepository.instance.getComplaints();
      if (!mounted) return;
      setState(() {
        _reviews = reviews.items;
        _avgRating = reviews.averageRating;
        _complaints = complaints.items;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = apiErrorMessage(e);
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return ManagerPageShell(
      title: 'Phản hồi & Khiếu nại',
      onRefresh: _load,
      body: _loading
          ? const ManagerLoadingBody()
          : _error != null
              ? ManagerErrorBody(message: _error!, onRetry: _load)
              : Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Padding(
                      padding: EdgeInsets.fromLTRB(16, 12, 16, 0),
                      child: ManagerPageIntro(
                        title: 'Chất lượng dịch vụ',
                        description:
                            'Theo dõi đánh giá suất ăn và xử lý khiếu nại từ khách hàng / đơn vị.',
                        icon: Icons.feedback_rounded,
                      ),
                    ),
                    ManagerSearchField(
                      controller: _searchCtrl,
                      hint: 'Tìm theo tên khách hoặc nội dung đánh giá…',
                      onSubmitted: _load,
                    ),
                    ManagerTabBar(
                      controller: _tabs,
                      tabs: [
                        'Đánh giá (${_reviews.length})',
                        'Khiếu nại (${_complaints.length})',
                      ],
                    ),
                    Expanded(
                      child: TabBarView(
                        controller: _tabs,
                        children: [
                          _reviewsList(),
                          _complaintsList(),
                        ],
                      ),
                    ),
                  ],
                ),
    );
  }

  Widget _reviewsList() {
    return ListView(
      padding: managerListPadding(context).copyWith(top: 12),
      children: [
        ManagerStatTile(
          label: 'Điểm trung bình',
          value: _avgRating.toStringAsFixed(1),
          icon: Icons.star_rounded,
          color: managerAccent,
        ),
        const SizedBox(height: 12),
        if (_reviews.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList(message: 'Chưa có đánh giá'))
        else
          ..._reviews.map(_reviewTile),
      ],
    );
  }

  Widget _reviewTile(ReviewModel r) {
    return ManagerDataRow(
      icon: Icons.person_rounded,
      iconColor: managerAccent,
      title: r.customerName.isNotEmpty ? r.customerName : 'Khách hàng',
      subtitle: [
        if (r.dishName.isNotEmpty) r.dishName,
        if (r.comment.isNotEmpty) r.comment,
        formatShortDate(r.createdAt),
      ].where((s) => s.isNotEmpty).join(' · '),
      badge: Row(
        mainAxisSize: MainAxisSize.min,
        children: List.generate(
          5,
          (i) => Icon(
            i < r.stars ? Icons.star_rounded : Icons.star_border_rounded,
            size: 14,
            color: managerAccent,
          ),
        ),
      ),
    );
  }

  Widget _complaintsList() {
    return ListView(
      padding: managerListPadding(context).copyWith(top: 12),
      children: [
        if (_complaints.isEmpty)
          const ManagerGlassCard(child: ManagerEmptyList(message: 'Chưa có khiếu nại'))
        else
          ..._complaints.map(_complaintTile),
      ],
    );
  }

  Widget _complaintTile(ComplaintModel c) {
    final statusColor = switch (c.status) {
      ComplaintStatus.resolved => AppDesignSystem.success,
      ComplaintStatus.processing => AppDesignSystem.info,
      _ => AppDesignSystem.warning,
    };
    final subtitle = [
      if (c.organizationName.isNotEmpty) c.organizationName,
      if (c.description.isNotEmpty) c.description,
      formatShortDate(c.createdAt),
    ].where((s) => s.isNotEmpty).join('\n');

    return ManagerDataRow(
      icon: Icons.report_problem_outlined,
      iconColor: statusColor,
      title: c.title,
      subtitle: subtitle,
      badge: ManagerStatusBadge(label: c.status.label, color: statusColor),
    );
  }
}
