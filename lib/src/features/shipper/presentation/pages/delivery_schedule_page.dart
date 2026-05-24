import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliverySchedulePage extends StatefulWidget {
  const DeliverySchedulePage({super.key});

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
      body: _loading
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: shipperListPadding(context),
                  children: [
                    const ShipperPageIntro(
                      title: 'Lịch trình giao hàng',
                      description:
                          'Xem tất cả đơn theo ngày phục vụ. Chọn ngày để lên kế hoạch tuyến đường.',
                      icon: Icons.calendar_month_rounded,
                    ),
                    const SizedBox(height: 14),
                    ShipperCard(
                      onTap: _pickDate,
                      child: Row(
                        children: [
                          Icon(Icons.calendar_month_rounded, color: shipperAccent),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text('Ngày giao', style: AppDesignSystem.body(size: 12)),
                                Text(
                                  formatShipperDate(_selected),
                                  style: AppDesignSystem.sectionTitle(),
                                ),
                              ],
                            ),
                          ),
                          Text(
                            '${_items.length} đơn',
                            style: AppDesignSystem.label(color: shipperAccent),
                          ),
                          const Icon(Icons.chevron_right_rounded),
                        ],
                      ),
                    ),
                    if (_items.isNotEmpty) ...[
                      const SizedBox(height: 14),
                      Row(
                        children: [
                          Expanded(
                            child: ShipperStatTile(
                              label: 'Chờ / nhận',
                              value: '$pending',
                              icon: Icons.pending_actions_rounded,
                              color: AppDesignSystem.warning,
                            ),
                          ),
                          const SizedBox(width: 8),
                          Expanded(
                            child: ShipperStatTile(
                              label: 'Đang giao',
                              value: '$inTransit',
                              icon: Icons.local_shipping_rounded,
                              color: shipperAccent,
                            ),
                          ),
                          const SizedBox(width: 8),
                          Expanded(
                            child: ShipperStatTile(
                              label: 'Xong',
                              value: '$done',
                              icon: Icons.check_circle_outline,
                              color: AppDesignSystem.success,
                            ),
                          ),
                        ],
                      ),
                    ],
                    const SizedBox(height: 14),
                    ShipperSectionHeader(
                      title: 'Đơn trong ngày',
                      subtitle: formatShipperDate(_selected),
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
