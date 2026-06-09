-- Quản lý công ty: duyệt phiếu đề xuất nhập & ghi nhận nhập kho thực tế
INSERT IGNORE INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT r.Id, p.Id, NOW(), 1
FROM roles r
CROSS JOIN permissions p
WHERE r.Name = 'Manager'
  AND p.Resource IN ('ingredient_intake_proposals', 'ingredient_actual_intakes')
  AND p.Action IN ('read', 'list', 'create', 'update');
