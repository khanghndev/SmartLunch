-- =====================================================
-- Seed: Stock Issues
-- =====================================================
SET @staff_id = (SELECT Id FROM users WHERE Username = 'nhanvien_kho');

INSERT INTO internal_stock_issues (IssueCode, IssuedAt, Reason, CreatedByUserId) VALUES
('PXK-20250321-A1B2', '2025-03-21 06:00:00', 'Xuất kho nguyên liệu bếp trưa 21/03', @staff_id),
('PXK-20250322-E5F6', '2025-03-22 06:00:00', 'Xuất kho nguyên liệu bếp trưa 22/03', @staff_id);

INSERT INTO internal_stock_issue_lines (IssueId, IngredientId, Quantity)
SELECT (SELECT Id FROM internal_stock_issues WHERE IssueCode = 'PXK-20250321-A1B2'), Id, 10 FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO internal_stock_issue_lines (IssueId, IngredientId, Quantity)
SELECT (SELECT Id FROM internal_stock_issues WHERE IssueCode = 'PXK-20250321-A1B2'), Id, 5 FROM ingredients WHERE Name = 'Rau muống';
INSERT INTO internal_stock_issue_lines (IssueId, IngredientId, Quantity)
SELECT (SELECT Id FROM internal_stock_issues WHERE IssueCode = 'PXK-20250321-A1B2'), Id, 30 FROM ingredients WHERE Name = 'Gạo ST25';
INSERT INTO internal_stock_issue_lines (IssueId, IngredientId, Quantity)
SELECT (SELECT Id FROM internal_stock_issues WHERE IssueCode = 'PXK-20250322-E5F6'), Id, 8 FROM ingredients WHERE Name = 'Thịt gà ta';
INSERT INTO internal_stock_issue_lines (IssueId, IngredientId, Quantity)
SELECT (SELECT Id FROM internal_stock_issues WHERE IssueCode = 'PXK-20250322-E5F6'), Id, 5 FROM ingredients WHERE Name = 'Cà chua';
