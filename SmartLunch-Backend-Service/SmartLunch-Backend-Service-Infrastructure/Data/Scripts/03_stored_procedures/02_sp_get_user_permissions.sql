DELIMITER //
CREATE PROCEDURE sp_get_user_permissions(IN p_user_id INT)
BEGIN
    SELECT PermissionId, PermissionName, Resource, Action, PermissionSource, RoleId, RoleName
    FROM vw_user_permissions WHERE UserId = p_user_id ORDER BY Resource, Action;
END //
DELIMITER ;
