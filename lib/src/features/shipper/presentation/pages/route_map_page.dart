import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../../utils/shipper_geo.dart';
import '../widgets/shipper_ui.dart';

class RouteMapPage extends StatefulWidget {
  const RouteMapPage({super.key});

  @override
  State<RouteMapPage> createState() => _RouteMapPageState();
}

class _RouteMapPageState extends State<RouteMapPage> {
  List<ShipperDeliveryListItemModel> _active = [];
  ShipperRouteOptimizeResponseModel? _route;
  bool _loading = true;
  bool _optimizing = false;
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
      _route = null;
    });
    try {
      final page = await ShipperRepository.instance.getDeliveries(
        scheduledOn: DateTime.now(),
        pageSize: 100,
      );
      final active = page.data.where((d) {
        final s = d.deliveryStatus.toLowerCase();
        return s == 'received' || s == 'in_transit';
      }).toList();
      if (!mounted) return;
      setState(() {
        _active = active;
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

  Future<void> _optimize() async {
    if (_active.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không có điểm giao để tối ưu')),
      );
      return;
    }
    setState(() => _optimizing = true);
    try {
      const start = kShipperDefaultStart;
      final stops = <ShipperRouteStopInput>[];
      for (var i = 0; i < _active.length; i++) {
        final d = _active[i];
        final geo = approximateCoordsForAddress(d.deliveryAddress, seed: d.deliveryId);
        stops.add(ShipperRouteStopInput(
          deliveryId: d.deliveryId,
          label: d.deliveryAddress,
          latitude: geo.latitude,
          longitude: geo.longitude,
        ));
      }
      final result = await ShipperRepository.instance.optimizeRoute(
        startLat: start.latitude,
        startLng: start.longitude,
        stops: stops,
      );
      if (!mounted) return;
      setState(() {
        _route = result;
        _optimizing = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _optimizing = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(shipperApiError(e))),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Tuyến giao hàng',
      onRefresh: _load,
      actions: [
        if (!_loading && _active.isNotEmpty)
          IconButton(
            onPressed: _optimizing ? null : _optimize,
            icon: _optimizing
                ? const SizedBox(
                    width: 22,
                    height: 22,
                    child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                  )
                : const Icon(Icons.route_rounded),
            tooltip: 'Tối ưu tuyến',
          ),
      ],
      body: _loading
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    ShipperCard(
                      child: Row(
                        children: [
                          Icon(Icons.info_outline, color: kShipperRole.primary),
                          const SizedBox(width: 10),
                          Expanded(
                            child: Text(
                              'Tọa độ điểm giao được ước lượng từ địa chỉ (BE chưa có GPS). '
                              'Nhấn biểu tượng tuyến để sắp xếp thứ tự giao tối ưu.',
                              style: AppDesignSystem.body(size: 12),
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 12),
                    if (_route != null) ...[
                      ModuleStatTile(
                        label: 'Quãng đường ước tính',
                        value: '${_route!.approxTotalDistanceKm.toStringAsFixed(2)} km',
                        icon: Icons.straighten_rounded,
                        color: kShipperRole.primary,
                      ),
                      const SizedBox(height: 12),
                      Text('Thứ tự giao đề xuất', style: AppDesignSystem.sectionTitle()),
                      const SizedBox(height: 8),
                      ..._route!.stops.map(_optimizedStopTile),
                      const SizedBox(height: 16),
                    ],
                    Text(
                      'Đơn đang xử lý (${_active.length})',
                      style: AppDesignSystem.sectionTitle(),
                    ),
                    const SizedBox(height: 8),
                    if (_active.isEmpty)
                      const ModuleEmptyList(
                        message: 'Không có đơn received/in_transit hôm nay',
                        icon: Icons.map_outlined,
                      )
                    else
                      ..._active.map(
                        (d) => Padding(
                          padding: const EdgeInsets.only(bottom: 8),
                          child: ShipperDeliveryTile(
                            item: d,
                            onTap: () => Navigator.of(context).pushNamed(
                              AppRoutes.shipperDeliveryDetail,
                              arguments: d.deliveryId,
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
    );
  }

  Widget _optimizedStopTile(ShipperRouteStopModel stop) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: ShipperCard(
        onTap: stop.deliveryId != null
            ? () => Navigator.of(context).pushNamed(
                  AppRoutes.shipperDeliveryDetail,
                  arguments: stop.deliveryId,
                )
            : null,
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            CircleAvatar(
              radius: 16,
              backgroundColor: kShipperRole.primary,
              child: Text(
                '${stop.sequence}',
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.w800,
                  fontSize: 13,
                ),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    stop.label,
                    style: AppDesignSystem.label(),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  Text(
                    '${stop.latitude.toStringAsFixed(5)}, ${stop.longitude.toStringAsFixed(5)}',
                    style: AppDesignSystem.body(size: 11),
                  ),
                ],
              ),
            ),
            const Icon(Icons.chevron_right_rounded),
          ],
        ),
      ),
    );
  }
}
