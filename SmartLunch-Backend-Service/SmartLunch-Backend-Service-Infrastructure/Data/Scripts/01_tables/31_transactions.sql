CREATE TABLE transactions (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    Date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description VARCHAR(255) NULL,
    Amount DECIMAL(12,2) NOT NULL,
    Category VARCHAR(100) NULL,
    Method VARCHAR(50) NULL,
    ReferenceId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_transactions_code (Code),
    INDEX IX_transactions_date (Date DESC),
    INDEX IX_transactions_category (Category)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thu chi tài chính';
