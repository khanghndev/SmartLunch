CREATE TABLE payments (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    OrderId INT NOT NULL,
    PayerId INT NULL,
    PaymentDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Amount DECIMAL(12,2) NOT NULL,
    Method VARCHAR(30) NOT NULL DEFAULT 'cash',
    Status VARCHAR(20) NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_payments_code (Code),
    INDEX IX_payments_order (OrderId),
    INDEX IX_payments_payer (PayerId),
    CONSTRAINT FK_payments_order FOREIGN KEY (OrderId) REFERENCES orders (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_payments_payer FOREIGN KEY (PayerId) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thanh toán đơn hàng';
