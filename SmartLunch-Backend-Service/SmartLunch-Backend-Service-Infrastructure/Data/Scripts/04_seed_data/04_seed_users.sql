-- Seed: Users (password = "123")
INSERT INTO users (Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, IsActive, IsEmailVerified, EmailVerifiedAt) VALUES
('superadmin', 'superadmin@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Super', 'Admin', '+84901000001', 1, 1, NOW()),
('admin', 'admin@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Quản Trị', 'Viên', '+84901000002', 1, 1, NOW()),
('quanly', 'quanly@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Minh', 'Nguyễn', '+84901000003', 1, 1, NOW()),
('nhanvien_kho', 'kho@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Kho', 'Nguyễn', '+84901000004', 1, 1, NOW()),
('nhanvien_bep', 'bep@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Bếp', 'Trần', '+84901000005', 1, 1, NOW()),
('nhanvien_banhang', 'sales@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Bán Hàng', 'Lê', '+84901000006', 1, 1, NOW()),
('cty_abc', 'contact@abctech.vn', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Đại Diện', 'ABC Tech', '+84901000007', 1, 1, NOW()),
('truong_xyz', 'contact@xyzschool.edu.vn', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Hương', 'Lê', '+84901000008', 1, 1, NOW()),
('shipper1', 'shipper1@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Giao', 'Hàng', '+84901000009', 1, 1, NOW()),
('shipper2', 'shipper2@smartlunch.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Vận', 'Chuyển', '+84901000010', 1, 1, NOW()),
('khach1', 'khach1@gmail.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Anh', 'Phạm', '+84901000011', 1, 1, NOW()),
('khach2', 'khach2@gmail.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Bình', 'Võ', '+84901000012', 1, 1, NOW()),
('khach3', 'khach3@gmail.com', '$2y$10$YjWZK8lORcwNgwbErRJwr.wuBnmk5KU16xxlyNyfKkkHrlZkFlE7u', 'Chi', 'Đỗ', '+84901000013', 1, 0, NULL);
