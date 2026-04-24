CREATE OR REPLACE VIEW vw_role_permissions_summary AS
SELECT r.Id AS RoleId, r.Name AS RoleName, r.IsActive AS RoleIsActive,
    COUNT(DISTINCT rp.PermissionId) AS PermissionCount,
    GROUP_CONCAT(DISTINCT p.Name ORDER BY p.Name SEPARATOR ', ') AS Permissions
FROM roles r
LEFT JOIN role_permissions rp ON r.Id = rp.RoleId AND rp.IsActive = 1
LEFT JOIN permissions p ON rp.PermissionId = p.Id AND p.IsActive = 1
GROUP BY r.Id, r.Name, r.IsActive;
