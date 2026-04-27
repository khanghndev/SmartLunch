CREATE TABLE user_organizations (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    UserId INT NOT NULL,
    OrganizationId INT NOT NULL,
    JoinedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_user_organizations_code (Code),
    UNIQUE KEY UK_user_organizations_user_org (UserId, OrganizationId),
    INDEX IX_user_organizations_org (OrganizationId),
    CONSTRAINT FK_user_organizations_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_user_organizations_org FOREIGN KEY (OrganizationId) REFERENCES organizations (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thành viên đơn vị';
