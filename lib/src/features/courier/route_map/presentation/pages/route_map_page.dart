import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class RouteMapPage extends StatelessWidget {
  const RouteMapPage({super.key});

  @override
  Widget build(BuildContext context) {
    final stops = const [
      _Stop(title: 'Văn phòng Q1', eta: '08:45', distance: '1.2 km'),
      _Stop(title: 'KTX Zone B', eta: '09:15', distance: '3.4 km'),
      _Stop(title: 'Kho Q7', eta: '10:00', distance: '6.1 km'),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Bản đồ tuyến đường'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: Column(
        children: [
          Container(
            margin: const EdgeInsets.fromLTRB(20, 16, 20, 10),
            padding: const EdgeInsets.all(14),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(14),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.04),
                  blurRadius: 12,
                  offset: const Offset(0, 8),
                ),
              ],
            ),
            child: Row(
              children: const [
                Icon(Icons.place_rounded, color: AppColors.courier),
                SizedBox(width: 10),
                Text(
                  'Điểm dừng hôm nay',
                  style: TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w900,
                    fontSize: 16,
                  ),
                ),
              ],
            ),
          ),
          Expanded(
            child: Container(
              margin: const EdgeInsets.symmetric(horizontal: 12),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(18),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.03),
                    blurRadius: 12,
                    offset: const Offset(0, 6),
                  ),
                ],
              ),
              child: const Center(
                child: Text(
                  'Map preview',
                  style: TextStyle(
                    color: AppColors.ink,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
            ),
          ),
          const SizedBox(height: 10),
          Container(
            height: 200,
            margin: const EdgeInsets.fromLTRB(20, 0, 20, 16),
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(14),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.04),
                  blurRadius: 12,
                  offset: const Offset(0, 8),
                ),
              ],
            ),
            child: ListView.separated(
              physics: const BouncingScrollPhysics(),
              itemBuilder:
                  (context, i) => _StopTile(stop: stops[i], index: i + 1),
              separatorBuilder: (_, __) => const Divider(height: 10),
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
    return Row(
      children: [
        CircleAvatar(
          radius: 18,
          backgroundColor: AppColors.courier.withOpacity(0.12),
          child: Text(
            '$index',
            style: const TextStyle(
              color: AppColors.courier,
              fontWeight: FontWeight.w800,
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
                style: const TextStyle(
                  color: AppColors.ink,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                'ETA: ${stop.eta} · ${stop.distance}',
                style: TextStyle(
                  color: AppColors.ink.withOpacity(0.65),
                  fontWeight: FontWeight.w600,
                ),
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
    );
  }
}
