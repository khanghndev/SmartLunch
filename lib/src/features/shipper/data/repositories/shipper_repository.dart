import '../../../../core/config/app_env.dart';
import '../../../../core/network/api_client.dart';
import '../datasources/shipper_remote_datasource.dart';
import '../models/shipper_delivery_models.dart';

class ShipperRepository {
  ShipperRepository._({ShipperRemoteDataSource? remote})
      : _remote = remote ??
            ShipperRemoteDataSource(ApiClient(baseUrl: AppEnv.apiBaseUrl));

  static final ShipperRepository instance = ShipperRepository._();

  final ShipperRemoteDataSource _remote;

  Future<ShipperDeliveriesPageModel> getDeliveries({
    int page = 1,
    int pageSize = 50,
    String? status,
    DateTime? scheduledOn,
  }) =>
      _remote.getDeliveries(
        page: page,
        pageSize: pageSize,
        status: status,
        scheduledOn: scheduledOn,
      );

  Future<ShipperDeliveryDetailModel> getDelivery(int id) =>
      _remote.getDelivery(id);

  Future<ShipperDeliveryDetailModel> updateDeliveryStatus(
    int id, {
    required String status,
    String? notes,
  }) =>
      _remote.updateDeliveryStatus(id, status: status, notes: notes);

  Future<ShipperDeliveryDetailModel> uploadDeliveryProof(
    int id, {
    required List<int> imageBytes,
    required String filename,
    required String contentType,
    required List<int> signatureBytes,
    required String recipientConfirmedName,
    String? notes,
  }) =>
      _remote.uploadDeliveryProof(
        id,
        imageBytes: imageBytes,
        filename: filename,
        contentType: contentType,
        signatureBytes: signatureBytes,
        recipientConfirmedName: recipientConfirmedName,
        notes: notes,
      );

  Future<ShipperRouteOptimizeResponseModel> optimizeRoute({
    required double startLat,
    required double startLng,
    required List<ShipperRouteStopInput> stops,
  }) =>
      _remote.optimizeRoute(
        startLat: startLat,
        startLng: startLng,
        stops: stops,
      );
}
