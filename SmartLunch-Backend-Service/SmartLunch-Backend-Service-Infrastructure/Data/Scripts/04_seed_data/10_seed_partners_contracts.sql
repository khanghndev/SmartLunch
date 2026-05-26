-- =====================================================
-- Seed: Partners, Contracts, Partner Payments
-- =====================================================
INSERT INTO partners (LegalName, BusinessRegistrationNumber, TaxId, LegalRepresentative, Address, ContactPerson, Phone, Email, PerformanceRating, ComplianceInfo, FinancialTerms, IsActive) VALUES
('Công ty TNHH Thực Phẩm Sạch Việt', 'DKKD-2020-001234', '0301234567', 'Nguyễn Văn An', '100 Trường Chinh, Tân Phú, TP.HCM', 'Trần Thị Bình', '+84901234567', 'contact@thucphamsachviet.vn', 4.50, 'VietGAP, ISO 22000:2018', 'Thanh toán 30 ngày', 1),
('HTX Rau Củ Đà Lạt', 'DKKD-2019-005678', '5801234568', 'Lê Minh Cường', '22 Phan Đình Phùng, Đà Lạt', 'Phạm Văn Đức', '+84909876543', 'sales@raucudalat.vn', 4.80, 'Organic, GlobalGAP', 'COD', 1),
('Công ty CP Gia Vị Miền Nam', 'DKKD-2018-009012', '0309012345', 'Hoàng Thị Em', '55 Nguyễn Thị Minh Khai, Q.1', 'Vũ Quốc Fong', '+84905551234', 'order@giavimiennam.vn', 4.20, 'HACCP', 'Thanh toán 15 ngày', 1);

-- Contracts
INSERT INTO contracts (PartnerId, ContractNumber, Description, SupplySchedule, StartDate, EndDate, TotalValue, DepositAmount, Status) VALUES
((SELECT Id FROM partners WHERE TaxId = '0301234567'), 'HD-2025-001', 'Cung cấp thịt heo, gà và hải sản tươi sống', 'T2, T4, T6 lúc 5h sáng', '2025-01-01', '2025-12-31', 500000000, 50000000, 'active'),
((SELECT Id FROM partners WHERE TaxId = '5801234568'), 'HD-2025-002', 'Cung cấp rau củ quả organic hàng ngày', 'Mỗi ngày lúc 4h30 sáng', '2025-01-01', '2025-06-30', 200000000, 20000000, 'active'),
((SELECT Id FROM partners WHERE TaxId = '0309012345'), 'HD-2025-003', 'Cung cấp gia vị và nước chấm hàng tháng', 'Ngày 1 và 15 hàng tháng', '2025-03-01', '2026-02-28', 80000000, 8000000, 'active');

INSERT INTO contracts (PartnerId, OrganizationId, ContractNumber, Description, SupplySchedule, StartDate, EndDate, TotalValue, DishValueId, MealUnitPrice, DepositAmount, Status) VALUES
((SELECT Id FROM partners WHERE TaxId = '0301234567'), (SELECT Id FROM organizations WHERE Name = 'Công ty TNHH ABC Tech'), 'HD-B2B-ABC-001', 'Hợp đồng cung cấp suất ăn văn phòng cho ABC Tech', 'Mỗi ngày lúc 11h sáng', '2025-01-01', '2026-12-31', 1000000000, (SELECT Id FROM dish_values WHERE Amount = 45000 LIMIT 1), 45000, 100000000, 'active'),
((SELECT Id FROM partners WHERE TaxId = '0301234567'), (SELECT Id FROM organizations WHERE Name = 'Trường THPT XYZ'), 'HD-B2B-XYZ-002', 'Hợp đồng cung cấp suất ăn trường học cho XYZ', 'Mỗi ngày lúc 10h sáng', '2025-01-01', '2026-12-31', 800000000, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 30000, 80000000, 'active');

-- Partner Payments
INSERT INTO partner_payments (ContractId, PartnerId, PaymentDate, Amount, Method, Status) VALUES
((SELECT Id FROM contracts WHERE ContractNumber = 'HD-2025-001'), (SELECT Id FROM partners WHERE TaxId = '0301234567'), '2025-02-01', 42000000, 'bank_transfer', 'completed'),
((SELECT Id FROM contracts WHERE ContractNumber = 'HD-2025-001'), (SELECT Id FROM partners WHERE TaxId = '0301234567'), '2025-03-01', 45000000, 'bank_transfer', 'completed'),
((SELECT Id FROM contracts WHERE ContractNumber = 'HD-2025-002'), (SELECT Id FROM partners WHERE TaxId = '5801234568'), '2025-02-15', 35000000, 'bank_transfer', 'completed'),
((SELECT Id FROM contracts WHERE ContractNumber = 'HD-2025-003'), (SELECT Id FROM partners WHERE TaxId = '0309012345'), '2025-03-01', 6500000, 'cash', 'completed');
