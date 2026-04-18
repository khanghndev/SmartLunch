import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';

class NotificationSettingsPage extends StatefulWidget {
  const NotificationSettingsPage({super.key});

  @override
  State<NotificationSettingsPage> createState() =>
      _NotificationSettingsPageState();
}

class _NotificationSettingsPageState extends State<NotificationSettingsPage> {
  bool pushOrder = true;
  bool pushPromo = true;
  bool emailSummary = false;
  bool smsDelivery = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7FA),
      appBar: AppBar(
        title: const Text('Thông báo'),
        centerTitle: true,
        backgroundColor: Colors.white,
        foregroundColor: AppColors.ink,
        elevation: 0,
        surfaceTintColor: Colors.white,
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 16, 20, 28),
        child: Column(
          children: [
            _Card(
              title: 'Thông báo đẩy',
              subtitle: 'Nhận ngay khi có cập nhật quan trọng',
              children: [
                _SwitchTile(
                  icon: Icons.receipt_long_rounded,
                  color: AppColors.customer,
                  label: 'Trạng thái đơn hàng',
                  subtitle: 'Xác nhận, đang giao, hoàn thành',
                  value: pushOrder,
                  onChanged: (v) => setState(() => pushOrder = v),
                ),
                const SizedBox(height: 6),
                _SwitchTile(
                  icon: Icons.local_offer_rounded,
                  color: AppColors.customer,
                  label: 'Ưu đãi & voucher',
                  subtitle: 'Giảm giá, tích điểm, chương trình mới',
                  value: pushPromo,
                  onChanged: (v) => setState(() => pushPromo = v),
                ),
              ],
            ),
            const SizedBox(height: 14),
            _Card(
              title: 'Email & SMS',
              subtitle: 'Tóm tắt định kỳ và thông báo khẩn',
              children: [
                _SwitchTile(
                  icon: Icons.mail_outline,
                  color: AppColors.org,
                  label: 'Email tóm tắt tuần',
                  subtitle: 'Số đơn, chi tiêu, điểm thưởng',
                  value: emailSummary,
                  onChanged: (v) => setState(() => emailSummary = v),
                ),
                const SizedBox(height: 6),
                _SwitchTile(
                  icon: Icons.sms_outlined,
                  color: AppColors.customerAlt,
                  label: 'SMS giao hàng',
                  subtitle: 'Tin nhắn khi shipper sắp tới',
                  value: smsDelivery,
                  onChanged: (v) => setState(() => smsDelivery = v),
                ),
              ],
            ),
            const SizedBox(height: 14),
            const _QuietHoursCard(),
            const SizedBox(height: 20),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.customer,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  elevation: 0,
                ),
                onPressed: () {},
                child: const Text(
                  'Lưu cài đặt',
                  style: TextStyle(fontWeight: FontWeight.w800, fontSize: 16),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _Card extends StatelessWidget {
  final String title;
  final String subtitle;
  final List<Widget> children;

  const _Card({
    required this.title,
    required this.subtitle,
    required this.children,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.w900,
              color: AppColors.ink,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            subtitle,
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.6),
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 10),
          ...children,
        ],
      ),
    );
  }
}

class _SwitchTile extends StatelessWidget {
  final IconData icon;
  final Color color;
  final String label;
  final String subtitle;
  final bool value;
  final ValueChanged<bool> onChanged;

  const _SwitchTile({
    required this.icon,
    required this.color,
    required this.label,
    required this.subtitle,
    required this.value,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: color.withOpacity(0.12),
              shape: BoxShape.circle,
            ),
            child: Icon(icon, color: color),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.9),
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  subtitle,
                  style: TextStyle(
                    color: AppColors.ink.withOpacity(0.6),
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ),
          Switch.adaptive(
            value: value,
            activeColor: AppColors.customer,
            onChanged: onChanged,
          ),
        ],
      ),
    );
  }
}

class _QuietHoursCard extends StatelessWidget {
  const _QuietHoursCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFECF3FF),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.org.withOpacity(0.16)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Colors.white,
                  shape: BoxShape.circle,
                ),
                child: const Icon(Icons.nightlight_round, color: AppColors.org),
              ),
              const SizedBox(width: 10),
              Text(
                'Chế độ yên lặng',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w900,
                  color: AppColors.org,
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          Text(
            'Tắt thông báo từ 22:00 - 07:00, vẫn nhận thông báo khẩn qua email.',
            style: TextStyle(
              color: AppColors.ink.withOpacity(0.7),
              fontWeight: FontWeight.w600,
              height: 1.3,
            ),
          ),
          const SizedBox(height: 10),
          OutlinedButton(
            onPressed: () {},
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.org,
              side: BorderSide(color: AppColors.org.withOpacity(0.35)),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            child: const Text(
              'Chỉnh thời gian yên lặng',
              style: TextStyle(fontWeight: FontWeight.w800),
            ),
          ),
        ],
      ),
    );
  }
}
