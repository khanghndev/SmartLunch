-- =====================================================
-- Table: banners (Quản lý Banner website)
-- =====================================================
CREATE TABLE banners (
    Id INT NOT NULL AUTO_INCREMENT,
    PageName VARCHAR(100) NOT NULL COMMENT 'Trang hiển thị (Home, Factory, etc.)',
    Title VARCHAR(255) NULL COMMENT 'Tiêu đề banner',
    Subtitle VARCHAR(500) NULL COMMENT 'Phụ đề banner',
    ImageUrl VARCHAR(500) NOT NULL COMMENT 'Đường dẫn ảnh banner',
    CtaLink VARCHAR(255) NULL COMMENT 'Đường dẫn khi click',
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    INDEX IX_banners_page_active (PageName, IsActive, DisplayOrder)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Banner quảng cáo';
