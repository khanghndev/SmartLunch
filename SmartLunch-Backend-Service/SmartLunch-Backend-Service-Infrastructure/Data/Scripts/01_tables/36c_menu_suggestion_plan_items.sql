CREATE TABLE menu_suggestion_plan_items (
    Id INT NOT NULL AUTO_INCREMENT,
    MenuSuggestionPlanDayId INT NOT NULL,
    SlotCategory VARCHAR(20) NOT NULL COMMENT 'main/side/soup/dessert',
    DishName VARCHAR(255) NOT NULL,
    DishSourceCategory VARCHAR(20) NULL COMMENT 'category gốc của dish (nếu có)',
    Score DECIMAL(6,3) NOT NULL,
    CostPerServing DECIMAL(10,2) NOT NULL,
    ReasonsJson JSON NULL COMMENT 'Danh sách lý do lựa chọn (JSON array of strings)',
    PRIMARY KEY (Id),
    INDEX IX_menu_suggestion_plan_items_day (MenuSuggestionPlanDayId),
    INDEX IX_menu_suggestion_plan_items_slot (SlotCategory),
    CONSTRAINT FK_menu_suggestion_plan_items_day FOREIGN KEY (MenuSuggestionPlanDayId)
        REFERENCES menu_suggestion_plan_days (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Các món/slot được chọn trong 1 ngày của phương án';

