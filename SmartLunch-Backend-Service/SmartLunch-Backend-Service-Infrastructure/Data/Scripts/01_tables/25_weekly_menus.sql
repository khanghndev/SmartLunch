CREATE TABLE weekly_menus (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    MenuType VARCHAR(50) NOT NULL DEFAULT 'General' COMMENT 'School | Worker | Office | Custom',
    EducationLevel VARCHAR(50) NULL COMMENT 'Mẫu giáo | Tiểu học | THCS | THPT (nếu là School)',
    Description VARCHAR(255) NULL COMMENT 'Mô tả thực đơn',
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_weekly_menus_code (Code),
    UNIQUE KEY UK_weekly_menus_dates_type (StartDate, EndDate, MenuType, EducationLevel),
    INDEX IX_weekly_menus_created_by (CreatedBy),
    CONSTRAINT FK_weekly_menus_user FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thực đơn tuần';
