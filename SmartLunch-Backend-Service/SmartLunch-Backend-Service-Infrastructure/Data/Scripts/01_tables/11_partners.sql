-- =====================================================
-- Table: partners (Nhà cung cấp)
-- =====================================================
CREATE TABLE partners (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    LegalName VARCHAR(255) NOT NULL COMMENT 'Tên pháp lý',
    BusinessRegistrationNumber VARCHAR(100) NULL COMMENT 'Số ĐKKD',
    TaxId VARCHAR(50) NULL COMMENT 'Mã số thuế',
    LegalRepresentative VARCHAR(255) NULL COMMENT 'Người đại diện pháp luật',
    Address VARCHAR(255) NULL,
    ContactPerson VARCHAR(255) NULL,
    Phone VARCHAR(50) NULL,
    Email VARCHAR(255) NULL,
    PerformanceRating DECIMAL(3,2) NULL,
    ComplianceInfo VARCHAR(2000) NULL COMMENT 'Thông tin tuân thủ',
    FinancialTerms VARCHAR(1000) NULL COMMENT 'Điều khoản thanh toán',
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY UK_partners_code (Code),
    UNIQUE KEY UK_partners_tax_id (TaxId),
    INDEX IX_partners_active (IsActive)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Nhà cung cấp';
