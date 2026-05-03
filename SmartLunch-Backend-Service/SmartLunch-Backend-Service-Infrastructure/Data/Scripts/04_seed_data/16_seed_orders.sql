-- =====================================================
-- Seed: Orders, Order Items, Deliveries, Payments
-- =====================================================
SET @sales = (SELECT Id FROM users WHERE Username = 'nhanvien_banhang');
SET @shipper = (SELECT Id FROM users WHERE Username = 'shipper1');
SET @cty_abc = (SELECT Id FROM users WHERE Username = 'cty_abc');
SET @truong_xyz = (SELECT Id FROM users WHERE Username = 'truong_xyz');
SET @khach1 = (SELECT Id FROM users WHERE Username = 'khach1');
SET @khach2 = (SELECT Id FROM users WHERE Username = 'khach2');
SET @org_abc = (SELECT Id FROM organizations WHERE Name = 'Công ty TNHH ABC Tech');
SET @org_xyz = (SELECT Id FROM organizations WHERE Name = 'Trường THPT XYZ');
SET @contract_abc = (SELECT Id FROM contracts WHERE ContractNumber = 'HD-B2B-ABC-001');
SET @contract_xyz = (SELECT Id FROM contracts WHERE ContractNumber = 'HD-B2B-XYZ-002');

-- Order 1: B2B ABC Tech (delivered)
INSERT INTO orders (UserId, ContractId, ScheduledDate, Status, TotalAmount, PaymentStatus, InvoiceCode, CreatedBySalesUserId, CreatedAt) VALUES
(@cty_abc, @contract_abc, DATE_SUB(CURDATE(), INTERVAL 2 DAY), 'delivered', 4500000, 'paid', 'INV-20250321-001', @sales, DATE_SUB(NOW(), INTERVAL 3 DAY));
SET @ord1 = LAST_INSERT_ID();

INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord1, Id, 50, 45000, 2250000 FROM dishes WHERE Name = 'Cơm sườn nướng';
INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord1, Id, 50, 20000, 1000000 FROM dishes WHERE Name = 'Rau muống xào tỏi';
INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord1, Id, 50, 25000, 1250000 FROM dishes WHERE Name = 'Canh chua tôm';

-- Order 2: B2B School XYZ (confirmed)
INSERT INTO orders (UserId, ContractId, ScheduledDate, Status, TotalAmount, PaymentStatus, InvoiceCode, CreatedBySalesUserId, CreatedAt) VALUES
(@truong_xyz, @contract_xyz, CURDATE(), 'confirmed', 3360000, 'unpaid', 'INV-20250323-002', @sales, DATE_SUB(NOW(), INTERVAL 1 DAY));
SET @ord2 = LAST_INSERT_ID();

INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord2, Id, 40, 42000, 1680000 FROM dishes WHERE Name = 'Cơm gà chiên mắm';
INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord2, Id, 40, 22000, 880000 FROM dishes WHERE Name = 'Đậu hũ sốt cà chua';
INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord2, Id, 40, 20000, 800000 FROM dishes WHERE Name = 'Rau muống xào tỏi';

-- Order 3: Individual khach1 (pending)
INSERT INTO orders (UserId, ContractId, ScheduledDate, Status, TotalAmount, PaymentStatus) VALUES
(@khach1, @contract_abc, CURDATE(), 'pending', 80000, 'unpaid');
SET @ord3 = LAST_INSERT_ID();

INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord3, Id, 1, 45000, 45000 FROM dishes WHERE Name = 'Cơm sườn nướng';
INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord3, Id, 1, 35000, 35000 FROM dishes WHERE Name = 'Canh chua tôm';

-- Order 4: Individual khach2 (delivered)
INSERT INTO orders (UserId, ContractId, ScheduledDate, Status, TotalAmount, PaymentStatus, CreatedAt) VALUES
(@khach2, @contract_xyz, DATE_SUB(CURDATE(), INTERVAL 1 DAY), 'delivered', 62000, 'paid', DATE_SUB(NOW(), INTERVAL 2 DAY));
SET @ord4 = LAST_INSERT_ID();

INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord4, Id, 1, 42000, 42000 FROM dishes WHERE Name = 'Cơm gà chiên mắm';
INSERT INTO order_items (OrderId, DishId, Quantity, UnitPrice, TotalPrice)
SELECT @ord4, Id, 1, 20000, 20000 FROM dishes WHERE Name = 'Rau muống xào tỏi';

-- Deliveries
INSERT INTO deliveries (OrderId, AssignedStaffId, DeliveryAddress, DeliveryStatus, DeliveredAt, Notes) VALUES
(@ord1, @shipper, '123 Nguyễn Huệ, Q.1, TP.HCM', 'completed', DATE_SUB(NOW(), INTERVAL 2 DAY), 'Giao thành công'),
(@ord2, @shipper, '456 Lê Lợi, Q.3, TP.HCM', 'pending', NULL, 'Dự kiến giao 11:00'),
(@ord4, @shipper, '456 Lê Lợi, Q.3, TP.HCM', 'completed', DATE_SUB(NOW(), INTERVAL 1 DAY), 'Đã giao');

-- Payments
INSERT INTO payments (OrderId, PayerId, PaymentDate, Amount, Method, Status) VALUES
(@ord1, @cty_abc, DATE_SUB(NOW(), INTERVAL 2 DAY), 4500000, 'bank_transfer', 'paid'),
(@ord4, @khach2, DATE_SUB(NOW(), INTERVAL 1 DAY), 62000, 'e_wallet', 'paid');
