import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/theme/app_design_system.dart';
import '../../data/models/shipper_delivery_models.dart';
import '../../data/repositories/shipper_repository.dart';
import '../widgets/shipper_ui.dart';

class DeliveryHistoryPage extends StatefulWidget {
  const DeliveryHistoryPage({super.key});

  @override
  State<DeliveryHistoryPage> createState() => _DeliveryHistoryPageState();
}

class _DeliveryHistoryPageState extends State<DeliveryHistoryPage> {
  static const _filters = ['Tất cả', 'Hoàn tất', 'Thất bại'];

  int _filterIndex = 0;
  List<ShipperDeliveryListItemModel> _items = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final List<ShipperDeliveryListItemModel> merged;
      if (_filterIndex == 1) {
        merged = (await ShipperRepository.instance.getDeliveries(
          status: 'completed',
          pageSize: 50,
        ))
            .data;
      } else if (_filterIndex == 2) {
        final failed = await ShipperRepository.instance.getDeliveries(
          status: 'failed',
          pageSize: 50,
        );
        final rejected = await ShipperRepository.instance.getDeliveries(
          status: 'rejected',
          pageSize: 50,
        );
        merged = [...failed.data, ...rejected.data];
      } else {
        final completed = await ShipperRepository.instance.getDeliveries(
          status: 'completed',
          pageSize: 50,
        );
        final failed = await ShipperRepository.instance.getDeliveries(
          status: 'failed',
          pageSize: 50,
        );
        final rejected = await ShipperRepository.instance.getDeliveries(
          status: 'rejected',
          pageSize: 50,
        );
        merged = [...completed.data, ...failed.data, ...rejected.data];
      }
      merged.sort((a, b) => b.scheduledDateUtc.compareTo(a.scheduledDateUtc));
      if (!mounted) return;
      setState(() {
        _items = merged;
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

  int get _completedCount =>
      _items.where((d) => d.deliveryStatus.toLowerCase() == 'completed').length;

  int get _failedCount => _items.length - _completedCount;

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: 'Lịch sử giao hàng',
      onRefresh: _load,
      body: _loading
          ? const ShipperLoadingBody(message: 'Đang tải lịch sử…')
          : _error != null
              ? ShipperErrorBody(message: _error!, onRetry: _load)
              : ModuleListView(
                  padding: shipperListPadding(context),
                  children: [
                    const ShipperPageIntro(
                      title: 'Lịch sử giao hàng',
                      description:
                          'Đơn đã hoàn tất hoặc thất bại / từ chối. Chạm để xem chi tiết và ảnh PoD.',
                      icon: Icons.history_rounded,
                    ),
                    const SizedBox(height: 14),
                    ShipperContentCard(
                      title: 'Bộ lọc lịch sử',
                      accent: shipperAccent,
                      child: ShipperPeriodChips(
                        labels: _filters,
                        selected: _filterIndex,
                        onSelected: (i) {
                          setState(() => _filterIndex = i);
                          _load();
                        },
                      ),
                    ),
                    if (_items.isNotEmpty) ...[
                      const SizedBox(height: 14),
                      ShipperContentCard(
                        title: 'Tổng kết',
                        subtitle: '${_items.length} đơn · ${_filters[_filterIndex]}',
                        accent: AppDesignSystem.success,
                        child: Row(
                          children: [
                            Expanded(
                              child: ShipperStatTile(
                                label: 'Hoàn tất',
                                value: '$_completedCount',
                                icon: Icons.check_circle_outline,
                                color: AppDesignSystem.success,
                              ),
                            ),
                            const SizedBox(width: 10),
                            Expanded(
                              child: ShipperStatTile(
                                label: 'Thất bại / từ chối',
                                value: '$_failedCount',
                                icon: Icons.cancel_outlined,
                                color: AppDesignSystem.danger,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                    const SizedBox(height: 16),
                    ShipperSectionHeader(
                      title: 'Danh sách',
                      subtitle: '${_items.length} đơn',
                    ),
                    const SizedBox(height: 10),
                    if (_items.isEmpty)
                      const ShipperEmptyList(
                        message: 'Chưa có đơn hoàn tất hoặc thất bại',
                        icon: Icons.history_rounded,
                      )
                    else
                      ..._items.map(
                        (item) => ShipperDeliveryTile(
                          item: item,
                          onTap: () => Navigator.of(context).pushNamed(
                            AppRoutes.shipperDeliveryDetail,
                            arguments: item.deliveryId,
                          ),
                        ),
                      ),
                  ],
                ),
    );
  }
}
