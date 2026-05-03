-- Bảng danh mục slot bữa ăn (khớp enum AI / meal_structure).
-- Code: mã nghiệp vụ PREFIX+8 số (trigger fn_next_code). SlotKey: khóa logic main|side|... (API / junction dishes).
CREATE TABLE dish_categories (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    SlotKey VARCHAR(32) NOT NULL COMMENT 'main|side|soup|vegetable|noodle_soup|dessert — khớp AI',
    Name VARCHAR(100) NOT NULL COMMENT 'Tên hiển thị (VI)',
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_dish_categories_code (Code),
    UNIQUE KEY UK_dish_categories_slot_key (SlotKey)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Danh mục slot món (AI meal slot)';
