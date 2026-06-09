-- dish_values + FK contracts / dish_ingredients (idempotent)

DROP PROCEDURE IF EXISTS smartlunch_dish_values_migration;
DELIMITER //
CREATE PROCEDURE smartlunch_dish_values_migration()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLES
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'dish_values'
    ) THEN
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
        ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci
          COMMENT = 'Mức giá suất ăn';
    END IF;

    INSERT IGNORE INTO code_prefixes (TableName, Prefix, Description, LastSequence) VALUES
    ('dish_values', 'DVL', 'Mức giá suất ăn', 0);

    INSERT IGNORE INTO dish_values (Amount, Label, SortOrder, IsActive) VALUES
    (25000, 'Suất 25.000đ', 10, 1),
    (30000, 'Suất 30.000đ', 20, 1),
    (45000, 'Suất 45.000đ', 30, 1),
    (60000, 'Suất 60.000đ', 40, 1);

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND COLUMN_NAME = 'DishValueId'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN DishValueId INT NULL COMMENT 'FK dish_values — mức giá suất ăn của hợp đồng'
            AFTER TotalValue;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND INDEX_NAME = 'IX_contracts_dish_value'
    ) THEN
        CREATE INDEX IX_contracts_dish_value ON contracts (DishValueId);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND CONSTRAINT_NAME = 'FK_contracts_dish_value'
    ) THEN
        ALTER TABLE contracts
            ADD CONSTRAINT FK_contracts_dish_value FOREIGN KEY (DishValueId)
                REFERENCES dish_values (Id) ON DELETE RESTRICT ON UPDATE CASCADE;
    END IF;

    UPDATE contracts c
    INNER JOIN dish_values dv ON dv.Amount = 30000
    SET c.DishValueId = dv.Id
    WHERE c.OrganizationId IS NOT NULL
      AND c.DishValueId IS NULL
      AND c.MealUnitPrice IS NULL;

    UPDATE contracts c
    INNER JOIN dish_values dv ON dv.Amount = c.MealUnitPrice
    SET c.DishValueId = dv.Id
    WHERE c.OrganizationId IS NOT NULL
      AND c.DishValueId IS NULL
      AND c.MealUnitPrice IS NOT NULL;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'dish_ingredients'
          AND COLUMN_NAME = 'DishValueId'
    ) THEN
        ALTER TABLE dish_ingredients
            ADD COLUMN DishValueId INT NULL COMMENT 'FK dish_values — định mức theo mức giá suất ăn'
            AFTER IngredientId;
    END IF;

    UPDATE dish_ingredients di
    INNER JOIN dish_values dv ON dv.Amount = 30000
    SET di.DishValueId = dv.Id
    WHERE di.DishValueId IS NULL;

    IF EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'dish_ingredients'
          AND COLUMN_NAME = 'DishValueId'
          AND IS_NULLABLE = 'YES'
    ) THEN
        ALTER TABLE dish_ingredients MODIFY DishValueId INT NOT NULL;
    END IF;

    -- Thêm UK mới TRƯỚC khi xóa UK cũ (FK_dish_ingredients_dish dùng prefix DishId của UK cũ)
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'dish_ingredients'
          AND INDEX_NAME = 'UK_dish_ingredients_dish_ing_value'
    ) THEN
        ALTER TABLE dish_ingredients
            ADD UNIQUE KEY UK_dish_ingredients_dish_ing_value (DishId, IngredientId, DishValueId);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'dish_ingredients'
          AND INDEX_NAME = 'IX_dish_ingredients_dish_value'
    ) THEN
        CREATE INDEX IX_dish_ingredients_dish_value ON dish_ingredients (DishValueId);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'dish_ingredients'
          AND CONSTRAINT_NAME = 'FK_dish_ingredients_dish_value'
    ) THEN
        ALTER TABLE dish_ingredients
            ADD CONSTRAINT FK_dish_ingredients_dish_value FOREIGN KEY (DishValueId)
                REFERENCES dish_values (Id) ON DELETE RESTRICT ON UPDATE CASCADE;
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'dish_ingredients'
          AND INDEX_NAME = 'UK_dish_ingredients_dish_ing'
    ) THEN
        ALTER TABLE dish_ingredients DROP INDEX UK_dish_ingredients_dish_ing;
    END IF;
END //
DELIMITER ;

CALL smartlunch_dish_values_migration();
DROP PROCEDURE IF EXISTS smartlunch_dish_values_migration;
