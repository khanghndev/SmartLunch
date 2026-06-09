import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:pdfx/pdfx.dart';

import '../../../../core/theme/app_design_system.dart';
import '../widgets/shipper_ui.dart';

/// Xem PDF biên bản bàn giao sau khi người nhận ký xác nhận giao hàng.
class ShipperHandoverPdfPage extends StatefulWidget {
  const ShipperHandoverPdfPage({
    super.key,
    required this.title,
    this.pdfUrl,
    this.subtitle,
  });

  final String title;
  final String? subtitle;
  final String? pdfUrl;

  @override
  State<ShipperHandoverPdfPage> createState() => _ShipperHandoverPdfPageState();
}

class _ShipperHandoverPdfPageState extends State<ShipperHandoverPdfPage> {
  PdfControllerPinch? _controller;
  String? _error;

  @override
  void initState() {
    super.initState();
    _openDocument();
  }

  Future<PdfDocument> _loadDocument() async {
    final url = widget.pdfUrl?.trim();
    if (url == null || url.isEmpty) {
      throw StateError('Không có liên kết biên bản bàn giao.');
    }
    final res = await http.get(Uri.parse(url));
    if (res.statusCode < 200 || res.statusCode >= 300) {
      throw StateError('Không tải PDF (${res.statusCode}).');
    }
    return PdfDocument.openData(res.bodyBytes);
  }

  Future<void> _openDocument() async {
    try {
      final docFuture = _loadDocument();
      if (!mounted) return;
      setState(() {
        _controller = PdfControllerPinch(document: docFuture);
        _error = null;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = 'Không mở được biên bản: $e');
    }
  }

  @override
  void dispose() {
    _controller?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ShipperPageShell(
      title: widget.title,
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (widget.subtitle != null) ...[
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
              child: ShipperInfoBanner(
                message: widget.subtitle!,
                icon: Icons.picture_as_pdf_outlined,
                color: AppDesignSystem.info,
              ),
            ),
            const SizedBox(height: 8),
          ],
          Expanded(
            child: _error != null
                ? Center(
                    child: Padding(
                      padding: const EdgeInsets.all(24),
                      child: Text(_error!, textAlign: TextAlign.center),
                    ),
                  )
                : _controller == null
                    ? const Center(child: CircularProgressIndicator())
                    : PdfViewPinch(
                        controller: _controller!,
                        padding: 10,
                      ),
          ),
        ],
      ),
    );
  }
}
