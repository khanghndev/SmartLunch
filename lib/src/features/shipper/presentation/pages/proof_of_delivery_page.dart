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
        SnackBar(content: Text('Không chọn được ảnh: ${shipperApiError(e)}')),
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
        const SnackBar(content: Text('Đã xác nhận giao hàng thành công')),
      );
      Navigator.of(context).pop(true);
    } catch (e) {
      if (!mounted) return;
      setState(() => _submitting = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(shipperApiError(e))),
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
            title: 'Proof of Delivery (PoD)',
            description:
                'Chụp ảnh minh chứng tại điểm giao. Hệ thống tự đánh dấu đơn hoàn tất và ghi nhận thời gian.',
            icon: Icons.camera_alt_rounded,
          ),
          const SizedBox(height: 16),
          ShipperCard(
            child: AspectRatio(
              aspectRatio: 4 / 3,
              child: _imageBytes == null
                  ? Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.add_a_photo_outlined,
                            size: 48, color: AppDesignSystem.gray400),
                        const SizedBox(height: 8),
                        Text('Chưa có ảnh', style: AppDesignSystem.body()),
                      ],
                    )
                  : ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Image.memory(_imageBytes!, fit: BoxFit.cover),
                    ),
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: _submitting ? null : () => _pickImage(ImageSource.camera),
                  icon: const Icon(Icons.camera_alt_outlined),
                  label: const Text('Chụp ảnh'),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: _submitting ? null : () => _pickImage(ImageSource.gallery),
                  icon: const Icon(Icons.photo_library_outlined),
                  label: const Text('Thư viện'),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          TextField(
            controller: _notesCtrl,
            maxLines: 3,
            decoration: AppDesignSystem.inputDecoration(
              label: 'Ghi chú (tuỳ chọn)',
              focusColor: shipperAccent,
            ),
          ),
          const SizedBox(height: 24),
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
