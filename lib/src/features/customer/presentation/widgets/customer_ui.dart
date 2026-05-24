import 'package:flutter/material.dart';

import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../../core/widgets/module_page_shell.dart';
import '../../../../core/widgets/module_scroll.dart';
import '../../../../core/widgets/role_module_header.dart';

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

/// Banner giới thiệu trong nội dung scroll (Trang chủ / màn con).
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
        gradient: LinearGradient(
          colors: [
            kCustomerRole.primary.withValues(alpha: 0.12),
            kCustomerRole.primaryAlt.withValues(alpha: 0.06),
          ],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
        border: Border.all(color: kCustomerRole.primary.withValues(alpha: 0.18)),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
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

class CustomerGlassCard extends ModuleCard {
  const CustomerGlassCard({super.key, required super.child, super.padding});
}

class CustomerLoadingBody extends StatelessWidget {
  final String? message;

  const CustomerLoadingBody({super.key, this.message});

  @override
  Widget build(BuildContext context) {
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

  const CustomerModuleHeader({
    super.key,
    required this.title,
    required this.subtitle,
    this.bottomPanel,
  });

  @override
  Widget build(BuildContext context) {
    return RoleModuleHeader(
      role: kCustomerRole,
      title: title,
      subtitle: subtitle,
      bottomPanel: bottomPanel,
      trustPill: 'Suất ăn sạch',
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
              hintText: 'Tìm món trên trang chủ...',
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

  const CustomerQuickActions({super.key, required this.onOpenMenu});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 4, 20, 8),
      child: RoleHeaderQuickActionsPanel(
        actions: [
          RoleHeaderQuickAction(
            label: 'Thực đơn đầy đủ',
            icon: Icons.restaurant_menu_rounded,
            color: kCustomerRole.primary,
            onTap: onOpenMenu,
          ),
          RoleHeaderQuickAction(
            label: 'Món nổi bật',
            icon: Icons.star_rounded,
            color: AppDesignSystem.orange600,
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
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 0),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          gradient: LinearGradient(
            colors: [
              kCustomerRole.primary.withValues(alpha: 0.12),
              kCustomerRole.primaryAlt.withValues(alpha: 0.08),
            ],
          ),
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: kCustomerRole.primary.withValues(alpha: 0.2)),
        ),
        child: Row(
          children: [
            _StatPill(value: '$categoryCount', label: 'Danh mục', icon: Icons.grid_view_rounded),
            Container(width: 1, height: 36, color: AppDesignSystem.gray200),
            _StatPill(value: '$dishCount', label: 'Món gợi ý', icon: Icons.ramen_dining_rounded),
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
  final int? cacheWidth;
  final VoidCallback? onTap;

  const CustomerFeaturedCard({
    super.key,
    required this.name,
    this.imageUrl,
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
          width: 188,
          height: 248,
          decoration: AppDesignSystem.card(radius: 20).copyWith(
            boxShadow: [
              BoxShadow(
                color: kCustomerRole.primary.withValues(alpha: 0.12),
                blurRadius: 20,
                offset: const Offset(0, 8),
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
                        colors: [
                          Colors.transparent,
                          Color(0xBF000000),
                        ],
                        stops: [0.45, 1.0],
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 10,
                  right: 10,
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
                      Text(
                        name,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: AppDesignSystem.sectionTitle(color: Colors.white),
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
  final int? cacheWidth;
  final VoidCallback? onTap;

  const CustomerDishCard({
    super.key,
    required this.name,
    this.imageUrl,
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
          decoration: AppDesignSystem.card(radius: AppDesignSystem.radiusLg),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                flex: 3,
                child: ClipRRect(
                  borderRadius: const BorderRadius.vertical(
                    top: Radius.circular(AppDesignSystem.radiusLg),
                  ),
                  child: CustomerDishImage(imageUrl: imageUrl, cacheWidth: cacheWidth),
                ),
              ),
              Expanded(
                flex: 2,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(12, 10, 12, 10),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        name,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: AppDesignSystem.sectionTitle().copyWith(fontSize: 13),
                      ),
                      const Spacer(),
                      Row(
                        children: [
                          Text(
                            'Xem chi tiết',
                            style: AppDesignSystem.body(size: 11, color: kCustomerRole.link),
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

class CustomerCategoryCard extends StatelessWidget {
  final String name;
  final IconData icon;
  final Color accent;
  final VoidCallback? onTap;

  const CustomerCategoryCard({
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
          decoration: AppDesignSystem.card(radius: 16),
          child: Row(
            children: [
              Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  color: accent.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, color: accent, size: 22),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  name,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: AppDesignSystem.label(),
                ),
              ),
              Icon(Icons.chevron_right_rounded, color: AppDesignSystem.gray400, size: 20),
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

  @override
  Widget build(BuildContext context) {
    final url = imageUrl?.trim();
    if (url == null || url.isEmpty) {
      return ColoredBox(
        color: AppDesignSystem.gray100,
        child: Center(
          child: Icon(Icons.fastfood_rounded, color: AppDesignSystem.gray400, size: 44),
        ),
      );
    }
    return Image.network(
      url,
      fit: BoxFit.cover,
      filterQuality: FilterQuality.medium,
      cacheWidth: cacheWidth,
      errorBuilder: (_, __, ___) => ColoredBox(
        color: AppDesignSystem.gray100,
        child: Icon(Icons.broken_image_outlined, color: AppDesignSystem.gray400, size: 36),
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
