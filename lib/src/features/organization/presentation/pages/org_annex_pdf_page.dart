import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:pdfx/pdfx.dart';

import '../../../../core/theme/app_design_system.dart';
import '../widgets/organization_ui.dart';

/// Xem PDF phụ lục — khớp iframe preview trên web `Profile/Contracts`.
class OrgAnnexPdfPage extends StatefulWidget {
  const OrgAnnexPdfPage({
    super.key,
    required this.title,
    this.pdfBytes,
    this.pdfUrl,
    this.subtitle,
  });

  final String title;
  final String? subtitle;
  final Uint8List? pdfBytes;
  final String? pdfUrl;

  @override
  State<OrgAnnexPdfPage> createState() => _OrgAnnexPdfPageState();
}

class _OrgAnnexPdfPageState extends State<OrgAnnexPdfPage> {
  PdfControllerPinch? _controller;
  String? _error;

  @override
  void initState() {
    super.initState();
    _openDocument();
  }

  Future<PdfDocument> _loadDocument() async {
    if (widget.pdfBytes != null && widget.pdfBytes!.isNotEmpty) {
      return PdfDocument.openData(widget.pdfBytes!);
    }
    final url = widget.pdfUrl?.trim();
    if (url == null || url.isEmpty) {
      throw StateError('Không có dữ liệu PDF.');
    }
    final res = await http.get(Uri.parse(url));
    if (res.statusCode < 200 || res.statusCode >= 300) {
      throw StateError('Không tải PDF từ liên kết (${res.statusCode}).');
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
      setState(() => _error = 'Không mở được PDF: $e');
    }
  }

  @override
  void dispose() {
    _controller?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return OrgPageShell(
      title: widget.title,
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (widget.subtitle != null) ...[
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
              child: OrgInfoBanner(
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
