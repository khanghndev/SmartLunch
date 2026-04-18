import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class ReconciliationPage extends StatelessWidget {
  const ReconciliationPage({super.key});

  @override
  Widget build(BuildContext context) {
    final invoices = const [
      _Invoice(
        code: '#INV-0324-01',
        amount: '12.500.000đ',
        period: '01 - 15/03/2024',
        status: InvoiceStatus.pending,
      ),
      _Invoice(
        code: '#INV-0224-02',
        amount: '10.200.000đ',
        period: '16 - 29/02/2024',
        status: InvoiceStatus.processing,
      ),
      _Invoice(
        code: '#INV-0224-01',
        amount: '9.850.000đ',
        period: '01 - 15/02/2024',
        status: InvoiceStatus.paid,
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Đối soát & thanh toán'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.help_outline)),
        ],
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            const _BalanceCard(),
            const SizedBox(height: 14),
            _InvoiceList(invoices: invoices),
            const SizedBox(height: 14),
            const _PaymentInfo(),
            const SizedBox(height: 14),
            const _SupportCard(),
          ],
        ),
      ),
    );
  }
}

class _BalanceCard extends StatelessWidget {
  const _BalanceCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.org, AppColors.orgAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.org.withOpacity(0.22),
            blurRadius: 16,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.16),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(
                  Icons.account_balance_wallet_rounded,
                  color: Colors.white,
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  'Công nợ hiện tại',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    color: Colors.white,
                    fontWeight: FontWeight.w900,
                    letterSpacing: -0.2,
                  ),
                ),
              ),
              TextButton(
                style: TextButton.styleFrom(foregroundColor: Colors.white),
                onPressed: () {},
                child: const Text('Lịch sử'),
              ),
            ],
          ),
          const SizedBox(height: 12),
          const Text(
            '22.700.000đ',
            style: TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.w900,
              fontSize: 28,
              letterSpacing: -0.3,
            ),
          ),
          const SizedBox(height: 6),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.18),
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.white.withOpacity(0.25)),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: const [
                Icon(Icons.schedule, color: Colors.white, size: 16),
                SizedBox(width: 6),
                Text(
                  'Hạn thanh toán: 05/04',
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
          ElevatedButton.icon(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.white,
              foregroundColor: AppColors.org,
              padding: const EdgeInsets.symmetric(vertical: 12),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onPressed: () {},
            icon: const Icon(Icons.verified_outlined),
            label: const Text(
              'Xác nhận đã chuyển khoản',
              style: TextStyle(fontWeight: FontWeight.w800),
            ),
          ),
        ],
      ),
    );
  }
}

enum InvoiceStatus { pending, processing, paid }

class _Invoice {
  final String code;
  final String amount;
  final String period;
  final InvoiceStatus status;

  const _Invoice({
    required this.code,
    required this.amount,
    required this.period,
    required this.status,
  });
}

class _InvoiceList extends StatelessWidget {
  final List<_Invoice> invoices;

  const _InvoiceList({required this.invoices});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.receipt_long_rounded, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Hóa đơn đối soát',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Tải tất cả')),
            ],
          ),
          const SizedBox(height: 8),
          ...invoices.map(
            (invoice) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: _InvoiceTile(invoice: invoice),
            ),
          ),
        ],
      ),
    );
  }
}

class _InvoiceTile extends StatelessWidget {
  final _Invoice invoice;

  const _InvoiceTile({required this.invoice});

  Color _statusColor() {
    switch (invoice.status) {
      case InvoiceStatus.pending:
        return AppColors.org;
      case InvoiceStatus.processing:
        return AppColors.orgAlt;
      case InvoiceStatus.paid:
        return AppColors.org;
    }
  }

  String _statusLabel() {
    switch (invoice.status) {
      case InvoiceStatus.pending:
        return 'Chờ thanh toán';
      case InvoiceStatus.processing:
        return 'Đang đối soát';
      case InvoiceStatus.paid:
        return 'Đã thanh toán';
    }
  }

  @override
  Widget build(BuildContext context) {
    final color = _statusColor();
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppColors.ink.withOpacity(0.01),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.ink.withOpacity(0.05)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: color.withOpacity(0.12),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(Icons.picture_as_pdf_rounded, color: color),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: Text(
                        invoice.code,
                        style: const TextStyle(
                          color: AppColors.ink,
                          fontWeight: FontWeight.w900,
                          fontSize: 16,
                        ),
                      ),
                    ),
                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 8,
                        vertical: 4,
                      ),
                      decoration: BoxDecoration(
                        color: color.withOpacity(0.14),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        _statusLabel(),
                        style: TextStyle(
                          color: color,
                          fontWeight: FontWeight.w800,
                          fontSize: 12.5,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  'Kỳ: ${invoice.period}',
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  invoice.amount,
                  style: const TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w900,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 8),
          Column(
            children: [
              IconButton(
                onPressed: () {},
                icon: const Icon(Icons.download_rounded),
                color: AppColors.org,
              ),
              TextButton(onPressed: () {}, child: const Text('Xác nhận')),
            ],
          ),
        ],
      ),
    );
  }
}

class _PaymentInfo extends StatelessWidget {
  const _PaymentInfo();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 12,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.account_balance, color: AppColors.org),
              const SizedBox(width: 8),
              Text(
                'Thông tin chuyển khoản',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.ink,
                ),
              ),
              const Spacer(),
              TextButton(onPressed: () {}, child: const Text('Copy')),
            ],
          ),
          const SizedBox(height: 10),
          const _BankRow(
            label: 'Ngân hàng',
            value: 'Vietcombank · CN Thủ Thiêm',
          ),
          const _BankRow(label: 'Chủ tài khoản', value: 'CTY TNHH SMARTLUNCH'),
          const _BankRow(label: 'Số tài khoản', value: '0123 456 789'),
          const _BankRow(label: 'Nội dung', value: 'INV-0324-01 _ TÊN CÔNG TY'),
        ],
      ),
    );
  }
}

class _BankRow extends StatelessWidget {
  final String label;
  final String value;

  const _BankRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Expanded(
            child: Text(
              label,
              style: TextStyle(
                color: AppColors.ink.withOpacity(0.65),
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
          Text(
            value,
            style: const TextStyle(
              color: AppColors.ink,
              fontWeight: FontWeight.w800,
            ),
          ),
        ],
      ),
    );
  }
}

class _SupportCard extends StatelessWidget {
  const _SupportCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFECF8F4),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.org.withOpacity(0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: const BoxDecoration(
                  color: Colors.white,
                  shape: BoxShape.circle,
                ),
                child: const Icon(Icons.support_agent, color: AppColors.org),
              ),
              const SizedBox(width: 10),
              Text(
                'Cần hỗ trợ thanh toán?',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.org,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            'Liên hệ CSKH hoặc chatbot để được hỗ trợ nhanh chóng về hóa đơn, biên lai và chỉnh sửa nội dung thanh toán.',
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.75),
              fontWeight: FontWeight.w600,
              height: 1.3,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              ElevatedButton.icon(
                onPressed: () {},
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.org,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(
                    vertical: 12,
                    horizontal: 14,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                icon: const Icon(Icons.chat_bubble_outline),
                label: const Text(
                  'Chatbot',
                  style: TextStyle(fontWeight: FontWeight.w800),
                ),
              ),
              const SizedBox(width: 10),
              OutlinedButton.icon(
                onPressed: () {},
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppColors.org,
                  side: BorderSide(color: AppColors.org.withOpacity(0.4)),
                  padding: const EdgeInsets.symmetric(
                    vertical: 12,
                    horizontal: 14,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                icon: const Icon(Icons.call_rounded),
                label: const Text(
                  'Gọi CSKH',
                  style: TextStyle(fontWeight: FontWeight.w800),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
