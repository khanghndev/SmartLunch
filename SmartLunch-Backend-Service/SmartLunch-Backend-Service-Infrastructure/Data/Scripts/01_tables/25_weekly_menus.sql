CREATE TABLE weekly_menus (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    MenuType VARCHAR(50) NOT NULL DEFAULT 'Default' COMMENT 'School | Worker | Office | Custom',
    CustomerTypeId INT NULL COMMENT 'FK -> customer_types (org_*)',
    Description VARCHAR(255) NULL COMMENT 'Mô tả thực đơn',
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_weekly_menus_code (Code),
    UNIQUE KEY UK_weekly_menus_dates_type (StartDate, EndDate, MenuType, CustomerTypeId),
    INDEX IX_weekly_menus_created_by (CreatedBy),
    INDEX IX_weekly_menus_customer_type (CustomerTypeId),
    CONSTRAINT FK_weekly_menus_user FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_weekly_menus_customer_type FOREIGN KEY (CustomerTypeId) REFERENCES customer_types (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thực đơn tuần';
