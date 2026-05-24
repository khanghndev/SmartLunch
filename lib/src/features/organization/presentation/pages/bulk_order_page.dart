import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/models/user_profile_model.dart';
import '../../../profile/data/profile_repository.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/org_repository.dart';
import '../../utils/org_meal_order_date_rules.dart';
import '../widgets/org_dish_picker_sheet.dart';
import '../widgets/org_meal_order_widgets.dart';
import '../widgets/org_service_date_picker.dart';
import '../widgets/organization_ui.dart';

/// Đặt suất tập trung — flow khớp web `OrganizationMealOrder/Index` (4 bước + Review).
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
  final _manualPromoCtrl = TextEditingController();
  final _recipientNameCtrl = TextEditingController();
  final _recipientPhoneCtrl = TextEditingController();
  final _recipientEmailCtrl = TextEditingController();
  final _deliveryAddressCtrl = TextEditingController();
  final _deliveryWardCtrl = TextEditingController();
  final _deliveryNotesCtrl = TextEditingController();
  final _deliveryTimeCtrl = TextEditingController(text: '11:30');
  final List<MealDayDraftModel> _mealDays = [];
  String? _activeDay;
  bool _submitting = false;

  List<EligiblePromotionModel> _eligiblePromotions = [];
  int? _selectedPromotionId;
  String _selectedPromotionName = '';
  String _promotionCode = '';
  double _promoDiscountAmount = 0;
  double _promoSubtotalAmount = 0;
  String _promoListMessage = '';
  bool _promoLoading = false;
  bool _promoApplying = false;
  String _promoPreviewMessage = '';
  bool _promoPreviewApplied = false;
  String? _formError;

  @override
  void initState() {
    super.initState();
    _pickerDate = OrgMealOrderDateRules.defaultServiceDate(OrgMealOrderDateRules.todayLocal());
    _bootstrap();
  }

  double get _pricePerPortion => double.tryParse(_priceCtrl.text) ?? 0;

  double _estimatedTotal() => _pricePerPortion * _totalMainQty();

  double _promoSubtotal() =>
      _promoSubtotalAmount > 0 ? _promoSubtotalAmount : _estimatedTotal();

  double _promoDiscount() =>
      (_selectedPromotionId != null || _promotionCode.isNotEmpty)
          ? _promoDiscountAmount
          : 0;

  double _promoTotalAfter() {
    final after = _promoSubtotal() - _promoDiscount();
    return after < 0 ? 0 : after;
  }

  @override
  void dispose() {
    _priceCtrl.dispose();
    _manualPromoCtrl.dispose();
    _recipientNameCtrl.dispose();
    _recipientPhoneCtrl.dispose();
    _recipientEmailCtrl.dispose();
    _deliveryAddressCtrl.dispose();
    _deliveryWardCtrl.dispose();
    _deliveryNotesCtrl.dispose();
    _deliveryTimeCtrl.dispose();
    super.dispose();
  }

  OrganizationMealDeliveryModel _buildDeliveryPayload() {
    return OrganizationMealDeliveryModel(
      recipientName: _recipientNameCtrl.text,
      recipientPhone: _recipientPhoneCtrl.text,
      recipientEmail: _recipientEmailCtrl.text,
      deliveryAddress: _deliveryAddressCtrl.text,
      deliveryWardDistrict: _deliveryWardCtrl.text,
      deliveryNotes: _deliveryNotesCtrl.text,
      preferredDeliveryTime: _deliveryTimeCtrl.text,
    );
  }

  void _prefillDeliveryFromProfile(UserProfileModel profile) {
    final unit = profile.unit;
    if (_recipientNameCtrl.text.isEmpty) {
      _recipientNameCtrl.text = (unit?.legalRepresentative?.trim().isNotEmpty == true
              ? unit!.legalRepresentative!
              : profile.displayName)
          .trim();
    }
    if (_recipientPhoneCtrl.text.isEmpty) {
      _recipientPhoneCtrl.text =
          (unit?.phone ?? profile.phoneNumber ?? '').trim();
    }
    if (_recipientEmailCtrl.text.isEmpty) {
      _recipientEmailCtrl.text =
          (unit?.contactEmail ?? profile.email).trim();
    }
    if (_deliveryAddressCtrl.text.isEmpty) {
      _deliveryAddressCtrl.text = (unit?.address ?? '').trim();
    }
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
      final dateMin = OrgMealOrderDateRules.resolveMinDate(
        cats.allowedFirstServiceDate,
        today,
      );
      final dateMax = OrgMealOrderDateRules.resolveMaxDate(
        cats.allowedLastServiceDate,
        today,
        dateMin,
      );
      final picker = OrgMealOrderDateRules.clamp(
        OrgMealOrderDateRules.defaultServiceDate(today),
        dateMin,
        dateMax,
      );

      _prefillDeliveryFromProfile(profile);

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
        _error = orgApiError(e);
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
        content: Text(
          'Đã thêm ${OrgMealOrderDateRules.weekdayShort(day)} · '
          '${OrgMealOrderDateRules.formatDisplay(iso)}',
        ),
        backgroundColor: AppDesignSystem.success,
      ),
    );
  }

  void _quickAddDefaultMonday() {
    final today = OrgMealOrderDateRules.todayLocal();
    final day = OrgMealOrderDateRules.clamp(
      OrgMealOrderDateRules.defaultServiceDate(today),
      _dateMin,
      _dateMax,
    );
    final iso = OrgMealOrderDateRules.toIsoDate(day);
    if (_mealDays.any((d) => d.serviceDate == iso)) {
      setState(() {
        _pickerDate = day;
        _activeDay = iso;
      });
      _showError('Thứ 2 tuần sau đã có trong lịch.');
      return;
    }
    setState(() {
      _pickerDate = day;
      _mealDays.add(MealDayDraftModel(serviceDate: iso));
      _mealDays.sort((a, b) => a.serviceDate.compareTo(b.serviceDate));
      _activeDay = iso;
    });
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          'Đã thêm ${OrgMealOrderDateRules.formatDisplayDate(day)} (Thứ 2 tuần sau)',
        ),
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

  Future<void> _goStep3() async {
    final err = _validateMenu();
    if (err != null) {
      _failValidation(err);
      return;
    }
    if (_totalMainQty() <= 0) {
      _failValidation('Chọn ít nhất một món chính trước khi tiếp tục.');
      return;
    }
    setState(() {
      _step = 3;
      _formError = null;
    });
    await _loadEligiblePromotions();
  }

  void _goStep4() {
    final err = _validateMenu();
    if (err != null) {
      _failValidation(err);
      return;
    }
    if (_totalMainQty() <= 0) {
      _failValidation('Chọn ít nhất một món chính trước khi tiếp tục.');
      return;
    }
    setState(() {
      _step = 4;
      _formError = null;
    });
  }

  Future<void> _loadEligiblePromotions() async {
    setState(() {
      _promoLoading = true;
      _eligiblePromotions = [];
      _promoListMessage = '';
    });
    try {
      final result = await OrgRepository.instance.listEligibleMealPromotions(
        organizationId: _organizationId,
        price: _pricePerPortion,
        mealDays: List.from(_mealDays),
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
      if (_promoSubtotalAmount <= 0) {
        _promoSubtotalAmount = _estimatedTotal();
      }
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
      final preview = await OrgRepository.instance.previewMealPromotion(
        organizationId: _organizationId,
        price: _pricePerPortion,
        mealDays: List.from(_mealDays),
        promotionCode: code,
      );
      if (!mounted) return;
      if (!preview.applied) {
        setState(() {
          _promoPreviewApplied = false;
          _promoPreviewMessage =
              preview.message ?? 'Mã không áp dụng được cho đơn này.';
        });
        return;
      }
      setState(() {
        _promotionCode = (preview.promotionCode ?? code).trim();
        _selectedPromotionId = preview.promotionId;
        _selectedPromotionName = preview.promotionName ?? code;
        _promoDiscountAmount = preview.discountAmount;
        _promoSubtotalAmount =
            preview.subtotal > 0 ? preview.subtotal : _estimatedTotal();
        _promoPreviewApplied = true;
        _promoPreviewMessage =
            'Đã áp dụng: ${preview.promotionName ?? code}';
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

  Future<void> _submitDraft() async {
    final err = _validateMenu();
    if (err != null) {
      _failValidation(err);
      return;
    }
    if (_totalMainQty() <= 0) {
      _failValidation('Đơn chưa có suất món chính. Quay lại bước Thực đơn để chọn món.');
      return;
    }
    setState(() {
      _submitting = true;
      _formError = null;
    });
    try {
      final delivery = _buildDeliveryPayload();
      final deliveryErr = OrganizationMealDeliveryModel.validate(
        recipientName: delivery.recipientName,
        recipientPhone: delivery.recipientPhone,
        recipientEmail: delivery.recipientEmail,
        deliveryAddress: delivery.deliveryAddress,
        preferredDeliveryTime: delivery.preferredDeliveryTime,
      );
      if (deliveryErr != null) {
        _failValidation(deliveryErr);
        setState(() => _submitting = false);
        return;
      }

      final draft = await OrgRepository.instance.prepareMealContract(
        organizationId: _organizationId,
        price: _pricePerPortion,
        mealDays: List.from(_mealDays),
        delivery: delivery,
        promotionCode:
            _promotionCode.isEmpty ? null : _promotionCode,
        promotionId: _selectedPromotionId,
        organizationName: _organizationName,
      );
      if (!mounted) return;
      await Navigator.of(context).pushNamed(
        AppRoutes.orgMealOrderReview,
        arguments: draft,
      );
    } catch (e) {
      _failValidation(orgApiError(e));
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  void _showError(String msg) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(msg), backgroundColor: AppDesignSystem.danger),
    );
  }

  void _failValidation(String msg) {
    setState(() => _formError = msg);
    _showError(msg);
  }

  String? _validateStep1() {
    if (_pricePerPortion <= 0) return 'Nhập giá thỏa thuận / suất hợp lệ.';
    if (_mealDays.isEmpty) {
      return 'Thêm ít nhất một ngày phục vụ (dùng nút Thêm nhanh Thứ 2 tuần sau).';
    }
    return null;
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đặt suất ăn tập trung',
      body: _loading
          ? const OrgLoadingBody(message: 'Đang tải thực đơn & đơn vị…')
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
                      _buildStep2()
                    else if (_step == 3)
                      _buildStep3()
                    else
                      _buildStep4(),
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
    const labels = ['Thiết lập', 'Thực đơn', 'Khuyến mãi', 'Giao hàng'];
    const hints = ['Giá & ngày giao', 'Chọn món từng ngày', 'Chọn mã giảm giá', 'Người nhận & địa chỉ'];
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
                    style: AppDesignSystem.body(size: 10).copyWith(
                      fontWeight: active ? FontWeight.w800 : FontWeight.w500,
                      color: active
                          ? orgAccent
                          : (done ? AppDesignSystem.success : AppDesignSystem.gray500),
                    ),
                  ),
                  Text(
                    hints[i],
                    textAlign: TextAlign.center,
                    style: AppDesignSystem.body(size: 8, color: AppDesignSystem.gray400),
                  ),
                ],
              ),
            );
          }),
        ),
      ),
    );
  }

  Widget _buildStep1() {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const OrgSectionHeader(
            title: 'Bước 1 — Thiết lập đơn hàng',
            subtitle: 'Giá thỏa thuận và ngày phục vụ',
          ),
          if (_formError != null && _step == 1) ...[
            const SizedBox(height: 10),
            OrgInfoBanner(
              message: _formError!,
              icon: Icons.error_outline,
              color: AppDesignSystem.danger,
            ),
          ],
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
          const SizedBox(height: 16),
          OrgServiceDatePicker(
            selectedDate: _pickerDate,
            minDate: _dateMin,
            maxDate: _dateMax,
            ruleHint: OrgMealOrderDateRules.ruleHint(OrgMealOrderDateRules.todayLocal()),
            onDateChanged: (d) => setState(() => _pickerDate = d),
            onAddDay: _addDay,
            onQuickAddDefaultMonday: _quickAddDefaultMonday,
            selectedDayIsos: _mealDays.map((d) => d.serviceDate).toList(),
            onRemoveDay: _removeDay,
            onSelectExistingDay: _selectExistingDay,
          ),
          const SizedBox(height: 20),
          OrgPrimaryButton(
            label: 'Tiếp tục — lập thực đơn',
            icon: Icons.arrow_forward_rounded,
            onPressed: () {
              final err = _validateStep1();
              if (err != null) {
                _failValidation(err);
                return;
              }
              setState(() {
                _step = 2;
                _activeDay ??= _mealDays.first.serviceDate;
                _formError = null;
              });
            },
          ),
        ],
      ),
    );
  }

  Widget _buildStep2() {
    final day = _day(_activeDay);
    final mainQty = _totalMainQty();
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (_formError != null) ...[
          OrgInfoBanner(
            message: _formError!,
            icon: Icons.error_outline,
            color: AppDesignSystem.danger,
          ),
          const SizedBox(height: 10),
        ],
        OrgCard(
          padding: const EdgeInsets.all(14),
          child: Row(
            children: [
              Icon(Icons.restaurant_menu_rounded, color: orgAccent),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Bước 2 — Lập thực đơn', style: AppDesignSystem.sectionTitle()),
                    Text(
                      '${_mealDays.length} ngày · $mainQty suất chính',
                      style: AppDesignSystem.body(size: 12),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 12),
        OrgMealDayTabBar(
          mealDays: _mealDays,
          activeDayIso: _activeDay,
          onDaySelected: (iso) {
            _selectExistingDay(iso);
            setState(() => _activeDay = iso);
          },
        ),
        const SizedBox(height: 14),
        if (day != null) ...[
          _slotSection(day, 'main', 'Món chính', Icons.rice_bowl_outlined, orgAccent),
          const SizedBox(height: 10),
          _slotSection(
            day,
            'side',
            'Món phụ',
            Icons.eco_outlined,
            AppDesignSystem.success,
          ),
          const SizedBox(height: 10),
          _slotSection(
            day,
            'soup',
            'Canh',
            Icons.soup_kitchen_outlined,
            const Color(0xFF0EA5E9),
          ),
        ],
        const SizedBox(height: 14),
        OrgMealOrderSummaryBar(
          pricePerPortion: _pricePerPortion,
          totalMainQty: mainQty,
          estimatedTotal: _estimatedTotal(),
          primaryLabel: mainQty > 0
              ? 'Tiếp — chọn khuyến mãi'
              : 'Chọn món chính trước',
          onBack: () => setState(() {
            _step = 1;
            _formError = null;
          }),
          onPrimary: mainQty > 0
              ? _goStep3
              : () => _failValidation(
                    'Mỗi ngày cần ít nhất một món chính (suất > 0).',
                  ),
        ),
      ],
    );
  }

  Widget _buildStep3() {
    return Column(
      children: [
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const OrgSectionHeader(
                title: 'Bước 3 — Chọn mã khuyến mãi',
                subtitle:
                    'Chọn từ danh sách hoặc nhập mã. Có thể bỏ qua nếu không dùng KM.',
              ),
              if (_promoLoading)
                const Padding(
                  padding: EdgeInsets.symmetric(vertical: 24),
                  child: Center(child: CircularProgressIndicator()),
                )
              else if (_eligiblePromotions.isEmpty)
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  child: Column(
                    children: [
                      Icon(Icons.confirmation_number_outlined,
                          size: 40, color: AppDesignSystem.gray400),
                      const SizedBox(height: 8),
                      Text(
                        'Không có mã khuyến mãi phù hợp',
                        style: AppDesignSystem.label(),
                      ),
                      if (_promoListMessage.isNotEmpty)
                        Text(
                          _promoListMessage,
                          style: AppDesignSystem.body(size: 12),
                          textAlign: TextAlign.center,
                        )
                      else
                        Text(
                          'Bạn vẫn có thể tiếp tục đặt hàng mà không áp dụng KM.',
                          style: AppDesignSystem.body(size: 12),
                          textAlign: TextAlign.center,
                        ),
                    ],
                  ),
                )
              else
                ..._eligiblePromotions.map(_promoTile),
              const SizedBox(height: 16),
              Text('Hoặc nhập mã khuyến mãi',
                  style: AppDesignSystem.body(size: 11)),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _manualPromoCtrl,
                      textCapitalization: TextCapitalization.characters,
                      decoration: const InputDecoration(
                        hintText: 'VD: DONVI10',
                        isDense: true,
                      ),
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
              const SizedBox(height: 12),
              RadioListTile<int?>(
                value: null,
                groupValue: _selectedPromotionId,
                onChanged: (_) => _clearPromotion(),
                title: const Text('Không sử dụng khuyến mãi'),
                contentPadding: EdgeInsets.zero,
                dense: true,
              ),
              if (_formError != null) ...[
                const SizedBox(height: 8),
                OrgInfoBanner(
                  message: _formError!,
                  icon: Icons.error_outline,
                  color: AppDesignSystem.danger,
                ),
              ],
            ],
          ),
        ),
        const SizedBox(height: 12),
        OrgCard(
          padding: const EdgeInsets.all(16),
          child: Column(
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Tổng thanh toán', style: AppDesignSystem.label()),
                  Text(
                    formatOrgVnd(_promoTotalAfter()),
                    style: AppDesignSystem.sectionTitle(
                      color: const Color(0xFF7C3AED),
                    ),
                  ),
                ],
              ),
              if (_promoSubtotal() > 0) ...[
                const SizedBox(height: 6),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('Tạm tính', style: AppDesignSystem.body(size: 12)),
                    Text(formatOrgVnd(_promoSubtotal()),
                        style: AppDesignSystem.body(size: 12)),
                  ],
                ),
              ],
              if (_promoDiscount() > 0) ...[
                const SizedBox(height: 4),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('Giảm giá', style: AppDesignSystem.body(size: 12)),
                    Text(
                      '−${formatOrgVnd(_promoDiscount())}',
                      style: AppDesignSystem.body(size: 12).copyWith(
                        color: AppDesignSystem.success,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
              ],
              if (_selectedPromotionName.isNotEmpty) ...[
                const SizedBox(height: 4),
                Text(
                  'Mã: $_selectedPromotionName',
                  style: AppDesignSystem.body(size: 11).copyWith(
                    color: const Color(0xFF7C3AED),
                  ),
                ),
              ],
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => setState(() => _step = 2),
                      child: const Text('Quay lại thực đơn'),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: FilledButton(
                      onPressed: _goStep4,
                      style: FilledButton.styleFrom(backgroundColor: orgAccent),
                      child: const Text('Tiếp — giao hàng'),
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

  Widget _buildStep4() {
    return OrgCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const OrgSectionHeader(
            title: 'Bước 4 — Giao hàng & người nhận',
            subtitle: 'Thông tin nhận suất tại đơn vị (bắt buộc khi tạo hợp đồng)',
          ),
          if (_formError != null && _step == 4) ...[
            const SizedBox(height: 10),
            OrgInfoBanner(
              message: _formError!,
              icon: Icons.error_outline,
              color: AppDesignSystem.danger,
            ),
          ],
          const SizedBox(height: 12),
          TextField(
            controller: _recipientNameCtrl,
            textCapitalization: TextCapitalization.words,
            decoration: const InputDecoration(
              labelText: 'Tên người nhận *',
              prefixIcon: Icon(Icons.person_outline_rounded),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _recipientPhoneCtrl,
            keyboardType: TextInputType.phone,
            decoration: const InputDecoration(
              labelText: 'SĐT người nhận *',
              hintText: '0901234567',
              prefixIcon: Icon(Icons.phone_outlined),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _recipientEmailCtrl,
            keyboardType: TextInputType.emailAddress,
            decoration: const InputDecoration(
              labelText: 'Email người nhận *',
              prefixIcon: Icon(Icons.email_outlined),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _deliveryAddressCtrl,
            maxLines: 2,
            decoration: const InputDecoration(
              labelText: 'Địa chỉ giao *',
              hintText: 'Số nhà, đường, quận/huyện…',
              prefixIcon: Icon(Icons.location_on_outlined),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _deliveryWardCtrl,
            decoration: const InputDecoration(
              labelText: 'Phường / quận (tuỳ chọn)',
              prefixIcon: Icon(Icons.map_outlined),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _deliveryTimeCtrl,
            decoration: const InputDecoration(
              labelText: 'Giờ giao mong muốn',
              hintText: '11:30',
              prefixIcon: Icon(Icons.schedule_outlined),
            ),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _deliveryNotesCtrl,
            maxLines: 2,
            decoration: const InputDecoration(
              labelText: 'Ghi chú giao hàng (tuỳ chọn)',
              prefixIcon: Icon(Icons.notes_outlined),
            ),
          ),
          const SizedBox(height: 20),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: () => setState(() {
                    _step = 3;
                    _formError = null;
                  }),
                  child: const Text('Quay lại KM'),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: FilledButton(
                  onPressed: _submitting ? null : _submitDraft,
                  style: FilledButton.styleFrom(backgroundColor: orgAccent),
                  child: Text(
                        _submitting
                            ? 'Đang tạo PDF hợp đồng…'
                            : 'Xem nháp hợp đồng',
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _promoTile(EligiblePromotionModel p) {
    final selected = _selectedPromotionId == p.promotionId;
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Material(
        color: selected
            ? const Color(0xFF7C3AED).withValues(alpha: 0.08)
            : Colors.white,
        borderRadius: BorderRadius.circular(12),
        child: InkWell(
          borderRadius: BorderRadius.circular(12),
          onTap: () => _selectPromotion(p),
          child: Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              border: Border.all(
                color: selected
                    ? const Color(0xFF7C3AED)
                    : AppDesignSystem.gray200,
                width: selected ? 2 : 1,
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (p.isRecommended)
                  Container(
                    margin: const EdgeInsets.only(bottom: 6),
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: Colors.orange,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Text(
                      'Đề xuất',
                      style: AppDesignSystem.body(size: 9, color: Colors.white),
                    ),
                  ),
                Text(p.promotionName, style: AppDesignSystem.label()),
                if (p.promotionCode.isNotEmpty)
                  Text(
                    p.promotionCode.toUpperCase(),
                    style: AppDesignSystem.body(size: 11).copyWith(
                      color: const Color(0xFF7C3AED),
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                if (p.description != null && p.description!.isNotEmpty)
                  Text(p.description!, style: AppDesignSystem.body(size: 11)),
                const SizedBox(height: 6),
                Text(
                  '−${formatOrgVnd(p.discountAmount)}',
                  style: AppDesignSystem.sectionTitle(
                    color: AppDesignSystem.success,
                  ),
                ),
                Text(
                  'Còn ${formatOrgVnd(p.totalAfter)}',
                  style: AppDesignSystem.body(size: 11),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _slotSection(
    MealDayDraftModel day,
    String slot,
    String label,
    IconData icon,
    Color accent,
  ) {
    final lines = day.linesFor(slot);
    final mainTotal = day.slotTotal('main');
    final slotTotal = day.slotTotal(slot);
    final canAdd = slot == 'main' || mainTotal == 0 || slotTotal < mainTotal;

    return OrgMealSlotCard(
      slot: slot,
      label: label,
      icon: icon,
      accentColor: accent,
      lines: lines,
      mainTotal: mainTotal,
      canAdd: canAdd,
      onAdd: () => OrgDishPickerSheet.show(
        context,
        categories: _categories,
        slotKey: slot,
        onPick: (dish) {
          final existing = lines.where((l) => l.dishId == dish.id).toList();
          if (existing.isNotEmpty) {
            if (slot != 'main' && day.slotTotal(slot) >= mainTotal) {
              _showError('Đã đạt giới hạn suất $label.');
              return;
            }
            setState(() => existing.first.quantity++);
          } else {
            if (slot != 'main' && mainTotal > 0 && day.slotTotal(slot) >= mainTotal) {
              _showError('Đã đạt giới hạn suất $label.');
              return;
            }
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
      ),
      maxQtyForLine: (line) {
        if (slot == 'main') return OrgMealSlotCard.mainQtyMax;
        final lineQty = line.quantity < 1 ? 1 : line.quantity;
        final others = day.slotTotal(slot) - lineQty;
        return (mainTotal - others).clamp(1, mainTotal);
      },
      onQtySet: (line, qty) {
        final max = slot == 'main'
            ? OrgMealSlotCard.mainQtyMax
            : () {
                final lineQty = line.quantity < 1 ? 1 : line.quantity;
                final others = day.slotTotal(slot) - lineQty;
                return (mainTotal - others).clamp(1, mainTotal);
              }();
        final next = qty.clamp(1, max);
        if (qty > max && slot != 'main') {
          _showError('Đã đạt giới hạn suất $label (tối đa $max).');
        }
        setState(() => line.quantity = next);
      },
      onRemoveLine: (line) => setState(() => lines.remove(line)),
    );
  }
}
