-- =====================================================
-- Seed: Intake Proposals & Actual Intakes
-- =====================================================
SET @staff_id = (SELECT Id FROM users WHERE Username = 'nhanvien_kho');
SET @admin_id = (SELECT Id FROM users WHERE Username = 'admin');

INSERT INTO ingredient_intake_proposals (ProposalCode, Status, HeaderNote, CreatedByUserId, CreatedAt, ReviewedByUserId, ReviewedAt, ReviewNote) VALUES
('DXN-20250324-A1B2', 'fulfilled', 'Đề xuất nhập thịt tuần 4/3', @staff_id, DATE_SUB(NOW(), INTERVAL 5 DAY), @admin_id, DATE_SUB(NOW(), INTERVAL 4 DAY), 'Duyệt'),
('DXN-20250325-E5F6', 'approved', 'Đề xuất nhập rau củ tuần 4/3', @staff_id, DATE_SUB(NOW(), INTERVAL 3 DAY), @admin_id, DATE_SUB(NOW(), INTERVAL 2 DAY), 'Duyệt'),
('DXN-20250326-I9J0', 'submitted', 'Đề xuất nhập gia vị tháng 4', @staff_id, DATE_SUB(NOW(), INTERVAL 1 DAY), NULL, NULL, NULL);

-- Proposal lines
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250324-A1B2'), Id, 30, 'Tươi trong ngày' FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250324-A1B2'), Id, 20, NULL FROM ingredients WHERE Name = 'Thịt gà ta';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250325-E5F6'), Id, 20, NULL FROM ingredients WHERE Name = 'Rau muống';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250325-E5F6'), Id, 15, NULL FROM ingredients WHERE Name = 'Cà chua';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250326-I9J0'), Id, 10, NULL FROM ingredients WHERE Name = 'Nước mắm';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250326-I9J0'), Id, 12, NULL FROM ingredients WHERE Name = 'Dầu thực vật';

-- Actual intake (for fulfilled proposal)
INSERT INTO ingredient_actual_intakes (ReceiptCode, ProposalId, CreatedByUserId, ReceivedAt, Note)
VALUES ('PNK-20250324-X1Y2', (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250324-A1B2'), @staff_id, DATE_SUB(NOW(), INTERVAL 3 DAY), 'Nhận đủ hàng');

INSERT INTO ingredient_actual_intake_lines (IntakeId, IngredientId, Quantity)
SELECT (SELECT Id FROM ingredient_actual_intakes WHERE ReceiptCode = 'PNK-20250324-X1Y2'), Id, 30 FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO ingredient_actual_intake_lines (IntakeId, IngredientId, Quantity)
SELECT (SELECT Id FROM ingredient_actual_intakes WHERE ReceiptCode = 'PNK-20250324-X1Y2'), Id, 20 FROM ingredients WHERE Name = 'Thịt gà ta';
