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
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
                  children: [
                    ModulePeriodChips(
                      labels: _filters.map((f) => f.label).toList(),
                      selected: _filterIndex,
                      onSelected: (i) {
                        setState(() => _filterIndex = i);
                        _load();
                      },
                      role: kShipperRole,
                    ),
                    const SizedBox(height: 12),
                    if (_items.isEmpty)
                      const ModuleEmptyList(
                        message: 'Không có đơn giao trong bộ lọc này',
                        icon: Icons.inventory_2_outlined,
                      )
                    else
                      ..._items.map(
                        (item) => Padding(
                          padding: const EdgeInsets.only(bottom: 10),
                          child: ShipperDeliveryTile(
                            item: item,
                            onTap: () => _openDetail(item),
                          ),
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
