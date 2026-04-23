CREATE TABLE menu_schedule (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    MenuId INT NOT NULL,
    Date DATE NOT NULL,
    MealSlot VARCHAR(20) NOT NULL DEFAULT 'lunch',
    DishId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_menu_schedule_code (Code),
    UNIQUE KEY UK_menu_schedule_combo (MenuId, Date, MealSlot, DishId),
    INDEX IX_menu_schedule_dish (DishId),
    INDEX IX_menu_schedule_date (Date),
    CONSTRAINT FK_menu_schedule_menu FOREIGN KEY (MenuId) REFERENCES weekly_menus (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_menu_schedule_dish FOREIGN KEY (DishId) REFERENCES dishes (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Lịch thực đơn theo ngày';
