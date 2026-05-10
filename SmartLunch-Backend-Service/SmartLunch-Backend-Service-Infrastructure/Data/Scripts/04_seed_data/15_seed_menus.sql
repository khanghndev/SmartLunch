-- =====================================================
-- Seed: Weekly Menus & Schedule
-- =====================================================
SET @admin_id = (SELECT Id FROM users WHERE Username = 'admin');

-- Create 1 weekly menu per customer_type (idempotent; unique key includes CustomerTypeId).
INSERT IGNORE INTO weekly_menus (StartDate, EndDate, MenuType, CustomerTypeId, Description, CreatedBy)
SELECT
  '2025-03-24',
  '2025-03-28',
  'General',
  ct.Id,
  CONCAT('Thực đơn tuần 4 tháng 3/2025 - ', ct.Name),
  @admin_id
FROM customer_types ct;

INSERT IGNORE INTO weekly_menus (StartDate, EndDate, MenuType, CustomerTypeId, Description, CreatedBy)
SELECT
  '2025-03-31',
  '2025-04-04',
  'General',
  ct.Id,
  CONCAT('Thực đơn tuần 1 tháng 4/2025 - ', ct.Name),
  @admin_id
FROM customer_types ct;

-- Resolve dish ids once (dishes are seeded before this script in run_all.sql)
SET @d1 = (SELECT Id FROM dishes WHERE Name = 'Cơm sườn nướng' LIMIT 1);
SET @d2 = (SELECT Id FROM dishes WHERE Name = 'Rau muống xào tỏi' LIMIT 1);
SET @d3 = (SELECT Id FROM dishes WHERE Name = 'Canh chua tôm' LIMIT 1);
SET @d4 = (SELECT Id FROM dishes WHERE Name = 'Cơm gà chiên mắm' LIMIT 1);
SET @d5 = (SELECT Id FROM dishes WHERE Name = 'Đậu hũ sốt cà chua' LIMIT 1);
SET @d6 = (SELECT Id FROM dishes WHERE Name = 'Trứng chiên' LIMIT 1);
SET @d7 = (SELECT Id FROM dishes WHERE Name = 'Cơm chiên dương châu' LIMIT 1);
SET @d8 = (SELECT Id FROM dishes WHERE Name = 'Khoai tây chiên' LIMIT 1);

-- =====================================================
-- Full menu by customer_type (Mon-Fri lunch, 3 dishes/day)
-- Idempotent: INSERT IGNORE to respect UK_menu_schedule_combo
-- =====================================================

-- Week 1 (2025-03-24 .. 2025-03-28)
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-24', 'lunch', @d1 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-24', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-24', 'lunch', @d3 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-25', 'lunch', @d4 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-25', 'lunch', @d5 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-25', 'lunch', @d6 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-26', 'lunch', @d7 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-26', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-26', 'lunch', @d3 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-27', 'lunch', @d1 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-27', 'lunch', @d8 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-27', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-28', 'lunch', @d4 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-28', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-28', 'lunch', @d3 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-24';

-- Week 2 (2025-03-31 .. 2025-04-04)
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-31', 'lunch', @d7 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-31', 'lunch', @d3 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-03-31', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-01', 'lunch', @d1 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-01', 'lunch', @d5 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-01', 'lunch', @d6 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-02', 'lunch', @d4 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-02', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-02', 'lunch', @d3 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-03', 'lunch', @d7 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-03', 'lunch', @d5 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-03', 'lunch', @d2 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';

INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-04', 'lunch', @d1 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-04', 'lunch', @d8 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
INSERT IGNORE INTO menu_schedule (MenuId, Date, MealSlot, DishId)
SELECT wm.Id, '2025-04-04', 'lunch', @d3 FROM weekly_menus wm WHERE wm.StartDate = '2025-03-31';
