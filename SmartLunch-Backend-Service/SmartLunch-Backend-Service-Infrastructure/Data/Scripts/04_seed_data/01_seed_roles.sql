-- =====================================================
-- Seed: Roles (7 vai trò hệ thống)
-- =====================================================
INSERT INTO roles (Name, Description, IsSystemRole, CreatedAt) VALUES
('Admin', 'Quản trị viên, quản lý nghiệp vụ chính', 1, NOW()),
('Manager', 'Quản lý công ty suất ăn, xem báo cáo tổng hợp', 1, NOW()),
('WarehouseStaff', 'Nhân viên bếp/kho/bán hàng', 1, NOW()),
('ChefStaff', 'Nhân viên bếp/kho/bán hàng', 1, NOW()),
('SalesStaff', 'Nhân viên bếp/kho/bán hàng', 1, NOW()),
('Organization', 'Đại diện đơn vị đặt suất ăn (B2B)', 1, NOW()),
('Shipper', 'Nhân viên giao hàng', 1, NOW()),
('Customer', 'Người dùng cuối, đặt cơm cá nhân', 1, NOW());