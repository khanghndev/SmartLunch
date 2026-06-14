import 'package:flutter/foundation.dart';

class OrgOrderListModel {
  final int page;
  final int pageSize;
  final int totalCount;
  final List<OrgOrderModel> items;

  OrgOrderListModel({
    this.page = 1,
    this.pageSize = 20,
    this.totalCount = 0,
    this.items = const [],
  });

  factory OrgOrderListModel.fromJson(Map<String, dynamic> json) {
    var rawData = json;
    if (json.containsKey('data') && json['data'] is Map) {
      rawData = json['data'];
    }

    final dataList = rawData['data'] as List? ?? [];
    return OrgOrderListModel(
      page: rawData['page'] as int? ?? 1,
      pageSize: rawData['pageSize'] as int? ?? 20,
      totalCount: rawData['totalCount'] as int? ?? 0,
      items: dataList.map((x) => OrgOrderModel.fromJson(x)).toList(),
    );
  }
}

class OrgOrderModel {
  final int id;
  final int? contractId;
  final String? invoiceCode;
  final DateTime? orderDate;
  final DateTime? scheduledDate;
  final String status;
  final String paymentStatus;
  final double totalAmount;
  final String? annexPdfUrl;
  final DateTime? annexSignedAt;

  OrgOrderModel({
    required this.id,
    this.contractId,
    this.invoiceCode,
    this.orderDate,
    this.scheduledDate,
    required this.status,
    required this.paymentStatus,
    required this.totalAmount,
    this.annexPdfUrl,
    this.annexSignedAt,
  });

  factory OrgOrderModel.fromJson(Map<String, dynamic> json) {
    return OrgOrderModel(
      id: json['id'] as int? ?? 0,
      contractId: json['contractId'] as int?,
      invoiceCode: json['invoiceCode'] as String?,
      orderDate: json['orderDate'] != null ? DateTime.tryParse(json['orderDate'].toString()) : null,
      scheduledDate: json['scheduledDate'] != null ? DateTime.tryParse(json['scheduledDate'].toString()) : null,
      status: json['status'] as String? ?? 'Pending',
      paymentStatus: json['paymentStatus'] as String? ?? 'Unpaid',
      totalAmount: (json['totalAmount'] as num?)?.toDouble() ?? 0.0,
      annexPdfUrl: json['annexPdfUrl'] as String?,
      annexSignedAt: json['annexSignedAt'] != null ? DateTime.tryParse(json['annexSignedAt'].toString()) : null,
    );
  }

  /// Trả về true nếu là đơn thuộc hợp đồng đặt theo tuần
  bool get isWeeklyFulfillmentOrder {
    if (invoiceCode == null || invoiceCode!.isEmpty) return false;
    return invoiceCode!.toUpperCase().contains('PRW') && (contractId != null && contractId! > 0);
  }

  String get displayStatus {
    switch (status.toLowerCase()) {
      case 'draft':
        return 'Nháp';
      case 'pending':
      case 'placed':
        return 'Chờ xác nhận';
      case 'confirmed':
        return 'Đã xác nhận';
      case 'preparing':
        return 'Đang chuẩn bị';
      case 'delivering':
      case 'out_for_delivery':
        return 'Đang giao';
      case 'delivered':
      case 'completed':
        return 'Hoàn thành';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return status;
    }
  }

  String get displayPaymentStatus {
    switch (paymentStatus.toLowerCase()) {
      case 'unpaid':
      case 'awaiting_payment':
        return 'Chưa thanh toán';
      case 'deposit_paid':
      case 'partial':
        return 'Đã đặt cọc';
      case 'paid':
        return 'Đã thanh toán';
      case 'refunded':
        return 'Đã hoàn tiền';
      default:
        return paymentStatus;
    }
  }

  String get typeLabel {
    if (invoiceCode?.startsWith('BO') == true) {
      return 'Suất ăn tập trung';
    } else if (invoiceCode?.startsWith('PBO') == true || contractId != null) {
      return 'Hợp đồng theo kỳ';
    }
    return 'Khách hàng doanh nghiệp';
  }
}
