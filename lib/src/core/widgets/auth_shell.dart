import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../features/auth/presentation/widgets/auth_theme.dart';
import '../../features/auth/presentation/widgets/auth_widgets.dart';
import 'staggered_reveal.dart';

/// Layout auth: hero + form trong **một** scroll — tránh che nội dung, hỗ trợ bàn phím.
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
    if (keyboardOpen) {
      return widget.style.minimalHero ? 88 : 112;
    }
    if (widget.style.minimalHero) {
      return (h * 0.28).clamp(200.0, 260.0);
    }
    final hasFeatures =
        widget.style.features != null && widget.style.features!.isNotEmpty;
    if (hasFeatures) {
      return (h * 0.38).clamp(280.0, 360.0);
    }
    return (h * 0.34).clamp(220.0, 300.0);
  }

  @override
  Widget build(BuildContext context) {
    final mq = MediaQuery.of(context);
    final bottomInset = mq.viewInsets.bottom;
    final bottomSafe = mq.padding.bottom;
    final keyboardOpen = bottomInset > 0;
    final heroHeight = _heroHeight(context, keyboardOpen);
    final horizontal = 20.0;
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
            SliverPadding(
              padding: EdgeInsets.fromLTRB(horizontal, 16, horizontal, 0),
              sliver: SliverList(
                delegate: SliverChildListDelegate([
                  StaggeredReveal(
                    delayMs: 180,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (widget.style.showFormRoleBadge &&
                            widget.style.roleLabel.isNotEmpty) ...[
                          Text(
                            widget.style.roleLabel,
                            style: AuthTheme.roleBadgeStyle(widget.style.accent),
                          ),
                          const SizedBox(height: 4),
                        ],
                        Text(widget.style.title, style: AuthTheme.titleStyle()),
                        if (widget.style.subtitle.isNotEmpty) ...[
                          const SizedBox(height: 8),
                          Text(widget.style.subtitle, style: AuthTheme.bodyStyle()),
                        ],
                      ],
                    ),
                  ),
                  const SizedBox(height: 16),
                  StaggeredReveal(
                    delayMs: 240,
                    child: Container(
                      width: double.infinity,
                      padding: const EdgeInsets.all(20),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                        border: Border.all(color: AuthTheme.gray100, width: 1),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withValues(alpha: 0.04),
                            blurRadius: 16,
                            offset: const Offset(0, 4),
                          ),
                        ],
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: widget.children,
                      ),
                    ),
                  ),
                  if (widget.footer.isNotEmpty) ...[
                    const SizedBox(height: 20),
                    StaggeredReveal(
                      delayMs: 300,
                      child: Column(
                        children: [
                          const Divider(color: AuthTheme.gray100, height: 32),
                          ...widget.footer,
                        ],
                      ),
                    ),
                  ],
                  SizedBox(height: bottomInset + bottomSafe + 24),
                ]),
              ),
            ),
          ],
        ),
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

    return SizedBox(
      height: height + topPad,
      width: double.infinity,
      child: Stack(
        fit: StackFit.expand,
        children: [
          ClipRect(
            child: Image.network(
              style.heroImageUrl,
              fit: BoxFit.cover,
              alignment: Alignment.center,
              filterQuality: FilterQuality.medium,
              cacheWidth: cacheWidth,
              errorBuilder: (_, __, ___) => ColoredBox(color: AuthTheme.slate900),
            ),
          ),
          const DecoratedBox(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [
                  Color(0xCC000000),
                  Color(0x66000000),
                  Color(0x33000000),
                ],
              ),
            ),
          ),
          Padding(
            padding: EdgeInsets.fromLTRB(16, topPad + 8, 16, 12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    if (showBackButton)
                      Material(
                        color: Colors.white.withValues(alpha: 0.12),
                        borderRadius: BorderRadius.circular(10),
                        child: InkWell(
                          onTap: () => Navigator.of(context).maybePop(),
                          borderRadius: BorderRadius.circular(10),
                          child: const Padding(
                            padding: EdgeInsets.all(8),
                            child: Icon(
                              Icons.arrow_back_rounded,
                              color: Colors.white,
                              size: 20,
                            ),
                          ),
                        ),
                      )
                    else
                      const SizedBox(width: 4),
                    const SizedBox(width: 8),
                    const Expanded(child: _BrandMark()),
                  ],
                ),
                Expanded(
                  child: style.minimalHero
                      ? Center(
                          child: Text(
                            'HUITMeal',
                            style: AuthTheme.titleStyle(
                              size: compact ? 26 : 34,
                              color: Colors.white,
                            ),
                          ),
                        )
                      : compact
                          ? const SizedBox.shrink()
                          : Align(
                              alignment: Alignment.bottomLeft,
                              child: SingleChildScrollView(
                                physics: const ClampingScrollPhysics(),
                                child: _AuthHeroBody(style: style),
                              ),
                            ),
                ),
              ],
            ),
          ),
        ],
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
        if (style.roleLabel.isNotEmpty)
          _HeroChip(label: style.roleLabel, icon: style.icon),
        if (style.heroTitle.isNotEmpty) ...[
          const SizedBox(height: 8),
          Text(
            style.heroTitle,
            style: AuthTheme.titleStyle(size: 22, color: Colors.white),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ],
        if (style.heroSubtitle.isNotEmpty) ...[
          const SizedBox(height: 4),
          Text(
            style.heroSubtitle,
            style: AuthTheme.bodyStyle(color: const Color(0xFFD1D5DB)),
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
          ),
        ],
        if (style.showTrustStats && style.trustStats != null) ...[
          const SizedBox(height: 10),
          Row(
            children: [
              for (var i = 0; i < style.trustStats!.length; i++) ...[
                if (i > 0) const SizedBox(width: 8),
                Expanded(
                  child: _TrustStat(
                    value: style.trustStats![i].value,
                    label: style.trustStats![i].label,
                  ),
                ),
              ],
            ],
          ),
        ],
        if (style.features != null)
          ...style.features!.map(
            (f) => Padding(
              padding: const EdgeInsets.only(top: 6),
              child: AuthFeatureRow(icon: f.icon, text: f.text),
            ),
          ),
      ],
    );
  }
}

class _BrandMark extends StatelessWidget {
  const _BrandMark();

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 36,
          height: 36,
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(10),
          ),
          child: const Icon(
            Icons.restaurant_menu_rounded,
            color: AuthTheme.orange500,
            size: 20,
          ),
        ),
        const SizedBox(width: 10),
        Flexible(
          child: Text(
            'HUITMeal',
            style: AuthTheme.displayFont.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.w900,
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
        color: Colors.white.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: Colors.white.withValues(alpha: 0.2)),
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
              letterSpacing: 2,
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

  const _TrustStat({required this.value, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
      decoration: BoxDecoration(
        color: Colors.white.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.white.withValues(alpha: 0.1)),
      ),
      child: Column(
        children: [
          Text(
            value,
            style: AuthTheme.displayFont.copyWith(
              color: AuthTheme.orange400,
              fontWeight: FontWeight.w900,
              fontSize: 15,
            ),
          ),
          Text(
            label.toUpperCase(),
            textAlign: TextAlign.center,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: AuthTheme.displayFont.copyWith(
              color: const Color(0xFF9CA3AF),
              fontSize: 8,
              fontWeight: FontWeight.w600,
              letterSpacing: 0.6,
            ),
          ),
        ],
      ),
    );
  }
}
