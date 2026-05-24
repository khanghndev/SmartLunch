import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../../utils/shipper_geo.dart';
import '../../utils/shipper_maps_launcher.dart';
import '../widgets/shipper_route_map_panel.dart';
import '../widgets/shipper_ui.dart';

class RouteMapPage extends StatefulWidget {
  const RouteMapPage({super.key});

  @override
  State<RouteMapPage> createState() => _RouteMapPageState();
}

class _RouteMapPageState extends State<RouteMapPage> with SingleTickerProviderStateMixin {
  late TabController _tabs;
  List<ShipperDeliveryListItemModel> _active = [];
  ShipperRouteOptimizeResponseModel? _route;
  bool _loading = true;
  bool _optimizing = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _tabs = TabController(length: 2, vsync: this);
    _load();
  }

  @override
  void dispose() {
    _tabs.dispose();
    super.dispose();
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
      if (_tabs.index != 1) {
        _tabs.animateTo(1);
      }
    } catch (e) {
      if (!mounted) return;
      setState(() => _optimizing = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(shipperApiError(e))),
      );
    }
  }

  List<ShipperMapMarker> get _mapMarkers {
    if (_route != null && _route!.stops.isNotEmpty) {
      return _route!.stops.map(ShipperMapMarker.fromRouteStop).toList();
    }
    return _active
        .asMap()
        .entries
        .map((e) => ShipperMapMarker.fromDelivery(e.value, sequence: e.key + 1))
        .toList();
  }

  void _openGoogleRoute() {
    final markers = _mapMarkers;
    if (markers.isEmpty) return;
    ShipperMapsLauncher.openMultiStopRoute(
      stops: markers.map((m) => ShipperGeoPoint(m.latitude, m.longitude)).toList(),
      origin: kShipperDefaultStart,
    );
  }

  Widget _introHeader() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
      child: const ShipperPageIntro(
        title: 'Tuyến giao hàng',
        description:
            'Bản đồ OpenStreetMap trong app: xem điểm giao, tối ưu thứ tự, mở Google Maps chỉ đường.',
        icon: Icons.map_rounded,
      ),
    );
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
          ? const ShipperLoadingBody(message: 'Đang tải đơn hôm nay…')
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    _introHeader(),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(16, 10, 16, 0),
                      child: DecoratedBox(
                        decoration: AppDesignSystem.card(radius: 14),
                        child: TabBar(
                          controller: _tabs,
                          labelColor: shipperAccent,
                          unselectedLabelColor: AppDesignSystem.gray500,
                          indicatorColor: shipperAccent,
                          tabs: const [
                            Tab(text: 'Danh sách'),
                            Tab(text: 'Bản đồ'),
                          ],
                        ),
                      ),
                    ),
                    Expanded(
                      child: TabBarView(
                        controller: _tabs,
                        children: [
                          _listTab(),
                          _mapTab(),
                        ],
                      ),
                    ),
                  ],
                ),
    );
  }

  Widget _listTab() {
    return ModuleListView(
      padding: shipperListPadding(context),
      children: [
        const ShipperInfoBanner(
          message:
              'Tọa độ ước lượng từ địa chỉ khi BE chưa có GPS. Nhấn biểu tượng tuyến trên AppBar để sắp xếp thứ tự tối ưu.',
          icon: Icons.info_outline_rounded,
        ),
        if (_route != null) ...[
          const SizedBox(height: 12),
          ShipperStatTile(
            label: 'Quãng đường ước tính',
            value: '${_route!.approxTotalDistanceKm.toStringAsFixed(2)} km',
            icon: Icons.straighten_rounded,
            color: shipperAccent,
          ),
          const SizedBox(height: 14),
          const ShipperSectionHeader(title: 'Thứ tự giao đề xuất'),
          const SizedBox(height: 8),
          ..._route!.stops.map(_optimizedStopRow),
          const SizedBox(height: 12),
        ],
        ShipperSectionHeader(
          title: 'Đơn đang xử lý',
          subtitle: '${_active.length} đơn received / in_transit',
        ),
        const SizedBox(height: 10),
        if (_active.isEmpty)
          const ShipperEmptyList(
            message: 'Không có đơn received/in_transit hôm nay',
            icon: Icons.map_outlined,
          )
        else
          ..._active.map(
            (d) => ShipperDeliveryTile(
              item: d,
              onTap: () => Navigator.of(context).pushNamed(
                AppRoutes.shipperDeliveryDetail,
                arguments: d.deliveryId,
              ),
            ),
          ),
      ],
    );
  }

  Widget _mapTab() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (_route != null)
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
            child: ShipperStatTile(
              label: 'Tuyến tối ưu (OSM)',
              value:
                  '${_route!.stops.length} điểm · ${_route!.approxTotalDistanceKm.toStringAsFixed(1)} km',
              icon: Icons.route_rounded,
              color: AppDesignSystem.success,
            ),
          ),
        Expanded(
          child: Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
            child: ShipperOsmMap(
              markers: _mapMarkers,
              origin: kShipperDefaultStart,
              onOpenGoogleMaps: _openGoogleRoute,
            ),
          ),
        ),
        if (_mapMarkers.isNotEmpty)
          SizedBox(
            height: 160,
            child: ModuleListView(
                padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
                children: [
                  const ShipperSectionHeader(title: 'Điểm trên bản đồ'),
                  const SizedBox(height: 6),
                  ..._mapMarkers.map(
                    (m) => ShipperDataRow(
                      icon: Icons.place_rounded,
                      iconColor: shipperAccent,
                      title: m.label.isNotEmpty ? m.label : 'Điểm giao',
                      subtitle:
                          '${m.latitude.toStringAsFixed(5)}, ${m.longitude.toStringAsFixed(5)}',
                      trailing: m.sequence != null ? '#${m.sequence}' : null,
                      onTap: m.deliveryId != null
                          ? () => Navigator.of(context).pushNamed(
                                AppRoutes.shipperDeliveryDetail,
                                arguments: m.deliveryId,
                              )
                          : null,
                    ),
                  ),
                ],
              ),
            ),
      ],
    );
  }

  Widget _optimizedStopRow(ShipperRouteStopModel stop) {
    return ShipperDataRow(
      icon: Icons.pin_drop_rounded,
      iconColor: shipperAccent,
      title: stop.label,
      subtitle: '${stop.latitude.toStringAsFixed(5)}, ${stop.longitude.toStringAsFixed(5)}',
      trailing: '#${stop.sequence}',
      onTap: stop.deliveryId != null
          ? () => Navigator.of(context).pushNamed(
                AppRoutes.shipperDeliveryDetail,
                arguments: stop.deliveryId,
              )
          : null,
    );
  }
}
