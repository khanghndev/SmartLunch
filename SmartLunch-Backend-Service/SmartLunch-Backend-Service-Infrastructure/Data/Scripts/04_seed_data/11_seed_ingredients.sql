-- =====================================================
-- Seed: Ingredient categories, Ingredients, Sources, Inventory
-- ingredient_categories (Code, Name, NameEnglish) + 27 nguyên liệu — pool 22 món test AI Menu Planner
-- =====================================================
SET @p1 = (SELECT Id FROM partners WHERE TaxId = '0301234567');
SET @p2 = (SELECT Id FROM partners WHERE TaxId = '5801234568');
SET @p3 = (SELECT Id FROM partners WHERE TaxId = '0309012345');

-- ── Ingredient categories (FK target for ingredients.CategoryId) ──
INSERT INTO ingredient_categories (Name, NameEnglish, Description) VALUES
('Thịt (Động vật)',     'meat', 'Thịt (Động vật)'),
('Cá & Hải sản',        'seafood', 'Cá & Hải sản'),
('Trứng',               'egg', 'Trứng'),
('Đậu & Đạm thực vật',   'plant_protein', 'Đậu & Đạm thực vật'),
('Rau củ quả',          'vegetable', 'Rau củ quả'),
('Gia vị',              'spice', 'Gia vị'),
('Tinh bột',             'carb', 'Tinh bột');

-- ── Nguyên liệu chính (protein) ──────────────────────
INSERT INTO ingredients (Name, NameEnglish, Unit, Description, DefaultSupplierId, CostPerUnit, IsActive) VALUES
('Thịt heo nạc vai',  'pork',             'kg',   'Thịt heo nạc vai tươi',              @p1, 120000, 1),
('Thịt gà ta',        'chicken',           'kg',   'Gà ta nuôi thả vườn',                @p1,  95000, 1),
('Tôm sú',            'shrimp',            'kg',   'Tôm sú size 30-35 con/kg',           @p1, 280000, 1),
('Cá lóc',            'snakehead_fish',    'kg',   'Cá lóc đồng tươi',                   @p1,  85000, 1),
('Cá basa',           'basa_fish',         'kg',   'Cá basa phi lê đông lạnh',           @p1,  60000, 1),
('Thịt bò',           'beef',              'kg',   'Thịt bò nạc vai',                    @p1, 200000, 1),
('Trứng gà',          'egg',               'quả',  'Trứng gà ta tươi',                   @p1,   4000, 1),
('Đậu hũ',            'tofu',              'miếng','Đậu hũ non tươi',                    @p1,   5000, 1),
-- ── Rau củ ───────────────────────────────────────────
('Rau muống',         'morning_glory',     'kg',   'Rau muống nước sạch',                @p2,  15000, 1),
('Cà chua',           'tomato',            'kg',   'Cà chua Đà Lạt organic',             @p2,  25000, 1),
('Hành tím',          'shallot',           'kg',   'Hành tím Sóc Trăng',                 @p2,  35000, 1),
('Tỏi',               'garlic',            'kg',   'Tỏi Lý Sơn',                         @p2,  80000, 1),
('Cà rốt',            'carrot',            'kg',   'Cà rốt Đà Lạt',                     @p2,  20000, 1),
('Khoai tây',         'potato',            'kg',   'Khoai tây Đà Lạt',                  @p2,  25000, 1),
('Bí đỏ',             'pumpkin',           'kg',   'Bí đỏ Đà Lạt',                      @p2,  18000, 1),
('Khổ qua',           'bitter_melon',      'kg',   'Khổ qua xanh',                       @p2,  25000, 1),
('Cải ngọt',          'bok_choy',          'kg',   'Cải ngọt tươi',                      @p2,  20000, 1),
('Đậu cove',          'green_bean',        'kg',   'Đậu cove tươi',                      @p2,  30000, 1),
('Mồng tơi',          'malabar_spinach',   'kg',   'Rau mồng tơi tươi',                  @p2,  15000, 1),
('Dưa leo',           'cucumber',          'kg',   'Dưa leo Đà Lạt',                     @p2,  15000, 1),
('Hành tây',          'onion',             'kg',   'Hành tây trắng',                     @p2,  22000, 1),
('Gừng',              'ginger',            'kg',   'Gừng tươi Hưng Yên',                 @p2,  40000, 1),
-- ── Tinh bột / cơ bản ────────────────────────────────
('Gạo ST25',          'rice',              'kg',   'Gạo ST25 Sóc Trăng',                @p2,  22000, 1),
-- ── Gia vị / dầu ─────────────────────────────────────
('Nước mắm',          'fish_sauce',        'lít',  'Nước mắm Phú Quốc 40 độ đạm',       @p3,  65000, 1),
('Đường cát trắng',   'sugar',             'kg',   'Đường tinh luyện',                   @p3,  18000, 1),
('Dầu thực vật',      'cooking_oil',       'lít',  'Dầu Neptune Gold',                   @p3,  42000, 1),
('Nước dừa tươi',     'coconut_water',     'lít',  'Nước dừa tươi Bến Tre',              @p3,  20000, 1);

-- ── Gán CategoryId (ingredients.CategoryId → ingredient_categories.Id) ──
UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'meat')
WHERE NameEnglish IN ('pork', 'chicken', 'beef', 'duck', 'goat', 'lamb');

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'seafood')
WHERE NameEnglish IN ('shrimp', 'snakehead_fish', 'basa_fish', 'fish', 'squid', 'crab', 'clam', 'salmon', 'catfish');

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'egg')
WHERE NameEnglish = 'egg';

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'plant_protein')
WHERE NameEnglish IN ('tofu', 'mushroom', 'bean');

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'vegetable')
WHERE NameEnglish IN ('morning_glory', 'pumpkin', 'cucumber', 'bok_choy', 'green_bean', 'malabar_spinach', 'bitter_melon', 'cabbage', 'spinach', 'carrot', 'tomato', 'potato', 'onion', 'shallot', 'garlic', 'ginger');

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'spice')
WHERE NameEnglish IN ('fish_sauce', 'salt', 'pepper', 'sugar', 'cooking_oil', 'coconut_water', 'soy_sauce', 'oyster_sauce', 'vinegar');

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'carb')
WHERE NameEnglish = 'rice';

-- ── Ingredient Sources ────────────────────────────────
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p1, 'LOT-TH-20250320', 'Trang trại Bình Dương',  '2025-03-20', '2025-03-23', 'VietGAHP'        FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p1, 'LOT-GA-20250320', 'Trang trại Long An',      '2025-03-20', '2025-03-22', 'VietGAHP'        FROM ingredients WHERE Name = 'Thịt gà ta';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p1, 'LOT-TOM-20250321','Vùng nuôi Cà Mau',        '2025-03-21', '2025-03-25', 'ASC'             FROM ingredients WHERE Name = 'Tôm sú';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p1, 'LOT-CA-20250322', 'Ao nuôi Đồng Tháp',       '2025-03-22', '2025-03-24', 'VietGAHP'        FROM ingredients WHERE Name = 'Cá lóc';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p2, 'LOT-RM-20250321', 'Vùng trồng rau Long An',  '2025-03-21', '2025-03-24', 'VietGAP'         FROM ingredients WHERE Name = 'Rau muống';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p2, 'LOT-G-20250301',  'Vùng lúa Sóc Trăng',      '2025-03-01', '2025-09-01', 'GlobalGAP'       FROM ingredients WHERE Name = 'Gạo ST25';
INSERT INTO ingredient_sources (IngredientId, PartnerId, BatchNumber, OriginDetails, ProductionDate, ExpirationDate, Certification)
SELECT Id, @p3, 'LOT-NM-20250201', 'Nhà thùng Phú Quốc',      '2025-02-01', '2027-02-01', 'HACCP, ISO 22000' FROM ingredients WHERE Name = 'Nước mắm';

-- ── Inventory ─────────────────────────────────────────
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  50, 20  FROM ingredients WHERE Name = 'Thịt heo nạc vai';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  35, 15  FROM ingredients WHERE Name = 'Thịt gà ta';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  20, 10  FROM ingredients WHERE Name = 'Tôm sú';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  30, 10  FROM ingredients WHERE Name = 'Cá lóc';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  40, 15  FROM ingredients WHERE Name = 'Cá basa';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  20,  8  FROM ingredients WHERE Name = 'Thịt bò';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id, 500,100  FROM ingredients WHERE Name = 'Trứng gà';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id, 100, 30  FROM ingredients WHERE Name = 'Đậu hũ';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  30, 10  FROM ingredients WHERE Name = 'Rau muống';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  25, 10  FROM ingredients WHERE Name = 'Cà chua';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  15,  5  FROM ingredients WHERE Name = 'Hành tím';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,   8,  3  FROM ingredients WHERE Name = 'Tỏi';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  40, 10  FROM ingredients WHERE Name = 'Cà rốt';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  35, 10  FROM ingredients WHERE Name = 'Khoai tây';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  35, 10  FROM ingredients WHERE Name = 'Bí đỏ';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  25,  8  FROM ingredients WHERE Name = 'Khổ qua';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  40, 12  FROM ingredients WHERE Name = 'Cải ngọt';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  30, 10  FROM ingredients WHERE Name = 'Đậu cove';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  25,  8  FROM ingredients WHERE Name = 'Mồng tơi';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  30, 10  FROM ingredients WHERE Name = 'Dưa leo';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  20,  5  FROM ingredients WHERE Name = 'Hành tây';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  10,  3  FROM ingredients WHERE Name = 'Gừng';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id, 200, 50  FROM ingredients WHERE Name = 'Gạo ST25';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  20,  5  FROM ingredients WHERE Name = 'Nước mắm';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  30, 10  FROM ingredients WHERE Name = 'Đường cát trắng';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  25,  8  FROM ingredients WHERE Name = 'Dầu thực vật';
INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel) SELECT Id,  15,  5  FROM ingredients WHERE Name = 'Nước dừa tươi';
