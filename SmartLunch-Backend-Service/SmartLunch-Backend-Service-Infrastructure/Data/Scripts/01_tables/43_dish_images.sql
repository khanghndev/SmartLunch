-- =====================================================
-- Table: dish_images
-- Description: Multiple images for a dish (cover + gallery)
-- =====================================================
CREATE TABLE dish_images (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    DishId INT NOT NULL,
    MediaFileId INT NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'gallery' COMMENT 'cover | gallery',
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_dish_images_code (Code),
    INDEX IX_dish_images_dish (DishId, SortOrder),
    INDEX IX_dish_images_media (MediaFileId),
    CONSTRAINT FK_dish_images_dish FOREIGN KEY (DishId) REFERENCES dishes (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_dish_images_media FOREIGN KEY (MediaFileId) REFERENCES media_files (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Ảnh món ăn';

