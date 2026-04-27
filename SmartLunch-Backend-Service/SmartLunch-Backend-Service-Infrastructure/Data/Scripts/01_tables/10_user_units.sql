CREATE TABLE user_units (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    UserId INT NOT NULL,
    UnitId INT NOT NULL,
    JoinedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_user_units_code (Code),
    UNIQUE KEY UK_user_units_user_unit (UserId, UnitId),
    INDEX IX_user_units_unit (UnitId),
    CONSTRAINT FK_user_units_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_user_units_unit FOREIGN KEY (UnitId) REFERENCES units (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thành viên đơn vị';
