CREATE TABLE deliveries (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    OrderId INT NOT NULL,
    AssignedStaffId INT NULL,
    DeliveryAddress VARCHAR(255) NOT NULL,
    DeliveryStatus VARCHAR(20) NOT NULL DEFAULT 'pending',
    DeliveredAt DATETIME NULL,
    ProofImageUrl VARCHAR(500) NULL,
    ProofCapturedAt DATETIME NULL,
    Notes VARCHAR(255) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_deliveries_code (Code),
    INDEX IX_deliveries_order (OrderId),
    INDEX IX_deliveries_staff (AssignedStaffId),
    CONSTRAINT FK_deliveries_order FOREIGN KEY (OrderId) REFERENCES orders (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_deliveries_staff FOREIGN KEY (AssignedStaffId) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Giao hàng';
