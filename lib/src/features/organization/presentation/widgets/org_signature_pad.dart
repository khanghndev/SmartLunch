import 'dart:convert';
import 'dart:typed_data';
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
  Uint8List? _cachedExportBytes;
  Size _lastLayoutSize = const Size(320, 180);

  bool get hasSignature =>
      _strokes.any((s) => s.length >= 2) || _current.length >= 2;

  void clear() {
    setState(() {
      _strokes.clear();
      _current.clear();
      _cachedExportBytes = null;
    });
    widget.onSignatureChanged?.call(false);
  }

  Future<Uint8List?> exportPngBytes() async {
    if (!hasSignature) return null;
    if (_cachedExportBytes != null && _cachedExportBytes!.isNotEmpty) {
      return _cachedExportBytes;
    }

    final boundary = _boundaryKey.currentContext?.findRenderObject();
    if (boundary is RenderRepaintBoundary) {
      try {
        final image = await boundary.toImage(pixelRatio: 2);
        final bytes = await image.toByteData(format: ui.ImageByteFormat.png);
        if (bytes != null) {
          _cachedExportBytes = bytes.buffer.asUint8List();
          return _cachedExportBytes;
        }
      } catch (_) {
        /* fallback below */
      }
    }

    _cachedExportBytes = await _renderStrokesToPng(_lastLayoutSize);
    return _cachedExportBytes;
  }

  Future<Uint8List?> _renderStrokesToPng(Size size) async {
    if (!hasSignature) return null;
    final w = (size.width * 2).round().clamp(1, 4096);
    final h = (size.height * 2).round().clamp(1, 4096);
    final recorder = ui.PictureRecorder();
    final canvas = Canvas(recorder);
    canvas.drawRect(
      Rect.fromLTWH(0, 0, w.toDouble(), h.toDouble()),
      Paint()..color = AppDesignSystem.gray50,
    );
    final scaleX = w / size.width;
    final scaleY = h / size.height;
    canvas.scale(scaleX, scaleY);
    _SignaturePainter(
      _strokes.map((s) => List<Offset>.from(s)).toList(growable: false),
    ).paint(canvas, size);
    final picture = recorder.endRecording();
    final image = picture.toImageSync(w, h);
    final bytes = await image.toByteData(format: ui.ImageByteFormat.png);
    return bytes?.buffer.asUint8List();
  }

  void _syncExportCache() {
    if (!hasSignature) {
      _cachedExportBytes = null;
      return;
    }
    _renderStrokesToPng(_lastLayoutSize).then((bytes) {
      _cachedExportBytes = bytes;
    });
  }

  Future<String?> exportDataUrl() async {
    final png = await exportPngBytes();
    if (png == null) return null;
    final b64 = base64Encode(png);
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
    _syncExportCache();
    widget.onSignatureChanged?.call(hasSignature);
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        LayoutBuilder(
          builder: (context, constraints) {
            final width = constraints.maxWidth.isFinite && constraints.maxWidth > 0
                ? constraints.maxWidth
                : 320.0;
            _lastLayoutSize = Size(width, 180);
            return RepaintBoundary(
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
            );
          },
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
