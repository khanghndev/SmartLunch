-- =====================================================
-- Seed: Roles (7 vai trò hệ thống)
-- =====================================================
INSERT INTO roles (Name, Description, IsSystemRole, CreatedAt) VALUES
('Super Admin', 'Quản trị viên cấp cao, toàn quyền hệ thống', 1, NOW()),
('Admin', 'Quản trị viên, quản lý nghiệp vụ chính', 1, NOW()),
('Quản lý công ty', 'Quản lý công ty suất ăn, xem báo cáo tổng hợp', 1, NOW()),
('Nhân viên', 'Nhân viên bếp/kho/bán hàng', 1, NOW()),
('Khách hàng doanh nghiệp', 'Đại diện công ty/trường học đặt suất ăn (B2B)', 1, NOW()),
('Nhân viên vận chuyển', 'Nhân viên giao hàng', 1, NOW()),
('Khách hàng cá nhân', 'Người dùng cuối, đặt cơm cá nhân', 1, NOW());
