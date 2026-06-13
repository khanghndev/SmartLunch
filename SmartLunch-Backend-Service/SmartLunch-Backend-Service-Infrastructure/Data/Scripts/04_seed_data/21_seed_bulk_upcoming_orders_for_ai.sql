-- =====================================================
-- Seed: BULK upcoming orders for AI ingredient prep testing
-- - Creates many future orders + order_items across active contracts
-- - Ensures chosen dishes have BOM (dish_ingredients) so AI demand isn't empty
-- - Idempotent: removes previous SEED-AI-* orders first
--
-- Usage:
--   mysql -u root -p < run_all.sql
--   or run_sql.bat reset
-- =====================================================

USE SmartLunch;

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

-- ---- Parameters ----
SET @seed_start_date = DATE_ADD(CURDATE(), INTERVAL 1 DAY);  -- tomorrow
SET @seed_days = 21;                                        -- horizon
SET @orders_per_day_per_contract = 2;                       -- scale
SET @items_per_order = 4;                                  -- dishes per order
SET @min_qty = 10;
SET @max_qty = 120;

-- ---- Cleanup previous seed ----
DELETE oi
FROM order_items oi
INNER JOIN orders o ON o.Id = oi.OrderId
WHERE o.InvoiceCode LIKE 'SEED-AI-%';

DELETE FROM orders
WHERE InvoiceCode LIKE 'SEED-AI-%';

-- ---- Build helper temporary tables ----
DROP TEMPORARY TABLE IF EXISTS tmp_seed_contracts;
CREATE TEMPORARY TABLE tmp_seed_contracts AS
SELECT
  c.Id AS ContractId,
  c.OrganizationId,
  c.DishValueId,
  c.MealUnitPrice
FROM contracts c
WHERE c.Status = 'active'
  AND c.OrganizationId IS NOT NULL;

DROP TEMPORARY TABLE IF EXISTS tmp_seed_dates;
CREATE TEMPORARY TABLE tmp_seed_dates AS
WITH RECURSIVE seq AS (
  SELECT 0 AS n
  UNION ALL
  SELECT n + 1 FROM seq WHERE n + 1 < @seed_days
)
SELECT DATE_ADD(@seed_start_date, INTERVAL n DAY) AS ServiceDate
FROM seq;

DROP TEMPORARY TABLE IF EXISTS tmp_seed_order_seq;
CREATE TEMPORARY TABLE tmp_seed_order_seq AS
WITH RECURSIVE seq AS (
  SELECT 1 AS n
  UNION ALL
  SELECT n + 1 FROM seq WHERE n + 1 <= @orders_per_day_per_contract
)
SELECT n AS OrdSeq FROM seq;

-- ---- Insert future orders ----
INSERT INTO orders (
  UserId,
  ContractId,
  OrderDate,
  ScheduledDate,
  Status,
  TotalAmount,
  PaymentStatus,
  InvoiceCode,
  CreatedBySalesUserId,
  CreatedAt
)
SELECT
  (
    SELECT u.Id
    FROM users u
    INNER JOIN user_organizations uo ON uo.UserId = u.Id
    WHERE uo.OrganizationId = sc.OrganizationId
    ORDER BY u.Id
    LIMIT 1
  ) AS UserId,
  sc.ContractId,
  NOW() AS OrderDate,
  d.ServiceDate AS ScheduledDate,
  'confirmed' AS Status,
  0 AS TotalAmount,
  'unpaid' AS PaymentStatus,
  CONCAT(
    'SEED-AI-',
    DATE_FORMAT(d.ServiceDate, '%Y%m%d'),
    '-C', sc.ContractId,
    '-N', os.OrdSeq
  ) AS InvoiceCode,
  (SELECT Id FROM users WHERE Username = 'nhanvien_banhang' LIMIT 1) AS CreatedBySalesUserId,
  NOW() AS CreatedAt
FROM tmp_seed_contracts sc
CROSS JOIN tmp_seed_dates d
CROSS JOIN tmp_seed_order_seq os;

-- ---- Collect inserted order ids ----
DROP TEMPORARY TABLE IF EXISTS tmp_seed_orders;
CREATE TEMPORARY TABLE tmp_seed_orders AS
SELECT
  o.Id AS OrderId,
  o.ContractId,
  DATE(o.ScheduledDate) AS ServiceDate,
  c.DishValueId,
  c.MealUnitPrice
FROM orders o
INNER JOIN contracts c ON c.Id = o.ContractId
WHERE o.InvoiceCode LIKE 'SEED-AI-%'
  AND DATE(o.ScheduledDate) >= @seed_start_date
  AND DATE(o.ScheduledDate) < DATE_ADD(@seed_start_date, INTERVAL @seed_days DAY);

-- ---- Dish candidates: only dishes that have any BOM lines (dish_ingredients) ----
DROP TEMPORARY TABLE IF EXISTS tmp_seed_dish_candidates;
CREATE TEMPORARY TABLE tmp_seed_dish_candidates AS
SELECT DISTINCT
  d.Id AS DishId
FROM dishes d
WHERE d.IsActive = 1
  AND EXISTS (
    SELECT 1
    FROM dish_ingredients di
    WHERE di.DishId = d.Id
  );

-- ---- Insert order items (random dishes, random quantities) ----
INSERT INTO order_items (OrderId, DishId, Quantity, ServiceDate, UnitPrice, TotalPrice)
SELECT
  x.OrderId,
  x.DishId,
  (@min_qty + FLOOR(RAND() * (@max_qty - @min_qty + 1))) AS Quantity,
  x.ServiceDate,
  0.00 AS UnitPrice,
  0.00 AS TotalPrice
FROM (
  SELECT
    so.OrderId,
    dc.DishId,
    so.ServiceDate,
    ROW_NUMBER() OVER (PARTITION BY so.OrderId ORDER BY RAND()) as rn
  FROM tmp_seed_orders so
  CROSS JOIN tmp_seed_dish_candidates dc
) x
WHERE x.rn <= @items_per_order AND MOD(x.rn + x.OrderId, 7) <> 0;


-- Fill unit price from contract.MealUnitPrice or dish catalog price
UPDATE order_items oi
INNER JOIN orders o ON o.Id = oi.OrderId
INNER JOIN contracts c ON c.Id = o.ContractId
INNER JOIN dishes d ON d.Id = oi.DishId
SET
  oi.UnitPrice = COALESCE(c.MealUnitPrice, d.Price),
  oi.TotalPrice = oi.Quantity * COALESCE(c.MealUnitPrice, d.Price)
WHERE o.InvoiceCode LIKE 'SEED-AI-%';

-- ---- Recompute order totals from items ----
UPDATE orders o
JOIN (
  SELECT OrderId, SUM(TotalPrice) AS Total
  FROM order_items
  GROUP BY OrderId
) x ON x.OrderId = o.Id
SET o.TotalAmount = x.Total
WHERE o.InvoiceCode LIKE 'SEED-AI-%';

SELECT
  COUNT(*) AS SeededOrders,
  (SELECT COUNT(*) FROM order_items oi INNER JOIN orders o2 ON o2.Id = oi.OrderId WHERE o2.InvoiceCode LIKE 'SEED-AI-%') AS SeededOrderItems
FROM orders o
WHERE o.InvoiceCode LIKE 'SEED-AI-%';

