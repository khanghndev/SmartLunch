CREATE OR REPLACE VIEW vw_active_users_with_roles AS
SELECT u.Id, u.Code, u.Username, u.Email, u.FirstName, u.LastName, u.IsActive, u.IsEmailVerified, u.LastLoginAt, u.CreatedAt,
    GROUP_CONCAT(DISTINCT r.Name ORDER BY r.Name SEPARATOR ', ') AS Roles,
    GROUP_CONCAT(DISTINCT r.Id ORDER BY r.Id SEPARATOR ', ') AS RoleIds
FROM users u
LEFT JOIN user_roles ur ON u.Id = ur.UserId AND ur.IsActive = 1
LEFT JOIN roles r ON ur.RoleId = r.Id AND r.IsActive = 1
WHERE u.IsActive = 1
GROUP BY u.Id, u.Code, u.Username, u.Email, u.FirstName, u.LastName, u.IsActive, u.IsEmailVerified, u.LastLoginAt, u.CreatedAt;
