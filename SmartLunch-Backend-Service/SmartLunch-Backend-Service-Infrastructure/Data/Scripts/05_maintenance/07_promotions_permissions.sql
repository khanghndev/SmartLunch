-- Quyền quản lý khuyến mãi + gán Manager/Admin (chạy an toàn nhiều lần).

INSERT IGNORE INTO permissions (Name, Description, Resource, Action) VALUES
('promotions.create', 'Tạo khuyến mãi', 'promotions', 'create'),
('promotions.read', 'Xem khuyến mãi', 'promotions', 'read'),
('promotions.update', 'Cập nhật khuyến mãi', 'promotions', 'update'),
('promotions.delete', 'Vô hiệu khuyến mãi', 'promotions', 'delete'),
('promotions.list', 'Danh sách khuyến mãi', 'promotions', 'list');

INSERT IGNORE INTO role_permissions (RoleId, PermissionId, GrantedAt, IsActive)
SELECT r.Id, p.Id, NOW(), 1
FROM roles r
CROSS JOIN permissions p
WHERE r.Name IN ('Admin', 'Super Admin', 'Manager', 'Quản lý công ty')
  AND p.Resource = 'promotions';
