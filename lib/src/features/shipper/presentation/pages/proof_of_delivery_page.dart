import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../../organization/presentation/widgets/org_signature_pad.dart';
import '../../../profile/data/models/user_profile_model.dart';
import '../../../profile/data/profile_repository.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class ProofOfDeliveryPage extends StatefulWidget {
  final int deliveryId;

  const ProofOfDeliveryPage({super.key, required this.deliveryId});

  @override
  State<ProofOfDeliveryPage> createState() => _ProofOfDeliveryPageState();
}

class _ProofOfDeliveryPageState extends State<ProofOfDeliveryPage> {
  final _recipientNameCtrl = TextEditingController();
  final _notesCtrl = TextEditingController();
  final _recipientSignatureKey = GlobalKey<OrgSignaturePadState>();
  final _shipperSignatureKey = GlobalKey<OrgSignaturePadState>();
  final _picker = ImagePicker();
  Uint8List? _imageBytes;
  String? _filename;
  String? _contentType;
  bool _hasRecipientSignature = false;
  bool _hasShipperSignature = false;
  String _shipperDisplayName = '';
  ShipperDeliveryDetailModel? _delivery;
  bool _loading = true;
  String? _loadError;
  bool _submitting = false;

  @override
  void initState() {
    super.initState();
    _loadDelivery();
  }

  @override
  void dispose() {
    _recipientNameCtrl.dispose();
    _notesCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadDelivery() async {
    if (widget.deliveryId <= 0) {
      setState(() {
        _loadError = 'Mã đơn giao không hợp lệ';
        _loading = false;
      });
      return;
    }
    setState(() {
      _loading = true;
      _loadError = null;
    });
    try {
      final results = await Future.wait([
        ShipperRepository.instance.getDelivery(widget.deliveryId),
        ProfileRepository.instance.getProfile(),
      ]);
      final d = results[0] as ShipperDeliveryDetailModel;
      final profile = results[1] as UserProfileModel;
      if (!mounted) return;
      setState(() {
        _delivery = d;
        _shipperDisplayName = profile.displayName.trim();
        _loading = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _loadError = shipperApiError(e);
        _loading = false;
      });
    }
  }

  Future<void> _pickImage(ImageSource source) async {
    try {
      final file = await _picker.pickImage(
        source: source,
        maxWidth: 1920,
        imageQuality: 85,
      );
      if (file == null) return;
      final bytes = await file.readAsBytes();
      final name = file.name;
      final mime = name.toLowerCase().endsWith('.png')
          ? 'image/png'
          : name.toLowerCase().endsWith('.webp')
              ? 'image/webp'
              : 'image/jpeg';
      setState(() {
        _imageBytes = bytes;
        _filename = name.isNotEmpty ? name : 'proof.jpg';
        _contentType = mime;
      });
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Không chọn được ảnh: ${shipperApiError(e)}'),
          behavior: SnackBarBehavior.floating,
        ),
      );
    }
  }

  bool get _canSubmit {
    final d = _delivery;
    if (d == null) return false;
    return d.deliveryStatus.toLowerCase() == 'in_transit' &&
        !_submitting &&
        !_loading;
  }

  Future<void> _submit() async {
    if (!_canSubmit) return;
    FocusScope.of(context).unfocus();

    final name = _recipientNameCtrl.text.trim();
    if (name.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng nhập tên người nhận xác nhận'),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    final shipperState = _shipperSignatureKey.currentState;
    if (shipperState?.hasSignature != true) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng ký xác nhận của shipper (bạn)'),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    final recipientState = _recipientSignatureKey.currentState;
    if (recipientState?.hasSignature != true) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng ký xác nhận của người nhận'),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    if (_imageBytes == null || _filename == null || _contentType == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Vui lòng chụp hoặc chọn ảnh xác nhận'),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    // Xuất shipper trước — dùng cache nội bộ, không phụ thuộc RepaintBoundary khi cuộn.
    final shipperSignatureBytes = await shipperState!.exportPngBytes();
    if (!mounted) return;
    if (shipperSignatureBytes == null || shipperSignatureBytes.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không đọc được chữ ký shipper. Vui lòng ký lại.'),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    final recipientSignatureBytes = await recipientState!.exportPngBytes();
    if (!mounted) return;
    if (recipientSignatureBytes == null || recipientSignatureBytes.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không đọc được chữ ký người nhận. Vui lòng ký lại.'),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    setState(() => _submitting = true);
    try {
      final updated = await ShipperRepository.instance.uploadDeliveryProof(
        widget.deliveryId,
        imageBytes: _imageBytes!,
        filename: _filename!,
        contentType: _contentType!,
        signatureBytes: recipientSignatureBytes,
        shipperSignatureBytes: shipperSignatureBytes,
        recipientConfirmedName: name,
        notes: _notesCtrl.text.trim().isEmpty ? null : _notesCtrl.text.trim(),
      );
      if (!mounted) return;

      final handoverUrl = updated.handoverDocumentUrl?.trim();
      if (handoverUrl != null && handoverUrl.isNotEmpty) {
        await Navigator.of(context).pushNamed(
          AppRoutes.shipperHandoverPdf,
          arguments: {
            'pdfUrl': handoverUrl,
            'title': 'Biên bản bàn giao',
            'subtitle':
                'PDF đã lưu trên hệ thống. Người nhận: $name · Đơn #${updated.orderId}',
          },
        );
      }

      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            handoverUrl != null && handoverUrl.isNotEmpty
                ? 'Đã hoàn tất giao hàng và lưu biên bản bàn giao'
                : 'Đã xác nhận giao hàng thành công',
          ),
          behavior: SnackBarBehavior.floating,
        ),
      );
      Navigator.of(context).pop(true);
    } catch (e) {
      if (!mounted) return;
      setState(() => _submitting = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(shipperApiError(e)),
          behavior: SnackBarBehavior.floating,
        ),
      );
    }
  }

  Widget _buildBody() {
    if (_loading) {
      return const ShipperLoadingBody(message: 'Đang tải thông tin đơn…');
    }
    if (_loadError != null) {
      return ShipperErrorBody(message: _loadError!, onRetry: _loadDelivery);
    }

    final d = _delivery;
    if (d == null) {
      return const ShipperLoadingBody();
    }

    final status = d.deliveryStatus.toLowerCase();
    final notInTransit = status != 'in_transit';

    return ModuleListView(
      padding: shipperListPadding(context),
      children: [
        ShipperPageIntro(
          title: 'Minh chứng giao hàng (PoD)',
          description: notInTransit
              ? 'Đơn phải ở trạng thái Đang giao trước khi xác nhận. Quay lại chi tiết và bấm «Bắt đầu giao».'
              : 'Shipper và người nhận cùng ký trên biên bản. Hệ thống tạo PDF và lưu sau khi xác nhận.',
          icon: Icons.camera_alt_rounded,
        ),
        if (notInTransit) ...[
          const SizedBox(height: 12),
          ShipperInfoBanner(
            message:
                'Trạng thái hiện tại: ${shipperDeliveryStatusLabelVi(d.deliveryStatus)}. '
                'Chỉ xác nhận PoD khi đơn đang giao (in_transit).',
            icon: Icons.warning_amber_rounded,
            color: AppDesignSystem.warning,
          ),
        ],
        if (d.requiresRecipientSignature && !notInTransit) ...[
          const SizedBox(height: 12),
          ShipperInfoBanner(
            message:
                'Shipper và người nhận đều phải ký trên màn hình. PDF biên bản sẽ được tạo tự động sau khi gửi.',
            icon: Icons.draw_outlined,
            color: shipperAccent,
          ),
        ],
        if (!notInTransit) ...[
          const SizedBox(height: 12),
          ShipperContentCard(
            title: 'Biên bản bàn giao',
            subtitle: 'Nội dung sẽ ghi vào PDF sau khi ký',
            headerIcon: Icons.description_outlined,
            accent: shipperAccent,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _handoverRow('Mã giao', '#${d.deliveryId}'),
                _handoverRow('Đơn hàng', '#${d.orderId}'),
                _handoverRow('Địa chỉ', d.deliveryAddress),
                _handoverRow('Số suất', '${d.mealCount} suất'),
                _handoverRow(
                  'Ngày phục vụ',
                  formatShipperDateTime(d.scheduledDateUtc),
                ),
                if (_shipperDisplayName.isNotEmpty)
                  _handoverRow('Shipper giao', _shipperDisplayName),
              ],
            ),
          ),
        ],
        const SizedBox(height: 14),
        _KeepAliveSection(
          child: ShipperContentCard(
            title: 'Shipper xác nhận',
            subtitle: 'Chữ ký của bạn trên biên bản (bắt buộc)',
            headerIcon: Icons.local_shipping_outlined,
            accent: kShipperRole.primaryAlt,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (_shipperDisplayName.isNotEmpty)
                  Text(
                    _shipperDisplayName,
                    style: AppDesignSystem.label(),
                  ),
                const SizedBox(height: 8),
                Text(
                  'Chữ ký shipper (bên giao)',
                  style: AppDesignSystem.label(color: AppDesignSystem.gray500),
                ),
                const SizedBox(height: 8),
                IgnorePointer(
                  ignoring: !_canSubmit,
                  child: Opacity(
                    opacity: _canSubmit ? 1 : 0.5,
                    child: OrgSignaturePad(
                      key: _shipperSignatureKey,
                      onSignatureChanged: (has) {
                        if (_hasShipperSignature != has) {
                          setState(() => _hasShipperSignature = has);
                        }
                      },
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 12),
        _KeepAliveSection(
          child: ShipperContentCard(
            title: 'Người nhận xác nhận',
            subtitle: 'Họ tên + chữ ký số trên biên bản (bắt buộc)',
            headerIcon: Icons.person_outline_rounded,
            accent: shipperAccent,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                TextField(
                  controller: _recipientNameCtrl,
                  enabled: _canSubmit,
                  textCapitalization: TextCapitalization.words,
                  decoration: AppDesignSystem.inputDecoration(
                    label: 'Họ tên người nhận',
                    hint: 'VD: Nguyễn Văn A',
                    focusColor: shipperAccent,
                  ),
                ),
                const SizedBox(height: 14),
                Text(
                  'Chữ ký người nhận trên biên bản',
                  style: AppDesignSystem.label(color: AppDesignSystem.gray500),
                ),
                const SizedBox(height: 8),
                IgnorePointer(
                  ignoring: !_canSubmit,
                  child: Opacity(
                    opacity: _canSubmit ? 1 : 0.5,
                    child: OrgSignaturePad(
                      key: _recipientSignatureKey,
                      onSignatureChanged: (has) {
                        if (_hasRecipientSignature != has) {
                          setState(() => _hasRecipientSignature = has);
                        }
                      },
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 12),
        ShipperPhotoCaptureCard(
          imageBytes: _imageBytes,
          enabled: _canSubmit,
          onCamera: () => _pickImage(ImageSource.camera),
          onGallery: () => _pickImage(ImageSource.gallery),
        ),
        const SizedBox(height: 12),
        ShipperContentCard(
          title: 'Ghi chú thêm',
          subtitle: 'Tuỳ chọn',
          accent: kShipperRole.primaryAlt,
          child: TextField(
            controller: _notesCtrl,
            maxLines: 3,
            enabled: _canSubmit,
            decoration: AppDesignSystem.inputDecoration(
              label: 'Nội dung ghi chú',
              focusColor: shipperAccent,
            ),
          ),
        ),
        const SizedBox(height: 20),
        ShipperPrimaryButton(
          label: _submitting ? 'Đang gửi…' : 'Ký & hoàn tất giao hàng',
          icon: Icons.check_rounded,
          loading: _submitting,
          onPressed: _canSubmit ? _submit : null,
        ),
      ],
    );
  }

  Widget _handoverRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 5),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 96,
            child: Text(
              label,
              style: AppDesignSystem.body(size: 12, color: AppDesignSystem.gray500),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: AppDesignSystem.body(size: 12).copyWith(fontWeight: FontWeight.w700),
            ),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Xác nhận giao hàng',
      onRefresh: _loadDelivery,
      body: _buildBody(),
    );
  }
}

/// Giữ state vùng ký khi cuộn ListView / đóng bàn phím.
class _KeepAliveSection extends StatefulWidget {
  const _KeepAliveSection({required this.child});

  final Widget child;

  @override
  State<_KeepAliveSection> createState() => _KeepAliveSectionState();
}

class _KeepAliveSectionState extends State<_KeepAliveSection>
    with AutomaticKeepAliveClientMixin {
  @override
  bool get wantKeepAlive => true;

  @override
  Widget build(BuildContext context) {
    super.build(context);
    return widget.child;
  }
}
