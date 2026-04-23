CREATE TABLE chatbot_logs (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    UserId INT NULL,
    Message TEXT NOT NULL,
    Response TEXT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_chatbot_logs_code (Code),
    INDEX IX_chatbot_logs_user (UserId),
    INDEX IX_chatbot_logs_created (CreatedAt DESC),
    CONSTRAINT FK_chatbot_logs_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Lịch sử chatbot';
