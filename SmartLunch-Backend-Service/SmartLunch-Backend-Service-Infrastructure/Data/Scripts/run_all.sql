-- =====================================
-- SmartLunch Database - Master Runner
-- MySQL 8.0+ | UTF-8 (utf8mb4)
--
-- Usage: mysql -u root -p < run_all.sql
-- =====================================

-- Step 0: Init Database + UTF-8
SOURCE 00_init_database.sql;

-- Step 0.5: Config table (prefix + sequence)
SOURCE 00_config/01_code_prefix_config.sql;

-- Step 1: Create Tables (dependency order)
SOURCE 01_tables/01_users.sql;
SOURCE 01_tables/02_roles.sql;
SOURCE 01_tables/03_permissions.sql;
SOURCE 01_tables/04_user_roles.sql;
SOURCE 01_tables/05_user_permissions.sql;
SOURCE 01_tables/06_role_permissions.sql;
SOURCE 01_tables/07_user_tokens.sql;
SOURCE 01_tables/08_media_files.sql;
SOURCE 01_tables/09_organizations.sql;
SOURCE 01_tables/10_user_organizations.sql;
SOURCE 01_tables/11_partners.sql;
SOURCE 01_tables/12_contracts.sql;
SOURCE 01_tables/13_partner_payments.sql;
SOURCE 01_tables/14_ingredient_categories.sql;
SOURCE 01_tables/14a_ingredients.sql;
SOURCE 01_tables/15_ingredient_sources.sql;
SOURCE 01_tables/16_inventory.sql;
SOURCE 01_tables/17_internal_stock_issues.sql;
SOURCE 01_tables/18_internal_stock_issue_lines.sql;
SOURCE 01_tables/19_ingredient_intake_proposals.sql;
SOURCE 01_tables/20_ingredient_intake_proposal_lines.sql;
SOURCE 01_tables/21_ingredient_actual_intakes.sql;
SOURCE 01_tables/22_ingredient_actual_intake_lines.sql;
SOURCE 01_tables/22b_cooking_methods.sql;
SOURCE 01_tables/23_dishes.sql;
SOURCE 01_tables/23b_dish_categories.sql;
SOURCE 01_tables/23c_dish_dish_categories.sql;
SOURCE 01_tables/24_dish_ingredients.sql;
SOURCE 01_tables/46_customer_type.sql;
SOURCE 01_tables/25_weekly_menus.sql;
SOURCE 01_tables/26_menu_schedule.sql;
SOURCE 01_tables/27_orders.sql;
SOURCE 01_tables/28_order_items.sql;
SOURCE 01_tables/29_deliveries.sql;
SOURCE 01_tables/30_payments.sql;
SOURCE 01_tables/31_transactions.sql;
SOURCE 01_tables/32_reviews.sql;
SOURCE 01_tables/33_sentiments.sql;
SOURCE 01_tables/34_complaints.sql;
SOURCE 01_tables/35_chatbot_logs.sql;
SOURCE 01_tables/36_menu_suggestions.sql;
SOURCE 01_tables/36a_menu_suggestion_plans.sql;
SOURCE 01_tables/36b_menu_suggestion_plan_days.sql;
SOURCE 01_tables/36c_menu_suggestion_plan_items.sql;
SOURCE 01_tables/37_news.sql;
SOURCE 01_tables/38_recruitment.sql;
SOURCE 01_tables/39_banners.sql;
SOURCE 01_tables/40_notifications.sql;
SOURCE 01_tables/41_system_logs.sql;
SOURCE 01_tables/42_system_backups.sql;
SOURCE 01_tables/43_dish_images.sql;
SOURCE 01_tables/44_weekly_menu_images.sql;
SOURCE 01_tables/45_partner_documents.sql;

-- Step 1.5: Code generation (function + triggers) — SAU khi tạo bảng
SOURCE 00_config/02_code_triggers.sql;

-- Step 2: Views
SOURCE 02_views/01_vw_active_users_with_roles.sql;
SOURCE 02_views/02_vw_user_permissions.sql;
SOURCE 02_views/03_vw_role_permissions_summary.sql;

-- Step 3: Stored Procedures
SOURCE 03_stored_procedures/01_sp_check_user_permission.sql;
SOURCE 03_stored_procedures/02_sp_get_user_permissions.sql;
SOURCE 03_stored_procedures/03_sp_get_user_roles.sql;
SOURCE 03_stored_procedures/04_sp_get_users_by_role.sql;

-- Step 4: Seed Data
SOURCE 04_seed_data/01_seed_roles.sql;
SOURCE 04_seed_data/02_seed_permissions.sql;
SOURCE 04_seed_data/03_seed_role_permissions.sql;
SOURCE 04_seed_data/04_seed_users.sql;
SOURCE 04_seed_data/05_seed_user_roles.sql;
SOURCE 04_seed_data/06_seed_user_permissions.sql;
SOURCE 04_seed_data/07_seed_user_tokens.sql;
SOURCE 04_seed_data/08_seed_media_files.sql;
SOURCE 04_seed_data/18_seed_customer_types.sql;
SOURCE 04_seed_data/09_seed_organizations.sql;
SOURCE 04_seed_data/10_seed_partners_contracts.sql;
SOURCE 04_seed_data/11_seed_ingredients.sql;
SOURCE 04_seed_data/12_seed_stock_issues.sql;
SOURCE 04_seed_data/13_seed_intake_proposals.sql;
SOURCE 04_seed_data/14_seed_dishes.sql;
SOURCE 04_seed_data/15_seed_menus.sql;
SOURCE 04_seed_data/16_seed_orders.sql;
SOURCE 04_seed_data/17_seed_remaining_tables.sql;

-- Step 5: Maintenance
SOURCE 05_maintenance/02_orders_annex_columns.sql;
SOURCE 05_maintenance/03_warehousestaff_grant_partner_perms.sql;
SOURCE 05_maintenance/01_analyze_optimize.sql;
SOURCE 05_maintenance/05_order_items_service_date.sql;

SELECT 'SmartLunch database setup completed!' AS Status;
