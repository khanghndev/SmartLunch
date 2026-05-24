-- =====================================================
-- Seed: Gia vị / nước chấm cho định mức món (idempotent)
-- Sinh bởi scratch/generate_dish_seasoning_bom.py
-- Gắn theo cooking_methods.MethodKey; không nhân bản khi chạy lại.
-- =====================================================

USE SmartLunch;
SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci;

-- stewed: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.02, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.004, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stewed: Hạt nêm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.03, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.015, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Nước tương
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- fried: Hành phi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.02, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.015, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Nước tương
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.008, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- stir_fried: Hạt nêm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.02, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Sả băm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Dầu thực vật
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- grilled: Tương ớt
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- steamed: Nước mắm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.015, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- steamed: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.015, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- steamed: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- steamed: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- steamed: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.002, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- steamed: Hành lá
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- boiled: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- boiled: Tiêu xay
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.002, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- boiled: Hành tím
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- boiled: Gừng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- boiled: Hạt nêm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- raw: Muối
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- raw: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- raw: Giấm ăn
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.02, 'lít'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- raw: Tỏi
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- raw: Ớt hiểm
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.002, 'kg'
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
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- dessert: Đường cát trắng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.015, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci

WHERE d.Name IN ('Chè bí đỏ', 'Chè bí đỏ nước dừa ngọt', 'Chè khoai tây đường cát', 'Cà rốt ngào đường', 'Nước dừa tươi ngọt lành', 'Trái cây dầm')
  
  
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- dessert: Đường phèn
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường phèn' COLLATE utf8mb4_unicode_ci

WHERE d.Name IN ('Chè bí đỏ', 'Chè bí đỏ nước dừa ngọt', 'Chè khoai tây đường cát', 'Cà rốt ngào đường', 'Nước dừa tươi ngọt lành', 'Trái cây dầm')
  
  
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );

-- Kho cá: mắm ruốc (Nam Bộ)
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId AND cm.MethodKey = 'stewed'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Mắm ruốc' COLLATE utf8mb4_unicode_ci
WHERE (d.Name LIKE '%cá %' OR d.Name LIKE 'Cá %' OR d.Name LIKE '%cá lóc%' OR d.Name LIKE '%Cá %')
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);

-- Canh: nước mắm pha nhẹ
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId AND cm.MethodKey = 'boiled'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'soup'
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);

-- Món nước (phở/bún): gia vị nước dùng
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.006, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Muối' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.015, 'lít'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Gừng' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Hành tím' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Đường cát trắng' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.002, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Tiêu xay' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.005, 'kg'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Sả băm' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);

SELECT 'Dish seasoning BOM sync completed.' AS Status;
