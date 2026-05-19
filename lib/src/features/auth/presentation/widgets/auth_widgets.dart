import 'package:flutter/material.dart';

import '../../../../core/network/api_exception.dart';
import 'auth_theme.dart';

String authErrorMessage(Object e) {
  if (e is ApiException) return e.message;
  final s = e.toString();
  if (s.startsWith('Exception: ')) return s.substring(11);
  return s;
}

/// SnackBar nổi phía trên bàn phím / navigation bar.
void showAuthSnackBar(
  BuildContext context, {
  required String message,
  bool isError = false,
}) {
  final bottom = MediaQuery.viewInsetsOf(context).bottom;
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      behavior: SnackBarBehavior.floating,
      margin: EdgeInsets.fromLTRB(16, 0, 16, bottom > 0 ? bottom + 12 : 16),
      backgroundColor: isError ? const Color(0xFFFEF2F2) : const Color(0xFFF0FDF4),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(
          color: isError ? const Color(0xFFFECACA) : const Color(0xFFBBF7D0),
        ),
      ),
    ),
  );
}

/// Input khớp `.input-field` trên web: nền #fafafa, viền 1.5px, bo 12px, focus cam/indigo.
class AuthTextField extends StatelessWidget {
  final TextEditingController controller;
  final String label;
  final String? hint;
  final IconData prefixIcon;
  final bool obscureText;
  final TextInputType keyboardType;
  final String? Function(String?)? validator;
  final Widget? suffix;
  final AuthPageStyle style;

  const AuthTextField({
    super.key,
    required this.controller,
    required this.label,
    this.hint,
    required this.prefixIcon,
    required this.style,
    this.obscureText = false,
    this.keyboardType = TextInputType.text,
    this.validator,
    this.suffix,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: AuthTheme.labelStyle()),
        const SizedBox(height: 6),
        TextFormField(
          controller: controller,
          obscureText: obscureText,
          keyboardType: keyboardType,
          validator: validator,
          style: AuthTheme.displayFont.copyWith(
            fontSize: 14,
            color: AuthTheme.gray900,
            fontWeight: FontWeight.w500,
          ),
          decoration: InputDecoration(
            hintText: hint,
            hintStyle: AuthTheme.displayFont.copyWith(
              fontSize: 14,
              color: AuthTheme.gray400,
            ),
            prefixIcon: Icon(prefixIcon, color: AuthTheme.gray400, size: 20),
            suffixIcon: suffix,
            filled: true,
            fillColor: AuthTheme.inputFill,
            contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: AuthTheme.gray200, width: 1.5),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: AuthTheme.gray200, width: 1.5),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: style.focusColor, width: 1.5),
            ),
            focusedErrorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.redAccent, width: 1.5),
            ),
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.redAccent, width: 1.5),
            ),
          ),
        ),
      ],
    );
  }
}

/// Nút submit khớp web: `rounded-xl font-black`, nền solid, shadow.
class AuthPrimaryButton extends StatelessWidget {
  final String label;
  final IconData? icon;
  final VoidCallback? onPressed;
  final bool loading;
  final AuthPageStyle style;

  const AuthPrimaryButton({
    super.key,
    required this.label,
    required this.style,
    this.icon,
    this.onPressed,
    this.loading = false,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 52,
      width: double.infinity,
      child: ElevatedButton(
        onPressed: loading ? null : onPressed,
        style: ElevatedButton.styleFrom(
          backgroundColor: style.buttonColor,
          foregroundColor: Colors.white,
          disabledBackgroundColor: style.buttonColor.withValues(alpha: 0.6),
          elevation: 0,
          shadowColor: style.buttonColor.withValues(alpha: 0.35),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        ).copyWith(
          elevation: WidgetStateProperty.resolveWith((states) {
            if (states.contains(WidgetState.pressed)) return 2;
            return 8;
          }),
        ),
        child: loading
            ? const SizedBox(
                width: 22,
                height: 22,
                child: CircularProgressIndicator(strokeWidth: 2.5, color: Colors.white),
              )
            : Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  if (icon != null) ...[
                    Icon(icon, size: 18),
                    const SizedBox(width: 8),
                  ],
                  Text(
                    label,
                    style: AuthTheme.displayFont.copyWith(
                      fontWeight: FontWeight.w900,
                      fontSize: 14,
                      letterSpacing: 0.6,
                    ),
                  ),
                ],
              ),
      ),
    );
  }
}

class AuthOutlinedButton extends StatelessWidget {
  final String label;
  final IconData icon;
  final VoidCallback onPressed;
  final AuthPageStyle style;

  const AuthOutlinedButton({
    super.key,
    required this.label,
    required this.icon,
    required this.onPressed,
    required this.style,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 52,
      width: double.infinity,
      child: OutlinedButton.icon(
        onPressed: onPressed,
        icon: Icon(icon, color: style.accent, size: 20),
        label: Text(
          label,
          style: AuthTheme.displayFont.copyWith(
            color: style.accent,
            fontWeight: FontWeight.w700,
            fontSize: 14,
          ),
        ),
        style: OutlinedButton.styleFrom(
          side: BorderSide(color: AuthTheme.gray200, width: 1.5),
          backgroundColor: Colors.white,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        ),
      ),
    );
  }
}

class AuthDivider extends StatelessWidget {
  final String label;

  const AuthDivider({super.key, this.label = 'HOẶC'});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        const Expanded(child: Divider(color: AuthTheme.gray100, thickness: 1.5)),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 14),
          child: Text(
            label,
            style: AuthTheme.displayFont.copyWith(
              color: AuthTheme.gray400,
              fontWeight: FontWeight.w700,
              fontSize: 11,
            ),
          ),
        ),
        const Expanded(child: Divider(color: AuthTheme.gray100, thickness: 1.5)),
      ],
    );
  }
}

class AuthLinkRow extends StatelessWidget {
  final String prompt;
  final String action;
  final VoidCallback onTap;
  final AuthPageStyle style;

  const AuthLinkRow({
    super.key,
    required this.prompt,
    required this.action,
    required this.onTap,
    required this.style,
  });

  @override
  Widget build(BuildContext context) {
    return Wrap(
      alignment: WrapAlignment.center,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        Text(prompt, style: AuthTheme.bodyStyle()),
        TextButton(
          onPressed: onTap,
          child: Text(
            action,
            style: AuthTheme.displayFont.copyWith(
              color: style.linkColor,
              fontWeight: FontWeight.w900,
            ),
          ),
        ),
      ],
    );
  }
}

class AuthRoleChip extends StatelessWidget {
  final String label;
  final IconData icon;
  final Color color;

  const AuthRoleChip({
    super.key,
    required this.label,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: color.withValues(alpha: 0.25)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: color),
          const SizedBox(width: 5),
          Text(
            label,
            style: AuthTheme.displayFont.copyWith(
              color: color,
              fontSize: 11,
              fontWeight: FontWeight.w700,
            ),
          ),
        ],
      ),
    );
  }
}

class AuthRememberRow extends StatelessWidget {
  final bool value;
  final ValueChanged<bool> onChanged;
  final VoidCallback onForgot;
  final AuthPageStyle style;

  const AuthRememberRow({
    super.key,
    required this.value,
    required this.onChanged,
    required this.onForgot,
    required this.style,
  });

  @override
  Widget build(BuildContext context) {
    final narrow = MediaQuery.sizeOf(context).width < 360;

    final remember = Row(
      children: [
        SizedBox(
          width: 22,
          height: 22,
          child: Checkbox(
            value: value,
            onChanged: (v) => onChanged(v ?? false),
            activeColor: style.focusColor,
            side: const BorderSide(color: AuthTheme.gray200, width: 1.5),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(4)),
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: GestureDetector(
            onTap: () => onChanged(!value),
            child: Text(
              'Ghi nhớ đăng nhập',
              style: AuthTheme.bodyStyle(color: AuthTheme.gray500),
            ),
          ),
        ),
      ],
    );

    final forgot = Align(
      alignment: narrow ? Alignment.centerRight : Alignment.centerRight,
      child: TextButton(
        onPressed: onForgot,
        style: TextButton.styleFrom(
          padding: EdgeInsets.zero,
          minimumSize: Size.zero,
          tapTargetSize: MaterialTapTargetSize.shrinkWrap,
        ),
        child: Text(
          'Quên mật khẩu?',
          style: AuthTheme.displayFont.copyWith(
            color: style.linkColor,
            fontWeight: FontWeight.w700,
            fontSize: 13,
          ),
        ),
      ),
    );

    if (narrow) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [remember, forgot],
      );
    }

    return Row(
      children: [
        SizedBox(
          width: 22,
          height: 22,
          child: Checkbox(
            value: value,
            onChanged: (v) => onChanged(v ?? false),
            activeColor: style.focusColor,
            side: const BorderSide(color: AuthTheme.gray200, width: 1.5),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(4)),
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: GestureDetector(
            onTap: () => onChanged(!value),
            child: Text(
              'Ghi nhớ đăng nhập',
              style: AuthTheme.bodyStyle(color: AuthTheme.gray500),
            ),
          ),
        ),
        TextButton(
          onPressed: onForgot,
          style: TextButton.styleFrom(
            padding: const EdgeInsets.symmetric(horizontal: 4),
            minimumSize: Size.zero,
            tapTargetSize: MaterialTapTargetSize.shrinkWrap,
          ),
          child: Text(
            'Quên mật khẩu?',
            style: AuthTheme.displayFont.copyWith(
              color: style.linkColor,
              fontWeight: FontWeight.w700,
              fontSize: 13,
            ),
          ),
        ),
      ],
    );
  }
}

class AuthFeatureRow extends StatelessWidget {
  final IconData icon;
  final String text;

  const AuthFeatureRow({super.key, required this.icon, required this.text});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: Colors.white.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icon, color: AuthTheme.orange400, size: 20),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Text(
              text,
              style: AuthTheme.displayFont.copyWith(
                color: Colors.white,
                fontWeight: FontWeight.w700,
                fontSize: 14,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
