-- Seed: Users (password = "123")
INSERT INTO users (Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, IsActive, IsEmailVerified, EmailVerifiedAt) VALUES
('admin', 'admin@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Quản Trị', 'Viên', '+84901000002', 1, 1, NOW()),
('manager', 'manager@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Minh', 'Nguyễn', '+84901000003', 1, 1, NOW()),
('warehouse_staff', 'warehouse@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Khoân Viên', 'Nguyễn', '+84901000004', 1, 1, NOW()),
('chef_staff', 'chef@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Bếp', 'Trần', '+84901000005', 1, 1, NOW()),
('sales_staff', 'sales@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Bán Hàng', 'Lê', '+84901000006', 1, 1, NOW()),
('abc_company', 'contact@abctech.vn', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Đại Diện', 'ABC Tech', '+84901000007', 1, 1, NOW()),
('xyz_school', 'contact@xyzschool.edu.vn', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Hương', 'Lê', '+84901000008', 1, 1, NOW()),
('shipper1', 'shipper1@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Giao', 'Hàng', '+84901000009', 1, 1, NOW()),
('shipper2', 'shipper2@smartlunch.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Vận', 'Chuyển', '+84901000010', 1, 1, NOW()),
('customer1', 'customer1@gmail.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Anh', 'Phạm', '+84901000011', 1, 1, NOW()),
('customer2', 'customer2@gmail.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Bình', 'Võ', '+84901000012', 1, 1, NOW()),
('customer3', 'customer3@gmail.com', '$2y$10$EeZcgdvgOqnUyCOR9gLJkupJgCFsu0TvqIYh7hfQOGOjWkNJuYHAC', 'Chi', 'Đỗ', '+84901000013', 1, 0, NULL);
