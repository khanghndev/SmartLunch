-- Hồ sơ công khai HuitMeal (chứng nhận, VSATTP, biên bản...) hiển thị trang Giới thiệu.
CREATE TABLE IF NOT EXISTS company_public_documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL COMMENT 'Tiêu đề hiển thị',
    DocumentType VARCHAR(50) NOT NULL DEFAULT 'other' COMMENT 'food_safety|iso|business_license|inspection|contract|other',
    Description VARCHAR(1000) NULL,
    MediaFileId INT NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    IsPublished TINYINT(1) NOT NULL DEFAULT 1,
    IssuedDate DATE NULL,
    ExpiryDate DATE NULL,
    CreatedByUserId INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    INDEX IX_company_public_documents_published (IsPublished, SortOrder),
    INDEX IX_company_public_documents_type (DocumentType),
    CONSTRAINT FK_company_public_documents_media FOREIGN KEY (MediaFileId) REFERENCES media_files(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_company_public_documents_user FOREIGN KEY (CreatedByUserId) REFERENCES users(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
