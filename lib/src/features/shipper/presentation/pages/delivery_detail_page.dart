import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../../utils/shipper_geo.dart';
import '../../utils/shipper_maps_launcher.dart';
import '../widgets/shipper_route_map_panel.dart';
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
        SnackBar(
          content: Text('Đã cập nhật: ${shipperDeliveryStatusLabelVi(status)}'),
          backgroundColor: AppDesignSystem.success,
        ),
      );
    } catch (e) {
      if (!mounted) return;
      setState(() => _updating = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(shipperApiError(e)),
          backgroundColor: AppDesignSystem.danger,
        ),
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
            style: FilledButton.styleFrom(backgroundColor: shipperAccent),
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
            style: FilledButton.styleFrom(backgroundColor: shipperAccent),
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

  Future<void> _openMaps() async {
    final d = _delivery;
    if (d == null) return;
    final ok = await ShipperMapsLauncher.openAddress(
      d.deliveryAddress,
      seed: d.deliveryId,
    );
    if (!mounted) return;
    if (!ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không mở được Google Maps trên thiết bị')),
      );
    }
  }

  List<Widget> _actionButtons(ShipperDeliveryDetailModel d) {
    if (_updating) {
      return [
        const Padding(
          padding: EdgeInsets.all(16),
          child: Center(child: CircularProgressIndicator()),
        ),
      ];
    }
    final s = d.deliveryStatus.toLowerCase();
    final buttons = <Widget>[];

    if (s == 'pending' || s == 'assigned') {
      buttons.addAll([
        ShipperPrimaryButton(
          label: 'Nhận đơn',
          icon: Icons.check_circle_outline,
          onPressed: () => _confirmAction('received', 'Xác nhận nhận đơn giao này?'),
        ),
        const SizedBox(height: 8),
        ShipperOutlineButton(
          label: 'Từ chối đơn',
          icon: Icons.cancel_outlined,
          onPressed: () => _promptNotesThenUpdate('rejected', 'Lý do từ chối'),
        ),
      ]);
    } else if (s == 'received') {
      buttons.addAll([
        ShipperPrimaryButton(
          label: 'Bắt đầu giao',
          icon: Icons.local_shipping_outlined,
          onPressed: () => _confirmAction('in_transit', 'Bắt đầu giao đơn này?'),
        ),
        const SizedBox(height: 8),
        ShipperOutlineButton(
          label: 'Giao thất bại',
          icon: Icons.error_outline,
          onPressed: () => _promptNotesThenUpdate('failed', 'Lý do giao thất bại'),
        ),
      ]);
    } else if (s == 'in_transit') {
      buttons.addAll([
        ShipperPrimaryButton(
          label: 'Xác nhận giao (chụp ảnh)',
          icon: Icons.camera_alt_outlined,
          onPressed: _openProof,
        ),
        const SizedBox(height: 8),
        ShipperOutlineButton(
          label: 'Giao thất bại',
          icon: Icons.error_outline,
          onPressed: () => _promptNotesThenUpdate('failed', 'Lý do giao thất bại'),
        ),
      ]);
    } else if (s == 'completed') {
      if (d.proofImageUrl != null && d.proofImageUrl!.isNotEmpty) {
        buttons.add(
          ShipperCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const ShipperSectionHeader(title: 'Ảnh xác nhận giao (PoD)'),
                const SizedBox(height: 10),
                ShipperProofImage(imageUrl: d.proofImageUrl, height: 220),
                if (d.proofCapturedAtUtc != null) ...[
                  const SizedBox(height: 8),
                  Text(
                    'Chụp lúc: ${formatShipperDateTime(d.proofCapturedAtUtc!)}',
                    style: AppDesignSystem.body(size: 12),
                  ),
                ],
                if (d.deliveredAtUtc != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    'Giao thực tế: ${formatShipperDateTime(d.deliveredAtUtc!)}',
                    style: AppDesignSystem.body(size: 12, color: AppDesignSystem.success)
                        .copyWith(fontWeight: FontWeight.w600),
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
                  : ModuleListView(
                      padding: shipperListPadding(context),
                      children: [
                        ShipperPageIntro(
                          title: 'Đơn #${_delivery!.orderId}',
                          description:
                              'Cập nhật trạng thái, mở bản đồ chỉ đường hoặc chụp ảnh xác nhận khi giao xong.',
                          icon: Icons.local_shipping_rounded,
                        ),
                        const SizedBox(height: 14),
                        ShipperCard(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  Expanded(
                                    child: Text(
                                      'Thông tin giao',
                                      style: AppDesignSystem.sectionTitle(),
                                    ),
                                  ),
                                  ShipperStatusBadge(status: _delivery!.deliveryStatus),
                                ],
                              ),
                              const SizedBox(height: 12),
                              ShipperDetailField(
                                icon: Icons.location_on_outlined,
                                label: 'Địa điểm giao',
                                value: _delivery!.deliveryAddress,
                                trailing: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    IconButton(
                                      icon: const Icon(Icons.map_rounded, size: 22),
                                      color: shipperAccent,
                                      onPressed: _openMaps,
                                      tooltip: 'Chỉ đường Google Maps',
                                    ),
                                    IconButton(
                                      icon: const Icon(Icons.copy_outlined, size: 20),
                                      onPressed: () =>
                                          _copyAddress(_delivery!.deliveryAddress),
                                      tooltip: 'Sao chép',
                                    ),
                                  ],
                                ),
                              ),
                              ShipperDetailField(
                                icon: Icons.restaurant_outlined,
                                label: 'Số suất ăn',
                                value: '${_delivery!.mealCount} suất',
                              ),
                              ShipperDetailField(
                                icon: Icons.schedule_outlined,
                                label: 'Thời gian giao dự kiến',
                                value: formatShipperDateTime(_delivery!.scheduledDateUtc),
                              ),
                              if (_delivery!.deliveredAtUtc != null)
                                ShipperDetailField(
                                  icon: Icons.check_circle_outline,
                                  label: 'Giao thực tế',
                                  value: formatShipperDateTime(_delivery!.deliveredAtUtc!),
                                ),
                              if (_delivery!.notes != null && _delivery!.notes!.isNotEmpty)
                                ShipperDetailField(
                                  icon: Icons.notes_outlined,
                                  label: 'Ghi chú',
                                  value: _delivery!.notes!,
                                ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 16),
                        const ShipperSectionHeader(
                          title: 'Bản đồ điểm giao (OSM)',
                          subtitle: 'Xem vị trí ước lượng từ địa chỉ',
                        ),
                        const SizedBox(height: 10),
                        ShipperOsmMap(
                          markers: [ShipperMapMarker.fromDetail(_delivery!)],
                          origin: kShipperDefaultStart,
                          height: 220,
                          drawRoutePolyline: false,
                          onOpenGoogleMaps: _openMaps,
                        ),
                        const SizedBox(height: 16),
                        const ShipperSectionHeader(title: 'Thao tác'),
                        const SizedBox(height: 10),
                        ..._actionButtons(_delivery!),
                      ],
                    ),
    );
  }
}
