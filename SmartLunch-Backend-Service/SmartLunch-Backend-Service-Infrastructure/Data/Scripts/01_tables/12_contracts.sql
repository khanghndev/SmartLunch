-- =====================================================
-- Table: contracts
-- =====================================================
CREATE TABLE contracts (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    PartnerId INT NOT NULL,
    ContractNumber VARCHAR(100) NULL,
    Description VARCHAR(1000) NULL COMMENT 'Mô tả hợp đồng',
    SupplySchedule VARCHAR(500) NULL COMMENT 'Lịch giao hàng',
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    TotalValue DECIMAL(12,2) NULL,
    DepositAmount DECIMAL(12,2) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'active' COMMENT 'active|expired|cancelled',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY UK_contracts_code (Code),
    INDEX IX_contracts_partner (PartnerId),
    INDEX IX_contracts_status (Status),

    CONSTRAINT FK_contracts_partner FOREIGN KEY (PartnerId) REFERENCES partners (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Hợp đồng nhà cung cấp';
