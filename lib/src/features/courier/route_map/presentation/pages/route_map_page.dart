import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class RouteMapPage extends StatelessWidget {
  const RouteMapPage({super.key});

  @override
  Widget build(BuildContext context) {
    final stops = const [
      _Stop(title: 'Kho trung chuyển Quận 4', eta: '08:45', distance: '1.2 km'),
      _Stop(title: 'Văn phòng Q1', eta: '09:10', distance: '2.8 km'),
      _Stop(title: 'KTX Zone B', eta: '09:35', distance: '4.6 km'),
      _Stop(title: 'Kho phụ Quận 7', eta: '10:00', distance: '6.1 km'),
    ];

    return Scaffold(
      backgroundColor: AppColors.surface,
      appBar: AppBar(title: const Text('Bản đồ tuyến đường')),
      body: CustomScrollView(
        physics: const BouncingScrollPhysics(),
        slivers: [
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 12, 20, 12),
              child: _RouteOverviewCard(stops: stops.length),
            ),
          ),
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: _MapPreview(stops: stops),
            ),
          ),
          SliverPadding(
            padding: const EdgeInsets.fromLTRB(20, 14, 20, 90),
            sliver: SliverList.separated(
              itemBuilder:
                  (_, index) => _StopTile(stop: stops[index], index: index),
              separatorBuilder: (_, __) => const SizedBox(height: 10),
              itemCount: stops.length,
            ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.courier,
        foregroundColor: Colors.white,
        onPressed: () {},
        icon: const Icon(Icons.navigation_rounded),
        label: const Text('Bắt đầu điều hướng'),
      ),
    );
  }
}

class _RouteOverviewCard extends StatelessWidget {
  final int stops;

  const _RouteOverviewCard({required this.stops});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        children: [
          Container(
            width: 42,
            height: 42,
            decoration: BoxDecoration(
              color: AppColors.tint(AppColors.courier, 0.14),
              borderRadius: BorderRadius.circular(12),
            ),
            child: const Icon(Icons.route_rounded, color: AppColors.courier),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Lộ trình sáng nay',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  '$stops điểm dừng • tổng quãng đường 6.1 km',
                  style: Theme.of(
                    context,
                  ).textTheme.bodySmall?.copyWith(color: AppColors.inkSoft),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _MapPreview extends StatelessWidget {
  final List<_Stop> stops;

  const _MapPreview({required this.stops});

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 280,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(22),
        gradient: const LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [Color(0xFFDCEEFF), Color(0xFFF5FBFF)],
        ),
        border: Border.all(color: AppColors.border),
      ),
      child: Stack(
        children: [
          Positioned.fill(
            child: CustomPaint(painter: _RoutePainter(count: stops.length)),
          ),
          Positioned(
            top: 16,
            right: 16,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.95),
                borderRadius: BorderRadius.circular(10),
                border: Border.all(color: AppColors.border),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: const [
                  Icon(Icons.speed_rounded, size: 16, color: AppColors.courier),
                  SizedBox(width: 6),
                  Text(
                    'Nhanh nhất',
                    style: TextStyle(
                      color: AppColors.ink,
                      fontWeight: FontWeight.w700,
                      fontSize: 12,
                    ),
                  ),
                ],
              ),
            ),
          ),
          Positioned(
            bottom: 16,
            left: 16,
            right: 16,
            child: Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.95),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppColors.border),
              ),
              child: Row(
                children: [
                  const Icon(
                    Icons.local_shipping_rounded,
                    color: AppColors.courier,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Đang di chuyển đến: ${stops[1].title}',
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _RoutePainter extends CustomPainter {
  final int count;

  const _RoutePainter({required this.count});

  @override
  void paint(Canvas canvas, Size size) {
    final roadPaint =
        Paint()
          ..color = AppColors.courier.withOpacity(0.18)
          ..style = PaintingStyle.stroke
          ..strokeWidth = 6
          ..strokeCap = StrokeCap.round;

    final path =
        Path()
          ..moveTo(28, size.height - 46)
          ..quadraticBezierTo(
            size.width * 0.22,
            size.height * 0.7,
            size.width * 0.36,
            size.height * 0.58,
          )
          ..quadraticBezierTo(
            size.width * 0.52,
            size.height * 0.43,
            size.width * 0.63,
            size.height * 0.36,
          )
          ..quadraticBezierTo(
            size.width * 0.78,
            size.height * 0.22,
            size.width - 32,
            52,
          );

    canvas.drawPath(path, roadPaint);

    final markerPaint = Paint()..color = AppColors.courier;
    final markerCount = count < 2 ? 2 : count;
    for (var i = 0; i < markerCount; i++) {
      final progress = i / (markerCount - 1);
      final metrics = path.computeMetrics().first;
      final tangent = metrics.getTangentForOffset(metrics.length * progress);
      if (tangent == null) continue;
      final pos = tangent.position;
      canvas.drawCircle(pos, 8, markerPaint);
      canvas.drawCircle(pos, 3.5, Paint()..color = Colors.white);
    }
  }

  @override
  bool shouldRepaint(covariant _RoutePainter oldDelegate) =>
      oldDelegate.count != count;
}

class _Stop {
  final String title;
  final String eta;
  final String distance;

  const _Stop({required this.title, required this.eta, required this.distance});
}

class _StopTile extends StatelessWidget {
  final _Stop stop;
  final int index;

  const _StopTile({required this.stop, required this.index});

  @override
  Widget build(BuildContext context) {
    final isCurrent = index == 1;
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(
          color:
              isCurrent
                  ? AppColors.courier.withOpacity(0.35)
                  : AppColors.border,
        ),
      ),
      child: Row(
        children: [
          Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
              color:
                  isCurrent
                      ? AppColors.tint(AppColors.courier, 0.16)
                      : AppColors.surfaceStrong,
              borderRadius: BorderRadius.circular(10),
            ),
            child: Center(
              child: Text(
                '${index + 1}',
                style: TextStyle(
                  color: isCurrent ? AppColors.courier : AppColors.inkSoft,
                  fontWeight: FontWeight.w800,
                ),
              ),
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  stop.title,
                  style: Theme.of(
                    context,
                  ).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w700),
                ),
                const SizedBox(height: 2),
                Text(
                  'ETA ${stop.eta} • ${stop.distance}',
                  style: Theme.of(
                    context,
                  ).textTheme.bodySmall?.copyWith(color: AppColors.inkSoft),
                ),
              ],
            ),
          ),
          IconButton(
            onPressed: () {},
            icon: const Icon(Icons.directions_rounded),
            color: AppColors.courier,
          ),
        ],
      ),
    );
  }
}
