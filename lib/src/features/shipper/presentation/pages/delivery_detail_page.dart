import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliveryDetailPage extends StatefulWidget {
  final int deliveryId;

  const DeliveryDetailPage({super.key, required this.deliveryId});

  @override
  State<DeliveryDetailPage> createState() => _DeliveryDetailPageState();
}

class _DeliveryDetailPageState extends State<DeliveryDetailPage> {
  ShipperDeliveryDetailModel? _delivery;
  bool _loading = true;
  String? _error;
  bool _updating = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    if (widget.deliveryId <= 0) {
      setState(() {
        _error = 'Không xác định được mã đơn giao.';
        _loading = false;
      });
      return;
    }
    setState(() {
      _loading = true;
      _error = null;
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
        _error = shipperApiError(e);
        _loading = false;
      });
    }
  }

  Future<void> _updateStatus(String status, {String? notes}) async {
    setState(() => _updating = true);
    try {
      final updated = await ShipperRepository.instance.updateDeliveryStatus(
        widget.deliveryId,
        status: status,
        notes: notes,
      );
      if (!mounted) return;
      setState(() {
        _delivery = updated;
        _updating = false;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Đã cập nhật: ${shipperDeliveryStatusLabelVi(status)}')),
      );
    } catch (e) {
      if (!mounted) return;
      setState(() => _updating = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(shipperApiError(e))),
      );
    }
  }

  Future<void> _promptNotesThenUpdate(String status, String title) async {
    final ctrl = TextEditingController();
    final notes = await showDialog<String>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(title),
        content: TextField(
          controller: ctrl,
          maxLines: 3,
          decoration: const InputDecoration(
            hintText: 'Ghi chú lý do (bắt buộc)',
            border: OutlineInputBorder(),
          ),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx), child: const Text('Hủy')),
          FilledButton(
            onPressed: () {
              if (ctrl.text.trim().isEmpty) return;
              Navigator.pop(ctx, ctrl.text.trim());
            },
            child: const Text('Xác nhận'),
          ),
        ],
      ),
    );
    ctrl.dispose();
    if (notes != null && notes.isNotEmpty) {
      await _updateStatus(status, notes: notes);
    }
  }

  Future<void> _confirmAction(String status, String message) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Xác nhận'),
        content: Text(message),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(backgroundColor: kShipperRole.primary),
            child: const Text('Đồng ý'),
          ),
        ],
      ),
    );
    if (ok == true) await _updateStatus(status);
  }

  void _openProof() {
    Navigator.of(context)
        .pushNamed(AppRoutes.shipperProof, arguments: widget.deliveryId)
        .then((_) => _load());
  }

  void _copyAddress(String address) {
    Clipboard.setData(ClipboardData(text: address));
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Đã sao chép địa chỉ')),
    );
  }

  List<Widget> _actionButtons(ShipperDeliveryDetailModel d) {
    if (_updating) {
      return [
        const Center(child: Padding(
          padding: EdgeInsets.all(16),
          child: CircularProgressIndicator(),
        )),
      ];
    }
    final s = d.deliveryStatus.toLowerCase();
    final buttons = <Widget>[];

    if (s == 'pending' || s == 'assigned') {
      buttons.addAll([
        _primaryBtn('Nhận đơn', Icons.check_circle_outline, () =>
            _confirmAction('received', 'Xác nhận nhận đơn giao này?')),
        _outlineBtn('Từ chối đơn', Icons.cancel_outlined, () =>
            _promptNotesThenUpdate('rejected', 'Lý do từ chối')),
      ]);
    } else if (s == 'received') {
      buttons.addAll([
        _primaryBtn('Bắt đầu giao', Icons.local_shipping_outlined, () =>
            _confirmAction('in_transit', 'Bắt đầu giao đơn này?')),
        _outlineBtn('Giao thất bại', Icons.error_outline, () =>
            _promptNotesThenUpdate('failed', 'Lý do giao thất bại')),
      ]);
    } else if (s == 'in_transit') {
      buttons.addAll([
        _primaryBtn('Xác nhận giao (chụp ảnh)', Icons.camera_alt_outlined, _openProof),
        _outlineBtn('Giao thất bại', Icons.error_outline, () =>
            _promptNotesThenUpdate('failed', 'Lý do giao thất bại')),
      ]);
    } else if (s == 'completed') {
      if (d.proofImageUrl != null && d.proofImageUrl!.isNotEmpty) {
        buttons.add(
          ShipperCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Ảnh xác nhận giao', style: AppDesignSystem.label()),
                const SizedBox(height: 8),
                Text(
                  d.proofImageUrl!,
                  style: AppDesignSystem.body(size: 11),
                  maxLines: 3,
                  overflow: TextOverflow.ellipsis,
                ),
                if (d.proofCapturedAtUtc != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    'Chụp lúc: ${formatShipperDateTime(d.proofCapturedAtUtc!)}',
                    style: AppDesignSystem.body(size: 12),
                  ),
                ],
              ],
            ),
          ),
        );
      }
    }

    return buttons;
  }

  Widget _primaryBtn(String label, IconData icon, VoidCallback onTap) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: SizedBox(
        width: double.infinity,
        child: FilledButton.icon(
          onPressed: onTap,
          icon: Icon(icon),
          label: Text(label),
          style: FilledButton.styleFrom(
            backgroundColor: kShipperRole.primary,
            padding: const EdgeInsets.symmetric(vertical: 14),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
          ),
        ),
      ),
    );
  }

  Widget _outlineBtn(String label, IconData icon, VoidCallback onTap) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: SizedBox(
        width: double.infinity,
        child: OutlinedButton.icon(
          onPressed: onTap,
          icon: Icon(icon, color: AppDesignSystem.danger),
          label: Text(label, style: const TextStyle(color: AppDesignSystem.danger)),
          style: OutlinedButton.styleFrom(
            padding: const EdgeInsets.symmetric(vertical: 14),
            side: const BorderSide(color: AppDesignSystem.danger),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
            ),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Chi tiết đơn giao',
      onRefresh: _load,
      body: _loading
          ? const ShipperLoadingBody()
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : _delivery == null
                  ? const ShipperLoadingBody()
                  : ListView(
                      padding: const EdgeInsets.all(16),
                      children: [
                        ShipperCard(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  Expanded(
                                    child: Text(
                                      'Đơn #${_delivery!.orderId}',
                                      style: AppDesignSystem.title(size: 20),
                                    ),
                                  ),
                                  ShipperStatusChip(status: _delivery!.deliveryStatus),
                                ],
                              ),
                              const SizedBox(height: 16),
                              _infoRow(Icons.location_on_outlined, 'Địa điểm giao',
                                  _delivery!.deliveryAddress,
                                  onCopy: () => _copyAddress(_delivery!.deliveryAddress)),
                              _infoRow(Icons.restaurant_outlined, 'Số suất ăn',
                                  '${_delivery!.mealCount} suất'),
                              _infoRow(Icons.schedule_outlined, 'Thời gian giao dự kiến',
                                  formatShipperDateTime(_delivery!.scheduledDateUtc)),
                              if (_delivery!.deliveredAtUtc != null)
                                _infoRow(Icons.check_circle_outline, 'Giao thực tế',
                                    formatShipperDateTime(_delivery!.deliveredAtUtc!)),
                              if (_delivery!.notes != null &&
                                  _delivery!.notes!.isNotEmpty)
                                _infoRow(Icons.notes_outlined, 'Ghi chú',
                                    _delivery!.notes!),
                            ],
                          ),
                        ),
                        const SizedBox(height: 16),
                        Text('Thao tác', style: AppDesignSystem.sectionTitle()),
                        const SizedBox(height: 8),
                        ..._actionButtons(_delivery!),
                      ],
                    ),
    );
  }

  Widget _infoRow(IconData icon, String label, String value, {VoidCallback? onCopy}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 20, color: kShipperRole.primary),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: AppDesignSystem.body(size: 12)),
                const SizedBox(height: 2),
                Text(value, style: AppDesignSystem.label()),
              ],
            ),
          ),
          if (onCopy != null)
            IconButton(
              icon: const Icon(Icons.copy_outlined, size: 20),
              onPressed: onCopy,
              tooltip: 'Sao chép',
            ),
        ],
      ),
    );
  }
}
