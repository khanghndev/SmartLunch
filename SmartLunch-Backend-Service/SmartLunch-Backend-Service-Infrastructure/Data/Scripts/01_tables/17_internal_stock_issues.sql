CREATE TABLE internal_stock_issues (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    IssueCode VARCHAR(40) NOT NULL,
    IssuedAt DATETIME NOT NULL,
    Reason VARCHAR(500) NULL,
    CreatedByUserId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_stock_issues_code (Code),
    UNIQUE KEY UK_stock_issues_issue_code (IssueCode),
    INDEX IX_stock_issues_user (CreatedByUserId),
    CONSTRAINT FK_stock_issues_user FOREIGN KEY (CreatedByUserId) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Phiếu xuất kho nội bộ';
