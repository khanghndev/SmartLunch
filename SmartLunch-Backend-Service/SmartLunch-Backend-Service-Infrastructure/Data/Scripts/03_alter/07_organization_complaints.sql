-- Khiếu nại doanh nghiệp: bằng chứng, hoàn tiền theo suất, xác nhận người nhận (shipper)
DROP PROCEDURE IF EXISTS smartlunch_organization_complaints;
DELIMITER //
CREATE PROCEDURE smartlunch_organization_complaints()
BEGIN
    -- deliveries: xác nhận người nhận
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'RecipientConfirmedName'
    ) THEN
        ALTER TABLE deliveries
            ADD COLUMN RecipientConfirmedName VARCHAR(200) NULL COMMENT 'Tên người nhận xác nhận' AFTER Notes;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'RecipientConfirmationCode'
    ) THEN
        ALTER TABLE deliveries
            ADD COLUMN RecipientConfirmationCode VARCHAR(20) NULL COMMENT 'Mã OTP/chữ ký người nhận' AFTER RecipientConfirmedName;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'RecipientConfirmedAt'
    ) THEN
        ALTER TABLE deliveries
            ADD COLUMN RecipientConfirmedAt DATETIME NULL AFTER RecipientConfirmationCode;
    END IF;

    -- complaints: mở rộng
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'Reason'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN Reason VARCHAR(40) NULL COMMENT 'missing_portions|spoiled_rice|wrong_dish|food_quality' AFTER Description;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'MissingPortionCount'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN MissingPortionCount INT NULL COMMENT 'Số suất thiếu (khiếu nại thiếu suất)' AFTER Reason;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'Resolution'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN Resolution VARCHAR(20) NULL COMMENT 'refund|rejected' AFTER Status;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'ResolutionNote'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN ResolutionNote TEXT NULL AFTER Resolution;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'ResolvedByUserId'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN ResolvedByUserId INT NULL AFTER AssignedTo;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'SubmittedAt'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN SubmittedAt DATETIME NULL AFTER CreatedAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'ComplaintDeadlineAt'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN ComplaintDeadlineAt DATETIME NULL COMMENT 'DeliveredAt + 24h' AFTER SubmittedAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'RefundPortionCount'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN RefundPortionCount INT NULL AFTER ComplaintDeadlineAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'SuggestedRefundAmount'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN SuggestedRefundAmount DECIMAL(18,2) NULL AFTER RefundPortionCount;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'FinalRefundAmount'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN FinalRefundAmount DECIMAL(18,2) NULL AFTER SuggestedRefundAmount;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND CONSTRAINT_NAME = 'FK_complaints_resolved_by'
    ) THEN
        ALTER TABLE complaints
            ADD CONSTRAINT FK_complaints_resolved_by FOREIGN KEY (ResolvedByUserId) REFERENCES users(Id) ON DELETE SET NULL;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND COLUMN_NAME = 'RefundPaymentId'
    ) THEN
        ALTER TABLE complaints
            ADD COLUMN RefundPaymentId INT NULL COMMENT 'Payment hoàn tiền' AFTER FinalRefundAmount;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaints' AND CONSTRAINT_NAME = 'FK_complaints_refund_payment'
    ) THEN
        ALTER TABLE complaints
            ADD CONSTRAINT FK_complaints_refund_payment FOREIGN KEY (RefundPaymentId) REFERENCES payments(Id) ON DELETE SET NULL;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLES
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'complaint_evidence'
    ) THEN
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
            CONSTRAINT FK_complaint_evidence_complaint FOREIGN KEY (ComplaintId) REFERENCES complaints(Id) ON DELETE CASCADE ON UPDATE CASCADE
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
          COMMENT='Bằng chứng khiếu nại (ảnh/video)';
    END IF;
END //
DELIMITER ;

CALL smartlunch_organization_complaints();
DROP PROCEDURE IF EXISTS smartlunch_organization_complaints;
