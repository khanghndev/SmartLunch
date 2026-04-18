import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/constants/app_colors.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/auth_shell.dart';
import '../../auth_types.dart';
import '../../data/repositories/auth_repository.dart';
import 'register_page.dart';

class RoleLoginConfig {
  final String title;
  final String subtitle;
  final String badge;
  final IconData icon;
  final Color accent;
  final List<Color> gradient;
  final String homeRoute;
  final bool supportsGoogle;
  final bool supportsRegister;
  final RegisterType? registerType;
  final List<String> allowedRoles;
  final bool allowWithoutRole;
  final AuthLoginMode loginMode;

  const RoleLoginConfig({
    required this.title,
    required this.subtitle,
    required this.badge,
    required this.icon,
    required this.accent,
    required this.gradient,
    required this.homeRoute,
    this.supportsGoogle = false,
    this.supportsRegister = true,
    this.registerType,
    this.allowedRoles = const [],
    this.allowWithoutRole = false,
    this.loginMode = AuthLoginMode.user,
  });

  factory RoleLoginConfig.customer() => const RoleLoginConfig(
    title: 'Đăng nhập',
    subtitle: 'Xem danh sách thực đơn.',
    badge: 'Khách hàng (người ăn)',
    icon: Icons.restaurant_menu_rounded,
    accent: AppColors.customer,
    gradient: [AppColors.customer, AppColors.customerAlt],
    homeRoute: AppRoutes.customerHome,
    supportsGoogle: true,
    registerType: RegisterType.customer,
    allowedRoles: ['User', 'Student'],
    allowWithoutRole: true,
    loginMode: AuthLoginMode.user,
  );

  factory RoleLoginConfig.courier() => const RoleLoginConfig(
    title: 'Đăng nhập',
    subtitle: 'Nhận đơn, giao hàng và cập nhật trạng thái giao hàng.',
    badge: 'Nhân viên giao hàng',
    icon: Icons.local_shipping_rounded,
    accent: AppColors.courier,
    gradient: [AppColors.courier, AppColors.courierAlt],
    homeRoute: AppRoutes.courierHome,
    supportsRegister: false,
    allowedRoles: ['Instructor', 'Courier', 'Shipper', 'Delivery'],
    loginMode: AuthLoginMode.auto,
  );

  factory RoleLoginConfig.org() => const RoleLoginConfig(
    title: 'Đăng nhập',
    subtitle:
        '• Tiến hành đặt cơm theo ngày (trước...\n• Tiến hành đặt cọc, thanh toán.\n• Đánh giá suất ăn.',
    badge: 'Đơn vị đặt suất ăn',
    icon: Icons.apartment_rounded,
    accent: AppColors.org,
    gradient: [AppColors.org, AppColors.orgAlt],
    homeRoute: AppRoutes.orgHome,
    supportsGoogle: true,
    registerType: RegisterType.org,
    allowedRoles: ['Admin', 'SuperAdmin', 'Organization', 'Org'],
    loginMode: AuthLoginMode.user,
  );
}

class RoleLoginPage extends StatefulWidget {
  final RoleLoginConfig config;

  const RoleLoginPage({super.key, required this.config});

  @override
  State<RoleLoginPage> createState() => _RoleLoginPageState();
}

class _RoleLoginPageState extends State<RoleLoginPage> {
  final AuthRepository _authRepository = AuthRepository.instance;
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  bool _obscurePassword = true;
  bool _isLoading = false;
  bool _isGoogleLoading = false;

  InputDecoration _fieldDecoration({
    required String label,
    required String hint,
    required IconData icon,
  }) {
    return InputDecoration(
      labelText: label,
      hintText: hint,
      labelStyle: TextStyle(
        color: AppColors.ink.withOpacity(0.7),
        fontWeight: FontWeight.w600,
      ),
      hintStyle: TextStyle(color: AppColors.ink.withOpacity(0.4)),
      prefixIcon: Container(
        margin: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: widget.config.accent.withOpacity(0.08),
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: widget.config.accent.withOpacity(0.16)),
        ),
        child: Icon(icon, color: widget.config.accent, size: 17),
      ),
      contentPadding: const EdgeInsets.symmetric(vertical: 14, horizontal: 12),
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide(color: AppColors.ink.withOpacity(0.12)),
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide(color: AppColors.ink.withOpacity(0.12)),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(12),
        borderSide: BorderSide(color: widget.config.accent, width: 1.6),
      ),
      filled: true,
      fillColor: const Color(0xFFF6F7FB),
    );
  }

  bool _isValidEmail(String value) {
    return RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$').hasMatch(value);
  }

  bool _isRoleAllowed(List<String> roles) {
    if (widget.config.allowedRoles.isEmpty) {
      return true;
    }

    if (roles.isEmpty && widget.config.allowWithoutRole) {
      return true;
    }

    final normalizedRoles = roles.map((e) => e.toLowerCase()).toSet();
    return widget.config.allowedRoles.any(
      (role) => normalizedRoles.contains(role.toLowerCase()),
    );
  }

  String _roleMismatchMessage(List<String> roles) {
    if (roles.isEmpty) {
      return 'Tài khoản chưa được gán quyền truy cập ${widget.config.badge.toLowerCase()}.';
    }
    return 'Tài khoản của bạn không thuộc nhóm ${widget.config.badge.toLowerCase()}.';
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

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _handleLogin() async {
    final formState = _formKey.currentState;
    if (formState == null || !formState.validate()) {
      return;
    }

    setState(() => _isLoading = true);

    try {
      final session = await _authRepository.login(
        identifier: _emailController.text.trim(),
        password: _passwordController.text.trim(),
        mode: widget.config.loginMode,
      );

      if (!_isRoleAllowed(session.roles)) {
        if (!mounted) return;
        _showSnack(_roleMismatchMessage(session.roles));
        return;
      }

      if (!mounted) return;
      Navigator.of(context).pushReplacementNamed(widget.config.homeRoute);
    } on ApiException catch (e) {
      if (!mounted) return;
      _showSnack(e.message);
    } catch (_) {
      if (!mounted) return;
      _showSnack('Không thể đăng nhập. Vui lòng thử lại.');
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _handleGoogleLogin() async {
    if (!widget.config.supportsGoogle || _isGoogleLoading) return;
    _showSnack('Chưa cấu hình đăng nhập Google/Firebase cho ứng dụng mobile.');
  }

  @override
  Widget build(BuildContext context) {
    return AuthShell(
      title: widget.config.title,
      subtitle: widget.config.subtitle,
      badge: widget.config.badge,
      icon: widget.config.icon,
      accent: widget.config.accent,
      gradient: widget.config.gradient,
      // mascotAsset: 'assets/images/linh_vat.png',
      mascotSize: 100,
      children: [
        Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Email/Phone field
              TextFormField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                textInputAction: TextInputAction.next,
                decoration: _fieldDecoration(
                  label: 'Email',
                  hint: 'Nhập email',
                  icon: Icons.alternate_email_rounded,
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Vui lòng nhập email';
                  }
                  if (!_isValidEmail(value.trim())) {
                    return 'Email không hợp lệ';
                  }
                  return null;
                },
              ),

              const SizedBox(height: 12),

              // Password field
              TextFormField(
                controller: _passwordController,
                obscureText: _obscurePassword,
                textInputAction: TextInputAction.done,
                onFieldSubmitted: (_) => _handleLogin(),
                decoration: _fieldDecoration(
                  label: 'Mật khẩu',
                  hint: 'Nhập mật khẩu',
                  icon: Icons.lock_outline_rounded,
                ).copyWith(
                  suffixIcon: IconButton(
                    icon: Icon(
                      _obscurePassword
                          ? Icons.visibility_outlined
                          : Icons.visibility_off_outlined,
                      color: AppColors.ink.withOpacity(0.5),
                    ),
                    onPressed: () {
                      setState(() => _obscurePassword = !_obscurePassword);
                    },
                  ),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Vui lòng nhập mật khẩu';
                  }
                  if (value.length < 6) {
                    return 'Mật khẩu phải có ít nhất 6 ký tự';
                  }
                  return null;
                },
              ),

              const SizedBox(height: 6),

              // Forgot password link
              Align(
                alignment: Alignment.centerRight,
                child: TextButton(
                  onPressed:
                      () => Navigator.of(
                        context,
                      ).pushNamed(AppRoutes.forgotPassword),
                  style: TextButton.styleFrom(
                    foregroundColor: widget.config.accent,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 4,
                    ),
                  ),
                  child: const Text(
                    'Quên mật khẩu?',
                    style: TextStyle(fontWeight: FontWeight.w600),
                  ),
                ),
              ),

              const SizedBox(height: 18),

              // Login button
              Container(
                height: 48,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [
                      widget.config.accent,
                      widget.config.gradient.last.withOpacity(0.9),
                    ],
                  ),
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: [
                    BoxShadow(
                      color: widget.config.accent.withOpacity(0.25),
                      blurRadius: 12,
                      offset: const Offset(0, 6),
                    ),
                  ],
                ),
                child: ElevatedButton(
                  onPressed:
                      _isLoading || _isGoogleLoading ? null : _handleLogin,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.transparent,
                    shadowColor: Colors.transparent,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child:
                      _isLoading
                          ? SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              valueColor: AlwaysStoppedAnimation<Color>(
                                Colors.white,
                              ),
                              strokeWidth: 2,
                            ),
                          )
                          : Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Text(
                                'Đăng nhập',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontSize: 15.5,
                                  fontWeight: FontWeight.w700,
                                  letterSpacing: 0.2,
                                ),
                              ),
                              const SizedBox(width: 6),
                              const Icon(
                                Icons.arrow_forward_rounded,
                                color: Colors.white,
                                size: 16,
                              ),
                            ],
                          ),
                ),
              ),

              if (widget.config.supportsGoogle) ...[
                const SizedBox(height: 10),
                OutlinedButton(
                  onPressed:
                      (_isLoading || _isGoogleLoading)
                          ? null
                          : _handleGoogleLogin,
                  style: OutlinedButton.styleFrom(
                    backgroundColor: Colors.white,
                    side: BorderSide(
                      color: AppColors.ink.withOpacity(0.12),
                      width: 1,
                    ),
                    padding: const EdgeInsets.symmetric(
                      vertical: 12,
                      horizontal: 12,
                    ),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Container(
                        width: 20,
                        height: 20,
                        decoration: BoxDecoration(
                          color: const Color(0xFFF1F3F7),
                          shape: BoxShape.circle,
                          border: Border.all(
                            color: AppColors.ink.withOpacity(0.08),
                          ),
                        ),
                        child: const Center(
                          child: Text(
                            'G',
                            style: TextStyle(
                              color: Color(0xFF4285F4),
                              fontWeight: FontWeight.w800,
                              fontSize: 13,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          _isGoogleLoading
                              ? 'Đang kết nối Google...'
                              : 'Đăng nhập với Google',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color: AppColors.ink.withOpacity(0.85),
                            fontSize: 14.5,
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],

              const SizedBox(height: 12),

              if (widget.config.supportsRegister) ...[
                // Divider
                Row(
                  children: [
                    Expanded(
                      child: Divider(
                        color: AppColors.ink.withOpacity(0.08),
                        thickness: 1,
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 12),
                      child: Text(
                        'hoặc',
                        style: TextStyle(
                          color: AppColors.ink.withOpacity(0.5),
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                    Expanded(
                      child: Divider(
                        color: AppColors.ink.withOpacity(0.08),
                        thickness: 1,
                      ),
                    ),
                  ],
                ),

                const SizedBox(height: 12),

                // Register button
                OutlinedButton(
                  onPressed:
                      () => Navigator.of(context).pushNamed(
                        AppRoutes.register,
                        arguments:
                            widget.config.registerType ?? RegisterType.customer,
                      ),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: widget.config.accent,
                    side: BorderSide(
                      color: widget.config.accent.withOpacity(0.6),
                      width: 1,
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child: const Text(
                    'Tạo tài khoản mới',
                    style: TextStyle(
                      fontSize: 15.5,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
              ],
            ],
          ),
        ),
      ],
      footer: [
        Text(
          'Bằng việc tiếp tục, bạn đồng ý với các điều khoản dịch vụ của SmartLunch.',
          textAlign: TextAlign.center,
          style: TextStyle(
            color: AppColors.ink.withOpacity(0.65),
            fontSize: 12,
            height: 1.4,
          ),
        ),
      ],
    );
  }
}
