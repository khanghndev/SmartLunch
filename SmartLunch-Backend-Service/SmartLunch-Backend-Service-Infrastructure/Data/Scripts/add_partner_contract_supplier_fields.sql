-- MySQL: bổ sung cột cho nghiệp vụ đối tác cung cấp suất ăn (PascalCase theo schema db.txt / EF).
-- Chạy trên DB đã tồn tại. Nếu cột đã có, bỏ dòng tương ứng.

ALTER TABLE partners
    ADD COLUMN BusinessRegistrationNumber VARCHAR(100) NULL AFTER LegalName,
    ADD COLUMN LegalRepresentative VARCHAR(255) NULL AFTER TaxId;

ALTER TABLE partners
    MODIFY COLUMN ComplianceInfo VARCHAR(2000) NULL,
    MODIFY COLUMN FinancialTerms VARCHAR(1000) NULL;

ALTER TABLE contracts
    ADD COLUMN ContractNumber VARCHAR(100) NULL AFTER PartnerId,
    ADD COLUMN SupplySchedule VARCHAR(500) NULL AFTER Description;

ALTER TABLE contracts
    MODIFY COLUMN Description VARCHAR(1000) NULL;
