/// Models cho Organization: Contracts và Payments.
/// Khớp endpoint `/api/v1/company/contracts/...` trong `UI_MOB.md`.

// ─── Contract ─────────────────────────────────────────────────────────────────

enum ContractStatus { draft, active, signed, expired }

extension ContractStatusX on ContractStatus {
  String get label {
    switch (this) {
      case ContractStatus.draft:   return 'Nháp';
      case ContractStatus.active:  return 'Đang hiệu lực';
      case ContractStatus.signed:  return 'Đã ký';
      case ContractStatus.expired: return 'Hết hạn';
    }
  }

  static ContractStatus fromString(String? s) {
    switch (s?.toLowerCase()) {
      case 'active':  return ContractStatus.active;
      case 'signed':  return ContractStatus.signed;
      case 'expired': return ContractStatus.expired;
      default:        return ContractStatus.draft;
    }
  }
}

class ContractModel {
  final int id;
  final String code;          // số hợp đồng
  final String organizationName;
  final String startDate;
  final String endDate;
  final double value;
  final String type;          // loại hợp đồng (văn phòng, trường học, ...)
  final ContractStatus status;

  const ContractModel({
    required this.id,
    required this.code,
    required this.organizationName,
    required this.startDate,
    required this.endDate,
    required this.value,
    required this.type,
    required this.status,
  });

  factory ContractModel.fromJson(Map<String, dynamic> json) => ContractModel(
        id: json['id'] as int? ?? 0,
        code: json['contractNumber']?.toString() ??
            json['code']?.toString() ??
            json['contractCode']?.toString() ??
            '#CTR-${json['id'] ?? 0}',
        organizationName:
            json['organizationName']?.toString() ??
                json['orgName']?.toString() ??
                '',
        startDate: json['startDate']?.toString() ?? '',
        endDate: json['endDate']?.toString() ?? '',
        value: (json['totalValue'] as num?)?.toDouble() ??
            (json['value'] as num?)?.toDouble() ??
            (json['contractValue'] as num?)?.toDouble() ??
            0,
        type: json['description']?.toString() ??
            json['type']?.toString() ??
            json['contractType']?.toString() ??
            '',
        status: ContractStatusX.fromString(json['status']?.toString()),
      );
}

// ─── Payment ──────────────────────────────────────────────────────────────────

enum PaymentStatus { pending, success, failed }

extension PaymentStatusX on PaymentStatus {
  String get label {
    switch (this) {
      case PaymentStatus.pending: return 'Chờ xử lý';
      case PaymentStatus.success: return 'Thành công';
      case PaymentStatus.failed:  return 'Thất bại';
    }
  }

  static PaymentStatus fromString(String? s) {
    switch (s?.toLowerCase()) {
      case 'success': return PaymentStatus.success;
      case 'failed':  return PaymentStatus.failed;
      default:        return PaymentStatus.pending;
    }
  }
}

class ContractPaymentModel {
  final String id;
  final String description;
  final double amount;
  final String date;
  final String method;
  final PaymentStatus status;
  final String? payosUrl;    // link PayOS nếu có

  const ContractPaymentModel({
    required this.id,
    required this.description,
    required this.amount,
    required this.date,
    required this.method,
    required this.status,
    this.payosUrl,
  });

  factory ContractPaymentModel.fromJson(Map<String, dynamic> json) =>
      ContractPaymentModel(
        id: json['id']?.toString() ?? '',
        description: json['description']?.toString() ??
            json['note']?.toString() ?? '',
        amount: (json['amount'] as num?)?.toDouble() ?? 0,
        date: json['date']?.toString() ?? json['createdAt']?.toString() ?? '',
        method: json['method']?.toString() ??
            json['paymentMethod']?.toString() ?? 'Khác',
        status: PaymentStatusX.fromString(json['status']?.toString()),
        payosUrl: json['payosUrl']?.toString() ??
            json['paymentUrl']?.toString(),
      );
}

// ─── Page wrappers ───────────────────────────────────────────────────────────

class ContractListModel {
  final List<ContractModel> items;
  final int totalCount;

  const ContractListModel({this.items = const [], this.totalCount = 0});

  factory ContractListModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['contracts'] as List? ??
        data['data'] as List? ??
        data['items'] as List? ??
        [];
    final items = list
        .whereType<Map<String, dynamic>>()
        .map(ContractModel.fromJson)
        .toList();
    return ContractListModel(
      items: items,
      totalCount: data['totalCount'] as int? ?? items.length,
    );
  }
}

/// Thanh toán / đối soát theo hợp đồng — `GET .../finance/contracts/{id}/payments`
/// hoặc `GET .../company/contracts/{id}/payments`.
class ContractFinancePaymentsModel {
  final List<ContractPaymentModel> customerOrderLines;
  final List<ContractPaymentModel> supplierLines;
  final double totalPaid;
  final double totalPending;
  final double outstandingReceivable;

  const ContractFinancePaymentsModel({
    this.customerOrderLines = const [],
    this.supplierLines = const [],
    this.totalPaid = 0,
    this.totalPending = 0,
    this.outstandingReceivable = 0,
  });

  List<ContractPaymentModel> get allLines => [
        ...customerOrderLines,
        ...supplierLines,
      ];

  factory ContractFinancePaymentsModel.fromJson(Map<String, dynamic> json) {
    final root = json['data'] as Map<String, dynamic>? ?? json;
    final customer = root['customer'] as Map<String, dynamic>? ?? {};
    final supplier = root['supplier'] as Map<String, dynamic>? ?? {};

    final orders = customer['orders'] as List? ?? [];
    final customerLines = orders.whereType<Map<String, dynamic>>().map((o) {
      final orderId = o['orderId'] as int? ?? 0;
      final paid = (o['paidAmount'] as num?)?.toDouble() ?? 0;
      final pending = (o['pendingPaymentAmount'] as num?)?.toDouble() ?? 0;
      final total = (o['orderTotal'] as num?)?.toDouble() ?? 0;
      final statusRaw = o['recordedPaymentStatus']?.toString() ?? '';
      PaymentStatus status;
      if (statusRaw.toLowerCase().contains('paid') || paid >= total && total > 0) {
        status = PaymentStatus.success;
      } else if (statusRaw.toLowerCase().contains('fail')) {
        status = PaymentStatus.failed;
      } else {
        status = PaymentStatus.pending;
      }
      final scheduled = o['scheduledDate']?.toString() ?? '';
      return ContractPaymentModel(
        id: 'order-$orderId',
        description: o['invoiceCode']?.toString().isNotEmpty == true
            ? 'Đơn ${o['invoiceCode']}'
            : 'Đơn hàng #$orderId',
        amount: pending > 0 ? pending : total,
        date: scheduled.length >= 10 ? scheduled.substring(0, 10) : scheduled,
        method: 'Đơn B2B',
        status: status,
      );
    }).toList();

    final payLines = supplier['paymentLines'] as List? ?? [];
    final supplierLines = payLines.whereType<Map<String, dynamic>>().map((p) {
      return ContractPaymentModel(
        id: '${p['id'] ?? ''}',
        description: p['code']?.toString().isNotEmpty == true
            ? 'Chi NCC: ${p['code']}'
            : 'Thanh toán NCC #${p['id']}',
        amount: (p['amount'] as num?)?.toDouble() ?? 0,
        date: p['paymentDate']?.toString() ?? '',
        method: p['method']?.toString() ?? 'Khác',
        status: PaymentStatusX.fromString(p['status']?.toString()),
      );
    }).toList();

    return ContractFinancePaymentsModel(
      customerOrderLines: customerLines,
      supplierLines: supplierLines,
      totalPaid: (customer['totalPaidFromPayments'] as num?)?.toDouble() ?? 0,
      totalPending: (customer['totalPendingFromPayments'] as num?)?.toDouble() ?? 0,
      outstandingReceivable:
          (customer['outstandingReceivable'] as num?)?.toDouble() ?? 0,
    );
  }
}

class ContractPaymentListModel {
  final List<ContractPaymentModel> items;
  final int totalCount;
  final ContractFinancePaymentsModel? finance;

  const ContractPaymentListModel({
    this.items = const [],
    this.totalCount = 0,
    this.finance,
  });

  factory ContractPaymentListModel.fromJson(Map<String, dynamic> json) {
    final finance = ContractFinancePaymentsModel.fromJson(json);
    final items = finance.allLines;
    return ContractPaymentListModel(
      items: items,
      totalCount: items.length,
      finance: finance,
    );
  }
}

/// Response khi tạo PayOS link.
class PayOSLinkModel {
  final String checkoutUrl;
  final String orderCode;
  final String status;

  const PayOSLinkModel({
    required this.checkoutUrl,
    required this.orderCode,
    required this.status,
  });

  factory PayOSLinkModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    return PayOSLinkModel(
      checkoutUrl: data['checkoutUrl']?.toString() ??
          data['paymentUrl']?.toString() ?? '',
      orderCode: data['orderCode']?.toString() ?? '',
      status: data['status']?.toString() ?? '',
    );
  }
}
