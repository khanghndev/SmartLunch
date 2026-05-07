CREATE TABLE system_backups (
    Id INT NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
    FileName VARCHAR(255) NOT NULL,
    StorageBucket VARCHAR(255) NOT NULL,
    StorageObjectName VARCHAR(1024) NOT NULL,
    SizeBytes BIGINT NOT NULL DEFAULT 0,
    CreatedAtUtc DATETIME NOT NULL,
    RestoredAtUtc DATETIME NULL,
    DeletedAtUtc DATETIME NULL,
    IsDeleted TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (Id),
    INDEX idx_system_backups_created_at (CreatedAtUtc),
    INDEX idx_system_backups_is_deleted (IsDeleted)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Backup files metadata';

