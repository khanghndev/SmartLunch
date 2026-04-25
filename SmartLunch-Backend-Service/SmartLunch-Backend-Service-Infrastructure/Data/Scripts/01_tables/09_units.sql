CREATE TABLE units (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    Name VARCHAR(255) NOT NULL COMMENT 'Tên đơn vị',
    Address VARCHAR(255) NULL,
    Phone VARCHAR(50) NULL,
    ContactPerson VARCHAR(255) NULL,
    ContactEmail VARCHAR(255) NULL,
    UnitType VARCHAR(50) NOT NULL DEFAULT 'Office' COMMENT 'Office | Factory | School',
    IsSubscriptionActive TINYINT(1) NOT NULL DEFAULT 0,
    DefaultDailyMeals INT NOT NULL DEFAULT 0,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT NULL,
    UpdatedBy INT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_units_code (Code),
    UNIQUE KEY UK_units_name (Name),
    INDEX IX_units_active (IsActive),
    CONSTRAINT FK_units_created_by FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT FK_units_updated_by FOREIGN KEY (UpdatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Đơn vị đặt hàng';

