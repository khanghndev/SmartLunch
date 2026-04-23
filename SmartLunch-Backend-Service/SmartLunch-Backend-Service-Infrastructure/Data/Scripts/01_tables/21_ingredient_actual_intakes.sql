CREATE TABLE ingredient_actual_intakes (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    ReceiptCode VARCHAR(40) NOT NULL,
    ProposalId INT NOT NULL,
    CreatedByUserId INT NOT NULL,
    ReceivedAt DATETIME NOT NULL,
    Note VARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_actual_intakes_code (Code),
    UNIQUE KEY UK_actual_intakes_receipt (ReceiptCode),
    UNIQUE KEY UK_actual_intakes_proposal (ProposalId),
    CONSTRAINT FK_actual_intakes_proposal FOREIGN KEY (ProposalId) REFERENCES ingredient_intake_proposals (Id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT FK_actual_intakes_user FOREIGN KEY (CreatedByUserId) REFERENCES users (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Phiếu nhập kho thực tế';
