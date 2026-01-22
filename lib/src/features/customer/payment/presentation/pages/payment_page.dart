import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class PaymentPage extends StatelessWidget {
  final bool embedded;
  final double bottomInset;

  const PaymentPage({
    super.key,
    this.embedded = false,
    this.bottomInset = 0,
  });

  @override
  Widget build(BuildContext context) {
    final transactions = [
      const _Transaction(
        title: 'Thanh toán đơn #SL1024',
        subtitle: 'Ví SmartLunch · 10:42 AM',
        amount: '-55.000đ',
        positive: false,
      ),
      const _Transaction(
        title: 'Nạp từ Vietcombank',
        subtitle: '***1234 · 09:10 AM',
        amount: '+200.000đ',
        positive: true,
      ),
      const _Transaction(
        title: 'Hoàn tiền đơn #SL0988',
        subtitle: 'Ví SmartLunch · Hôm qua',
        amount: '+45.000đ',
        positive: true,
      ),
    ];

    final content = SingleChildScrollView(
      physics: const BouncingScrollPhysics(),
      padding: EdgeInsets.fromLTRB(
          20, embedded ? 6 : 18, 20, 28 + bottomInset),
      child: Column(
        children: [
          const _BalanceCard(),
          const SizedBox(height: 16),
          const _PaymentActions(),
          const SizedBox(height: 18),
          const _MethodCard(),
          const SizedBox(height: 18),
          _TransactionsList(transactions: transactions),
        ],
      ),
    );

    if (embedded) {
      return content;
    }

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Thanh toán'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: content,
    );
  }
}

class _BalanceCard extends StatelessWidget {
  const _BalanceCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.customer, AppColors.customerAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: AppColors.customer.withOpacity(0.28),
            blurRadius: 18,
            offset: const Offset(0, 12),
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
                  color: Colors.white.withOpacity(0.2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.account_balance_wallet_rounded,
                    color: Colors.white, size: 22),
              ),
              const SizedBox(width: 10),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Ví SmartLunch',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.w800,
                        ),
                  ),
                  Text(
                    'Số dư khả dụng',
                    style: TextStyle(
                      color: Colors.white.withOpacity(0.85),
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            ],
          ),
          const SizedBox(height: 14),
          Text(
            '520.000đ',
            style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.w900,
                  letterSpacing: -0.2,
                ),
          ),
          const SizedBox(height: 10),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: const [
              _BalancePill(
                label: 'Đã khóa: 80.000đ',
                icon: Icons.lock_outline,
              ),
              _BalancePill(
                label: 'Ưu đãi: 2 voucher',
                icon: Icons.card_giftcard_outlined,
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _BalancePill extends StatelessWidget {
  final String label;
  final IconData icon;

  const _BalancePill({required this.label, required this.icon});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 7),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.2),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 15, color: Colors.white),
          const SizedBox(width: 6),
          Text(
            label,
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.w700,
              fontSize: 12.5,
            ),
          ),
        ],
      ),
    );
  }
}

class _PaymentActions extends StatelessWidget {
  const _PaymentActions();

  @override
  Widget build(BuildContext context) {
    final actions = [
      const _ActionButton(
        label: 'Nạp tiền',
        icon: Icons.add_rounded,
        color: AppColors.customer,
      ),
      const _ActionButton(
        label: 'Rút về ngân hàng',
        icon: Icons.south_west_rounded,
        color: Color(0xFF1F3C88),
      ),
      const _ActionButton(
        label: 'Chuyển / tặng',
        icon: Icons.swap_horiz_rounded,
        color: Color(0xFFE07A24),
      ),
    ];

    return Row(
      children: actions
          .asMap()
          .entries
          .map(
            (entry) => Expanded(
              child: Padding(
                padding: EdgeInsets.only(
                    right: entry.key == actions.length - 1 ? 0 : 10),
                child: _ActionTile(button: entry.value),
              ),
            ),
          )
          .toList(),
    );
  }
}

class _ActionButton {
  final String label;
  final IconData icon;
  final Color color;

  const _ActionButton({
    required this.label,
    required this.icon,
    required this.color,
  });
}

class _ActionTile extends StatelessWidget {
  final _ActionButton button;

  const _ActionTile({required this.button});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(14),
      elevation: 2,
      shadowColor: Colors.black.withOpacity(0.04),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: () {},
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 10),
          child: Column(
            children: [
              CircleAvatar(
                radius: 22,
                backgroundColor: button.color.withOpacity(0.12),
                child: Icon(button.icon, color: button.color, size: 22),
              ),
              const SizedBox(height: 8),
              Text(
                button.label,
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.8),
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _MethodCard extends StatelessWidget {
  const _MethodCard();

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
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: AppColors.customer.withOpacity(0.08),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.credit_card_rounded,
                    color: AppColors.customer, size: 20),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Phương thức thanh toán',
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.w800,
                            color: AppColors.ink,
                          ),
                    ),
                    Text(
                      'Chọn nguồn tiền ưu tiên khi thanh toán',
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(
                        color: AppColors.ink.withOpacity(0.6),
                        fontWeight: FontWeight.w600,
                        fontSize: 13,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          _MethodTile(
            title: 'Ví SmartLunch',
            subtitle: '520.000đ · Được ưu tiên sử dụng',
            selected: true,
            icon: Icons.account_balance_wallet_rounded,
          ),
          const SizedBox(height: 8),
          _MethodTile(
            title: 'Vietcombank · ***1234',
            subtitle: 'Miễn phí chuyển khoản nội bộ',
            selected: false,
            icon: Icons.account_balance_rounded,
          ),
          const SizedBox(height: 8),
          _MethodTile(
            title: 'Thêm thẻ / tài khoản',
            subtitle: 'Hỗ trợ thẻ ATM, Visa, Mastercard',
            selected: false,
            icon: Icons.add_card_rounded,
          ),
        ],
      ),
    );
  }
}

class _MethodTile extends StatelessWidget {
  final String title;
  final String subtitle;
  final bool selected;
  final IconData icon;

  const _MethodTile({
    required this.title,
    required this.subtitle,
    required this.selected,
    required this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: selected ? AppColors.customer.withOpacity(0.06) : Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: selected
              ? AppColors.customer.withOpacity(0.35)
              : AppColors.ink.withOpacity(0.05),
        ),
      ),
      child: Row(
        children: [
          CircleAvatar(
            radius: 20,
            backgroundColor: AppColors.customer.withOpacity(0.12),
            child: Icon(icon, color: AppColors.customer, size: 18),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w800,
                        color: AppColors.ink,
                      ),
                ),
                const SizedBox(height: 4),
                Text(
                  subtitle,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(width: 8),
          Icon(
            selected ? Icons.radio_button_checked : Icons.radio_button_off,
            color: selected ? AppColors.customer : AppColors.ink.withOpacity(0.5),
            size: 22,
          ),
        ],
      ),
    );
  }
}

class _TransactionsList extends StatelessWidget {
  final List<_Transaction> transactions;

  const _TransactionsList({required this.transactions});

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
              const Icon(Icons.receipt_long_rounded,
                  color: AppColors.customer, size: 20),
              const SizedBox(width: 8),
              Text(
                'Giao dịch gần đây',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w900,
                      color: AppColors.ink,
                    ),
              ),
              const Spacer(),
              TextButton(
                onPressed: () {},
                child: const Text('Xem tất cả'),
              ),
            ],
          ),
          const SizedBox(height: 6),
          ...transactions.map(
            (tx) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 6),
              child: Row(
                children: [
                  CircleAvatar(
                    radius: 20,
                    backgroundColor: tx.positive
                        ? const Color(0xFFE7F7F4)
                        : const Color(0xFFFEEFE8),
                    child: Icon(
                      tx.positive
                          ? Icons.arrow_downward_rounded
                          : Icons.arrow_upward_rounded,
                      color: tx.positive
                          ? AppColors.customer
                          : const Color(0xFFE07A24),
                      size: 18,
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          tx.title,
                          style: TextStyle(
                            color: AppColors.ink,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          tx.subtitle,
                          style: TextStyle(
                            color: AppColors.ink.withOpacity(0.6),
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 10),
                  Text(
                    tx.amount,
                    style: TextStyle(
                      color: tx.positive
                          ? AppColors.customer
                          : const Color(0xFFE07A24),
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _Transaction {
  final String title;
  final String subtitle;
  final String amount;
  final bool positive;

  const _Transaction({
    required this.title,
    required this.subtitle,
    required this.amount,
    required this.positive,
  });
}
