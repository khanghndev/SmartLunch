-- =====================================================
-- Seed: Role-Permission Assignments
-- Admin: tất cả
-- Quản lý công ty: xem báo cáo, quản lý NCC, hợp đồng, tài chính
-- Nhân viên kho/bếp (WarehouseStaff, ChefStaff, SalesStaff): kho, bếp, đề xuất nhập, xuất kho, món ăn, thực đơn
-- KH doanh nghiệp (Company): đặt hàng, xem thực đơn, đánh giá, khiếu nại
-- NV vận chuyển (Shipper): giao hàng
-- KH cá nhân (Customer): đặt hàng cá nhân, đánh giá, chatbot
-- =====================================================

-- Admin: tất cả trừ hệ thống (roles, permissions, role_permissions, user_permissions)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'Admin'), p.Id, NOW(), 1
FROM permissions p;

-- Quản lý công ty
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'Manager'), p.Id, NOW(), 1
FROM permissions p
WHERE p.Resource IN (
    'users', 'organizations', 'user_organizations',
    'partners', 'contracts', 'partner_payments',
    'ingredients', 'inventory', 'orders', 'order_items', 'deliveries', 
    'payments', 'transactions', 'reviews', 'sentiments', 'complaints',
    'dishes', 'weekly_menus', 'menu_schedule', 'menu_suggestions'
);

-- WarehouseStaff (Nhân viên kho)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'WarehouseStaff'), p.Id, NOW(), 1
FROM permissions p
WHERE p.Resource IN (
    'ingredients', 'ingredient_sources', 'inventory',
    'internal_stock_issues', 'internal_stock_issue_lines',
    'ingredient_intake_proposals', 'ingredient_intake_proposal_lines',
    'ingredient_actual_intakes', 'ingredient_actual_intake_lines',
    'dishes', 'dish_ingredients', 'weekly_menus', 'menu_schedule', 'orders', 'order_items', 'media_files'
);

-- ChefStaff (Nhân viên bếp)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'ChefStaff'), p.Id, NOW(), 1
FROM permissions p
WHERE p.Resource IN (
    'ingredients', 'ingredient_sources', 'inventory',
    'internal_stock_issues', 'internal_stock_issue_lines',
    'ingredient_intake_proposals', 'ingredient_intake_proposal_lines',
    'ingredient_actual_intakes', 'ingredient_actual_intake_lines',
    'dishes', 'dish_ingredients', 'weekly_menus', 'menu_schedule', 'orders', 'order_items', 'media_files'
);

-- SalesStaff (Nhân viên bán hàng)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'SalesStaff'), p.Id, NOW(), 1
FROM permissions p
WHERE p.Resource IN (
    'ingredients', 'ingredient_sources', 'inventory',
    'internal_stock_issues', 'internal_stock_issue_lines',
    'ingredient_intake_proposals', 'ingredient_intake_proposal_lines',
    'ingredient_actual_intakes', 'ingredient_actual_intake_lines',
    'dishes', 'dish_ingredients', 'weekly_menus', 'menu_schedule', 'orders', 'order_items', 'media_files'
);

-- Khách hàng doanh nghiệp (B2B)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'Organization'), p.Id, NOW(), 1
FROM permissions p
WHERE (p.Resource IN ('orders', 'order_items') AND p.Action IN ('create', 'read', 'list'))
   OR (p.Resource IN ('dishes', 'weekly_menus', 'menu_schedule') AND p.Action IN ('read', 'list'))
   OR (p.Resource IN ('reviews', 'complaints') AND p.Action IN ('create', 'read', 'list'))
   OR (p.Resource = 'deliveries' AND p.Action IN ('read', 'list'))
   OR (p.Resource = 'chatbot_logs' AND p.Action IN ('create', 'read'));

-- Nhân viên vận chuyển (Shipper)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'Shipper'), p.Id, NOW(), 1
FROM permissions p
WHERE (p.Resource = 'deliveries')
   OR (p.Resource = 'orders' AND p.Action IN ('read', 'list', 'update'));

-- Khách hàng cá nhân (Customer)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT (SELECT Id FROM roles WHERE Name = 'Customer'), p.Id, NOW(), 1
FROM permissions p
WHERE (p.Resource IN ('orders', 'order_items') AND p.Action IN ('create', 'read', 'list'))
   OR (p.Resource IN ('dishes', 'weekly_menus', 'menu_schedule') AND p.Action IN ('read', 'list'))
   OR (p.Resource IN ('reviews') AND p.Action IN ('create', 'read', 'list'))
   OR (p.Resource = 'chatbot_logs' AND p.Action IN ('create', 'read'))
   OR (p.Resource = 'deliveries' AND p.Action IN ('read'));
