import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../profile/data/profile_repository.dart';
import '../../data/models/bulk_order_models.dart';
import '../../data/org_repository.dart';
import '../widgets/organization_ui.dart';

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

  DateTime _start = DateTime.now();
  DateTime _end = DateTime.now().add(const Duration(days: 29));
  final List<DateTime> _excludedDates = [];

  final _mealsPerDayCtrl = TextEditingController(text: '50');
  final _unitPriceCtrl = TextEditingController(text: '35000');

  final _recipientNameCtrl = TextEditingController();
  final _recipientPhoneCtrl = TextEditingController();
  final _recipientEmailCtrl = TextEditingController();
  final _deliveryAddressCtrl = TextEditingController();
  final _deliveryWardCtrl = TextEditingController();
  final _deliveryNotesCtrl = TextEditingController();
  final _deliveryTimeCtrl = TextEditingController(text: '11:30');

  bool _submitting = false;

  @override
  void initState() {
    super.initState();
    _bootstrap();
  }

  @override
  void dispose() {
    _mealsPerDayCtrl.dispose();
    _unitPriceCtrl.dispose();
    _recipientNameCtrl.dispose();
    _recipientPhoneCtrl.dispose();
    _recipientEmailCtrl.dispose();
    _deliveryAddressCtrl.dispose();
    _deliveryWardCtrl.dispose();
    _deliveryNotesCtrl.dispose();
    _deliveryTimeCtrl.dispose();
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

      _recipientNameCtrl.text = (unit.legalRepresentative?.trim().isNotEmpty ==
              true
          ? unit.legalRepresentative!
          : profile.displayName);
      _recipientPhoneCtrl.text = (unit.phone ?? profile.phoneNumber ?? '').trim();
      _recipientEmailCtrl.text = (unit.contactEmail ?? profile.email).trim();
      _deliveryAddressCtrl.text = (unit.address ?? '').trim();

      setState(() {
        _orgId = unit.id;
        _orgName = unit.name;
        _loading = false;
      });
    } catch (e) {
      setState(() {
        _error = orgApiError(e);
        _loading = false;
      });
    }
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

  Future<void> _pickPeriod() async {
    final picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime.now().subtract(const Duration(days: 1)),
      lastDate: DateTime.now().add(const Duration(days: 365)),
      initialDateRange: DateTimeRange(start: _start, end: _end),
      helpText: 'Chọn thời hạn hợp đồng',
      locale: const Locale('vi', 'VN'),
    );
    if (picked == null) return;
    setState(() {
      _start = DateTime(picked.start.year, picked.start.month, picked.start.day);
      _end = DateTime(picked.end.year, picked.end.month, picked.end.day);
      _excludedDates.removeWhere((d) => d.isBefore(_start) || d.isAfter(_end));
    });
  }

  Future<void> _addExcludedDate() async {
    final picked = await showDatePicker(
      context: context,
      firstDate: _start,
      lastDate: _end,
      initialDate: _start,
      helpText: 'Chọn ngày không cung cấp suất',
      locale: const Locale('vi', 'VN'),
    );
    if (picked == null) return;
    final normalized = DateTime(picked.year, picked.month, picked.day);
    if (_excludedDates.any((d) => _isSameDay(d, normalized))) return;
    setState(() {
      _excludedDates.add(normalized);
      _excludedDates.sort((a, b) => a.compareTo(b));
    });
  }

  void _removeExcluded(DateTime d) {
    setState(() {
      _excludedDates.removeWhere((x) => _isSameDay(x, d));
    });
  }

  Future<void> _submit() async {
    final deliveryError = OrganizationMealDeliveryModel.validate(
      recipientName: _recipientNameCtrl.text,
      recipientPhone: _recipientPhoneCtrl.text,
      recipientEmail: _recipientEmailCtrl.text,
      deliveryAddress: _deliveryAddressCtrl.text,
      preferredDeliveryTime: _deliveryTimeCtrl.text,
    );
    if (deliveryError != null) {
      setState(() => _error = deliveryError);
      return;
    }

    final mealsPerDay = int.tryParse(_mealsPerDayCtrl.text) ?? 0;
    final price = double.tryParse(_unitPriceCtrl.text) ?? 0;
    if (mealsPerDay < 1) {
      setState(() => _error = 'Số suất/ngày phải >= 1.');
      return;
    }
    if (price <= 0) {
      setState(() => _error = 'Giá/suất phải > 0.');
      return;
    }

    setState(() {
      _submitting = true;
      _error = null;
    });

    try {
      final draft = await OrgRepository.instance.prepareMealPeriodContract(
        organizationId: _orgId,
        startDate: _toIsoDate(_start),
        endDate: _toIsoDate(_end),
        excludedDates: _excludedDates.map(_toIsoDate).toList(),
        mealsPerDay: mealsPerDay,
        mealUnitPrice: price,
        delivery: _delivery(),
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
      setState(() => _error = orgApiError(e));
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Đặt suất theo hợp đồng',
      body: ModuleListView(
        padding: orgListPadding(context),
        children: [
          OrgPageIntro(
            title: 'Thiết lập hợp đồng theo kỳ',
            description: _orgName.isNotEmpty
                ? 'Đơn vị: $_orgName · Chỉ chọn món chính theo tuần'
                : 'Chỉ chọn món chính theo tuần',
            icon: Icons.description_outlined,
          ),
          if (_loading) ...[
            const SizedBox(height: 12),
            const Center(child: CircularProgressIndicator()),
            const SizedBox(height: 12),
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
                    title: 'Thời hạn & ngày nghỉ',
                    subtitle: 'Chọn khoảng thời gian và các ngày không cung cấp suất',
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(
                        child: _InfoTile(
                          label: 'Bắt đầu',
                          value: _toIsoDate(_start),
                        ),
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: _InfoTile(
                          label: 'Kết thúc',
                          value: _toIsoDate(_end),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  OutlinedButton.icon(
                    onPressed: _pickPeriod,
                    icon: const Icon(Icons.date_range_outlined),
                    label: const Text('Chọn thời hạn'),
                    style: OutlinedButton.styleFrom(foregroundColor: orgAccent),
                  ),
                  const SizedBox(height: 10),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: _excludedDates
                        .map(
                          (d) => InputChip(
                            label: Text(_toIsoDate(d)),
                            onDeleted: () => _removeExcluded(d),
                          ),
                        )
                        .toList(),
                  ),
                  const SizedBox(height: 8),
                  TextButton.icon(
                    onPressed: _addExcludedDate,
                    icon: const Icon(Icons.add_circle_outline),
                    label: const Text('Thêm ngày không cung cấp'),
                    style: TextButton.styleFrom(foregroundColor: orgAccent),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            OrgCard(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const OrgSectionHeader(
                    title: 'Số suất & đơn giá',
                    subtitle: 'Áp dụng cho tất cả ngày phục vụ trong kỳ',
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(
                        child: TextField(
                          controller: _mealsPerDayCtrl,
                          keyboardType: TextInputType.number,
                          inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                          decoration: AppDesignSystem.inputDecoration(
                            label: 'Số suất/ngày',
                            hint: 'VD: 50',
                            focusColor: orgAccent,
                          ),
                        ),
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: TextField(
                          controller: _unitPriceCtrl,
                          keyboardType: TextInputType.number,
                          inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                          decoration: AppDesignSystem.inputDecoration(
                            label: 'Giá một suất (VND)',
                            hint: 'VD: 35000',
                            focusColor: orgAccent,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            OrgCard(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const OrgSectionHeader(
                    title: 'Giao hàng & người nhận',
                    subtitle: 'Thông tin mặc định lấy từ hồ sơ đơn vị',
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    controller: _recipientNameCtrl,
                    decoration: AppDesignSystem.inputDecoration(
                      label: 'Người nhận',
                      focusColor: orgAccent,
                    ),
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(
                        child: TextField(
                          controller: _recipientPhoneCtrl,
                          keyboardType: TextInputType.phone,
                          decoration: AppDesignSystem.inputDecoration(
                            label: 'Số điện thoại',
                            focusColor: orgAccent,
                          ),
                        ),
                      ),
                      const SizedBox(width: 10),
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
                    controller: _recipientEmailCtrl,
                    keyboardType: TextInputType.emailAddress,
                    decoration: AppDesignSystem.inputDecoration(
                      label: 'Email',
                      focusColor: orgAccent,
                    ),
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    controller: _deliveryAddressCtrl,
                    decoration: AppDesignSystem.inputDecoration(
                      label: 'Địa chỉ giao',
                      focusColor: orgAccent,
                    ),
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    controller: _deliveryWardCtrl,
                    decoration: AppDesignSystem.inputDecoration(
                      label: 'Phường/Quận (tuỳ chọn)',
                      focusColor: orgAccent,
                    ),
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    controller: _deliveryNotesCtrl,
                    decoration: AppDesignSystem.inputDecoration(
                      label: 'Ghi chú (tuỳ chọn)',
                      focusColor: orgAccent,
                    ),
                    minLines: 2,
                    maxLines: 4,
                  ),
                ],
              ),
            ),
            const SizedBox(height: 14),
            FilledButton.icon(
              onPressed: _submitting ? null : _submit,
              icon: _submitting
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.check_circle_outline),
              label: Text(_submitting ? 'Đang tạo nháp…' : 'Tạo nháp hợp đồng'),
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

  static bool _isSameDay(DateTime a, DateTime b) =>
      a.year == b.year && a.month == b.month && a.day == b.day;

  static String _toIsoDate(DateTime d) =>
      '${d.year.toString().padLeft(4, '0')}-'
      '${d.month.toString().padLeft(2, '0')}-'
      '${d.day.toString().padLeft(2, '0')}';
}

class _InfoTile extends StatelessWidget {
  const _InfoTile({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppDesignSystem.gray50,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppDesignSystem.gray200),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: AppDesignSystem.body(
              size: 11,
              color: AppDesignSystem.gray700,
            ).copyWith(fontWeight: FontWeight.w800),
          ),
          const SizedBox(height: 2),
          Text(value, style: AppDesignSystem.body().copyWith(fontWeight: FontWeight.w700)),
        ],
      ),
    );
  }
}

