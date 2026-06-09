-- WarehouseStaff: quyền ghi nhận thanh toán NCC (chi công nợ phải trả)
INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT r.Id, p.Id, NOW(), 1
FROM roles r
JOIN permissions p ON p.Code = 'partner_payments.create'
WHERE r.Name = 'WarehouseStaff'
  AND NOT EXISTS (
        SELECT 1 FROM role_permissions rp
        WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id
  );
