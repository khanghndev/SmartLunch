-- Quyền Backup/Restore hệ thống + gán Admin / Super Admin (chạy an toàn nhiều lần).

INSERT IGNORE INTO permissions (Name, Description, Resource, Action) VALUES
('systems.log', 'Xem nhật ký hệ thống', 'systems', 'log'),
('systems.backup', 'Sao lưu & quản lý backup', 'systems', 'backup'),
('systems.restore', 'Khôi phục từ backup', 'systems', 'restore');

INSERT IGNORE INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT r.Id, p.Id, NOW(), 1
FROM roles r
CROSS JOIN permissions p
WHERE r.Name IN ('Admin', 'Super Admin')
  AND p.Name IN ('systems.log', 'systems.backup', 'systems.restore');
