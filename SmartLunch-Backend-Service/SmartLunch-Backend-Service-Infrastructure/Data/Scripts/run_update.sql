-- =====================================
-- SmartLunch Database - Incremental Update
-- Không xóa dữ liệu — chỉ bổ sung schema / quyền / maintenance (idempotent)
--
-- Dùng khi DB đã tồn tại: run_sql.bat hoặc run_sql.bat update
-- Reset toàn bộ (init + seed + bước này): run_sql.bat reset → run_all.sql
-- =====================================

USE SmartLunch;

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
SET character_set_connection = utf8mb4;

-- Maintenance & schema patches
SOURCE 05_maintenance/02_orders_annex_columns.sql;
SOURCE 05_maintenance/03_warehousestaff_grant_partner_perms.sql;
SOURCE 05_maintenance/05_order_items_service_date.sql;
SOURCE 05_maintenance/06_orders_promotion_columns.sql;
SOURCE 05_maintenance/07_promotions_permissions.sql;
SOURCE 05_maintenance/08_promotions_tables.sql;
SOURCE 05_maintenance/08_systems_backup_permissions.sql;

SOURCE 01_tables/50_system_backup_schedule.sql;
SOURCE 01_tables/51_system_backups_add_source.sql;
SOURCE migrations/52_contracts_add_source_order_id.sql;
SOURCE migrations/53_company_public_documents.sql;
SOURCE migrations/54_reviews_manager_reply.sql;
SOURCE migrations/55_manager_intake_permissions.sql;

-- Tối ưu thống kê (tùy chọn, an toàn)
SOURCE 05_maintenance/01_analyze_optimize.sql;

SELECT 'SmartLunch database update completed!' AS Status;
