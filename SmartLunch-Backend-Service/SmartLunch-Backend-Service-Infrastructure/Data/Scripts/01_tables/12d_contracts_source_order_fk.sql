-- FK contracts.SourceOrderId → orders (sau khi bảng orders đã tạo)
ALTER TABLE contracts
    ADD INDEX IX_contracts_source_order (SourceOrderId),
    ADD CONSTRAINT FK_contracts_source_order FOREIGN KEY (SourceOrderId)
        REFERENCES orders (Id) ON DELETE SET NULL ON UPDATE CASCADE;
