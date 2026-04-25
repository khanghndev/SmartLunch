-- =====================================================
-- Table: recruitment (Tuyển dụng)
-- =====================================================
CREATE TABLE recruitment (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    Title VARCHAR(255) NOT NULL COMMENT 'Tiêu đề tuyển dụng',
    Position VARCHAR(255) NOT NULL COMMENT 'Vị trí công việc',
    Location VARCHAR(255) NULL COMMENT 'Địa điểm làm việc',
    JobType VARCHAR(50) NOT NULL DEFAULT 'Full-time' COMMENT 'Full-time | Part-time | Freelance',
    SalaryRange VARCHAR(100) NULL COMMENT 'Mức lương (VD: 10-15 triệu)',
    Description TEXT NOT NULL COMMENT 'Mô tả công việc',
    Requirements TEXT NULL COMMENT 'Yêu cầu ứng viên',
    Benefits TEXT NULL COMMENT 'Quyền lợi',
    Deadline DATE NULL COMMENT 'Hạn chót ứng tuyển',
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY UK_recruitment_code (Code),
    INDEX IX_recruitment_active (IsActive, CreatedAt DESC)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thông tin tuyển dụng';
