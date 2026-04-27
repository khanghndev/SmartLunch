CREATE TABLE system_logs (
    Id INT NOT NULL AUTO_INCREMENT COMMENT 'Auto-increment Primary Key',
    Timestamp DATETIME,
    Level VARCHAR(50),
    Template TEXT,
    Message TEXT,
    Exception TEXT,
    Properties TEXT,
    PRIMARY KEY (Id)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Log hệ thống';