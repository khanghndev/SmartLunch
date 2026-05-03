CREATE TABLE ingredient_categories (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Code VARCHAR(50) NOT NULL COMMENT 'Mã tự sinh',
    Name VARCHAR(100) NOT NULL,
    NameEnglish VARCHAR(50) NOT NULL COMMENT 'Identifier used in AI rules, e.g. meat, seafood',
    Description VARCHAR(255),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY UQ_Cat_Code (Code),
    UNIQUE KEY UQ_Cat_NameEn (NameEnglish)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Danh mục nguyên liệu';