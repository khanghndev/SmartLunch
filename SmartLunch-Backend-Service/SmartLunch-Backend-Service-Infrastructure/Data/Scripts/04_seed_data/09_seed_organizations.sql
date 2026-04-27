-- Seed: Organizations & User-Organization memberships
INSERT INTO organizations (Name, Address, Phone, ContactPerson, ContactEmail, IsActive, CreatedBy) VALUES
('Công ty TNHH ABC Tech', '123 Nguyễn Huệ, Q.1, TP.HCM', '+84281234567', 'Đại Diện ABC', 'contact@abctech.vn', 1, (SELECT Id FROM users WHERE Username = 'admin')),
('Trường THPT XYZ', '456 Lê Lợi, Q.3, TP.HCM', '+84289876543', 'Hương Lê', 'contact@xyzschool.edu.vn', 1, (SELECT Id FROM users WHERE Username = 'admin')),
('Nhà máy Sản Xuất DEF', '789 KCN Tân Bình, TP.HCM', '+84287654321', 'Tùng Phạm', 'tung@def-factory.vn', 1, (SELECT Id FROM users WHERE Username = 'admin')),
('Văn phòng GHI Corp', '321 Pasteur, Q.1, TP.HCM', '+84283216549', 'Lan Nguyễn', 'lan@ghi-corp.vn', 1, (SELECT Id FROM users WHERE Username = 'admin'));

INSERT INTO user_organizations (UserId, OrganizationId, JoinedAt, IsActive)
SELECT u.Id, un.Id, NOW(), 1 FROM users u, organizations un WHERE u.Username = 'cty_abc' AND un.Name = 'Công ty TNHH ABC Tech';
INSERT INTO user_organizations (UserId, OrganizationId, JoinedAt, IsActive)
SELECT u.Id, un.Id, NOW(), 1 FROM users u, organizations un WHERE u.Username = 'truong_xyz' AND un.Name = 'Trường THPT XYZ';
INSERT INTO user_organizations (UserId, OrganizationId, JoinedAt, IsActive)
SELECT u.Id, un.Id, NOW(), 1 FROM users u, organizations un WHERE u.Username = 'khach1' AND un.Name = 'Công ty TNHH ABC Tech';
INSERT INTO user_organizations (UserId, OrganizationId, JoinedAt, IsActive)
SELECT u.Id, un.Id, NOW(), 1 FROM users u, organizations un WHERE u.Username = 'khach2' AND un.Name = 'Trường THPT XYZ';
