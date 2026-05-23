-- Gắn hợp đồng đặt suất với đơn nguồn (idempotent).

DROP PROCEDURE IF EXISTS smartlunch_contracts_add_source_order_id;
DELIMITER //
CREATE PROCEDURE smartlunch_contracts_add_source_order_id()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND COLUMN_NAME = 'SourceOrderId'
    ) THEN
        ALTER TABLE contracts
            ADD COLUMN SourceOrderId INT NULL COMMENT 'Đơn hàng tạo ra HĐ đặt suất' AFTER OrganizationId;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND INDEX_NAME = 'IX_contracts_source_order'
    ) THEN
        CREATE INDEX IX_contracts_source_order ON contracts (SourceOrderId);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'contracts'
          AND CONSTRAINT_NAME = 'FK_contracts_source_order'
    ) THEN
        ALTER TABLE contracts
            ADD CONSTRAINT FK_contracts_source_order FOREIGN KEY (SourceOrderId)
                REFERENCES orders (Id) ON DELETE SET NULL ON UPDATE CASCADE;
    END IF;
END //
DELIMITER ;

CALL smartlunch_contracts_add_source_order_id();
DROP PROCEDURE IF EXISTS smartlunch_contracts_add_source_order_id;
