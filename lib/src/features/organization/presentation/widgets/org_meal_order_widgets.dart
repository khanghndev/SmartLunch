import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/models/bulk_order_models.dart';
import '../../utils/org_meal_order_date_rules.dart';
import 'organization_ui.dart';

/// Tab ngày phục vụ — hiển thị thứ + số suất món chính.
class OrgMealDayTabBar extends StatelessWidget {
  final List<MealDayDraftModel> mealDays;
  final String? activeDayIso;
  final ValueChanged<String> onDaySelected;

  const OrgMealDayTabBar({
    super.key,
    required this.mealDays,
    required this.activeDayIso,
    required this.onDaySelected,
  });

  @override
  Widget build(BuildContext context) {
    if (mealDays.isEmpty) return const SizedBox.shrink();

    return SizedBox(
      height: 92,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 2),
        itemCount: mealDays.length,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (_, i) {
          final d = mealDays[i];
          final active = d.serviceDate == activeDayIso;
          final mainQty = d.slotTotal('main');
          final hasMain = mainQty > 0;
          DateTime? parsed;
          try {
            final p = d.serviceDate.split('-');
            if (p.length == 3) {
              parsed = DateTime(int.parse(p[0]), int.parse(p[1]), int.parse(p[2]));
            }
          } catch (_) {}

          final weekday = parsed != null
              ? OrgMealOrderDateRules.weekdayShort(parsed)
              : '—';
          final dateLabel =
              OrgMealOrderDateRules.formatDisplayShort(d.serviceDate);

          return Material(
            color: Colors.transparent,
            child: InkWell(
              onTap: () => onDaySelected(d.serviceDate),
              borderRadius: BorderRadius.circular(14),
              child: Ink(
                width: 76,
                decoration: BoxDecoration(
                  color: active ? orgAccent : Colors.white,
                  borderRadius: BorderRadius.circular(14),
                  border: Border.all(
                    color: active
                        ? orgAccent
                        : (hasMain
                            ? AppDesignSystem.gray200
                            : AppDesignSystem.warning),
                    width: active ? 2 : 1,
                  ),
                ),
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 8),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        weekday,
                        style: TextStyle(
                          fontSize: 10,
                          fontWeight: FontWeight.w800,
                          color:
                              active ? Colors.white70 : AppDesignSystem.gray500,
                        ),
                      ),
                      FittedBox(
                        fit: BoxFit.scaleDown,
                        child: Text(
                          dateLabel,
                          style: TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.w900,
                            color:
                                active ? Colors.white : AppDesignSystem.gray700,
                          ),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        hasMain ? '$mainQty suất' : 'Chưa có món',
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(
                          fontSize: 9,
                          fontWeight: FontWeight.w700,
                          color: active
                              ? Colors.white
                              : (hasMain ? orgAccent : AppDesignSystem.warning),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

/// Điều chỉnh suất: +/- và nhập tay (vd. 1000 suất).
class OrgMealLineQtyControl extends StatefulWidget {
  final int quantity;
  final int minQty;
  final int maxQty;
  final ValueChanged<int> onChanged;
  final VoidCallback onRemove;

  const OrgMealLineQtyControl({
    super.key,
    required this.quantity,
    required this.minQty,
    required this.maxQty,
    required this.onChanged,
    required this.onRemove,
  });

  @override
  State<OrgMealLineQtyControl> createState() => _OrgMealLineQtyControlState();
}

class _OrgMealLineQtyControlState extends State<OrgMealLineQtyControl> {
  late final TextEditingController _ctrl;

  @override
  void initState() {
    super.initState();
    _ctrl = TextEditingController(text: '${widget.quantity}');
  }

  @override
  void didUpdateWidget(covariant OrgMealLineQtyControl oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.quantity != widget.quantity &&
        _ctrl.text != '${widget.quantity}') {
      _ctrl.text = '${widget.quantity}';
    }
  }

  @override
  void dispose() {
    _ctrl.dispose();
    super.dispose();
  }

  void _commitInput() {
    final parsed = int.tryParse(_ctrl.text.trim());
    if (parsed == null || parsed < widget.minQty) {
      _ctrl.text = '${widget.minQty}';
      widget.onChanged(widget.minQty);
      return;
    }
    final clamped = parsed > widget.maxQty ? widget.maxQty : parsed;
    _ctrl.text = '$clamped';
    widget.onChanged(clamped);
  }

  void _applyDelta(int delta) {
    final next = (widget.quantity + delta).clamp(widget.minQty, widget.maxQty);
    _ctrl.text = '$next';
    widget.onChanged(next);
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: AppDesignSystem.gray50,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: AppDesignSystem.gray200),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          IconButton(
            icon: const Icon(Icons.remove_rounded, size: 20),
            onPressed: () {
              if (widget.quantity <= widget.minQty) {
                widget.onRemove();
              } else {
                _applyDelta(-1);
              }
            },
            visualDensity: VisualDensity.compact,
          ),
          SizedBox(
            width: 56,
            child: TextField(
              controller: _ctrl,
              keyboardType: TextInputType.number,
              inputFormatters: [FilteringTextInputFormatter.digitsOnly],
              textAlign: TextAlign.center,
              style: AppDesignSystem.sectionTitle().copyWith(fontSize: 14),
              decoration: const InputDecoration(
                isDense: true,
                contentPadding: EdgeInsets.symmetric(vertical: 8, horizontal: 4),
                border: InputBorder.none,
                hintText: 'SL',
              ),
              onSubmitted: (_) => _commitInput(),
              onEditingComplete: _commitInput,
            ),
          ),
          IconButton(
            icon: const Icon(Icons.add_rounded, size: 20),
            onPressed: widget.quantity >= widget.maxQty ? null : () => _applyDelta(1),
            visualDensity: VisualDensity.compact,
          ),
        ],
      ),
    );
  }
}

/// Thẻ slot món (main / side / soup) — dễ thêm và chỉnh số lượng.
class OrgMealSlotCard extends StatelessWidget {
  final String slot;
  final String label;
  final IconData icon;
  final Color accentColor;
  final List<MealLineDraftModel> lines;
  final int mainTotal;
  final bool canAdd;
  final VoidCallback onAdd;
  final int Function(MealLineDraftModel line) maxQtyForLine;
  final void Function(MealLineDraftModel line, int qty) onQtySet;
  final void Function(MealLineDraftModel line) onRemoveLine;

  const OrgMealSlotCard({
    super.key,
    required this.slot,
    required this.label,
    required this.icon,
    required this.accentColor,
    required this.lines,
    required this.mainTotal,
    required this.canAdd,
    required this.onAdd,
    required this.maxQtyForLine,
    required this.onQtySet,
    required this.onRemoveLine,
  });

  static const int mainQtyMax = 99999;

  int get _slotTotal =>
      lines.fold(0, (s, l) => s + (l.quantity < 1 ? 1 : l.quantity));

  @override
  Widget build(BuildContext context) {
    final quota = slot == 'main' ? '$_slotTotal suất' : '$_slotTotal / $mainTotal';

    return OrgCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Container(
            padding: const EdgeInsets.fromLTRB(14, 12, 8, 12),
            decoration: BoxDecoration(
              color: accentColor.withValues(alpha: 0.08),
              borderRadius: const BorderRadius.vertical(top: Radius.circular(14)),
            ),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: accentColor.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Icon(icon, color: accentColor, size: 22),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(label, style: AppDesignSystem.sectionTitle()),
                      Text(
                        slot == 'main'
                            ? 'Tính suất & thành tiền'
                            : 'Miễn phí — tối đa bằng suất chính',
                        style: AppDesignSystem.body(size: 11),
                      ),
                    ],
                  ),
                ),
                Text(quota, style: AppDesignSystem.label(color: accentColor)),
                const SizedBox(width: 4),
                IconButton.filled(
                  onPressed: canAdd ? onAdd : null,
                  icon: const Icon(Icons.add_rounded, size: 22),
                  style: IconButton.styleFrom(
                    backgroundColor: canAdd ? accentColor : AppDesignSystem.gray200,
                    foregroundColor: Colors.white,
                    disabledBackgroundColor: AppDesignSystem.gray200,
                  ),
                  tooltip: 'Thêm món',
                ),
              ],
            ),
          ),
          if (lines.isEmpty)
            Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  Icon(Icons.restaurant_menu_outlined,
                      color: AppDesignSystem.gray400, size: 20),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Text(
                      'Chưa có món — nhấn + để chọn từ thư viện',
                      style: AppDesignSystem.body(size: 12),
                    ),
                  ),
                ],
              ),
            )
          else
            ...lines.map((line) {
              return Container(
                decoration: BoxDecoration(
                  border: Border(
                    top: BorderSide(color: AppDesignSystem.gray100),
                  ),
                ),
                child: ListTile(
                  contentPadding:
                      const EdgeInsets.symmetric(horizontal: 14, vertical: 2),
                  title: Text(line.dishName, style: AppDesignSystem.label()),
                  subtitle: Text(
                    'Nhập số suất hoặc dùng +/-',
                    style: AppDesignSystem.body(size: 10),
                  ),
                  trailing: OrgMealLineQtyControl(
                    quantity: line.quantity < 1 ? 1 : line.quantity,
                    minQty: 1,
                    maxQty: maxQtyForLine(line),
                    onChanged: (q) => onQtySet(line, q),
                    onRemove: () => onRemoveLine(line),
                  ),
                ),
              );
            }),
        ],
      ),
    );
  }
}

/// Thanh tóm tắt tạm tính cố định cuối bước thực đơn.
class OrgMealOrderSummaryBar extends StatelessWidget {
  final double pricePerPortion;
  final int totalMainQty;
  final double estimatedTotal;
  final String primaryLabel;
  final VoidCallback onPrimary;
  final VoidCallback? onBack;
  final bool loading;

  const OrgMealOrderSummaryBar({
    super.key,
    required this.pricePerPortion,
    required this.totalMainQty,
    required this.estimatedTotal,
    required this.primaryLabel,
    required this.onPrimary,
    this.onBack,
    this.loading = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: orgAccent.withValues(alpha: 0.25)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.06),
            blurRadius: 12,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Tạm tính', style: AppDesignSystem.body(size: 12)),
                    Text(
                      formatOrgVnd(estimatedTotal),
                      style: AppDesignSystem.title(size: 22, color: orgAccent),
                    ),
                    Text(
                      '${formatOrgVnd(pricePerPortion)}/suất × $totalMainQty suất chính',
                      style: AppDesignSystem.body(size: 11),
                    ),
                  ],
                ),
              ),
              Icon(Icons.receipt_long_rounded, color: orgAccent.withValues(alpha: 0.5), size: 36),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              if (onBack != null)
                Expanded(
                  child: OutlinedButton(
                    onPressed: loading ? null : onBack,
                    child: const Text('Quay lại'),
                  ),
                ),
              if (onBack != null) const SizedBox(width: 10),
              Expanded(
                flex: onBack != null ? 2 : 1,
                child: FilledButton(
                  onPressed: loading ? null : onPrimary,
                  style: FilledButton.styleFrom(
                    backgroundColor: orgAccent,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                  child: loading
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: Colors.white,
                          ),
                        )
                      : Text(primaryLabel),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
