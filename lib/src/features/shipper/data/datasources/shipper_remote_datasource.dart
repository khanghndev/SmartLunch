import 'package:http/http.dart' as http;

import '../../../../core/network/api_client.dart';
import '../../../../core/network/api_exception.dart';
import '../models/shipper_delivery_models.dart';

class ShipperRemoteDataSource {
  ShipperRemoteDataSource(this._apiClient);

  final ApiClient _apiClient;

  Future<ShipperDeliveriesPageModel> getDeliveries({
    int page = 1,
    int pageSize = 50,
    String? status,
    DateTime? scheduledOn,
  }) async {
    try {
      final q = <String, dynamic>{
        'page': page,
        'pageSize': pageSize,
      };
      if (status != null && status.isNotEmpty) {
        q['status'] = status;
      }
      if (scheduledOn != null) {
        final d = scheduledOn.toUtc();
        q['scheduledOn'] =
            '${d.year.toString().padLeft(4, '0')}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';
      }

      final response = await _apiClient.get(
        '/api/v1/shipper/deliveries',
        queryParameters: q,
      );
      final data = response['data'] as Map<String, dynamic>?;
      if (data == null) {
        throw ApiException('Không có dữ liệu đơn giao', statusCode: 200);
      }
      return ShipperDeliveriesPageModel.fromJson(data);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải danh sách giao: $e', statusCode: 500);
    }
  }

  Future<ShipperDeliveryDetailModel> getDelivery(int id) async {
    try {
      final response = await _apiClient.get(
        '/api/v1/shipper/deliveries/$id',
        queryParameters: const {},
      );
      final data = response['data'] as Map<String, dynamic>?;
      final delivery = data?['delivery'] as Map<String, dynamic>?;
      if (delivery == null) {
        throw ApiException('Không tìm thấy đơn giao', statusCode: 404);
      }
      return ShipperDeliveryDetailModel.fromJson(delivery);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải chi tiết đơn: $e', statusCode: 500);
    }
  }

  Future<ShipperDeliveryDetailModel> updateDeliveryStatus(
    int id, {
    required String status,
    String? notes,
  }) async {
    try {
      final response = await _apiClient.patch(
        '/api/v1/shipper/deliveries/$id/status',
        body: {
          'status': status,
          if (notes != null && notes.isNotEmpty) 'notes': notes,
        },
      );
      final data = response['data'] as Map<String, dynamic>?;
      final delivery = data?['delivery'] as Map<String, dynamic>?;
      if (delivery == null) {
        throw ApiException('Phản hồi không hợp lệ', statusCode: 200);
      }
      return ShipperDeliveryDetailModel.fromJson(delivery);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi cập nhật trạng thái: $e', statusCode: 500);
    }
  }

  /// `POST .../deliveries/{id}/proof` — multipart: `file`, `signature`,
  /// `RecipientConfirmedName`, optional `notes`.
  Future<ShipperDeliveryDetailModel> uploadDeliveryProof(
    int deliveryId, {
    required List<int> imageBytes,
    required String filename,
    required String contentType,
    required List<int> signatureBytes,
    required String recipientConfirmedName,
    String? notes,
  }) async {
    try {
      final file = ApiClient.multipartImageBytes(
        fieldName: 'file',
        bytes: imageBytes,
        filename: filename,
        contentType: contentType,
      );
      final signatureFile = ApiClient.multipartImageBytes(
        fieldName: 'signature',
        bytes: signatureBytes,
        filename: 'recipient-signature.png',
        contentType: 'image/png',
      );
      final fields = <String, String>{
        'RecipientConfirmedName': recipientConfirmedName.trim(),
      };
      if (notes != null && notes.trim().isNotEmpty) {
        fields['notes'] = notes.trim();
      }
      final response = await _apiClient.postMultipart(
        '/api/v1/shipper/deliveries/$deliveryId/proof',
        files: <http.MultipartFile>[file, signatureFile],
        fields: fields,
      );
      final data = response['data'] as Map<String, dynamic>?;
      final delivery = data?['delivery'] as Map<String, dynamic>?;
      if (delivery == null) {
        throw ApiException('Phản hồi không hợp lệ', statusCode: 200);
      }
      return ShipperDeliveryDetailModel.fromJson(delivery);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tải ảnh minh chứng: $e', statusCode: 500);
    }
  }

  Future<ShipperRouteOptimizeResponseModel> optimizeRoute({
    required double startLat,
    required double startLng,
    required List<ShipperRouteStopInput> stops,
  }) async {
    try {
      final response = await _apiClient.post(
        '/api/v1/shipper/routes/optimize',
        body: {
          'start': {
            'latitude': startLat,
            'longitude': startLng,
          },
          'stops': stops.map((s) => s.toJson()).toList(),
        },
      );
      final data = response['data'] as Map<String, dynamic>? ?? response;
      return ShipperRouteOptimizeResponseModel.fromJson(data);
    } on ApiException {
      rethrow;
    } catch (e) {
      throw ApiException('Lỗi tối ưu hóa tuyến đường: $e', statusCode: 500);
    }
  }
}
