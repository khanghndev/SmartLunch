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

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Lịch trình giao',
      onRefresh: _load,
      body: _loading
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    ShipperCard(
                      onTap: _pickDate,
                      child: Row(
                        children: [
                          Icon(Icons.calendar_month_rounded, color: kShipperRole.primary),
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
                            style: AppDesignSystem.label(color: kShipperRole.primary),
                          ),
                          const Icon(Icons.chevron_right_rounded),
                        ],
                      ),
                    ),
                    const SizedBox(height: 12),
                    if (_items.isEmpty)
                      const ModuleEmptyList(
                        message: 'Không có lịch giao trong ngày này',
                        icon: Icons.event_busy_rounded,
                      )
                    else
                      ..._items.map(
                        (item) => Padding(
                          padding: const EdgeInsets.only(bottom: 10),
                          child: ShipperDeliveryTile(
                            item: item,
                            onTap: () => Navigator.of(context).pushNamed(
                              AppRoutes.shipperDeliveryDetail,
                              arguments: item.deliveryId,
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
    );
  }
}
