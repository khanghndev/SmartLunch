import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';

class ChatbotPage extends StatelessWidget {
  const ChatbotPage({super.key});

  @override
  Widget build(BuildContext context) {
    final messages = [
      const _Message(
        text: 'Chào bạn, SmartLunch có thể giúp gì hôm nay?',
        isMe: false,
        time: '10:20',
      ),
      const _Message(
        text: 'Mình muốn kiểm tra đơn #SL1024 đang ở đâu?',
        isMe: true,
        time: '10:21',
      ),
      const _Message(
        text:
            'Đơn #SL1024 đang giao, dự kiến đến 11:40. Bạn có cần đổi địa chỉ không?',
        isMe: false,
        time: '10:21',
      ),
    ];

    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: Row(
          children: const [
            CircleAvatar(
              radius: 16,
              backgroundColor: AppColors.customer,
              child: Icon(Icons.support_agent, color: Colors.white, size: 18),
            ),
            SizedBox(width: 10),
            Text('SmartLunch'),
          ],
        ),
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
        actions: [
          IconButton(onPressed: () {}, icon: const Icon(Icons.history_rounded)),
          IconButton(
            onPressed: () {},
            icon: const Icon(Icons.more_vert_rounded),
          ),
        ],
      ),
      body: Column(
        children: [
          const _QuickActions(),
          Expanded(
            child: ListView.builder(
              physics: const BouncingScrollPhysics(),
              padding: const EdgeInsets.fromLTRB(16, 10, 16, 10),
              itemCount: messages.length,
              itemBuilder:
                  (context, index) => _MessageBubble(message: messages[index]),
            ),
          ),
          const _InputBar(),
        ],
      ),
    );
  }
}

class _QuickActions extends StatelessWidget {
  const _QuickActions();

  @override
  Widget build(BuildContext context) {
    final actions = [
      _ActionChipData('Theo dõi đơn hàng', Icons.receipt_long_rounded),
      _ActionChipData('Đổi địa chỉ giao', Icons.location_on_rounded),
      _ActionChipData('Ưu đãi hôm nay', Icons.local_offer_rounded),
    ];
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Row(
          children:
              actions
                  .map(
                    (a) => Padding(
                      padding: const EdgeInsets.only(right: 8),
                      child: _QuickActionChip(data: a),
                    ),
                  )
                  .toList(),
        ),
      ),
    );
  }
}

class _ActionChipData {
  final String label;
  final IconData icon;

  const _ActionChipData(this.label, this.icon);
}

class _QuickActionChip extends StatelessWidget {
  final _ActionChipData data;

  const _QuickActionChip({required this.data});

  @override
  Widget build(BuildContext context) {
    return ActionChip(
      avatar: Icon(data.icon, size: 18, color: AppColors.customer),
      backgroundColor: AppColors.customer.withOpacity(0.08),
      label: Text(
        data.label,
        style: TextStyle(
          color: AppColors.ink.withOpacity(0.85),
          fontWeight: FontWeight.w700,
        ),
      ),
      onPressed: () {},
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: AppColors.customer.withOpacity(0.16)),
      ),
    );
  }
}

class _Message {
  final String text;
  final bool isMe;
  final String time;

  const _Message({required this.text, required this.isMe, required this.time});
}

class _MessageBubble extends StatelessWidget {
  final _Message message;

  const _MessageBubble({required this.message});

  @override
  Widget build(BuildContext context) {
    final color = message.isMe ? AppColors.customer : Colors.white;
    final textColor =
        message.isMe ? Colors.white : AppColors.ink.withOpacity(0.85);
    return Align(
      alignment: message.isMe ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        constraints: BoxConstraints(
          maxWidth: MediaQuery.of(context).size.width * 0.82,
        ),
        margin: const EdgeInsets.symmetric(vertical: 6),
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: color,
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(14),
            topRight: const Radius.circular(14),
            bottomLeft:
                message.isMe
                    ? const Radius.circular(14)
                    : const Radius.circular(4),
            bottomRight:
                message.isMe
                    ? const Radius.circular(4)
                    : const Radius.circular(14),
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 8,
              offset: const Offset(0, 4),
            ),
          ],
          border: Border.all(
            color:
                message.isMe
                    ? Colors.transparent
                    : AppColors.ink.withOpacity(0.06),
          ),
        ),
        child: Column(
          crossAxisAlignment:
              message.isMe ? CrossAxisAlignment.end : CrossAxisAlignment.start,
          children: [
            Text(
              message.text,
              style: TextStyle(
                color: textColor,
                fontWeight: FontWeight.w600,
                height: 1.35,
              ),
            ),
            const SizedBox(height: 6),
            Text(
              message.time,
              style: TextStyle(
                color: textColor.withOpacity(0.7),
                fontWeight: FontWeight.w600,
                fontSize: 12,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _InputBar extends StatelessWidget {
  const _InputBar();

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Container(
        padding: const EdgeInsets.fromLTRB(16, 6, 16, 12),
        decoration: const BoxDecoration(
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: Colors.black12,
              blurRadius: 10,
              offset: Offset(0, -2),
            ),
          ],
        ),
        child: Row(
          children: [
            IconButton(
              onPressed: () {},
              icon: const Icon(Icons.add_rounded),
              color: AppColors.customer,
            ),
            Expanded(
              child: TextField(
                decoration: InputDecoration(
                  hintText: 'Nhắn tin cho SmartLunch...',
                  filled: true,
                  fillColor: const Color(0xFFF8F9FB),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(24),
                    borderSide: BorderSide(
                      color: AppColors.ink.withOpacity(0.08),
                    ),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(24),
                    borderSide: const BorderSide(
                      color: AppColors.customer,
                      width: 1.4,
                    ),
                  ),
                  contentPadding: const EdgeInsets.symmetric(
                    horizontal: 14,
                    vertical: 12,
                  ),
                ),
              ),
            ),
            const SizedBox(width: 10),
            FloatingActionButton.small(
              heroTag: 'send-btn',
              onPressed: () {},
              backgroundColor: AppColors.customer,
              child: const Icon(Icons.send_rounded, color: Colors.white),
            ),
          ],
        ),
      ),
    );
  }
}
