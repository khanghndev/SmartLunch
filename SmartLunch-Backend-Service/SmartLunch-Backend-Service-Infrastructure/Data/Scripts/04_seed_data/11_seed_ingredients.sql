-- =====================================================
-- Seed: Ingredients, Sources, Inventory
-- =====================================================
SET @p1 = (SELECT Id FROM partners WHERE TaxId = '0301234567');
SET @p2 = (SELECT Id FROM partners WHERE TaxId = '5801234568');
SET @p3 = (SELECT Id FROM partners WHERE TaxId = '0309012345');

INSERT INTO ingredients (Name, Unit, Description, DefaultSupplierId, CostPerUnit, IsActive) VALUES
('Thịt heo nạc vai', 'kg', 'Thịt heo nạc vai tươi', @p1, 120000, 1),
('Thịt gà ta', 'kg', 'Gà ta nuôi thả vườn', @p1, 95000, 1),
('Tôm sú', 'kg', 'Tôm sú size 30-35 con/kg', @p1, 280000, 1),
('Rau muống', 'kg', 'Rau muống nước sạch', @p2, 15000, 1),
('Cà chua', 'kg', 'Cà chua Đà Lạt organic', @p2, 25000, 1),
('Hành tím', 'kg', 'Hành tím Sóc Trăng', @p2, 35000, 1),
('Tỏi', 'kg', 'Tỏi Lý Sơn', @p2, 80000, 1),
('Gạo ST25', 'kg', 'Gạo ST25 Sóc Trăng', @p2, 22000, 1),
('Nước mắm', 'lít', 'Nước mắm Phú Quốc 40 độ đạm', @p3, 65000, 1),
('Đường cát trắng', 'kg', 'Đường tinh luyện', @p3, 18000, 1),
('Dầu thực vật', 'lít', 'Dầu Neptune Gold', @p3, 42000, 1),
('Trứng gà', 'quả', 'Trứng gà ta tươi', @p1, 4000, 1),
('Đậu hũ', 'miếng', 'Đậu hũ non tươi', @p1, 5000, 1),
('Cà rốt', 'kg', 'Cà rốt Đà Lạt', @p2, 20000, 1),
('Khoai tây', 'kg', 'Khoai tây Đà Lạt', @p2, 25000, 1);

-- Ingredient Sources
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p1, 'LOT-TH-20250320', 'Trang trại Bình Dương', '2025-03-20', '2025-03-23', 'VietGAHP' FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p2, 'LOT-RM-20250321', 'Vùng trồng rau Long An', '2025-03-21', '2025-03-24', 'VietGAP' FROM ingredients WHERE Name = 'Rau muống';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p2, 'LOT-G-20250301', 'Vùng lúa Sóc Trăng', '2025-03-01', '2025-09-01', 'GlobalGAP' FROM ingredients WHERE Name = 'Gạo ST25';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p3, 'LOT-NM-20250201', 'Nhà thùng Phú Quốc', '2025-02-01', '2027-02-01', 'HACCP, ISO 22000' FROM ingredients WHERE Name = 'Nước mắm';

-- Inventory
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 50, 20 FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 35, 15 FROM ingredients WHERE Name = 'Thịt gà ta';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 20, 10 FROM ingredients WHERE Name = 'Tôm sú';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 30, 10 FROM ingredients WHERE Name = 'Rau muống';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 25, 10 FROM ingredients WHERE Name = 'Cà chua';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 15, 5 FROM ingredients WHERE Name = 'Hành tím';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 8, 3 FROM ingredients WHERE Name = 'Tỏi';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 200, 50 FROM ingredients WHERE Name = 'Gạo ST25';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 20, 5 FROM ingredients WHERE Name = 'Nước mắm';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 30, 10 FROM ingredients WHERE Name = 'Đường cát trắng';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 25, 8 FROM ingredients WHERE Name = 'Dầu thực vật';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 500, 100 FROM ingredients WHERE Name = 'Trứng gà';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 100, 30 FROM ingredients WHERE Name = 'Đậu hũ';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 40, 10 FROM ingredients WHERE Name = 'Cà rốt';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT Id, 35, 10 FROM ingredients WHERE Name = 'Khoai tây';
