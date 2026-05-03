CREATE TABLE menu_suggestion_plans (
    Id INT NOT NULL AUTO_INCREMENT,
    MenuSuggestionId INT NOT NULL,
    `Rank` INT NOT NULL,
    PlanScore DECIMAL(6,3) NOT NULL,
    ObjectiveValue DECIMAL(18,3) NOT NULL,
    PRIMARY KEY (Id),
    INDEX IX_menu_suggestion_plans_suggestion (MenuSuggestionId),
    UNIQUE KEY UK_menu_suggestion_plans_suggestion_rank (MenuSuggestionId, `Rank`),
    CONSTRAINT FK_menu_suggestion_plans_suggestion FOREIGN KEY (MenuSuggestionId)
        REFERENCES menu_suggestions (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Các phương án (top-K) cho 1 lần gợi ý thực đơn';

