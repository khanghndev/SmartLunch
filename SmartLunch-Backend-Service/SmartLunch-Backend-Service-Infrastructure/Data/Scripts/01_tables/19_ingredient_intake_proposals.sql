CREATE TABLE ingredient_intake_proposals (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    ProposalCode VARCHAR(40) NOT NULL,
    Status VARCHAR(20) NOT NULL,
    HeaderNote VARCHAR(500) NULL,
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ReviewedByUserId INT NULL,
    ReviewedAt DATETIME NULL,
    ReviewNote VARCHAR(500) NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_intake_proposals_code (Code),
    UNIQUE KEY UK_intake_proposals_proposal_code (ProposalCode),
    INDEX IX_intake_proposals_status (Status),
    INDEX IX_intake_proposals_creator (CreatedByUserId),
    CONSTRAINT FK_intake_proposals_creator FOREIGN KEY (CreatedByUserId) REFERENCES users (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_intake_proposals_reviewer FOREIGN KEY (ReviewedByUserId) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Phiếu đề xuất nhập nguyên liệu';
