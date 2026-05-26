-- =====================================================
-- Seed: Gia vị / nước chấm cho định mức món (idempotent)
-- Sinh bởi scratch/generate_dish_seasoning_bom.py
-- Gắn theo cooking_methods.MethodKey; không nhân bản khi chạy lại.
-- =====================================================

USE SmartLunch;
SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci;

-- stewed: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.004, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Muối' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tỏi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành tím' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Gừng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Dầu thực vật' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stewed: Hạt nêm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hạt nêm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stewed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Dầu thực vật' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.015, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Nước tương
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước tương' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tỏi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Muối' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- fried: Hành phi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành phi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Dầu thực vật' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.015, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Nước tương
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước tương' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tỏi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.008, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành tím' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- stir_fried: Hạt nêm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hạt nêm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'stir_fried'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tỏi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Gừng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Sả băm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Sả băm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Dầu thực vật' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- grilled: Tương ớt
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tương ớt' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'grilled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- steamed: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.015, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'steamed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- steamed: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.015, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Gừng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'steamed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- steamed: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành tím' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'steamed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- steamed: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tỏi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'steamed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- steamed: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.002, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'steamed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- steamed: Hành lá
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành lá' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'steamed'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- boiled: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Muối' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'boiled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- boiled: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.002, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'boiled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- boiled: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành tím' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'boiled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- boiled: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Gừng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'boiled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- boiled: Hạt nêm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hạt nêm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'boiled'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- raw: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Muối' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'raw'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- raw: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'raw'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- raw: Giấm ăn
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Giấm ăn' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'raw'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- raw: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tỏi' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'raw'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- raw: Ớt hiểm
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.002, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Ớt hiểm' COLLATE utf8mb4_unicode_ci

LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'
WHERE 1=1
  AND cm.MethodKey = 'raw'
  AND cat.Id IS NULL
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- dessert: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.015, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci

WHERE d.Name IN ('Chè bí đỏ', 'Chè bí đỏ nước dừa ngọt', 'Chè khoai tây đường cát', 'Cà rốt ngào đường', 'Nước dừa tươi ngọt lành', 'Trái cây dầm')
  
  
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- dessert: Đường phèn
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường phèn' COLLATE utf8mb4_unicode_ci

WHERE d.Name IN ('Chè bí đỏ', 'Chè bí đỏ nước dừa ngọt', 'Chè khoai tây đường cát', 'Cà rốt ngào đường', 'Nước dừa tươi ngọt lành', 'Trái cây dầm')
  
  
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1)
  );

-- Kho cá: mắm ruốc (Nam Bộ)
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId AND cm.MethodKey = 'stewed'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Mắm ruốc' COLLATE utf8mb4_unicode_ci
WHERE (d.Name LIKE '%cá %' OR d.Name LIKE 'Cá %' OR d.Name LIKE '%cá lóc%' OR d.Name LIKE '%Cá %')
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));

-- Canh: nước mắm pha nhẹ
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId AND cm.MethodKey = 'boiled'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'soup'
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));

-- Món nước (phở/bún): gia vị nước dùng
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.006, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Muối' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.015, 'lít'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Gừng' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành tím' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.002, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.005, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Sả băm' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id AND di.DishValueId = (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1));

SELECT 'Dish seasoning BOM sync completed.' AS Status;
