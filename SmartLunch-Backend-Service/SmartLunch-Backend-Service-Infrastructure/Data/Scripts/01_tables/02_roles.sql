-- =====================================================
-- Table: roles
-- =====================================================
CREATE TABLE roles (
    Id INT NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    Name VARCHAR(100) NOT NULL COMMENT 'Tên vai trò',
    Description VARCHAR(500) NULL COMMENT 'Mô tả',
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    IsSystemRole TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Vai trò hệ thống (không xóa được)',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT NULL,
    UpdatedBy INT NULL,

    PRIMARY KEY (Id),
    UNIQUE KEY UK_roles_code (Code),
    UNIQUE KEY UK_roles_name (Name),
    INDEX IX_roles_active_system (IsActive, IsSystemRole),

    CONSTRAINT FK_roles_created_by FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT FK_roles_updated_by FOREIGN KEY (UpdatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Vai trò hệ thống';
