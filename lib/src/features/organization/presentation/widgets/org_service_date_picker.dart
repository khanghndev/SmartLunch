import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../utils/org_meal_order_date_rules.dart';
import 'organization_ui.dart';

/// Chọn ngày phục vụ — giao diện rõ ràng, mặc định Thứ 2 tuần sau (không chọn được hôm nay).
class OrgServiceDatePicker extends StatelessWidget {
  final DateTime selectedDate;
  final DateTime minDate;
  final DateTime maxDate;
  final String ruleHint;
  final ValueChanged<DateTime> onDateChanged;
  final VoidCallback onAddDay;
  final VoidCallback? onQuickAddDefaultMonday;
  final List<String> selectedDayIsos;
  final void Function(String iso)? onRemoveDay;
  final void Function(String iso)? onSelectExistingDay;

  const OrgServiceDatePicker({
    super.key,
    required this.selectedDate,
    required this.minDate,
    required this.maxDate,
    required this.ruleHint,
    required this.onDateChanged,
    required this.onAddDay,
    this.onQuickAddDefaultMonday,
    this.selectedDayIsos = const [],
    this.onRemoveDay,
    this.onSelectExistingDay,
  });

  Future<void> _openCalendar(BuildContext context) async {
    final min = OrgMealOrderDateRules.dateOnly(minDate);
    final max = OrgMealOrderDateRules.dateOnly(maxDate);
    final initial = OrgMealOrderDateRules.clamp(selectedDate, min, max);
    if (min.isAfter(max)) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Khoảng ngày phục vụ không hợp lệ. Vui lòng liên hệ hỗ trợ.'),
        ),
      );
      return;
    }

    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: min,
      lastDate: max,
      helpText: 'Chọn ngày phục vụ',
      cancelText: 'Hủy',
      confirmText: 'Chọn',
      selectableDayPredicate: (day) {
        final d = OrgMealOrderDateRules.dateOnly(day);
        if (OrgMealOrderDateRules.isToday(d)) return false;
        return OrgMealOrderDateRules.isAllowed(d, min, max);
      },
    );
    if (picked != null) {
      onDateChanged(OrgMealOrderDateRules.dateOnly(picked));
    }
  }

  @override
  Widget build(BuildContext context) {
    final today = OrgMealOrderDateRules.todayLocal();
    final min = OrgMealOrderDateRules.dateOnly(minDate);
    final max = OrgMealOrderDateRules.dateOnly(maxDate);
    final current = OrgMealOrderDateRules.clamp(selectedDate, min, max);
    final iso = OrgMealOrderDateRules.toIsoDate(current);
    final canShiftBack = current.isAfter(min);
    final canShiftForward = current.isBefore(max);
    final mondays = OrgMealOrderDateRules.mondaysInRange(min, max);
    final defaultMonday = OrgMealOrderDateRules.defaultServiceDate(today);
    final alreadyHasDefault = selectedDayIsos.contains(
      OrgMealOrderDateRules.toIsoDate(defaultMonday),
    );

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: AppDesignSystem.gray50,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: AppDesignSystem.gray200),
          ),
          child: Row(
            children: [
              Icon(Icons.today_rounded, color: AppDesignSystem.gray500, size: 20),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Hôm nay: ${OrgMealOrderDateRules.formatDisplayDate(today)}',
                      style: AppDesignSystem.label(),
                    ),
                    Text(
                      'Không thể chọn hôm nay — đặt trước tối thiểu 3 ngày',
                      style: AppDesignSystem.body(size: 11, color: AppDesignSystem.gray500),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: orgAccent.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Icon(Icons.calendar_month_rounded, color: orgAccent, size: 22),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Ngày phục vụ', style: AppDesignSystem.sectionTitle()),
                  Text(
                    '${OrgMealOrderDateRules.formatDisplayDate(min)} → ${OrgMealOrderDateRules.formatDisplayDate(max)}',
                    style: AppDesignSystem.body(size: 12),
                  ),
                ],
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        if (mondays.isNotEmpty) ...[
          Text('Gợi ý nhanh — Thứ 2', style: AppDesignSystem.body(size: 12)),
          const SizedBox(height: 8),
          SizedBox(
            height: 40,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              itemCount: mondays.length,
              separatorBuilder: (_, __) => const SizedBox(width: 8),
              itemBuilder: (_, i) {
                final d = mondays[i];
                final dIso = OrgMealOrderDateRules.toIsoDate(d);
                final isSelected = dIso == iso;
                final inList = selectedDayIsos.contains(dIso);
                return ActionChip(
                  avatar: Icon(
                    inList ? Icons.check_circle : Icons.event_rounded,
                    size: 18,
                    color: isSelected ? Colors.white : orgAccent,
                  ),
                  label: Text(OrgMealOrderDateRules.formatDisplayDate(d)),
                  backgroundColor: isSelected
                      ? orgAccent
                      : AppDesignSystem.gray50,
                  labelStyle: TextStyle(
                    fontWeight: FontWeight.w700,
                    color: isSelected ? Colors.white : AppDesignSystem.gray700,
                    fontSize: 12,
                  ),
                  side: BorderSide(
                    color: isSelected ? orgAccent : AppDesignSystem.gray200,
                  ),
                  onPressed: () => onDateChanged(d),
                );
              },
            ),
          ),
          const SizedBox(height: 12),
        ],
        Row(
          children: [
            IconButton.filledTonal(
              onPressed: canShiftBack
                  ? () => onDateChanged(
                        OrgMealOrderDateRules.shiftDay(current, -1, min, max),
                      )
                  : null,
              icon: const Icon(Icons.chevron_left_rounded),
              tooltip: 'Ngày trước',
            ),
            Expanded(
              child: Material(
                color: Colors.white,
                borderRadius: BorderRadius.circular(14),
                elevation: 0,
                child: InkWell(
                  onTap: () => _openCalendar(context),
                  borderRadius: BorderRadius.circular(14),
                  child: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 16),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(14),
                      border: Border.all(color: orgAccent, width: 2),
                      gradient: LinearGradient(
                        colors: [
                          orgAccent.withValues(alpha: 0.06),
                          Colors.white,
                        ],
                        begin: Alignment.topLeft,
                        end: Alignment.bottomRight,
                      ),
                    ),
                    child: Row(
                      children: [
                        Container(
                          width: 48,
                          height: 48,
                          decoration: BoxDecoration(
                            color: orgAccent,
                            borderRadius: BorderRadius.circular(12),
                          ),
                          alignment: Alignment.center,
                          child: Text(
                            OrgMealOrderDateRules.weekdayShort(current),
                            style: const TextStyle(
                              color: Colors.white,
                              fontWeight: FontWeight.w900,
                              fontSize: 13,
                            ),
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                OrgMealOrderDateRules.formatDisplayDate(current),
                                style: AppDesignSystem.title(size: 18),
                              ),
                              Text(
                                'Chạm để mở lịch',
                                style: AppDesignSystem.body(size: 11),
                              ),
                            ],
                          ),
                        ),
                        Icon(Icons.edit_calendar_rounded, color: orgAccent),
                      ],
                    ),
                  ),
                ),
              ),
            ),
            IconButton.filledTonal(
              onPressed: canShiftForward
                  ? () => onDateChanged(
                        OrgMealOrderDateRules.shiftDay(current, 1, min, max),
                      )
                  : null,
              icon: const Icon(Icons.chevron_right_rounded),
              tooltip: 'Ngày sau',
            ),
          ],
        ),
        const SizedBox(height: 10),
        if (!alreadyHasDefault && onQuickAddDefaultMonday != null)
          OutlinedButton.icon(
            onPressed: onQuickAddDefaultMonday,
            icon: const Icon(Icons.bolt_rounded),
            label: Text(
              'Thêm nhanh Thứ 2 tuần sau (${OrgMealOrderDateRules.formatDisplayDate(defaultMonday)})',
            ),
            style: OutlinedButton.styleFrom(
              foregroundColor: orgAccent,
              side: BorderSide(color: orgAccent.withValues(alpha: 0.5)),
              padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
            ),
          ),
        const SizedBox(height: 10),
        SizedBox(
          width: double.infinity,
          child: FilledButton.icon(
            onPressed: onAddDay,
            icon: const Icon(Icons.add_rounded),
            label: Text(
              'Thêm ${OrgMealOrderDateRules.formatDisplayDate(current)} vào lịch',
            ),
            style: FilledButton.styleFrom(
              backgroundColor: AppDesignSystem.success,
              padding: const EdgeInsets.symmetric(vertical: 14),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
            ),
          ),
        ),
        const SizedBox(height: 10),
        Container(
          width: double.infinity,
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: AppDesignSystem.info.withValues(alpha: 0.08),
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: AppDesignSystem.info.withValues(alpha: 0.2)),
          ),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(Icons.info_outline_rounded, size: 18, color: AppDesignSystem.info),
              const SizedBox(width: 8),
              Expanded(
                child: Text(ruleHint, style: AppDesignSystem.body(size: 12)),
              ),
            ],
          ),
        ),
        if (selectedDayIsos.isNotEmpty) ...[
          const SizedBox(height: 14),
          Text(
            'Lịch đã chọn (${selectedDayIsos.length} ngày)',
            style: AppDesignSystem.label(),
          ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: selectedDayIsos.map((dayIso) {
              final isPickerDay = dayIso == iso;
              DateTime? parsed;
              try {
                final p = dayIso.split('-');
                if (p.length == 3) {
                  parsed = DateTime(int.parse(p[0]), int.parse(p[1]), int.parse(p[2]));
                }
              } catch (_) {}
              final label = parsed != null
                  ? '${OrgMealOrderDateRules.weekdayShort(parsed)} · ${OrgMealOrderDateRules.formatDisplay(dayIso)}'
                  : OrgMealOrderDateRules.formatDisplay(dayIso);
              return InputChip(
                avatar: CircleAvatar(
                  radius: 10,
                  backgroundColor: isPickerDay ? orgAccent : AppDesignSystem.gray200,
                  child: Text(
                    parsed != null
                        ? OrgMealOrderDateRules.weekdayShort(parsed)
                        : '•',
                    style: TextStyle(
                      fontSize: 8,
                      fontWeight: FontWeight.w900,
                      color: isPickerDay ? Colors.white : AppDesignSystem.gray500,
                    ),
                  ),
                ),
                label: Text(label),
                selected: isPickerDay,
                selectedColor: orgAccent.withValues(alpha: 0.15),
                onPressed: onSelectExistingDay != null
                    ? () => onSelectExistingDay!(dayIso)
                    : null,
                onDeleted: onRemoveDay != null ? () => onRemoveDay!(dayIso) : null,
                deleteIcon: const Icon(Icons.close, size: 18),
              );
            }).toList(),
          ),
        ],
      ],
    );
  }
}
