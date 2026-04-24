CREATE TABLE user_roles (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    AssignedBy INT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_user_roles_code (Code),
    UNIQUE KEY UK_user_roles_user_role (UserId, RoleId),
    INDEX IX_user_roles_role (RoleId),
    CONSTRAINT FK_user_roles_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_user_roles_role FOREIGN KEY (RoleId) REFERENCES roles (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_user_roles_assigned_by FOREIGN KEY (AssignedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Gán vai trò cho người dùng';
