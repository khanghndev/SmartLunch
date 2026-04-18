import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../../app/app_routes.dart';
import '../../../../../core/constants/app_colors.dart';

class PromoCode {
  final String code;
  final String title;
  final String description;
  final String expires;
  final String tag;
  final Color color;
  final double usage; // 0-1 progress of remaining quota
  final bool highlight;

  const PromoCode({
    required this.code,
    required this.title,
    required this.description,
    required this.expires,
    required this.tag,
    required this.color,
    this.usage = 1,
    this.highlight = false,
  });
}

class PromoCodesPage extends StatefulWidget {
  const PromoCodesPage({super.key});

  @override
  State<PromoCodesPage> createState() => _PromoCodesPageState();
}

class _PromoCodesPageState extends State<PromoCodesPage> {
  final List<PromoCode> _promos = const [
    PromoCode(
      code: 'LUNCH25',
      title: 'Giảm 25% bữa trưa',
      description: 'Áp dụng cho đơn từ 120.000đ, giao trước 12h30.',
      expires: 'HSD: 30/09',
      tag: 'Hot deal',
      color: AppColors.customer,
      usage: 0.72,
      highlight: true,
    ),
    PromoCode(
      code: 'TEAM10K',
      title: 'Ưu đãi nhóm +10.000đ',
      description: 'Đặt từ 5 suất trở lên, áp dụng menu doanh nghiệp.',
      expires: 'HSD: 15/10',
      tag: 'Team order',
      color: AppColors.customerAlt,
      usage: 0.56,
    ),
    PromoCode(
      code: 'CLEAN15',
      title: 'Eat Clean -15%',
      description: 'Giảm cho suất eatclean, salad & low-carb.',
      expires: 'HSD: 07/10',
      tag: 'Eat clean',
      color: AppColors.customer,
      usage: 0.38,
    ),
    PromoCode(
      code: 'FREESHIP',
      title: 'Miễn phí giao 5km',
      description: 'Tặng phí ship đơn từ 80.000đ, tối đa 15.000đ.',
      expires: 'HSD: 20/10',
      tag: 'Vận chuyển',
      color: AppColors.customerAlt,
      usage: 0.88,
    ),
  ];

  final Set<String> _filters = {'Giao trưa', 'Doanh nghiệp'};

  void _toggleFilter(String filter) {
    setState(() {
      if (_filters.contains(filter)) {
        _filters.remove(filter);
      } else {
        _filters.add(filter);
      }
    });
  }

  void _copyCode(BuildContext context, String code) {
    Clipboard.setData(ClipboardData(text: code));
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Đã sao chép mã $code'),
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        backgroundColor: AppColors.customer,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF6F7F9),
      appBar: AppBar(
        title: const Text('Mã khuyến mãi'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.shopping_bag_outlined),
            onPressed:
                () => Navigator.of(context).pushNamed(AppRoutes.customerOrder),
          ),
          const SizedBox(width: 4),
        ],
      ),
      body: SafeArea(
        child: CustomScrollView(
          slivers: [
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _HeroCard(filters: _filters, onToggleFilter: _toggleFilter),
                    const SizedBox(height: 16),
                    _SearchFilters(
                      filters: _filters,
                      onToggleFilter: _toggleFilter,
                    ),
                    const SizedBox(height: 12),
                  ],
                ),
              ),
            ),
            SliverList.separated(
              itemCount: _promos.length,
              separatorBuilder: (_, __) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final promo = _promos[index];
                return Padding(
                  padding: EdgeInsets.fromLTRB(16, index == 0 ? 0 : 0, 16, 0),
                  child: _PromoCard(
                    promo: promo,
                    onCopy: () => _copyCode(context, promo.code),
                    onApply:
                        () => ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text(
                              'Áp dụng mã ${promo.code} cho đơn hiện tại',
                            ),
                            behavior: SnackBarBehavior.floating,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                            backgroundColor: promo.color,
                          ),
                        ),
                  ),
                );
              },
            ),
            const SliverToBoxAdapter(child: SizedBox(height: 24)),
          ],
        ),
      ),
    );
  }
}

class _HeroCard extends StatelessWidget {
  final Set<String> filters;
  final void Function(String) onToggleFilter;

  const _HeroCard({required this.filters, required this.onToggleFilter});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.customer, AppColors.customerAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: AppColors.customer.withOpacity(0.3),
            blurRadius: 22,
            offset: const Offset(0, 12),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.16),
                  borderRadius: BorderRadius.circular(14),
                ),
                child: const Icon(
                  Icons.local_offer_rounded,
                  color: Colors.white,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: const [
                    Text(
                      'Ưu đãi hiện hành',
                      style: TextStyle(
                        color: Colors.white,
                        fontWeight: FontWeight.w800,
                        fontSize: 16,
                      ),
                    ),
                    SizedBox(height: 2),
                    Text(
                      'Áp dụng ngay cho đơn giao trưa, suất nhóm và gói eatclean.',
                      style: TextStyle(color: Colors.white70, height: 1.35),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              _heroStat('Tiết kiệm tối đa', '250k/tuần'),
              const SizedBox(width: 12),
              _heroStat('Mã khả dụng', '4 mã'),
            ],
          ),
          const SizedBox(height: 12),
        ],
      ),
    );
  }

  Widget _heroStat(String label, String value) {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
        decoration: BoxDecoration(
          color: Colors.white.withOpacity(0.12),
          borderRadius: BorderRadius.circular(14),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              label,
              style: const TextStyle(
                color: Colors.white70,
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: 6),
            Text(
              value,
              style: const TextStyle(
                color: Colors.white,
                fontWeight: FontWeight.w900,
                fontSize: 16,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _pill(String label, bool selected) {
    return FilterChip(
      label: Text(label),
      selected: selected,
      onSelected: (_) => onToggleFilter(label),
      selectedColor: Colors.white.withOpacity(0.18),
      checkmarkColor: Colors.white,
      labelStyle: TextStyle(
        color: Colors.white.withOpacity(selected ? 1 : 0.85),
        fontWeight: FontWeight.w800,
      ),
      backgroundColor: Colors.white.withOpacity(0.12),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: Colors.white.withOpacity(selected ? 0.8 : 0.3)),
      ),
    );
  }
}

class _SearchFilters extends StatelessWidget {
  final Set<String> filters;
  final void Function(String) onToggleFilter;

  const _SearchFilters({required this.filters, required this.onToggleFilter});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 14,
            offset: const Offset(0, 8),
            spreadRadius: -2,
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          TextField(
            decoration: InputDecoration(
              hintText: 'Tìm mã theo ưu đãi, ví dụ: freeshop, eat clean...',
              prefixIcon: const Icon(
                Icons.search_rounded,
                color: AppColors.customer,
              ),
              filled: true,
              fillColor: const Color(0xFFF6F7FB),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: AppColors.ink.withOpacity(0.1)),
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide(color: AppColors.ink.withOpacity(0.08)),
              ),
            ),
          ),
          const SizedBox(height: 12),
        ],
      ),
    );
  }
}

class _PromoCard extends StatelessWidget {
  final PromoCode promo;
  final VoidCallback onCopy;
  final VoidCallback onApply;

  const _PromoCard({
    required this.promo,
    required this.onCopy,
    required this.onApply,
  });

  @override
  Widget build(BuildContext context) {
    final accent = promo.color;
    return InkWell(
      borderRadius: BorderRadius.circular(18),
      onTap: onApply,
      child: Ink(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: accent.withOpacity(0.14), width: 1.1),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 14,
              offset: const Offset(0, 8),
            ),
          ],
        ),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [accent, accent.withOpacity(0.76)],
                        begin: Alignment.topLeft,
                        end: Alignment.bottomRight,
                      ),
                      borderRadius: BorderRadius.circular(14),
                    ),
                    child: const Icon(
                      Icons.discount_rounded,
                      color: Colors.white,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          promo.title,
                          style: Theme.of(context).textTheme.titleMedium
                              ?.copyWith(fontWeight: FontWeight.w800),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          promo.description,
                          style: Theme.of(
                            context,
                          ).textTheme.bodySmall?.copyWith(
                            color: AppColors.ink.withOpacity(0.68),
                            height: 1.35,
                          ),
                        ),
                      ],
                    ),
                  ),
                  _tag(promo.tag, accent),
                ],
              ),
              const SizedBox(height: 14),
              Row(
                children: [
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        vertical: 8,
                        horizontal: 12,
                      ),
                      decoration: BoxDecoration(
                        color: accent.withOpacity(0.06),
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: accent.withOpacity(0.18)),
                      ),
                      child: Row(
                        children: [
                          Text(
                            promo.code,
                            style: TextStyle(
                              color: accent,
                              fontWeight: FontWeight.w900,
                              letterSpacing: 0.5,
                              fontSize: 15,
                            ),
                          ),
                          const Spacer(),
                          IconButton(
                            icon: Icon(Icons.copy_rounded, color: accent),
                            onPressed: onCopy,
                            tooltip: 'Sao chép mã',
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  ElevatedButton(
                    onPressed: onApply,
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(
                        vertical: 12,
                        horizontal: 14,
                      ),
                      backgroundColor: accent,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                    ),
                    child: const Text(
                      'Áp dụng',
                      style: TextStyle(fontWeight: FontWeight.w800),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Icon(Icons.av_timer_rounded, size: 16, color: accent),
                  const SizedBox(width: 6),
                  Text(
                    promo.expires,
                    style: TextStyle(
                      color: accent,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const Spacer(),
                  Text(
                    'Còn ${((promo.usage) * 100).round()}% lượt',
                    style: TextStyle(
                      color: AppColors.ink.withOpacity(0.65),
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 6),
              ClipRRect(
                borderRadius: BorderRadius.circular(999),
                child: LinearProgressIndicator(
                  value: promo.usage,
                  backgroundColor: accent.withOpacity(0.08),
                  valueColor: AlwaysStoppedAnimation<Color>(accent),
                  minHeight: 8,
                ),
              ),
              if (promo.highlight) ...[
                const SizedBox(height: 8),
                Row(
                  children: [
                    Icon(Icons.verified_rounded, size: 16, color: accent),
                    const SizedBox(width: 6),
                    Text(
                      'Ưu tiên cho đơn giao trưa hôm nay',
                      style: TextStyle(
                        color: accent,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _tag(String label, Color color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: color.withOpacity(0.12),
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        label,
        style: TextStyle(
          color: color,
          fontWeight: FontWeight.w800,
          fontSize: 12,
        ),
      ),
    );
  }
}
