import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../widgets/meal_cards.dart';

class DashboardTab extends StatefulWidget {
  final double bottomInset;

  const DashboardTab({
    super.key,
    required this.bottomInset,
  });

  @override
  State<DashboardTab> createState() => _DashboardTabState();
}

class _DashboardTabState extends State<DashboardTab> {
  final ScrollController _scrollController = ScrollController();
  final PageController _heroController =
      PageController(viewportFraction: 0.9, keepPage: true);
  int _heroIndex = 0;

  @override
  void dispose() {
    _heroController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const userName = 'Nguyễn Văn A';
    return Container(
      decoration: const BoxDecoration(
        gradient: LinearGradient(
          colors: [Color(0xFFF7F9FB), Color(0xFFF0F3F7)],
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
        ),
      ),
      child: SafeArea(
        child: CustomScrollView(
          controller: _scrollController,
          primary: false,
          physics: const BouncingScrollPhysics(),
          slivers: [
            SliverPadding(
              padding:
                  EdgeInsets.fromLTRB(20, 16, 20, 20 + widget.bottomInset),
              sliver: SliverList(
                delegate: SliverChildListDelegate(
                  [
                    _TopBar(userName: userName),
                    const SizedBox(height: 12),
                    _HeroCarousel(
                      controller: _heroController,
                      currentIndex: _heroIndex,
                      onPageChanged: (i) => setState(() => _heroIndex = i),
                      onTap: (route) =>
                          Navigator.of(context).pushNamed(route),
                    ),
                    const SizedBox(height: 18),
                    const _SectionHeader(
                      title: 'Tiện ích nhanh',
                      subtitle: 'Những thao tác bạn dùng nhiều nhất',
                    ),
                    const SizedBox(height: 12),
                    Wrap(
                      spacing: 12,
                      runSpacing: 12,
                      children: _homeActions
                          .map(
                            (action) => _HomeActionCard(
                              data: action,
                              onTap: () =>
                                  Navigator.of(context).pushNamed(action.route),
                            ),
                          )
                          .toList(),
                    ),
                    const SizedBox(height: 18),
                    const _StatusChips(),
                    const SizedBox(height: 20),
                    _SectionHeader(
                      title: 'Gợi ý hôm nay',
                      subtitle: 'Chọn nhanh món hợp khẩu vị',
                      actionLabel: 'Xem tất cả',
                      onActionTap: () =>
                          Navigator.of(context).pushNamed(AppRoutes.customerMenu),
                    ),
                    const SizedBox(height: 12),
                    ...suggestedMeals.take(2).map(
                      (meal) => Padding(
                        padding: const EdgeInsets.only(bottom: 12),
                        child: MiniMealCard(meal: meal),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _TopBar extends StatelessWidget {
  final String userName;

  const _TopBar({
    required this.userName,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Xin chào,',
          style: Theme.of(context).textTheme.labelLarge?.copyWith(
                color: AppColors.ink.withOpacity(0.65),
              ),
        ),
        const SizedBox(height: 4),
        Text(
          userName,
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                color: AppColors.ink,
                fontWeight: FontWeight.w800,
                letterSpacing: -0.2,
              ),
        ),
        const SizedBox(height: 6),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
          decoration: BoxDecoration(
            color: AppColors.customer.withOpacity(0.1),
            borderRadius: BorderRadius.circular(999),
            border: Border.all(
              color: AppColors.customer.withOpacity(0.2),
            ),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: const [
              Icon(Icons.verified_user_outlined,
                  size: 14, color: AppColors.customer),
              SizedBox(width: 6),
              Text(
                'Khách hàng cá nhân',
                style: TextStyle(
                  color: AppColors.customer,
                  fontWeight: FontWeight.w700,
                  fontSize: 12,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _HeroSlide {
  final String title;
  final String subtitle;
  final IconData icon;
  final List<Color> gradient;
  final String route;

  const _HeroSlide({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.gradient,
    required this.route,
  });
}

const _heroSlides = [
  _HeroSlide(
    title: 'Đặt bữa nhanh',
    subtitle: 'Còn 25 phút để chốt đơn hôm nay.',
    icon: Icons.bento_rounded,
    gradient: [AppColors.customer, AppColors.customerAlt],
    route: AppRoutes.customerOrder,
  ),
  _HeroSlide(
    title: 'Ưu đãi hôm nay',
    subtitle: 'Nhận mã giảm đến 25% cho món mới.',
    icon: Icons.local_offer_rounded,
    gradient: [Color(0xFF1F3C88), Color(0xFF5C7DC7)],
    route: AppRoutes.customerMenu,
  ),
  _HeroSlide(
    title: 'Thanh toán nhanh',
    subtitle: 'Ví, ngân hàng và hóa đơn một chạm.',
    icon: Icons.credit_card_rounded,
    gradient: [Color(0xFFF1AE62), Color(0xFFE07A24)],
    route: AppRoutes.customerPayment,
  ),
];

class _HeroCarousel extends StatelessWidget {
  final PageController controller;
  final int currentIndex;
  final ValueChanged<int> onPageChanged;
  final ValueChanged<String> onTap;

  const _HeroCarousel({
    required this.controller,
    required this.currentIndex,
    required this.onPageChanged,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        SizedBox(
          height: 150,
          child: PageView.builder(
            controller: controller,
            itemCount: _heroSlides.length,
            onPageChanged: onPageChanged,
            padEnds: false,
            itemBuilder: (context, index) {
              final slide = _heroSlides[index];
              return Padding(
                padding: EdgeInsets.only(right: index == _heroSlides.length - 1 ? 0 : 10),
                child: _PrimaryBanner(
                  onTap: () => onTap(slide.route),
                  title: slide.title,
                  subtitle: slide.subtitle,
                  icon: slide.icon,
                  gradient: slide.gradient,
                ),
              );
            },
          ),
        ),
        const SizedBox(height: 10),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(
            _heroSlides.length,
            (i) => AnimatedContainer(
              duration: const Duration(milliseconds: 200),
              margin: const EdgeInsets.symmetric(horizontal: 4),
              height: 6,
              width: currentIndex == i ? 22 : 8,
              decoration: BoxDecoration(
                color: currentIndex == i
                    ? AppColors.customer
                    : AppColors.ink.withOpacity(0.2),
                borderRadius: BorderRadius.circular(999),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _PrimaryBanner extends StatelessWidget {
  final VoidCallback onTap;
  final String title;
  final String subtitle;
  final IconData icon;
  final List<Color> gradient;

  const _PrimaryBanner({
    required this.onTap,
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.gradient,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          gradient: LinearGradient(
            colors: gradient,
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
          ),
          borderRadius: BorderRadius.circular(18),
          boxShadow: [
            BoxShadow(
              color: gradient.first.withOpacity(0.25),
              blurRadius: 18,
              offset: const Offset(0, 10),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.18),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(icon, color: Colors.white, size: 22),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.w800,
                      fontSize: 16,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 13,
                      height: 1.4,
                    ),
                  ),
                ],
              ),
            ),
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.18),
                shape: BoxShape.circle,
              ),
              child: const Icon(Icons.arrow_forward_ios_rounded,
                  color: Colors.white, size: 14),
            ),
          ],
        ),
      ),
    );
  }
}

class _HomeAction {
  final IconData icon;
  final String title;
  final String subtitle;
  final Color color;
  final String route;

  const _HomeAction({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.color,
    required this.route,
  });
}

const _homeActions = [
  _HomeAction(
    icon: Icons.menu_book_rounded,
    title: 'Thực đơn',
    subtitle: 'Món hôm nay',
    color: AppColors.customer,
    route: AppRoutes.customerMenu,
  ),
  _HomeAction(
    icon: Icons.shopping_bag_rounded,
    title: 'Đặt nhanh',
    subtitle: 'Chốt bữa trưa',
    color: Color(0xFF1F3C88),
    route: AppRoutes.customerOrder,
  ),
  _HomeAction(
    icon: Icons.local_offer_outlined,
    title: 'Ưu đãi',
    subtitle: 'Mã giảm giá',
    color: Color(0xFFF1AE62),
    route: AppRoutes.customerMenu,
  ),
  _HomeAction(
    icon: Icons.payments_rounded,
    title: 'Thanh toán',
    subtitle: 'Ví & hóa đơn',
    color: AppColors.customerAlt,
    route: AppRoutes.customerPayment,
  ),
];

class _HomeActionCard extends StatelessWidget {
  final _HomeAction data;
  final VoidCallback onTap;

  const _HomeActionCard({
    required this.data,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final cardWidth = (MediaQuery.of(context).size.width - 20 * 2 - 12) / 2;
        return SizedBox(
          width: cardWidth,
          child: Material(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            child: InkWell(
              onTap: onTap,
              borderRadius: BorderRadius.circular(16),
              child: Padding(
                padding: const EdgeInsets.all(14),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Container(
                      width: 42,
                      height: 42,
                      decoration: BoxDecoration(
                        color: data.color.withOpacity(0.12),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Icon(
                        data.icon,
                        color: data.color,
                        size: 22,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Text(
                      data.title,
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.w800,
                            color: AppColors.ink,
                            letterSpacing: -0.1,
                          ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      data.subtitle,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color: AppColors.ink.withOpacity(0.65),
                          ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}

class _StatusChips extends StatelessWidget {
  const _StatusChips();

  @override
  Widget build(BuildContext context) {
    final chips = [
      const _StatusChip(
        icon: Icons.timelapse_rounded,
        label: 'Còn 25 phút để đặt bữa trưa',
        color: AppColors.customer,
      ),
      const _StatusChip(
        icon: Icons.redeem_rounded,
        label: 'Điểm thưởng: 120',
        color: Color(0xFF1F3C88),
      ),
      const _StatusChip(
        icon: Icons.location_on_outlined,
        label: 'Địa điểm: Văn phòng Q1',
        color: Color(0xFFF1AE62),
      ),
    ];

    return Wrap(
      spacing: 10,
      runSpacing: 10,
      children: chips,
    );
  }
}

class _StatusChip extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;

  const _StatusChip({
    required this.icon,
    required this.label,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color: color.withOpacity(0.16),
          width: 1.2,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.03),
            blurRadius: 12,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: color.withOpacity(0.12),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(
              icon,
              size: 16,
              color: color,
            ),
          ),
          const SizedBox(width: 10),
          Text(
            label,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.85),
              fontWeight: FontWeight.w700,
              fontSize: 13,
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  final String title;
  final String subtitle;
  final String? actionLabel;
  final VoidCallback? onActionTap;

  const _SectionHeader({
    required this.title,
    required this.subtitle,
    this.actionLabel,
    this.onActionTap,
  });

  @override
  Widget build(BuildContext context) {
    final label = actionLabel;
    final action = onActionTap;

    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                title,
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.w900,
                      color: AppColors.ink,
                      letterSpacing: -0.4,
                    ),
              ),
              const SizedBox(height: 4),
              Text(
                subtitle,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Colors.grey.shade600,
                    ),
              ),
            ],
          ),
        ),
        if (label != null && action != null)
          TextButton(
            onPressed: action,
            style: TextButton.styleFrom(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              foregroundColor: AppColors.customer,
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  label,
                  style: const TextStyle(
                    fontWeight: FontWeight.w700,
                  ),
                ),
                const SizedBox(width: 6),
                const Icon(Icons.arrow_forward_rounded, size: 16),
              ],
            ),
          ),
      ],
    );
  }
}
