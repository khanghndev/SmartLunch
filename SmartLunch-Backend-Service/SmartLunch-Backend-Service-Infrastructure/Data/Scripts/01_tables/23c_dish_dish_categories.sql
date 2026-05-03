-- Nhiều slot trên một món (vd phở: main + soup + vegetable). Thay thế cơ chế CoversCategoriesEnglish.
CREATE TABLE dish_dish_categories (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    DishId INT NOT NULL,
    DishCategoryId INT NOT NULL COMMENT 'FK dish_categories.Id',
    PRIMARY KEY (Id),
    UNIQUE KEY UK_dish_dish_categories_code (Code),
    UNIQUE KEY UK_dish_dish_categories_dish_cat (DishId, DishCategoryId),
    INDEX IX_dish_dish_categories_category (DishCategoryId),
    CONSTRAINT FK_ddc_dish FOREIGN KEY (DishId) REFERENCES dishes (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_ddc_category FOREIGN KEY (DishCategoryId) REFERENCES dish_categories (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Món — nhiều slot (AI covers)';
