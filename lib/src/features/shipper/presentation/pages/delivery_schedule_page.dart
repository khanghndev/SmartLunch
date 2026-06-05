import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliverySchedulePage extends StatefulWidget {
  final bool embeddedInModuleShell;

  const DeliverySchedulePage({
    super.key,
    this.embeddedInModuleShell = false,
  });

  @override
  State<DeliverySchedulePage> createState() => _DeliverySchedulePageState();
}

class _DeliverySchedulePageState extends State<DeliverySchedulePage> {
  DateTime _selected = DateTime.now();
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
      final page = await ShipperRepository.instance.getDeliveries(
        scheduledOn: _selected,
        pageSize: 100,
      );
      if (!mounted) return;
      setState(() {
        _items = page.data;
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

  Future<void> _pickDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _selected,
      firstDate: DateTime.now().subtract(const Duration(days: 30)),
      lastDate: DateTime.now().add(const Duration(days: 60)),
    );
    if (picked != null) {
      setState(() => _selected = picked);
      _load();
    }
  }

  int _countByStatus(Set<String> statuses) => _items
      .where((d) => statuses.contains(d.deliveryStatus.toLowerCase()))
      .length;

  @override
  Widget build(BuildContext context) {
    final pending = _countByStatus({'pending', 'assigned', 'received'});
    final inTransit = _countByStatus({'in_transit'});
    final done = _countByStatus({'completed'});

    return ShipperPageShell(
      title: 'Lịch trình giao',
      onRefresh: _load,
      embeddedInModuleShell: widget.embeddedInModuleShell,
      body: _loading
          ? const ShipperLoadingBody(message: 'Đang tải lịch giao…')
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: shipperListPadding(context),
                  children: [
                    const ShipperPageIntro(
                      title: 'Lịch trình giao hàng',
                      description:
                          'Xem đơn theo ngày phục vụ, lên kế hoạch tuyến và mở bản đồ tối ưu.',
                      icon: Icons.calendar_month_rounded,
                    ),
                    const SizedBox(height: 14),
                    ShipperDatePickerCard(
                      date: _selected,
                      orderCount: _items.length,
                      onTap: _pickDate,
                    ),
                    if (_items.isNotEmpty) ...[
                      const SizedBox(height: 14),
                      ShipperOpsStrip(
                        outerMargin: false,
                        pendingLabel: '$pending',
                        inTransitLabel: '$inTransit',
                        completedLabel: '$done',
                      ),
                      const SizedBox(height: 14),
                      ShipperShortcutRow(
                        leftLabel: 'Bản đồ tuyến',
                        leftIcon: Icons.map_rounded,
                        onLeft: () => Navigator.of(context).pushNamed(AppRoutes.shipperRouteMap),
                        rightLabel: 'Danh sách đơn',
                        rightIcon: Icons.list_alt_rounded,
                        onRight: () => Navigator.of(context).pushNamed(AppRoutes.shipperDeliveryList),
                      ),
                    ],
                    const SizedBox(height: 16),
                    ShipperSectionHeader(
                      title: 'Đơn trong ngày',
                      subtitle: '${_items.length} đơn · ${formatShipperDate(_selected)}',
                    ),
                    const SizedBox(height: 10),
                    if (_items.isEmpty)
                      const ShipperEmptyList(
                        message: 'Không có lịch giao trong ngày này',
                        icon: Icons.event_busy_rounded,
                      )
                    else
                      ..._items.map(
                        (item) => ShipperDeliveryTile(
                          item: item,
                          onTap: () => Navigator.of(context).pushNamed(
                            AppRoutes.shipperDeliveryDetail,
                            arguments: item.deliveryId,
                          ),
                        ),
                      ),
                  ],
                ),
    );
  }
}
