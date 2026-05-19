import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/org_repository.dart';
import 'organization_ui.dart';

/// Bottom sheet chọn món theo category (khớp web dish picker).
class OrgDishPickerSheet extends StatefulWidget {
  final List<DishCategoryModel> categories;
  final String slotKey;
  final void Function(OrganizationDishModel dish) onPick;

  const OrgDishPickerSheet({
    super.key,
    required this.categories,
    required this.slotKey,
    required this.onPick,
  });

  static Future<void> show(
    BuildContext context, {
    required List<DishCategoryModel> categories,
    required String slotKey,
    required void Function(OrganizationDishModel dish) onPick,
  }) {
    return showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => OrgDishPickerSheet(
        categories: categories,
        slotKey: slotKey,
        onPick: onPick,
      ),
    );
  }

  @override
  State<OrgDishPickerSheet> createState() => _OrgDishPickerSheetState();
}

class _OrgDishPickerSheetState extends State<OrgDishPickerSheet> {
  late int _categoryId;
  bool _loading = false;
  List<OrganizationDishModel> _dishes = [];

  List<DishCategoryModel> get _filtered => widget.categories
      .where((c) => c.slotKey.toLowerCase() == widget.slotKey.toLowerCase())
      .toList()
    ..sort((a, b) => a.sortOrder.compareTo(b.sortOrder));

  @override
  void initState() {
    super.initState();
    final cats = _filtered;
    _categoryId = cats.isNotEmpty ? cats.first.id : 0;
    if (_categoryId > 0) _loadDishes(_categoryId);
  }

  Future<void> _loadDishes(int categoryId) async {
    setState(() => _loading = true);
    try {
      final res = await OrgRepository.instance.getDishesByCategory(categoryId);
      if (mounted) {
        setState(() {
          _dishes = res.dishes;
          _loading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() => _loading = false);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Lỗi tải món: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final cats = _filtered;
    return Container(
      height: MediaQuery.of(context).size.height * 0.75,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 16, 12, 8),
            child: Row(
              children: [
                Expanded(
                  child: Text(
                    'Chọn món — ${widget.slotKey == 'main' ? 'Món chính' : widget.slotKey == 'side' ? 'Món phụ' : 'Canh'}',
                    style: AppDesignSystem.sectionTitle(),
                  ),
                ),
                IconButton(
                  onPressed: () => Navigator.pop(context),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
          ),
          if (cats.isEmpty)
            const Expanded(
              child: Center(child: Text('Không có danh mục cho khung này.')),
            )
          else ...[
            SizedBox(
              height: 44,
              child: ListView.separated(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                scrollDirection: Axis.horizontal,
                itemCount: cats.length,
                separatorBuilder: (_, __) => const SizedBox(width: 8),
                itemBuilder: (_, i) {
                  final c = cats[i];
                  final active = c.id == _categoryId;
                  return ChoiceChip(
                    label: Text(c.name),
                    selected: active,
                    onSelected: (_) {
                      setState(() => _categoryId = c.id);
                      _loadDishes(c.id);
                    },
                    selectedColor: orgAccent.withValues(alpha: 0.2),
                    labelStyle: TextStyle(
                      color: active ? orgAccent : AppDesignSystem.gray500,
                      fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                    ),
                  );
                },
              ),
            ),
            const SizedBox(height: 8),
            Expanded(
              child: _loading
                  ? Center(child: CircularProgressIndicator(color: orgAccent))
                  : _dishes.isEmpty
                      ? Center(
                          child: Text(
                            'Không có món.',
                            style: AppDesignSystem.body(),
                          ),
                        )
                      : ListView.separated(
                          padding: const EdgeInsets.all(16),
                          itemCount: _dishes.length,
                          separatorBuilder: (_, __) => const SizedBox(height: 8),
                          itemBuilder: (_, i) {
                            final d = _dishes[i];
                            return ListTile(
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(12),
                                side: const BorderSide(color: AppDesignSystem.gray100),
                              ),
                              leading: CircleAvatar(
                                backgroundColor: orgAccent.withValues(alpha: 0.12),
                                child: Icon(Icons.restaurant, color: orgAccent, size: 20),
                              ),
                              title: Text(d.name, style: AppDesignSystem.label()),
                              trailing: const Icon(Icons.add_circle_outline),
                              onTap: () {
                                widget.onPick(d);
                                Navigator.pop(context);
                              },
                            );
                          },
                        ),
            ),
          ],
        ],
      ),
    );
  }
}
