import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/org_repository.dart';
import 'organization_ui.dart';

/// Bottom sheet chọn món chính (slot main) cho hợp đồng theo kỳ.
class OrgMainDishPickerSheet extends StatefulWidget {
  const OrgMainDishPickerSheet({
    super.key,
    required this.onPick,
  });

  final void Function(OrganizationDishModel dish) onPick;

  static Future<void> show(
    BuildContext context, {
    required void Function(OrganizationDishModel dish) onPick,
  }) {
    return showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => OrgMainDishPickerSheet(onPick: onPick),
    );
  }

  @override
  State<OrgMainDishPickerSheet> createState() => _OrgMainDishPickerSheetState();
}

class _OrgMainDishPickerSheetState extends State<OrgMainDishPickerSheet> {
  bool _loading = false;
  String _search = '';
  List<OrganizationDishModel> _dishes = const [];

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    try {
      final res = await OrgRepository.instance.getMealContractMainDishes(
        page: 1,
        pageSize: 100,
        search: _search.isEmpty ? null : _search,
      );
      if (!mounted) return;
      setState(() {
        _dishes = res.dishes;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _loading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Lỗi tải món chính: ${orgApiError(e)}')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
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
                    'Chọn món chính',
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
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 8),
            child: TextField(
              decoration: AppDesignSystem.inputDecoration(
                label: 'Tìm món',
                hint: 'Nhập tên món',
                prefixIcon: Icons.search_rounded,
                focusColor: orgAccent,
              ),
              onChanged: (v) {
                _search = v.trim();
              },
              onSubmitted: (_) => _load(),
            ),
          ),
          Expanded(
            child: _loading
                ? Center(child: CircularProgressIndicator(color: orgAccent))
                : _dishes.isEmpty
                    ? Center(child: Text('Không có món.', style: AppDesignSystem.body()))
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
      ),
    );
  }
}

