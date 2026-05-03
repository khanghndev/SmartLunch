-- =====================================================
-- Maintenance: ANALYZE & OPTIMIZE tables
-- Run periodically for performance monitoring
-- =====================================================

ANALYZE TABLE
    users, roles, permissions, user_roles, user_permissions, role_permissions, user_tokens, media_files,
    units, user_units,
    partners, contracts, partner_payments,
    ingredients, ingredient_sources, inventory,
    internal_stock_issues, internal_stock_issue_lines,
    ingredient_intake_proposals, ingredient_intake_proposal_lines,
    ingredient_actual_intakes, ingredient_actual_intake_lines,
    dishes, dish_ingredients,
    weekly_menus, menu_schedule,
    orders, order_items, deliveries, payments,
    transactions, reviews, sentiments, complaints, chatbot_logs, menu_suggestions,
    news, recruitment, banners, notifications,
    system_logs, system_backups;

OPTIMIZE TABLE
    users, roles, permissions, user_roles, user_permissions, role_permissions, user_tokens, media_files,
    units, user_units,
    partners, contracts, partner_payments,
    ingredients, ingredient_sources, inventory,
    internal_stock_issues, internal_stock_issue_lines,
    ingredient_intake_proposals, ingredient_intake_proposal_lines,
    ingredient_actual_intakes, ingredient_actual_intake_lines,
    dishes, dish_ingredients,
    weekly_menus, menu_schedule,
    orders, order_items, deliveries, payments,
    transactions, reviews, sentiments, complaints, chatbot_logs, menu_suggestions,
    news, recruitment, banners, notifications,
    system_logs, system_backups;