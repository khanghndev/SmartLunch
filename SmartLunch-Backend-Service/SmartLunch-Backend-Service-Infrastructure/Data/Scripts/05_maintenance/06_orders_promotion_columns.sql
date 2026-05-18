-- Cột tổng tiền trước/sau KM trên đơn hàng. An toàn chạy nhiều lần.

DROP PROCEDURE IF EXISTS smartlunch_add_orders_promotion_columns;
DELIMITER //
CREATE PROCEDURE smartlunch_add_orders_promotion_columns()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'SubtotalAmount'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN SubtotalAmount DECIMAL(12,2) NULL COMMENT 'Tổng trước KM' AFTER TotalAmount;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'orders'
          AND COLUMN_NAME = 'DiscountAmount'
    ) THEN
        ALTER TABLE orders
            ADD COLUMN DiscountAmount DECIMAL(12,2) NOT NULL DEFAULT 0 COMMENT 'Tiền giảm' AFTER SubtotalAmount;
    END IF;
END //
DELIMITER ;

CALL smartlunch_add_orders_promotion_columns();
DROP PROCEDURE IF EXISTS smartlunch_add_orders_promotion_columns;
