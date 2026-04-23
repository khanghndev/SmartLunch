-- =====================================================
-- Seed: Dishes & Dish Ingredients
-- =====================================================
INSERT INTO dishes (Name, Description, Category, Price, DietaryLabel, IsActive) VALUES
('Cơm sườn nướng', 'Cơm trắng kèm sườn heo nướng sả ớt', 'Cơm phần', 45000, NULL, 1),
('Cơm gà chiên mắm', 'Cơm trắng kèm đùi gà chiên nước mắm', 'Cơm phần', 42000, NULL, 1),
('Canh chua tôm', 'Canh chua nấu tôm sú với cà chua', 'Canh', 35000, NULL, 1),
('Rau muống xào tỏi', 'Rau muống xào tỏi phi thơm', 'Rau', 20000, 'vegan', 1),
('Trứng chiên', 'Trứng gà ta chiên vàng', 'Món phụ', 15000, NULL, 1),
('Đậu hũ sốt cà chua', 'Đậu hũ non sốt cà chua', 'Rau', 22000, 'vegan', 1),
('Khoai tây chiên', 'Khoai tây chiên giòn', 'Món phụ', 25000, 'vegan', 1),
('Cơm chiên dương châu', 'Cơm chiên với tôm, trứng, cà rốt', 'Cơm phần', 40000, NULL, 1);

-- Dish ingredients (BOM)
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm sườn nướng' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm sườn nướng' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.25, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.03, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm' AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh chua tôm' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh chua tôm' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.30, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào tỏi' AND i.Name = 'Rau muống';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 2.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Trứng chiên' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ sốt cà chua' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây chiên' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu' AND i.Name = 'Cà rốt';
