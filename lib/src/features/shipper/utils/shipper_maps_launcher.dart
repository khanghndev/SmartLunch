import 'package:flutter/foundation.dart';
import 'package:url_launcher/url_launcher.dart';

import 'shipper_geo.dart';

/// Mở Google Maps (app hoặc web) để chỉ đường / xem điểm giao.
abstract final class ShipperMapsLauncher {
  static Future<bool> openDirections({
    required double latitude,
    required double longitude,
    String? label,
  }) async {
    final uri = Uri.parse(
      'https://www.google.com/maps/dir/?api=1&destination=$latitude,$longitude&travelmode=driving',
    );
    final query = label != null && label.isNotEmpty
        ? Uri.encodeComponent(label)
        : '$latitude,$longitude';
    final searchUri = Uri.parse(
      'https://www.google.com/maps/search/?api=1&query=$query',
    );
    if (await canLaunchUrl(uri)) {
      return launchUrl(uri, mode: LaunchMode.externalApplication);
    }
    return launchUrl(searchUri, mode: LaunchMode.externalApplication);
  }

  static Future<bool> openAddress(String address, {int seed = 0}) async {
    final geo = approximateCoordsForAddress(address, seed: seed);
    return openDirections(
      latitude: geo.latitude,
      longitude: geo.longitude,
      label: address,
    );
  }

  static Future<bool> openMultiStopRoute({
    required List<ShipperGeoPoint> stops,
    ShipperGeoPoint? origin,
  }) async {
    if (stops.isEmpty) return false;
    if (stops.length == 1) {
      return openDirections(
        latitude: stops.first.latitude,
        longitude: stops.first.longitude,
      );
    }
    final start = origin ?? kShipperDefaultStart;
    final dest = stops.last;
    final waypoints = stops.length > 2
        ? stops.sublist(0, stops.length - 1).map((p) => '${p.latitude},${p.longitude}').join('|')
        : null;
    var url =
        'https://www.google.com/maps/dir/?api=1&origin=${start.latitude},${start.longitude}'
        '&destination=${dest.latitude},${dest.longitude}&travelmode=driving';
    if (waypoints != null && waypoints.isNotEmpty) {
      url += '&waypoints=$waypoints';
    }
    final uri = Uri.parse(url);
    try {
      if (await canLaunchUrl(uri)) {
        return launchUrl(uri, mode: LaunchMode.externalApplication);
      }
    } catch (e) {
      debugPrint('openMultiStopRoute: $e');
    }
    return openDirections(latitude: dest.latitude, longitude: dest.longitude);
  }
}
