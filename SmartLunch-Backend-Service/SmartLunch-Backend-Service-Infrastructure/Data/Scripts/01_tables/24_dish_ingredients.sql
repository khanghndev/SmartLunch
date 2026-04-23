CREATE TABLE dish_ingredients (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    DishId INT NOT NULL,
    IngredientId INT NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    Unit VARCHAR(20) NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_dish_ingredients_code (Code),
    UNIQUE KEY UK_dish_ingredients_dish_ing (DishId, IngredientId),
    INDEX IX_dish_ingredients_ingredient (IngredientId),
    CONSTRAINT FK_dish_ingredients_dish FOREIGN KEY (DishId) REFERENCES dishes (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_dish_ingredients_ingredient FOREIGN KEY (IngredientId) REFERENCES ingredients (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Nguyên liệu của món ăn';
