-- =====================================================
-- Seed: User-Role Assignments (by username → role name)
-- =====================================================
INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'superadmin' AND r.Name = 'Super Admin';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'admin' AND r.Name = 'Admin';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'quanly' AND r.Name = 'Quản lý công ty';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'nhanvien_kho' AND r.Name = 'Nhân viên';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'nhanvien_bep' AND r.Name = 'Nhân viên';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'nhanvien_banhang' AND r.Name = 'Nhân viên';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'cty_abc' AND r.Name = 'Khách hàng doanh nghiệp';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'truong_xyz' AND r.Name = 'Khách hàng doanh nghiệp';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'shipper1' AND r.Name = 'Nhân viên vận chuyển';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'shipper2' AND r.Name = 'Nhân viên vận chuyển';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'khach1' AND r.Name = 'Khách hàng cá nhân';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'khach2' AND r.Name = 'Khách hàng cá nhân';

INSERT INTO user_roles (UserId, RoleId, AssignedAt, AssignedBy, IsActive)
SELECT u.Id, r.Id, NOW(), 1, 1 FROM users u, roles r WHERE u.Username = 'khach3' AND r.Name = 'Khách hàng cá nhân';
