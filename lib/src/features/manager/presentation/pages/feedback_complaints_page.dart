import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';
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
                  children: [
                    Padding(
                      padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
                      child: TextField(
                        controller: _searchCtrl,
                        decoration: InputDecoration(
                          hintText: 'Tìm đánh giá...',
                          prefixIcon: const Icon(Icons.search_rounded),
                          filled: true,
                          fillColor: Colors.white,
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(14),
                            borderSide: BorderSide(color: AppColors.border),
                          ),
                          enabledBorder: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(14),
                            borderSide: BorderSide(color: AppColors.border),
                          ),
                        ),
                        onSubmitted: (_) => _load(),
                      ),
                    ),
                    Material(
                      color: Colors.transparent,
                      child: TabBar(
                        controller: _tabs,
                        labelColor: managerAccent,
                        indicatorColor: managerAccent,
                        tabs: [
                          Tab(text: 'Đánh giá (${_reviews.length})'),
                          Tab(text: 'Khiếu nại (${_complaints.length})'),
                        ],
                      ),
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
      padding: const EdgeInsets.all(16),
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
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: ManagerGlassCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  backgroundColor: managerAccent.withValues(alpha: 0.15),
                  child: Text(
                    r.customerName.isNotEmpty ? r.customerName[0].toUpperCase() : '?',
                    style: TextStyle(color: managerAccent, fontWeight: FontWeight.bold),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(r.customerName, style: const TextStyle(fontWeight: FontWeight.w700)),
                      if (r.dishName.isNotEmpty)
                        Text(
                          r.dishName,
                          style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                        ),
                    ],
                  ),
                ),
                Row(
                  children: List.generate(
                    5,
                    (i) => Icon(
                      i < r.stars ? Icons.star_rounded : Icons.star_border_rounded,
                      size: 16,
                      color: managerAccent,
                    ),
                  ),
                ),
              ],
            ),
            if (r.comment.isNotEmpty) ...[
              const SizedBox(height: 8),
              Text(r.comment, style: TextStyle(color: Colors.grey.shade800)),
            ],
            const SizedBox(height: 6),
            Text(
              formatShortDate(r.createdAt),
              style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
            ),
          ],
        ),
      ),
    );
  }

  Widget _complaintsList() {
    return ListView(
      padding: const EdgeInsets.all(16),
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
      ComplaintStatus.resolved => AppColors.success,
      ComplaintStatus.processing => AppColors.info,
      _ => AppColors.warning,
    };
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: ManagerGlassCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(c.title, style: const TextStyle(fontWeight: FontWeight.w700)),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: statusColor.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    c.status.label,
                    style: TextStyle(color: statusColor, fontSize: 11, fontWeight: FontWeight.w600),
                  ),
                ),
              ],
            ),
            if (c.organizationName.isNotEmpty)
              Padding(
                padding: const EdgeInsets.only(top: 4),
                child: Text(
                  c.organizationName,
                  style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                ),
              ),
            if (c.description.isNotEmpty) ...[
              const SizedBox(height: 6),
              Text(
                c.description,
                maxLines: 3,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(color: Colors.grey.shade700),
              ),
            ],
            const SizedBox(height: 6),
            Text(
              formatShortDate(c.createdAt),
              style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
            ),
          ],
        ),
      ),
    );
  }
}
