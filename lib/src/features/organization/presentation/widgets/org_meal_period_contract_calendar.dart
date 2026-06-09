import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../utils/org_meal_period_contract_date_rules.dart';
import 'organization_ui.dart';

/// Lịch kỳ HĐ — chọn ngày, loại trừ, tùy chỉnh suất/ngày (khớp web Index step 1).
class OrgMealPeriodContractCalendar extends StatefulWidget {
  final DateTime periodStart;
  final DateTime periodEnd;
  final int mealsPerDay;
  final List<String> excludedDates;
  final Map<String, int> dailyOverrides;
  final String? selectedDayIso;
  final ValueChanged<String?> onSelectedDayChanged;
  final void Function(String iso) onToggleExcluded;
  final void Function(String iso, int mealCount) onApplyDayMeals;
  final void Function(String iso) onClearDayMeals;

  const OrgMealPeriodContractCalendar({
    super.key,
    required this.periodStart,
    required this.periodEnd,
    required this.mealsPerDay,
    required this.excludedDates,
    required this.dailyOverrides,
    required this.selectedDayIso,
    required this.onSelectedDayChanged,
    required this.onToggleExcluded,
    required this.onApplyDayMeals,
    required this.onClearDayMeals,
  });

  @override
  State<OrgMealPeriodContractCalendar> createState() =>
      _OrgMealPeriodContractCalendarState();
}

class _OrgMealPeriodContractCalendarState
    extends State<OrgMealPeriodContractCalendar> {
  late int _calYear;
  late int _calMonth;
  late TextEditingController _dayMealsCtrl;

  @override
  void initState() {
    super.initState();
    _calYear = widget.periodStart.year;
    _calMonth = widget.periodStart.month;
    _dayMealsCtrl = TextEditingController(
      text: '${widget.mealsPerDay}',
    );
  }

  @override
  void didUpdateWidget(covariant OrgMealPeriodContractCalendar oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.selectedDayIso != oldWidget.selectedDayIso ||
        widget.mealsPerDay != oldWidget.mealsPerDay) {
      final iso = widget.selectedDayIso;
      if (iso != null && !_isExcluded(iso)) {
        _dayMealsCtrl.text = '${_mealsForDate(iso)}';
      }
    }
  }

  @override
  void dispose() {
    _dayMealsCtrl.dispose();
    super.dispose();
  }

  bool _isInRange(String iso) {
    final d = DateTime.tryParse(iso);
    if (d == null) return false;
    return OrgMealPeriodContractDateRules.isInRange(d, widget.periodStart, widget.periodEnd);
  }

  bool _isExcluded(String iso) => widget.excludedDates.contains(iso);

  int _mealsForDate(String iso) {
    if (_isExcluded(iso)) return 0;
    final custom = widget.dailyOverrides[iso];
    if (custom != null && custom > 0) return custom;
    return widget.mealsPerDay < 1 ? 1 : widget.mealsPerDay;
  }

  void _prevMonth() {
    setState(() {
      if (_calMonth == 1) {
        _calMonth = 12;
        _calYear--;
      } else {
        _calMonth--;
      }
    });
  }

  void _nextMonth() {
    setState(() {
      if (_calMonth == 12) {
        _calMonth = 1;
        _calYear++;
      } else {
        _calMonth++;
      }
    });
  }

  List<_CalCell> _cells() {
    final first = DateTime(_calYear, _calMonth, 1);
    final offset = (first.weekday + 6) % 7;
    final daysInMonth = DateTime(_calYear, _calMonth + 1, 0).day;
    final cells = <_CalCell>[];
    for (var i = 0; i < offset; i++) {
      cells.add(const _CalCell.empty());
    }
    for (var d = 1; d <= daysInMonth; d++) {
      final date = DateTime(_calYear, _calMonth, d);
      final iso = OrgMealPeriodContractDateRules.toIsoDate(date);
      final inRange = _isInRange(iso);
      final excluded = _isExcluded(iso);
      final note = OrgMealPeriodContractDateRules.weekendNote(date);
      final meals = excluded ? 0 : _mealsForDate(iso);
      cells.add(_CalCell(
        iso: iso,
        dayNum: d,
        inRange: inRange,
        excluded: excluded,
        note: note.isNotEmpty ? note : null,
        mealsLabel: excluded ? 'Nghỉ' : '$meals suất',
        customMeals: widget.dailyOverrides.containsKey(iso),
        selected: widget.selectedDayIso == iso,
      ));
    }
    return cells;
  }

  @override
  Widget build(BuildContext context) {
    const weekdays = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];
    final monthLabel = 'Tháng $_calMonth/$_calYear';
    final selected = widget.selectedDayIso;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                'Lịch kỳ hợp đồng',
                style: AppDesignSystem.label().copyWith(fontSize: 12),
              ),
            ),
            IconButton(
              onPressed: _prevMonth,
              icon: const Icon(Icons.chevron_left),
              visualDensity: VisualDensity.compact,
            ),
            Text(monthLabel, style: AppDesignSystem.body().copyWith(fontWeight: FontWeight.w700)),
            IconButton(
              onPressed: _nextMonth,
              icon: const Icon(Icons.chevron_right),
              visualDensity: VisualDensity.compact,
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: weekdays
              .map(
                (w) => Expanded(
                  child: Center(
                    child: Text(
                      w,
                      style: AppDesignSystem.body(size: 10).copyWith(
                        fontWeight: FontWeight.w800,
                        color: AppDesignSystem.gray400,
                      ),
                    ),
                  ),
                ),
              )
              .toList(),
        ),
        const SizedBox(height: 6),
        GridView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 7,
            mainAxisSpacing: 4,
            crossAxisSpacing: 4,
            childAspectRatio: 0.68,
          ),
          itemCount: _cells().length,
          itemBuilder: (_, i) {
            final cell = _cells()[i];
            if (cell.iso == null) return const SizedBox.shrink();
            return _DayCell(
              cell: cell,
              onTap: () {
                if (!cell.inRange) return;
                widget.onSelectedDayChanged(cell.iso);
                _dayMealsCtrl.text = '${_mealsForDate(cell.iso!)}';
              },
              onLongPress: cell.inRange
                  ? () => widget.onToggleExcluded(cell.iso!)
                  : null,
            );
          },
        ),
        const SizedBox(height: 10),
        Text(
          'Đã loại trừ ${widget.excludedDates.length} ngày · '
          '${widget.dailyOverrides.length} ngày tùy chỉnh suất',
          style: AppDesignSystem.body(size: 12),
        ),
        const SizedBox(height: 10),
        if (selected != null && _isInRange(selected)) ...[
          OrgCard(
            padding: const EdgeInsets.all(14),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Row(
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Ngày đang chọn',
                            style: AppDesignSystem.body(size: 10).copyWith(
                              fontWeight: FontWeight.w800,
                              color: orgAccent,
                            ),
                          ),
                          Text(
                            OrgMealPeriodContractDateRules.formatDisplay(DateTime.parse(selected)),
                            style: AppDesignSystem.label().copyWith(fontSize: 16),
                          ),
                          if (OrgMealPeriodContractDateRules.weekendNote(DateTime.parse(selected)).isNotEmpty)
                            Text(
                              OrgMealPeriodContractDateRules.weekendNote(DateTime.parse(selected)),
                              style: AppDesignSystem.body(size: 11),
                            ),
                        ],
                      ),
                    ),
                    IconButton(
                      onPressed: () => widget.onSelectedDayChanged(null),
                      icon: const Icon(Icons.close),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                TextField(
                  controller: _dayMealsCtrl,
                  enabled: !_isExcluded(selected),
                  keyboardType: TextInputType.number,
                  inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                  decoration: AppDesignSystem.inputDecoration(
                    label: 'Suất ăn ngày này',
                    hint: 'Mặc định ${widget.mealsPerDay}',
                    focusColor: orgAccent,
                  ),
                ),
                const SizedBox(height: 8),
                FilledButton(
                  onPressed: _isExcluded(selected)
                      ? null
                      : () {
                          final count =
                              int.tryParse(_dayMealsCtrl.text) ?? widget.mealsPerDay;
                          widget.onApplyDayMeals(selected, count);
                        },
                  style: FilledButton.styleFrom(backgroundColor: orgAccent),
                  child: const Text('Lưu số suất'),
                ),
                const SizedBox(height: 6),
                OutlinedButton(
                  onPressed: () => widget.onClearDayMeals(selected),
                  child: Text('Dùng mặc định (${widget.mealsPerDay})'),
                ),
                const SizedBox(height: 6),
                OutlinedButton(
                  onPressed: () => widget.onToggleExcluded(selected),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: _isExcluded(selected)
                        ? AppDesignSystem.success
                        : AppDesignSystem.danger,
                  ),
                  child: Text(
                    _isExcluded(selected) ? 'Bỏ loại trừ' : 'Loại trừ ngày này',
                  ),
                ),
              ],
            ),
          ),
        ] else
          OrgInfoBanner(
            message:
                'Chạm một ngày trong kỳ để chỉnh suất hoặc loại trừ. Giữ lâu để loại trừ nhanh.',
            icon: Icons.touch_app_outlined,
            color: AppDesignSystem.info,
          ),
      ],
    );
  }
}

class _CalCell {
  final String? iso;
  final int dayNum;
  final bool inRange;
  final bool excluded;
  final String? note;
  final String mealsLabel;
  final bool customMeals;
  final bool selected;

  const _CalCell({
    required this.iso,
    required this.dayNum,
    required this.inRange,
    required this.excluded,
    required this.note,
    required this.mealsLabel,
    required this.customMeals,
    required this.selected,
  });

  const _CalCell.empty()
      : iso = null,
        dayNum = 0,
        inRange = false,
        excluded = false,
        note = null,
        mealsLabel = '',
        customMeals = false,
        selected = false;
}

class _DayCell extends StatelessWidget {
  final _CalCell cell;
  final VoidCallback onTap;
  final VoidCallback? onLongPress;

  const _DayCell({
    required this.cell,
    required this.onTap,
    this.onLongPress,
  });

  @override
  Widget build(BuildContext context) {
    if (cell.iso == null) return const SizedBox.shrink();

    Color bg = Colors.white;
    Color fg = AppDesignSystem.gray900;
    Border? border;

    if (!cell.inRange) {
      bg = Colors.transparent;
      fg = AppDesignSystem.gray400;
    } else if (cell.excluded) {
      bg = AppDesignSystem.danger.withValues(alpha: 0.12);
      fg = AppDesignSystem.danger;
      border = Border.all(color: AppDesignSystem.danger.withValues(alpha: 0.4));
    } else if (cell.selected) {
      bg = orgAccent.withValues(alpha: 0.15);
      fg = orgAccent;
      border = Border.all(color: orgAccent, width: 2);
    }

    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: cell.inRange ? onTap : null,
        onLongPress: onLongPress,
        borderRadius: BorderRadius.circular(10),
        child: Ink(
          decoration: BoxDecoration(
            color: bg,
            borderRadius: BorderRadius.circular(10),
            border: border ?? Border.all(color: AppDesignSystem.gray200),
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 3, horizontal: 2),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(
                  '${cell.dayNum}',
                  style: TextStyle(
                    fontWeight: FontWeight.w800,
                    fontSize: 13,
                    color: fg,
                    height: 1.1,
                  ),
                ),
                if (cell.inRange && cell.note != null)
                  Text(
                    cell.note!,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    textAlign: TextAlign.center,
                    style: AppDesignSystem.body(size: 8, color: AppDesignSystem.gray400)
                        .copyWith(height: 1.1),
                  ),
                if (cell.inRange)
                  Text(
                    cell.mealsLabel,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    textAlign: TextAlign.center,
                    style: AppDesignSystem.body(size: 8).copyWith(
                      color: cell.customMeals ? AppDesignSystem.success : AppDesignSystem.gray500,
                      fontWeight: FontWeight.w700,
                      height: 1.1,
                    ),
                  ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
