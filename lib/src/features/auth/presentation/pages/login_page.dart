import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/widgets/auth_shell.dart';
import '../../data/repositories/auth_repository.dart';
import '../widgets/auth_theme.dart';
import '../widgets/auth_widgets.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  bool _isLoading = false;
  bool _obscurePassword = true;
  bool _rememberMe = false;

  static const _style = AuthPageStyle.login;

  Future<void> _handleLogin() async {
    if (!_formKey.currentState!.validate()) return;

    FocusScope.of(context).unfocus();
    setState(() => _isLoading = true);
    try {
      final session = await AuthRepository.instance.login(
        identifier: _emailController.text,
        password: _passwordController.text,
      );

      if (!mounted) return;

      final roles = session.roles.map((e) => e.toLowerCase()).toList();
      String nextRoute = AppRoutes.customerHome;

      if (roles.contains('admin') || roles.contains('manager')) {
        nextRoute = AppRoutes.managerHome;
      } else if (roles.contains('shipper') || roles.contains('delivery')) {
        nextRoute = AppRoutes.shipperHome;
      } else if (roles.contains('company') ||
          roles.contains('organization') ||
          roles.contains('org')) {
        nextRoute = AppRoutes.orgHome;
      }

      Navigator.of(context).pushReplacementNamed(nextRoute);
    } catch (e) {
      if (!mounted) return;
      showAuthSnackBar(context, message: authErrorMessage(e), isError: true);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => FocusScope.of(context).unfocus(),
      child: AuthShell(
        showBackButton: false,
        style: _style,
        footer: [
          const AuthDivider(),
          const SizedBox(height: 12),
          AuthOutlinedButton(
            label: 'Xem thực đơn không cần đăng nhập',
            icon: Icons.menu_book_rounded,
            style: _style,
            onPressed: () {
              Navigator.of(context).pushReplacementNamed(AppRoutes.customerHome);
            },
          ),
          const SizedBox(height: 6),
          AuthLinkRow(
            prompt: 'Chưa có tài khoản?',
            action: 'Đăng ký ngay',
            style: _style,
            onTap: () => Navigator.of(context).pushNamed(AppRoutes.register),
          ),
        ],
        children: [
          Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                AuthTextField(
                  controller: _emailController,
                  label: 'Email / Số điện thoại',
                  hint: 'Nhập email hoặc số điện thoại...',
                  prefixIcon: Icons.mail_outline_rounded,
                  keyboardType: TextInputType.emailAddress,
                  style: _style,
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Vui lòng nhập email';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                AuthTextField(
                  controller: _passwordController,
                  label: 'Mật khẩu',
                  hint: 'Nhập mật khẩu...',
                  prefixIcon: Icons.lock_outline_rounded,
                  obscureText: _obscurePassword,
                  style: _style,
                  suffix: IconButton(
                    icon: Icon(
                      _obscurePassword ? Icons.visibility_off_outlined : Icons.visibility_outlined,
                      color: AuthTheme.gray400,
                    ),
                    onPressed: () => setState(() => _obscurePassword = !_obscurePassword),
                  ),
                  validator: (v) {
                    if (v == null || v.isEmpty) return 'Vui lòng nhập mật khẩu';
                    if (v.length < 6) return 'Mật khẩu tối thiểu 6 ký tự';
                    return null;
                  },
                ),
                const SizedBox(height: 8),
                AuthRememberRow(
                  value: _rememberMe,
                  onChanged: (v) => setState(() => _rememberMe = v),
                  onForgot: () => Navigator.of(context).pushNamed(AppRoutes.forgotPassword),
                  style: _style,
                ),
                const SizedBox(height: 16),
                AuthPrimaryButton(
                  label: 'Đăng nhập',
                  icon: Icons.login_rounded,
                  loading: _isLoading,
                  onPressed: _handleLogin,
                  style: _style,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
