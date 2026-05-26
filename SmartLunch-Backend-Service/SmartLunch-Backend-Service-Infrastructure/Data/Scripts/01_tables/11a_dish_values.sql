-- =====================================================
-- Table: dish_values
-- Mức giá suất ăn (định mức BOM theo từng mức giá)
-- =====================================================
CREATE TABLE dish_values (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    Amount DECIMAL(12,2) NOT NULL COMMENT 'Giá trị suất ăn (VND)',
    Label VARCHAR(100) NULL COMMENT 'Tên hiển thị',
    SortOrder INT NOT NULL DEFAULT 0,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_dish_values_code (Code),
    UNIQUE KEY UK_dish_values_amount (Amount),
    INDEX IX_dish_values_active (IsActive)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Mức giá suất ăn';
