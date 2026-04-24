DELIMITER //
CREATE PROCEDURE sp_get_user_roles(IN p_user_id INT)
BEGIN
    SELECT r.Id, r.Code, r.Name, r.Description, r.IsActive, ur.AssignedAt, ur.AssignedBy
    FROM user_roles ur INNER JOIN roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = p_user_id AND ur.IsActive = 1 AND r.IsActive = 1 ORDER BY ur.AssignedAt DESC;
END //
DELIMITER ;
