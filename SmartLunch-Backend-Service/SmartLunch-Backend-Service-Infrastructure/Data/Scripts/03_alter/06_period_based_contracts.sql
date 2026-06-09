-- Period-Based organization meal contracts (idempotent)
USE SmartLunch;

SET NAMES utf8mb4;

DROP PROCEDURE IF EXISTS smartlunch_period_based_contracts;
DELIMITER //
CREATE PROCEDURE smartlunch_period_based_contracts()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contracts' AND COLUMN_NAME = 'MealsPerDay'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN MealsPerDay INT NULL COMMENT 'Số suất ăn mỗi ngày phục vụ (HĐ theo kỳ)' AFTER MealUnitPrice;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contracts' AND COLUMN_NAME = 'LastWeeklyReminderWeekStart'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN LastWeeklyReminderWeekStart DATE NULL COMMENT 'Tuần (Thứ 2) đã gửi mail nhắc đặt món' AFTER DepositAmount;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contracts' AND COLUMN_NAME = 'WeeklyAutoFillWeekStart'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN WeeklyAutoFillWeekStart DATE NULL COMMENT 'Tuần (Thứ 2) đã auto random món chính' AFTER LastWeeklyReminderWeekStart;
    END IF;
END //
DELIMITER ;

CALL smartlunch_period_based_contracts();
DROP PROCEDURE IF EXISTS smartlunch_period_based_contracts;

CREATE TABLE IF NOT EXISTS contract_excluded_dates (
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
