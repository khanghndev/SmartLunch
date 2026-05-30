-- OTP xác nhận người nhận khi giao hàng (Organization xem, Shipper nhập khi upload proof)
DROP PROCEDURE IF EXISTS smartlunch_delivery_otp;
DELIMITER //
CREATE PROCEDURE smartlunch_delivery_otp()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'DeliveryOtp'
    ) THEN
        ALTER TABLE deliveries
            ADD COLUMN DeliveryOtp VARCHAR(6) NULL COMMENT 'Mã OTP 6 số cho người nhận' AFTER RecipientConfirmedAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'deliveries' AND COLUMN_NAME = 'DeliveryOtpExpiresAt'
    ) THEN
        ALTER TABLE deliveries
            ADD COLUMN DeliveryOtpExpiresAt DATETIME NULL AFTER DeliveryOtp;
    END IF;
END //
DELIMITER ;

CALL smartlunch_delivery_otp();
DROP PROCEDURE IF EXISTS smartlunch_delivery_otp;
