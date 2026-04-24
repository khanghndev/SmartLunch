CREATE TABLE dishes (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    Name VARCHAR(255) NOT NULL COMMENT 'Tên món ăn',
    Description VARCHAR(255) NULL COMMENT 'Mô tả',
    Category VARCHAR(100) NULL COMMENT 'Danh mục',
    Price DECIMAL(10,2) NOT NULL COMMENT 'Giá mỗi phần',
    DietaryLabel VARCHAR(50) NULL COMMENT 'Nhãn dinh dưỡng',
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_dishes_code (Code),
    UNIQUE KEY UK_dishes_name (Name),
    INDEX IX_dishes_active_category (IsActive, Category)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Món ăn';
