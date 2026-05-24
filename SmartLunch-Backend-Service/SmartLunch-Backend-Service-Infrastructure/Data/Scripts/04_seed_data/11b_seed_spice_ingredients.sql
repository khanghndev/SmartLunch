-- =====================================================
-- Bổ sung nguyên liệu gia vị (idempotent — chạy lại an toàn)
-- =====================================================
SET @p3 = (SELECT Id FROM partners WHERE TaxId = '0309012345');

INSERT INTO ingredients (Name, NameEnglish, Unit, Description, DefaultSupplierId, CostPerUnit, IsActive)
SELECT v.Name, v.NameEnglish, v.Unit, v.Description, @p3, v.Cost, 1
FROM (
    SELECT 'Muối' AS Name, 'salt' AS NameEnglish, 'kg' AS Unit, 'Muối tinh' AS Description, 8000 AS Cost UNION ALL
    SELECT 'Tiêu xay', 'pepper', 'kg', 'Tiêu đen xay', 250000 UNION ALL
    SELECT 'Nước tương', 'soy_sauce', 'lít', 'Nước tương Maggi/Chinsu', 45000 UNION ALL
    SELECT 'Dầu hào', 'oyster_sauce', 'lít', 'Dầu hào Maggi', 55000 UNION ALL
    SELECT 'Giấm ăn', 'vinegar', 'lít', 'Giấm gạo / táo', 25000 UNION ALL
    SELECT 'Hạt nêm', 'seasoning_powder', 'kg', 'Hạt nêm Ajinomoto', 120000 UNION ALL
    SELECT 'Bột ngọt', 'msg', 'kg', 'Bột ngọt (MSG)', 90000 UNION ALL
    SELECT 'Sả băm', 'lemongrass', 'kg', 'Sả tươi băm nhỏ', 35000 UNION ALL
    SELECT 'Ớt hiểm', 'chili', 'kg', 'Ớt hiểm tươi', 40000 UNION ALL
    SELECT 'Hành lá', 'green_onion', 'kg', 'Hành lá cắt nhỏ', 30000 UNION ALL
    SELECT 'Hành phi', 'fried_shallot', 'kg', 'Hành phi vàng', 180000 UNION ALL
    SELECT 'Tương ớt', 'chili_sauce', 'lít', 'Tương ớt Cholimex', 35000 UNION ALL
    SELECT 'Mắm ruốc', 'shrimp_paste', 'kg', 'Mắm ruốc loại ngon', 120000 UNION ALL
    SELECT 'Đường phèn', 'rock_sugar', 'kg', 'Đường phèn viên', 22000
) AS v
WHERE NOT EXISTS (
    SELECT 1 FROM ingredients i WHERE i.Name = v.Name COLLATE utf8mb4_unicode_ci
);

UPDATE ingredients SET CategoryId = (SELECT Id FROM ingredient_categories WHERE NameEnglish = 'spice')
WHERE NameEnglish IN (
    'salt', 'pepper', 'soy_sauce', 'oyster_sauce', 'vinegar', 'seasoning_powder', 'msg',
    'lemongrass', 'chili', 'green_onion', 'fried_shallot', 'chili_sauce', 'shrimp_paste', 'rock_sugar',
    'fish_sauce', 'sugar', 'cooking_oil', 'coconut_water', 'garlic', 'ginger', 'shallot'
);

INSERT INTO inventory (IngredientId, QuantityAvailable, ReorderLevel)
SELECT i.Id, 50, 10
FROM ingredients i
WHERE i.Name IN (
    'Muối', 'Tiêu xay', 'Nước tương', 'Dầu hào', 'Giấm ăn', 'Hạt nêm', 'Bột ngọt',
    'Sả băm', 'Ớt hiểm', 'Hành lá', 'Hành phi', 'Tương ớt', 'Mắm ruốc', 'Đường phèn'
)
AND NOT EXISTS (SELECT 1 FROM inventory inv WHERE inv.IngredientId = i.Id);
