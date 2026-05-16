-- Ngày giao suất trên từng dòng đơn (đặt suất nhiều ngày). An toàn chạy nhiều lần.

DROP PROCEDURE IF EXISTS smartlunch_add_order_items_service_date;
DELIMITER //
CREATE PROCEDURE smartlunch_add_order_items_service_date()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'order_items'
          AND COLUMN_NAME = 'ServiceDate'
    ) THEN
        ALTER TABLE order_items
            ADD COLUMN ServiceDate DATE NULL COMMENT 'Ngày giao suất (đơn nhiều ngày)' AFTER Quantity;
    END IF;
END //
DELIMITER ;

CALL smartlunch_add_order_items_service_date();
DROP PROCEDURE IF EXISTS smartlunch_add_order_items_service_date;
