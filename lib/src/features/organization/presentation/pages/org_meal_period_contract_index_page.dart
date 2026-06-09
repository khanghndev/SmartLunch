import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/models/org_meal_contract_models.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_period_contract_date_rules.dart';
import '../widgets/org_meal_period_contract_calendar.dart';
import '../widgets/organization_ui.dart';

/// Đặt suất theo hợp đồng kỳ — 4 bước khớp web `OrganizationMealContractOrder/Index`.
class OrgMealPeriodContractIndexPage extends StatefulWidget {
  const OrgMealPeriodContractIndexPage({super.key});

  @override
  State<OrgMealPeriodContractIndexPage> createState() =>
      _OrgMealPeriodContractIndexPageState();
}

class _OrgMealPeriodContractIndexPageState
    extends State<OrgMealPeriodContractIndexPage> {
  bool _loading = true;
  String? _error;
  int _orgId = 0;
  String _orgName = '';
  int _step = 1;
  String? _formError;

  late DateTime _periodStart;
  late DateTime _periodEnd;
  late DateTime _periodStartMin;
  late String _dateRuleHint;

  final _mealsPerDayCtrl = TextEditingController(text: '50');
  List<MealPortionPriceOptionModel> _portionPrices = [];
  int? _selectedDishValueId;
  double _mealUnitPrice = 0;

  final List<String> _excludedDates = [];
  final Map<String, int> _dailyOverrides = {};
  String? _selectedDayIso;

  final _recipientNameCtrl = TextEditingController();
  final _recipientPhoneCtrl = TextEditingController();
  final _recipientEmailCtrl = TextEditingController();
  final _deliveryAddressCtrl = TextEditingController();
  final _deliveryWardCtrl = TextEditingController();
  final _deliveryNotesCtrl = TextEditingController();
  final _deliveryTimeCtrl = TextEditingController(text: '11:30');
  final _manualPromoCtrl = TextEditingController();

  bool _promoLoading = false;
  bool _promoApplying = false;
  List<EligiblePromotionModel> _eligiblePromotions = [];
  String _promoListMessage = '';
  double _promoSubtotalAmount = 0;
  double _promoDiscountAmount = 0;
  int? _selectedPromotionId;
  String _selectedPromotionName = '';
  String _promotionCode = '';
  String _promoPreviewMessage = '';
  bool _promoPreviewApplied = false;

  bool _submitting = false;

  @override
  void initState() {
    super.initState();
    final today = OrgMealPeriodContractDateRules.todayLocal();
    _periodStartMin = OrgMealPeriodContractDateRules.getMinPeriodStart(today);
    _periodStart = OrgMealPeriodContractDateRules.getDefaultPeriodStart(today);
    _periodEnd = OrgMealPeriodContractDateRules.getDefaultPeriodEnd(_periodStart);
    _dateRuleHint = OrgMealPeriodContractDateRules.getRuleHint(today);
    _bootstrap();
  }

  @override
  void dispose() {
    _mealsPerDayCtrl.dispose();
    _recipientNameCtrl.dispose();
    _recipientPhoneCtrl.dispose();
    _recipientEmailCtrl.dispose();
    _deliveryAddressCtrl.dispose();
    _deliveryWardCtrl.dispose();
    _deliveryNotesCtrl.dispose();
    _deliveryTimeCtrl.dispose();
    _manualPromoCtrl.dispose();
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

      _recipientNameCtrl.text =
          (unit.legalRepresentative?.trim().isNotEmpty == true
              ? unit.legalRepresentative!
              : profile.displayName);
      _recipientPhoneCtrl.text = (unit.phone ?? profile.phoneNumber ?? '').trim();
      _recipientEmailCtrl.text = (unit.contactEmail ?? profile.email).trim();
      _deliveryAddressCtrl.text = (unit.address ?? '').trim();

      final prices = await OrgRepository.instance.getMealPortionPrices();
      if (prices.isNotEmpty) {
        _selectedDishValueId = prices.first.id;
        _mealUnitPrice = prices.first.amount;
      }

      setState(() {
        _orgId = unit.id;
        _orgName = unit.name;
        _portionPrices = prices;
        _loading = false;
      });
    } catch (e) {
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
  }

  int get _mealsPerDay {
    final v = int.tryParse(_mealsPerDayCtrl.text) ?? 0;
    return v < 1 ? 1 : v;
  }

  String get _startIso => OrgMealPeriodContractDateRules.toIsoDate(_periodStart);
  String get _endIso => OrgMealPeriodContractDateRules.toIsoDate(_periodEnd);

  int _serviceDaysCount() => OrgMealPeriodContractDateRules.countServiceDays(
        _periodStart,
        _periodEnd,
        _excludedDates,
      );

  int _totalMealsCount() => OrgMealPeriodContractDateRules.countTotalMeals(
        start: _periodStart,
        end: _periodEnd,
        excludedIso: _excludedDates,
        defaultMealsPerDay: _mealsPerDay,
        dailyOverridesByIso: _dailyOverrides,
      );

  double _estimatedTotal() => OrgMealPeriodContractDateRules.computeTotal(
        start: _periodStart,
        end: _periodEnd,
        excludedIso: _excludedDates,
        mealsPerDay: _mealsPerDay,
        mealUnitPrice: _mealUnitPrice,
        dailyOverridesByIso: _dailyOverrides,
      );

  double _promoSubtotal() =>
      _promoSubtotalAmount > 0 ? _promoSubtotalAmount : _estimatedTotal();

  double _promoDiscount() =>
      (_selectedPromotionId != null || _promotionCode.isNotEmpty)
          ? _promoDiscountAmount
          : 0;

  double _promoTotalAfter() =>
      (_promoSubtotal() - _promoDiscount()).clamp(0, double.infinity);

  List<Map<String, dynamic>> _dailyMealPortionsPayload() {
    final def = _mealsPerDay;
    return _dailyOverrides.entries
        .where(
          (e) =>
              OrgMealPeriodContractDateRules.isInRange(DateTime.parse(e.key), _periodStart, _periodEnd) &&
              !_excludedDates.contains(e.key) &&
              e.value != def,
        )
        .map((e) => {'serviceDate': e.key, 'mealCount': e.value})
        .toList();
  }

  OrganizationMealDeliveryModel _delivery() => OrganizationMealDeliveryModel(
        recipientName: _recipientNameCtrl.text,
        recipientPhone: _recipientPhoneCtrl.text,
        recipientEmail: _recipientEmailCtrl.text,
        deliveryAddress: _deliveryAddressCtrl.text,
        deliveryWardDistrict: _deliveryWardCtrl.text,
        deliveryNotes: _deliveryNotesCtrl.text,
        preferredDeliveryTime: _deliveryTimeCtrl.text,
      );

  void _onPeriodStartChanged(DateTime? d) {
    if (d == null) return;
    setState(() {
      _periodStart = OrgMealPeriodContractDateRules.dateOnly(d);
      if (_periodStart.isBefore(_periodStartMin)) {
        _periodStart = _periodStartMin;
      }
      final maxEnd = OrgMealPeriodContractDateRules.getMaxPeriodEnd(_periodStart);
      if (_periodEnd.isAfter(maxEnd)) _periodEnd = maxEnd;
      if (_periodEnd.isBefore(_periodStart)) _periodEnd = _periodStart;
      _pruneExcludedAndOverrides();
    });
  }

  void _onPeriodEndChanged(DateTime? d) {
    if (d == null) return;
    setState(() {
      _periodEnd = OrgMealPeriodContractDateRules.dateOnly(d);
      final maxEnd = OrgMealPeriodContractDateRules.getMaxPeriodEnd(_periodStart);
      if (_periodEnd.isAfter(maxEnd)) _periodEnd = maxEnd;
      if (_periodEnd.isBefore(_periodStart)) _periodEnd = _periodStart;
      _pruneExcludedAndOverrides();
    });
  }

  void _pruneExcludedAndOverrides() {
    _excludedDates.removeWhere(
      (iso) => !OrgMealPeriodContractDateRules.isInRange(DateTime.parse(iso), _periodStart, _periodEnd),
    );
    _dailyOverrides.removeWhere(
      (iso, _) =>
          !OrgMealPeriodContractDateRules.isInRange(DateTime.parse(iso), _periodStart, _periodEnd) ||
          _excludedDates.contains(iso),
    );
    if (_selectedDayIso != null &&
        !OrgMealPeriodContractDateRules.isInRange(DateTime.parse(_selectedDayIso!), _periodStart, _periodEnd)) {
      _selectedDayIso = null;
    }
  }

  void _onMealPriceTierChanged(int? id) {
    if (id == null) return;
    MealPortionPriceOptionModel? tier;
    for (final p in _portionPrices) {
      if (p.id == id) {
        tier = p;
        break;
      }
    }
    if (tier == null) return;
    setState(() {
      _selectedDishValueId = tier!.id;
      _mealUnitPrice = tier.amount;
    });
  }

  String? _validatePeriod() {
    if (_periodStart.isBefore(_periodStartMin)) {
      return 'Ngày bắt đầu phải đặt trước ít nhất 3 ngày (để chuẩn bị suất ăn).';
    }
    if (_periodEnd.isBefore(_periodStart)) {
      return 'Ngày kết thúc phải sau ngày bắt đầu.';
    }
    if (_periodEnd.isAfter(OrgMealPeriodContractDateRules.getMaxPeriodEnd(_periodStart))) {
      return 'Thời hạn hợp đồng tối đa 1 tháng.';
    }
    if (_mealsPerDay < 1) return 'Nhập số suất/ngày hợp lệ.';
    if (_portionPrices.isEmpty) {
      return 'Chưa có mức giá suất ăn. Vui lòng liên hệ quản trị.';
    }
    if (_selectedDishValueId == null || _mealUnitPrice <= 0) {
      return 'Chọn đơn giá/suất hợp lệ.';
    }
    if (_serviceDaysCount() < 1) {
      return 'Phải có ít nhất 1 ngày phục vụ sau khi loại trừ.';
    }
    return null;
  }

  String? _validateDelivery() {
    return OrganizationMealDeliveryModel.validate(
      recipientName: _recipientNameCtrl.text,
      recipientPhone: _recipientPhoneCtrl.text,
      recipientEmail: _recipientEmailCtrl.text,
      deliveryAddress: _deliveryAddressCtrl.text,
      preferredDeliveryTime: _deliveryTimeCtrl.text,
    );
  }

  void _goStep2() {
    final err = _validatePeriod();
    setState(() {
      _formError = err;
      if (err == null) _step = 2;
    });
  }

  Future<void> _goStep3() async {
    final err = _validatePeriod() ?? _validateDelivery();
    if (err != null) {
      setState(() => _formError = err);
      return;
    }
    setState(() {
      _step = 3;
      _formError = null;
    });
    await _loadEligiblePromotions();
  }

  void _goStep4() {
    final err = _validatePeriod() ?? _validateDelivery();
    setState(() {
      _formError = err;
      if (err == null) _step = 4;
    });
  }

  Future<void> _loadEligiblePromotions() async {
    setState(() {
      _promoLoading = true;
      _eligiblePromotions = [];
      _promoListMessage = '';
    });
    try {
      final result = await OrgRepository.instance.listEligiblePeriodPromotions(
        organizationId: _orgId,
        startDate: _startIso,
        endDate: _endIso,
        excludedDates: List.from(_excludedDates),
        mealsPerDay: _mealsPerDay,
        mealUnitPrice: _mealUnitPrice,
        dailyMealOverrides: Map.from(_dailyOverrides),
      );
      if (!mounted) return;
      setState(() {
        _promoSubtotalAmount =
            result.subtotal > 0 ? result.subtotal : _estimatedTotal();
        _promoListMessage = result.message ?? '';
        _eligiblePromotions = result.items;
      });
      EligiblePromotionModel? recommended;
      for (final p in _eligiblePromotions) {
        if (p.isRecommended) {
          recommended = p;
          break;
        }
      }
      recommended ??=
          _eligiblePromotions.isNotEmpty ? _eligiblePromotions.first : null;
      if (recommended != null) {
        _selectPromotion(recommended, showMessage: false);
      } else {
        _clearPromotion(showMessage: false);
      }
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _promoListMessage = orgApiError(e);
        _promoSubtotalAmount = _estimatedTotal();
      });
      _clearPromotion(showMessage: false);
    } finally {
      if (mounted) setState(() => _promoLoading = false);
    }
  }

  void _selectPromotion(EligiblePromotionModel p, {bool showMessage = true}) {
    setState(() {
      _selectedPromotionId = p.promotionId;
      _selectedPromotionName = p.promotionName;
      _promotionCode = p.promotionCode.trim();
      _promoDiscountAmount = p.discountAmount;
      if (_promoSubtotalAmount <= 0) _promoSubtotalAmount = _estimatedTotal();
      _manualPromoCtrl.text = _promotionCode;
      if (showMessage) {
        _promoPreviewApplied = true;
        _promoPreviewMessage = 'Đã chọn: ${p.promotionName}';
      }
    });
  }

  void _clearPromotion({bool showMessage = true}) {
    setState(() {
      _selectedPromotionId = null;
      _selectedPromotionName = '';
      _promotionCode = '';
      _promoDiscountAmount = 0;
      _manualPromoCtrl.clear();
      if (showMessage) {
        _promoPreviewApplied = false;
        _promoPreviewMessage = '';
      }
    });
  }

  Future<void> _applyManualCode() async {
    final code = _manualPromoCtrl.text.trim();
    if (code.isEmpty) {
      setState(() {
        _promoPreviewApplied = false;
        _promoPreviewMessage = 'Nhập mã khuyến mãi.';
      });
      return;
    }
    setState(() {
      _promoApplying = true;
      _promoPreviewMessage = '';
    });
    try {
      final preview = await OrgRepository.instance.previewPeriodPromotion(
        organizationId: _orgId,
        startDate: _startIso,
        endDate: _endIso,
        excludedDates: List.from(_excludedDates),
        mealsPerDay: _mealsPerDay,
        mealUnitPrice: _mealUnitPrice,
        promotionCode: code,
        dailyMealOverrides: Map.from(_dailyOverrides),
      );
      if (!mounted) return;
      if (!preview.applied) {
        setState(() {
          _promoPreviewApplied = false;
          _promoPreviewMessage = preview.message ?? 'Mã không áp dụng được.';
        });
        return;
      }
      setState(() {
        _promotionCode = (preview.promotionCode ?? code).trim();
        _selectedPromotionId = preview.promotionId;
        _selectedPromotionName = preview.promotionName ?? code;
        _promoDiscountAmount = preview.discountAmount;
        _promoSubtotalAmount = preview.subtotal > 0 ? preview.subtotal : _estimatedTotal();
        _promoPreviewApplied = true;
        _promoPreviewMessage = 'Đã áp dụng: ${preview.promotionName ?? code}';
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _promoPreviewApplied = false;
        _promoPreviewMessage = orgApiError(e);
      });
    } finally {
      if (mounted) setState(() => _promoApplying = false);
    }
  }

  Future<void> _submitContract() async {
    final err = _validatePeriod() ?? _validateDelivery();
    if (err != null) {
      setState(() => _formError = err);
      return;
    }

    setState(() {
      _submitting = true;
      _formError = null;
    });

    try {
      final draft = await OrgRepository.instance.prepareMealPeriodContract(
        organizationId: _orgId,
        startDate: _startIso,
        endDate: _endIso,
        excludedDates: List.from(_excludedDates),
        mealsPerDay: _mealsPerDay,
        mealUnitPrice: _mealUnitPrice,
        delivery: _delivery(),
        dailyMealPortions: _dailyMealPortionsPayload(),
        dishValueId: _selectedDishValueId,
        promotionCode:
            _selectedPromotionId == null && _promotionCode.isNotEmpty
                ? _promotionCode
                : null,
        promotionId: _selectedPromotionId,
      );

      if (!mounted) return;
      if (draft.draftId.isEmpty) {
        throw ApiException('Không tạo được bản nháp hợp đồng.', statusCode: 500);
      }

      await Navigator.of(context).pushNamed(
        AppRoutes.orgMealPeriodContractReview,
        arguments: draft,
      );
    } catch (e) {
      if (!mounted) return;
      setState(() => _formError = orgApiError(e));
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  Future<void> _pickDate({
    required DateTime initial,
    required DateTime first,
    required DateTime last,
    required ValueChanged<DateTime?> onPicked,
  }) async {
    if (first.isAfter(last)) return;
    final picked = await showDatePicker(
      context: context,
      initialDate: initial.isBefore(first)
          ? first
          : (initial.isAfter(last) ? last : initial),
      firstDate: first,
      lastDate: last,
      helpText: 'Chọn ngày',
      cancelText: 'Hủy',
      confirmText: 'Chọn',
    );
    onPicked(picked);
  }

  Widget _buildPeriodDateField({
    required String label,
    required DateTime date,
    required VoidCallback onTap,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          label,
          style: AppDesignSystem.body(size: 11).copyWith(
            fontWeight: FontWeight.w800,
            color: AppDesignSystem.gray500,
          ),
        ),
        const SizedBox(height: 6),
        Material(
          color: Colors.white,
          borderRadius: BorderRadius.circular(12),
          child: InkWell(
            onTap: onTap,
            borderRadius: BorderRadius.circular(12),
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppDesignSystem.gray200),
              ),
              child: Row(
                children: [
                  Icon(Icons.event_outlined, size: 18, color: orgAccent),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Text(
                      OrgMealPeriodContractDateRules.formatDisplay(date),
                      style: AppDesignSystem.body().copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  Icon(Icons.expand_more, color: AppDesignSystem.gray400),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đặt suất theo hợp đồng',
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tải đơn vị & mức giá suất…')
          : _error != null
              ? OrgErrorBody(message: _error!, onRetry: _bootstrap)
              : ModuleListView(
                  padding: orgListPadding(context),
                  children: [
                    _buildHero(),
                    const SizedBox(height: 16),
                    _buildStepper(),
                    const SizedBox(height: 16),
                    if (_step == 1)
                      _buildStep1()
                    else if (_step == 2)
                      _buildStep2Content()
                    else if (_step == 3)
                      _buildStep3Content()
                    else
                      _buildStep4Content(),
                  ],
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
            'Hợp đồng theo kỳ',
            style: AppDesignSystem.title(size: 20, color: Colors.white),
          ),
          const SizedBox(height: 6),
          Text(
            'Chọn kỳ, số suất/ngày và đơn giá. Món chính chọn theo tuần sau khi ký HĐ.',
            style: AppDesignSystem.body(color: Colors.white.withValues(alpha: 0.9)),
          ),
          if (_orgName.isNotEmpty) ...[
            const SizedBox(height: 10),
            Text(
              _orgName,
              style: AppDesignSystem.label().copyWith(color: Colors.white),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildStepper() {
    const labels = ['Kỳ HĐ', 'Giao hàng', 'Khuyến mãi', 'Xác nhận'];
    const hints = ['Ngày & đơn giá', 'Người nhận', 'Mã giảm giá', 'Tạo nháp'];
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 4),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppDesignSystem.gray200),
      ),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Row(
          children: List.generate(4, (i) {
            final n = i + 1;
            final active = _step == n;
            final done = _step > n;
            return SizedBox(
              width: 88,
              child: Column(
                children: [
                  CircleAvatar(
                    radius: 16,
                    backgroundColor: done
                        ? AppDesignSystem.success
                        : active
                            ? orgAccent
                            : AppDesignSystem.gray100,
                    child: done
                        ? const Icon(Icons.check, color: Colors.white, size: 16)
                        : Text(
                            '$n',
                            style: TextStyle(
                              color: active ? Colors.white : AppDesignSystem.gray500,
                              fontWeight: FontWeight.w800,
                              fontSize: 12,
                            ),
                          ),
                  ),
                  const SizedBox(height: 6),
                  Text(
                    labels[i],
                    textAlign: TextAlign.center,
                    style: AppDesignSystem.body(size: 11).copyWith(
                      fontWeight: FontWeight.w800,
                      color: active ? orgAccent : AppDesignSystem.gray500,
                    ),
                  ),
                  Text(
                    hints[i],
                    textAlign: TextAlign.center,
                    style: AppDesignSystem.body(size: 9, color: AppDesignSystem.gray400),
                  ),
                ],
              ),
            );
          }),
        ),
      ),
    );
  }

  Widget _buildSummaryCard({required String title, required List<Widget> rows}) {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(title, style: AppDesignSystem.label()),
          const SizedBox(height: 10),
          ...rows,
        ],
      ),
    );
  }

  Widget _summaryRow(String label, String value, {Color? valueColor}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: AppDesignSystem.body(size: 13)),
          Text(
            value,
            style: AppDesignSystem.body(size: 13).copyWith(
              fontWeight: FontWeight.w800,
              color: valueColor ?? AppDesignSystem.gray900,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStep1() {
    final maxEnd = OrgMealPeriodContractDateRules.getMaxPeriodEnd(_periodStart);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const OrgSectionHeader(
                title: 'Bước 1 — Kỳ hợp đồng & đơn giá',
                subtitle: 'Thời hạn tối đa 1 tháng · loại trừ ngày nghỉ trên lịch',
              ),
              const SizedBox(height: 12),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: _buildPeriodDateField(
                      label: 'Ngày bắt đầu *',
                      date: _periodStart,
                      onTap: () => _pickDate(
                        initial: _periodStart,
                        first: _periodStartMin,
                        last: maxEnd,
                        onPicked: _onPeriodStartChanged,
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _buildPeriodDateField(
                      label: 'Ngày kết thúc *',
                      date: _periodEnd,
                      onTap: () => _pickDate(
                        initial: _periodEnd,
                        first: _periodStart,
                        last: maxEnd,
                        onPicked: _onPeriodEndChanged,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              TextField(
                controller: _mealsPerDayCtrl,
                keyboardType: TextInputType.number,
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                onChanged: (_) => setState(_pruneExcludedAndOverrides),
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Suất ăn / ngày',
                  focusColor: orgAccent,
                ),
              ),
              const SizedBox(height: 12),
              if (_portionPrices.isEmpty)
                OrgInfoBanner(
                  message: 'Chưa có mức giá suất ăn trên hệ thống. Vui lòng liên hệ quản trị.',
                  icon: Icons.warning_amber_outlined,
                  color: AppDesignSystem.warning,
                )
              else
                DropdownButtonFormField<int>(
                  value: _selectedDishValueId,
                  decoration: AppDesignSystem.inputDecoration(
                    label: 'Đơn giá / suất (VND)',
                    focusColor: orgAccent,
                  ),
                  items: _portionPrices
                      .map(
                        (p) => DropdownMenuItem(
                          value: p.id,
                          child: Text(p.label),
                        ),
                      )
                      .toList(),
                  onChanged: _onMealPriceTierChanged,
                ),
              const SizedBox(height: 12),
              OrgInfoBanner(
                message: _dateRuleHint,
                icon: Icons.info_outline,
                color: AppDesignSystem.info,
              ),
              const SizedBox(height: 14),
              OrgMealPeriodContractCalendar(
                periodStart: _periodStart,
                periodEnd: _periodEnd,
                mealsPerDay: _mealsPerDay,
                excludedDates: _excludedDates,
                dailyOverrides: _dailyOverrides,
                selectedDayIso: _selectedDayIso,
                onSelectedDayChanged: (iso) => setState(() => _selectedDayIso = iso),
                onToggleExcluded: (iso) {
                  setState(() {
                    if (_excludedDates.contains(iso)) {
                      _excludedDates.remove(iso);
                    } else {
                      _excludedDates.add(iso);
                      _excludedDates.sort();
                      _dailyOverrides.remove(iso);
                    }
                  });
                },
                onApplyDayMeals: (iso, count) {
                  setState(() {
                    if (count == _mealsPerDay) {
                      _dailyOverrides.remove(iso);
                    } else {
                      _dailyOverrides[iso] = count;
                    }
                  });
                },
                onClearDayMeals: (iso) {
                  setState(() {
                    _dailyOverrides.remove(iso);
                  });
                },
              ),
              if (_formError != null) ...[
                const SizedBox(height: 10),
                OrgInfoBanner(
                  message: _formError!,
                  icon: Icons.error_outline,
                  color: AppDesignSystem.danger,
                ),
              ],
              const SizedBox(height: 14),
              FilledButton.icon(
                onPressed: _goStep2,
                icon: const Icon(Icons.arrow_forward),
                label: const Text('Tiếp tục — giao hàng'),
                style: FilledButton.styleFrom(
                  backgroundColor: orgAccent,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _buildSummaryCard(
          title: 'Tổng dự kiến',
          rows: [
            _summaryRow('Ngày phục vụ', '${_serviceDaysCount()}'),
            _summaryRow('Tổng suất', '${_totalMealsCount()}'),
            _summaryRow('Đơn giá', formatOrgVnd(_mealUnitPrice)),
            _summaryRow(
              'Tổng HĐ',
              formatOrgVnd(_estimatedTotal()),
              valueColor: AppDesignSystem.success,
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStep2Content() {
    return Column(
      children: [
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const OrgSectionHeader(
                title: 'Bước 2 — Địa điểm giao & người nhận',
                subtitle: 'Email xác nhận và PDF hợp đồng gửi tới email bên dưới',
              ),
              const SizedBox(height: 12),
              TextField(
                controller: _recipientNameCtrl,
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Họ tên người nhận',
                  focusColor: orgAccent,
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _recipientPhoneCtrl,
                keyboardType: TextInputType.phone,
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Số điện thoại',
                  focusColor: orgAccent,
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _recipientEmailCtrl,
                keyboardType: TextInputType.emailAddress,
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Email nhận thông báo',
                  focusColor: orgAccent,
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _deliveryAddressCtrl,
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Địa chỉ giao hàng',
                  focusColor: orgAccent,
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _deliveryWardCtrl,
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Phường / quận / thành phố',
                  focusColor: orgAccent,
                ),
              ),
              const SizedBox(height: 10),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _deliveryTimeCtrl,
                      decoration: AppDesignSystem.inputDecoration(
                        label: 'Giờ giao (HH:mm)',
                        focusColor: orgAccent,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _deliveryNotesCtrl,
                decoration: AppDesignSystem.inputDecoration(
                  label: 'Ghi chú giao hàng',
                  focusColor: orgAccent,
                ),
              ),
              if (_formError != null) ...[
                const SizedBox(height: 10),
                OrgInfoBanner(
                  message: _formError!,
                  icon: Icons.error_outline,
                  color: AppDesignSystem.danger,
                ),
              ],
              const SizedBox(height: 14),
              OutlinedButton(
                onPressed: () => setState(() => _step = 1),
                child: const Text('Quay lại kỳ HĐ'),
              ),
              const SizedBox(height: 8),
              FilledButton.icon(
                onPressed: _goStep3,
                icon: const Icon(Icons.arrow_forward),
                label: const Text('Khuyến mãi'),
                style: FilledButton.styleFrom(
                  backgroundColor: const Color(0xFF7C3AED),
                  padding: const EdgeInsets.symmetric(vertical: 14),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _buildSummaryCard(
          title: 'Tổng dự kiến',
          rows: [
            _summaryRow(
              'Tổng HĐ',
              formatOrgVnd(_estimatedTotal()),
              valueColor: AppDesignSystem.success,
            ),
          ],
        ),
      ],
    );
  }

  Widget _promoTile(EligiblePromotionModel p) {
    final selected = _selectedPromotionId == p.promotionId;
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Material(
        color: selected ? const Color(0xFFF5F3FF) : Colors.white,
        borderRadius: BorderRadius.circular(14),
        child: InkWell(
          onTap: () => _selectPromotion(p),
          borderRadius: BorderRadius.circular(14),
          child: Ink(
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(14),
              border: Border.all(
                color: selected ? const Color(0xFF7C3AED) : AppDesignSystem.gray200,
                width: selected ? 2 : 1,
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(p.promotionName, style: AppDesignSystem.label()),
                Text(
                  p.promotionCode,
                  style: AppDesignSystem.body(size: 12).copyWith(
                    color: const Color(0xFF6D28D9),
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  '−${formatOrgVnd(p.discountAmount)}',
                  style: AppDesignSystem.label().copyWith(
                    color: AppDesignSystem.success,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildStep3Content() {
    return Column(
      children: [
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const OrgSectionHeader(
                title: 'Bước 3 — Chọn mã khuyến mãi',
                subtitle: 'Có thể bỏ qua nếu không dùng KM',
              ),
              if (_promoLoading)
                const Padding(
                  padding: EdgeInsets.symmetric(vertical: 24),
                  child: Center(child: CircularProgressIndicator()),
                )
              else if (_eligiblePromotions.isEmpty)
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  child: Text(
                    _promoListMessage.isNotEmpty
                        ? _promoListMessage
                        : 'Không có mã khuyến mãi phù hợp. Bạn vẫn có thể tiếp tục.',
                    style: AppDesignSystem.body(size: 13),
                    textAlign: TextAlign.center,
                  ),
                )
              else
                ..._eligiblePromotions.map(_promoTile),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _manualPromoCtrl,
                      textCapitalization: TextCapitalization.characters,
                      decoration: const InputDecoration(hintText: 'VD: DONVI10'),
                      onSubmitted: (_) => _applyManualCode(),
                    ),
                  ),
                  const SizedBox(width: 8),
                  FilledButton(
                    onPressed: _promoApplying ? null : _applyManualCode,
                    style: FilledButton.styleFrom(
                      backgroundColor: const Color(0xFF7C3AED),
                    ),
                    child: Text(_promoApplying ? '…' : 'Áp dụng'),
                  ),
                ],
              ),
              if (_promoPreviewMessage.isNotEmpty)
                Padding(
                  padding: const EdgeInsets.only(top: 8),
                  child: Text(
                    _promoPreviewMessage,
                    style: AppDesignSystem.body(size: 12).copyWith(
                      color: _promoPreviewApplied
                          ? AppDesignSystem.success
                          : AppDesignSystem.danger,
                    ),
                  ),
                ),
              RadioListTile<int?>(
                value: null,
                groupValue: _selectedPromotionId,
                onChanged: (_) => _clearPromotion(),
                title: const Text('Không sử dụng khuyến mãi'),
                contentPadding: EdgeInsets.zero,
                dense: true,
              ),
              OutlinedButton(
                onPressed: () => setState(() => _step = 2),
                child: const Text('Quay lại giao hàng'),
              ),
              const SizedBox(height: 8),
              FilledButton.icon(
                onPressed: _goStep4,
                icon: const Icon(Icons.arrow_forward),
                label: const Text('Xem tóm tắt'),
                style: FilledButton.styleFrom(
                  backgroundColor: const Color(0xFF7C3AED),
                  padding: const EdgeInsets.symmetric(vertical: 14),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _buildSummaryCard(
          title: 'Tổng hợp đồng',
          rows: [
            _summaryRow('Tạm tính', formatOrgVnd(_promoSubtotal())),
            if (_promoDiscount() > 0)
              _summaryRow(
                'Giảm giá',
                '−${formatOrgVnd(_promoDiscount())}',
                valueColor: AppDesignSystem.success,
              ),
            _summaryRow(
              'Sau KM',
              formatOrgVnd(_promoTotalAfter()),
              valueColor: orgAccent,
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStep4Content() {
    final delivery = _delivery();
    return Column(
      children: [
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const OrgSectionHeader(
                title: 'Bước 4 — Tóm tắt & tạo nháp hợp đồng',
                subtitle: 'Kiểm tra lại trước khi gọi API tạo nháp',
              ),
              const SizedBox(height: 10),
              _summaryRow(
                'Kỳ HĐ',
                '${OrgMealPeriodContractDateRules.formatDisplay(_periodStart)} – ${OrgMealPeriodContractDateRules.formatDisplay(_periodEnd)}',
              ),
              _summaryRow('Ngày phục vụ', '${_serviceDaysCount()}'),
              _summaryRow('Suất mặc định/ngày', '${_mealsPerDay}'),
              _summaryRow('Tổng suất', '${_totalMealsCount()}'),
              _summaryRow('Đơn giá/suất', formatOrgVnd(_mealUnitPrice)),
              const Divider(height: 24),
              Text('Giao hàng', style: AppDesignSystem.label()),
              const SizedBox(height: 6),
              Text(delivery.recipientName, style: AppDesignSystem.body()),
              Text(
                '${delivery.recipientPhone} · ${delivery.recipientEmail}',
                style: AppDesignSystem.body(size: 12),
              ),
              Text(delivery.deliveryAddress, style: AppDesignSystem.body(size: 12)),
              if (_promoDiscount() > 0 || _selectedPromotionName.isNotEmpty) ...[
                const Divider(height: 24),
                Text('Khuyến mãi', style: AppDesignSystem.label()),
                if (_selectedPromotionName.isNotEmpty)
                  Text(_selectedPromotionName, style: AppDesignSystem.body()),
                if (_promoDiscount() > 0)
                  Text(
                    'Giảm ${formatOrgVnd(_promoDiscount())}',
                    style: AppDesignSystem.body(color: AppDesignSystem.success),
                  ),
              ],
              if (_formError != null) ...[
                const SizedBox(height: 10),
                OrgInfoBanner(
                  message: _formError!,
                  icon: Icons.error_outline,
                  color: AppDesignSystem.danger,
                ),
              ],
              const SizedBox(height: 14),
              OutlinedButton(
                onPressed: () => setState(() => _step = 3),
                child: const Text('Quay lại khuyến mãi'),
              ),
              const SizedBox(height: 8),
              FilledButton.icon(
                onPressed: _submitting ? null : _submitContract,
                icon: _submitting
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.file_copy_outlined),
                label: Text(_submitting ? 'Đang xử lý…' : 'Tạo nháp hợp đồng'),
                style: FilledButton.styleFrom(
                  backgroundColor: orgAccent,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        _buildSummaryCard(
          title: 'Tổng hợp đồng',
          rows: [
            _summaryRow('Tạm tính', formatOrgVnd(_promoSubtotal())),
            if (_promoDiscount() > 0)
              _summaryRow('Giảm', '−${formatOrgVnd(_promoDiscount())}'),
            _summaryRow(
              'Thanh toán dự kiến',
              formatOrgVnd(_promoTotalAfter()),
              valueColor: AppDesignSystem.success,
            ),
          ],
        ),
      ],
    );
  }
}
