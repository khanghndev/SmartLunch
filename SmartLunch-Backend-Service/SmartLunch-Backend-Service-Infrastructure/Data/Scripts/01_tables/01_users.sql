-- =====================================================
-- Table: users
-- Description: User accounts for RBAC system
-- =====================================================
CREATE TABLE users (
    Id INT NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    Username VARCHAR(100) NOT NULL COMMENT 'Tên đăng nhập',
    Email VARCHAR(255) NOT NULL COMMENT 'Email',
    PasswordHash VARCHAR(500) NOT NULL COMMENT 'Mật khẩu đã hash (BCrypt/Argon2)',
    FirstName VARCHAR(100) NULL COMMENT 'Họ',
    LastName VARCHAR(100) NULL COMMENT 'Tên',
    PhoneNumber VARCHAR(20) NULL COMMENT 'Số điện thoại',
    AvatarUrl VARCHAR(500) NULL COMMENT 'Ảnh đại diện',
    Gender VARCHAR(10) NULL COMMENT 'Giới tính',
    BirthDate DATE NULL COMMENT 'Ngày sinh',
    Address VARCHAR(255) NULL COMMENT 'Địa chỉ',
    Provider VARCHAR(50) NOT NULL DEFAULT 'system' COMMENT 'Nhà cung cấp xác thực (system, google, facebook, firebase)',
    IsActive TINYINT(1) NOT NULL DEFAULT 1 COMMENT 'Trạng thái hoạt động',
    IsEmailVerified TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Email đã xác thực',
    EmailVerifiedAt DATETIME NULL COMMENT 'Thời điểm xác thực email',
    LastLoginAt DATETIME NULL COMMENT 'Đăng nhập lần cuối',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT NULL COMMENT 'Người tạo',
    UpdatedBy INT NULL COMMENT 'Người cập nhật',

    PRIMARY KEY (Id),
    UNIQUE KEY UK_users_code (Code),
    UNIQUE KEY UK_users_username (Username),
    UNIQUE KEY UK_users_email (Email),
    INDEX IX_users_active_created (IsActive, CreatedAt DESC),
    INDEX IX_users_provider (Provider),

    CONSTRAINT FK_users_created_by FOREIGN KEY (CreatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT FK_users_updated_by FOREIGN KEY (UpdatedBy) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Người dùng hệ thống';
