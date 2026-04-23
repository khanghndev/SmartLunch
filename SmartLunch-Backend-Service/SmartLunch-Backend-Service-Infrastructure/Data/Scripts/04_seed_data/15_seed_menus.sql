-- =====================================================
-- Seed: Weekly Menus & Schedule
-- =====================================================
SET @admin_id = (SELECT Id FROM users WHERE Username = 'admin');

INSERT INTO weekly_menus (StartDate, EndDate, Description, CreatedBy) VALUES
('2025-03-24', '2025-03-28', 'Thực đơn tuần 4 tháng 3/2025', @admin_id),
('2025-03-31', '2025-04-04', 'Thực đơn tuần 1 tháng 4/2025', @admin_id);

SET @menu1 = (SELECT Id FROM weekly_menus WHERE StartDate = '2025-03-24');
SET @menu2 = (SELECT Id FROM weekly_menus WHERE StartDate = '2025-03-31');

-- Week 1 schedule
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-24', 'lunch', Id FROM dishes WHERE Name = 'Cơm sườn nướng';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-24', 'lunch', Id FROM dishes WHERE Name = 'Rau muống xào tỏi';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-24', 'lunch', Id FROM dishes WHERE Name = 'Canh chua tôm';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-25', 'lunch', Id FROM dishes WHERE Name = 'Cơm gà chiên mắm';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-25', 'lunch', Id FROM dishes WHERE Name = 'Đậu hũ sốt cà chua';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-25', 'lunch', Id FROM dishes WHERE Name = 'Trứng chiên';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-26', 'lunch', Id FROM dishes WHERE Name = 'Cơm chiên dương châu';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-26', 'lunch', Id FROM dishes WHERE Name = 'Rau muống xào tỏi';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-27', 'lunch', Id FROM dishes WHERE Name = 'Cơm sườn nướng';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-27', 'lunch', Id FROM dishes WHERE Name = 'Khoai tây chiên';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-28', 'lunch', Id FROM dishes WHERE Name = 'Cơm gà chiên mắm';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu1, '2025-03-28', 'lunch', Id FROM dishes WHERE Name = 'Rau muống xào tỏi';

-- Week 2 schedule
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu2, '2025-03-31', 'lunch', Id FROM dishes WHERE Name = 'Cơm chiên dương châu';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu2, '2025-03-31', 'lunch', Id FROM dishes WHERE Name = 'Canh chua tôm';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu2, '2025-04-01', 'lunch', Id FROM dishes WHERE Name = 'Cơm sườn nướng';
INSERT INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT @menu2, '2025-04-01', 'lunch', Id FROM dishes WHERE Name = 'Đậu hũ sốt cà chua';
