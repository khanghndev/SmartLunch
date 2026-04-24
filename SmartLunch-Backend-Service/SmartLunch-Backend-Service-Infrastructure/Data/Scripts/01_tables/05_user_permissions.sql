CREATE TABLE user_permissions (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    UserId INT NOT NULL,
    PermissionId INT NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    AssignedBy INT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_user_permissions_code (Code),
    UNIQUE KEY UK_user_permissions_user_perm (UserId, PermissionId),
    INDEX IX_user_permissions_perm (PermissionId),
    CONSTRAINT FK_user_permissions_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_user_permissions_perm FOREIGN KEY (PermissionId) REFERENCES permissions (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_user_permissions_assigned_by FOREIGN KEY (AssignedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Gán quyền trực tiếp cho người dùng';
