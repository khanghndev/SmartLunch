CREATE TABLE ingredient_intake_proposal_lines (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'M� t? sinh (trigger)',
    ProposalId INT NOT NULL,
    IngredientId INT NOT NULL,
    Quantity DECIMAL(12,2) NOT NULL,
    LineNote VARCHAR(255) NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_intake_proposal_lines_code (Code),
    INDEX IX_intake_proposal_lines_proposal (ProposalId),
    INDEX IX_intake_proposal_lines_ingredient (IngredientId),
    CONSTRAINT FK_intake_proposal_lines_proposal FOREIGN KEY (ProposalId) REFERENCES ingredient_intake_proposals (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_intake_proposal_lines_ingredient FOREIGN KEY (IngredientId) REFERENCES ingredients (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Chi tiết phiếu đề xuất nhập';
