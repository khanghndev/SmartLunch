-- Chữ ký shipper trên biên bản bàn giao giao hàng.
SET @db := DATABASE();

SET @sql := IF(
    (SELECT COUNT(*) FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA = @db AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'ShipperSignatureUrl') = 0,
    'ALTER TABLE deliveries ADD COLUMN ShipperSignatureUrl VARCHAR(500) NULL COMMENT ''URL ảnh chữ ký shipper'' AFTER HandoverDocumentUrl',
    'SELECT ''ShipperSignatureUrl already exists'' AS msg');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
