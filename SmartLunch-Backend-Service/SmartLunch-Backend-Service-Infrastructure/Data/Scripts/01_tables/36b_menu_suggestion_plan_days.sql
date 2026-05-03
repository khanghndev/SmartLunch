CREATE TABLE menu_suggestion_plan_days (
    Id INT NOT NULL AUTO_INCREMENT,
    MenuSuggestionPlanId INT NOT NULL,
    DayIndex TINYINT NOT NULL,
    DayName VARCHAR(20) NOT NULL,
    PRIMARY KEY (Id),
    INDEX IX_menu_suggestion_plan_days_plan (MenuSuggestionPlanId),
    UNIQUE KEY UK_menu_suggestion_plan_days_plan_dayindex (MenuSuggestionPlanId, DayIndex),
    CONSTRAINT FK_menu_suggestion_plan_days_plan FOREIGN KEY (MenuSuggestionPlanId)
        REFERENCES menu_suggestion_plans (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Các ngày trong 1 phương án thực đơn';

