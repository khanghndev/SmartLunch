import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_order_date_rules.dart';
import '../widgets/org_dish_picker_sheet.dart';
import '../widgets/org_service_date_picker.dart';
import '../widgets/organization_ui.dart';

/// Đặt suất tập trung — flow 3 bước (web OrganizationMealOrder/Index).
class BulkOrderPage extends StatefulWidget {
  const BulkOrderPage({super.key});

  @override
  State<BulkOrderPage> createState() => _BulkOrderPageState();
}

class _BulkOrderPageState extends State<BulkOrderPage> {
  int _step = 1;
  bool _loading = true;
  String? _error;

  int _organizationId = 0;
  String _organizationName = '';
  List<DishCategoryModel> _categories = [];
  DateTime _dateMin = OrgMealOrderDateRules.minimumServiceDate(OrgMealOrderDateRules.todayLocal());
  DateTime _dateMax = OrgMealOrderDateRules.maximumServiceDate(OrgMealOrderDateRules.todayLocal());
  late DateTime _pickerDate;

  final _priceCtrl = TextEditingController(text: '45000');
  final _promotionCtrl = TextEditingController();
  final List<MealDayDraftModel> _mealDays = [];
  String? _activeDay;
  bool _submitting = false;

  @override
  void initState() {
    super.initState();
    _pickerDate = OrgMealOrderDateRules.defaultServiceDate(OrgMealOrderDateRules.todayLocal());
    _bootstrap();
  }

  double get _pricePerPortion => double.tryParse(_priceCtrl.text) ?? 0;

  @override
  void dispose() {
    _priceCtrl.dispose();
    _promotionCtrl.dispose();
    super.dispose();
  }

  Future<void> _bootstrap() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final profile = await ProfileRepository.instance.getProfile();
      final unit = profile.unit;
      if (unit == null || unit.id <= 0) {
        throw ApiException(
          'Tài khoản chưa được gán đơn vị (Organization).',
          statusCode: 403,
        );
      }
      final cats = await OrgRepository.instance.getDishCategories();
      final today = OrgMealOrderDateRules.todayLocal();
      var dateMin = cats.allowedFirstServiceDate ??
          OrgMealOrderDateRules.minimumServiceDate(today);
      var dateMax = cats.allowedLastServiceDate ??
          OrgMealOrderDateRules.maximumServiceDate(today);
      dateMin = OrgMealOrderDateRules.dateOnly(dateMin);
      dateMax = OrgMealOrderDateRules.dateOnly(dateMax);
      if (dateMax.isBefore(dateMin)) dateMax = dateMin;

      var picker = OrgMealOrderDateRules.defaultServiceDate(today);
      picker = OrgMealOrderDateRules.clamp(picker, dateMin, dateMax);

      setState(() {
        _organizationId = unit.id;
        _organizationName = unit.name;
        _categories = cats.categories;
        _dateMin = dateMin;
        _dateMax = dateMax;
        _pickerDate = picker;
        _loading = false;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  void _addDay() {
    final day = OrgMealOrderDateRules.clamp(_pickerDate, _dateMin, _dateMax);
    final iso = OrgMealOrderDateRules.toIsoDate(day);
    if (_mealDays.any((d) => d.serviceDate == iso)) {
      _showError('Ngày đã có trong danh sách.');
      return;
    }
    setState(() {
      _mealDays.add(MealDayDraftModel(serviceDate: iso));
      _mealDays.sort((a, b) => a.serviceDate.compareTo(b.serviceDate));
      _activeDay = iso;
      _pickerDate = day;
    });
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Đã thêm ngày ${OrgMealOrderDateRules.formatDisplay(iso)}'),
        backgroundColor: AppDesignSystem.success,
      ),
    );
  }

  void _selectExistingDay(String iso) {
    final parts = iso.split('-');
    if (parts.length != 3) return;
    final d = DateTime(
      int.parse(parts[0]),
      int.parse(parts[1]),
      int.parse(parts[2]),
    );
    setState(() => _pickerDate = OrgMealOrderDateRules.clamp(d, _dateMin, _dateMax));
  }

  void _removeDay(String iso) {
    setState(() {
      _mealDays.removeWhere((d) => d.serviceDate == iso);
      _activeDay = _mealDays.isEmpty ? null : _mealDays.first.serviceDate;
    });
  }

  MealDayDraftModel? _day(String? iso) {
    if (iso == null) return null;
    try {
      return _mealDays.firstWhere((d) => d.serviceDate == iso);
    } catch (_) {
      return null;
    }
  }

  int _totalMainQty() {
    var t = 0;
    for (final d in _mealDays) {
      t += d.slotTotal('main');
    }
    return t;
  }

  String? _validateMenu() {
    if (_pricePerPortion <= 0) return 'Nhập giá thỏa thuận / suất hợp lệ.';
    if (_mealDays.isEmpty) return 'Thêm ít nhất một ngày phục vụ.';
    for (final day in _mealDays) {
      if (day.slotTotal('main') <= 0) {
        return 'Ngày ${OrgMealOrderDateRules.formatDisplay(day.serviceDate)}: cần ít nhất một món chính.';
      }
      final main = day.slotTotal('main');
      if (day.slotTotal('side') > main) {
        return 'Ngày ${OrgMealOrderDateRules.formatDisplay(day.serviceDate)}: món phụ vượt suất chính.';
      }
      if (day.slotTotal('soup') > main) {
        return 'Ngày ${OrgMealOrderDateRules.formatDisplay(day.serviceDate)}: canh vượt suất chính.';
      }
    }
    return null;
  }

  Future<void> _submitDraft() async {
    final err = _validateMenu();
    if (err != null) {
      _showError(err);
      return;
    }
    setState(() => _submitting = true);
    try {
      final draft = await OrgRepository.instance.prepareMealContract(
        organizationId: _organizationId,
        price: _pricePerPortion,
        mealDays: List.from(_mealDays),
        promotionCode: _promotionCtrl.text.trim().isEmpty
            ? null
            : _promotionCtrl.text.trim(),
        organizationName: _organizationName,
      );
      if (!mounted) return;
      await Navigator.of(context).pushNamed(
        AppRoutes.orgMealOrderReview,
        arguments: draft,
      );
    } catch (e) {
      _showError(e.toString());
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  void _showError(String msg) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(msg), backgroundColor: AppDesignSystem.danger),
    );
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đặt suất ăn tập trung',
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? _buildError()
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    _buildHero(),
                    const SizedBox(height: 16),
                    _buildStepper(),
                    const SizedBox(height: 16),
                    if (_step == 1) _buildStep1() else _buildStep2(),
                  ],
                ),
    );
  }

  Widget _buildError() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.error_outline, size: 48, color: AppDesignSystem.danger),
            const SizedBox(height: 12),
            Text(_error!, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(onPressed: _bootstrap, child: const Text('Thử lại')),
          ],
        ),
      ),
    );
  }

  Widget _buildHero() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(colors: kOrgRole.gradient),
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Thiết lập thực đơn cho đơn vị',
            style: AppDesignSystem.title(size: 20, color: Colors.white),
          ),
          if (_organizationName.isNotEmpty) ...[
            const SizedBox(height: 8),
            Text(
              _organizationName,
              style: AppDesignSystem.body(color: Colors.white.withValues(alpha: 0.9)),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildStepper() {
    const labels = ['Thiết lập', 'Thực đơn', 'Xác nhận'];
    return Row(
      children: List.generate(3, (i) {
        final n = i + 1;
        final active = _step == n || (n == 3 && _step > 2);
        final done = _step > n;
        return Expanded(
          child: Column(
            children: [
              CircleAvatar(
                radius: 16,
                backgroundColor: done
                    ? AppDesignSystem.success
                    : active
                        ? orgAccent
                        : AppDesignSystem.gray200,
                child: Text(
                  '$n',
                  style: TextStyle(
                    color: done || active ? Colors.white : AppDesignSystem.gray500,
                    fontWeight: FontWeight.w800,
                    fontSize: 12,
                  ),
                ),
              ),
              const SizedBox(height: 4),
              Text(
                labels[i],
                style: AppDesignSystem.body(size: 10).copyWith(
                  fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                  color: active ? orgAccent : AppDesignSystem.gray500,
                ),
              ),
            ],
          ),
        );
      }),
    );
  }

  Widget _buildStep1() {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Bước 1 — Thiết lập đơn hàng', style: AppDesignSystem.sectionTitle()),
          const SizedBox(height: 12),
          TextField(
            controller: _priceCtrl,
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            decoration: const InputDecoration(
              labelText: 'Giá thỏa thuận / suất (VND)',
              prefixIcon: Icon(Icons.payments_outlined),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _promotionCtrl,
            decoration: const InputDecoration(
              labelText: 'Mã khuyến mãi (tùy chọn)',
              prefixIcon: Icon(Icons.local_offer_outlined),
            ),
          ),
          const SizedBox(height: 16),
          OrgServiceDatePicker(
            selectedDate: _pickerDate,
            minDate: _dateMin,
            maxDate: _dateMax,
            ruleHint: OrgMealOrderDateRules.ruleHint(OrgMealOrderDateRules.todayLocal()),
            onDateChanged: (d) => setState(() => _pickerDate = d),
            onAddDay: _addDay,
            selectedDayIsos: _mealDays.map((d) => d.serviceDate).toList(),
            onRemoveDay: _removeDay,
            onSelectExistingDay: _selectExistingDay,
          ),
          const SizedBox(height: 20),
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: () {
                final err = _validateMenu();
                if (_mealDays.isEmpty) {
                  _showError('Thêm ít nhất một ngày phục vụ.');
                  return;
                }
                if (_pricePerPortion <= 0) {
                  _showError('Nhập giá thỏa thuận hợp lệ.');
                  return;
                }
                setState(() {
                  _step = 2;
                  _activeDay ??= _mealDays.first.serviceDate;
                });
              },
              style: FilledButton.styleFrom(
                backgroundColor: orgAccent,
                padding: const EdgeInsets.symmetric(vertical: 14),
              ),
              child: const Text('Tiếp tục — lập thực đơn'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStep2() {
    final day = _day(_activeDay);
    return Column(
      children: [
        if (_mealDays.isNotEmpty)
          SizedBox(
            height: 48,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 4),
              itemCount: _mealDays.length,
              separatorBuilder: (_, __) => const SizedBox(width: 8),
              itemBuilder: (_, i) {
                final d = _mealDays[i];
                final active = d.serviceDate == _activeDay;
                return ChoiceChip(
                  label: Text(OrgMealOrderDateRules.formatDisplay(d.serviceDate)),
                  selected: active,
                  onSelected: (_) => setState(() => _activeDay = d.serviceDate),
                  selectedColor: orgAccent.withValues(alpha: 0.2),
                );
              },
            ),
          ),
        const SizedBox(height: 12),
        if (day != null) ...[
          _slotSection(day, 'main', 'Món chính', Icons.rice_bowl_outlined),
          const SizedBox(height: 10),
          _slotSection(day, 'side', 'Món phụ', Icons.eco_outlined),
          const SizedBox(height: 10),
          _slotSection(day, 'soup', 'Canh', Icons.soup_kitchen_outlined),
        ],
        const SizedBox(height: 12),
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Tạm tính', style: AppDesignSystem.label()),
                  Text(
                    formatOrgVnd(_pricePerPortion * _totalMainQty()),
                    style: AppDesignSystem.sectionTitle(color: orgAccent),
                  ),
                ],
              ),
              Text(
                '${formatOrgVnd(_pricePerPortion)}/suất × $_totalMainQty suất chính',
                style: AppDesignSystem.body(size: 12),
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => setState(() => _step = 1),
                      child: const Text('Quay lại'),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: FilledButton(
                      onPressed: _submitting ? null : _submitDraft,
                      style: FilledButton.styleFrom(backgroundColor: orgAccent),
                      child: Text(_submitting ? 'Đang xử lý…' : 'Xem nháp hợp đồng'),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _slotSection(
    MealDayDraftModel day,
    String slot,
    String label,
    IconData icon,
  ) {
    final lines = day.linesFor(slot);
    final mainTotal = day.slotTotal('main');
    final slotTotal = day.slotTotal(slot);
    final canAdd = slot == 'main' || mainTotal == 0 || slotTotal < mainTotal;

    return OrgCard(
      padding: const EdgeInsets.all(12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: orgAccent, size: 20),
              const SizedBox(width: 8),
              Expanded(child: Text(label, style: AppDesignSystem.sectionTitle())),
              Text(
                slot == 'main' ? '$slotTotal suất' : '$slotTotal / $mainTotal',
                style: AppDesignSystem.body(size: 12),
              ),
              IconButton(
                onPressed: canAdd
                    ? () => OrgDishPickerSheet.show(
                          context,
                          categories: _categories,
                          slotKey: slot,
                          onPick: (dish) {
                            final existing = lines.where((l) => l.dishId == dish.id).toList();
                            if (existing.isNotEmpty) {
                              if (slot != 'main' && !canAdd) return;
                              setState(() => existing.first.quantity++);
                            } else {
                              setState(() {
                                lines.add(
                                  MealLineDraftModel(
                                    dishId: dish.id,
                                    dishName: dish.name,
                                  ),
                                );
                              });
                            }
                          },
                        )
                    : null,
                icon: const Icon(Icons.add_circle_outline),
                color: orgAccent,
              ),
            ],
          ),
          if (lines.isEmpty)
            Text('Chưa có món — nhấn + để thêm', style: AppDesignSystem.body(size: 12))
          else
            ...lines.map((line) {
              return ListTile(
                contentPadding: EdgeInsets.zero,
                title: Text(line.dishName, style: AppDesignSystem.label()),
                trailing: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    IconButton(
                      icon: const Icon(Icons.remove_circle_outline),
                      onPressed: () {
                        setState(() {
                          if (line.quantity > 1) {
                            line.quantity--;
                          } else {
                            lines.remove(line);
                          }
                        });
                      },
                    ),
                    Text('${line.quantity}', style: AppDesignSystem.sectionTitle()),
                    IconButton(
                      icon: const Icon(Icons.add_circle_outline),
                      onPressed: () {
                        if (slot != 'main') {
                          final main = day.slotTotal('main');
                          if (day.slotTotal(slot) >= main) {
                            _showError('Đã đạt giới hạn suất $label.');
                            return;
                          }
                        }
                        setState(() => line.quantity++);
                      },
                    ),
                  ],
                ),
              );
            }),
        ],
      ),
    );
  }
}
