ALTER TABLE system_backups
    ADD COLUMN BackupSource VARCHAR(16) NOT NULL DEFAULT 'Manual' COMMENT 'Manual | Scheduled' AFTER SizeBytes;
