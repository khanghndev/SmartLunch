DELIMITER //
CREATE PROCEDURE sp_get_users_by_role(IN p_role_name VARCHAR(100))
BEGIN
    SELECT u.Id, u.Code, u.Username, u.Email, u.FirstName, u.LastName, u.IsActive, ur.AssignedAt
    FROM users u INNER JOIN user_roles ur ON u.Id = ur.UserId INNER JOIN roles r ON ur.RoleId = r.Id
    WHERE r.Name = p_role_name AND u.IsActive = 1 AND ur.IsActive = 1 AND r.IsActive = 1 ORDER BY ur.AssignedAt DESC;
END //
DELIMITER ;
