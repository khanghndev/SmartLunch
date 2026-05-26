-- =====================================================
-- Seed: Dishes & Dish Ingredients (BOM)
-- 50 món — 46 món tách slot + 2 món nước composite + 2 tráng miệng (pool AI / demo)
--
-- Slot AI: chỉ còn trên dish_dish_categories → dish_categories.SlotKey (không còn Category/CategoryEnglish trên dishes).
-- Món nước composite (phở, bánh canh): junction gồm main, soup, vegetable, noodle_soup.
--
-- Phân bố slot (theo junction):
--   main         : 16 món
--   soup         : 11 món
--   vegetable    : 10 món
--   side         : 9 món
--   dessert      : 2 món
--   noodle_soup  : 2 món composite (+ covers main,soup,vegetable)
--
-- BOM chuẩn: dish_ingredients.DishValueId = suất 30.000đ (14b nhân sang 25k/45k/60k).
-- Budget target: tổng 4 slot ≤ 60,000 VND/ngày
--   main  ~20,000–28,000 | soup ~7,000–12,000
--   veg   ~5,000–9,000   | side ~3,000–8,000
--
-- CookingMethod: FK dishes.CookingMethodId → cooking_methods.MethodKey (không còn cột chữ trên dishes).
-- =====================================================

-- ════════════════════════════════════════════════════
--  COOKING_METHODS (enum AI; Code sinh bởi trigger)
-- ════════════════════════════════════════════════════
INSERT INTO cooking_methods (MethodKey, Name, SortOrder) VALUES
('fried', 'Chiên / rán', 10),
('stewed', 'Kho / hầm', 20),
('boiled', 'Luộc / nấu', 30),
('stir_fried', 'Xào', 40),
('grilled', 'Nướng', 50),
('steamed', 'Hấp', 60),
('raw', 'Sống / trộng', 70);

-- ════════════════════════════════════════════════════
--  MAIN — 7 + 9 món (đa dạng protein: pork/chicken/fish/beef/shrimp)
-- ════════════════════════════════════════════════════
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Cơm sườn nướng',        'Grilled Pork Rib Rice',               'Cơm trắng kèm sườn heo nướng sả ớt',               (SELECT Id FROM cooking_methods WHERE MethodKey = 'grilled' LIMIT 1),    25000, NULL,    1),
('Cơm gà chiên mắm',      'Fish Sauce Fried Chicken Rice',       'Cơm trắng kèm đùi gà chiên nước mắm',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1),      24000, NULL,    1),
('Cơm chiên dương châu',  'Yangzhou Fried Rice',                 'Cơm chiên với tôm, trứng, cà rốt',                  (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 22000, NULL,    1),
('Thịt heo kho trứng',    'Braised Pork with Egg',               'Thịt heo ba chỉ kho nước dừa với trứng gà',         (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1),     22000, NULL,    1),
('Gà kho gừng',           'Ginger Braised Chicken',              'Đùi gà ta kho gừng sả đậm đà',                     (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1),     20000, NULL,    1),
('Cá lóc kho tộ',         'Claypot Braised Snakehead Fish',      'Cá lóc đồng kho tiêu đặc trưng Nam Bộ',            (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1),     24000, NULL,    1),
('Thịt bò xào hành tây',  'Beef Stir-Fried with Onion',          'Thịt bò nạc xào hành tây và ớt chuông',            (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 28000, NULL,    1);

-- BOM — main
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm sườn nướng'        AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm sườn nướng'        AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.25, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm'      AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm'      AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm'      AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho trứng'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả'  FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho trứng'    AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho trứng'    AND i.Name = 'Nước dừa tươi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà kho gừng'           AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà kho gừng'           AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc kho tộ'         AND i.Name = 'Cá lóc';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.14, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây'  AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây'  AND i.Name = 'Hành tây';

-- MAIN — bổ sung (9 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Cơm cá basa kho tộ',      'Braised Basa Claypot Rice',           'Cơm trắng kèm cá basa kho tiêu',                    (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1),     23000, NULL,    1),
('Thịt heo xào cà chua',    'Pork Stir-Fry with Tomato',           'Thịt heo thái mỏng xào cà chua chua ngọt',          (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 21000, NULL,    1),
('Gà nướng sả',             'Lemongrass Grilled Chicken',          'Đùi gà nướng sả ớt thơm',                            (SELECT Id FROM cooking_methods WHERE MethodKey = 'grilled' LIMIT 1),      26000, NULL,    1),
('Tôm rang me',             'Tamarind Glazed Shrimp',              'Tôm sú rang sốt me chua ngọt',                       (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 32000, NULL,    1),
('Cá basa chiên giòn',      'Crispy Fried Basa',                   'Cá basa phi lê chiên giòn',                          (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1),      22000, NULL,    1),
('Bò xào cà chua',          'Beef Tomato Stir-Fry',                'Thịt bò xào cà chua tươi',                            (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 27000, NULL,    1),
('Cơm rang thịt gà',        'Chicken Fried Rice',                  'Cơm rang gạo ST25 với thịt gà xé',                    (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 21000, NULL,    1),
('Thịt heo xào sả ớt',      'Lemongrass Chili Pork Stir-Fry',      'Thịt heo xào sả ớt đậm đà',                          (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 23000, NULL,    1),
('Gà hấp gừng',             'Ginger Steamed Chicken',              'Đùi gà hấp gừng mềm ngọt',                            (SELECT Id FROM cooking_methods WHERE MethodKey = 'steamed' LIMIT 1),      20000, NULL,    1);

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm cá basa kho tộ'      AND i.Name = 'Cá basa';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm cá basa kho tộ'      AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm cá basa kho tộ'      AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.16, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào cà chua'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào cà chua'    AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà nướng sả'             AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà nướng sả'             AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà nướng sả'             AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Tôm rang me'             AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Tôm rang me'             AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cá basa chiên giòn'      AND i.Name = 'Cá basa';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Cá basa chiên giòn'      AND i.Name = 'Dầu thực vật';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.13, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bò xào cà chua'          AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bò xào cà chua'          AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm rang thịt gà'        AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm rang thịt gà'        AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm rang thịt gà'        AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.17, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào sả ớt'      AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào sả ớt'      AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp gừng'             AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp gừng'             AND i.Name = 'Gừng';

-- ════════════════════════════════════════════════════
--  SOUP — 5 + 6 món (chi phí thấp, phù hợp gộp vào budget)
-- ════════════════════════════════════════════════════
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Canh chua tôm',           'Sour Shrimp Soup',                'Canh chua nấu tôm sú với cà chua dứa',             (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 12000, NULL,    1),
('Canh bí đỏ thịt bằm',    'Pumpkin Soup with Minced Pork',   'Canh bí đỏ nấu với thịt heo xay thơm ngọt',        (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  8000, NULL,    1),
('Canh cải ngọt thịt bằm', 'Bok Choy Soup with Minced Pork',  'Canh cải ngọt nấu thịt heo xay',                   (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7000, NULL,    1),
('Canh khổ qua nhồi thịt', 'Stuffed Bitter Melon Soup',       'Khổ qua xanh nhồi thịt heo xay nấu nước trong',    (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 10000, NULL,    1),
('Canh mồng tơi tôm',      'Malabar Spinach Shrimp Soup',     'Canh mồng tơi nấu tôm sú tươi',                    (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  9000, NULL,    1);

-- BOM — soup
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh chua tôm'           AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh chua tôm'           AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ thịt bằm'    AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ thịt bằm'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt thịt bằm' AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt thịt bằm' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua nhồi thịt' AND i.Name = 'Khổ qua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua nhồi thịt' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi tôm'      AND i.Name = 'Mồng tơi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi tôm'      AND i.Name = 'Tôm sú';

-- SOUP — bổ sung (6 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Canh rau củ thịt bằm',    'Mixed Vegetable Minced Pork Soup',  'Canh cà rốt khoai tây nấu thịt heo xay',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7500, NULL,    1),
('Canh cà chua trứng',      'Tomato Egg Drop Soup',                'Canh cà chua chua ngọt kèm trứng',                 (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  6500, NULL,    1),
('Canh đậu hũ cà chua',     'Tofu Tomato Soup',                    'Đậu hũ non nấu cà chua thanh mát',                 (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7000, 'vegan', 1),
('Canh khoai tây thịt bằm', 'Potato Minced Pork Soup',             'Khoai tây nấu thịt heo xay bùi ngọt',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  8000, NULL,    1),
('Canh cải ngọt cà chua',   'Bok Choy Tomato Soup',                'Cải ngọt nấu cà chua tươi',                         (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7000, 'vegan', 1),
('Canh khổ qua cà chua',    'Bitter Melon Tomato Soup',            'Khổ qua nấu cà chua giảm đắng',                    (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  9000, NULL,    1);

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh rau củ thịt bằm'    AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh rau củ thịt bằm'    AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh rau củ thịt bằm'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua trứng'      AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua trứng'      AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Canh đậu hũ cà chua'     AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh đậu hũ cà chua'     AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khoai tây thịt bằm' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khoai tây thịt bằm' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt cà chua'   AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt cà chua'   AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua cà chua'    AND i.Name = 'Khổ qua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua cà chua'    AND i.Name = 'Cà chua';

-- ════════════════════════════════════════════════════
--  VEGETABLE — 5 + 5 món
-- ════════════════════════════════════════════════════
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Rau muống xào tỏi',  'Stir-Fried Water Spinach with Garlic', 'Rau muống xào tỏi phi thơm',                  (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Đậu hũ sốt cà chua', 'Tofu in Tomato Sauce',                 'Đậu hũ non sốt cà chua tươi',                 (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1),     8000, 'vegan', 1),
('Cải ngọt xào tỏi',   'Stir-Fried Bok Choy with Garlic',     'Cải ngọt tươi xào tỏi phi thơm',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 5000, 'vegan', 1),
('Đậu cove xào thịt',  'Green Bean Stir-Fried with Pork',     'Đậu cove xào thịt heo thái lát',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 8000, NULL,    1),
('Bí đỏ xào tôm',      'Pumpkin Stir-Fried with Shrimp',      'Bí đỏ non xào tôm sú tươi',                   (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 7000, NULL,    1);

-- BOM — vegetable
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.30, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào tỏi'  AND i.Name = 'Rau muống';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào tỏi'  AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ sốt cà chua' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ sốt cà chua' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.25, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi'   AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi'   AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào thịt'  AND i.Name = 'Đậu cove';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào thịt'  AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tôm'      AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tôm'      AND i.Name = 'Tôm sú';

-- VEGETABLE — bổ sung (5 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Cà rốt xào trứng',   'Carrot Egg Stir-Fry',        'Cà rốt xào trứng mềm ngọt',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 5500, NULL,    1),
('Khoai tây xào thịt', 'Potato Pork Stir-Fry',       'Khoai tây xào thịt heo thái lát',   (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 7500, NULL,    1),
('Bí đỏ xào tỏi',      'Garlic Stir-Fried Pumpkin',  'Bí đỏ non xào tỏi phi',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Cải ngọt luộc',      'Blanched Bok Choy',          'Cải ngọt luộc vừa chín giòn',       (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),     4500, 'vegan', 1),
('Cà chua xào trứng',  'Tomato Egg Stir-Fry',        'Cà chua xào trứng đậm đà',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6500, NULL,    1);

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào trứng'   AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào trứng'   AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào thịt' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào thịt' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tỏi'      AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tỏi'      AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.28, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt luộc'     AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà chua xào trứng' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Cà chua xào trứng' AND i.Name = 'Trứng gà';

-- ════════════════════════════════════════════════════
--  SIDE — 5 + 4 món (giá thấp, đa dạng cooking method)
-- ════════════════════════════════════════════════════
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Trứng chiên',        'Fried Egg',          'Trứng gà ta chiên vàng',                    (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1),  6000, NULL,    1),
('Khoai tây chiên',    'Fried Potato',       'Khoai tây Đà Lạt chiên giòn',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1),  8000, 'vegan', 1),
('Đậu hũ chiên giòn', 'Crispy Fried Tofu',  'Đậu hũ non chiên vàng giòn',               (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1),  5000, 'vegan', 1),
('Trứng luộc',         'Boiled Egg',         'Trứng gà ta luộc chín vừa',                 (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 4000, NULL,    1),
('Dưa leo trộn',       'Cucumber Salad',     'Dưa leo Đà Lạt trộn tỏi ớt chua ngọt',     (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    3000, 'vegan', 1);

-- BOM — side
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Trứng chiên'       AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây chiên'   AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ chiên giòn' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Trứng luộc'        AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo trộn'      AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo trộn'      AND i.Name = 'Tỏi';

-- SIDE — bổ sung (4 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Salad dưa leo cà rốt', 'Cucumber Carrot Salad', 'Dưa leo cà rốt trộn chua ngọt',     (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    5000, 'vegan', 1),
('Sốt cà chua chấm',     'Tomato Dipping Sauce',  'Sốt cà chua nấu nhừ chấm kèm cơm',  (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 4000, 'vegan', 1),
('Khoai tây luộc',       'Boiled Potato',         'Khoai tây luộc chín mềm',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 5500, 'vegan', 1),
('Cà rốt muối chua',     'Pickled Carrot',        'Cà rốt ngâm chua ngọt giòn',         (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    3500, 'vegan', 1);

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà rốt' AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà rốt' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua chấm'     AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua chấm'     AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây luộc'       AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt muối chua'     AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt muối chua'     AND i.Name = 'Đường cát trắng';

-- DESSERT — 2 món (nguyên liệu có sẵn trong seed ingredients)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Chè bí đỏ',      'Sweet Pumpkin Dessert', 'Bí đỏ hầm nước dừa đường thanh mát', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 8000, 'vegan', 1),
('Trái cây dầm',   'Fruit in Syrup',        'Dưa leo cà rốt dầm đường lạnh',       (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    7000, 'vegan', 1);

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ'      AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.04, 'lít'   FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ'      AND i.Name = 'Nước dừa tươi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ'      AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Trái cây dầm'  AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Trái cây dầm'  AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Trái cây dầm'  AND i.Name = 'Đường cát trắng';

-- ════════════════════════════════════════════════════
--  NOODLE_SOUP — composite (1 phần = main + soup + vegetable trong meal_structure)
-- ════════════════════════════════════════════════════
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Phở bò', 'Beef Pho', 'Bánh phở, nước dùng hầm xương, thịt bò tái/chín, hành gừng; kèm rau thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 45000, NULL, 1),
('Bánh canh cua', 'Crab Banh Canh', 'Sợi bánh canh gạo, nước dùng hải sản, tôm/chả cua kiểu Nam Bộ, rau thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 42000, NULL, 1);

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.14, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.04, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Hành tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Rau muống';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Mồng tơi';

-- ════════════════════════════════════════════════════
--  DISH_CATEGORIES + dish_dish_categories (slot AI)
--  Code sinh bởi trigger; SlotKey khớp API DishSlotCategoryCodes.
-- ════════════════════════════════════════════════════
INSERT INTO dish_categories (SlotKey, Name, SortOrder) VALUES
('main', 'Món chính', 10),
('side', 'Món phụ', 20),
('soup', 'Canh / súp', 30),
('vegetable', 'Rau / món xanh', 40),
('noodle_soup', 'Món nước / phở / bún', 50),
('dessert', 'Tráng miệng', 60);

-- Gán slot theo nhóm món (dishes không còn cột category)
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'main'
WHERE d.Name IN (
  'Cơm sườn nướng','Cơm gà chiên mắm','Cơm chiên dương châu','Thịt heo kho trứng','Gà kho gừng','Cá lóc kho tộ','Thịt bò xào hành tây',
  'Cơm cá basa kho tộ','Thịt heo xào cà chua','Gà nướng sả','Tôm rang me','Cá basa chiên giòn','Bò xào cà chua','Cơm rang thịt gà','Thịt heo xào sả ớt','Gà hấp gừng'
);
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'soup'
WHERE d.Name IN (
  'Canh chua tôm','Canh bí đỏ thịt bằm','Canh cải ngọt thịt bằm','Canh khổ qua nhồi thịt','Canh mồng tơi tôm',
  'Canh rau củ thịt bằm','Canh cà chua trứng','Canh đậu hũ cà chua','Canh khoai tây thịt bằm','Canh cải ngọt cà chua','Canh khổ qua cà chua'
);
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'vegetable'
WHERE d.Name IN (
  'Rau muống xào tỏi','Đậu hũ sốt cà chua','Cải ngọt xào tỏi','Đậu cove xào thịt','Bí đỏ xào tôm',
  'Cà rốt xào trứng','Khoai tây xào thịt','Bí đỏ xào tỏi','Cải ngọt luộc','Cà chua xào trứng'
);
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'side'
WHERE d.Name IN (
  'Trứng chiên','Khoai tây chiên','Đậu hũ chiên giòn','Trứng luộc','Dưa leo trộn',
  'Salad dưa leo cà rốt','Sốt cà chua chấm','Khoai tây luộc','Cà rốt muối chua'
);
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'dessert'
WHERE d.Name IN (
  'Chè bí đỏ','Trái cây dầm'
);

-- Món nước: noodle_soup
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d, dish_categories c 
WHERE d.Name IN ('Phở bò', 'Bánh canh cua') 
AND c.SlotKey = 'noodle_soup';

-- ════════════════════════════════════════════════════
--  70 MÓN BỔ SUNG PHONG PHÚ (Main: 24 | Soup: 14 | Vegetable: 14 | Side: 12 | Dessert: 4 | Noodle Soup: 2)
-- ════════════════════════════════════════════════════

-- 1. MAIN (24 món mới)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Sườn xào chua ngọt', 'Sweet and Sour Pork Ribs', 'Sườn heo xào sốt cà chua chua ngọt thơm ngon đậm đà', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 26000, NULL, 1),
('Cá lóc hấp hành gừng', 'Steamed Snakehead Fish with Ginger', 'Cá lóc đồng phi lê hấp gừng tươi hành tây thơm ngậy', (SELECT Id FROM cooking_methods WHERE MethodKey = 'steamed' LIMIT 1), 25000, NULL, 1),
('Thịt kho tàu nước dừa', 'Coconut Braised Pork with Egg', 'Thịt heo ba chỉ kho trứng gà ta nước dừa ngọt béo', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 24000, NULL, 1),
('Gà chiên nước mắm', 'Fish Sauce Fried Chicken', 'Cánh tỏi gà ta chiên sốt nước mắm tỏi ớt vàng giòn', (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1), 25000, NULL, 1),
('Thịt bò xào đậu cove', 'Beef Stir-Fried with Green Beans', 'Thịt bò nạc xào đậu cove Đà Lạt giòn ngọt thơm ngon', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 28000, NULL, 1),
('Tôm ram mặn ngọt', 'Salty Sweet Caramelized Shrimp', 'Tôm sú rim mặn ngọt đậm đà đưa cơm ăn kèm dưa trộn', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 30000, NULL, 1),
('Cơm chiên tỏi trứng', 'Garlic Egg Fried Rice', 'Cơm rang hạt gạo ST25 giòn thơm trứng gà ta phi tỏi Lý Sơn', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 18000, NULL, 1),
('Đậu hũ dồn thịt sốt cà chua', 'Tomato Stuffed Tofu with Pork', 'Đậu hũ dồn thịt ba chỉ bằm mịn kho nước cà chua thơm ngon', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 22000, NULL, 1),
('Cá lóc chiên giòn', 'Crispy Fried Snakehead Fish', 'Cá lóc phi lê tẩm bột chiên giòn rụm chấm nước mắm gừng', (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1), 26000, NULL, 1),
('Gà xào sả ớt', 'Lemongrass Chili Chicken Stir-Fry', 'Đùi gà ta xào sả ớt cay nồng bản vị ẩm thực Việt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 23000, NULL, 1),
('Thịt heo luộc', 'Boiled Pork Slice', 'Thịt heo nạc luộc chín vừa thái lát chấm mắm gừng tỏi', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 21000, NULL, 1),
('Thịt bò xào tỏi', 'Garlic Beef Stir-Fry', 'Thịt bò nạc mông thái mỏng xào tỏi Lý Sơn phi thơm béo', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 28000, NULL, 1),
('Tôm nướng muối ớt', 'Chili Salt Grilled Shrimp', 'Tôm sú xiên que nướng muối ớt cay nồng giòn dai ngọt lịm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'grilled' LIMIT 1), 32000, NULL, 1),
('Gà hấp hành', 'Steamed Chicken with Shallots', 'Đùi gà ta hấp hành gừng thơm mát ngọt tự nhiên', (SELECT Id FROM cooking_methods WHERE MethodKey = 'steamed' LIMIT 1), 24000, NULL, 1),
('Cá basa kho gừng', 'Ginger Braised Basa Fish', 'Cá basa phi lê kho gừng sả ngọt lịm ăn kèm canh cải nóng', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 21000, NULL, 1),
('Cơm chiên hải sản', 'Seafood Fried Rice', 'Cơm rang ST25 với tôm sú thái hạt lựu kèm trứng gà tươi', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 28000, NULL, 1),
('Cơm thịt nướng sả', 'Sautéed Lemongrass Pork Rice', 'Thịt heo nạc nướng sả ớt sả ăn kèm cơm ST25 dẻo thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'grilled' LIMIT 1), 25000, NULL, 1),
('Gà kho sả ớt', 'Lemongrass Braised Chicken', 'Thịt gà kho sả ớt cay thơm đậm vị cơm gia đình', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 22000, NULL, 1),
('Thịt heo kho gừng', 'Ginger Braised Pork', 'Thịt heo ba chỉ kho gừng ấm nồng đậm đà giòn ngọt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 21000, NULL, 1),
('Tôm chiên bột giòn', 'Crispy Fried Tempura Shrimp', 'Tôm sú lăn bột chiên vàng ươm giòn rụm chấm tương cà', (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1), 28000, NULL, 1),
('Thịt bò xào hành tây sả', 'Beef Stir-Fried with Onion Lemongrass', 'Thịt bò xào sả hành tây bản vị sần sật ngọt ngào', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 28000, NULL, 1),
('Cơm gà luộc', 'Boiled Chicken Rice', 'Cơm dẻo thơm nấu nước dùng gà kèm thịt gà ta xé phay hành phi', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 25000, NULL, 1),
('Đậu hũ kho tộ mặn ngọt', 'Claypot Braised Savory Tofu', 'Đậu hũ non kho tộ mặn ngọt tiêu đen đưa cơm dân dã', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 16000, 'vegan', 1),
('Cơm thịt xá xíu', 'Char Siu Pork Rice', 'Cơm trắng ST25 ăn kèm thịt heo xá xíu nướng mật ong thơm lừng', (SELECT Id FROM cooking_methods WHERE MethodKey = 'grilled' LIMIT 1), 24000, NULL, 1);

-- 2. SOUP (14 món mới)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Canh thịt bằm khoai tây cà rốt', 'Potato Carrot Pork Soup', 'Canh khoai tây cà rốt bổ dưỡng nấu thịt heo xay thanh ngọt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 8500, NULL, 1),
('Canh cá lóc nấu chua', 'Snakehead Fish Sour Soup with Tomato', 'Canh chua cá lóc đồng nấu cà chua dứa đậu cove chua mát', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 13000, NULL, 1),
('Canh trứng đậu hũ cà chua', 'Tomato Egg Drop Tofu Soup', 'Canh cà chua thả trứng hoa trứng gà và đậu hũ thanh mát', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 7500, NULL, 1),
('Canh bí đỏ nấu tôm', 'Pumpkin Shrimp Soup', 'Canh bí đỏ non hầm mềm tơi nấu tôm sú tươi ngọt lịm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 9500, NULL, 1),
('Canh cải ngọt nấu tôm', 'Bok Choy Shrimp Soup', 'Canh rau cải ngọt nấu tôm sú tươi băm nhỏ thanh giải nhiệt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 9000, NULL, 1),
('Canh khổ qua thịt bằm', 'Bitter Melon Minced Pork Soup', 'Khổ qua bào mỏng nấu canh thịt heo bằm mát gan giải nhiệt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 9000, NULL, 1),
('Canh mồng tơi thịt bằm', 'Malabar Spinach Minced Pork Soup', 'Canh rau mồng tơi trơn ngọt mát lành nấu thịt heo nạc bằm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 7500, NULL, 1),
('Canh cà chua nấu tôm', 'Tomato Shrimp Soup', 'Canh cà chua tươi chín mềm nấu tôm sú bổ dưỡng thanh ngọt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 10000, NULL, 1),
('Canh gà nấu gừng', 'Chicken Ginger Soup', 'Canh thịt gà ta nấu nước gừng tươi ấm áp phục hồi thể lực', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 11000, NULL, 1),
('Canh cá basa nấu chua ngót', 'Basa Fish Sour Soup with Tomato', 'Canh ngót cá basa phi lê nấu cà chua tươi hành tây ngọt nước', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 11000, NULL, 1),
('Canh gà hành tây', 'Chicken Onion Soup', 'Canh đùi gà ta hầm hành tây trắng thanh mát bổ dưỡng ngọt lịm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 11500, NULL, 1),
('Canh rau muống nấu cà chua', 'Water Spinach Tomato Soup', 'Canh rau muống luộc dầm sấu hoặc nấu cà chua thanh dịu ngọt nước', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 6000, 'vegan', 1),
('Canh đậu hũ cải ngọt', 'Tofu Bok Choy Soup', 'Canh cải ngọt nấu đậu hũ non thanh nhẹ tốt cho hệ tiêu hóa', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 6500, 'vegan', 1),
('Canh mồng tơi hành tím', 'Malabar Spinach Shallot Soup', 'Canh rau mồng tơi tươi phi hành tím thơm lành mát da giải nhiệt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 5500, 'vegan', 1);

-- 3. VEGETABLE (14 món mới)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Rau muống luộc', 'Blanched Water Spinach', 'Rau muống luộc xanh mướt giòn sần sật chấm kho quẹt nước mắm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 4500, 'vegan', 1),
('Cải ngọt xào thịt bò', 'Stir-Fried Bok Choy with Beef', 'Cải ngọt xào thịt bò nạc tỏi phi thơm mọng đậm đà', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 12000, NULL, 1),
('Đậu cove xào tỏi', 'Garlic Stir-Fried Green Beans', 'Đậu cove giòn ngọt xào tỏi Lý Sơn phi vàng thơm nức', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Bí đỏ xào hành tím', 'Garlic Shallot Stir-Fried Pumpkin', 'Bí đỏ non ngọt dẻo xào với hành tím phi thơm bùi đưa cơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Cà rốt xào tỏi', 'Garlic Stir-Fried Carrot', 'Cà rốt Đà Lạt thái sợi mỏng xào tỏi giòn ngọt tự nhiên', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 5500, 'vegan', 1),
('Khoai tây xào tỏi', 'Garlic Stir-Fried Potato', 'Khoai tây Đà Lạt cắt lát mỏng xào tỏi phi giòn dẻo đậm vị', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Đậu cove xào tôm', 'Shrimp Stir-Fried with Green Beans', 'Đậu cove xào tôm sú bóc vỏ tươi ngọt béo ngậy', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 10000, NULL, 1),
('Đậu hũ xào rau củ', 'Stir-Fried Tofu with Mixed Vegetables', 'Đậu hũ chiên xào đậu cove cà rốt tỏi phi thanh ngọt nhẹ', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 8500, 'vegan', 1),
('Rau muống xào thịt bò', 'Stir-Fried Water Spinach with Beef', 'Rau muống giòn xào lửa lớn thịt bò nạc tái tỏi thơm nồng', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 12000, NULL, 1),
('Cải ngọt xào tỏi gừng', 'Garlic Ginger Bok Choy Stir-Fry', 'Cải ngọt tươi xào tỏi gừng ấm nóng kích thích tiêu hóa', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 5000, 'vegan', 1),
('Bí đỏ luộc', 'Blanched Sweet Pumpkin', 'Bí đỏ cắt miếng luộc chín dẻo bùi ngọt tự nhiên dồi dào vitamin', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 4500, 'vegan', 1),
('Cà rốt luộc', 'Blanched Carrot', 'Cà rốt Đà Lạt luộc vừa chín giòn thanh mát giữ nguyên dưỡng chất', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 4500, 'vegan', 1),
('Khoai tây luộc chín', 'Boiled Potato Soft', 'Khoai tây Đà Lạt luộc chín bùi bở mềm ngậy tốt cho sức khỏe', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 5000, 'vegan', 1),
('Đậu cove luộc', 'Blanched Green Beans', 'Đậu cove Đà Lạt luộc chín xanh ngọt mọng nước giòn giòn mát lành', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 5000, 'vegan', 1);

-- 4. SIDE (12 món mới)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Trứng cuộn chiên hành', 'Fried Rolled Egg with Shallots', 'Trứng gà ta đánh đều cuộn tròn chiên hành tím thơm mềm ngậy', (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1), 6500, NULL, 1),
('Khoai tây nghiền hành tím', 'Mashed Potatoes and Shallot', 'Khoai tây luộc đánh nhuyễn mịn bùi trộn hành tím phi vàng thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 7000, 'vegan', 1),
('Đậu hũ chiên sả ớt', 'Crispy Lemongrass Fried Tofu', 'Đậu hũ non chiên sả ớt muối vừng vàng rụm sần sật nồng cay', (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1), 6000, 'vegan', 1),
('Dưa leo muối chua', 'Garlic Sweet Pickled Cucumber', 'Dưa leo cắt lát ngâm dấm đường tỏi ớt giòn chua ngọt cực đưa cơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1), 3500, 'vegan', 1),
('Cà rốt trộn chua ngọt', 'Pickled Shredded Carrot', 'Cà rốt bào sợi ngâm dấm tỏi đường chua ngọt sần sật giải ngấy', (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1), 3500, 'vegan', 1),
('Salad dưa leo cà chua', 'Tomato Cucumber Salad with Shallot', 'Dưa leo cà chua thái lát bóp dấm đường tỏi hành tím thanh mát', (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1), 5000, 'vegan', 1),
('Trứng hấp thịt bằm', 'Steamed Egg with Minced Pork', 'Trứng gà ta hấp thịt heo bằm hành tươi mềm ngậy tan trong miệng', (SELECT Id FROM cooking_methods WHERE MethodKey = 'steamed' LIMIT 1), 8000, NULL, 1),
('Đậu hũ luộc chín', 'Plain Boiled Soft Tofu', 'Đậu hũ non luộc nóng hổi mềm mại mộc mạc giàu đạm thực vật', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 4000, 'vegan', 1),
('Khoai tây chiên lắc tỏi', 'Garlic Crispy Fried Potato Fries', 'Khoai tây chiên giòn đảo tỏi sấy Lý Sơn cực kỳ thơm bùi ngậy', (SELECT Id FROM cooking_methods WHERE MethodKey = 'fried' LIMIT 1), 8500, 'vegan', 1),
('Salad dưa leo hành tím', 'Cucumber Salad with Shallots', 'Dưa leo cắt khoanh trộn chua ngọt hành phi giòn tan dễ ăn', (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1), 4000, 'vegan', 1),
('Sốt cà chua hành tỏi', 'Tomato Gravy with Garlic Shallot', 'Sốt cà chua cô đặc phi hành tỏi béo ngọt để chấm cùng cơm mặn', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 3500, 'vegan', 1),
('Cà rốt dưa leo muối', 'Pickled Carrot and Cucumber', 'Cà rốt dưa leo dầm chua ngọt ăn chống ngấy hiệu quả', (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1), 3500, 'vegan', 1);

-- 5. DESSERT (4 món mới)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Chè khoai tây đường cát', 'Sweet Candied Potato Dessert', 'Chè khoai tây Đà Lạt thái hạt lựu hầm nước dừa đường cát ngọt thanh', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 8000, 'vegan', 1),
('Nước dừa tươi ngọt lành', 'Fresh Sweet Coconut Juice Cup', 'Ly nước dừa tươi nguyên chất Bến Tre ngọt lịm giải nhiệt hiệu quả', (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1), 10000, 'vegan', 1),
('Cà rốt ngào đường', 'Sweet Candied Carrots Glaze', 'Mứt cà rốt dẻo ngọt rim đường cát sấy giòn thơm đẹp mắt', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Chè bí đỏ nước dừa ngọt', 'Sweet Pumpkin Dessert with Coconut Milk', 'Chè bí đỏ non dẻo quánh chan nước dừa béo ngọt thanh mát bùi bùi', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 8000, 'vegan', 1);

-- 6. NOODLE SOUP (2 món mới)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Bún bò nạm bò', 'Savory Beef Noodle Soup', 'Bún tươi chan nước hầm xương nạm bò đậm vị gừng sả tỏi hành nồng', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 45000, NULL, 1),
('Bún gà xé phay', 'Chicken Noodle Soup with Herbs', 'Bún tươi chan nước luộc gà thanh ngọt kèm thịt gà ta xé và rau thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 40000, NULL, 1);

-- ════════════════════════════════════════════════════
--  ĐỊNH LƯỢNG NGUYÊN LIỆU (BOM) CHO 70 MÓN MỚI
-- ════════════════════════════════════════════════════

-- 1. BOM — Main
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Sườn xào chua ngọt' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Sườn xào chua ngọt' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Sườn xào chua ngọt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Sườn xào chua ngọt' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc hấp hành gừng' AND i.Name = 'Cá lóc';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc hấp hành gừng' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc hấp hành gừng' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc hấp hành gừng' AND i.Name = 'Tỏi';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt kho tàu nước dừa' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Thịt kho tàu nước dừa' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Thịt kho tàu nước dừa' AND i.Name = 'Nước dừa tươi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Thịt kho tàu nước dừa' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà chiên nước mắm' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Gà chiên nước mắm' AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà chiên nước mắm' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Gà chiên nước mắm' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.14, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào đậu cove' AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào đậu cove' AND i.Name = 'Đậu cove';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào đậu cove' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào đậu cove' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm ram mặn ngọt' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm ram mặn ngọt' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm ram mặn ngọt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Tôm ram mặn ngọt' AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm ram mặn ngọt' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên tỏi trứng' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên tỏi trứng' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên tỏi trứng' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên tỏi trứng' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ dồn thịt sốt cà chua' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ dồn thịt sốt cà chua' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ dồn thịt sốt cà chua' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ dồn thịt sốt cà chua' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc chiên giòn' AND i.Name = 'Cá lóc';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc chiên giòn' AND i.Name = 'Dầu thực vật';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc chiên giòn' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà xào sả ớt' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà xào sả ớt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Gà xào sả ớt' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo luộc' AND i.Name = 'Thịt heo nạc vai';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào tỏi' AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào tỏi' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm nướng muối ớt' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm nướng muối ớt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Tôm nướng muối ớt' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp hành' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp hành' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp hành' AND i.Name = 'Gừng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá basa kho gừng' AND i.Name = 'Cá basa';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá basa kho gừng' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cá basa kho gừng' AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cá basa kho gừng' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên hải sản' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên hải sản' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên hải sản' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên hải sản' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm thịt nướng sả' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm thịt nướng sả' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm thịt nướng sả' AND i.Name = 'Tỏi';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà kho sả ớt' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà kho sả ớt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Gà kho sả ớt' AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Gà kho sả ớt' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho gừng' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho gừng' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho gừng' AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho gừng' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Tôm chiên bột giòn' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Tôm chiên bột giòn' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.14, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây sả' AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây sả' AND i.Name = 'Hành tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây sả' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà luộc' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà luộc' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà luộc' AND i.Name = 'Hành tím';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 3.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ kho tộ mặn ngọt' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ kho tộ mặn ngọt' AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ kho tộ mặn ngọt' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.16, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm thịt xá xíu' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm thịt xá xíu' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cơm thịt xá xíu' AND i.Name = 'Tỏi';

-- 2. BOM — Soup
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh thịt bằm khoai tây cà rốt' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh thịt bằm khoai tây cà rốt' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh thịt bằm khoai tây cà rốt' AND i.Name = 'Cà rốt';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá lóc nấu chua' AND i.Name = 'Cá lóc';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá lóc nấu chua' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá lóc nấu chua' AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá lóc nấu chua' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Canh trứng đậu hũ cà chua' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Canh trứng đậu hũ cà chua' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh trứng đậu hũ cà chua' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh trứng đậu hũ cà chua' AND i.Name = 'Hành tím';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ nấu tôm' AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ nấu tôm' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ nấu tôm' AND i.Name = 'Hành tím';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt nấu tôm' AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt nấu tôm' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt nấu tôm' AND i.Name = 'Hành tím';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua thịt bằm' AND i.Name = 'Khổ qua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua thịt bằm' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua thịt bằm' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi thịt bằm' AND i.Name = 'Mồng tơi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi thịt bằm' AND i.Name = 'Thịt heo nạc vai';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua nấu tôm' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua nấu tôm' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua nấu tôm' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh gà nấu gừng' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh gà nấu gừng' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Canh gà nấu gừng' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá basa nấu chua ngót' AND i.Name = 'Cá basa';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá basa nấu chua ngót' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Canh cá basa nấu chua ngót' AND i.Name = 'Nước mắm';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.12, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh gà hành tây' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh gà hành tây' AND i.Name = 'Hành tây';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh rau muống nấu cà chua' AND i.Name = 'Rau muống';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh rau muống nấu cà chua' AND i.Name = 'Cà chua';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Canh đậu hũ cải ngọt' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh đậu hũ cải ngọt' AND i.Name = 'Cải ngọt';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi hành tím' AND i.Name = 'Mồng tơi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi hành tím' AND i.Name = 'Hành tím';

-- 3. BOM — Vegetable
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.28, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống luộc' AND i.Name = 'Rau muống';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào thịt bò' AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào thịt bò' AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào thịt bò' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào thịt bò' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tỏi' AND i.Name = 'Đậu cove';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tỏi' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào hành tím' AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào hành tím' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào hành tím' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào tỏi' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào tỏi' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào tỏi' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào tỏi' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tôm' AND i.Name = 'Đậu cove';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tôm' AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tôm' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào tôm' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 1.50, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ xào rau củ' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ xào rau củ' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.08, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ xào rau củ' AND i.Name = 'Đậu cove';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ xào rau củ' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào thịt bò' AND i.Name = 'Rau muống';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.06, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào thịt bò' AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào thịt bò' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào thịt bò' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi gừng' AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi gừng' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi gừng' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi gừng' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ luộc' AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt luộc' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây luộc chín' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.22, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove luộc' AND i.Name = 'Đậu cove';

-- 4. BOM — Side
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Trứng cuộn chiên hành' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Trứng cuộn chiên hành' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Trứng cuộn chiên hành' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây nghiền hành tím' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây nghiền hành tím' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ chiên sả ớt' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ chiên sả ớt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ chiên sả ớt' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo muối chua' AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo muối chua' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo muối chua' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt trộn chua ngọt' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt trộn chua ngọt' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt trộn chua ngọt' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà chua' AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà chua' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà chua' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà chua' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Trứng hấp thịt bằm' AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Trứng hấp thịt bằm' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Trứng hấp thịt bằm' AND i.Name = 'Hành tím';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ luộc chín' AND i.Name = 'Đậu hũ';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây chiên lắc tỏi' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây chiên lắc tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây chiên lắc tỏi' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo hành tím' AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo hành tím' AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo hành tím' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua hành tỏi' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua hành tỏi' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua hành tỏi' AND i.Name = 'Dầu thực vật';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt dưa leo muối' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt dưa leo muối' AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt dưa leo muối' AND i.Name = 'Đường cát trắng';

-- 5. BOM — Dessert
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.20, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Chè khoai tây đường cát' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Chè khoai tây đường cát' AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.04, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Chè khoai tây đường cát' AND i.Name = 'Nước dừa tươi';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.25, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Nước dừa tươi ngọt lành' AND i.Name = 'Nước dừa tươi';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.15, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt ngào đường' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt ngào đường' AND i.Name = 'Đường cát trắng';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.18, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ nước dừa ngọt' AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.05, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ nước dừa ngọt' AND i.Name = 'Nước dừa tươi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.03, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ nước dừa ngọt' AND i.Name = 'Đường cát trắng';

-- 6. BOM — Noodle Soup
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.14, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún bò nạm bò' AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún bò nạm bò' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Bún bò nạm bò' AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún bò nạm bò' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún bò nạm bò' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún bò nạm bò' AND i.Name = 'Hành tím';

INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.14, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún gà xé phay' AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.10, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún gà xé phay' AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Bún gà xé phay' AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún gà xé phay' AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún gà xé phay' AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, DishValueId, Quantity, Unit) SELECT d.Id, i.Id, (SELECT Id FROM dish_values WHERE Amount = 30000 LIMIT 1), 0.01, 'kg' FROM dishes d, ingredients i WHERE d.Name = 'Bún gà xé phay' AND i.Name = 'Hành tím';

-- ════════════════════════════════════════════════════
--  GÁN SLOT BỮA ĂN CHO 70 MÓN MỚI (DISH_DISH_CATEGORIES)
-- ════════════════════════════════════════════════════

-- 1. Main slot mapping
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'main'
WHERE d.Name IN (
  'Sườn xào chua ngọt', 'Cá lóc hấp hành gừng', 'Thịt kho tàu nước dừa', 'Gà chiên nước mắm', 'Thịt bò xào đậu cove',
  'Tôm ram mặn ngọt', 'Cơm chiên tỏi trứng', 'Đậu hũ dồn thịt sốt cà chua', 'Cá lóc chiên giòn', 'Gà xào sả ớt',
  'Thịt heo luộc', 'Thịt bò xào tỏi', 'Tôm nướng muối ớt', 'Gà hấp hành', 'Cá basa kho gừng', 'Cơm chiên hải sản',
  'Cơm thịt nướng sả', 'Gà kho sả ớt', 'Thịt heo kho gừng', 'Tôm chiên bột giòn', 'Thịt bò xào hành tây sả',
  'Cơm gà luộc', 'Đậu hũ kho tộ mặn ngọt', 'Cơm thịt xá xíu'
);

-- 2. Soup slot mapping
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'soup'
WHERE d.Name IN (
  'Canh thịt bằm khoai tây cà rốt', 'Canh cá lóc nấu chua', 'Canh trứng đậu hũ cà chua', 'Canh bí đỏ nấu tôm', 'Canh cải ngọt nấu tôm',
  'Canh khổ qua thịt bằm', 'Canh mồng tơi thịt bằm', 'Canh cà chua nấu tôm', 'Canh gà nấu gừng', 'Canh cá basa nấu chua ngót',
  'Canh gà hành tây', 'Canh rau muống nấu cà chua', 'Canh đậu hũ cải ngọt', 'Canh mồng tơi hành tím'
);

-- 3. Vegetable slot mapping
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'vegetable'
WHERE d.Name IN (
  'Rau muống luộc', 'Cải ngọt xào thịt bò', 'Đậu cove xào tỏi', 'Bí đỏ xào hành tím', 'Cà rốt xào tỏi',
  'Khoai tây xào tỏi', 'Đậu cove xào tôm', 'Đậu hũ xào rau củ', 'Rau muống xào thịt bò', 'Cải ngọt xào tỏi gừng',
  'Bí đỏ luộc', 'Cà rốt luộc', 'Khoai tây luộc chín', 'Đậu cove luộc'
);

-- 4. Side slot mapping
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'side'
WHERE d.Name IN (
  'Trứng cuộn chiên hành', 'Khoai tây nghiền hành tím', 'Đậu hũ chiên sả ớt', 'Dưa leo muối chua', 'Cà rốt trộn chua ngọt',
  'Salad dưa leo cà chua', 'Trứng hấp thịt bằm', 'Đậu hũ luộc chín', 'Khoai tây chiên lắc tỏi', 'Salad dưa leo hành tím',
  'Sốt cà chua hành tỏi', 'Cà rốt dưa leo muối'
);

-- 5. Dessert slot mapping
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d INNER JOIN dish_categories c ON c.SlotKey = 'dessert'
WHERE d.Name IN (
  'Chè khoai tây đường cát', 'Nước dừa tươi ngọt lành', 'Cà rốt ngào đường', 'Chè bí đỏ nước dừa ngọt'
);

-- 6. Noodle Soup mapping
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d, dish_categories c 
WHERE d.Name IN ('Bún bò nạm bò', 'Bún gà xé phay') 
AND c.SlotKey = 'noodle_soup';
