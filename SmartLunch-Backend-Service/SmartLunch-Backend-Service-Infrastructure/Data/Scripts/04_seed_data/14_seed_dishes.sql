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
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm sườn nướng'        AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm sườn nướng'        AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.25, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm'      AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm'      AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.03, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm gà chiên mắm'      AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 1.00, 'quả'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm chiên dương châu'   AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho trứng'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 1.00, 'quả'  FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho trứng'    AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo kho trứng'    AND i.Name = 'Nước dừa tươi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.22, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà kho gừng'           AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà kho gừng'           AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cá lóc kho tộ'         AND i.Name = 'Cá lóc';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.14, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây'  AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt bò xào hành tây'  AND i.Name = 'Hành tây';

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

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm cá basa kho tộ'      AND i.Name = 'Cá basa';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm cá basa kho tộ'      AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm cá basa kho tộ'      AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.16, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào cà chua'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào cà chua'    AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.22, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà nướng sả'             AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà nướng sả'             AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.01, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà nướng sả'             AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Tôm rang me'             AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.03, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Tôm rang me'             AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cá basa chiên giòn'      AND i.Name = 'Cá basa';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'lít'  FROM dishes d, ingredients i WHERE d.Name = 'Cá basa chiên giòn'      AND i.Name = 'Dầu thực vật';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.13, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bò xào cà chua'          AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bò xào cà chua'          AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm rang thịt gà'        AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Cơm rang thịt gà'        AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 1.00, 'quả'  FROM dishes d, ingredients i WHERE d.Name = 'Cơm rang thịt gà'        AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.17, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào sả ớt'      AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Thịt heo xào sả ớt'      AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.22, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp gừng'             AND i.Name = 'Thịt gà ta';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Gà hấp gừng'             AND i.Name = 'Gừng';

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
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh chua tôm'           AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh chua tôm'           AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ thịt bằm'    AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh bí đỏ thịt bằm'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt thịt bằm' AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt thịt bằm' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua nhồi thịt' AND i.Name = 'Khổ qua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua nhồi thịt' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi tôm'      AND i.Name = 'Mồng tơi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.06, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh mồng tơi tôm'      AND i.Name = 'Tôm sú';

-- SOUP — bổ sung (6 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Canh rau củ thịt bằm',    'Mixed Vegetable Minced Pork Soup',  'Canh cà rốt khoai tây nấu thịt heo xay',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7500, NULL,    1),
('Canh cà chua trứng',      'Tomato Egg Drop Soup',                'Canh cà chua chua ngọt kèm trứng',                 (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  6500, NULL,    1),
('Canh đậu hũ cà chua',     'Tofu Tomato Soup',                    'Đậu hũ non nấu cà chua thanh mát',                 (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7000, 'vegan', 1),
('Canh khoai tây thịt bằm', 'Potato Minced Pork Soup',             'Khoai tây nấu thịt heo xay bùi ngọt',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  8000, NULL,    1),
('Canh cải ngọt cà chua',   'Bok Choy Tomato Soup',                'Cải ngọt nấu cà chua tươi',                         (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  7000, 'vegan', 1),
('Canh khổ qua cà chua',    'Bitter Melon Tomato Soup',            'Khổ qua nấu cà chua giảm đắng',                    (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),  9000, NULL,    1);

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh rau củ thịt bằm'    AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh rau củ thịt bằm'    AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh rau củ thịt bằm'    AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua trứng'      AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 1.00, 'quả' FROM dishes d, ingredients i WHERE d.Name = 'Canh cà chua trứng'      AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Canh đậu hũ cà chua'     AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh đậu hũ cà chua'     AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khoai tây thịt bằm' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khoai tây thịt bằm' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt cà chua'   AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh cải ngọt cà chua'   AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua cà chua'    AND i.Name = 'Khổ qua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'  FROM dishes d, ingredients i WHERE d.Name = 'Canh khổ qua cà chua'    AND i.Name = 'Cà chua';

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
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.30, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào tỏi'  AND i.Name = 'Rau muống';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Rau muống xào tỏi'  AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ sốt cà chua' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ sốt cà chua' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.25, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi'   AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt xào tỏi'   AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào thịt'  AND i.Name = 'Đậu cove';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Đậu cove xào thịt'  AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tôm'      AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tôm'      AND i.Name = 'Tôm sú';

-- VEGETABLE — bổ sung (5 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Cà rốt xào trứng',   'Carrot Egg Stir-Fry',        'Cà rốt xào trứng mềm ngọt',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 5500, NULL,    1),
('Khoai tây xào thịt', 'Potato Pork Stir-Fry',       'Khoai tây xào thịt heo thái lát',   (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 7500, NULL,    1),
('Bí đỏ xào tỏi',      'Garlic Stir-Fried Pumpkin',  'Bí đỏ non xào tỏi phi',              (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6000, 'vegan', 1),
('Cải ngọt luộc',      'Blanched Bok Choy',          'Cải ngọt luộc vừa chín giòn',       (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1),     4500, 'vegan', 1),
('Cà chua xào trứng',  'Tomato Egg Stir-Fry',        'Cà chua xào trứng đậm đà',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'stir_fried' LIMIT 1), 6500, NULL,    1);

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.22, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào trứng'   AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 2.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt xào trứng'   AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào thịt' AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.06, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây xào thịt' AND i.Name = 'Thịt heo nạc vai';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tỏi'      AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Bí đỏ xào tỏi'      AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.28, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cải ngọt luộc'     AND i.Name = 'Cải ngọt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà chua xào trứng' AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 2.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Cà chua xào trứng' AND i.Name = 'Trứng gà';

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
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 2.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Trứng chiên'       AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây chiên'   AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 2.00, 'miếng' FROM dishes d, ingredients i WHERE d.Name = 'Đậu hũ chiên giòn' AND i.Name = 'Đậu hũ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 1.00, 'quả'   FROM dishes d, ingredients i WHERE d.Name = 'Trứng luộc'        AND i.Name = 'Trứng gà';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo trộn'      AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.01, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Dưa leo trộn'      AND i.Name = 'Tỏi';

-- SIDE — bổ sung (4 món)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Salad dưa leo cà rốt', 'Cucumber Carrot Salad', 'Dưa leo cà rốt trộn chua ngọt',     (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    5000, 'vegan', 1),
('Sốt cà chua chấm',     'Tomato Dipping Sauce',  'Sốt cà chua nấu nhừ chấm kèm cơm',  (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 4000, 'vegan', 1),
('Khoai tây luộc',       'Boiled Potato',         'Khoai tây luộc chín mềm',           (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 5500, 'vegan', 1),
('Cà rốt muối chua',     'Pickled Carrot',        'Cà rốt ngâm chua ngọt giòn',         (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    3500, 'vegan', 1);

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà rốt' AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Salad dưa leo cà rốt' AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.15, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua chấm'     AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Sốt cà chua chấm'     AND i.Name = 'Tỏi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.22, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Khoai tây luộc'       AND i.Name = 'Khoai tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.18, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt muối chua'     AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Cà rốt muối chua'     AND i.Name = 'Đường cát trắng';

-- DESSERT — 2 món (nguyên liệu có sẵn trong seed ingredients)
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Chè bí đỏ',      'Sweet Pumpkin Dessert', 'Bí đỏ hầm nước dừa đường thanh mát', (SELECT Id FROM cooking_methods WHERE MethodKey = 'stewed' LIMIT 1), 8000, 'vegan', 1),
('Trái cây dầm',   'Fruit in Syrup',        'Dưa leo cà rốt dầm đường lạnh',       (SELECT Id FROM cooking_methods WHERE MethodKey = 'raw' LIMIT 1),    7000, 'vegan', 1);

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.20, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ'      AND i.Name = 'Bí đỏ';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.04, 'lít'   FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ'      AND i.Name = 'Nước dừa tươi';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.03, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Chè bí đỏ'      AND i.Name = 'Đường cát trắng';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Trái cây dầm'  AND i.Name = 'Dưa leo';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Trái cây dầm'  AND i.Name = 'Cà rốt';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.03, 'kg'    FROM dishes d, ingredients i WHERE d.Name = 'Trái cây dầm'  AND i.Name = 'Đường cát trắng';

-- ════════════════════════════════════════════════════
--  NOODLE_SOUP — composite (1 phần = main + soup + vegetable trong meal_structure)
-- ════════════════════════════════════════════════════
INSERT INTO dishes (Name, NameEnglish, Description, CookingMethodId, Price, DietaryLabel, IsActive) VALUES
('Phở bò', 'Beef Pho', 'Bánh phở, nước dùng hầm xương, thịt bò tái/chín, hành gừng; kèm rau thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 45000, NULL, 1),
('Bánh canh cua', 'Crab Banh Canh', 'Sợi bánh canh gạo, nước dùng hải sản, tôm/chả cua kiểu Nam Bộ, rau thơm', (SELECT Id FROM cooking_methods WHERE MethodKey = 'boiled' LIMIT 1), 42000, NULL, 1);

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.14, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Thịt bò';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.04, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Hành tây';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.01, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Gừng';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.06, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Phở bò'           AND i.Name = 'Rau muống';

INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.12, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Tôm sú';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.10, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Gạo ST25';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'lít' FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Nước mắm';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.08, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Cà chua';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.02, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Hành tím';
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit) SELECT d.Id, i.Id, 0.05, 'kg'   FROM dishes d, ingredients i WHERE d.Name = 'Bánh canh cua'    AND i.Name = 'Mồng tơi';

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

-- Món nước composite: main + soup + vegetable + noodle_soup (AI primary = noodle_soup)
INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d, dish_categories c WHERE d.Name = 'Phở bò' AND c.SlotKey = 'main';

INSERT INTO dish_dish_categories (DishId, DishCategoryId)
SELECT d.Id, c.Id FROM dishes d, dish_categories c WHERE d.Name = 'Bánh canh cua' AND c.SlotKey = 'main';
