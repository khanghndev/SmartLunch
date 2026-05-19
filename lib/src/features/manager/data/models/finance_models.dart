/// Models Finance — khớp DTO backend `/api/v1/finance/...`.

// ─── CashFlow ────────────────────────────────────────────────────────────────

class CashFlowItemModel {
  final String periodKey;
  final DateTime? periodStart;
  final double collectedPayments;
  final double transactionIncome;
  final double transactionExpense;
  final double netFlow;

  const CashFlowItemModel({
    required this.periodKey,
    this.periodStart,
    this.collectedPayments = 0,
    this.transactionIncome = 0,
    this.transactionExpense = 0,
    this.netFlow = 0,
  });

  String get label => periodKey;

  double get income => collectedPayments + transactionIncome;

  double get expense => transactionExpense;

  double get profit => netFlow != 0 ? netFlow : income - expense;

  factory CashFlowItemModel.fromJson(Map<String, dynamic> json) =>
      CashFlowItemModel(
        periodKey: json['periodKey']?.toString() ?? json['label']?.toString() ?? '',
        periodStart: _parseDate(json['periodStart']),
        collectedPayments: _num(json['collectedPayments']),
        transactionIncome: _num(json['transactionIncome'] ?? json['income']),
        transactionExpense: _num(json['transactionExpense'] ?? json['expense']),
        netFlow: _num(json['netFlow']),
      );
}

class CashFlowSummaryModel {
  final DateTime? from;
  final DateTime? to;
  final String granularity;
  final double totalCollectedPayments;
  final double totalTransactionIncome;
  final double totalTransactionExpense;
  final double totalNet;
  final List<CashFlowItemModel> items;

  const CashFlowSummaryModel({
    this.from,
    this.to,
    this.granularity = '',
    this.totalCollectedPayments = 0,
    this.totalTransactionIncome = 0,
    this.totalTransactionExpense = 0,
    this.totalNet = 0,
    this.items = const [],
  });

  double get totalIncome => totalCollectedPayments + totalTransactionIncome;

  double get totalExpense => totalTransactionExpense;

  double get totalProfit => totalNet != 0 ? totalNet : totalIncome - totalExpense;

  factory CashFlowSummaryModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final buckets = data['buckets'] as List? ?? data['items'] as List? ?? [];
    return CashFlowSummaryModel(
      from: _parseDate(data['from']),
      to: _parseDate(data['to']),
      granularity: data['granularity']?.toString() ?? '',
      totalCollectedPayments: _num(data['totalCollectedPayments'] ?? data['totalIncome']),
      totalTransactionIncome: _num(data['totalTransactionIncome']),
      totalTransactionExpense: _num(data['totalTransactionExpense'] ?? data['totalExpense']),
      totalNet: _num(data['totalNet']),
      items: buckets
          .whereType<Map<String, dynamic>>()
          .map(CashFlowItemModel.fromJson)
          .toList(),
    );
  }
}

class PaymentTransactionModel {
  final String id;
  final String description;
  final double amount;
  final bool isIncome;
  final String category;
  final String date;
  final String method;
  final String status;
  final String source;

  const PaymentTransactionModel({
    required this.id,
    required this.description,
    required this.amount,
    required this.isIncome,
    required this.category,
    required this.date,
    required this.method,
    this.status = '',
    this.source = '',
  });

  factory PaymentTransactionModel.fromJson(Map<String, dynamic> json) {
    final source = json['source']?.toString() ?? '';
    final isCustomer = source.toLowerCase() == 'customer';
    final org = json['organizationName']?.toString();
    final partner = json['partnerLegalName']?.toString();
    final desc = org?.isNotEmpty == true
        ? org!
        : (partner?.isNotEmpty == true
            ? partner!
            : (json['description']?.toString() ?? 'Giao dịch'));
    final dateRaw = json['paymentDate'] ?? json['date'];
    return PaymentTransactionModel(
      id: json['entryId']?.toString() ?? json['id']?.toString() ?? '',
      description: desc,
      amount: _num(json['amount']),
      isIncome: json['isIncome'] as bool? ?? isCustomer,
      category: json['contractNumber']?.toString() ?? source,
      date: dateRaw?.toString() ?? '',
      method: json['method']?.toString() ?? 'Khác',
      status: json['status']?.toString() ?? '',
      source: source,
    );
  }
}

// ─── Reconciliation ───────────────────────────────────────────────────────────

enum ReconciliationStatus { pending, matched, disputed }

extension ReconciliationStatusX on ReconciliationStatus {
  String get label {
    switch (this) {
      case ReconciliationStatus.pending:
        return 'Chờ đối soát';
      case ReconciliationStatus.matched:
        return 'Khớp';
      case ReconciliationStatus.disputed:
        return 'Lệch';
    }
  }

  static ReconciliationStatus fromAligned(bool? aligned) {
    if (aligned == true) return ReconciliationStatus.matched;
    if (aligned == false) return ReconciliationStatus.disputed;
    return ReconciliationStatus.pending;
  }
}

class ReconciliationItemModel {
  final String id;
  final String code;
  final String orgName;
  final double amount;
  final double paidAmount;
  final double difference;
  final String period;
  final ReconciliationStatus status;
  final String dueDate;
  final String? issue;

  const ReconciliationItemModel({
    required this.id,
    required this.code,
    required this.orgName,
    required this.amount,
    this.paidAmount = 0,
    this.difference = 0,
    required this.period,
    required this.status,
    required this.dueDate,
    this.issue,
  });

  factory ReconciliationItemModel.fromJson(Map<String, dynamic> json) =>
      ReconciliationItemModel(
        id: json['orderId']?.toString() ?? json['id']?.toString() ?? '',
        code: json['code']?.toString() ?? 'ĐH-${json['orderId']}',
        orgName: json['organizationName']?.toString() ?? json['orgName']?.toString() ?? '',
        amount: _num(json['orderTotal'] ?? json['amount']),
        paidAmount: _num(json['paidAmount']),
        difference: _num(json['difference']),
        period: json['scheduledDate']?.toString() ?? json['period']?.toString() ?? '',
        status: ReconciliationStatusX.fromAligned(json['isAligned'] as bool?),
        dueDate: json['dueDate']?.toString() ?? '',
        issue: json['issue']?.toString(),
      );
}

class PaymentReconciliationModel {
  final DateTime? from;
  final DateTime? to;
  final int orderCount;
  final int mismatchCount;
  final List<ReconciliationItemModel> items;

  const PaymentReconciliationModel({
    this.from,
    this.to,
    this.orderCount = 0,
    this.mismatchCount = 0,
    this.items = const [],
  });

  double get totalOutstanding =>
      items.fold<double>(0, (a, b) => a + b.difference.abs());

  factory PaymentReconciliationModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['lines'] as List? ?? data['items'] as List? ?? data['invoices'] as List? ?? [];
    return PaymentReconciliationModel(
      from: _parseDate(data['from']),
      to: _parseDate(data['to']),
      orderCount: data['orderCount'] as int? ?? 0,
      mismatchCount: data['mismatchCount'] as int? ?? 0,
      items: list
          .whereType<Map<String, dynamic>>()
          .map(ReconciliationItemModel.fromJson)
          .toList(),
    );
  }
}

// ─── Organization Receivables ─────────────────────────────────────────────────

class OrganizationReceivableModel {
  final int organizationId;
  final String organizationName;
  final int orderCount;
  final double totalBilled;
  final double totalPaid;
  final double totalReceivable;
  final double overdue;
  final String lastPaymentDate;

  const OrganizationReceivableModel({
    required this.organizationId,
    required this.organizationName,
    this.orderCount = 0,
    this.totalBilled = 0,
    this.totalPaid = 0,
    required this.totalReceivable,
    this.overdue = 0,
    required this.lastPaymentDate,
  });

  factory OrganizationReceivableModel.fromJson(Map<String, dynamic> json) =>
      OrganizationReceivableModel(
        organizationId: json['organizationId'] as int? ?? 0,
        organizationName: json['organizationName']?.toString() ?? '',
        orderCount: json['orderCount'] as int? ?? 0,
        totalBilled: _num(json['totalBilled']),
        totalPaid: _num(json['totalPaid']),
        totalReceivable: _num(json['outstanding'] ?? json['totalReceivable']),
        overdue: _num(json['overdue']),
        lastPaymentDate: json['lastPaymentDate']?.toString() ?? '',
      );
}

class OrganizationReceivablesModel {
  final double grandTotal;
  final List<OrganizationReceivableModel> lines;

  const OrganizationReceivablesModel({
    this.grandTotal = 0,
    this.lines = const [],
  });

  factory OrganizationReceivablesModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['lines'] as List? ?? [];
    return OrganizationReceivablesModel(
      grandTotal: _num(data['grandTotalOutstanding']),
      lines: list
          .whereType<Map<String, dynamic>>()
          .map(OrganizationReceivableModel.fromJson)
          .toList(),
    );
  }
}

// ─── Supplier Payables ────────────────────────────────────────────────────────

class SupplierPayableModel {
  final int partnerId;
  final String partnerName;
  final double totalContractValue;
  final double totalPaid;
  final double outstanding;

  const SupplierPayableModel({
    required this.partnerId,
    required this.partnerName,
    this.totalContractValue = 0,
    this.totalPaid = 0,
    this.outstanding = 0,
  });

  factory SupplierPayableModel.fromJson(Map<String, dynamic> json) =>
      SupplierPayableModel(
        partnerId: json['partnerId'] as int? ?? 0,
        partnerName: json['partnerLegalName']?.toString() ?? json['partnerName']?.toString() ?? '',
        totalContractValue: _num(json['totalContractValue']),
        totalPaid: _num(json['totalPaid']),
        outstanding: _num(json['outstanding']),
      );
}

class SupplierPayablesModel {
  final double grandTotal;
  final List<SupplierPayableModel> lines;

  const SupplierPayablesModel({
    this.grandTotal = 0,
    this.lines = const [],
  });

  factory SupplierPayablesModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['lines'] as List? ?? [];
    return SupplierPayablesModel(
      grandTotal: _num(data['grandTotalOutstanding']),
      lines: list
          .whereType<Map<String, dynamic>>()
          .map(SupplierPayableModel.fromJson)
          .toList(),
    );
  }
}

// ─── Payment History ─────────────────────────────────────────────────────────

class FinancePaymentHistoryModel {
  final List<PaymentTransactionModel> items;
  final int totalCount;
  final int page;
  final int pageSize;

  const FinancePaymentHistoryModel({
    this.items = const [],
    this.totalCount = 0,
    this.page = 1,
    this.pageSize = 20,
  });

  factory FinancePaymentHistoryModel.fromJson(Map<String, dynamic> json) {
    final data = json['data'] as Map<String, dynamic>? ?? json;
    final list = data['entries'] as List? ?? data['items'] as List? ?? data['data'] as List? ?? [];
    final items = list
        .whereType<Map<String, dynamic>>()
        .map(PaymentTransactionModel.fromJson)
        .toList();
    return FinancePaymentHistoryModel(
      items: items,
      totalCount: data['totalCount'] as int? ?? items.length,
      page: data['page'] as int? ?? 1,
      pageSize: data['pageSize'] as int? ?? 20,
    );
  }
}

double _num(dynamic v) {
  if (v is num) return v.toDouble();
  return double.tryParse('$v') ?? 0;
}

DateTime? _parseDate(dynamic v) {
  if (v == null) return null;
  if (v is DateTime) return v;
  return DateTime.tryParse(v.toString());
}
