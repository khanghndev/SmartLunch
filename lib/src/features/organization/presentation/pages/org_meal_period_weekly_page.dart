import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/models/contract_models.dart';
import '../../data/org_repository.dart';
import '../widgets/organization_ui.dart';
import '../widgets/org_main_dish_picker_sheet.dart';

class OrgMealPeriodWeeklyPage extends StatefulWidget {
  const OrgMealPeriodWeeklyPage({super.key, required this.contractId});

  final int contractId;

  @override
  State<OrgMealPeriodWeeklyPage> createState() => _OrgMealPeriodWeeklyPageState();
}

class _OrgMealPeriodWeeklyPageState extends State<OrgMealPeriodWeeklyPage> {
  ContractModel? _contract;
  bool _loading = true;
  String? _error;

  DateTime _weekStart = _mondayOf(DateTime.now().add(const Duration(days: 7)));
  final Map<String, List<MealLineDraftModel>> _mainByDate = {};
  bool _submitting = false;

  int get _mealsPerDay => _contract?.mealsPerDay ?? 1;

  @override
  void initState() {
    super.initState();
    _loadContract();
  }

  Future<void> _loadContract() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final c = await OrgRepository.instance.getContractDetail(widget.contractId);
      if (!mounted) return;
      if (c == null || c.id <= 0) {
        setState(() {
          _error = 'Không tìm thấy hợp đồng.';
          _loading = false;
        });
        return;
      }
      setState(() {
        _contract = c;
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  Future<void> _pickWeekStart() async {
    final picked = await showDatePicker(
      context: context,
      firstDate: DateTime.now().subtract(const Duration(days: 30)),
      lastDate: DateTime.now().add(const Duration(days: 365)),
      initialDate: _weekStart,
      helpText: 'Chọn tuần (bấm vào Thứ 2)',
      cancelText: 'Hủy',
      confirmText: 'Chọn',
    );
    if (picked == null) return;
    final monday = _mondayOf(picked);
    setState(() {
      _weekStart = monday;
      // giữ lại dữ liệu nếu user chọn lại cùng ngày; còn nếu khác tuần thì reset
      _mainByDate.clear();
    });
  }

  List<DateTime> get _weekDates => List.generate(
        7,
        (i) => DateTime(_weekStart.year, _weekStart.month, _weekStart.day + i),
      );

  String _iso(DateTime d) =>
      '${d.year.toString().padLeft(4, '0')}-'
      '${d.month.toString().padLeft(2, '0')}-'
      '${d.day.toString().padLeft(2, '0')}';

  void _addDishForDay(String isoDate) {
    OrgMainDishPickerSheet.show(
      context,
      onPick: (dish) {
        setState(() {
          final lines = _mainByDate.putIfAbsent(isoDate, () => []);
          final existing = lines.where((l) => l.dishId == dish.id).toList();
          if (existing.isNotEmpty) {
            existing.first.quantity += 1;
          } else {
            lines.add(MealLineDraftModel(dishId: dish.id, dishName: dish.name, quantity: 1));
          }
        });
      },
    );
  }

  void _inc(String isoDate, int dishId) {
    setState(() {
      final lines = _mainByDate[isoDate] ?? [];
      final line = lines.firstWhere((l) => l.dishId == dishId);
      line.quantity += 1;
    });
  }

  void _dec(String isoDate, int dishId) {
    setState(() {
      final lines = _mainByDate[isoDate] ?? [];
      final line = lines.firstWhere((l) => l.dishId == dishId);
      line.quantity -= 1;
      if (line.quantity < 1) {
        lines.removeWhere((x) => x.dishId == dishId);
      }
      if (lines.isEmpty) _mainByDate.remove(isoDate);
    });
  }

  int _dayTotal(String isoDate) =>
      (_mainByDate[isoDate] ?? const []).fold(0, (s, l) => s + l.quantity);

  Future<void> _submit() async {
    final weekStartIso = _iso(_weekStart);

    final mealDays = <Map<String, dynamic>>[];
    for (final entry in _mainByDate.entries) {
      final isoDate = entry.key;
      final lines = entry.value;
      if (lines.isEmpty) continue;

      final total = lines.fold(0, (s, l) => s + l.quantity);
      if (total != _mealsPerDay) {
        setState(() => _error =
            'Ngày $isoDate: tổng số suất món chính phải bằng $_mealsPerDay.');
        return;
      }

      mealDays.add({
        'serviceDate': isoDate,
        'mealPlan': {
          'main': lines.map((l) => l.toJson()).toList(),
        },
      });
    }

    if (mealDays.isEmpty) {
      setState(() => _error = 'Vui lòng chọn món cho ít nhất 1 ngày trong tuần.');
      return;
    }

    setState(() {
      _submitting = true;
      _error = null;
    });
    try {
      final res = await OrgRepository.instance.submitMealPeriodWeeklyMeals(
        contractId: widget.contractId,
        weekStart: weekStartIso,
        mealDays: mealDays,
      );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Đã lưu chọn món tuần ($weekStartIso). ${res.itemCount} dòng.'),
          backgroundColor: AppDesignSystem.success,
        ),
      );
      Navigator.of(context).pop();
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = orgApiError(e));
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Chọn món theo tuần',
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          OrgPageIntro(
            title: 'Hợp đồng #${widget.contractId}',
            description: 'Chỉ chọn món chính. Mỗi ngày phải đủ $_mealsPerDay suất.',
            icon: Icons.calendar_month_outlined,
          ),
          if (_loading) ...[
            const SizedBox(height: 12),
            const Center(child: CircularProgressIndicator()),
          ] else ...[
            if (_error != null) ...[
              const SizedBox(height: 12),
              OrgInfoBanner(
                message: _error!,
                icon: Icons.error_outline,
                color: AppDesignSystem.danger,
              ),
            ],
            const SizedBox(height: 12),
            OrgCard(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const OrgSectionHeader(
                    title: 'Chọn tuần',
                    subtitle: 'Chọn ngày bất kỳ, hệ thống sẽ tự quy về Thứ 2 tuần đó',
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          '${_iso(_weekStart)} → ${_iso(_weekStart.add(const Duration(days: 6)))}',
                          style: AppDesignSystem.sectionTitle(),
                        ),
                      ),
                      OutlinedButton.icon(
                        onPressed: _pickWeekStart,
                        icon: const Icon(Icons.date_range_outlined),
                        label: const Text('Đổi tuần'),
                        style: OutlinedButton.styleFrom(foregroundColor: orgAccent),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            ..._weekDates.map((d) {
              final isoDate = _iso(d);
              final lines = _mainByDate[isoDate] ?? const [];
              final total = _dayTotal(isoDate);
              return Padding(
                padding: const EdgeInsets.only(bottom: 10),
                child: OrgCard(
                  padding: const EdgeInsets.all(14),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Expanded(
                            child: Text(
                              isoDate,
                              style: AppDesignSystem.sectionTitle(),
                            ),
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                            decoration: BoxDecoration(
                              color: (total == _mealsPerDay)
                                  ? AppDesignSystem.success.withValues(alpha: 0.12)
                                  : AppDesignSystem.warning.withValues(alpha: 0.12),
                              borderRadius: BorderRadius.circular(20),
                            ),
                            child: Text(
                              '$total/$_mealsPerDay suất',
                              style: AppDesignSystem.body(
                                size: 12,
                                color: total == _mealsPerDay
                                    ? AppDesignSystem.success
                                    : AppDesignSystem.warning,
                              ).copyWith(fontWeight: FontWeight.w800),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 10),
                      if (lines.isEmpty)
                        Text('Chưa chọn món.', style: AppDesignSystem.body(size: 12))
                      else
                        ...lines.map((l) => Padding(
                              padding: const EdgeInsets.only(bottom: 6),
                              child: Row(
                                children: [
                                  Expanded(
                                    child: Text(
                                      l.dishName,
                                      style: AppDesignSystem.body(
                                        size: 13,
                                        color: AppDesignSystem.gray700,
                                      ).copyWith(fontWeight: FontWeight.w700),
                                    ),
                                  ),
                                  IconButton(
                                    onPressed: () => _dec(isoDate, l.dishId),
                                    icon: const Icon(Icons.remove_circle_outline),
                                  ),
                                  SizedBox(
                                    width: 42,
                                    child: TextField(
                                      controller: TextEditingController(text: '${l.quantity}'),
                                      textAlign: TextAlign.center,
                                      keyboardType: TextInputType.number,
                                      inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                                      onSubmitted: (v) {
                                        final n = int.tryParse(v) ?? l.quantity;
                                        setState(() => l.quantity = n < 1 ? 1 : n);
                                      },
                                      decoration: const InputDecoration(
                                        isDense: true,
                                        border: OutlineInputBorder(),
                                      ),
                                    ),
                                  ),
                                  IconButton(
                                    onPressed: () => _inc(isoDate, l.dishId),
                                    icon: const Icon(Icons.add_circle_outline),
                                  ),
                                ],
                              ),
                            )),
                      const SizedBox(height: 8),
                      OutlinedButton.icon(
                        onPressed: () => _addDishForDay(isoDate),
                        icon: const Icon(Icons.add_rounded),
                        label: const Text('Thêm món chính'),
                        style: OutlinedButton.styleFrom(foregroundColor: orgAccent),
                      ),
                    ],
                  ),
                ),
              );
            }),
            const SizedBox(height: 10),
            FilledButton.icon(
              onPressed: _submitting ? null : _submit,
              icon: _submitting
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.save_outlined),
              label: Text(_submitting ? 'Đang lưu…' : 'Lưu chọn món tuần'),
              style: FilledButton.styleFrom(
                backgroundColor: orgAccent,
                padding: const EdgeInsets.symmetric(vertical: 14),
              ),
            ),
          ],
        ],
      ),
    );
  }

  static DateTime _mondayOf(DateTime d) {
    final normalized = DateTime(d.year, d.month, d.day);
    final weekday = normalized.weekday; // Mon=1..Sun=7
    return normalized.subtract(Duration(days: weekday - DateTime.monday));
  }
}

