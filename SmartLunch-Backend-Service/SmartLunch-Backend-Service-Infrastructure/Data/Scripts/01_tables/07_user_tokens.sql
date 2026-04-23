CREATE TABLE user_tokens (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    UserId INT NOT NULL,
    AccessToken VARCHAR(2000) NOT NULL,
    RefreshToken VARCHAR(2000) NOT NULL,
    IssuedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ExpiresAt DATETIME NOT NULL,
    RevokedAt DATETIME NULL,
    ReplacedByToken VARCHAR(2000) NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_user_tokens_code (Code),
    INDEX IX_user_tokens_user (UserId),
    INDEX IX_user_tokens_active (IsActive),
    INDEX IX_user_tokens_expires (ExpiresAt),
    CONSTRAINT FK_user_tokens_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Token xác thực';
