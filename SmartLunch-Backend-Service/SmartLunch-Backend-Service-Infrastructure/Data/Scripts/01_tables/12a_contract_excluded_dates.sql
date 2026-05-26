-- =====================================================
-- Table: contract_excluded_dates
-- =====================================================
CREATE TABLE contract_excluded_dates (
    Id INT NOT NULL AUTO_INCREMENT,
    ContractId INT NOT NULL,
    ExcludedDate DATE NOT NULL COMMENT 'Ngày không cung cấp suất trong thời hạn HĐ',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_contract_excluded (ContractId, ExcludedDate),
    INDEX IX_contract_excluded_contract (ContractId),
    CONSTRAINT FK_contract_excluded_contract
        FOREIGN KEY (ContractId) REFERENCES contracts (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci
  COMMENT = 'Ngày loại trừ khỏi lịch cung cấp suất (HĐ Period-Based)';
