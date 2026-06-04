-- Chữ ký người nhận khi shipper POST /proof (thay OTP).
SET @db := DATABASE();

SET @col_exists := (
    SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = @db AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'RecipientSignatureUrl'
);
SET @sql := IF(@col_exists = 0,
    'ALTER TABLE deliveries ADD COLUMN RecipientSignatureUrl VARCHAR(500) NULL COMMENT ''URL ảnh chữ ký người nhận'' AFTER RecipientConfirmationCode',
    'SELECT ''RecipientSignatureUrl already exists'' AS msg');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
