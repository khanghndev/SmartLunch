import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';
import '../../data/models/finance_models.dart';
import '../../data/repositories/manager_repository.dart';
import '../widgets/manager_ui.dart';

class CashFlowPage extends StatefulWidget {
  const CashFlowPage({super.key});

  @override
  State<CashFlowPage> createState() => _CashFlowPageState();
}

class _CashFlowPageState extends State<CashFlowPage> {
  int _periodIndex = 0;
  CashFlowSummaryModel? _summary;
  List<PaymentTransactionModel> _history = [];
  bool _loading = true;
  String? _error;

  static const _periodLabels = ['Ngày', 'Tuần', 'Tháng'];
  static const _periodKeys = ['daily', 'weekly', 'monthly'];

  (DateTime, DateTime) _range() {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    switch (_periodIndex) {
      case 0:
        return (today.subtract(const Duration(days: 13)), today);
      case 1:
        return (today.subtract(const Duration(days: 27)), today);
      default:
        return (DateTime(today.year, today.month - 2, today.day), today);
    }
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final (start, end) = _range();
      final period = _periodKeys[_periodIndex];
      final summary = await ManagerRepository.instance.getCashFlowSummary(
        startDate: start,
        endDate: end,
        granularity: cashflowGranularityFor(period),
      );
      final history = await ManagerRepository.instance.getPaymentHistory(
        startDate: start,
        endDate: end,
        pageSize: 25,
      );
      if (!mounted) return;
      setState(() {
        _summary = summary;
        _history = history.items;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = apiErrorMessage(e);
        _loading = false;
      });
    }
  }

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  Widget build(BuildContext context) {
    final s = _summary;

    return ManagerPageShell(
      title: 'Quản lý thu chi',
      onRefresh: _load,
      body: _loading
          ? const ManagerLoadingBody()
          : _error != null
              ? ManagerErrorBody(message: _error!, onRetry: _load)
              : ListView(
                  physics: const AlwaysScrollableScrollPhysics(),
                  padding: const EdgeInsets.fromLTRB(16, 16, 16, 32),
                  children: [
                    ManagerPeriodChips(
                      labels: _periodLabels,
                      selected: _periodIndex,
                      onSelected: (i) {
                        setState(() => _periodIndex = i);
                        _load();
                      },
                    ),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: ManagerStatTile(
                            label: 'Tổng thu',
                            value: formatVnd(s?.totalIncome ?? 0, compact: true),
                            icon: Icons.arrow_downward_rounded,
                            color: AppColors.success,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Expanded(
                          child: ManagerStatTile(
                            label: 'Tổng chi',
                            value: formatVnd(s?.totalExpense ?? 0, compact: true),
                            icon: Icons.arrow_upward_rounded,
                            color: AppColors.danger,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 10),
                    ManagerStatTile(
                      label: 'Lợi nhuận ròng',
                      value: formatVnd(s?.totalProfit ?? 0, compact: true),
                      icon: Icons.account_balance_wallet_rounded,
                      color: managerAccent,
                    ),
                    const SizedBox(height: 16),
                    ManagerGlassCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Biểu đồ dòng tiền',
                            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                                  fontWeight: FontWeight.w800,
                                ),
                          ),
                          const SizedBox(height: 12),
                          ManagerBarChart(
                            data: {
                              for (final b in s?.items ?? <CashFlowItemModel>[])
                                if (b.periodKey.isNotEmpty) b.periodKey: b.income,
                            },
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'Lịch sử giao dịch',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.w800,
                          ),
                    ),
                    const SizedBox(height: 8),
                    if (_history.isEmpty)
                      const ManagerGlassCard(
                        child: ManagerEmptyList(
                          message: 'Chưa có giao dịch trong kỳ',
                          icon: Icons.receipt_long_rounded,
                        ),
                      )
                    else
                      ..._history.map(_txnTile),
                  ],
                ),
    );
  }

  Widget _txnTile(PaymentTransactionModel t) {
    final color = t.isIncome ? AppColors.success : AppColors.danger;
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: ManagerGlassCard(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Icon(
                t.isIncome ? Icons.add_circle_outline : Icons.remove_circle_outline,
                color: color,
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    t.description,
                    style: const TextStyle(fontWeight: FontWeight.w600),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  Text(
                    '${formatShortDate(t.date)} · ${t.method}',
                    style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                  ),
                ],
              ),
            ),
            Text(
              '${t.isIncome ? '+' : '-'}${formatVnd(t.amount, compact: true)}',
              style: TextStyle(fontWeight: FontWeight.w700, color: color),
            ),
          ],
        ),
      ),
    );
  }
}
