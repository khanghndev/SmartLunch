import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../core/constants/app_colors.dart';
import '../core/widgets/staggered_reveal.dart';
import 'app_routes.dart';

class WelcomePage extends StatefulWidget {
  const WelcomePage({super.key});

  @override
  State<WelcomePage> createState() => _WelcomePageState();
}

class _WelcomePageState extends State<WelcomePage> {
  @override
  void initState() {
    super.initState();
    SystemChrome.setSystemUIOverlayStyle(
      const SystemUiOverlayStyle(
        statusBarColor: Colors.transparent,
        statusBarIconBrightness: Brightness.dark,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final roles = [
      _RoleOption(
        title: 'Nhân viên giao hàng',
        subtitle: 'Nhận đơn, giao bữa ăn và cập nhật trạng thái giao.',
        icon: Icons.local_shipping_rounded,
        accent: AppColors.courier,
        gradient: [AppColors.courier, AppColors.courierAlt],
        route: AppRoutes.loginCourier,
      ),
      _RoleOption(
        title: 'Khách hàng (người ăn)',
        subtitle: 'Xem danh sách thực đơn.',
        icon: Icons.restaurant_menu_rounded,
        accent: AppColors.customer,
        gradient: [AppColors.customer, AppColors.customerAlt],
        route: AppRoutes.loginCustomer,
      ),
      _RoleOption(
        title: 'Đơn vị đặt suất ăn',
        subtitle:
            '• Tiến hành đặt cơm theo ngày (trước...\n• Tiến hành đặt cọc, thanh toán.\n• Đánh giá suất ăn.',
        icon: Icons.apartment_rounded,
        accent: AppColors.org,
        gradient: [AppColors.org, AppColors.orgAlt],
        route: AppRoutes.loginOrg,
      ),
    ];

    return Scaffold(
      body: Stack(
        children: [
          // Light, quiet background
          Container(
            decoration: const BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: [Color(0xFFF9FAFB), Color(0xFFEFF1F5)],
              ),
            ),
          ),
          const Positioned(
            top: -60,
            right: -30,
            child: _BlurOrb(color: Color(0x2222A75A), size: 200),
          ),
          const Positioned(
            bottom: -80,
            left: -40,
            child: _BlurOrb(color: Color(0x223BCB7A), size: 190),
          ),

          SafeArea(
            child: SingleChildScrollView(
              padding: const EdgeInsets.fromLTRB(20, 28, 20, 32),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  // Brand chip
                  StaggeredReveal(
                    delayMs: 0,
                    child: Align(
                      alignment: Alignment.centerLeft,
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 14,
                          vertical: 9,
                        ),
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: AppColors.ink.withOpacity(0.07),
                          ),
                          boxShadow: [
                            BoxShadow(
                              color: Colors.black.withOpacity(0.025),
                              blurRadius: 14,
                              offset: const Offset(0, 6),
                            ),
                          ],
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Flexible(
                              child: SizedBox(
                                height: 36,
                                child: Image.asset(
                                  'assets/images/sv_logo_dashboard.png',
                                  fit: BoxFit.contain,
                                ),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Text(
                                'Chọn vai trò để tiếp tục',
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                                style: Theme.of(
                                  context,
                                ).textTheme.bodySmall?.copyWith(
                                  color: AppColors.ink.withOpacity(0.6),
                                  height: 1.2,
                                  fontSize: 12,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 20),

                  // Main heading
                  StaggeredReveal(
                    delayMs: 150,
                    child: Text(
                      'Bạn thuộc nhóm nào?',
                      textAlign: TextAlign.center,
                      style: Theme.of(
                        context,
                      ).textTheme.headlineSmall?.copyWith(
                        color: AppColors.ink,
                        fontWeight: FontWeight.w800,
                        height: 1.2,
                        letterSpacing: -0.2,
                        fontSize: 24,
                      ),
                    ),
                  ),

                  const SizedBox(height: 10),

                  // Subtitle
                  StaggeredReveal(
                    delayMs: 250,
                    child: Text(
                      'Chọn vai trò phù hợp để SmartLunch mang đến trải nghiệm tốt nhất cho bạn.',
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                        color: AppColors.ink.withOpacity(0.7),
                        height: 1.5,
                        fontSize: 15,
                      ),
                    ),
                  ),

                  const SizedBox(height: 22),

                  StaggeredReveal(
                    delayMs: 320,
                    child: Align(
                      alignment: Alignment.centerLeft,
                      child: Text(
                        'Vai trò của bạn',
                        style: Theme.of(context).textTheme.labelLarge?.copyWith(
                          color: AppColors.ink.withOpacity(0.65),
                          letterSpacing: 0.4,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                  ),

                  const SizedBox(height: 12),

                  // Role cards
                  ...List.generate(roles.length, (index) {
                    final role = roles[index];
                    return Padding(
                      padding: EdgeInsets.only(
                        bottom: index == roles.length - 1 ? 0 : 12,
                      ),
                      child: StaggeredReveal(
                        delayMs: 350 + index * 100,
                        child: _RoleCard(role: role),
                      ),
                    );
                  }),

                  const SizedBox(height: 20),

                  // 💬 Support CTA
                  StaggeredReveal(
                    delayMs: 750,
                    child: Container(
                      width: double.infinity,
                      padding: const EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 14,
                      ),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(14),
                        border: Border.all(
                          color: AppColors.ink.withOpacity(0.08),
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
                      child: InkWell(
                        onTap:
                            () => Navigator.of(
                              context,
                            ).pushNamed(AppRoutes.chatbot),
                        borderRadius: BorderRadius.circular(14),
                        child: Row(
                          children: [
                            Container(
                              padding: const EdgeInsets.all(8),
                              decoration: BoxDecoration(
                                color: AppColors.customer.withOpacity(0.12),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Icon(
                                Icons.chat_bubble_outline_rounded,
                                color: AppColors.customer,
                                size: 20,
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Text(
                                'Cần hỗ trợ? Trò chuyện với SmartLunch',
                                maxLines: 2,
                                overflow: TextOverflow.ellipsis,
                                style: Theme.of(
                                  context,
                                ).textTheme.bodyMedium?.copyWith(
                                  fontWeight: FontWeight.w600,
                                  color: AppColors.ink,
                                ),
                              ),
                            ),
                            const SizedBox(width: 8),
                            Icon(
                              Icons.chevron_right_rounded,
                              size: 18,
                              color: AppColors.ink.withOpacity(0.6),
                            ),
                          ],
                        ),
                      ),
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

/* =======================================================
 * ROLE MODEL
 * ======================================================= */

class _RoleOption {
  final String title;
  final String subtitle;
  final IconData icon;
  final Color accent;
  final List<Color> gradient;
  final String route;

  const _RoleOption({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.accent,
    required this.gradient,
    required this.route,
  });
}

/* =======================================================
 * ROLE CARD - Clean Layout
 * ======================================================= */

class _RoleCard extends StatefulWidget {
  final _RoleOption role;

  const _RoleCard({required this.role});

  @override
  State<_RoleCard> createState() => _RoleCardState();
}

class _RoleCardState extends State<_RoleCard>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 200),
    );
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 0.98,
    ).animate(CurvedAnimation(parent: _controller, curve: Curves.easeInOut));
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ScaleTransition(
      scale: _scaleAnimation,
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(
            color: widget.role.accent.withOpacity(0.12),
            width: 1,
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.02),
              blurRadius: 14,
              offset: const Offset(0, 8),
            ),
          ],
        ),
        child: Material(
          color: Colors.transparent,
          child: InkWell(
            borderRadius: BorderRadius.circular(16),
            onTap: () => Navigator.of(context).pushNamed(widget.role.route),
            onTapDown: (_) => _controller.forward(),
            onTapUp: (_) => _controller.reverse(),
            onTapCancel: () => _controller.reverse(),
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
              child: Row(
                children: [
                  Container(
                    width: 52,
                    height: 52,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: widget.role.accent.withOpacity(0.1),
                      border: Border.all(
                        color: widget.role.accent.withOpacity(0.18),
                      ),
                    ),
                    child: Icon(
                      widget.role.icon,
                      color: widget.role.accent,
                      size: 24,
                    ),
                  ),

                  const SizedBox(width: 12),

                  // Text content
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          widget.role.title,
                          style: Theme.of(
                            context,
                          ).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.w700,
                            color: AppColors.ink,
                            letterSpacing: -0.15,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          widget.role.subtitle,
                          style: Theme.of(
                            context,
                          ).textTheme.bodyMedium?.copyWith(
                            color: AppColors.ink.withOpacity(0.7),
                            height: 1.45,
                            fontSize: 14,
                          ),
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(width: 10),

                  Container(
                    width: 32,
                    height: 32,
                    decoration: BoxDecoration(
                      color: widget.role.accent.withOpacity(0.08),
                      shape: BoxShape.circle,
                      border: Border.all(
                        color: widget.role.accent.withOpacity(0.22),
                      ),
                    ),
                    child: Icon(
                      Icons.chevron_right_rounded,
                      color: widget.role.accent,
                      size: 18,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _BlurOrb extends StatelessWidget {
  final Color color;
  final double size;

  const _BlurOrb({required this.color, this.size = 240});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: color,
        boxShadow: [
          BoxShadow(
            color: color,
            blurRadius: size * 0.45,
            spreadRadius: size * 0.12,
          ),
        ],
      ),
    );
  }
}
