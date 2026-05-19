import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliveryHistoryPage extends StatefulWidget {
  const DeliveryHistoryPage({super.key});

  @override
  State<DeliveryHistoryPage> createState() => _DeliveryHistoryPageState();
}

class _DeliveryHistoryPageState extends State<DeliveryHistoryPage> {
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
      final completed = await ShipperRepository.instance.getDeliveries(
        status: 'completed',
        pageSize: 50,
      );
      final failed = await ShipperRepository.instance.getDeliveries(
        status: 'failed',
        pageSize: 50,
      );
      final rejected = await ShipperRepository.instance.getDeliveries(
        status: 'rejected',
        pageSize: 50,
      );
      final merged = [
        ...completed.data,
        ...failed.data,
        ...rejected.data,
      ];
      merged.sort((a, b) => b.scheduledDateUtc.compareTo(a.scheduledDateUtc));
      if (!mounted) return;
      setState(() {
        _items = merged;
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

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Lịch sử giao hàng',
      onRefresh: _load,
      body: _loading
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : _items.isEmpty
                  ? const ModuleEmptyList(
                      message: 'Chưa có đơn hoàn tất hoặc thất bại',
                      icon: Icons.history_rounded,
                    )
                  : ListView.builder(
                      padding: const EdgeInsets.all(16),
                      itemCount: _items.length,
                      itemBuilder: (context, i) {
                        final item = _items[i];
                        return Padding(
                          padding: const EdgeInsets.only(bottom: 10),
                          child: ShipperDeliveryTile(
                            item: item,
                            onTap: () => Navigator.of(context).pushNamed(
                              AppRoutes.shipperDeliveryDetail,
                              arguments: item.deliveryId,
                            ),
                          ),
                        );
                      },
                    ),
    );
  }
}
