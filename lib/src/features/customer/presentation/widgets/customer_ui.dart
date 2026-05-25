import 'package:flutter/material.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../../../core/widgets/module_scroll.dart';
import '../../../../core/widgets/premium_drawer.dart';
import '../../../../core/widgets/role_module_header.dart';
import '../../../auth/data/auth_storage.dart';

export '../../../../core/widgets/module_page_shell.dart';
export '../../../../core/widgets/module_scroll.dart';
export 'customer_shell.dart';

EdgeInsets customerListPadding(BuildContext context) =>
    moduleListPadding(context, bottomBarInset: 0);

const RolePalette kCustomerRole = RolePalette.customer;

/// Hero ảnh thực phẩm — đồng bộ Auth / web HUITMeal.
const String kCustomerHeroImageUrl =
    'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1200';

Color get customerAccent => kCustomerRole.primary;

String customerApiError(Object e) {
  if (e is ApiException) return e.message;
  return e.toString();
}

// ─── Shell ───────────────────────────────────────────────────────────────────

class CustomerPageShell extends StatelessWidget {
  final String title;
  final Widget body;
  final List<Widget>? actions;
  final Future<void> Function()? onRefresh;

  const CustomerPageShell({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.onRefresh,
  });

  @override
  Widget build(BuildContext context) => ModulePageShell(
        title: title,
        body: body,
        role: kCustomerRole,
        actions: actions,
        onRefresh: onRefresh,
      );
}

// ─── Module con — UI chuẩn Customer ──────────────────────────────────────────

/// Banner chào khách / khách vãng lai — ngắn gọn, dễ đọc.
class CustomerWelcomeBanner extends StatelessWidget {
  const CustomerWelcomeBanner({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(20, 14, 20, 0),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            kCustomerRole.primary,
            Color.lerp(kCustomerRole.primaryAlt, kCustomerRole.primary, 0.35)!,
          ],
        ),
        boxShadow: [
          BoxShadow(
            color: kCustomerRole.primary.withValues(alpha: 0.28),
            blurRadius: 18,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(20),
        child: Stack(
          children: [
            Positioned(
              right: -20,
              bottom: -20,
              child: Icon(
                Icons.restaurant_rounded,
                size: 120,
                color: Colors.white.withValues(alpha: 0.12),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(18, 18, 18, 18),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.white.withValues(alpha: 0.2),
                      borderRadius: BorderRadius.circular(999),
                      border: Border.all(color: Colors.white24),
                    ),
                    child: Text(
                      'Không cần đăng nhập để xem',
                      style: AppDesignSystem.body(size: 11, color: Colors.white)
                          .copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  const SizedBox(height: 10),
                  Text(
                    'Thực đơn HUITMeal',
                    style: AppDesignSystem.title(size: 22, color: Colors.white),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    'Duyệt món theo danh mục, xem ảnh và chi tiết — phù hợp khách vãng lai và nhân viên muốn tham khảo nhanh.',
                    style: AppDesignSystem.body(size: 13, color: Colors.white.withValues(alpha: 0.92)),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Banner giới thiệu trong nội dung scroll (màn con).
class CustomerPageIntro extends StatelessWidget {
  final String title;
  final String description;
  final IconData icon;

  const CustomerPageIntro({
    super.key,
    required this.title,
    required this.description,
    this.icon = Icons.restaurant_rounded,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(20, 12, 20, 0),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: AppDesignSystem.gray100),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: kCustomerRole.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icon, color: kCustomerRole.primary, size: 26),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: AppDesignSystem.sectionTitle()),
                const SizedBox(height: 4),
                Text(
                  description,
                  style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Ba điểm tin cậy — giúp khách vãng lai yên tâm khi duyệt.
class CustomerTrustStrip extends StatelessWidget {
  const CustomerTrustStrip({super.key});

  @override
  Widget build(BuildContext context) {
    const items = [
      (Icons.visibility_rounded, 'Xem miễn phí', 'Không bắt buộc đăng nhập'),
      (Icons.eco_rounded, 'Suất ăn sạch', 'Chuẩn doanh nghiệp'),
      (Icons.update_rounded, 'Cập nhật', 'Theo thực đơn tuần'),
    ];

    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 14, 20, 0),
      child: Row(
        children: [
          for (var i = 0; i < items.length; i++) ...[
            if (i > 0) const SizedBox(width: 8),
            Expanded(
              child: _TrustMiniCard(
                icon: items[i].$1,
                title: items[i].$2,
                subtitle: items[i].$3,
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _TrustMiniCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;

  const _TrustMiniCard({
    required this.icon,
    required this.title,
    required this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 10),
      decoration: AppDesignSystem.card(radius: 14),
      child: Column(
        children: [
          Icon(icon, size: 20, color: kCustomerRole.primary),
          const SizedBox(height: 6),
          Text(
            title,
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: AppDesignSystem.label().copyWith(fontSize: 11),
          ),
          const SizedBox(height: 2),
          Text(
            subtitle,
            textAlign: TextAlign.center,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: AppDesignSystem.body(size: 9, color: AppDesignSystem.gray500),
          ),
        ],
      ),
    );
  }
}

/// Gợi ý đăng nhập nhẹ — chỉ hiện khi chưa có phiên.
class CustomerGuestPromptBar extends StatelessWidget {
  final VoidCallback onLogin;

  const CustomerGuestPromptBar({super.key, required this.onLogin});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 14, 20, 0),
      child: Material(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        child: InkWell(
          onTap: onLogin,
          borderRadius: BorderRadius.circular(14),
          child: Ink(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: kCustomerRole.primary.withValues(alpha: 0.25)),
            ),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: kCustomerRole.primary.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(Icons.person_outline_rounded, color: kCustomerRole.primary, size: 22),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Đã có tài khoản?', style: AppDesignSystem.label()),
                      Text(
                        'Đăng nhập để quản lý hồ sơ cá nhân',
                        style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                      ),
                    ],
                  ),
                ),
                Icon(Icons.login_rounded, color: kCustomerRole.link, size: 22),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

/// Kiểm tra phiên — dùng cho banner khách / nút header.
class CustomerSessionScope extends StatefulWidget {
  final Widget Function(BuildContext context, bool hasSession) builder;

  const CustomerSessionScope({super.key, required this.builder});

  @override
  State<CustomerSessionScope> createState() => _CustomerSessionScopeState();
}

class _CustomerSessionScopeState extends State<CustomerSessionScope> {
  bool? _hasSession;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final session = await const AuthStorage().readSession();
    if (mounted) setState(() => _hasSession = session != null);
  }

  @override
  Widget build(BuildContext context) {
    if (_hasSession == null) return const SizedBox.shrink();
    return widget.builder(context, _hasSession!);
  }
}

/// Nút đăng nhập gọn trên header (khách vãng lai).
class CustomerHeaderLoginAction extends StatelessWidget {
  const CustomerHeaderLoginAction({super.key});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white.withValues(alpha: 0.18),
      borderRadius: BorderRadius.circular(10),
      child: InkWell(
        onTap: () => navigateAppToLogin(context),
        borderRadius: BorderRadius.circular(10),
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.login_rounded, color: Colors.white, size: 18),
              const SizedBox(width: 4),
              Text(
                'Đăng nhập',
                style: AppDesignSystem.label(color: Colors.white).copyWith(fontSize: 12),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class CustomerGlassCard extends ModuleCard {
  const CustomerGlassCard({super.key, required super.child, super.padding});
}

class CustomerLoadingBody extends StatelessWidget {
  final String? message;
  final bool showHomeSkeleton;

  const CustomerLoadingBody({
    super.key,
    this.message,
    this.showHomeSkeleton = false,
  });

  @override
  Widget build(BuildContext context) {
    if (showHomeSkeleton) {
      return const CustomerHomeSkeleton();
    }
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(40),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            CircularProgressIndicator(color: kCustomerRole.primary),
            if (message != null) ...[
              const SizedBox(height: 16),
              Text(message!, style: AppDesignSystem.body(), textAlign: TextAlign.center),
            ],
          ],
        ),
      ),
    );
  }
}

/// Placeholder khi tải trang chủ — tránh màn hình trống.
class CustomerHomeSkeleton extends StatelessWidget {
  const CustomerHomeSkeleton({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      physics: const NeverScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
      children: const [
        CustomerSkeletonBox(height: 120, radius: 20),
        SizedBox(height: 14),
        Row(
          children: [
            Expanded(child: CustomerSkeletonBox(height: 72, radius: 14)),
            SizedBox(width: 8),
            Expanded(child: CustomerSkeletonBox(height: 72, radius: 14)),
            SizedBox(width: 8),
            Expanded(child: CustomerSkeletonBox(height: 72, radius: 14)),
          ],
        ),
        SizedBox(height: 20),
        CustomerSkeletonBox(height: 18, width: 140, radius: 8),
        SizedBox(height: 12),
        CustomerSkeletonBox(height: 220, radius: 20),
        SizedBox(height: 20),
        CustomerSkeletonBox(height: 18, width: 160, radius: 8),
        SizedBox(height: 12),
        CustomerSkeletonBox(height: 88, radius: 16),
        SizedBox(height: 10),
        CustomerSkeletonBox(height: 88, radius: 16),
      ],
    );
  }
}

/// Placeholder lưới món (tab Thực đơn).
class CustomerDishGridSkeleton extends StatelessWidget {
  const CustomerDishGridSkeleton({super.key});

  @override
  Widget build(BuildContext context) {
    return GridView.builder(
      physics: const NeverScrollableScrollPhysics(),
      padding: const EdgeInsets.all(16),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        childAspectRatio: 0.76,
        crossAxisSpacing: 12,
        mainAxisSpacing: 12,
      ),
      itemCount: 6,
      itemBuilder: (_, __) => const CustomerSkeletonBox(height: double.infinity, radius: 16),
    );
  }
}

class CustomerSkeletonBox extends StatelessWidget {
  final double height;
  final double? width;
  final double radius;

  const CustomerSkeletonBox({
    super.key,
    required this.height,
    this.width,
    this.radius = 12,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: AppDesignSystem.gray100,
        borderRadius: BorderRadius.circular(radius),
      ),
    );
  }
}

class CustomerErrorBody extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const CustomerErrorBody({super.key, required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) => ModuleErrorBody(
        message: message,
        onRetry: onRetry,
        role: kCustomerRole,
      );
}

class CustomerPrimaryButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback? onPressed;

  const CustomerPrimaryButton({
    super.key,
    required this.label,
    this.icon = Icons.arrow_forward_rounded,
    this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return FilledButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, size: 20),
      label: Text(label, style: AppDesignSystem.label(color: Colors.white)),
      style: FilledButton.styleFrom(
        backgroundColor: kCustomerRole.primary,
        minimumSize: const Size.fromHeight(48),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
    );
  }
}

// ─── Header module (đồng bộ mọi tab) ─────────────────────────────────────────

/// Mục danh mục (tab Thực đơn).
class CustomerMenuCategory {
  final int id;
  final String name;

  const CustomerMenuCategory({required this.id, required this.name});
}

/// Header chuẩn Customer — delegate [RoleModuleHeader] (đồng bộ Manager / Org / Shipper).
class CustomerModuleHeader extends StatelessWidget {
  final String title;
  final String subtitle;
  final Widget? bottomPanel;
  final Widget? trailing;

  const CustomerModuleHeader({
    super.key,
    required this.title,
    required this.subtitle,
    this.bottomPanel,
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return CustomerSessionScope(
      builder: (context, hasSession) {
        return RoleModuleHeader(
          role: kCustomerRole,
          title: title,
          subtitle: subtitle,
          bottomPanel: bottomPanel,
          trustPill: 'Suất ăn sạch',
          trailing: trailing ?? (hasSession ? null : const CustomerHeaderLoginAction()),
        );
      },
    );
  }
}

/// Ô tìm kiếm chuẩn (tab Trang chủ).
class CustomerHeaderSearchPanel extends StatelessWidget {
  final TextEditingController controller;
  final ValueChanged<String>? onChanged;
  final VoidCallback? onSubmit;

  const CustomerHeaderSearchPanel({
    super.key,
    required this.controller,
    this.onChanged,
    this.onSubmit,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(12, 12, 12, 12),
      child: ListenableBuilder(
        listenable: controller,
        builder: (context, _) {
          return TextField(
            controller: controller,
            onChanged: onChanged,
            onSubmitted: onSubmit != null ? (_) => onSubmit!() : null,
            style: AppDesignSystem.body(color: AppDesignSystem.gray900),
            decoration: InputDecoration(
              hintText: 'Gõ tên món để lọc nhanh...',
              hintStyle: AppDesignSystem.body(color: AppDesignSystem.gray400),
              prefixIcon: Icon(Icons.search_rounded, color: kCustomerRole.primary),
              suffixIcon: controller.text.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.close_rounded, size: 20),
                      onPressed: () {
                        controller.clear();
                        onChanged?.call('');
                      },
                    )
                  : null,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12),
            borderSide: BorderSide.none,
          ),
          filled: true,
          fillColor: AppDesignSystem.gray50,
              contentPadding: const EdgeInsets.symmetric(vertical: 12),
            ),
          );
        },
      ),
    );
  }
}

/// Chip danh mục (tab Thực đơn).
class CustomerHeaderCategoryPanel extends StatelessWidget {
  final List<CustomerMenuCategory> categories;
  final int selectedCategoryId;
  final ValueChanged<int> onCategorySelected;
  final bool isLoading;

  const CustomerHeaderCategoryPanel({
    super.key,
    required this.categories,
    required this.selectedCategoryId,
    required this.onCategorySelected,
    this.isLoading = false,
  });

  @override
  Widget build(BuildContext context) {
    if (isLoading) {
      return const Padding(
        padding: EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        child: LinearProgressIndicator(
          minHeight: 3,
          borderRadius: BorderRadius.all(Radius.circular(4)),
        ),
      );
    }
    if (categories.isEmpty) {
      return Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        child: Text('Chưa có danh mục món', style: AppDesignSystem.body(size: 13)),
      );
    }
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 12, 8, 14),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        primary: false,
        child: Row(
          children: [
            for (var i = 0; i < categories.length; i++) ...[
              if (i > 0) const SizedBox(width: 8),
              RoleHeaderChip(
                label: categories[i].name,
                selected: categories[i].id == selectedCategoryId,
                role: kCustomerRole,
                onTap: () => onCategorySelected(categories[i].id),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class CustomerSectionHeader extends StatelessWidget {
  final String title;
  final String? subtitle;
  final String? actionLabel;
  final VoidCallback? onAction;

  const CustomerSectionHeader({
    super.key,
    required this.title,
    this.subtitle,
    this.actionLabel,
    this.onAction,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 8, 20, 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: AppDesignSystem.sectionTitle()),
                if (subtitle != null) ...[
                  const SizedBox(height: 2),
                  Text(subtitle!, style: AppDesignSystem.body(size: 12)),
                ],
              ],
            ),
          ),
          if (actionLabel != null && onAction != null)
            TextButton.icon(
              onPressed: onAction,
              icon: Icon(Icons.arrow_forward_rounded, size: 16, color: kCustomerRole.link),
              label: Text(actionLabel!, style: AppDesignSystem.label(color: kCustomerRole.link)),
              style: TextButton.styleFrom(
                padding: const EdgeInsets.symmetric(horizontal: 8),
                minimumSize: Size.zero,
                tapTargetSize: MaterialTapTargetSize.shrinkWrap,
              ),
            ),
        ],
      ),
    );
  }
}

/// Shortcut Trang chủ — đồng bộ panel header Manager.
class CustomerQuickActions extends StatelessWidget {
  final VoidCallback onOpenMenu;
  final VoidCallback? onScrollCategories;

  const CustomerQuickActions({
    super.key,
    required this.onOpenMenu,
    this.onScrollCategories,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 4, 20, 8),
      child: RoleHeaderQuickActionsPanel(
        actions: [
          RoleHeaderQuickAction(
            label: 'Toàn bộ thực đơn',
            icon: Icons.restaurant_menu_rounded,
            color: kCustomerRole.primary,
            onTap: onOpenMenu,
          ),
          RoleHeaderQuickAction(
            label: 'Danh mục món',
            icon: Icons.grid_view_rounded,
            color: AppDesignSystem.orange600,
            onTap: onScrollCategories ?? onOpenMenu,
          ),
          RoleHeaderQuickAction(
            label: 'Món gợi ý',
            icon: Icons.star_rounded,
            color: const Color(0xFFFB7185),
            onTap: onOpenMenu,
          ),
        ],
      ),
    );
  }
}

class CustomerStatBanner extends StatelessWidget {
  final int categoryCount;
  final int dishCount;

  const CustomerStatBanner({
    super.key,
    required this.categoryCount,
    required this.dishCount,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 14, 20, 0),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: AppDesignSystem.gray100),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.04),
              blurRadius: 10,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Row(
          children: [
            _StatPill(value: '$categoryCount', label: 'Nhóm món', icon: Icons.grid_view_rounded),
            Container(width: 1, height: 36, color: AppDesignSystem.gray200),
            _StatPill(value: '$dishCount', label: 'Đang hiển thị', icon: Icons.ramen_dining_rounded),
            const Spacer(),
            Icon(Icons.verified_rounded, color: kCustomerRole.primary, size: 28),
          ],
        ),
      ),
    );
  }
}

// ─── Cards ───────────────────────────────────────────────────────────────────

class CustomerFeaturedCard extends StatelessWidget {
  final String name;
  final String? imageUrl;
  final String? categoryName;
  final int? cacheWidth;
  final VoidCallback? onTap;

  const CustomerFeaturedCard({
    super.key,
    required this.name,
    this.imageUrl,
    this.categoryName,
    this.cacheWidth,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(20),
        child: Ink(
          width: 200,
          height: 260,
          decoration: AppDesignSystem.card(radius: 20).copyWith(
            boxShadow: [
              BoxShadow(
                color: kCustomerRole.primary.withValues(alpha: 0.14),
                blurRadius: 22,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(20),
            child: Stack(
              fit: StackFit.expand,
              children: [
                IgnorePointer(
                  child: CustomerDishImage(imageUrl: imageUrl, cacheWidth: cacheWidth),
                ),
                const IgnorePointer(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [Colors.transparent, Color(0xD9000000)],
                        stops: [0.4, 1.0],
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 10,
                  left: 10,
                  child: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: kCustomerRole.primary,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Text(
                      'Gợi ý',
                      style: AppDesignSystem.body(size: 10, color: Colors.white)
                          .copyWith(fontWeight: FontWeight.w800),
                    ),
                  ),
                ),
                Positioned(
                  left: 12,
                  right: 12,
                  bottom: 12,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      if (categoryName != null && categoryName!.isNotEmpty) ...[
                        Text(
                          categoryName!,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: AppDesignSystem.body(size: 11, color: Colors.white70),
                        ),
                        const SizedBox(height: 4),
                      ],
                      Text(
                        name,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: AppDesignSystem.sectionTitle(color: Colors.white),
                      ),
                      const SizedBox(height: 8),
                      Row(
                        children: [
                          Text(
                            'Xem món',
                            style: AppDesignSystem.label(color: Colors.white).copyWith(fontSize: 11),
                          ),
                          const SizedBox(width: 4),
                          const Icon(Icons.arrow_forward_rounded, color: Colors.white, size: 14),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class CustomerDishCard extends StatelessWidget {
  final String name;
  final String? imageUrl;
  final String? categoryName;
  final int? cacheWidth;
  final VoidCallback? onTap;

  const CustomerDishCard({
    super.key,
    required this.name,
    this.imageUrl,
    this.categoryName,
    this.cacheWidth,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        child: Ink(
          decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg).copyWith(
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                flex: 11,
                child: ClipRRect(
                  borderRadius: const BorderRadius.vertical(
                    top: Radius.circular(AppDesignSystem.radiusLg),
                  ),
                  child: Stack(
                    fit: StackFit.expand,
                    children: [
                      CustomerDishImage(imageUrl: imageUrl, cacheWidth: cacheWidth),
                      Positioned(
                        left: 8,
                        bottom: 8,
                        child: Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          decoration: BoxDecoration(
                            color: Colors.black.withValues(alpha: 0.55),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              const Icon(Icons.touch_app_rounded, color: Colors.white, size: 12),
                              const SizedBox(width: 4),
                              Text(
                                'Chạm xem',
                                style: AppDesignSystem.body(size: 10, color: Colors.white),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Expanded(
                flex: 9,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(12, 10, 12, 10),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (categoryName != null && categoryName!.isNotEmpty)
                        Text(
                          categoryName!,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: AppDesignSystem.body(size: 10, color: AppDesignSystem.gray500),
                        ),
                      Text(
                        name,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: AppDesignSystem.sectionTitle().copyWith(fontSize: 13, height: 1.2),
                      ),
                      const Spacer(),
                      Row(
                        children: [
                          Text(
                            'Chi tiết',
                            style: AppDesignSystem.label(color: kCustomerRole.link).copyWith(fontSize: 11),
                          ),
                          const SizedBox(width: 2),
                          Icon(Icons.arrow_forward_rounded, size: 14, color: kCustomerRole.link),
                        ],
                      ),
                    ],
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

/// Ô danh mục dạng lưới — dễ quét hơn danh sách dọc.
class CustomerCategoryTile extends StatelessWidget {
  final String name;
  final IconData icon;
  final Color accent;
  final VoidCallback? onTap;

  const CustomerCategoryTile({
    super.key,
    required this.name,
    required this.icon,
    required this.accent,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Ink(
          padding: const EdgeInsets.all(14),
          decoration: AppDesignSystem.card(radius: 16).copyWith(
            border: Border.all(color: accent.withValues(alpha: 0.2)),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [accent.withValues(alpha: 0.2), accent.withValues(alpha: 0.08)],
                  ),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, color: accent, size: 24),
              ),
              const Spacer(),
              Text(
                name,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: AppDesignSystem.label().copyWith(height: 1.2),
              ),
              const SizedBox(height: 6),
              Row(
                children: [
                  Text(
                    'Xem món',
                    style: AppDesignSystem.body(size: 11, color: kCustomerRole.link),
                  ),
                  Icon(Icons.arrow_forward_rounded, size: 14, color: kCustomerRole.link),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Chip danh mục — delegate [RoleHeaderChip].
class CustomerCategoryChip extends StatelessWidget {
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const CustomerCategoryChip({
    super.key,
    required this.label,
    required this.selected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return RoleHeaderChip(
      label: label,
      selected: selected,
      role: kCustomerRole,
      onTap: onTap,
    );
  }
}

class CustomerEmptyState extends StatelessWidget {
  final IconData icon;
  final String title;
  final String? subtitle;
  final String? actionLabel;
  final VoidCallback? onAction;
  final bool compact;

  const CustomerEmptyState({
    super.key,
    this.icon = Icons.restaurant_menu_outlined,
    required this.title,
    this.subtitle,
    this.actionLabel,
    this.onAction,
    this.compact = false,
  });

  @override
  Widget build(BuildContext context) {
    final content = Column(
      mainAxisAlignment: MainAxisAlignment.center,
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          padding: EdgeInsets.all(compact ? 16 : 20),
          decoration: BoxDecoration(
            color: kCustomerRole.primary.withValues(alpha: 0.08),
            shape: BoxShape.circle,
          ),
          child: Icon(
            icon,
            size: compact ? 40 : 48,
            color: kCustomerRole.primary.withValues(alpha: 0.75),
          ),
        ),
        SizedBox(height: compact ? 12 : 16),
        Text(title, textAlign: TextAlign.center, style: AppDesignSystem.sectionTitle()),
        if (subtitle != null) ...[
          const SizedBox(height: 6),
          Text(
            subtitle!,
            textAlign: TextAlign.center,
            style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray500),
          ),
        ],
        if (actionLabel != null && onAction != null) ...[
          SizedBox(height: compact ? 14 : 20),
          CustomerPrimaryButton(label: actionLabel!, onPressed: onAction),
        ],
      ],
    );

    if (compact) {
      return Padding(padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 20), child: content);
    }
    return Center(
      child: Padding(padding: const EdgeInsets.all(32), child: content),
    );
  }
}

/// Tham số điều hướng chi tiết món.
class CustomerDishDetailArgs {
  final int id;
  final String name;
  final String? imageUrl;
  final String? categoryName;

  const CustomerDishDetailArgs({
    required this.id,
    required this.name,
    this.imageUrl,
    this.categoryName,
  });

  static CustomerDishDetailArgs? fromRoute(Object? args) {
    if (args is CustomerDishDetailArgs) return args;
    if (args is int) return CustomerDishDetailArgs(id: args, name: 'Món #$args');
    if (args is Map) {
      return CustomerDishDetailArgs(
        id: args['id'] is int ? args['id'] as int : int.tryParse('${args['id']}') ?? 0,
        name: args['name']?.toString() ?? 'Món ăn',
        imageUrl: args['imageUrl']?.toString(),
        categoryName: args['categoryName']?.toString(),
      );
    }
    return null;
  }
}

class CustomerDishImage extends StatelessWidget {
  final String? imageUrl;
  final int? cacheWidth;

  const CustomerDishImage({super.key, this.imageUrl, this.cacheWidth});

  Widget _placeholder() {
    return DecoratedBox(
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            kCustomerRole.primary.withValues(alpha: 0.08),
            AppDesignSystem.gray100,
          ],
        ),
      ),
      child: Center(
        child: Icon(Icons.fastfood_rounded, color: kCustomerRole.primary.withValues(alpha: 0.45), size: 44),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final url = imageUrl?.trim();
    if (url == null || url.isEmpty) return _placeholder();
    return Image.network(
      url,
      fit: BoxFit.cover,
      filterQuality: FilterQuality.medium,
      cacheWidth: cacheWidth,
      loadingBuilder: (context, child, progress) {
        if (progress == null) return child;
        return Stack(
          fit: StackFit.expand,
          children: [
            _placeholder(),
            Center(
              child: SizedBox(
                width: 28,
                height: 28,
                child: CircularProgressIndicator(
                  strokeWidth: 2.5,
                  color: kCustomerRole.primary,
                  value: progress.expectedTotalBytes != null
                      ? progress.cumulativeBytesLoaded / progress.expectedTotalBytes!
                      : null,
                ),
              ),
            ),
          ],
        );
      },
      errorBuilder: (_, __, ___) => _placeholder(),
    );
  }
}

class CustomerSecondaryButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback? onPressed;

  const CustomerSecondaryButton({
    super.key,
    required this.label,
    this.icon = Icons.arrow_back_rounded,
    this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return OutlinedButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, size: 20, color: kCustomerRole.primary),
      label: Text(label, style: AppDesignSystem.label(color: kCustomerRole.primary)),
      style: OutlinedButton.styleFrom(
        minimumSize: const Size.fromHeight(48),
        side: BorderSide(color: kCustomerRole.primary.withValues(alpha: 0.45)),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDesignSystem.radiusMd),
        ),
      ),
    );
  }
}

// ─── Private helpers ───────────────────────────────────────────────────────────

class _StatPill extends StatelessWidget {
  final String value;
  final String label;
  final IconData icon;

  const _StatPill({required this.value, required this.label, required this.icon});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Row(
        children: [
          Icon(icon, size: 18, color: kCustomerRole.primary),
          const SizedBox(width: 8),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(value, style: AppDesignSystem.title(size: 18, color: AppDesignSystem.gray900)),
              Text(label, style: AppDesignSystem.body(size: 11)),
            ],
          ),
        ],
      ),
    );
  }
}

/// Icons xoay vòng cho danh mục.
IconData customerCategoryIcon(int index) {
  const icons = [
    Icons.ramen_dining_rounded,
    Icons.lunch_dining_rounded,
    Icons.local_pizza_rounded,
    Icons.rice_bowl_rounded,
    Icons.egg_alt_rounded,
    Icons.icecream_rounded,
    Icons.bakery_dining_rounded,
    Icons.set_meal_rounded,
  ];
  return icons[index % icons.length];
}

Color customerCategoryColor(int index) {
  const colors = [
    AppDesignSystem.orange500,
    AppDesignSystem.orange600,
    AppDesignSystem.orange400,
    Color(0xFFFB7185),
    Color(0xFFFBBF24),
    Color(0xFF34D399),
    Color(0xFF60A5FA),
    Color(0xFFA78BFA),
  ];
  return colors[index % colors.length];
}
