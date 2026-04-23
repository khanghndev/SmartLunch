CREATE TABLE complaints (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    UserId INT NOT NULL,
    OrderId INT NULL,
    Title VARCHAR(255) NOT NULL,
    Description TEXT NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'new',
    AssignedTo INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ResolvedAt DATETIME NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_complaints_code (Code),
    INDEX IX_complaints_user (UserId),
    INDEX IX_complaints_status (Status),
    CONSTRAINT FK_complaints_user FOREIGN KEY (UserId) REFERENCES users (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_complaints_order FOREIGN KEY (OrderId) REFERENCES orders (Id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT FK_complaints_assignee FOREIGN KEY (AssignedTo) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Khiếu nại khách hàng';
