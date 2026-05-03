-- Danh mục phương pháp chế biến (khớp enum AI CookingMethod).
-- Code: mã nghiệp vụ (trigger). MethodKey: fried|stewed|boiled|… — FK từ dishes.CookingMethodId.
CREATE TABLE cooking_methods (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    MethodKey VARCHAR(32) NOT NULL COMMENT 'fried|stewed|boiled|stir_fried|grilled|steamed|raw',
    Name VARCHAR(100) NOT NULL COMMENT 'Tên hiển thị (VI)',
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_cooking_methods_code (Code),
    UNIQUE KEY UK_cooking_methods_method_key (MethodKey)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Phương pháp chế biến (AI)';
