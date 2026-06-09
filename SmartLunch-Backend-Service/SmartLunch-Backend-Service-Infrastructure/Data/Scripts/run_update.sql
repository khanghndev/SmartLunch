-- =====================================
-- SmartLunch Database - INCREMENTAL UPDATE
-- Không xóa dữ liệu — bổ sung schema / quyền (idempotent, chạy lại được)
--
-- Chạy: run_sql.bat update
-- Reset + seed: run_sql.bat reset  (hoặc run_sql.bat mặc định)
-- =====================================

USE SmartLunch;

SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
SET character_set_connection = utf8mb4;

-- ---------- 05_maintenance (schema patches cũ) ----------
SOURCE 05_maintenance/02_orders_annex_columns.sql;
SOURCE 05_maintenance/03_warehousestaff_grant_partner_perms.sql;
SOURCE 05_maintenance/05_order_items_service_date.sql;
SOURCE 05_maintenance/06_orders_promotion_columns.sql;
SOURCE 05_maintenance/07_promotions_permissions.sql;
SOURCE 05_maintenance/08_promotions_tables.sql;
SOURCE 05_maintenance/08_systems_backup_permissions.sql;

-- ---------- 01_tables (bổ sung bảng cho DB cũ) ----------
SOURCE 01_tables/50_system_backup_schedule.sql;
SOURCE 01_tables/51_system_backups_add_source.sql;

-- ---------- 03_alter (migration patches theo thứ tự) ----------
SOURCE 03_alter/01_partner_contract_supplier_fields.sql;
SOURCE 03_alter/02_contracts_source_order_id.sql;
SOURCE 03_alter/03_reviews_manager_reply.sql;
SOURCE 03_alter/04_orders_recipient_delivery_email.sql;
SOURCE 03_alter/11_company_public_documents.sql;
SOURCE 03_alter/12_contact_inquiries.sql;
SOURCE 03_alter/13_organization_legal_documents.sql;
SOURCE 03_alter/05_dish_values.sql;
SOURCE 03_alter/06_period_based_contracts.sql;
SOURCE 03_alter/07_organization_complaints.sql;
SOURCE 03_alter/08_complaint_refund_payment.sql;
SOURCE 03_alter/09_delivery_otp.sql;
SOURCE 03_alter/10_recipient_signature.sql;
SOURCE 03_alter/14_contract_weekly_selections.sql;
SOURCE 03_alter/15_delivery_handover_document.sql;
SOURCE 03_alter/16_shipper_signature.sql;

-- ---------- 04_seed_data (bổ sung seed idempotent) ----------
SOURCE 04_seed_data/22_seed_manager_intake_permissions.sql;
SOURCE 04_seed_data/23_seed_warehousestaff_partner_payment.sql;
SOURCE 04_seed_data/11b_seed_spice_ingredients.sql;
SOURCE 04_seed_data/20_seed_dish_seasonings.sql;
SOURCE 04_seed_data/14b_clone_dish_ingredients_by_value.sql;
SOURCE 04_seed_data/19_seed_dish_images.sql;
SOURCE 04_seed_data/21_seed_bulk_upcoming_orders_for_ai.sql;

-- ---------- Tối ưu thống kê ----------
SOURCE 05_maintenance/01_analyze_optimize.sql;

SELECT 'SmartLunch incremental update (run_update.sql) completed!' AS Status;
