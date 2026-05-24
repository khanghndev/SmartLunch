-- Patch: system_backups.BackupSource (Manual | Scheduled) — idempotent
USE SmartLunch;

SET @col_exists := (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'system_backups'
      AND COLUMN_NAME = 'BackupSource'
);

SET @ddl := IF(
    @col_exists = 0,
    'ALTER TABLE system_backups ADD COLUMN BackupSource VARCHAR(16) NOT NULL DEFAULT ''Manual'' COMMENT ''Manual | Scheduled'' AFTER SizeBytes',
    'SELECT ''system_backups.BackupSource already exists'' AS Status'
);

PREPARE stmt FROM @ddl;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
