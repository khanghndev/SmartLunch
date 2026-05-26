-- =====================================================
-- Nhân BOM từ mức chuẩn 30.000đ sang các mức giá suất khác
-- (tỷ lệ Quantity theo Amount / 30000)
-- Chạy sau 14_seed_dishes + 20_seed_dish_seasonings
-- =====================================================
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit)
SELECT
    di.DishId,
    di.IngredientId,
    dv.Id,
    ROUND(di.Quantity * dv.Amount / base.Amount, 2),
    di.Unit
FROM dish_ingredients di
INNER JOIN dish_values base ON base.Amount = 30000
INNER JOIN dish_values dv ON dv.Amount IN (25000, 45000, 60000)
WHERE di.DishValueId = base.Id
  AND NOT EXISTS (
      SELECT 1
      FROM dish_ingredients x
      WHERE x.DishId = di.DishId
        AND x.IngredientId = di.IngredientId
        AND x.DishValueId = dv.Id
  );
