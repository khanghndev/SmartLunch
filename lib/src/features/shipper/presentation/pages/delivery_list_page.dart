import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliveryListPage extends StatefulWidget {
  final bool embeddedInModuleShell;

  const DeliveryListPage({
    super.key,
    this.embeddedInModuleShell = false,
  });

  @override
  State<DeliveryListPage> createState() => _DeliveryListPageState();
}

class _DeliveryListPageState extends State<DeliveryListPage> {
  static const _filters = [
    _ListFilter(label: 'Hôm nay', status: null, todayOnly: true),
    _ListFilter(label: 'Chờ nhận', status: 'pending', todayOnly: false),
    _ListFilter(label: 'Đang giao', status: 'in_transit', todayOnly: false),
    _ListFilter(label: 'Tất cả', status: null, todayOnly: false),
  ];

  int _filterIndex = 0;
  List<ShipperDeliveryListItemModel> _items = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final f = _filters[_filterIndex];
      final page = await ShipperRepository.instance.getDeliveries(
        pageSize: 100,
        status: f.status,
        scheduledOn: f.todayOnly ? DateTime.now() : null,
      );
      var items = page.data;
      if (f.status == 'pending') {
        items = items
            .where((d) {
              final s = d.deliveryStatus.toLowerCase();
              return s == 'pending' || s == 'assigned' || s == 'received';
            })
            .toList();
      }
      if (!mounted) return;
      setState(() {
        _items = items;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = shipperApiError(e);
        _loading = false;
      });
    }
  }

  int get _pendingCount => _items
      .where((d) {
        final s = d.deliveryStatus.toLowerCase();
        return s == 'pending' || s == 'assigned' || s == 'received';
      })
      .length;

  int get _inTransitCount =>
      _items.where((d) => d.deliveryStatus.toLowerCase() == 'in_transit').length;

  int get _completedCount =>
      _items.where((d) => d.deliveryStatus.toLowerCase() == 'completed').length;

  void _openDetail(ShipperDeliveryListItemModel item) {
    Navigator.of(context)
        .pushNamed(AppRoutes.shipperDeliveryDetail, arguments: item.deliveryId)
        .then((_) => _load());
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Đơn cần giao',
      onRefresh: _load,
      embeddedInModuleShell: widget.embeddedInModuleShell,
      body: _loading
          ? const ShipperLoadingBody(message: 'Đang tải danh sách đơn…')
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: shipperListPadding(context),
                  children: [
                    const ShipperPageIntro(
                      title: 'Đơn cần giao',
                      description:
                          'Lọc nhanh theo ngày và trạng thái. Chạm đơn để cập nhật trạng thái hoặc chụp PoD.',
                      icon: Icons.inventory_2_rounded,
                    ),
                    const SizedBox(height: 14),
                    ShipperContentCard(
                      title: 'Bộ lọc',
                      subtitle: _filters[_filterIndex].label,
                      accent: shipperAccent,
                      child: Column(
                        children: [
                          ShipperPeriodChips(
                            labels: _filters.map((f) => f.label).toList(),
                            selected: _filterIndex,
                            onSelected: (i) {
                              setState(() => _filterIndex = i);
                              _load();
                            },
                          ),
                          if (_items.isNotEmpty) ...[
                            const SizedBox(height: 14),
                            ShipperOpsStrip(
                              outerMargin: false,
                              pendingLabel: '$_pendingCount',
                              inTransitLabel: '$_inTransitCount',
                              completedLabel: '$_completedCount',
                            ),
                          ],
                        ],
                      ),
                    ),
                    const SizedBox(height: 14),
                    ShipperShortcutRow(
                      leftLabel: 'Bản đồ tuyến',
                      leftIcon: Icons.map_rounded,
                      onLeft: () => Navigator.of(context).pushNamed(AppRoutes.shipperRouteMap),
                      rightLabel: 'Lịch giao',
                      rightIcon: Icons.calendar_month_rounded,
                      onRight: () => Navigator.of(context).pushNamed(AppRoutes.shipperSchedule),
                    ),
                    const SizedBox(height: 16),
                    ShipperSectionHeader(
                      title: 'Danh sách đơn',
                      subtitle: '${_items.length} đơn · ${_filters[_filterIndex].label}',
                    ),
                    const SizedBox(height: 10),
                    if (_items.isEmpty)
                      const ShipperEmptyList(
                        message: 'Không có đơn giao trong bộ lọc này',
                        icon: Icons.inventory_2_outlined,
                      )
                    else
                      ..._items.map(
                        (item) => ShipperDeliveryTile(
                          item: item,
                          onTap: () => _openDetail(item),
                        ),
                      ),
                  ],
                ),
    );
  }
}

class _ListFilter {
  final String label;
  final String? status;
  final bool todayOnly;

  const _ListFilter({
    required this.label,
    required this.status,
    required this.todayOnly,
  });
}
