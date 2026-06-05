-- Suất ăn tuần theo HĐ Period-Based (một HĐ — nhiều tuần, không tạo HĐ mới)
CREATE TABLE IF NOT EXISTS contract_weekly_selections (
    Id INT NOT NULL AUTO_INCREMENT,
    ContractId INT NOT NULL,
    WeekMonday DATE NOT NULL COMMENT 'Thứ 2 của tuần phục vụ',
    Status VARCHAR(20) NOT NULL DEFAULT 'pending' COMMENT 'pending | selected | auto_filled',
    FulfillmentOrderId INT NULL COMMENT 'Đơn fulfillment tuần (kitchen/giao hàng)',
    SelectedAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_weekly_sel_contract_week (ContractId, WeekMonday),
    INDEX IX_weekly_sel_status (Status),
    CONSTRAINT FK_weekly_sel_contract
        FOREIGN KEY (ContractId) REFERENCES contracts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_weekly_sel_order
        FOREIGN KEY (FulfillmentOrderId) REFERENCES orders(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS contract_weekly_selection_items (
    Id INT NOT NULL AUTO_INCREMENT,
    WeeklySelectionId INT NOT NULL,
    ServiceDate DATE NOT NULL,
    DishId INT NOT NULL,
    Quantity INT NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_weekly_item (WeeklySelectionId, ServiceDate, DishId),
    CONSTRAINT FK_weekly_item_selection
        FOREIGN KEY (WeeklySelectionId) REFERENCES contract_weekly_selections(Id) ON DELETE CASCADE,
    CONSTRAINT FK_weekly_item_dish
        FOREIGN KEY (DishId) REFERENCES dishes(Id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
