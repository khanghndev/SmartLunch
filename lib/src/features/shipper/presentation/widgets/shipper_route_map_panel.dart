import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../utils/shipper_geo.dart';
import '../../utils/shipper_maps_launcher.dart';
import 'shipper_ui.dart';

/// Bản đồ OpenStreetMap (flutter_map) — xem điểm giao & tuyến nối các điểm.
class ShipperOsmMap extends StatefulWidget {
  final List<ShipperMapMarker> markers;
  final ShipperGeoPoint? origin;
  final double? height;
  final bool showGoogleMapsButton;
  final bool drawRoutePolyline;
  final VoidCallback? onOpenGoogleMaps;

  const ShipperOsmMap({
    super.key,
    required this.markers,
    this.origin,
    this.height = 280,
    this.showGoogleMapsButton = true,
    this.drawRoutePolyline = true,
    this.onOpenGoogleMaps,
  });

  @override
  State<ShipperOsmMap> createState() => _ShipperOsmMapState();
}

class _ShipperOsmMapState extends State<ShipperOsmMap> {
  final MapController _controller = MapController();
  bool _mapReady = false;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  void didUpdateWidget(ShipperOsmMap oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.markers != widget.markers ||
        oldWidget.origin != widget.origin) {
      WidgetsBinding.instance.addPostFrameCallback((_) => _fitToMarkers());
    }
  }

  List<LatLng> get _polylinePoints {
    final pts = <LatLng>[];
    final origin = widget.origin;
    if (origin != null) {
      pts.add(LatLng(origin.latitude, origin.longitude));
    }
    for (final m in widget.markers) {
      pts.add(LatLng(m.latitude, m.longitude));
    }
    return pts;
  }

  List<LatLng> get _allPoints => _polylinePoints;

  LatLng get _mapCenter {
    final points = _allPoints;
    if (points.isEmpty) {
      return const LatLng(10.7769, 106.7009);
    }
    var lat = 0.0;
    var lng = 0.0;
    for (final p in points) {
      lat += p.latitude;
      lng += p.longitude;
    }
    return LatLng(lat / points.length, lng / points.length);
  }

  void _fitToMarkers() {
    if (!_mapReady || !mounted) return;
    final points = _allPoints;
    if (points.isEmpty) return;

    final center = _mapCenter;
    if (points.length == 1) {
      _controller.move(center, 15);
      return;
    }

    final bounds = LatLngBounds.fromPoints(points);
    final latSpan = (bounds.north - bounds.south).abs();
    final lngSpan = (bounds.east - bounds.west).abs();
    if (latSpan < 1e-5 && lngSpan < 1e-5) {
      _controller.move(center, 15);
      return;
    }

    try {
      _controller.fitCamera(
        CameraFit.bounds(
          bounds: bounds,
          padding: const EdgeInsets.all(40),
        ),
      );
    } catch (_) {
      _controller.move(center, 14);
    }
  }

  void _onMapReady() {
    _mapReady = true;
    WidgetsBinding.instance.addPostFrameCallback((_) => _fitToMarkers());
  }

  void _openGoogleMaps() {
    if (widget.onOpenGoogleMaps != null) {
      widget.onOpenGoogleMaps!();
      return;
    }
    final stops = widget.markers
        .map((m) => ShipperGeoPoint(m.latitude, m.longitude))
        .toList();
    if (stops.isEmpty) return;
    ShipperMapsLauncher.openMultiStopRoute(
      stops: stops,
      origin: widget.origin,
    );
  }

  @override
  Widget build(BuildContext context) {
    if (widget.markers.isEmpty) {
      return ShipperCard(
        child: Column(
          children: [
            Icon(Icons.map_outlined, size: 40, color: AppDesignSystem.gray400),
            const SizedBox(height: 8),
            Text(
              'Chưa có điểm giao để hiển thị trên bản đồ',
              style: AppDesignSystem.body(),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      );
    }

    final polyline = _polylinePoints;
    final initial = _mapCenter;

    final map = ClipRRect(
      borderRadius: BorderRadius.circular(AppDesignSystem.radiusLg),
      child: Stack(
        children: [
          FlutterMap(
            mapController: _controller,
            options: MapOptions(
              initialCenter: initial,
              initialZoom: 13,
              onMapReady: _onMapReady,
              interactionOptions: const InteractionOptions(
                flags: InteractiveFlag.all & ~InteractiveFlag.rotate,
              ),
            ),
            children: [
              TileLayer(
                urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                userAgentPackageName: 'com.smartlunch.mobile',
              ),
              if (widget.drawRoutePolyline && polyline.length > 1)
                PolylineLayer(
                  polylines: [
                    Polyline(
                      points: polyline,
                      color: shipperAccent,
                      strokeWidth: 4,
                      borderColor: Colors.white,
                      borderStrokeWidth: 1.5,
                    ),
                  ],
                ),
              MarkerLayer(
                markers: [
                  if (widget.origin != null)
                    Marker(
                      point: LatLng(
                        widget.origin!.latitude,
                        widget.origin!.longitude,
                      ),
                      width: 36,
                      height: 36,
                      child: const Icon(
                        Icons.my_location_rounded,
                        color: AppDesignSystem.success,
                        size: 32,
                      ),
                    ),
                  ...widget.markers.map(_buildMarker),
                ],
              ),
            ],
          ),
          Positioned(
            right: 8,
            bottom: 8,
            child: Material(
              color: Colors.white.withValues(alpha: 0.92),
              borderRadius: BorderRadius.circular(8),
              elevation: 2,
              child: IconButton(
                icon: Icon(Icons.center_focus_strong_rounded, color: shipperAccent),
                tooltip: 'Căn giữa tuyến',
                onPressed: _fitToMarkers,
              ),
            ),
          ),
          const Positioned(
            left: 6,
            bottom: 4,
            child: _OsmAttribution(),
          ),
        ],
      ),
    );

    const buttonBlock = 58.0;

    return LayoutBuilder(
      builder: (context, constraints) {
        final maxH = constraints.maxHeight;
        final resolvedHeight = widget.height ??
            (maxH.isFinite && maxH > buttonBlock + 120
                ? maxH - (widget.showGoogleMapsButton ? buttonBlock : 0)
                : 280.0);

        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            SizedBox(
              height: resolvedHeight.clamp(120.0, 520.0),
              child: map,
            ),
            if (widget.showGoogleMapsButton) ...[
              const SizedBox(height: 10),
              ShipperOutlineButton(
                label: 'Chỉ đường Google Maps',
                icon: Icons.navigation_rounded,
                onPressed: _openGoogleMaps,
              ),
            ],
          ],
        );
      },
    );
  }

  Marker _buildMarker(ShipperMapMarker m) {
    return Marker(
      point: LatLng(m.latitude, m.longitude),
      width: 44,
      height: 44,
      child: Column(
        children: [
          Container(
            width: 30,
            height: 30,
            decoration: BoxDecoration(
              color: m.color ?? shipperAccent,
              shape: BoxShape.circle,
              border: Border.all(color: Colors.white, width: 2),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withValues(alpha: 0.2),
                  blurRadius: 4,
                ),
              ],
            ),
            alignment: Alignment.center,
            child: Text(
              m.sequence != null ? '${m.sequence}' : '•',
              style: const TextStyle(
                color: Colors.white,
                fontWeight: FontWeight.w800,
                fontSize: 12,
              ),
            ),
          ),
          Icon(Icons.arrow_drop_down, size: 18, color: m.color ?? shipperAccent),
        ],
      ),
    );
  }
}

class _OsmAttribution extends StatelessWidget {
  const _OsmAttribution();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
      child: Text(
        '© OpenStreetMap',
        style: AppDesignSystem.body(size: 9, color: AppDesignSystem.gray500),
      ),
    );
  }
}

/// Panel bản đồ tuyến (OSM) — dùng trên màn tuyến giao / chi tiết đơn.
class ShipperRouteMapPanel extends StatelessWidget {
  final List<ShipperMapMarker> markers;
  final ShipperGeoPoint? center;
  final double height;
  final VoidCallback? onOpenGoogleMaps;

  const ShipperRouteMapPanel({
    super.key,
    required this.markers,
    this.center,
    this.height = 280,
    this.onOpenGoogleMaps,
  });

  @override
  Widget build(BuildContext context) {
    return ShipperOsmMap(
      markers: markers,
      origin: center,
      height: height,
      onOpenGoogleMaps: onOpenGoogleMaps,
    );
  }
}

class ShipperMapMarker {
  final double latitude;
  final double longitude;
  final int? sequence;
  final int? deliveryId;
  final String label;
  final Color? color;

  const ShipperMapMarker({
    required this.latitude,
    required this.longitude,
    this.sequence,
    this.deliveryId,
    this.label = '',
    this.color,
  });

  factory ShipperMapMarker.fromDelivery(
    ShipperDeliveryListItemModel d, {
    int? sequence,
  }) {
    final geo = approximateCoordsForAddress(d.deliveryAddress, seed: d.deliveryId);
    return ShipperMapMarker(
      latitude: geo.latitude,
      longitude: geo.longitude,
      sequence: sequence,
      deliveryId: d.deliveryId,
      label: d.deliveryAddress,
    );
  }

  factory ShipperMapMarker.fromDetail(ShipperDeliveryDetailModel d) {
    final geo = approximateCoordsForAddress(d.deliveryAddress, seed: d.deliveryId);
    return ShipperMapMarker(
      latitude: geo.latitude,
      longitude: geo.longitude,
      deliveryId: d.deliveryId,
      label: d.deliveryAddress,
      sequence: 1,
    );
  }

  factory ShipperMapMarker.fromRouteStop(ShipperRouteStopModel stop) {
    return ShipperMapMarker(
      latitude: stop.latitude,
      longitude: stop.longitude,
      sequence: stop.sequence,
      deliveryId: stop.deliveryId,
      label: stop.label,
    );
  }
}
