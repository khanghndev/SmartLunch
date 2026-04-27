CREATE TABLE order_items (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    OrderId INT NOT NULL,
    DishId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(12,2) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_order_items_code (Code),
    INDEX IX_order_items_order (OrderId),
    INDEX IX_order_items_dish (DishId),
    CONSTRAINT FK_order_items_order FOREIGN KEY (OrderId) REFERENCES orders (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_order_items_dish FOREIGN KEY (DishId) REFERENCES dishes (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Chi tiết đơn hàng';
