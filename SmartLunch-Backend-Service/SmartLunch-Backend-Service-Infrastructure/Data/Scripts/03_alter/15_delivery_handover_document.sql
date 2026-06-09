-- Biên bản bàn giao PDF sau khi người nhận ký xác nhận giao hàng.
SET @db := DATABASE();

SET @sql := IF(
    (SELECT COUNT(*) FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA = @db AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'HandoverDocumentUrl') = 0,
    'ALTER TABLE deliveries ADD COLUMN HandoverDocumentUrl VARCHAR(500) NULL COMMENT ''URL PDF biên bản bàn giao (signed)'' AFTER RecipientSignatureUrl',
    'SELECT ''HandoverDocumentUrl already exists'' AS msg');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
