import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../features/auth/presentation/widgets/auth_theme.dart';
import '../../features/auth/presentation/widgets/auth_widgets.dart';
import 'staggered_reveal.dart';

/// Layout auth: hero + form card chồng nhau — gọn, chuyên nghiệp, hỗ trợ bàn phím.
class AuthShell extends StatefulWidget {
  final AuthPageStyle style;
  final List<Widget> children;
  final List<Widget> footer;
  final bool showBackButton;

  const AuthShell({
    super.key,
    required this.style,
    required this.children,
    this.footer = const [],
    this.showBackButton = true,
  });

  @override
  State<AuthShell> createState() => _AuthShellState();
}

class _AuthShellState extends State<AuthShell> {
  /// Chồng nhẹ — tránh đè sát stats hero (vùng chuyển tiếp hẹp).
  static const _panelOverlap = 10.0;

  static double _horizontalInset(BuildContext context) {
    final w = MediaQuery.sizeOf(context).width;
    if (w < 360) return 10;
    return 12;
  }

  @override
  void initState() {
    super.initState();
    SystemChrome.setSystemUIOverlayStyle(
      const SystemUiOverlayStyle(
        statusBarColor: Colors.transparent,
        statusBarIconBrightness: Brightness.light,
        systemNavigationBarColor: AuthTheme.gray50,
        systemNavigationBarIconBrightness: Brightness.dark,
      ),
    );
  }

  double _heroHeight(BuildContext context, bool keyboardOpen) {
    final h = MediaQuery.sizeOf(context).height;
    if (keyboardOpen) return 120;
    if (widget.style.minimalHero) return (h * 0.26).clamp(190.0, 230.0);
    final hasFeatures =
        widget.style.features != null && widget.style.features!.isNotEmpty;
    if (hasFeatures) return (h * 0.36).clamp(270.0, 340.0);
    if (widget.style.showTrustStats) {
      return (h * 0.36).clamp(280.0, 340.0);
    }
    return (h * 0.32).clamp(240.0, 300.0);
  }

  @override
  Widget build(BuildContext context) {
    final mq = MediaQuery.of(context);
    final bottomInset = mq.viewInsets.bottom;
    final bottomSafe = mq.padding.bottom;
    final keyboardOpen = bottomInset > 0;
    final heroHeight = _heroHeight(context, keyboardOpen);
    final horizontal = _horizontalInset(context);
    final dpr = mq.devicePixelRatio;

    return Scaffold(
      resizeToAvoidBottomInset: true,
      backgroundColor: AuthTheme.gray50,
      body: SafeArea(
        top: false,
        bottom: false,
        child: CustomScrollView(
          keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
          physics: const BouncingScrollPhysics(
            parent: AlwaysScrollableScrollPhysics(),
          ),
          slivers: [
            SliverToBoxAdapter(
              child: _AuthHero(
                style: widget.style,
                height: heroHeight,
                showBackButton: widget.showBackButton,
                compact: keyboardOpen,
                cacheWidth: (mq.size.width * dpr).round(),
              ),
            ),
            SliverToBoxAdapter(
              child: Transform.translate(
                offset: const Offset(0, -_panelOverlap),
                child: Padding(
                  padding: EdgeInsets.symmetric(horizontal: horizontal),
                  child: Column(
                    children: [
                      const SizedBox(height: 4),
                      StaggeredReveal(
                        delayMs: 160,
                        child: _AuthFormCard(
                          style: widget.style,
                          children: [
                            _AuthFormHeader(style: widget.style),
                            SizedBox(height: MediaQuery.sizeOf(context).width < 360 ? 16 : 20),
                            ...widget.children,
                          ],
                        ),
                      ),
                      if (widget.footer.isNotEmpty) ...[
                        const SizedBox(height: 12),
                        StaggeredReveal(
                          delayMs: 260,
                          child: _AuthFooterCard(children: widget.footer),
                        ),
                      ],
                      SizedBox(height: bottomInset + bottomSafe + 20),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _AuthFormCard extends StatelessWidget {
  final AuthPageStyle style;
  final List<Widget> children;

  const _AuthFormCard({required this.style, required this.children});

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(22),
        boxShadow: [
          BoxShadow(
            color: style.accent.withValues(alpha: 0.12),
            blurRadius: 28,
            offset: const Offset(0, 12),
          ),
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(22),
        child: Material(
          color: Colors.white,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Container(
                height: 4,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [style.buttonColor, style.buttonHover],
                  ),
                ),
              ),
              Padding(
                padding: EdgeInsets.fromLTRB(
                  18,
                  22,
                  18,
                  MediaQuery.sizeOf(context).width < 360 ? 18 : 22,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: children,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _AuthFormHeader extends StatelessWidget {
  final AuthPageStyle style;

  const _AuthFormHeader({required this.style});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (style.showFormRoleBadge && style.roleLabel.isNotEmpty) ...[
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
            decoration: BoxDecoration(
              color: style.accent.withValues(alpha: 0.08),
              borderRadius: BorderRadius.circular(999),
              border: Border.all(color: style.accent.withValues(alpha: 0.15)),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(style.icon, size: 13, color: style.accent),
                const SizedBox(width: 6),
                Text(
                  style.roleLabel,
                  style: AuthTheme.roleBadgeStyle(style.accent).copyWith(fontSize: 9),
                ),
              ],
            ),
          ),
          const SizedBox(height: 12),
        ],
        Text(
          style.title,
          style: AuthTheme.titleStyle(size: 27),
        ),
        if (style.subtitle.isNotEmpty) ...[
          const SizedBox(height: 8),
          Text(
            style.subtitle,
            style: AuthTheme.bodyStyle(color: AuthTheme.gray500).copyWith(
              fontSize: 13.5,
              height: 1.45,
            ),
          ),
        ],
      ],
    );
  }
}

class _AuthFooterCard extends StatelessWidget {
  final List<Widget> children;

  const _AuthFooterCard({required this.children});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: AuthTheme.gray100),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.03),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: children,
      ),
    );
  }
}

class _AuthHero extends StatelessWidget {
  final AuthPageStyle style;
  final double height;
  final bool showBackButton;
  final bool compact;
  final int cacheWidth;

  const _AuthHero({
    required this.style,
    required this.height,
    required this.showBackButton,
    required this.compact,
    required this.cacheWidth,
  });

  @override
  Widget build(BuildContext context) {
    final topPad = MediaQuery.paddingOf(context).top;

    return ClipRRect(
      borderRadius: const BorderRadius.only(
        bottomLeft: Radius.circular(28),
        bottomRight: Radius.circular(28),
      ),
      child: SizedBox(
        height: height + topPad,
        width: double.infinity,
        child: Stack(
          fit: StackFit.expand,
          children: [
            Image.network(
              style.heroImageUrl,
              fit: BoxFit.cover,
              alignment: const Alignment(0, -0.15),
              filterQuality: FilterQuality.medium,
              cacheWidth: cacheWidth,
              errorBuilder: (_, __, ___) => ColoredBox(color: AuthTheme.slate900),
            ),
            DecoratedBox(
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: [
                    const Color(0xCC0A0A0A),
                    style.accent.withValues(alpha: 0.55),
                    style.accent.withValues(alpha: 0.82),
                  ],
                  stops: const [0.0, 0.5, 1.0],
                ),
              ),
            ),
            IgnorePointer(
              child: Stack(
                children: [
                  Positioned(
                    top: -30,
                    right: -20,
                    child: _HeroGlow(
                      color: style.buttonHover.withValues(alpha: 0.35),
                      size: 140,
                    ),
                  ),
                  Positioned(
                    bottom: 40,
                    left: -40,
                    child: _HeroGlow(
                      color: style.accent.withValues(alpha: 0.25),
                      size: 100,
                    ),
                  ),
                ],
              ),
            ),
            Padding(
              padding: EdgeInsets.fromLTRB(
                16,
                topPad + 8,
                16,
                compact ? 16 : (style.showTrustStats ? 40 : 28),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      if (showBackButton)
                        _GlassIconButton(
                          icon: Icons.arrow_back_rounded,
                          onTap: () => Navigator.of(context).maybePop(),
                        )
                      else
                        const SizedBox(width: 4),
                      const SizedBox(width: 10),
                      Expanded(child: _BrandMark(accent: style.accent)),
                    ],
                  ),
                  const Spacer(),
                  if (!compact && !style.minimalHero)
                    _AuthHeroBody(style: style)
                  else if (style.minimalHero && !compact)
                    Align(
                      alignment: Alignment.bottomLeft,
                      child: Text(
                        style.heroTitle.isNotEmpty ? style.heroTitle : 'HUITMeal',
                        style: AuthTheme.titleStyle(size: 24, color: Colors.white),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _HeroGlow extends StatelessWidget {
  final Color color;
  final double size;

  const _HeroGlow({required this.color, required this.size});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        gradient: RadialGradient(
          colors: [color, color.withValues(alpha: 0)],
        ),
      ),
    );
  }
}

class _GlassIconButton extends StatelessWidget {
  final IconData icon;
  final VoidCallback onTap;

  const _GlassIconButton({required this.icon, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(12),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 8, sigmaY: 8),
        child: Material(
          color: Colors.white.withValues(alpha: 0.16),
          child: InkWell(
            onTap: onTap,
            child: Container(
              padding: const EdgeInsets.all(9),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: Colors.white.withValues(alpha: 0.22)),
              ),
              child: Icon(icon, color: Colors.white, size: 20),
            ),
          ),
        ),
      ),
    );
  }
}

class _AuthHeroBody extends StatelessWidget {
  final AuthPageStyle style;

  const _AuthHeroBody({required this.style});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (style.roleLabel.isNotEmpty && !style.showFormRoleBadge)
          _HeroChip(label: style.roleLabel, icon: style.icon),
        if (style.heroTitle.isNotEmpty) ...[
          if (style.roleLabel.isNotEmpty && !style.showFormRoleBadge)
            const SizedBox(height: 10),
          Text(
            style.heroTitle,
            style: AuthTheme.titleStyle(size: 24, color: Colors.white),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ],
        if (style.heroSubtitle.isNotEmpty) ...[
          const SizedBox(height: 6),
          Text(
            style.heroSubtitle,
            style: AuthTheme.bodyStyle(color: Colors.white.withValues(alpha: 0.88))
                .copyWith(fontSize: 13.5, height: 1.35),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ],
        if (style.showTrustStats && style.trustStats != null) ...[
          const SizedBox(height: 16),
          Row(
            children: [
              for (var i = 0; i < style.trustStats!.length; i++) ...[
                if (i > 0) const SizedBox(width: 8),
                Expanded(
                  child: _TrustStat(
                    value: style.trustStats![i].value,
                    label: style.trustStats![i].label,
                    color: style.buttonHover,
                  ),
                ),
              ],
            ],
          ),
        ],
        if (style.features != null)
          ...style.features!.map(
            (f) => Padding(
              padding: const EdgeInsets.only(top: 8),
              child: AuthFeatureRow(icon: f.icon, text: f.text, color: style.buttonHover),
            ),
          ),
      ],
    );
  }
}

class _BrandMark extends StatelessWidget {
  final Color accent;
  const _BrandMark({required this.accent});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 38,
          height: 38,
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(12),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.15),
                blurRadius: 8,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Icon(Icons.restaurant_menu_rounded, color: accent, size: 20),
        ),
        const SizedBox(width: 10),
        Flexible(
          child: Text(
            'HUITMeal',
            style: AuthTheme.displayFont.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w800,
              fontSize: 18,
              letterSpacing: -0.3,
            ),
            overflow: TextOverflow.ellipsis,
          ),
        ),
      ],
    );
  }
}

class _HeroChip extends StatelessWidget {
  final String label;
  final IconData icon;

  const _HeroChip({required this.label, required this.icon});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.14),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: Colors.white.withValues(alpha: 0.22)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 12, color: Colors.white70),
          const SizedBox(width: 6),
          Text(
            label,
            style: AuthTheme.displayFont.copyWith(
              color: Colors.white,
              fontSize: 10,
              fontWeight: FontWeight.w700,
              letterSpacing: 1.2,
            ),
          ),
        ],
      ),
    );
  }
}

class _TrustStat extends StatelessWidget {
  final String value;
  final String label;
  final Color color;

  const _TrustStat({required this.value, required this.label, required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 6),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.white.withValues(alpha: 0.14)),
      ),
      child: Column(
        children: [
          Text(
            value,
            style: AuthTheme.displayFont.copyWith(
              color: color,
              fontWeight: FontWeight.w900,
              fontSize: 16,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            label.toUpperCase(),
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: AuthTheme.displayFont.copyWith(
              color: Colors.white.withValues(alpha: 0.75),
              fontSize: 8,
              fontWeight: FontWeight.w700,
              letterSpacing: 0.5,
            ),
          ),
        ],
      ),
    );
  }
}
