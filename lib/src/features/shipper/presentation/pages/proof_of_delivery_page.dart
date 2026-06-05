import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../../organization/presentation/widgets/org_signature_pad.dart';
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
  final _signatureKey = GlobalKey<OrgSignaturePadState>();
  final _picker = ImagePicker();
  Uint8List? _imageBytes;
  String? _filename;
  String? _contentType;
  bool _hasSignature = false;
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
      final d = await ShipperRepository.instance.getDelivery(widget.deliveryId);
      if (!mounted) return;
      setState(() {
        _delivery = d;
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

    final signatureBytes = await _signatureKey.currentState?.exportPngBytes();
    if (!mounted) return;
    if (signatureBytes == null || signatureBytes.isEmpty) {
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

    setState(() => _submitting = true);
    try {
      await ShipperRepository.instance.uploadDeliveryProof(
        widget.deliveryId,
        imageBytes: _imageBytes!,
        filename: _filename!,
        contentType: _contentType!,
        signatureBytes: signatureBytes,
        recipientConfirmedName: name,
        notes: _notesCtrl.text.trim().isEmpty ? null : _notesCtrl.text.trim(),
      );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Đã xác nhận giao hàng thành công'),
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
              : 'Chụp ảnh tại điểm giao, thu chữ ký và tên người nhận để hoàn tất đơn.',
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
                'Yêu cầu người nhận ký trực tiếp trên màn hình trước khi hoàn tất giao hàng.',
            icon: Icons.draw_outlined,
            color: shipperAccent,
          ),
        ],
        const SizedBox(height: 14),
        ShipperContentCard(
          title: 'Người nhận xác nhận',
          subtitle: 'Họ tên + chữ ký (bắt buộc)',
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
                'Chữ ký người nhận',
                style: AppDesignSystem.label(color: AppDesignSystem.gray500),
              ),
              const SizedBox(height: 8),
              IgnorePointer(
                ignoring: !_canSubmit,
                child: Opacity(
                  opacity: _canSubmit ? 1 : 0.5,
                  child: OrgSignaturePad(
                    key: _signatureKey,
                    onSignatureChanged: (has) {
                      if (_hasSignature != has) {
                        setState(() => _hasSignature = has);
                      }
                    },
                  ),
                ),
              ),
            ],
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
          label: _submitting ? 'Đang gửi…' : 'Hoàn tất giao hàng',
          icon: Icons.check_rounded,
          loading: _submitting,
          onPressed: _canSubmit ? _submit : null,
        ),
      ],
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
