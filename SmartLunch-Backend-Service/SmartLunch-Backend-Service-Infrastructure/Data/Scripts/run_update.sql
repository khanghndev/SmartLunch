-- =====================================
-- SmartLunch Database - Incremental Update
-- Không xóa dữ liệu — chỉ bổ sung schema / quyền / maintenance
--
-- Dùng khi DB đã tồn tại (dev/staging/production local).
-- Reset toàn bộ: chạy run_all.sql (qua run_sql.bat reset)
-- =====================================

USE SmartLunch;

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
SET character_set_connection = utf8mb4;

-- Step 5: Maintenance (idempotent — chạy lại được)
SOURCE 05_maintenance/02_orders_annex_columns.sql;
SOURCE 05_maintenance/03_warehousestaff_grant_partner_perms.sql;
SOURCE 05_maintenance/05_order_items_service_date.sql;
SOURCE 05_maintenance/06_orders_promotion_columns.sql;
SOURCE 05_maintenance/07_promotions_permissions.sql;
SOURCE 05_maintenance/08_promotions_tables.sql;

-- Tối ưu thống kê (tùy chọn, an toàn)
SOURCE 05_maintenance/01_analyze_optimize.sql;

SELECT 'SmartLunch database update completed!' AS Status;
