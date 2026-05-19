import 'package:flutter/material.dart';

class MealDetailPage extends StatelessWidget {
  const MealDetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    final args = ModalRoute.of(context)?.settings.arguments;
    final dishId = args is int ? args : 0;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Chi tiết món ăn'),
      ),
      body: Center(
        child: Text('Chi tiết món ăn ID: $dishId\n(Đang phát triển)'),
      ),
    );
  }
}
