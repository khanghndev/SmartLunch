-- =====================================================
-- Seed: Intake Proposals & Actual Intakes
-- =====================================================
-- Use variable names consistent with seeded user roles
SET @warehouse_staff_id = (SELECT Id FROM users WHERE Username = 'warehouse_staff');
SET @manager_id = (SELECT Id FROM users WHERE Username = 'manager');

-- Define "now" to be consistent for all generated dates
SET @now = NOW();

-- Seed intake proposals
INSERT INTO ingredient_intake_proposals (ProposalCode, Status, HeaderNote, CreatedByUserId, CreatedAt, ReviewedByUserId, ReviewedAt, ReviewNote)
VALUES
  ('DXN-20250324-A1B2', 'fulfilled', N'Đề xuất nhập thịt tuần 4/3', @warehouse_staff_id, DATE_SUB(@now, INTERVAL 5 DAY), @manager_id, DATE_SUB(@now, INTERVAL 4 DAY), N'Duyệt'),
  ('DXN-20250325-E5F6', 'approved', N'Đề xuất nhập rau củ tuần 4/3', @warehouse_staff_id, DATE_SUB(@now, INTERVAL 5 DAY), @manager_id, DATE_SUB(@now, INTERVAL 2 DAY), N'Duyệt'),
  ('DXN-20250326-I9J0', 'submitted', N'Đề xuất nhập gia vị tháng 4', @warehouse_staff_id, DATE_SUB(@now, INTERVAL 5 DAY), NULL, NULL, NULL);

-- Proposal lines (ingredient_intake_proposal_lines)
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250324-A1B2'), Id, 30, N'Tươi trong ngày' FROM ingredients WHERE Name = N'Thịt heo nạc vai';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250324-A1B2'), Id, 20, NULL FROM ingredients WHERE Name = N'Thịt gà ta';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250325-E5F6'), Id, 20, NULL FROM ingredients WHERE Name = N'Rau muống';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250325-E5F6'), Id, 15, NULL FROM ingredients WHERE Name = N'Cà chua';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250326-I9J0'), Id, 10, NULL FROM ingredients WHERE Name = N'Nước mắm';
INSERT INTO ingredient_intake_proposal_lines (ProposalId, IngredientId, Quantity, LineNote)
SELECT (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250326-I9J0'), Id, 12, NULL FROM ingredients WHERE Name = N'Dầu thực vật';

-- Actual intake (for fulfilled proposal)
INSERT INTO ingredient_actual_intakes (ReceiptCode, ProposalId, CreatedByUserId, ReceivedAt, Note)
VALUES ('PNK-20250324-X1Y2', (SELECT Id FROM ingredient_intake_proposals WHERE ProposalCode = 'DXN-20250324-A1B2'), @warehouse_staff_id, DATE_SUB(@now, INTERVAL 3 DAY), N'Nhận đủ hàng');

INSERT INTO ingredient_actual_intake_lines (IntakeId, IngredientId, Quantity)
SELECT (SELECT Id FROM ingredient_actual_intakes WHERE ReceiptCode = 'PNK-20250324-X1Y2'), Id, 30 FROM ingredients WHERE Name = N'Thịt heo nạc vai';
INSERT INTO ingredient_actual_intake_lines (IntakeId, IngredientId, Quantity)
SELECT (SELECT Id FROM ingredient_actual_intakes WHERE ReceiptCode = 'PNK-20250324-X1Y2'), Id, 20 FROM ingredients WHERE Name = N'Thịt gà ta';
