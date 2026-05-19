import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../../customer/data/models/customer_menu_models.dart';
import '../../../customer/data/repositories/customer_menu_repository.dart';
import '../widgets/shipper_ui.dart';

class ShipperWeeklyMenuPage extends StatefulWidget {
  const ShipperWeeklyMenuPage({super.key});

  @override
  State<ShipperWeeklyMenuPage> createState() => _ShipperWeeklyMenuPageState();
}

class _ShipperWeeklyMenuPageState extends State<ShipperWeeklyMenuPage> {
  List<CustomerWeeklyMenuSummaryModel> _menus = [];
  CustomerWeeklyMenuModel? _detail;
  int? _selectedId;
  bool _loadingList = true;
  bool _loadingDetail = false;
  String? _error;

  String _fmtDate(DateTime d) =>
      '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';

  @override
  void initState() {
    super.initState();
    _loadList();
  }

  Future<void> _loadList() async {
    setState(() {
      _loadingList = true;
      _error = null;
    });
    try {
      final list = await CustomerMenuRepository.instance.listWeeklyMenus();
      if (!mounted) return;
      setState(() {
        _menus = list;
        _loadingList = false;
        if (list.isNotEmpty && _selectedId == null) {
          _selectMenu(list.first.id);
        }
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = shipperApiError(e);
        _loadingList = false;
      });
    }
  }

  Future<void> _selectMenu(int id) async {
    setState(() {
      _selectedId = id;
      _loadingDetail = true;
      _detail = null;
    });
    try {
      final detail = await CustomerMenuRepository.instance.getWeeklyMenuDetail(id);
      if (!mounted) return;
      setState(() {
        _detail = detail;
        _loadingDetail = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _loadingDetail = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(shipperApiError(e))),
      );
    }
  }

  Map<String, List<CustomerMenuScheduleModel>> _groupByDay(
    List<CustomerMenuScheduleModel> schedules,
  ) {
    final map = <String, List<CustomerMenuScheduleModel>>{};
    for (final s in schedules) {
      final key = _fmtDate(s.date);
      map.putIfAbsent(key, () => []).add(s);
    }
    return map;
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Thực đơn tuần',
      onRefresh: _loadList,
      body: _loadingList
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _loadList)
              : _menus.isEmpty
                  ? const ModuleEmptyList(message: 'Chưa có thực đơn tuần')
                  : Column(
                      children: [
                        SizedBox(
                          height: 88,
                          child: ListView.builder(
                            scrollDirection: Axis.horizontal,
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                            itemCount: _menus.length,
                            itemBuilder: (context, i) {
                              final m = _menus[i];
                              final selected = m.id == _selectedId;
                              return Padding(
                                padding: const EdgeInsets.only(right: 8),
                                child: ChoiceChip(
                                  label: Text(
                                    '${_fmtDate(m.startDate)} – ${_fmtDate(m.endDate)}',
                                    style: AppDesignSystem.body(size: 12),
                                  ),
                                  selected: selected,
                                  onSelected: (_) => _selectMenu(m.id),
                                  selectedColor:
                                      kShipperRole.primary.withValues(alpha: 0.2),
                                ),
                              );
                            },
                          ),
                        ),
                        Expanded(
                          child: _loadingDetail
                              ? const ShipperLoadingBody()
                              : _detail == null
                                  ? const ModuleEmptyList(
                                      message: 'Chọn kỳ thực đơn để xem chi tiết',
                                    )
                                  : Builder(
                                      builder: (context) {
                                        final grouped =
                                            _groupByDay(_detail!.schedules);
                                        final days = grouped.keys.toList()..sort();
                                        return ListView.builder(
                                          padding: const EdgeInsets.all(16),
                                          itemCount: days.length,
                                          itemBuilder: (context, i) {
                                            final day = days[i];
                                            final meals = grouped[day]!;
                                            return Padding(
                                              padding: const EdgeInsets.only(bottom: 10),
                                              child: ShipperCard(
                                                child: Column(
                                                  crossAxisAlignment:
                                                      CrossAxisAlignment.start,
                                                  children: [
                                                    Text(
                                                      day,
                                                      style: AppDesignSystem.sectionTitle(),
                                                    ),
                                                    const SizedBox(height: 8),
                                                    ...meals.map(
                                                      (s) => Padding(
                                                        padding: const EdgeInsets.only(
                                                            bottom: 6),
                                                        child: Row(
                                                          children: [
                                                            Container(
                                                              padding:
                                                                  const EdgeInsets.symmetric(
                                                                horizontal: 8,
                                                                vertical: 2,
                                                              ),
                                                              decoration: BoxDecoration(
                                                                color: kShipperRole.primary
                                                                    .withValues(alpha: 0.1),
                                                                borderRadius:
                                                                    BorderRadius.circular(6),
                                                              ),
                                                              child: Text(
                                                                s.mealSlot,
                                                                style: AppDesignSystem.body(
                                                                  size: 11,
                                                                  color: kShipperRole.primary,
                                                                ),
                                                              ),
                                                            ),
                                                            const SizedBox(width: 8),
                                                            Expanded(
                                                              child: Text(
                                                                s.dish.name,
                                                                style: AppDesignSystem.body(
                                                                  size: 13,
                                                                  color: AppDesignSystem
                                                                      .gray900,
                                                                ),
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
