import 'package:flutter/material.dart';

import '../../../../core/constants/app_colors.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/auth_shell.dart';
import '../../data/repositories/auth_repository.dart';

enum RegisterType { customer, org }

class RegisterPage extends StatefulWidget {
  final RegisterType type;

  const RegisterPage({super.key, this.type = RegisterType.customer});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final AuthRepository _authRepository = AuthRepository.instance;
  final _nameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  bool _obscurePassword = true;
  bool _obscureConfirm = true;
  bool _isLoading = false;

  @override
  void dispose() {
    _nameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  InputDecoration _fieldDecoration({
    required String label,
    required String hint,
    required IconData icon,
    required Color accent,
    Widget? suffix,
  }) {
    return InputDecoration(
      labelText: label,
      hintText: hint,
      labelStyle: TextStyle(
        color: AppColors.ink.withOpacity(0.75),
        fontWeight: FontWeight.w700,
      ),
      hintStyle: TextStyle(color: AppColors.ink.withOpacity(0.42)),
      prefixIcon: Container(
        margin: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: accent.withOpacity(0.08),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Icon(icon, color: accent, size: 18),
      ),
      suffixIcon: suffix,
      filled: true,
      fillColor: const Color(0xFFF7F8FB),
      contentPadding: const EdgeInsets.symmetric(vertical: 14, horizontal: 14),
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: BorderSide(color: accent.withOpacity(0.1)),
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: BorderSide(color: accent.withOpacity(0.08)),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: BorderSide(color: accent, width: 1.6),
      ),
    );
  }

  Widget _buildInfoCard({
    required Color accent,
    required String title,
    required String body,
  }) {
    return Container(
      margin: const EdgeInsets.only(bottom: 14),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: accent.withOpacity(0.06),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: accent.withOpacity(0.1)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 10,
                  offset: const Offset(0, 4),
                ),
              ],
            ),
            child: Icon(Icons.flash_on_rounded, color: accent),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: Theme.of(
                    context,
                  ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w800),
                ),
                const SizedBox(height: 4),
                Text(
                  body,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppColors.ink.withOpacity(0.7),
                    height: 1.35,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLaterSteps({
    required List<String> steps,
    required List<Color> gradient,
    required Color accent,
    required String headline,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: gradient,
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(18),
        boxShadow: [
          BoxShadow(
            color: accent.withOpacity(0.28),
            blurRadius: 18,
            offset: const Offset(0, 10),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.route_rounded, color: Colors.white),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  headline,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.w800,
                    fontSize: 15.5,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          ...steps.asMap().entries.map(
            (entry) => Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    width: 26,
                    height: 26,
                    decoration: BoxDecoration(
                      color: Colors.white.withOpacity(0.16),
                      borderRadius: BorderRadius.circular(9),
                    ),
                    child: Center(
                      child: Text(
                        '${entry.key + 1}',
                        style: const TextStyle(
                          color: Colors.white,
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Text(
                      entry.value,
                      style: const TextStyle(color: Colors.white, height: 1.35),
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

  bool _isValidEmail(String value) {
    return RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$').hasMatch(value);
  }

  void _showSnack(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
      ),
    );
  }

  Future<void> _handleRegister() async {
    final formState = _formKey.currentState;
    if (formState == null || !formState.validate()) {
      return;
    }

    setState(() => _isLoading = true);
    try {
      await _authRepository.register(
        email: _emailController.text.trim(),
        password: _passwordController.text.trim(),
        confirmPassword: _confirmPasswordController.text.trim(),
      );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: const Text('Đăng ký thành công. Vui lòng đăng nhập.'),
          backgroundColor: Colors.green.shade600,
          behavior: SnackBarBehavior.floating,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
        ),
      );
      Navigator.of(context).maybePop();
    } on ApiException catch (e) {
      if (!mounted) return;
      _showSnack(e.message);
    } catch (_) {
      if (!mounted) return;
      _showSnack('Không thể đăng ký. Vui lòng thử lại.');
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isCustomer = widget.type == RegisterType.customer;
    final accent = isCustomer ? AppColors.customer : AppColors.org;
    final gradient =
        isCustomer
            ? [AppColors.customer, AppColors.customerAlt]
            : [AppColors.org, AppColors.orgAlt];
    final title =
        isCustomer
            ? 'Đăng ký khách hàng cá nhân'
            : 'Đăng ký khách hàng doanh nghiệp';
    final subtitle =
        isCustomer
            ? 'Tạo tài khoản để đặt món và theo dõi đơn nhanh chóng.'
            : 'Điền 3 trường cơ bản để vào hệ thống. Hồ sơ chi tiết bổ sung sau.';
    final badge = isCustomer ? 'Tài khoản cá nhân' : 'Đăng ký nhanh';
    final icon =
        isCustomer ? Icons.restaurant_menu_rounded : Icons.apartment_rounded;
    final infoTitle =
        isCustomer ? 'Đăng ký trong 1 phút' : 'Đăng ký siêu nhanh';
    final infoBody =
        isCustomer
            ? 'Nhập tên, email hoặc số điện thoại để bắt đầu đặt bữa ngay.'
            : 'Chỉ cần tên, email và mật khẩu. MST, địa chỉ giao, giấy tờ sẽ bổ sung sau khi đăng nhập.';
    final steps =
        isCustomer
            ? [
              'Xác minh email hoặc số điện thoại.',
              'Thêm địa chỉ giao nhận và lưu người nhận.',
              'Lưu phương thức thanh toán an toàn.',
              'Đặt món và theo dõi trạng thái đơn hàng.',
            ]
            : [
              'Hoàn tất hồ sơ doanh nghiệp (MST, địa chỉ giao).',
              'Upload giấy phép kinh doanh nếu cần.',
              'Chờ admin duyệt và kích hoạt.',
              'Bắt đầu xem thực đơn, đặt suất và thanh toán.',
            ];
    final headline =
        isCustomer ? 'Sau khi đăng ký' : 'Bổ sung sau khi đăng nhập';
    final nameLabel = isCustomer ? 'Họ và tên' : 'Tên doanh nghiệp / đơn vị';
    final nameHint =
        isCustomer ? 'Ví dụ: Nguyễn Văn A' : 'Ví dụ: Công ty TNHH ABC';
    final emailLabel = 'Email';
    final emailHint = 'Email nhận OTP và thông báo';
    final minPasswordLength = isCustomer ? 6 : 8;
    final passwordHint = 'Tối thiểu $minPasswordLength ký tự';
    final confirmHint = isCustomer ? 'Nhập lại mật khẩu' : 'Nhập lại mật khẩu';
    final secondaryCta =
        isCustomer ? 'Hoàn tất sau' : 'Bổ sung hồ sơ sau khi đăng nhập';

    return AuthShell(
      title: title,
      subtitle: subtitle,
      badge: badge,
      icon: icon,
      accent: accent,
      gradient: gradient,
      mascotAsset: 'assets/images/linh_vat.png',
      mascotSize: 92,
      children: [
        _buildInfoCard(accent: accent, title: infoTitle, body: infoBody),
        Form(
          key: _formKey,
          child: Container(
            margin: const EdgeInsets.only(bottom: 16),
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(18),
              border: Border.all(color: accent.withOpacity(0.08)),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 16,
                  offset: const Offset(0, 10),
                ),
              ],
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: AppColors.org.withOpacity(0.12),
                        borderRadius: BorderRadius.circular(14),
                      ),
                      child: Icon(Icons.badge_rounded, color: accent),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Thông tin bắt buộc',
                            style: Theme.of(
                              context,
                            ).textTheme.titleMedium?.copyWith(
                              fontWeight: FontWeight.w800,
                              letterSpacing: -0.2,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            isCustomer
                                ? 'Điền thông tin cơ bản để bắt đầu đặt món.'
                                : 'Chỉ 3 trường để khởi tạo tài khoản doanh nghiệp.',
                            style: Theme.of(
                              context,
                            ).textTheme.bodySmall?.copyWith(
                              color: AppColors.ink.withOpacity(0.65),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 14),
                TextFormField(
                  controller: _nameController,
                  decoration: _fieldDecoration(
                    label: nameLabel,
                    hint: nameHint,
                    icon:
                        isCustomer
                            ? Icons.person_rounded
                            : Icons.apartment_rounded,
                    accent: accent,
                  ),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Vui lòng nhập ${nameLabel.toLowerCase()}';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _emailController,
                  keyboardType: TextInputType.emailAddress,
                  decoration: _fieldDecoration(
                    label: emailLabel,
                    hint: emailHint,
                    icon: Icons.email_rounded,
                    accent: accent,
                  ),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Vui lòng nhập email';
                    }
                    if (!_isValidEmail(value.trim())) {
                      return 'Email không hợp lệ';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _passwordController,
                  obscureText: _obscurePassword,
                  decoration: _fieldDecoration(
                    label: 'Mật khẩu',
                    hint: passwordHint,
                    icon: Icons.lock_outline_rounded,
                    accent: accent,
                    suffix: IconButton(
                      icon: Icon(
                        _obscurePassword
                            ? Icons.visibility_outlined
                            : Icons.visibility_off_outlined,
                        color: AppColors.ink.withOpacity(0.5),
                      ),
                      onPressed:
                          () => setState(
                            () => _obscurePassword = !_obscurePassword,
                          ),
                    ),
                  ),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Vui lòng nhập mật khẩu';
                    }
                    if (value.length < minPasswordLength) {
                      return 'Mật khẩu phải có ít nhất $minPasswordLength ký tự';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _confirmPasswordController,
                  obscureText: _obscureConfirm,
                  decoration: _fieldDecoration(
                    label: 'Xác nhận mật khẩu',
                    hint: confirmHint,
                    icon: Icons.lock_reset_rounded,
                    accent: accent,
                    suffix: IconButton(
                      icon: Icon(
                        _obscureConfirm
                            ? Icons.visibility_outlined
                            : Icons.visibility_off_outlined,
                        color: AppColors.ink.withOpacity(0.5),
                      ),
                      onPressed:
                          () =>
                              setState(() => _obscureConfirm = !_obscureConfirm),
                    ),
                  ),
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Vui lòng xác nhận mật khẩu';
                    }
                    if (value.trim() != _passwordController.text.trim()) {
                      return 'Mật khẩu xác nhận không khớp';
                    }
                    return null;
                  },
                ),
              ],
            ),
          ),
        ),
        _buildLaterSteps(
          steps: steps,
          gradient: gradient,
          accent: accent,
          headline: headline,
        ),
        const SizedBox(height: 16),
        Container(
          height: 54,
          decoration: BoxDecoration(
            gradient: LinearGradient(colors: gradient),
            borderRadius: BorderRadius.circular(16),
            boxShadow: [
              BoxShadow(
                color: accent.withOpacity(0.4),
                blurRadius: 20,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: ElevatedButton(
            onPressed: _isLoading ? null : _handleRegister,
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.transparent,
              shadowColor: Colors.transparent,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(16),
              ),
            ),
            child:
                _isLoading
                    ? const SizedBox(
                      width: 22,
                      height: 22,
                      child: CircularProgressIndicator(
                        valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                        strokeWidth: 2,
                      ),
                    )
                    : Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(
                          Icons.rocket_launch_rounded,
                          color: Colors.white,
                        ),
                        const SizedBox(width: 8),
                        const Text(
                          'Tạo tài khoản',
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 16,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                      ],
                    ),
          ),
        ),
        const SizedBox(height: 10),
        OutlinedButton.icon(
          onPressed: () => Navigator.of(context).maybePop(),
          style: OutlinedButton.styleFrom(
            foregroundColor: accent,
            side: BorderSide(color: accent.withOpacity(0.6)),
            padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(14),
            ),
          ),
          icon: const Icon(Icons.check_circle_outline_rounded),
          label: Text(
            secondaryCta,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
      ],
      footer: [
        Text(
          isCustomer
              ? 'Thông tin chỉ dùng để tạo tài khoản đặt món. Bạn có thể cập nhật địa chỉ và thanh toán bất cứ lúc nào.'
              : 'Thông tin này chỉ dùng để kích hoạt tài khoản SmartLunch. Bạn có thể cập nhật và bổ sung hồ sơ doanh nghiệp bất cứ lúc nào.',
          textAlign: TextAlign.center,
          style: TextStyle(
            color: AppColors.ink.withOpacity(0.65),
            fontSize: 12.5,
            height: 1.4,
          ),
        ),
      ],
    );
  }
}
