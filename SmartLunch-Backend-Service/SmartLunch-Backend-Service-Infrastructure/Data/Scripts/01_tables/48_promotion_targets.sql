CREATE TABLE promotion_targets (
    Id INT NOT NULL AUTO_INCREMENT,
    PromotionId INT NOT NULL,
    TargetType VARCHAR(30) NOT NULL COMMENT 'dish|organization|contract_type',
    TargetId INT NULL,
    TargetKey VARCHAR(50) NULL COMMENT 'VD: Order-Based, Frame',
    PRIMARY KEY (Id),
    INDEX IX_promotion_targets_promotion (PromotionId),
    CONSTRAINT FK_promotion_targets_promotion FOREIGN KEY (PromotionId) REFERENCES promotions (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci COMMENT = 'Đối tượng áp dụng KM (món, đơn vị, loại HĐ)';
