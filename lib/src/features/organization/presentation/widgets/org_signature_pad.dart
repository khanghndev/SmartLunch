import 'dart:convert';
import 'dart:ui' as ui;

import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';

import '../../../../core/theme/app_design_system.dart';
/// Vùng ký tên (chuột/cảm ứng) — xuất PNG data URL cho API `sign-annex`.
class OrgSignaturePad extends StatefulWidget {
  const OrgSignaturePad({super.key, this.onSignatureChanged});

  final ValueChanged<bool>? onSignatureChanged;

  @override
  State<OrgSignaturePad> createState() => OrgSignaturePadState();
}

class OrgSignaturePadState extends State<OrgSignaturePad> {
  final GlobalKey _boundaryKey = GlobalKey();
  final List<List<Offset>> _strokes = [];
  List<Offset> _current = [];

  bool get hasSignature =>
      _strokes.any((s) => s.length >= 2) || _current.length >= 2;

  void clear() {
    setState(() {
      _strokes.clear();
      _current.clear();
    });
    widget.onSignatureChanged?.call(false);
  }

  Future<String?> exportDataUrl() async {
    if (!hasSignature) return null;
    final boundary = _boundaryKey.currentContext?.findRenderObject();
    if (boundary is! RenderRepaintBoundary) return null;
    final image = await boundary.toImage(pixelRatio: 2);
    final bytes = await image.toByteData(format: ui.ImageByteFormat.png);
    if (bytes == null) return null;
    final b64 = base64Encode(bytes.buffer.asUint8List());
    return 'data:image/png;base64,$b64';
  }

  void _start(Offset pos) {
    setState(() {
      _current = [pos];
      _strokes.add(_current);
    });
  }

  void _move(Offset pos) {
    if (_current.isEmpty) return;
    setState(() => _current.add(pos));
    widget.onSignatureChanged?.call(hasSignature);
  }

  void _end() {
    widget.onSignatureChanged?.call(hasSignature);
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        RepaintBoundary(
          key: _boundaryKey,
          child: Container(
            height: 180,
            decoration: BoxDecoration(
              color: AppDesignSystem.gray50,
              borderRadius: BorderRadius.circular(14),
              border: Border.all(color: AppDesignSystem.gray200, width: 2),
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: GestureDetector(
                onPanStart: (d) => _start(d.localPosition),
                onPanUpdate: (d) => _move(d.localPosition),
                onPanEnd: (_) => _end(),
                child: SizedBox.expand(
                  child: CustomPaint(
                    painter: _SignaturePainter(
                      _strokes
                          .map((s) => List<Offset>.from(s))
                          .toList(growable: false),
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            TextButton.icon(
              onPressed: clear,
              icon: const Icon(Icons.refresh_rounded, size: 18),
              label: const Text('Xóa chữ ký'),
            ),
            const Spacer(),
            Text(
              'Ký bằng ngón tay hoặc bút',
              style: AppDesignSystem.body(size: 11),
            ),
          ],
        ),
      ],
    );
  }
}

class _SignaturePainter extends CustomPainter {
  _SignaturePainter(this.strokes);

  final List<List<Offset>> strokes;

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = const Color(0xFF0F172A)
      ..strokeWidth = 3.0
      ..strokeCap = StrokeCap.round
      ..style = PaintingStyle.stroke;

    for (final stroke in strokes) {
      if (stroke.length < 2) continue;
      final path = Path()..moveTo(stroke.first.dx, stroke.first.dy);
      for (var i = 1; i < stroke.length; i++) {
        path.lineTo(stroke[i].dx, stroke[i].dy);
      }
      canvas.drawPath(path, paint);
    }
  }

  @override
  bool shouldRepaint(covariant _SignaturePainter old) {
    if (old.strokes.length != strokes.length) return true;
    for (var i = 0; i < strokes.length; i++) {
      if (old.strokes[i].length != strokes[i].length) return true;
    }
    return true;
  }
}
