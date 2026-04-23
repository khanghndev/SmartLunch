CREATE TABLE permissions (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(500) NULL,
    Resource VARCHAR(100) NOT NULL,
    Action VARCHAR(50) NOT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT NULL,
    UpdatedBy INT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_permissions_code (Code),
    UNIQUE KEY UK_permissions_name (Name),
    INDEX IX_permissions_resource_action (Resource, Action, IsActive),
    CONSTRAINT FK_permissions_created_by FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT FK_permissions_updated_by FOREIGN KEY (UpdatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Quyền hạn hệ thống';
