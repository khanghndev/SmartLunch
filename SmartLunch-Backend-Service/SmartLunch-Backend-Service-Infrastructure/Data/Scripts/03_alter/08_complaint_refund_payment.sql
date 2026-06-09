-- Liên kết khiếu nại với payment hoàn tiền
DROP PROCEDURE IF EXISTS smartlunch_complaint_refund_payment;
DELIMITER //
CREATE PROCEDURE smartlunch_complaint_refund_payment()
BEGIN
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
END //
DELIMITER ;

CALL smartlunch_complaint_refund_payment();
DROP PROCEDURE IF EXISTS smartlunch_complaint_refund_payment;
