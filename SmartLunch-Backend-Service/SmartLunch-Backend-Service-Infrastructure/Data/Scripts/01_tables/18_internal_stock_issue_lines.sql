CREATE TABLE internal_stock_issue_lines (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    IssueId INT NOT NULL,
    IngredientId INT NOT NULL,
    Quantity DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_stock_issue_lines_code (Code),
    INDEX IX_stock_issue_lines_issue (IssueId),
    INDEX IX_stock_issue_lines_ingredient (IngredientId),
    CONSTRAINT FK_stock_issue_lines_issue FOREIGN KEY (IssueId) REFERENCES internal_stock_issues (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_stock_issue_lines_ingredient FOREIGN KEY (IngredientId) REFERENCES ingredients (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Chi tiết phiếu xuất kho';
