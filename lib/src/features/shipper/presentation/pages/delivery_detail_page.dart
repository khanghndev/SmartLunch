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
          behavior: SnackBarBehavior.floating,
        ),
      );
    } catch (e) {
      if (!mounted) return;
      setState(() => _updating = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(shipperApiError(e)),
          backgroundColor: AppDesignSystem.danger,
          behavior: SnackBarBehavior.floating,
        ),
      );
    }
  }

  Future<void> _promptNotesThenUpdate(String status, String title) async {
    final ctrl = TextEditingController();
    final notes = await showDialog<String>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Text(title, style: AppDesignSystem.sectionTitle()),
        content: TextField(
          controller: ctrl,
          maxLines: 3,
          decoration: AppDesignSystem.inputDecoration(
            label: 'Ghi chú lý do',
            focusColor: shipperAccent,
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
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
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

  void _openHandoverPdf(ShipperDeliveryDetailModel d) {
    final url = d.handoverDocumentUrl?.trim();
    if (url == null || url.isEmpty) return;
    Navigator.of(context).pushNamed(
      AppRoutes.shipperHandoverPdf,
      arguments: {
        'pdfUrl': url,
        'title': 'Biên bản bàn giao',
        'subtitle': 'Đơn #${d.orderId} · ${d.recipientConfirmedName ?? 'Người nhận'}',
      },
    );
  }

  void _copyAddress(String address) {
    Clipboard.setData(ClipboardData(text: address));
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Đã sao chép địa chỉ'),
        behavior: SnackBarBehavior.floating,
      ),
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
          padding: EdgeInsets.symmetric(vertical: 20),
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
        const SizedBox(height: 10),
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
        const SizedBox(height: 10),
        ShipperOutlineButton(
          label: 'Giao thất bại',
          icon: Icons.error_outline,
          onPressed: () => _promptNotesThenUpdate('failed', 'Lý do giao thất bại'),
        ),
      ]);
    } else if (s == 'in_transit') {
      if (d.requiresRecipientSignature) {
        buttons.add(
          ShipperInfoBanner(
            message:
                'Đơn đang giao: thu chữ ký và tên người nhận trên màn hình xác nhận PoD.',
            icon: Icons.draw_outlined,
            color: shipperAccent,
          ),
        );
        buttons.add(const SizedBox(height: 10));
      }
      buttons.addAll([
        ShipperPrimaryButton(
          label: 'Xác nhận giao (ảnh + chữ ký)',
          icon: Icons.camera_alt_outlined,
          onPressed: _openProof,
        ),
        const SizedBox(height: 10),
        ShipperOutlineButton(
          label: 'Giao thất bại',
          icon: Icons.error_outline,
          onPressed: () => _promptNotesThenUpdate('failed', 'Lý do giao thất bại'),
        ),
      ]);
    } else if (s == 'completed') {
      if (d.proofImageUrl != null && d.proofImageUrl!.isNotEmpty) {
        buttons.add(
          ShipperContentCard(
            title: 'Ảnh xác nhận giao (PoD)',
            headerIcon: Icons.verified_outlined,
            accent: AppDesignSystem.success,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ShipperProofImage(imageUrl: d.proofImageUrl, height: 220),
                if (d.proofCapturedAtUtc != null) ...[
                  const SizedBox(height: 10),
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
                        .copyWith(fontWeight: FontWeight.w700),
                  ),
                ],
                if (d.recipientConfirmedName != null &&
                    d.recipientConfirmedName!.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Text(
                    'Người nhận: ${d.recipientConfirmedName}',
                    style: AppDesignSystem.body(size: 12),
                  ),
                ],
                if (d.recipientConfirmedAtUtc != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    'Xác nhận lúc: ${formatShipperDateTime(d.recipientConfirmedAtUtc!)}',
                    style: AppDesignSystem.body(size: 12),
                  ),
                ],
                if (d.shipperSignatureUrl != null &&
                    d.shipperSignatureUrl!.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    'Chữ ký shipper',
                    style: AppDesignSystem.label(color: AppDesignSystem.gray500),
                  ),
                  const SizedBox(height: 8),
                  ShipperProofImage(imageUrl: d.shipperSignatureUrl, height: 120),
                ],
                if (d.recipientSignatureUrl != null &&
                    d.recipientSignatureUrl!.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    'Chữ ký người nhận',
                    style: AppDesignSystem.label(color: AppDesignSystem.gray500),
                  ),
                  const SizedBox(height: 8),
                  ShipperProofImage(imageUrl: d.recipientSignatureUrl, height: 120),
                ],
                if (d.handoverDocumentUrl != null &&
                    d.handoverDocumentUrl!.isNotEmpty) ...[
                  const SizedBox(height: 14),
                  ShipperOutlineButton(
                    label: 'Xem biên bản bàn giao (PDF)',
                    icon: Icons.picture_as_pdf_outlined,
                    onPressed: () => _openHandoverPdf(d),
                  ),
                ],
              ],
            ),
          ),
        );
      }
    }

    if (buttons.isEmpty) {
      buttons.add(
        ShipperInfoBanner(
          message: 'Đơn ở trạng thái ${shipperDeliveryStatusLabelVi(d.deliveryStatus)} — không còn thao tác.',
          icon: Icons.info_outline_rounded,
          color: shipperAccent,
        ),
      );
    }

    return buttons;
  }

  @override
  Widget build(BuildContext context) {
    final d = _delivery;

    return ShipperPageShell(
      title: 'Chi tiết giao hàng',
      onRefresh: _load,
      body: _loading
          ? const ShipperLoadingBody(message: 'Đang tải thông tin đơn…')
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : d == null
                  ? const ShipperLoadingBody()
                  : ModuleListView(
                      padding: shipperListPadding(context),
                      children: [
                        ShipperPageIntro(
                          title: 'Đơn giao #${d.deliveryId}',
                          description:
                              'Theo dõi tiến trình, xem bản đồ và cập nhật trạng thái theo quy trình giao hàng.',
                          icon: Icons.local_shipping_rounded,
                        ),
                        const SizedBox(height: 12),
                        ShipperOrderHeaderCard(
                          orderId: d.orderId,
                          status: d.deliveryStatus,
                          subtitle:
                              '${d.mealCount} suất · ${formatShipperDateTime(d.scheduledDateUtc)}',
                        ),
                        const SizedBox(height: 12),
                        ShipperDeliveryProgressBar(status: d.deliveryStatus),
                        const SizedBox(height: 12),
                        ShipperAddressHighlightCard(
                          address: d.deliveryAddress,
                          onOpenMaps: _openMaps,
                          onCopy: () => _copyAddress(d.deliveryAddress),
                        ),
                        const SizedBox(height: 12),
                        ShipperContentCard(
                          title: 'Thông tin đơn',
                          subtitle: 'Suất ăn, lịch giao và ghi chú',
                          headerIcon: Icons.receipt_long_outlined,
                          accent: shipperAccent,
                          child: Column(
                            children: [
                              ShipperDetailField(
                                icon: Icons.tag_outlined,
                                label: 'Mã đơn giao',
                                value: '#${d.deliveryId}',
                              ),
                              ShipperDetailField(
                                icon: Icons.shopping_bag_outlined,
                                label: 'Mã đơn hàng',
                                value: '#${d.orderId}',
                              ),
                              ShipperDetailField(
                                icon: Icons.restaurant_outlined,
                                label: 'Số suất ăn',
                                value: '${d.mealCount} suất',
                              ),
                              ShipperDetailField(
                                icon: Icons.schedule_outlined,
                                label: 'Thời gian giao dự kiến',
                                value: formatShipperDateTime(d.scheduledDateUtc),
                              ),
                              if (d.deliveredAtUtc != null)
                                ShipperDetailField(
                                  icon: Icons.check_circle_outline,
                                  label: 'Giao thực tế',
                                  value: formatShipperDateTime(d.deliveredAtUtc!),
                                ),
                              if (d.recipientConfirmedName != null &&
                                  d.recipientConfirmedName!.isNotEmpty)
                                ShipperDetailField(
                                  icon: Icons.person_outline,
                                  label: 'Người nhận xác nhận',
                                  value: d.recipientConfirmedName!,
                                ),
                              if (d.recipientConfirmedAtUtc != null)
                                ShipperDetailField(
                                  icon: Icons.verified_user_outlined,
                                  label: 'Thời điểm xác nhận',
                                  value: formatShipperDateTime(d.recipientConfirmedAtUtc!),
                                ),
                              if (d.notes != null && d.notes!.isNotEmpty)
                                ShipperDetailField(
                                  icon: Icons.notes_outlined,
                                  label: 'Ghi chú',
                                  value: d.notes!,
                                ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 12),
                        ShipperContentCard(
                          title: 'Bản đồ điểm giao',
                          subtitle: 'OpenStreetMap · tọa độ ước lượng từ địa chỉ',
                          headerIcon: Icons.map_outlined,
                          accent: AppDesignSystem.info,
                          child: ShipperOsmMap(
                            markers: [ShipperMapMarker.fromDetail(d)],
                            origin: kShipperDefaultStart,
                            height: 240,
                            drawRoutePolyline: false,
                            showGoogleMapsButton: false,
                          ),
                        ),
                        const SizedBox(height: 10),
                        ShipperOutlineButton(
                          label: 'Mở Google Maps chỉ đường',
                          icon: Icons.navigation_rounded,
                          onPressed: _openMaps,
                        ),
                        const SizedBox(height: 12),
                        ShipperContentCard(
                          title: 'Thao tác giao hàng',
                          subtitle: 'Cập nhật trạng thái theo quy trình',
                          headerIcon: Icons.touch_app_outlined,
                          accent: AppDesignSystem.success,
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: _actionButtons(d),
                          ),
                        ),
                        const SizedBox(height: 8),
                      ],
                    ),
    );
  }
}
