-- =====================================
-- SmartLunch Database - INCREMENTAL UPDATE (TOÀN BỘ PATCH)
-- Không xóa dữ liệu — bổ sung schema / quyền / bảng mới (idempotent, chạy lại được)
--
-- Cách chạy (từ thư mục Data\):
--   run_sql.bat          hoặc   run_sql.bat update
--   → mysql trong Docker container, thực thi TOÀN BỘ các SOURCE bên dưới
--
-- DB chưa tồn tại / muốn reset + seed lại:
--   run_sql.bat reset    → run_all.sql (init + seed + lại file này)
-- =====================================

USE SmartLunch;

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
SET character_set_connection = utf8mb4;

-- ---------- 05_maintenance (schema patches) ----------
SOURCE 05_maintenance/02_orders_annex_columns.sql;
SOURCE 05_maintenance/03_warehousestaff_grant_partner_perms.sql;
SOURCE 05_maintenance/05_order_items_service_date.sql;
SOURCE 05_maintenance/06_orders_promotion_columns.sql;
SOURCE 05_maintenance/07_promotions_permissions.sql;
SOURCE 05_maintenance/08_promotions_tables.sql;
SOURCE 05_maintenance/08_systems_backup_permissions.sql;

-- ---------- 01_tables (bổ sung bảng/cột cho DB cũ) ----------
SOURCE 01_tables/50_system_backup_schedule.sql;
SOURCE 01_tables/51_system_backups_add_source.sql;

-- ---------- migrations (theo thứ tự số, thêm file mới vào cuối danh sách) ----------
SOURCE migrations/add_partner_contract_supplier_fields.sql;
SOURCE migrations/52_contracts_add_source_order_id.sql;
SOURCE migrations/53_company_public_documents.sql;
SOURCE migrations/54_reviews_manager_reply.sql;
SOURCE migrations/55_manager_intake_permissions.sql;
SOURCE migrations/56_contact_inquiries.sql;
SOURCE migrations/57_warehousestaff_partner_payment_create.sql;
SOURCE migrations/58_orders_recipient_delivery_email.sql;

-- ---------- Tối ưu thống kê (tùy chọn, an toàn) ----------
SOURCE 05_maintenance/01_analyze_optimize.sql;

SELECT 'SmartLunch incremental update (run_update.sql) completed!' AS Status;
