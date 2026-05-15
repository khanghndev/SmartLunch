-- Thêm giá / suất thỏa thuận trên hợp đồng (đặt suất đơn vị).
ALTER TABLE contracts
    ADD COLUMN MealUnitPrice DECIMAL(12,2) NULL COMMENT 'Giá thỏa thuận / suất (đơn vị đặt)' AFTER TotalValue;
