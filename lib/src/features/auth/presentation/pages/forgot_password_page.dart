import 'package:flutter/material.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/widgets/auth_shell.dart';
import '../../data/repositories/auth_repository.dart';
import '../widgets/auth_theme.dart';
import '../widgets/auth_widgets.dart';

class ForgotPasswordPage extends StatefulWidget {
  const ForgotPasswordPage({super.key});

  @override
  State<ForgotPasswordPage> createState() => _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends State<ForgotPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _currentPasswordController = TextEditingController();
  final _newPasswordController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _obscureCurrent = true;
  bool _obscureNew = true;
  bool _obscureConfirm = true;
  bool _loading = false;
  bool _done = false;

  static const _style = AuthPageStyle.forgotPassword;

  @override
  void dispose() {
    _emailController.dispose();
    _currentPasswordController.dispose();
    _newPasswordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _reset() async {
    if (!_formKey.currentState!.validate()) return;
    FocusScope.of(context).unfocus();
    setState(() => _loading = true);
    try {
      await AuthRepository.instance.resetPassword(
        email: _emailController.text,
        currentPassword: _currentPasswordController.text,
        newPassword: _newPasswordController.text,
        confirmPassword: _confirmController.text,
      );
      if (!mounted) return;
      setState(() => _done = true);
    } catch (e) {
      if (!mounted) return;
      showAuthSnackBar(context, message: authErrorMessage(e), isError: true);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () => FocusScope.of(context).unfocus(),
      child: AuthShell(
        style: _style,
        footer: _done
            ? []
            : [
                AuthLinkRow(
                  prompt: 'Nhớ mật khẩu?',
                  action: 'Đăng nhập',
                  style: _style,
                  onTap: () => Navigator.of(context).pop(),
                ),
              ],
        children: [
          if (_done)
            Column(
              children: [
                Icon(Icons.check_circle_rounded, size: 64, color: Colors.green.shade500),
                const SizedBox(height: 12),
                Text(
                  'Mật khẩu đã được cập nhật thành công.',
                  textAlign: TextAlign.center,
                  style: AuthTheme.bodyStyle(),
                ),
                const SizedBox(height: 20),
                AuthPrimaryButton(
                  label: 'VỀ ĐĂNG NHẬP',
                  icon: Icons.login_rounded,
                  onPressed: () => Navigator.of(context).pushNamedAndRemoveUntil(
                    AppRoutes.login,
                    (r) => false,
                  ),
                  style: _style,
                ),
              ],
            )
          else
            Form(
              key: _formKey,
              child: Column(
                children: [
                  AuthTextField(
                    controller: _emailController,
                    label: 'Email đăng ký',
                    hint: 'example@gmail.com',
                    prefixIcon: Icons.mail_outline_rounded,
                    keyboardType: TextInputType.emailAddress,
                    style: _style,
                    validator: (v) {
                      if (v == null || !v.contains('@')) return 'Email không hợp lệ';
                      return null;
                    },
                  ),
                  const SizedBox(height: 16),
                  AuthTextField(
                    controller: _currentPasswordController,
                    label: 'Mật khẩu hiện tại',
                    prefixIcon: Icons.lock_outline_rounded,
                    obscureText: _obscureCurrent,
                    style: _style,
                    suffix: IconButton(
                      icon: Icon(_obscureCurrent ? Icons.visibility_off : Icons.visibility),
                      onPressed: () => setState(() => _obscureCurrent = !_obscureCurrent),
                    ),
                    validator: (v) => (v == null || v.isEmpty) ? 'Nhập mật khẩu hiện tại' : null,
                  ),
                  const SizedBox(height: 16),
                  AuthTextField(
                    controller: _newPasswordController,
                    label: 'Mật khẩu mới',
                    prefixIcon: Icons.vpn_key_outlined,
                    obscureText: _obscureNew,
                    style: _style,
                    suffix: IconButton(
                      icon: Icon(_obscureNew ? Icons.visibility_off : Icons.visibility),
                      onPressed: () => setState(() => _obscureNew = !_obscureNew),
                    ),
                    validator: (v) {
                      if (v == null || v.length < 6) return 'Tối thiểu 6 ký tự';
                      return null;
                    },
                  ),
                  const SizedBox(height: 16),
                  AuthTextField(
                    controller: _confirmController,
                    label: 'Xác nhận mật khẩu mới',
                    prefixIcon: Icons.verified_user_outlined,
                    obscureText: _obscureConfirm,
                    style: _style,
                    suffix: IconButton(
                      icon: Icon(_obscureConfirm ? Icons.visibility_off : Icons.visibility),
                      onPressed: () => setState(() => _obscureConfirm = !_obscureConfirm),
                    ),
                    validator: (v) {
                      if (v != _newPasswordController.text) return 'Mật khẩu không khớp';
                      return null;
                    },
                  ),
                  const SizedBox(height: 20),
                  AuthPrimaryButton(
                    label: 'CẬP NHẬT MẬT KHẨU',
                    icon: Icons.lock_reset_rounded,
                    loading: _loading,
                    onPressed: _reset,
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
