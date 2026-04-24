CREATE OR REPLACE VIEW vw_user_permissions AS
SELECT DISTINCT u.Id AS UserId, u.Username,
    p.Id AS PermissionId, p.Name AS PermissionName, p.Resource, p.Action,
    CASE WHEN up.Id IS NOT NULL THEN 'direct' WHEN rp.Id IS NOT NULL THEN 'role' ELSE 'none' END AS PermissionSource,
    COALESCE(ur.RoleId, 0) AS RoleId, COALESCE(r.Name, '') AS RoleName
FROM users u
LEFT JOIN user_permissions up ON u.Id = up.UserId AND up.IsActive = 1
LEFT JOIN permissions p1 ON up.PermissionId = p1.Id AND p1.IsActive = 1
LEFT JOIN user_roles ur ON u.Id = ur.UserId AND ur.IsActive = 1
LEFT JOIN roles r ON ur.RoleId = r.Id AND r.IsActive = 1
LEFT JOIN role_permissions rp ON r.Id = rp.RoleId AND rp.IsActive = 1
LEFT JOIN permissions p ON (p1.Id = p.Id OR rp.PermissionId = p.Id) AND p.IsActive = 1
WHERE u.IsActive = 1 AND p.Id IS NOT NULL;
