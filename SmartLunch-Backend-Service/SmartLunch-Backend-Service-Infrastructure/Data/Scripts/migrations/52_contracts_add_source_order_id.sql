-- Liên kết hợp đồng Order-Based với đơn đặt suất tạo ra (phục vụ PDF gộp & quản lý).
ALTER TABLE contracts
    ADD COLUMN SourceOrderId INT NULL COMMENT 'Đơn hàng nguồn (đặt suất đơn vị)' AFTER OrganizationId;

ALTER TABLE contracts
    ADD INDEX IX_contracts_source_order (SourceOrderId);
