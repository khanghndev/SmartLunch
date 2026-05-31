-- Số suất ăn theo từng ngày phục vụ (Period-Based), khi khác MealsPerDay mặc định
CREATE TABLE IF NOT EXISTS contract_daily_meal_portions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ContractId INT NOT NULL,
    ServiceDate DATE NOT NULL COMMENT 'Ngày phục vụ trong thời hạn HĐ',
    MealCount INT NOT NULL COMMENT 'Số suất ăn ngày đó',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY UK_contract_daily_meal (ContractId, ServiceDate),
    CONSTRAINT FK_contract_daily_meal_contract
        FOREIGN KEY (ContractId) REFERENCES contracts(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
