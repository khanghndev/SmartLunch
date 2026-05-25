import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../core/theme/app_design_system.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class ProofOfDeliveryPage extends StatefulWidget {
  final int deliveryId;

  const ProofOfDeliveryPage({super.key, required this.deliveryId});

  @override
  State<ProofOfDeliveryPage> createState() => _ProofOfDeliveryPageState();
}

class _ProofOfDeliveryPageState extends State<ProofOfDeliveryPage> {
  final _notesCtrl = TextEditingController();
  final _picker = ImagePicker();
  Uint8List? _imageBytes;
  String? _filename;
  String? _contentType;
  bool _submitting = false;

  @override
  void dispose() {
    _notesCtrl.dispose();
    super.dispose();
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

  Future<void> _submit() async {
    if (widget.deliveryId <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Mã đơn giao không hợp lệ')),
      );
      return;
    }
    if (_imageBytes == null || _filename == null || _contentType == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chụp hoặc chọn ảnh xác nhận')),
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
        SnackBar(content: Text(shipperApiError(e)), behavior: SnackBarBehavior.floating),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Xác nhận giao hàng',
      body: ModuleListView(
        padding: shipperListPadding(context),
        children: [
          const ShipperPageIntro(
            title: 'Minh chứng giao hàng (PoD)',
            description:
                'Chụp ảnh tại điểm giao để hoàn tất đơn. Hệ thống ghi nhận thời gian và cập nhật trạng thái completed.',
            icon: Icons.camera_alt_rounded,
          ),
          const SizedBox(height: 14),
          ShipperPhotoCaptureCard(
            imageBytes: _imageBytes,
            enabled: !_submitting,
            onCamera: () => _pickImage(ImageSource.camera),
            onGallery: () => _pickImage(ImageSource.gallery),
          ),
          const SizedBox(height: 12),
          ShipperContentCard(
            title: 'Ghi chú thêm',
            subtitle: 'Tuỳ chọn — lý do trễ, người nhận, v.v.',
            accent: kShipperRole.primaryAlt,
            child: TextField(
              controller: _notesCtrl,
              maxLines: 3,
              enabled: !_submitting,
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
            onPressed: _submitting ? null : _submit,
          ),
        ],
      ),
    );
  }
}
