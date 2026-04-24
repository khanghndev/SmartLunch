DELIMITER //
CREATE PROCEDURE sp_check_user_permission(IN p_user_id INT, IN p_permission_name VARCHAR(200), OUT p_has_permission TINYINT(1))
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*) INTO v_count FROM vw_user_permissions WHERE UserId = p_user_id AND PermissionName = p_permission_name;
    SET p_has_permission = IF(v_count > 0, 1, 0);
END //
DELIMITER ;
