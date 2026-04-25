-- =====================================================
-- Table: news (Bản tin / Tin tức)
-- =====================================================
CREATE TABLE news (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(20) NULL COMMENT 'Mã tự sinh (trigger)',
    Title VARCHAR(255) NOT NULL COMMENT 'Tiêu đề bài viết',
    Slug VARCHAR(255) NOT NULL COMMENT 'Đường dẫn thân thiện',
    Summary TEXT NULL COMMENT 'Tóm tắt bài viết',
    Content LONGTEXT NOT NULL COMMENT 'Nội dung chi tiết',
    ThumbnailUrl VARCHAR(500) NULL COMMENT 'Ảnh đại diện bài viết',
    Category VARCHAR(100) NULL COMMENT 'Danh mục (Tin tức, Sự kiện, Khuyến mãi)',
    AuthorId INT NULL COMMENT 'Người viết bài',
    IsPublished TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Trạng thái xuất bản',
    PublishedAt DATETIME NULL COMMENT 'Thời điểm xuất bản',
    ViewCount INT NOT NULL DEFAULT 0 COMMENT 'Lượt xem',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY UK_news_code (Code),
    UNIQUE KEY UK_news_slug (Slug),
    INDEX IX_news_published (IsPublished, PublishedAt DESC),
    CONSTRAINT FK_news_author FOREIGN KEY (AuthorId) REFERENCES users (Id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Bản tin hệ thống';
