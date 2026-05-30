CREATE TABLE complaint_evidence (
    Id INT NOT NULL AUTO_INCREMENT,
    ComplaintId INT NOT NULL,
    Kind VARCHAR(40) NOT NULL COMMENT 'receipt_photo|unboxing_video|portion_count_video|food_condition_video|other',
    MediaType VARCHAR(10) NOT NULL COMMENT 'image|video',
    StorageObjectName VARCHAR(500) NOT NULL,
    ContentType VARCHAR(100) NULL,
    FileSizeBytes BIGINT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_complaint_evidence_complaint (ComplaintId),
    CONSTRAINT FK_complaint_evidence_complaint FOREIGN KEY (ComplaintId) REFERENCES complaints (Id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC COMMENT = 'Bằng chứng khiếu nại (ảnh/video)';
