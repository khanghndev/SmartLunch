import 'package:flutter/material.dart';

import '../../../../core/theme/app_design_system.dart';
import '../widgets/organization_ui.dart';

class _ChatMessage {
  final String text;
  final bool fromUser;
  final DateTime at;

  _ChatMessage({required this.text, required this.fromUser}) : at = DateTime.now();
}

/// Chatbot CSKH — giao diện hội thoại (phản hồi mẫu, chưa tích hợp AI backend).
class ChatbotPage extends StatefulWidget {
  const ChatbotPage({super.key});

  @override
  State<ChatbotPage> createState() => _ChatbotPageState();
}

class _ChatbotPageState extends State<ChatbotPage> {
  final _input = TextEditingController();
  final _scroll = ScrollController();
  final List<_ChatMessage> _messages = [
    _ChatMessage(
      fromUser: false,
      text:
          'Xin chào! Tôi là trợ lý HUITMeal. Bạn có thể hỏi về đặt suất, hợp đồng, thanh toán hoặc thực đơn tuần.',
    ),
  ];

  @override
  void dispose() {
    _input.dispose();
    _scroll.dispose();
    super.dispose();
  }

  void _send() {
    final text = _input.text.trim();
    if (text.isEmpty) return;
    setState(() {
      _messages.add(_ChatMessage(text: text, fromUser: true));
      _messages.add(_ChatMessage(
        fromUser: false,
        text: _botReply(text),
      ));
    });
    _input.clear();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scroll.hasClients) {
        _scroll.animateTo(
          _scroll.position.maxScrollExtent,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeOut,
        );
      }
    });
  }

  String _botReply(String user) {
    final q = user.toLowerCase();
    if (q.contains('đặt suất') || q.contains('thực đơn')) {
      return 'Vào menu **Đặt suất ăn tập trung**: bước 1 chọn ngày phục vụ (tối thiểu 3 ngày, từ Thứ 2 tuần sau), bước 2 chọn món theo ngày, bước 3 xác nhận và thanh toán cọc.';
    }
    if (q.contains('hợp đồng') || q.contains('ký')) {
      return 'Xem **Hợp đồng & Thanh toán** để ký số và theo dõi các kỳ thanh toán PayOS.';
    }
    if (q.contains('payos') || q.contains('thanh toán')) {
      return 'Sau khi checkout, hệ thống tạo link PayOS. Bạn có thể sao chép link từ màn hợp đồng nếu cần thanh toán lại.';
    }
    return 'Cảm ơn bạn. Đội CSKH sẽ phản hồi chi tiết qua hotline trong giờ hành chính. '
        'Bạn cũng có thể dùng mục Hợp đồng & Thanh toán trên app.';
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: 'Chatbot CSKH',
      body: Column(
        children: [
          Expanded(
            child: ListView.builder(
              controller: _scroll,
              padding: const EdgeInsets.all(16),
              itemCount: _messages.length,
              itemBuilder: (context, i) {
                final m = _messages[i];
                return Align(
                  alignment:
                      m.fromUser ? Alignment.centerRight : Alignment.centerLeft,
                  child: Container(
                    margin: const EdgeInsets.only(bottom: 8),
                    padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                    constraints: BoxConstraints(
                      maxWidth: MediaQuery.sizeOf(context).width * 0.78,
                    ),
                    decoration: BoxDecoration(
                      color: m.fromUser
                          ? orgAccent
                          : Colors.white,
                      borderRadius: BorderRadius.circular(16),
                      border: m.fromUser
                          ? null
                          : Border.all(color: AppDesignSystem.gray200),
                    ),
                    child: Text(
                      m.text,
                      style: AppDesignSystem.body(
                        size: 14,
                        color: m.fromUser ? Colors.white : AppDesignSystem.gray900,
                      ),
                    ),
                  ),
                );
              },
            ),
          ),
          Container(
            padding: const EdgeInsets.fromLTRB(12, 8, 12, 12),
            decoration: BoxDecoration(
              color: Colors.white,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withValues(alpha: 0.06),
                  blurRadius: 12,
                  offset: const Offset(0, -4),
                ),
              ],
            ),
            child: SafeArea(
              top: false,
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _input,
                      decoration: AppDesignSystem.inputDecoration(
                        label: 'Nhập câu hỏi',
                        focusColor: orgAccent,
                      ),
                      onSubmitted: (_) => _send(),
                    ),
                  ),
                  const SizedBox(width: 8),
                  IconButton.filled(
                    onPressed: _send,
                    style: IconButton.styleFrom(backgroundColor: orgAccent),
                    icon: const Icon(Icons.send_rounded, color: Colors.white),
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
