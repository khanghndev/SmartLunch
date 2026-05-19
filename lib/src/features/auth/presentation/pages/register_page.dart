import 'package:flutter/material.dart';
import 'package:flutter/gestures.dart';

import '../../../../app/app_routes.dart';
import '../../../../core/widgets/auth_shell.dart';
import '../../data/repositories/auth_repository.dart';
import '../widgets/auth_theme.dart';
import '../widgets/auth_widgets.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();
  bool _obscurePassword = true;
  bool _obscureConfirm = true;
  bool _loading = false;
  bool _agreed = false;

  static const _style = AuthPageStyle.register;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    if (!_formKey.currentState!.validate()) return;
    if (!_agreed) {
      showAuthSnackBar(context, message: 'Vui lòng đồng ý điều khoản sử dụng', isError: true);
      return;
    }

    FocusScope.of(context).unfocus();
    setState(() => _loading = true);
    try {
      await AuthRepository.instance.register(
        email: _emailController.text,
        password: _passwordController.text,
        confirmPassword: _confirmController.text,
      );
      if (!mounted) return;
      showAuthSnackBar(context, message: 'Đăng ký thành công! Vui lòng đăng nhập.');
      Navigator.of(context).pushReplacementNamed(AppRoutes.login);
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
        footer: [
          AuthLinkRow(
            prompt: 'Đã có tài khoản?',
            action: 'Đăng nhập ngay',
            style: _style,
            onTap: () => Navigator.of(context).pop(),
          ),
        ],
        children: [
          Form(
            key: _formKey,
            child: Column(
              children: [
                AuthTextField(
                  controller: _emailController,
                  label: 'Email',
                  hint: 'example@gmail.com',
                  prefixIcon: Icons.mail_outline_rounded,
                  keyboardType: TextInputType.emailAddress,
                  style: _style,
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) return 'Nhập email';
                    if (!v.contains('@')) return 'Email không hợp lệ';
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                AuthTextField(
                  controller: _passwordController,
                  label: 'Mật khẩu',
                  hint: '••••••••',
                  prefixIcon: Icons.lock_outline_rounded,
                  obscureText: _obscurePassword,
                  style: _style,
                  suffix: IconButton(
                    icon: Icon(_obscurePassword ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _obscurePassword = !_obscurePassword),
                  ),
                  validator: (v) {
                    if (v == null || v.length < 6) return 'Tối thiểu 6 ký tự';
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                AuthTextField(
                  controller: _confirmController,
                  label: 'Xác nhận mật khẩu',
                  hint: '••••••••',
                  prefixIcon: Icons.verified_user_outlined,
                  obscureText: _obscureConfirm,
                  style: _style,
                  suffix: IconButton(
                    icon: Icon(_obscureConfirm ? Icons.visibility_off : Icons.visibility),
                    onPressed: () => setState(() => _obscureConfirm = !_obscureConfirm),
                  ),
                  validator: (v) {
                    if (v != _passwordController.text) return 'Mật khẩu không khớp';
                    return null;
                  },
                ),
                const SizedBox(height: 14),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    SizedBox(
                      width: 22,
                      height: 22,
                      child: Checkbox(
                        value: _agreed,
                        onChanged: (v) => setState(() => _agreed = v ?? false),
                        activeColor: AuthTheme.orange500,
                        side: const BorderSide(color: AuthTheme.gray200, width: 1.5),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(4),
                        ),
                      ),
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: RichText(
                        text: TextSpan(
                          style: AuthTheme.bodyStyle(color: AuthTheme.gray500).copyWith(fontSize: 13),
                          children: [
                            const TextSpan(text: 'Tôi đồng ý với các '),
                            TextSpan(
                              text: 'Điều khoản dịch vụ',
                              style: TextStyle(
                                color: AuthTheme.orange600,
                                fontWeight: FontWeight.w700,
                              ),
                              recognizer: TapGestureRecognizer()..onTap = () {},
                            ),
                            const TextSpan(text: ' và '),
                            TextSpan(
                              text: 'Chính sách bảo mật',
                              style: TextStyle(
                                color: AuthTheme.orange600,
                                fontWeight: FontWeight.w700,
                              ),
                              recognizer: TapGestureRecognizer()..onTap = () {},
                            ),
                            const TextSpan(text: ' của HuitMeal.'),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                AuthPrimaryButton(
                  label: 'ĐĂNG KÝ NGAY',
                  icon: Icons.person_add_rounded,
                  loading: _loading,
                  onPressed: _register,
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
