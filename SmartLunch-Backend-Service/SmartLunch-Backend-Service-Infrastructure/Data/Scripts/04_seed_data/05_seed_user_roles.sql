-- =====================================================
-- Seed: User-Role Assignments (by username → role name)
-- Expect usernames to match those seeded in 04_seed_users.sql
-- Role names must match those seeded in 01_seed_roles.sql
-- =====================================================
-- Admin
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'admin' AND r.Name = 'Admin';

-- Manager
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'manager' AND r.Name = 'Manager';

-- WarehouseStaff
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'warehouse_staff' AND r.Name = 'WarehouseStaff';

-- ChefStaff
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'chef_staff' AND r.Name = 'ChefStaff';

-- SalesStaff
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'sales_staff' AND r.Name = 'SalesStaff';

-- Organization (B2B customers)
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'abc_company' AND r.Name = 'Organization';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'xyz_school' AND r.Name = 'Organization';

-- Shipper
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'shipper1' AND r.Name = 'Shipper';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'shipper2' AND r.Name = 'Shipper';

-- Customer (B2C)
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'customer1' AND r.Name = 'Customer';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'customer2' AND r.Name = 'Customer';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'customer3' AND r.Name = 'Customer';
