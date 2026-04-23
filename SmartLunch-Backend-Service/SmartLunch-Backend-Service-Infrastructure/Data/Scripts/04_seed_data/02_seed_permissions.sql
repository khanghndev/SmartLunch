-- =====================================================
-- Seed: Permissions (36 bảng × 5 actions = 180 permissions)
-- Actions: create, read, update, delete, list
-- =====================================================

-- 01. users
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('users.create', 'Tạo người dùng', 'users', 'create'),
('users.read', 'Xem thông tin người dùng', 'users', 'read'),
('users.update', 'Cập nhật người dùng', 'users', 'update'),
('users.delete', 'Xóa người dùng', 'users', 'delete'),
('users.list', 'Danh sách người dùng', 'users', 'list');

-- 02. roles
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('roles.create', 'Tạo vai trò', 'roles', 'create'),
('roles.read', 'Xem vai trò', 'roles', 'read'),
('roles.update', 'Cập nhật vai trò', 'roles', 'update'),
('roles.delete', 'Xóa vai trò', 'roles', 'delete'),
('roles.list', 'Danh sách vai trò', 'roles', 'list');

-- 03. permissions
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('permissions.create', 'Tạo quyền', 'permissions', 'create'),
('permissions.read', 'Xem quyền', 'permissions', 'read'),
('permissions.update', 'Cập nhật quyền', 'permissions', 'update'),
('permissions.delete', 'Xóa quyền', 'permissions', 'delete'),
('permissions.list', 'Danh sách quyền', 'permissions', 'list');

-- 04. user_roles
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('user_roles.create', 'Gán vai trò cho user', 'user_roles', 'create'),
('user_roles.read', 'Xem gán vai trò', 'user_roles', 'read'),
('user_roles.update', 'Cập nhật gán vai trò', 'user_roles', 'update'),
('user_roles.delete', 'Xóa gán vai trò', 'user_roles', 'delete'),
('user_roles.list', 'Danh sách gán vai trò', 'user_roles', 'list');

-- 05. user_permissions
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('user_permissions.create', 'Gán quyền trực tiếp', 'user_permissions', 'create'),
('user_permissions.read', 'Xem quyền trực tiếp', 'user_permissions', 'read'),
('user_permissions.update', 'Cập nhật quyền trực tiếp', 'user_permissions', 'update'),
('user_permissions.delete', 'Xóa quyền trực tiếp', 'user_permissions', 'delete'),
('user_permissions.list', 'Danh sách quyền trực tiếp', 'user_permissions', 'list');

-- 06. role_permissions
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('role_permissions.create', 'Gán quyền cho vai trò', 'role_permissions', 'create'),
('role_permissions.read', 'Xem quyền vai trò', 'role_permissions', 'read'),
('role_permissions.update', 'Cập nhật quyền vai trò', 'role_permissions', 'update'),
('role_permissions.delete', 'Xóa quyền vai trò', 'role_permissions', 'delete'),
('role_permissions.list', 'Danh sách quyền vai trò', 'role_permissions', 'list');

-- 07. user_tokens
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('user_tokens.create', 'Tạo token', 'user_tokens', 'create'),
('user_tokens.read', 'Xem token', 'user_tokens', 'read'),
('user_tokens.update', 'Cập nhật token', 'user_tokens', 'update'),
('user_tokens.delete', 'Xóa/thu hồi token', 'user_tokens', 'delete'),
('user_tokens.list', 'Danh sách token', 'user_tokens', 'list');

-- 08. media_files
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('media_files.create', 'Upload tệp', 'media_files', 'create'),
('media_files.read', 'Xem tệp', 'media_files', 'read'),
('media_files.update', 'Cập nhật tệp', 'media_files', 'update'),
('media_files.delete', 'Xóa tệp', 'media_files', 'delete'),
('media_files.list', 'Danh sách tệp', 'media_files', 'list');

-- 09. units
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('units.create', 'Tạo đơn vị', 'units', 'create'),
('units.read', 'Xem đơn vị', 'units', 'read'),
('units.update', 'Cập nhật đơn vị', 'units', 'update'),
('units.delete', 'Xóa đơn vị', 'units', 'delete'),
('units.list', 'Danh sách đơn vị', 'units', 'list');

-- 10. user_units
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('user_units.create', 'Gán thành viên đơn vị', 'user_units', 'create'),
('user_units.read', 'Xem thành viên đơn vị', 'user_units', 'read'),
('user_units.update', 'Cập nhật thành viên', 'user_units', 'update'),
('user_units.delete', 'Xóa thành viên đơn vị', 'user_units', 'delete'),
('user_units.list', 'Danh sách thành viên', 'user_units', 'list');

-- 11. partners
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('partners.create', 'Tạo nhà cung cấp', 'partners', 'create'),
('partners.read', 'Xem nhà cung cấp', 'partners', 'read'),
('partners.update', 'Cập nhật nhà cung cấp', 'partners', 'update'),
('partners.delete', 'Xóa nhà cung cấp', 'partners', 'delete'),
('partners.list', 'Danh sách nhà cung cấp', 'partners', 'list');

-- 12. contracts
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('contracts.create', 'Tạo hợp đồng', 'contracts', 'create'),
('contracts.read', 'Xem hợp đồng', 'contracts', 'read'),
('contracts.update', 'Cập nhật hợp đồng', 'contracts', 'update'),
('contracts.delete', 'Xóa hợp đồng', 'contracts', 'delete'),
('contracts.list', 'Danh sách hợp đồng', 'contracts', 'list');

-- 13. partner_payments
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('partner_payments.create', 'Tạo thanh toán NCC', 'partner_payments', 'create'),
('partner_payments.read', 'Xem thanh toán NCC', 'partner_payments', 'read'),
('partner_payments.update', 'Cập nhật thanh toán NCC', 'partner_payments', 'update'),
('partner_payments.delete', 'Xóa thanh toán NCC', 'partner_payments', 'delete'),
('partner_payments.list', 'Danh sách thanh toán NCC', 'partner_payments', 'list');

-- 14. ingredients
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('ingredients.create', 'Tạo nguyên liệu', 'ingredients', 'create'),
('ingredients.read', 'Xem nguyên liệu', 'ingredients', 'read'),
('ingredients.update', 'Cập nhật nguyên liệu', 'ingredients', 'update'),
('ingredients.delete', 'Xóa nguyên liệu', 'ingredients', 'delete'),
('ingredients.list', 'Danh sách nguyên liệu', 'ingredients', 'list');

-- 15. ingredient_sources
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('ingredient_sources.create', 'Tạo nguồn gốc lô hàng', 'ingredient_sources', 'create'),
('ingredient_sources.read', 'Xem nguồn gốc lô hàng', 'ingredient_sources', 'read'),
('ingredient_sources.update', 'Cập nhật nguồn gốc', 'ingredient_sources', 'update'),
('ingredient_sources.delete', 'Xóa nguồn gốc', 'ingredient_sources', 'delete'),
('ingredient_sources.list', 'Danh sách nguồn gốc', 'ingredient_sources', 'list');

-- 16. inventory
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('inventory.create', 'Tạo tồn kho', 'inventory', 'create'),
('inventory.read', 'Xem tồn kho', 'inventory', 'read'),
('inventory.update', 'Cập nhật tồn kho', 'inventory', 'update'),
('inventory.delete', 'Xóa tồn kho', 'inventory', 'delete'),
('inventory.list', 'Danh sách tồn kho', 'inventory', 'list');

-- 17. internal_stock_issues
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('internal_stock_issues.create', 'Tạo phiếu xuất kho', 'internal_stock_issues', 'create'),
('internal_stock_issues.read', 'Xem phiếu xuất kho', 'internal_stock_issues', 'read'),
('internal_stock_issues.update', 'Cập nhật phiếu xuất', 'internal_stock_issues', 'update'),
('internal_stock_issues.delete', 'Xóa phiếu xuất kho', 'internal_stock_issues', 'delete'),
('internal_stock_issues.list', 'Danh sách phiếu xuất', 'internal_stock_issues', 'list');

-- 18. internal_stock_issue_lines
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('internal_stock_issue_lines.create', 'Tạo chi tiết phiếu xuất', 'internal_stock_issue_lines', 'create'),
('internal_stock_issue_lines.read', 'Xem chi tiết phiếu xuất', 'internal_stock_issue_lines', 'read'),
('internal_stock_issue_lines.update', 'Cập nhật chi tiết xuất', 'internal_stock_issue_lines', 'update'),
('internal_stock_issue_lines.delete', 'Xóa chi tiết phiếu xuất', 'internal_stock_issue_lines', 'delete'),
('internal_stock_issue_lines.list', 'Danh sách chi tiết xuất', 'internal_stock_issue_lines', 'list');

-- 19. ingredient_intake_proposals
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('ingredient_intake_proposals.create', 'Tạo phiếu đề xuất nhập', 'ingredient_intake_proposals', 'create'),
('ingredient_intake_proposals.read', 'Xem phiếu đề xuất', 'ingredient_intake_proposals', 'read'),
('ingredient_intake_proposals.update', 'Cập nhật đề xuất nhập', 'ingredient_intake_proposals', 'update'),
('ingredient_intake_proposals.delete', 'Xóa phiếu đề xuất', 'ingredient_intake_proposals', 'delete'),
('ingredient_intake_proposals.list', 'Danh sách đề xuất nhập', 'ingredient_intake_proposals', 'list');

-- 20. ingredient_intake_proposal_lines
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('ingredient_intake_proposal_lines.create', 'Tạo chi tiết đề xuất', 'ingredient_intake_proposal_lines', 'create'),
('ingredient_intake_proposal_lines.read', 'Xem chi tiết đề xuất', 'ingredient_intake_proposal_lines', 'read'),
('ingredient_intake_proposal_lines.update', 'Cập nhật chi tiết đề xuất', 'ingredient_intake_proposal_lines', 'update'),
('ingredient_intake_proposal_lines.delete', 'Xóa chi tiết đề xuất', 'ingredient_intake_proposal_lines', 'delete'),
('ingredient_intake_proposal_lines.list', 'Danh sách chi tiết đề xuất', 'ingredient_intake_proposal_lines', 'list');

-- 21. ingredient_actual_intakes
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('ingredient_actual_intakes.create', 'Tạo phiếu nhập kho', 'ingredient_actual_intakes', 'create'),
('ingredient_actual_intakes.read', 'Xem phiếu nhập kho', 'ingredient_actual_intakes', 'read'),
('ingredient_actual_intakes.update', 'Cập nhật phiếu nhập', 'ingredient_actual_intakes', 'update'),
('ingredient_actual_intakes.delete', 'Xóa phiếu nhập kho', 'ingredient_actual_intakes', 'delete'),
('ingredient_actual_intakes.list', 'Danh sách phiếu nhập', 'ingredient_actual_intakes', 'list');

-- 22. ingredient_actual_intake_lines
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('ingredient_actual_intake_lines.create', 'Tạo chi tiết phiếu nhập', 'ingredient_actual_intake_lines', 'create'),
('ingredient_actual_intake_lines.read', 'Xem chi tiết phiếu nhập', 'ingredient_actual_intake_lines', 'read'),
('ingredient_actual_intake_lines.update', 'Cập nhật chi tiết nhập', 'ingredient_actual_intake_lines', 'update'),
('ingredient_actual_intake_lines.delete', 'Xóa chi tiết phiếu nhập', 'ingredient_actual_intake_lines', 'delete'),
('ingredient_actual_intake_lines.list', 'Danh sách chi tiết nhập', 'ingredient_actual_intake_lines', 'list');

-- 23. dishes
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('dishes.create', 'Tạo món ăn', 'dishes', 'create'),
('dishes.read', 'Xem món ăn', 'dishes', 'read'),
('dishes.update', 'Cập nhật món ăn', 'dishes', 'update'),
('dishes.delete', 'Xóa món ăn', 'dishes', 'delete'),
('dishes.list', 'Danh sách món ăn', 'dishes', 'list');

-- 24. dish_ingredients
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('dish_ingredients.create', 'Thêm nguyên liệu món', 'dish_ingredients', 'create'),
('dish_ingredients.read', 'Xem nguyên liệu món', 'dish_ingredients', 'read'),
('dish_ingredients.update', 'Cập nhật nguyên liệu món', 'dish_ingredients', 'update'),
('dish_ingredients.delete', 'Xóa nguyên liệu món', 'dish_ingredients', 'delete'),
('dish_ingredients.list', 'Danh sách nguyên liệu món', 'dish_ingredients', 'list');

-- 25. weekly_menus
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('weekly_menus.create', 'Tạo thực đơn tuần', 'weekly_menus', 'create'),
('weekly_menus.read', 'Xem thực đơn tuần', 'weekly_menus', 'read'),
('weekly_menus.update', 'Cập nhật thực đơn', 'weekly_menus', 'update'),
('weekly_menus.delete', 'Xóa thực đơn tuần', 'weekly_menus', 'delete'),
('weekly_menus.list', 'Danh sách thực đơn', 'weekly_menus', 'list');

-- 26. menu_schedule
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('menu_schedule.create', 'Tạo lịch thực đơn', 'menu_schedule', 'create'),
('menu_schedule.read', 'Xem lịch thực đơn', 'menu_schedule', 'read'),
('menu_schedule.update', 'Cập nhật lịch thực đơn', 'menu_schedule', 'update'),
('menu_schedule.delete', 'Xóa lịch thực đơn', 'menu_schedule', 'delete'),
('menu_schedule.list', 'Danh sách lịch thực đơn', 'menu_schedule', 'list');

-- 27. orders
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('orders.create', 'Tạo đơn hàng', 'orders', 'create'),
('orders.read', 'Xem đơn hàng', 'orders', 'read'),
('orders.update', 'Cập nhật đơn hàng', 'orders', 'update'),
('orders.delete', 'Xóa đơn hàng', 'orders', 'delete'),
('orders.list', 'Danh sách đơn hàng', 'orders', 'list');

-- 28. order_items
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('order_items.create', 'Thêm chi tiết đơn', 'order_items', 'create'),
('order_items.read', 'Xem chi tiết đơn', 'order_items', 'read'),
('order_items.update', 'Cập nhật chi tiết đơn', 'order_items', 'update'),
('order_items.delete', 'Xóa chi tiết đơn', 'order_items', 'delete'),
('order_items.list', 'Danh sách chi tiết đơn', 'order_items', 'list');

-- 29. deliveries
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('deliveries.create', 'Tạo giao hàng', 'deliveries', 'create'),
('deliveries.read', 'Xem giao hàng', 'deliveries', 'read'),
('deliveries.update', 'Cập nhật giao hàng', 'deliveries', 'update'),
('deliveries.delete', 'Xóa giao hàng', 'deliveries', 'delete'),
('deliveries.list', 'Danh sách giao hàng', 'deliveries', 'list');

-- 30. payments
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('payments.create', 'Tạo thanh toán', 'payments', 'create'),
('payments.read', 'Xem thanh toán', 'payments', 'read'),
('payments.update', 'Cập nhật thanh toán', 'payments', 'update'),
('payments.delete', 'Xóa thanh toán', 'payments', 'delete'),
('payments.list', 'Danh sách thanh toán', 'payments', 'list');

-- 31. transactions
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('transactions.create', 'Tạo giao dịch thu chi', 'transactions', 'create'),
('transactions.read', 'Xem giao dịch', 'transactions', 'read'),
('transactions.update', 'Cập nhật giao dịch', 'transactions', 'update'),
('transactions.delete', 'Xóa giao dịch', 'transactions', 'delete'),
('transactions.list', 'Danh sách giao dịch', 'transactions', 'list');

-- 32. reviews
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('reviews.create', 'Tạo đánh giá', 'reviews', 'create'),
('reviews.read', 'Xem đánh giá', 'reviews', 'read'),
('reviews.update', 'Cập nhật đánh giá', 'reviews', 'update'),
('reviews.delete', 'Xóa đánh giá', 'reviews', 'delete'),
('reviews.list', 'Danh sách đánh giá', 'reviews', 'list');

-- 33. sentiments
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('sentiments.create', 'Tạo phân tích cảm xúc', 'sentiments', 'create'),
('sentiments.read', 'Xem phân tích cảm xúc', 'sentiments', 'read'),
('sentiments.update', 'Cập nhật phân tích', 'sentiments', 'update'),
('sentiments.delete', 'Xóa phân tích', 'sentiments', 'delete'),
('sentiments.list', 'Danh sách phân tích', 'sentiments', 'list');

-- 34. complaints
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('complaints.create', 'Tạo khiếu nại', 'complaints', 'create'),
('complaints.read', 'Xem khiếu nại', 'complaints', 'read'),
('complaints.update', 'Cập nhật khiếu nại', 'complaints', 'update'),
('complaints.delete', 'Xóa khiếu nại', 'complaints', 'delete'),
('complaints.list', 'Danh sách khiếu nại', 'complaints', 'list');

-- 35. chatbot_logs
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('chatbot_logs.create', 'Tạo log chatbot', 'chatbot_logs', 'create'),
('chatbot_logs.read', 'Xem log chatbot', 'chatbot_logs', 'read'),
('chatbot_logs.update', 'Cập nhật log chatbot', 'chatbot_logs', 'update'),
('chatbot_logs.delete', 'Xóa log chatbot', 'chatbot_logs', 'delete'),
('chatbot_logs.list', 'Danh sách log chatbot', 'chatbot_logs', 'list');

-- 36. menu_suggestions
INSERT INTO permissions (Name, Description, Resource, Action) VALUES
('menu_suggestions.create', 'Tạo gợi ý thực đơn', 'menu_suggestions', 'create'),
('menu_suggestions.read', 'Xem gợi ý thực đơn', 'menu_suggestions', 'read'),
('menu_suggestions.update', 'Cập nhật gợi ý', 'menu_suggestions', 'update'),
('menu_suggestions.delete', 'Xóa gợi ý thực đơn', 'menu_suggestions', 'delete'),
('menu_suggestions.list', 'Danh sách gợi ý', 'menu_suggestions', 'list');
