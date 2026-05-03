-- =====================================================
-- Table: partner_documents
-- Description: Partner documents (business license, tax cert, etc.) - private by default
-- =====================================================
CREATE TABLE partner_documents (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    PartnerId INT NOT NULL,
    MediaFileId INT NOT NULL,
    DocumentType VARCHAR(50) NOT NULL DEFAULT 'other' COMMENT 'business_license | tax_certificate | other',
    IsVerified TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_partner_documents_code (Code),
    INDEX IX_partner_documents_partner (PartnerId, CreatedAt),
    INDEX IX_partner_documents_media (MediaFileId),
    CONSTRAINT FK_partner_documents_partner FOREIGN KEY (PartnerId) REFERENCES partners (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_partner_documents_media FOREIGN KEY (MediaFileId) REFERENCES media_files (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Tài liệu doanh nghiệp';

