import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/customer_menu_models.dart';
import '../../data/repositories/customer_menu_repository.dart';
import '../widgets/customer_ui.dart';

class MealDetailPage extends StatefulWidget {
  const MealDetailPage({super.key});

  @override
  State<MealDetailPage> createState() => _MealDetailPageState();
}

class _MealDetailPageState extends State<MealDetailPage> {
  bool _isFavorite = false;
  CustomerDishDetailModel? _detail;
  bool _isLoading = true;
  String? _error;
  int _selectedTierIndex = 0;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (_detail == null && _isLoading && _error == null) {
      _loadDetail();
    }
  }

  Future<void> _loadDetail() async {
    final args = CustomerDishDetailArgs.fromRoute(
      ModalRoute.of(context)?.settings.arguments,
    );
    if (args == null || args.id <= 0) {
      setState(() {
        _isLoading = false;
        _error = 'Không tìm thấy món ăn';
      });
      return;
    }

    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final detail = await CustomerMenuRepository.instance.getPublicDishDetail(args.id);
      if (!mounted) return;
      setState(() {
        _detail = detail;
        _isLoading = false;
        _selectedTierIndex = 0;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _error = customerApiError(e);
      });
    }
  }

  String _formatPrice(double price) {
    if (price <= 0) return 'Liên hệ đơn vị';
    if (price >= 1e6) return '${(price / 1e6).toStringAsFixed(1)}Mđ/suất';
    if (price >= 1e3) return '${(price / 1e3).toStringAsFixed(0)}Kđ/suất';
    return '${price.toStringAsFixed(0)}đ/suất';
  }

  @override
  Widget build(BuildContext context) {
    final args = CustomerDishDetailArgs.fromRoute(
      ModalRoute.of(context)?.settings.arguments,
    );

    if (args == null) {
      return CustomerPageShell(
        title: 'Chi tiết món',
        body: CustomerEmptyState(
          title: 'Không tìm thấy món',
          actionLabel: 'Quay lại',
          onAction: () => Navigator.of(context).pop(),
        ),
      );
    }

    if (_isLoading) {
      return Scaffold(
        backgroundColor: AppDesignSystem.gray50,
        appBar: AppBar(
          title: const Text('Chi tiết món'),
          backgroundColor: kCustomerRole.primary,
          foregroundColor: Colors.white,
        ),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    if (_error != null || _detail == null) {
      return CustomerPageShell(
        title: 'Chi tiết món',
        body: CustomerErrorBody(
          message: _error ?? 'Không tải được thông tin món',
          onRetry: _loadDetail,
        ),
      );
    }

    final detail = _detail!;
    final dpr = MediaQuery.devicePixelRatioOf(context);
    final screenWidth = MediaQuery.sizeOf(context).width;
    final cacheWidth = (screenWidth * dpr).round();
    final heroUrl = detail.heroImageUrl ?? args.imageUrl;
    final categoryLabel = args.categoryName?.isNotEmpty == true
        ? args.categoryName!
        : detail.slotCategoryLabel;

    final tiers = detail.priceTiers;
    final activeTier = tiers.isNotEmpty
        ? tiers[_selectedTierIndex.clamp(0, tiers.length - 1)]
        : null;
    final activeQuotas = activeTier?.ingredientQuotas.isNotEmpty == true
        ? activeTier!.ingredientQuotas
        : detail.ingredientQuotas;

    return Scaffold(
      backgroundColor: AppDesignSystem.gray50,
      body: CustomScrollView(
        physics: const BouncingScrollPhysics(),
        slivers: [
          SliverAppBar(
            expandedHeight: 260,
            pinned: true,
            stretch: true,
            leadingWidth: 56,
            leading: Padding(
              padding: const EdgeInsets.only(left: 12),
              child: Center(
                child: Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: Colors.black.withValues(alpha: 0.35),
                    shape: BoxShape.circle,
                  ),
                  child: IconButton(
                    icon: const Icon(Icons.arrow_back_rounded, color: Colors.white, size: 20),
                    padding: EdgeInsets.zero,
                    onPressed: () => Navigator.of(context).pop(),
                  ),
                ),
              ),
            ),
            actions: [
              Padding(
                padding: const EdgeInsets.only(right: 16),
                child: Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: Colors.black.withValues(alpha: 0.35),
                    shape: BoxShape.circle,
                  ),
                  child: IconButton(
                    icon: Icon(
                      _isFavorite ? Icons.favorite_rounded : Icons.favorite_border_rounded,
                      color: _isFavorite ? Colors.red : Colors.white,
                      size: 20,
                    ),
                    padding: EdgeInsets.zero,
                    onPressed: () {
                      setState(() => _isFavorite = !_isFavorite);
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text(
                            _isFavorite
                                ? 'Đã thêm "${detail.name}" vào yêu thích'
                                : 'Đã xóa khỏi danh sách yêu thích',
                          ),
                          duration: const Duration(seconds: 1),
                        ),
                      );
                    },
                  ),
                ),
              ),
            ],
            backgroundColor: kCustomerRole.primary,
            foregroundColor: Colors.white,
            elevation: 0,
            surfaceTintColor: Colors.transparent,
            flexibleSpace: FlexibleSpaceBar(
              stretchModes: const [
                StretchMode.zoomBackground,
                StretchMode.fadeTitle,
              ],
              background: Stack(
                fit: StackFit.expand,
                children: [
                  CustomerDishImage(imageUrl: heroUrl, cacheWidth: cacheWidth),
                  const DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                        colors: [Colors.black45, Colors.transparent, Colors.black54],
                        stops: [0.0, 0.4, 1.0],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          SliverPadding(
            padding: const EdgeInsets.fromLTRB(16, 16, 16, 24),
            sliver: SliverList(
              delegate: SliverChildListDelegate([
                _SectionCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (categoryLabel.isNotEmpty) ...[
                        _TagChip(label: categoryLabel, color: kCustomerRole.primary),
                        const SizedBox(height: 10),
                      ],
                      Text(
                        detail.name,
                        style: AppDesignSystem.title(size: 22, color: AppDesignSystem.gray900),
                      ),
                      if (detail.nameEnglish?.trim().isNotEmpty == true) ...[
                        const SizedBox(height: 4),
                        Text(
                          detail.nameEnglish!,
                          style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray500)
                              .copyWith(fontStyle: FontStyle.italic),
                        ),
                      ],
                      const SizedBox(height: 12),
                      Wrap(
                        spacing: 8,
                        runSpacing: 8,
                        children: [
                          if (detail.cookingMethodLabel.isNotEmpty)
                            _TagChip(
                              icon: Icons.restaurant_rounded,
                              label: detail.cookingMethodLabel,
                              color: kCustomerRole.primary,
                            ),
                          if (detail.dietaryLabel?.trim().isNotEmpty == true)
                            _TagChip(
                              icon: Icons.eco_rounded,
                              label: detail.dietaryLabel!,
                              color: AppDesignSystem.success,
                            ),
                          _TagChip(
                            icon: Icons.visibility_rounded,
                            label: 'Xem miễn phí',
                            color: AppDesignSystem.info,
                          ),
                        ],
                      ),
                      const SizedBox(height: 14),
                      const Divider(height: 1, color: AppDesignSystem.gray100),
                      const SizedBox(height: 12),
                      _MetaRow(
                        icon: Icons.payments_outlined,
                        label: 'Giá tham khảo',
                        value: _formatPrice(detail.price),
                      ),
                      if (detail.code?.trim().isNotEmpty == true) ...[
                        const SizedBox(height: 8),
                        _MetaRow(
                          icon: Icons.tag_rounded,
                          label: 'Mã món',
                          value: detail.code!,
                        ),
                      ],
                      if (detail.slotCategoryCodes.length > 1) ...[
                        const SizedBox(height: 8),
                        _MetaRow(
                          icon: Icons.category_outlined,
                          label: 'Nhóm món',
                          value: detail.slotCategoryCodes
                              .map(CustomerDishDetailModel.slotCategoryLabelOf)
                              .join(' · '),
                        ),
                      ],
                    ],
                  ),
                ),
                const SizedBox(height: 12),

                _SectionCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Về món ăn',
                        style: AppDesignSystem.sectionTitle(color: AppDesignSystem.gray900),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'Thông tin dành cho khách tham khảo',
                        style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                      ),
                      const SizedBox(height: 12),
                      Text(
                        detail.description?.trim().isNotEmpty == true
                            ? detail.description!.trim()
                            : 'Món được phục vụ theo thực đơn tuần của đơn vị. '
                                'Bạn có thể xem thực đơn đầy đủ mà không cần đăng nhập.',
                        style: AppDesignSystem.body(size: 14, color: AppDesignSystem.gray700)
                            .copyWith(height: 1.55),
                      ),
                      const SizedBox(height: 12),
                      _InfoHintRow(
                        icon: Icons.calendar_month_outlined,
                        text: 'Thực đơn có thể thay đổi theo tuần',
                      ),
                      const SizedBox(height: 8),
                      _InfoHintRow(
                        icon: Icons.groups_outlined,
                        text: 'Đặt suất tập thể — liên hệ quản lý đơn vị',
                      ),
                    ],
                  ),
                ),

                if (detail.hasNutrition) ...[
                  const SizedBox(height: 12),
                  _SectionCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Thông tin dinh dưỡng',
                          style: AppDesignSystem.sectionTitle(color: AppDesignSystem.gray900),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Giá trị ước tính cho mỗi suất ăn tiêu chuẩn',
                          style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                        ),
                        const SizedBox(height: 14),
                        Row(
                          children: [
                            if (detail.calories != null)
                              _NutritionItem(
                                value: detail.calories!.round().toString(),
                                unit: 'Kcal',
                                label: 'Năng lượng',
                                color: const Color(0xFFF59E0B),
                                percent: (detail.calories! / 800).clamp(0.15, 0.95),
                              ),
                            if (detail.protein != null)
                              _NutritionItem(
                                value: '${detail.protein!.toStringAsFixed(0)}g',
                                unit: 'Protein',
                                label: 'Chất đạm',
                                color: const Color(0xFFF43F5E),
                                percent: (detail.protein! / 40).clamp(0.15, 0.95),
                              ),
                            if (detail.fat != null)
                              _NutritionItem(
                                value: '${detail.fat!.toStringAsFixed(0)}g',
                                unit: 'Fat',
                                label: 'Chất béo',
                                color: const Color(0xFF0EA5E9),
                                percent: (detail.fat! / 30).clamp(0.15, 0.95),
                              ),
                            if (detail.carbs != null)
                              _NutritionItem(
                                value: '${detail.carbs!.toStringAsFixed(0)}g',
                                unit: 'Carbs',
                                label: 'Tinh bột',
                                color: const Color(0xFF10B981),
                                percent: (detail.carbs! / 80).clamp(0.15, 0.95),
                              ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ],

                if (detail.hasIngredients) ...[
                  const SizedBox(height: 12),
                  _SectionCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Nguyên liệu & định mức chế biến',
                          style: AppDesignSystem.sectionTitle(color: AppDesignSystem.gray900),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Danh sách nguyên liệu dùng để chế biến món theo định mức suất ăn',
                          style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
                        ),
                        if (activeTier != null && activeTier.portionWeightGrams > 0) ...[
                          const SizedBox(height: 10),
                          Container(
                            width: double.infinity,
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                            decoration: BoxDecoration(
                              color: kCustomerRole.primary.withValues(alpha: 0.07),
                              borderRadius: BorderRadius.circular(12),
                              border: Border.all(
                                color: kCustomerRole.primary.withValues(alpha: 0.12),
                              ),
                            ),
                            child: Row(
                              children: [
                                Icon(
                                  Icons.scale_rounded,
                                  size: 18,
                                  color: kCustomerRole.primary,
                                ),
                                const SizedBox(width: 8),
                                Expanded(
                                  child: Text(
                                    'Khối lượng tham chiếu 01 suất: '
                                    '${activeTier.portionWeightGrams.toStringAsFixed(0)}g',
                                    style: AppDesignSystem.body(
                                      size: 12,
                                      color: kCustomerRole.link,
                                    ).copyWith(fontWeight: FontWeight.w700),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                        if (tiers.length > 1) ...[
                          const SizedBox(height: 12),
                          SingleChildScrollView(
                            scrollDirection: Axis.horizontal,
                            child: Row(
                              children: [
                                for (var i = 0; i < tiers.length; i++)
                                  Padding(
                                    padding: EdgeInsets.only(right: i < tiers.length - 1 ? 8 : 0),
                                    child: ChoiceChip(
                                      label: Text(tiers[i].tierTitle),
                                      selected: _selectedTierIndex == i,
                                      onSelected: (_) => setState(() => _selectedTierIndex = i),
                                      selectedColor: kCustomerRole.primary.withValues(alpha: 0.15),
                                      labelStyle: AppDesignSystem.body(
                                        size: 12,
                                        color: _selectedTierIndex == i
                                            ? kCustomerRole.primary
                                            : AppDesignSystem.gray700,
                                      ).copyWith(fontWeight: FontWeight.w700),
                                      side: BorderSide(
                                        color: _selectedTierIndex == i
                                            ? kCustomerRole.primary.withValues(alpha: 0.35)
                                            : AppDesignSystem.gray200,
                                      ),
                                    ),
                                  ),
                              ],
                            ),
                          ),
                        ],
                        const SizedBox(height: 12),
                        if (activeQuotas.isEmpty)
                          Text(
                            'Chưa có định mức nguyên liệu chi tiết.',
                            style: AppDesignSystem.body(size: 13),
                          )
                        else
                          ...activeQuotas.map(
                            (q) => Padding(
                              padding: const EdgeInsets.only(bottom: 8),
                              child: _IngredientRow(quota: q),
                            ),
                          ),
                      ],
                    ),
                  ),
                ],

                if (detail.cookingMethodLabel.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  _SectionCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Quy trình chế biến',
                          style: AppDesignSystem.sectionTitle(color: AppDesignSystem.gray900),
                        ),
                        const SizedBox(height: 10),
                        _InfoHintRow(
                          icon: Icons.soup_kitchen_rounded,
                          text: 'Phương pháp: ${detail.cookingMethodLabel}',
                        ),
                        const SizedBox(height: 8),
                        _InfoHintRow(
                          icon: Icons.verified_user_rounded,
                          text: 'Nguyên liệu được kiểm soát theo định mức bếp trung tâm',
                        ),
                        const SizedBox(height: 8),
                        _InfoHintRow(
                          icon: Icons.no_food_rounded,
                          text: 'Không sử dụng chất bảo quản trong khâu chế biến suất ăn',
                        ),
                      ],
                    ),
                  ),
                ],

                if (detail.images.length > 1) ...[
                  const SizedBox(height: 12),
                  _SectionCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Hình ảnh món',
                          style: AppDesignSystem.sectionTitle(color: AppDesignSystem.gray900),
                        ),
                        const SizedBox(height: 12),
                        SizedBox(
                          height: 88,
                          child: ListView.separated(
                            scrollDirection: Axis.horizontal,
                            itemCount: detail.images.length,
                            separatorBuilder: (_, __) => const SizedBox(width: 10),
                            itemBuilder: (context, index) {
                              final img = detail.images[index];
                              return ClipRRect(
                                borderRadius: BorderRadius.circular(12),
                                child: Image.network(
                                  img.url,
                                  width: 88,
                                  height: 88,
                                  fit: BoxFit.cover,
                                  errorBuilder: (_, __, ___) => Container(
                                    width: 88,
                                    height: 88,
                                    color: AppDesignSystem.gray100,
                                    child: const Icon(Icons.broken_image_outlined),
                                  ),
                                ),
                              );
                            },
                          ),
                        ),
                      ],
                    ),
                  ),
                ],

                const SizedBox(height: 20),
                CustomerPrimaryButton(
                  label: 'Quay lại thực đơn',
                  icon: Icons.restaurant_menu_rounded,
                  onPressed: () => Navigator.of(context).pop(),
                ),
                const SizedBox(height: 10),
                CustomerSecondaryButton(
                  label: 'Về trang chủ',
                  icon: Icons.home_rounded,
                  onPressed: () => Navigator.of(context).popUntil((route) => route.isFirst),
                ),
              ]),
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionCard extends StatelessWidget {
  final Widget child;

  const _SectionCard({required this.child});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: AppDesignSystem.card(radius: 20),
      child: child,
    );
  }
}

class _TagChip extends StatelessWidget {
  final String label;
  final Color color;
  final IconData? icon;

  const _TagChip({required this.label, required this.color, this.icon});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.09),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: color.withValues(alpha: 0.15)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) ...[
            Icon(icon, size: 14, color: color),
            const SizedBox(width: 5),
          ],
          Text(
            label,
            style: AppDesignSystem.body(size: 11, color: color)
                .copyWith(fontWeight: FontWeight.w700),
          ),
        ],
      ),
    );
  }
}

class _MetaRow extends StatelessWidget {
  final IconData icon;
  final String label;
  final String value;

  const _MetaRow({
    required this.icon,
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(icon, size: 18, color: AppDesignSystem.gray400),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(label, style: AppDesignSystem.body(size: 11, color: AppDesignSystem.gray400)),
              const SizedBox(height: 1),
              Text(
                value,
                style: AppDesignSystem.label().copyWith(fontSize: 13.5),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _InfoHintRow extends StatelessWidget {
  final IconData icon;
  final String text;

  const _InfoHintRow({required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(icon, size: 18, color: kCustomerRole.primary),
        const SizedBox(width: 10),
        Expanded(
          child: Text(
            text,
            style: AppDesignSystem.body(size: 13, color: AppDesignSystem.gray700),
          ),
        ),
      ],
    );
  }
}

class _IngredientRow extends StatelessWidget {
  final CustomerDishIngredientQuotaModel quota;

  const _IngredientRow({required this.quota});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: AppDesignSystem.gray50,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppDesignSystem.gray100),
      ),
      child: Row(
        children: [
          Container(
            width: 34,
            height: 34,
            decoration: BoxDecoration(
              color: kCustomerRole.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Icon(Icons.grain_rounded, size: 18, color: kCustomerRole.primary),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              quota.ingredientName.isNotEmpty ? quota.ingredientName : 'Nguyên liệu #${quota.ingredientId}',
              style: AppDesignSystem.label().copyWith(fontSize: 13),
            ),
          ),
          Text(
            quota.quantityLabel,
            style: AppDesignSystem.body(size: 12, color: kCustomerRole.link)
                .copyWith(fontWeight: FontWeight.w800),
          ),
        ],
      ),
    );
  }
}

class _NutritionItem extends StatelessWidget {
  final String value;
  final String unit;
  final String label;
  final Color color;
  final double percent;

  const _NutritionItem({
    required this.value,
    required this.unit,
    required this.label,
    required this.color,
    required this.percent,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Column(
        children: [
          Stack(
            alignment: Alignment.center,
            children: [
              SizedBox(
                width: 48,
                height: 48,
                child: CircularProgressIndicator(
                  value: percent,
                  strokeWidth: 4,
                  backgroundColor: AppDesignSystem.gray100,
                  color: color,
                ),
              ),
              Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    value,
                    style: AppDesignSystem.font.copyWith(
                      fontSize: 12,
                      fontWeight: FontWeight.w800,
                      color: AppDesignSystem.gray900,
                      height: 1.1,
                    ),
                  ),
                  Text(
                    unit,
                    style: AppDesignSystem.body(size: 8, color: AppDesignSystem.gray500),
                  ),
                ],
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            label,
            textAlign: TextAlign.center,
            style: AppDesignSystem.body(size: 10, color: AppDesignSystem.gray700)
                .copyWith(fontWeight: FontWeight.w700),
          ),
        ],
      ),
    );
  }
}
