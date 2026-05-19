import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../utils/org_meal_order_date_rules.dart';
import 'organization_ui.dart';

/// Chọn ngày phục vụ — tương đương web `<input type="date">` + nút Thêm ngày.
class OrgServiceDatePicker extends StatelessWidget {
  final DateTime selectedDate;
  final DateTime minDate;
  final DateTime maxDate;
  final String ruleHint;
  final ValueChanged<DateTime> onDateChanged;
  final VoidCallback onAddDay;
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
    this.selectedDayIsos = const [],
    this.onRemoveDay,
    this.onSelectExistingDay,
  });

  Future<void> _openCalendar(BuildContext context) async {
    final min = OrgMealOrderDateRules.dateOnly(minDate);
    final max = OrgMealOrderDateRules.dateOnly(maxDate);
    var initial = OrgMealOrderDateRules.clamp(selectedDate, min, max);

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
    );
    if (picked != null) {
      onDateChanged(OrgMealOrderDateRules.dateOnly(picked));
    }
  }

  @override
  Widget build(BuildContext context) {
    final min = OrgMealOrderDateRules.dateOnly(minDate);
    final max = OrgMealOrderDateRules.dateOnly(maxDate);
    final current = OrgMealOrderDateRules.clamp(selectedDate, min, max);
    final iso = OrgMealOrderDateRules.toIsoDate(current);
    final canShiftBack = current.isAfter(min);
    final canShiftForward = current.isBefore(max);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: AppDesignSystem.success.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(10),
              ),
              child: const Icon(Icons.calendar_month_rounded,
                  color: AppDesignSystem.success, size: 22),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Thêm ngày phục vụ', style: AppDesignSystem.label()),
                  Text(
                    'Từ ${OrgMealOrderDateRules.formatDisplay(OrgMealOrderDateRules.toIsoDate(min))}'
                    ' đến ${OrgMealOrderDateRules.formatDisplay(OrgMealOrderDateRules.toIsoDate(max))}',
                    style: AppDesignSystem.body(size: 12),
                  ),
                ],
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
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
                color: AppDesignSystem.gray50,
                borderRadius: BorderRadius.circular(12),
                child: InkWell(
                  onTap: () => _openCalendar(context),
                  borderRadius: BorderRadius.circular(12),
                  child: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: orgAccent.withValues(alpha: 0.35), width: 1.5),
                    ),
                    child: Row(
                      children: [
                        Icon(Icons.event_rounded, color: orgAccent, size: 22),
                        const SizedBox(width: 10),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                OrgMealOrderDateRules.formatDisplay(iso),
                                style: AppDesignSystem.sectionTitle(),
                              ),
                              Text(
                                iso,
                                style: AppDesignSystem.body(size: 11),
                              ),
                            ],
                          ),
                        ),
                        Icon(Icons.edit_calendar_rounded, color: orgAccent, size: 20),
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
        SizedBox(
          width: double.infinity,
          child: FilledButton.icon(
            onPressed: onAddDay,
            icon: const Icon(Icons.add_rounded),
            label: const Text('Thêm ngày vào lịch'),
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
            'Đã chọn ${selectedDayIsos.length} ngày — chạm để chọn lại',
            style: AppDesignSystem.body(size: 12),
          ),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: selectedDayIsos.map((iso) {
              final isPickerDay = iso == OrgMealOrderDateRules.toIsoDate(current);
              return InputChip(
                label: Text(OrgMealOrderDateRules.formatDisplay(iso)),
                selected: isPickerDay,
                selectedColor: orgAccent.withValues(alpha: 0.2),
                onPressed: onSelectExistingDay != null
                    ? () => onSelectExistingDay!(iso)
                    : null,
                onDeleted: onRemoveDay != null ? () => onRemoveDay!(iso) : null,
                deleteIcon: const Icon(Icons.close, size: 18),
              );
            }).toList(),
          ),
        ],
      ],
    );
  }
}
