CREATE TABLE menu_suggestions (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    WeekStart DATE NOT NULL,
    GeneratedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    SuggestionText TEXT NOT NULL,
    AlgorithmVersion VARCHAR(50) NULL,
    CreatedBy INT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_menu_suggestions_code (Code),
    INDEX IX_menu_suggestions_week (WeekStart),
    CONSTRAINT FK_menu_suggestions_user FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Gợi ý thực đơn AI';
