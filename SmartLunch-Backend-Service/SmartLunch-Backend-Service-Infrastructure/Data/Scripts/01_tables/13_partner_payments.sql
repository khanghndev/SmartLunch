CREATE TABLE partner_payments (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    ContractId INT NOT NULL,
    PartnerId INT NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    Method VARCHAR(30) NOT NULL DEFAULT 'bank_transfer',
    Status VARCHAR(20) NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_partner_payments_code (Code),
    INDEX IX_partner_payments_contract (ContractId),
    INDEX IX_partner_payments_partner (PartnerId),
    CONSTRAINT FK_partner_payments_contract FOREIGN KEY (ContractId) REFERENCES contracts (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_partner_payments_partner FOREIGN KEY (PartnerId) REFERENCES partners (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thanh toán nhà cung cấp';
