CREATE TABLE ingredient_actual_intake_lines (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    IntakeId INT NOT NULL,
    IngredientId INT NOT NULL,
    Quantity DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_actual_intake_lines_code (Code),
    INDEX IX_actual_intake_lines_intake (IntakeId),
    INDEX IX_actual_intake_lines_ingredient (IngredientId),
    CONSTRAINT FK_actual_intake_lines_intake FOREIGN KEY (IntakeId) REFERENCES ingredient_actual_intakes (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_actual_intake_lines_ingredient FOREIGN KEY (IngredientId) REFERENCES ingredients (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Chi tiết phiếu nhập kho';
