-- =====================================================
-- Table: contracts
-- =====================================================
CREATE TABLE contracts (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    PartnerId INT NULL COMMENT 'Nhà cung cấp (nếu là hợp đồng mua)',
    OrganizationId INT NULL COMMENT 'Đơn vị khách hàng (nếu là hợp đồng bán)',
    
    ContractNumber VARCHAR(100) NULL,
    ContractType VARCHAR(50) NOT NULL DEFAULT 'Framework' COMMENT 'Framework | Order-Based',
    Description VARCHAR(1000) NULL COMMENT 'Mô tả hợp đồng',
    SupplySchedule VARCHAR(500) NULL COMMENT 'Lịch giao hàng',
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    TotalValue DECIMAL(12,2) NULL,
    MealUnitPrice DECIMAL(12,2) NULL COMMENT 'Giá thỏa thuận / suất (đơn vị đặt) — không dùng giá catalog Dish',
    DepositAmount DECIMAL(12,2) NULL,
    ContractFileUrl VARCHAR(500) NULL COMMENT 'Đường dẫn file hợp đồng',
    
    -- Digital Signature Info
    IsDigitallySigned TINYINT(1) NOT NULL DEFAULT 0,
    DigitalSignature LONGTEXT NULL COMMENT 'Dữ liệu chữ ký số',
    DigitallySignedAt DATETIME NULL,
    SignatureImage VARCHAR(500) NULL COMMENT 'Ảnh chữ ký (nếu có)',
    
    Status VARCHAR(20) NOT NULL DEFAULT 'active' COMMENT 'active|expired|cancelled|pending_signature',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY UK_contracts_code (Code),
    INDEX IX_contracts_partner (PartnerId),
    INDEX IX_contracts_org (OrganizationId),
    INDEX IX_contracts_status (Status),

    CONSTRAINT FK_contracts_partner FOREIGN KEY (PartnerId) REFERENCES partners (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_contracts_org FOREIGN KEY (OrganizationId) REFERENCES organizations (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Hợp đồng và Chữ ký số';
