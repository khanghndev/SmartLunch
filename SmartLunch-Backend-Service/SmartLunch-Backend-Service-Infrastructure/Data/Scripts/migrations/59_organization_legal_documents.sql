-- Tài liệu pháp lý do khách hàng doanh nghiệp tự upload (hồ sơ đơn vị).
CREATE TABLE IF NOT EXISTS organization_legal_documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrganizationId INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    DocumentType VARCHAR(50) NOT NULL DEFAULT 'other' COMMENT 'business_license|tax|authorization|other',
    Description VARCHAR(1000) NULL,
    MediaFileId INT NOT NULL,
    IssuedDate DATE NULL,
    ExpiryDate DATE NULL,
    UploadedByUserId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    INDEX IX_org_legal_docs_org (OrganizationId),
    INDEX IX_org_legal_docs_type (DocumentType),
    CONSTRAINT FK_org_legal_docs_org FOREIGN KEY (OrganizationId) REFERENCES organizations(Id) ON DELETE CASCADE,
    CONSTRAINT FK_org_legal_docs_media FOREIGN KEY (MediaFileId) REFERENCES media_files(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_org_legal_docs_user FOREIGN KEY (UploadedByUserId) REFERENCES users(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
