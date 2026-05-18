-- Tạo bảng khuyến mãi nếu DB cũ chưa có (an toàn chạy nhiều lần).

CREATE TABLE IF NOT EXISTS promotions (
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(40) NULL COMMENT 'Mã KM (tùy chọn, nhập khi đặt hàng)',
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(500) NULL,
    ScopeType VARCHAR(30) NOT NULL COMMENT 'dish|order_quantity|customer',
    DiscountType VARCHAR(20) NOT NULL COMMENT 'percent|fixed_amount',
    DiscountValue DECIMAL(12,2) NOT NULL,
    Priority INT NOT NULL DEFAULT 0 COMMENT 'Cao hơn = kiểm tra trước',
    SelectionMode VARCHAR(20) NOT NULL DEFAULT 'best_discount' COMMENT 'first_match|best_discount',
    Channel VARCHAR(20) NOT NULL DEFAULT 'all' COMMENT 'all|b2c|b2b_org',
    MinOrderQuantity INT NULL COMMENT 'Áp dụng scope order_quantity / điều kiện phụ',
    MinOrderAmount DECIMAL(12,2) NULL,
    ValidFrom DATE NOT NULL,
    ValidTo DATE NOT NULL,
    BookingTimeStart TIME NULL COMMENT 'Khung giờ đặt (VN) — bắt đầu',
    BookingTimeEnd TIME NULL COMMENT 'Khung giờ đặt (VN) — kết thúc',
    MaxTotalUses INT NULL,
    MaxUsesPerUser INT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE KEY UK_promotions_code (Code),
    INDEX IX_promotions_active_window (IsActive, ValidFrom, ValidTo),
    INDEX IX_promotions_priority (Priority DESC)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci COMMENT = 'Cấu hình khuyến mãi';

CREATE TABLE IF NOT EXISTS promotion_targets (
    Id INT NOT NULL AUTO_INCREMENT,
    PromotionId INT NOT NULL,
    TargetType VARCHAR(30) NOT NULL COMMENT 'dish|organization|contract_type',
    TargetId INT NULL,
    TargetKey VARCHAR(50) NULL COMMENT 'VD: Order-Based, Frame',
    PRIMARY KEY (Id),
    INDEX IX_promotion_targets_promotion (PromotionId),
    CONSTRAINT FK_promotion_targets_promotion FOREIGN KEY (PromotionId) REFERENCES promotions (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci COMMENT = 'Đối tượng áp dụng KM (món, đơn vị, loại HĐ)';

CREATE TABLE IF NOT EXISTS order_promotion_applications (
    Id INT NOT NULL AUTO_INCREMENT,
    OrderId INT NOT NULL,
    PromotionId INT NOT NULL,
    PromotionCode VARCHAR(40) NULL,
    PromotionName VARCHAR(200) NOT NULL,
    ScopeType VARCHAR(30) NOT NULL,
    DiscountType VARCHAR(20) NOT NULL,
    DiscountValue DECIMAL(12,2) NOT NULL,
    SubtotalBefore DECIMAL(12,2) NOT NULL,
    DiscountAmount DECIMAL(12,2) NOT NULL,
    TotalAfter DECIMAL(12,2) NOT NULL,
    SnapshotJson TEXT NULL COMMENT 'JSON đối soát',
    AppliedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_order_promo_order (OrderId),
    INDEX IX_order_promo_promotion (PromotionId),
    CONSTRAINT FK_order_promo_order FOREIGN KEY (OrderId) REFERENCES orders (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_order_promo_promotion FOREIGN KEY (PromotionId) REFERENCES promotions (Id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci COMMENT = 'KM đã áp dụng trên đơn';
