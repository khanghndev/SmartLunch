-- Bổ sung cột đối tác / hợp đồng cho DB cũ (idempotent).

DROP PROCEDURE IF EXISTS smartlunch_add_partner_contract_supplier_fields;
DELIMITER //
CREATE PROCEDURE smartlunch_add_partner_contract_supplier_fields()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'partners'
          AND COLUMN_NAME = 'BusinessRegistrationNumber'
    ) THEN
        ALTER TABLE partners
            ADD COLUMN BusinessRegistrationNumber VARCHAR(100) NULL AFTER LegalName;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'partners'
          AND COLUMN_NAME = 'LegalRepresentative'
    ) THEN
        ALTER TABLE partners
            ADD COLUMN LegalRepresentative VARCHAR(255) NULL AFTER TaxId;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND COLUMN_NAME = 'ContractNumber'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN ContractNumber VARCHAR(100) NULL AFTER PartnerId;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND COLUMN_NAME = 'SupplySchedule'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN SupplySchedule VARCHAR(500) NULL AFTER Description;
    END IF;

    -- Chuẩn hóa độ dài cột (an toàn chạy lại)
    ALTER TABLE partners
        MODIFY COLUMN ComplianceInfo VARCHAR(2000) NULL,
        MODIFY COLUMN FinancialTerms VARCHAR(1000) NULL;

    ALTER TABLE contracts
        MODIFY COLUMN Description VARCHAR(1000) NULL;
END //
DELIMITER ;

CALL smartlunch_add_partner_contract_supplier_fields();
DROP PROCEDURE IF EXISTS smartlunch_add_partner_contract_supplier_fields;
