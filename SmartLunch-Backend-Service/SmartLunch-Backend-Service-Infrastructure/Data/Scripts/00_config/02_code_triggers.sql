-- =====================================================
-- Function: fn_next_code
-- Sinh mã Code độc lập với Id (dùng sequence riêng)
-- Usage: SELECT fn_next_code('users') → 'USR00000001'
-- =====================================================
DELIMITER //

CREATE FUNCTION fn_next_code(p_table_name VARCHAR(100))
RETURNS VARCHAR(20)
DETERMINISTIC
MODIFIES SQL DATA
BEGIN
    DECLARE v_prefix VARCHAR(5);
    DECLARE v_seq INT;

    UPDATE code_prefixes
    SET LastSequence = LastSequence + 1
    WHERE TableName = p_table_name;

    SELECT Prefix, LastSequence
    INTO v_prefix, v_seq
    FROM code_prefixes
    WHERE TableName = p_table_name;

    RETURN CONCAT(v_prefix, LPAD(v_seq, 8, '0'));
END //

-- =====================================================
-- Triggers: BEFORE INSERT cho mỗi bảng
-- Tự động set Code nếu chưa có
-- =====================================================
CREATE TRIGGER trg_users_code BEFORE INSERT ON users
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('users'); END IF; END //

CREATE TRIGGER trg_roles_code BEFORE INSERT ON roles
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('roles'); END IF; END //

CREATE TRIGGER trg_permissions_code BEFORE INSERT ON permissions
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('permissions'); END IF; END //

CREATE TRIGGER trg_user_roles_code BEFORE INSERT ON user_roles
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_roles'); END IF; END //

CREATE TRIGGER trg_user_permissions_code BEFORE INSERT ON user_permissions
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_permissions'); END IF; END //

CREATE TRIGGER trg_role_permissions_code BEFORE INSERT ON role_permissions
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('role_permissions'); END IF; END //

CREATE TRIGGER trg_user_tokens_code BEFORE INSERT ON user_tokens
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_tokens'); END IF; END //

CREATE TRIGGER trg_media_files_code BEFORE INSERT ON media_files
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('media_files'); END IF; END //

CREATE TRIGGER trg_organizations_code BEFORE INSERT ON organizations
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('organizations'); END IF; END //

CREATE TRIGGER trg_user_organizations_code BEFORE INSERT ON user_organizations
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('user_organizations'); END IF; END //

CREATE TRIGGER trg_partners_code BEFORE INSERT ON partners
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('partners'); END IF; END //

CREATE TRIGGER trg_contracts_code BEFORE INSERT ON contracts
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('contracts'); END IF; END //

CREATE TRIGGER trg_partner_payments_code BEFORE INSERT ON partner_payments
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('partner_payments'); END IF; END //

CREATE TRIGGER trg_ingredients_code BEFORE INSERT ON ingredients
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredients'); END IF; END //

CREATE TRIGGER trg_ingredient_sources_code BEFORE INSERT ON ingredient_sources
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_sources'); END IF; END //

CREATE TRIGGER trg_internal_stock_issues_code BEFORE INSERT ON internal_stock_issues
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('internal_stock_issues'); END IF; END //

CREATE TRIGGER trg_internal_stock_issue_lines_code BEFORE INSERT ON internal_stock_issue_lines
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('internal_stock_issue_lines'); END IF; END //

CREATE TRIGGER trg_ingredient_intake_proposals_code BEFORE INSERT ON ingredient_intake_proposals
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_intake_proposals'); END IF; END //

CREATE TRIGGER trg_ingredient_intake_proposal_lines_code BEFORE INSERT ON ingredient_intake_proposal_lines
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_intake_proposal_lines'); END IF; END //

CREATE TRIGGER trg_ingredient_actual_intakes_code BEFORE INSERT ON ingredient_actual_intakes
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_actual_intakes'); END IF; END //

CREATE TRIGGER trg_ingredient_actual_intake_lines_code BEFORE INSERT ON ingredient_actual_intake_lines
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_actual_intake_lines'); END IF; END //

CREATE TRIGGER trg_dishes_code BEFORE INSERT ON dishes
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dishes'); END IF; END //

CREATE TRIGGER trg_dish_ingredients_code BEFORE INSERT ON dish_ingredients
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dish_ingredients'); END IF; END //

CREATE TRIGGER trg_weekly_menus_code BEFORE INSERT ON weekly_menus
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('weekly_menus'); END IF; END //

CREATE TRIGGER trg_menu_schedule_code BEFORE INSERT ON menu_schedule
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('menu_schedule'); END IF; END //

CREATE TRIGGER trg_orders_code BEFORE INSERT ON orders
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('orders'); END IF; END //

CREATE TRIGGER trg_order_items_code BEFORE INSERT ON order_items
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('order_items'); END IF; END //

CREATE TRIGGER trg_deliveries_code BEFORE INSERT ON deliveries
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('deliveries'); END IF; END //

CREATE TRIGGER trg_payments_code BEFORE INSERT ON payments
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('payments'); END IF; END //

CREATE TRIGGER trg_transactions_code BEFORE INSERT ON transactions
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('transactions'); END IF; END //

CREATE TRIGGER trg_reviews_code BEFORE INSERT ON reviews
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('reviews'); END IF; END //

CREATE TRIGGER trg_sentiments_code BEFORE INSERT ON sentiments
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('sentiments'); END IF; END //

CREATE TRIGGER trg_complaints_code BEFORE INSERT ON complaints
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('complaints'); END IF; END //

CREATE TRIGGER trg_chatbot_logs_code BEFORE INSERT ON chatbot_logs
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('chatbot_logs'); END IF; END //

CREATE TRIGGER trg_menu_suggestions_code BEFORE INSERT ON menu_suggestions
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('menu_suggestions'); END IF; END //

CREATE TRIGGER trg_news_code BEFORE INSERT ON news
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('news'); END IF; END //

CREATE TRIGGER trg_recruitment_code BEFORE INSERT ON recruitment
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('recruitment'); END IF; END //

CREATE TRIGGER trg_banners_code BEFORE INSERT ON banners
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('banners'); END IF; END //

CREATE TRIGGER trg_notifications_code BEFORE INSERT ON notifications
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('notifications'); END IF; END //

CREATE TRIGGER trg_ingredient_categories_code BEFORE INSERT ON ingredient_categories
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('ingredient_categories'); END IF; END //

CREATE TRIGGER trg_dish_categories_code BEFORE INSERT ON dish_categories
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dish_categories'); END IF; END //

CREATE TRIGGER trg_dish_dish_categories_code BEFORE INSERT ON dish_dish_categories
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('dish_dish_categories'); END IF; END //

CREATE TRIGGER trg_cooking_methods_code BEFORE INSERT ON cooking_methods
FOR EACH ROW BEGIN IF NEW.Code IS NULL THEN SET NEW.Code = fn_next_code('cooking_methods'); END IF; END //

DELIMITER ;
