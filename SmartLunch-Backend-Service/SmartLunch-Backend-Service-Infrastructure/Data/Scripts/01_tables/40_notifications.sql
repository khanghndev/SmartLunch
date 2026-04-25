-- =====================================================
-- Table: notifications (Thông báo hệ thống)
-- =====================================================
CREATE TABLE notifications (
    Id INT NOT NULL AUTO_INCREMENT,
    UserId INT NOT NULL COMMENT 'Người nhận',
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    Type VARCHAR(50) NOT NULL DEFAULT 'info' COMMENT 'info | warning | success | error',
    IsRead TINYINT(1) NOT NULL DEFAULT 0,
    Link VARCHAR(255) NULL COMMENT 'Đường dẫn khi click vào thông báo',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    INDEX IX_notifications_user_read (UserId, IsRead, CreatedAt DESC),
    CONSTRAINT FK_notifications_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Thông báo người dùng';
