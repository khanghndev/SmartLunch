-- Thêm cột lưu PDF phụ lục đặt hàng (ký mô phỏng + cloud).
-- An toàn để chạy nhiều lần: chỉ thêm cột khi chưa tồn tại.

DROP PROCEDURE IF EXISTS smartlunch_add_orders_annex_columns;
DELIMITER //
CREATE PROCEDURE smartlunch_add_orders_annex_columns()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'AnnexPdfUrl'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN AnnexPdfUrl VARCHAR(2048) NULL COMMENT 'URL signed PDF phụ lục đơn' AFTER InvoiceCode;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'AnnexSignedAt'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN AnnexSignedAt DATETIME NULL COMMENT 'Thời điểm ký phụ lục (UTC)' AFTER AnnexPdfUrl;
    END IF;
END //
DELIMITER ;

CALL smartlunch_add_orders_annex_columns();
DROP PROCEDURE IF EXISTS smartlunch_add_orders_annex_columns;
