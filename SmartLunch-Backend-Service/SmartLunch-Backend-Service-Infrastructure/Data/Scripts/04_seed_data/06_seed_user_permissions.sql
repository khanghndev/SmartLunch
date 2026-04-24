-- =====================================================
-- Seed: User Permissions (direct)
-- =====================================================
-- Quản lý công ty: thêm quyền quản lý user_roles
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, p.Id, NOW(), 1, 1
FROM users u, permissions p
WHERE u.Username = 'quanly' AND p.Name = 'user_roles.create';
