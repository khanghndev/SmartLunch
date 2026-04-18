import 'package:flutter/material.dart';

import '../../../../../core/constants/app_colors.dart';

class ProofOfDeliveryPage extends StatefulWidget {
  const ProofOfDeliveryPage({super.key});

  @override
  State<ProofOfDeliveryPage> createState() => _ProofOfDeliveryPageState();
}

class _ProofOfDeliveryPageState extends State<ProofOfDeliveryPage> {
  bool _receivedByGuard = true;
  bool _sealedPackage = true;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.surface,
      appBar: AppBar(title: const Text('Minh chứng giao hàng')),
      body: ListView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.fromLTRB(20, 10, 20, 98),
        children: [
          _TaskSummaryCard(),
          const SizedBox(height: 12),
          _UploadCard(),
          const SizedBox(height: 12),
          _FormCard(
            receivedByGuard: _receivedByGuard,
            sealedPackage: _sealedPackage,
            onReceivedByGuardChanged: (value) {
              setState(() => _receivedByGuard = value);
            },
            onSealedPackageChanged: (value) {
              setState(() => _sealedPackage = value);
            },
          ),
          const SizedBox(height: 12),
          _ChecklistCard(
            receivedByGuard: _receivedByGuard,
            sealedPackage: _sealedPackage,
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.courier,
        foregroundColor: Colors.white,
        onPressed: () {},
        icon: const Icon(Icons.check_circle_rounded),
        label: const Text('Gửi xác nhận'),
      ),
    );
  }
}

class _TaskSummaryCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppColors.courier, AppColors.courierAlt],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Đơn #DL-2301',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w800,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            'Văn phòng Q1 · 08:52 · 25/03',
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
              color: Colors.white.withOpacity(0.92),
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 10),
          Row(
            children: const [
              _Tag(text: '4 suất ăn'),
              SizedBox(width: 8),
              _Tag(text: 'COD 375.000đ'),
              SizedBox(width: 8),
              _Tag(text: 'Ưu tiên cao'),
            ],
          ),
        ],
      ),
    );
  }
}

class _Tag extends StatelessWidget {
  final String text;

  const _Tag({required this.text});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.2),
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        text,
        style: Theme.of(context).textTheme.labelSmall?.copyWith(
          color: Colors.white,
          fontWeight: FontWeight.w700,
        ),
      ),
    );
  }
}

class _UploadCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _SectionHeader(
            icon: Icons.camera_alt_outlined,
            title: 'Ảnh minh chứng',
          ),
          const SizedBox(height: 10),
          SizedBox(
            height: 120,
            child: Row(
              children: [
                _UploadTile(
                  icon: Icons.add_a_photo_rounded,
                  label: 'Chụp ảnh',
                  highlighted: true,
                ),
                const SizedBox(width: 10),
                _UploadTile(
                  icon: Icons.collections_outlined,
                  label: 'Thư viện',
                ),
                const SizedBox(width: 10),
                _UploadTile(
                  icon: Icons.receipt_long_outlined,
                  label: 'Biên nhận',
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _UploadTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final bool highlighted;

  const _UploadTile({
    required this.icon,
    required this.label,
    this.highlighted = false,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color:
              highlighted
                  ? AppColors.tint(AppColors.courier, 0.11)
                  : AppColors.surfaceStrong,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color:
                highlighted
                    ? AppColors.courier.withOpacity(0.32)
                    : Colors.transparent,
          ),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              icon,
              color: highlighted ? AppColors.courier : AppColors.inkSoft,
              size: 24,
            ),
            const SizedBox(height: 8),
            Text(
              label,
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.labelMedium?.copyWith(
                color: highlighted ? AppColors.courier : AppColors.inkSoft,
                fontWeight: FontWeight.w700,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _FormCard extends StatelessWidget {
  final bool receivedByGuard;
  final bool sealedPackage;
  final ValueChanged<bool> onReceivedByGuardChanged;
  final ValueChanged<bool> onSealedPackageChanged;

  const _FormCard({
    required this.receivedByGuard,
    required this.sealedPackage,
    required this.onReceivedByGuardChanged,
    required this.onSealedPackageChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _SectionHeader(
            icon: Icons.receipt_long_rounded,
            title: 'Thông tin giao',
          ),
          const SizedBox(height: 10),
          const _InputField(
            label: 'Mã giao hàng',
            hint: '#DL-2301',
            icon: Icons.tag_rounded,
          ),
          const SizedBox(height: 10),
          const _InputField(
            label: 'Thời gian giao thực tế',
            hint: '08:52',
            icon: Icons.access_time_rounded,
          ),
          const SizedBox(height: 10),
          const _InputField(
            label: 'Người nhận',
            hint: 'Lễ tân tầng 1',
            icon: Icons.person_outline_rounded,
          ),
          const SizedBox(height: 10),
          const _InputField(
            label: 'Ghi chú',
            hint: 'Hàng nguyên seal, khách sẽ kiểm tra sau.',
            icon: Icons.edit_note_rounded,
            maxLines: 3,
          ),
          const SizedBox(height: 8),
          SwitchListTile(
            contentPadding: EdgeInsets.zero,
            title: const Text('Giao cho lễ tân/bảo vệ'),
            value: receivedByGuard,
            activeColor: AppColors.courier,
            onChanged: onReceivedByGuardChanged,
          ),
          SwitchListTile(
            contentPadding: EdgeInsets.zero,
            title: const Text('Kiện hàng còn nguyên niêm phong'),
            value: sealedPackage,
            activeColor: AppColors.courier,
            onChanged: onSealedPackageChanged,
          ),
        ],
      ),
    );
  }
}

class _ChecklistCard extends StatelessWidget {
  final bool receivedByGuard;
  final bool sealedPackage;

  const _ChecklistCard({
    required this.receivedByGuard,
    required this.sealedPackage,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _SectionHeader(
            icon: Icons.rule_folder_outlined,
            title: 'Checklist xác nhận',
          ),
          const SizedBox(height: 10),
          _CheckRow(text: 'Đã chụp ít nhất 1 ảnh minh chứng', passed: true),
          _CheckRow(text: 'Thông tin đơn hàng đầy đủ', passed: true),
          _CheckRow(text: 'Giao cho lễ tân/bảo vệ', passed: receivedByGuard),
          _CheckRow(text: 'Kiện hàng còn niêm phong', passed: sealedPackage),
        ],
      ),
    );
  }
}

class _CheckRow extends StatelessWidget {
  final String text;
  final bool passed;

  const _CheckRow({required this.text, required this.passed});

  @override
  Widget build(BuildContext context) {
    final color = passed ? AppColors.success : AppColors.warning;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          Icon(
            passed ? Icons.check_circle_rounded : Icons.info_rounded,
            color: color,
            size: 18,
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              text,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: AppColors.inkSoft,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  final IconData icon;
  final String title;

  const _SectionHeader({required this.icon, required this.title});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Icon(icon, color: AppColors.courier),
        const SizedBox(width: 8),
        Text(
          title,
          style: Theme.of(
            context,
          ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w800),
        ),
      ],
    );
  }
}

class _InputField extends StatelessWidget {
  final String label;
  final String hint;
  final IconData icon;
  final int maxLines;

  const _InputField({
    required this.label,
    required this.hint,
    required this.icon,
    this.maxLines = 1,
  });

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      maxLines: maxLines,
      decoration: InputDecoration(
        labelText: label,
        hintText: hint,
        prefixIcon: Icon(icon, color: AppColors.courier),
        filled: true,
        fillColor: AppColors.surfaceStrong.withOpacity(0.65),
      ),
    );
  }
}
