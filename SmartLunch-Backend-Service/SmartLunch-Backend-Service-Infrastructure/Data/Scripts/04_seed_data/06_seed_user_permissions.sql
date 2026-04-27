-- =====================================================
-- Seed: User Permissions (direct assignments từ role -> user)
-- Cấp lại toàn bộ quyền từ role_permission vào user_permission ứng với user (đảm bảo mapping role-user-permission)
-- =====================================================

-- Admin user: Map all Admin role permissions to user 'admin'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Admin'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'admin';

-- Manager user: Map all Manager role permissions to user 'manager'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Manager'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'manager';

-- Warehouse Staff: Map all Staff role permissions to user 'warehouse_staff'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Staff'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'warehouse_staff';

-- Chef Staff: Map all Staff role permissions to user 'chef_staff'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Staff'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'chef_staff';

-- Sales Staff: Map all Staff role permissions to user 'sales_staff'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Staff'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'sales_staff';

-- Company customer: Map all Company role permissions to user 'abc_company'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Company'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'abc_company';

-- Company customer: Map all Company role permissions to user 'xyz_school'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Company'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'xyz_school';

-- Shipper 1: Map all Shipper role permissions to user 'shipper1'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Shipper'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'shipper1';

-- Shipper 2: Map all Shipper role permissions to user 'shipper2'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Shipper'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'shipper2';

-- Customer 1: Map all Customer role permissions to user 'customer1'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Customer'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'customer1';

-- Customer 2: Map all Customer role permissions to user 'customer2'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Customer'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'customer2';

-- Customer 3: Map all Customer role permissions to user 'customer3'
INSERT INTO user_permissions (UserId, PermissionId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, rp.PermissionId, NOW(), 1, 1
FROM users u
JOIN roles r ON r.Name = 'Customer'
JOIN user_roles ur ON ur.UserId = u.Id AND ur.RoleId = r.Id AND ur.IsActive = 1
JOIN role_permissions rp ON rp.RoleId = r.Id AND rp.IsActive = 1
WHERE u.Username = 'customer3';