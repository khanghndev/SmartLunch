CREATE TABLE ingredient_sources (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    IngredientId INT NOT NULL,
    PartnerId INT NULL,
    BatchNumber VARCHAR(50) NULL,
    OriginDetails VARCHAR(255) NULL,
    ProductionDate DATE NULL,
    ExpirationDate DATE NULL,
    Certification VARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_ingredient_sources_code (Code),
    INDEX IX_ingredient_sources_ingredient (IngredientId),
    INDEX IX_ingredient_sources_partner (PartnerId),
    CONSTRAINT FK_ingredient_sources_ingredient FOREIGN KEY (IngredientId) REFERENCES ingredients (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_ingredient_sources_partner FOREIGN KEY (PartnerId) REFERENCES partners (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Nguồn gốc lô nguyên liệu';
