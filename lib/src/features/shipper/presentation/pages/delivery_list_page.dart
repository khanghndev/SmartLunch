import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliveryListPage extends StatefulWidget {
  const DeliveryListPage({super.key});

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
                          'Lọc theo hôm nay, trạng thái chờ nhận hoặc đang giao. Chạm đơn để xem chi tiết và cập nhật.',
                      icon: Icons.inventory_2_rounded,
                    ),
                    const SizedBox(height: 14),
                    ShipperPeriodChips(
                      labels: _filters.map((f) => f.label).toList(),
                      selected: _filterIndex,
                      onSelected: (i) {
                        setState(() => _filterIndex = i);
                        _load();
                      },
                    ),
                    if (!_loading && _items.isNotEmpty) ...[
                      const SizedBox(height: 14),
                      Row(
                        children: [
                          Expanded(
                            child: ShipperStatTile(
                              label: 'Chờ xử lý',
                              value: '$_pendingCount',
                              icon: Icons.hourglass_top_rounded,
                              color: AppDesignSystem.warning,
                            ),
                          ),
                          const SizedBox(width: 10),
                          Expanded(
                            child: ShipperStatTile(
                              label: 'Đang giao',
                              value: '$_inTransitCount',
                              icon: Icons.local_shipping_rounded,
                              color: shipperAccent,
                            ),
                          ),
                        ],
                      ),
                    ],
                    const SizedBox(height: 14),
                    ShipperSectionHeader(
                      title: 'Danh sách',
                      subtitle: '${_items.length} đơn',
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
