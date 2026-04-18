import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class StaffPage extends StatelessWidget {
  const StaffPage({super.key});

  @override
  Widget build(BuildContext context) {
    final staff = const [
      _Staff(
        name: 'Nguyễn Minh Anh',
        email: 'anh.nguyen@acme.com',
        dept: 'Kế toán',
        shift: 'Ca trưa',
        status: StaffStatus.active,
      ),
      _Staff(
        name: 'Trần Hữu Khang',
        email: 'khang.tran@acme.com',
        dept: 'IT',
        shift: 'Ca trưa',
        status: StaffStatus.active,
      ),
      _Staff(
        name: 'Lê Thu Hà',
        email: 'ha.le@acme.com',
        dept: 'CSKH',
        shift: 'Ca sáng',
        status: StaffStatus.invited,
      ),
      _Staff(
        name: 'Phạm Bảo Long',
        email: 'long.pham@acme.com',
        dept: 'Vận hành',
        shift: 'Ca tối',
        status: StaffStatus.paused,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Danh sách nhân viên'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.upload_file)),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.org,
        foregroundColor: Colors.white,
        onPressed: () {},
        icon: const Icon(Icons.person_add_alt_1_rounded),
        label: const Text('Thêm nhân viên'),
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            const _HeaderSummary(),
            const SizedBox(height: 14),
            const _SearchAndFilter(),
            const SizedBox(height: 14),
            _StaffList(staff: staff),
          ],
        ),
      ),
    );
  }
}

class _HeaderSummary extends StatelessWidget {
  const _HeaderSummary();

  @override
  Widget build(BuildContext context) {
    final cards = [
      _SummaryCard(
        label: 'Tổng nhân viên',
        value: '128',
        icon: Icons.groups_rounded,
        color: AppColors.org,
      ),
      _SummaryCard(
        label: 'Đang hoạt động',
        value: '120',
        icon: Icons.verified_user_rounded,
        color: AppColors.org,
      ),
      _SummaryCard(
        label: 'Mời tham gia',
        value: '8',
        icon: Icons.mark_email_read_rounded,
        color: AppColors.orgAlt,
      ),
    ];

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.org, AppColors.orgAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.org.withOpacity(0.2),
            blurRadius: 16,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.18),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.groups, color: Colors.white),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  'Quản lý danh sách',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                  ),
                ),
              ),
              TextButton(
                onPressed: () {},
                style: TextButton.styleFrom(foregroundColor: Colors.white),
                child: const Text('Xuất CSV'),
              ),
            ],
          ),
          const SizedBox(height: 12),
          SizedBox(
            height: 100,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              itemBuilder: (context, i) => cards[i],
              separatorBuilder: (_, __) => const SizedBox(width: 10),
              itemCount: cards.length,
            ),
          ),
        ],
      ),
    );
  }
}

class _SummaryCard extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final Color color;

  const _SummaryCard({
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 180,
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.12),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.white.withOpacity(0.2)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.16),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: Colors.white),
          ),
          const SizedBox(width: 10),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                label,
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
                  fontSize: 12.5,
                ),
              ),
              Text(
                value,
                style: TextStyle(
                  color: color == Colors.white ? AppColors.ink : Colors.white,
                  fontWeight: FontWeight.w900,
                  fontSize: 16,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _SearchAndFilter extends StatelessWidget {
  const _SearchAndFilter();

  @override
  Widget build(BuildContext context) {
    final chips = ['Tất cả', 'Kế toán', 'IT', 'CSKH', 'Vận hành'];
    return Container(
      padding: const EdgeInsets.all(12),
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
        children: [
          TextField(
            decoration: InputDecoration(
              hintText: 'Tìm tên hoặc email',
              prefixIcon: Icon(
                Icons.search,
                color: AppColors.ink.withOpacity(0.65),
              ),
              filled: true,
              fillColor: const Color(0xFFF8F9FB),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: AppColors.ink.withOpacity(0.08)),
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: const BorderSide(color: AppColors.org, width: 1.4),
              ),
            ),
          ),
          const SizedBox(height: 10),
          SizedBox(
            height: 40,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              itemBuilder:
                  (context, i) => FilterChip(
                    label: Text(chips[i]),
                    selected: i == 0,
                    onSelected: (_) {},
                    selectedColor: AppColors.org.withOpacity(0.12),
                    checkmarkColor: AppColors.org,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                      side: BorderSide(
                        color:
                            i == 0
                                ? AppColors.org.withOpacity(0.45)
                                : AppColors.ink.withOpacity(0.08),
                      ),
                    ),
                    labelStyle: TextStyle(
                      color:
                          i == 0
                              ? AppColors.org
                              : AppColors.ink.withOpacity(0.75),
                      fontWeight: FontWeight.w700,
                    ),
                  ),
              separatorBuilder: (_, __) => const SizedBox(width: 8),
              itemCount: chips.length,
            ),
          ),
        ],
      ),
    );
  }
}

enum StaffStatus { active, invited, paused }

class _Staff {
  final String name;
  final String email;
  final String dept;
  final String shift;
  final StaffStatus status;

  const _Staff({
    required this.name,
    required this.email,
    required this.dept,
    required this.shift,
    required this.status,
  });
}

class _StaffList extends StatelessWidget {
  final List<_Staff> staff;

  const _StaffList({required this.staff});

  @override
  Widget build(BuildContext context) {
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
              const Icon(Icons.list_alt_rounded, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Danh sách',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Chọn nhiều')),
            ],
          ),
          const SizedBox(height: 8),
          ...staff.map(
            (s) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: _StaffTile(staff: s),
            ),
          ),
        ],
      ),
    );
  }
}

class _StaffTile extends StatelessWidget {
  final _Staff staff;

  const _StaffTile({required this.staff});

  Color _statusColor() {
    switch (staff.status) {
      case StaffStatus.active:
        return AppColors.org;
      case StaffStatus.invited:
        return AppColors.orgAlt;
      case StaffStatus.paused:
        return AppColors.org;
    }
  }

  String _statusLabel() {
    switch (staff.status) {
      case StaffStatus.active:
        return 'Đang dùng';
      case StaffStatus.invited:
        return 'Đã mời';
      case StaffStatus.paused:
        return 'Tạm dừng';
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _statusColor();
    return InkWell(
      borderRadius: BorderRadius.circular(12),
      onTap: () {},
      child: Row(
        children: [
          CircleAvatar(
            radius: 24,
            backgroundColor: AppColors.org.withOpacity(0.1),
            child: Text(
              staff.name.isNotEmpty ? staff.name[0] : '?',
              style: TextStyle(
                color: AppColors.org,
                fontWeight: FontWeight.w800,
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  staff.name,
                  style: const TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w900,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  staff.email,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 4),
                Wrap(
                  spacing: 6,
                  runSpacing: 6,
                  children: [_Tag(text: staff.dept), _Tag(text: staff.shift)],
                ),
              ],
            ),
          ),
          const SizedBox(width: 10),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
            decoration: BoxDecoration(
              color: color.withOpacity(0.14),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Text(
              _statusLabel(),
              style: TextStyle(
                color: color,
                fontWeight: FontWeight.w800,
                fontSize: 12.5,
              ),
            ),
          ),
          IconButton(onPressed: () {}, icon: const Icon(Icons.more_vert)),
        ],
      ),
    );
  }
}

class _Tag extends StatelessWidget {
  final String text;

  const _Tag({required this.text});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: AppColors.org.withOpacity(0.08),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Text(
        text,
        style: TextStyle(
          color: AppColors.ink.withOpacity(0.75),
          fontWeight: FontWeight.w700,
          fontSize: 12.5,
        ),
      ),
    );
  }
}
