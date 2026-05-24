import 'package:flutter/material.dart';

/// Chiều cao nội dung [BottomNavigationBar] (Material mặc định).
const double kModuleBottomNavHeight = kBottomNavigationBarHeight;

/// Padding đáy khi nội dung nằm trên bottom nav + safe area (+ bàn phím nếu có).
double moduleScrollBottomPadding(
  BuildContext context, {
  double extra = 16,
  double bottomBarInset = 0,
  bool includeKeyboard = true,
}) {
  final mq = MediaQuery.of(context);
  final keyboard = includeKeyboard ? mq.viewInsets.bottom : 0;
  return mq.padding.bottom + bottomBarInset + keyboard + extra;
}

/// Padding chuẩn cho ListView trong module (AppBar full-screen).
EdgeInsets moduleListPadding(
  BuildContext context, {
  double horizontal = 16,
  double top = 16,
  double extraBottom = 24,
  double bottomBarInset = 0,
}) {
  return EdgeInsets.fromLTRB(
    horizontal,
    top,
    horizontal,
    moduleScrollBottomPadding(
      context,
      extra: extraBottom,
      bottomBarInset: bottomBarInset,
    ),
  );
}

/// ListView bọc sẵn physics + padding đáy (tránh che bởi nav / bàn phím).
class ModuleListView extends StatelessWidget {
  final List<Widget> children;
  final EdgeInsetsGeometry? padding;
  final double bottomBarInset;
  final Future<void> Function()? onRefresh;
  final Color? refreshColor;

  const ModuleListView({
    super.key,
    required this.children,
    this.padding,
    this.bottomBarInset = 0,
    this.onRefresh,
    this.refreshColor,
  });

  @override
  Widget build(BuildContext context) {
    final resolvedPadding = padding ??
        moduleListPadding(context, bottomBarInset: bottomBarInset);

    final list = ListView(
      physics: const AlwaysScrollableScrollPhysics(
        parent: BouncingScrollPhysics(),
      ),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      padding: resolvedPadding,
      children: children,
    );

    if (onRefresh == null) return list;

    return RefreshIndicator(
      color: refreshColor,
      onRefresh: onRefresh!,
      child: list,
    );
  }
}
