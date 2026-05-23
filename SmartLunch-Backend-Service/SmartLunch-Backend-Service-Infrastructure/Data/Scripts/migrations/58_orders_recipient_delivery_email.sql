-- Thông tin người nhận / giao hàng trên đơn + theo dõi email (idempotent).

DROP PROCEDURE IF EXISTS smartlunch_orders_recipient_delivery_email;
DELIMITER //
CREATE PROCEDURE smartlunch_orders_recipient_delivery_email()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'RecipientName'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN RecipientName VARCHAR(120) NULL COMMENT 'Tên người nhận' AFTER AnnexSignedAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'RecipientPhone'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN RecipientPhone VARCHAR(20) NULL COMMENT 'SĐT người nhận' AFTER RecipientName;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'RecipientEmail'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN RecipientEmail VARCHAR(254) NULL COMMENT 'Email nhận thông báo' AFTER RecipientPhone;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'DeliveryAddress'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN DeliveryAddress VARCHAR(500) NULL COMMENT 'Địa chỉ giao hàng' AFTER RecipientEmail;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'DeliveryWardDistrict'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN DeliveryWardDistrict VARCHAR(255) NULL COMMENT 'Phường/xã, quận/huyện' AFTER DeliveryAddress;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'DeliveryNotes'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN DeliveryNotes VARCHAR(500) NULL COMMENT 'Ghi chú giao hàng' AFTER DeliveryWardDistrict;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'PreferredDeliveryTime'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN PreferredDeliveryTime VARCHAR(32) NULL COMMENT 'Giờ giao mong muốn (HH:mm)' AFTER DeliveryNotes;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'OrderConfirmationEmailSentAt'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN OrderConfirmationEmailSentAt DATETIME NULL COMMENT 'Đã gửi email xác nhận đơn' AFTER PreferredDeliveryTime;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'PaymentReminderSentAt'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN PaymentReminderSentAt DATETIME NULL COMMENT 'Đã gửi email nhắc thanh toán' AFTER OrderConfirmationEmailSentAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND INDEX_NAME = 'IX_orders_recipient_email'
    ) THEN
        CREATE INDEX IX_orders_recipient_email ON orders (RecipientEmail);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND INDEX_NAME = 'IX_orders_payment_reminder'
    ) THEN
        CREATE INDEX IX_orders_payment_reminder ON orders (PaymentStatus, PaymentReminderSentAt);
    END IF;
END //
DELIMITER ;

CALL smartlunch_orders_recipient_delivery_email();
DROP PROCEDURE IF EXISTS smartlunch_orders_recipient_delivery_email;
