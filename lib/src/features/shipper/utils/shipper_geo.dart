/// Ước lượng tọa độ từ địa chỉ giao (BE chưa trả lat/lng) — phục vụ API tối ưu tuyến.
class ShipperGeoPoint {
  final double latitude;
  final double longitude;

  const ShipperGeoPoint(this.latitude, this.longitude);
}

/// Trung tâm khu vực TP.HCM (gần HUIT) + offset nhỏ theo hash địa chỉ.
ShipperGeoPoint approximateCoordsForAddress(String address, {int seed = 0}) {
  const baseLat = 10.7769;
  const baseLng = 106.7009;
  final h = address.hashCode.abs() + seed * 997;
  final dLat = ((h % 200) - 100) / 10000.0;
  final dLng = (((h ~/ 200) % 200) - 100) / 10000.0;
  return ShipperGeoPoint(baseLat + dLat, baseLng + dLng);
}

/// Vị trí mặc định khi shipper chưa bật GPS.
const ShipperGeoPoint kShipperDefaultStart = ShipperGeoPoint(10.7769, 106.7009);
