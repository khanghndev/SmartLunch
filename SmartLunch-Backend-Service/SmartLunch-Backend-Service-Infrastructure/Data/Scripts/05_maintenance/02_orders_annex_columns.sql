-- Thêm cột lưu PDF phụ lục đặt hàng (ký mô phỏng + cloud). Chạy một lần trên DB hiện có.
ALTER TABLE orders
    ADD COLUMN AnnexPdfUrl VARCHAR(2048) NULL COMMENT 'URL signed PDF phụ lục đơn' AFTER InvoiceCode,
    ADD COLUMN AnnexSignedAt DATETIME NULL COMMENT 'Thời điểm ký phụ lục (UTC)' AFTER AnnexPdfUrl;
