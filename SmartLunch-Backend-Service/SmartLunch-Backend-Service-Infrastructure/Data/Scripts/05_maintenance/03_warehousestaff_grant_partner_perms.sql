-- =====================================================
-- Maintenance: Cấp thêm quyền đọc Partners / Contracts / Partner Payments
-- cho role WarehouseStaff để xem báo cáo Công nợ phải trả NCC và
-- chọn nhà cung cấp khi nhập kho.
--
-- An toàn để chạy nhiều lần: chỉ thêm các permission còn thiếu.
-- =====================================================

INSERT INTO role_permissions (RoleId, PermissionId, AssignedAt, IsActive)
SELECT r.Id, p.Id, NOW(), 1
FROM roles r
JOIN permissions p
  ON p.Resource IN ('partners', 'contracts', 'partner_payments')
 AND p.Action   IN ('read', 'list')
WHERE r.Name = 'WarehouseStaff'
  AND NOT EXISTS (
        SELECT 1
        FROM role_permissions rp
        WHERE rp.RoleId = r.Id
          AND rp.PermissionId = p.Id
  );
