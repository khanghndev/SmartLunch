import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';

class HelpCenterPage extends StatefulWidget {
  const HelpCenterPage({super.key});

  @override
  State<HelpCenterPage> createState() => _HelpCenterPageState();
}

class _HelpCenterPageState extends State<HelpCenterPage> {
  String _searchQuery = '';

  @override
  Widget build(BuildContext context) {
    final faqs =
        [
          const _Faq(
            title: 'Làm sao để đổi phương thức thanh toán?',
            answer:
                'Vào mục Thanh toán trong hồ sơ và chọn thẻ/ví mặc định mới.',
            category: 'Thanh toán',
          ),
          const _Faq(
            title: 'Chính sách hoàn tiền khi hủy đơn?',
            answer:
                'Đơn hủy trước giờ chế biến sẽ được hoàn tiền trong 1-3 ngày làm việc.',
            category: 'Đơn hàng',
          ),
          const _Faq(
            title: 'Cách cập nhật địa chỉ giao hàng mặc định?',
            answer: 'Vào Sổ địa chỉ, chọn địa chỉ và đặt làm mặc định.',
            category: 'Tài khoản',
          ),
          const _Faq(
            title: 'Tại sao tôi không nhận được thông báo?',
            answer:
                'Kiểm tra phần Cài đặt thông báo và quyền thông báo của thiết bị.',
            category: 'Thông báo',
          ),
        ].where((faq) {
          if (_searchQuery.trim().isEmpty) return true;
          return faq.title.toLowerCase().contains(_searchQuery.toLowerCase());
        }).toList();

    return Scaffold(
      backgroundColor: AppColors.surface,
      appBar: AppBar(title: const Text('Trung tâm trợ giúp')),
      body: ListView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 10, 20, 24),
        children: [
          TextField(
            onChanged: (value) => setState(() => _searchQuery = value),
            decoration: InputDecoration(
              hintText: 'Tìm câu hỏi, ví dụ: thanh toán, hoàn tiền...',
              prefixIcon: const Icon(Icons.search_rounded),
              suffixIcon:
                  _searchQuery.isEmpty
                      ? null
                      : IconButton(
                        icon: const Icon(Icons.close_rounded),
                        onPressed: () => setState(() => _searchQuery = ''),
                      ),
            ),
          ),
          const SizedBox(height: 14),
          const _QuickHelpGrid(),
          const SizedBox(height: 14),
          _SectionTitle(
            title: 'Câu hỏi thường gặp',
            trailing: '${faqs.length} mục',
          ),
          const SizedBox(height: 8),
          ...faqs.map((faq) => _FaqTile(faq: faq)),
          const SizedBox(height: 14),
          _ContactSupportCard(),
        ],
      ),
    );
  }
}

class _SectionTitle extends StatelessWidget {
  final String title;
  final String trailing;

  const _SectionTitle({required this.title, required this.trailing});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Text(
          title,
          style: Theme.of(
            context,
          ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w800),
        ),
        const Spacer(),
        Text(
          trailing,
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
            color: AppColors.inkSoft,
            fontWeight: FontWeight.w700,
          ),
        ),
      ],
    );
  }
}

class _QuickHelpGrid extends StatelessWidget {
  const _QuickHelpGrid();

  @override
  Widget build(BuildContext context) {
    final cards = const [
      _QuickCard(
        icon: Icons.article_outlined,
        label: 'Hướng dẫn sử dụng',
        color: AppColors.customer,
      ),
      _QuickCard(
        icon: Icons.history_edu_outlined,
        label: 'Theo dõi yêu cầu',
        color: AppColors.courier,
      ),
      _QuickCard(
        icon: Icons.receipt_long_outlined,
        label: 'Tra cứu hóa đơn',
        color: AppColors.org,
      ),
      _QuickCard(
        icon: Icons.call_rounded,
        label: 'Hotline 24/7',
        color: AppColors.info,
      ),
    ];

    return GridView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: cards.length,
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        crossAxisSpacing: 10,
        mainAxisSpacing: 10,
        childAspectRatio: 1.65,
      ),
      itemBuilder: (_, index) => cards[index],
    );
  }
}

class _QuickCard extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;

  const _QuickCard({
    required this.icon,
    required this.label,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(14),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: () {},
        child: Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: AppColors.border),
          ),
          child: Row(
            children: [
              Container(
                width: 38,
                height: 38,
                decoration: BoxDecoration(
                  color: color.withOpacity(0.12),
                  borderRadius: BorderRadius.circular(11),
                ),
                child: Icon(icon, color: color),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  label,
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w700,
                    color: AppColors.ink,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _FaqTile extends StatelessWidget {
  final _Faq faq;

  const _FaqTile({required this.faq});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: ExpansionTile(
        tilePadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 2),
        childrenPadding: const EdgeInsets.fromLTRB(14, 0, 14, 14),
        leading: Container(
          width: 32,
          height: 32,
          decoration: BoxDecoration(
            color: AppColors.tint(AppColors.customer, 0.12),
            borderRadius: BorderRadius.circular(10),
          ),
          child: const Icon(
            Icons.help_outline_rounded,
            color: AppColors.customer,
            size: 18,
          ),
        ),
        title: Text(
          faq.title,
          style: Theme.of(
            context,
          ).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w700),
        ),
        subtitle: Text(
          faq.category,
          style: Theme.of(context).textTheme.labelSmall?.copyWith(
            color: AppColors.inkSoft,
            fontWeight: FontWeight.w700,
          ),
        ),
        children: [
          Align(
            alignment: Alignment.centerLeft,
            child: Text(
              faq.answer,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: AppColors.inkSoft,
                height: 1.45,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _ContactSupportCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [Color(0xFFEDF6FF), Color(0xFFF8FCFF)],
        ),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.info.withOpacity(0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Cần hỗ trợ trực tiếp?',
            style: Theme.of(
              context,
            ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w800),
          ),
          const SizedBox(height: 6),
          Text(
            'Đội chăm sóc khách hàng hoạt động 07:00 - 22:00 mỗi ngày.',
            style: Theme.of(
              context,
            ).textTheme.bodyMedium?.copyWith(color: AppColors.inkSoft),
          ),
          const SizedBox(height: 10),
          Row(
            children: [
              Expanded(
                child: FilledButton.icon(
                  onPressed: () {},
                  icon: const Icon(Icons.call_rounded),
                  label: const Text('Gọi hotline'),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.info,
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: () {},
                  icon: const Icon(Icons.chat_bubble_outline_rounded),
                  label: const Text('Chat trực tuyến'),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppColors.info,
                    side: BorderSide(color: AppColors.info.withOpacity(0.35)),
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _Faq {
  final String title;
  final String answer;
  final String category;

  const _Faq({
    required this.title,
    required this.answer,
    required this.category,
  });
}
