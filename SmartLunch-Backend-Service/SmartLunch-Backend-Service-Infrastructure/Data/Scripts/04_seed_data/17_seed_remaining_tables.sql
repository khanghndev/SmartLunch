-- =====================================================
-- Seed: Transactions, Reviews, Sentiments, Complaints, Chatbot, Menu Suggestions
-- =====================================================

-- Transactions
INSERT INTO transactions (Date, Description, Amount, Category, Method) VALUES
(DATE_SUB(NOW(), INTERVAL 2 DAY), 'Thu tiền đơn INV-20250321-001 ABC Tech', 4500000, 'income_order', 'bank_transfer'),
(DATE_SUB(NOW(), INTERVAL 1 DAY), 'Thu tiền đơn khách lẻ', 62000, 'income_order', 'e_wallet'),
(DATE_SUB(NOW(), INTERVAL 5 DAY), 'Thanh toán NCC Thực Phẩm Sạch Việt', -45000000, 'expense_supplier', 'bank_transfer'),
(DATE_SUB(NOW(), INTERVAL 7 DAY), 'Chi phí gas bếp tháng 3', -2500000, 'expense_utility', 'cash'),
(DATE_SUB(NOW(), INTERVAL 7 DAY), 'Chi phí điện nước tháng 3', -4200000, 'expense_utility', 'bank_transfer');

-- Reviews
INSERT INTO reviews (UserId, DishId, OrderId, Rating, Comment)
SELECT (SELECT Id FROM users WHERE Username = 'khach1'), (SELECT Id FROM dishes WHERE Name = 'Cơm sườn nướng'), @ord3, 5, 'Cơm sườn nướng rất ngon, thịt mềm gia vị vừa ăn!';
INSERT INTO reviews (UserId, DishId, OrderId, Rating, Comment)
SELECT (SELECT Id FROM users WHERE Username = 'khach2'), (SELECT Id FROM dishes WHERE Name = 'Cơm gà chiên mắm'), @ord4, 4, 'Gà chiên mắm ngon, hơi mặn. Cần giảm nước mắm.';
INSERT INTO reviews (UserId, DishId, OrderId, Rating, Comment)
SELECT (SELECT Id FROM users WHERE Username = 'cty_abc'), (SELECT Id FROM dishes WHERE Name = 'Cơm chiên dương châu'), @ord1, 5, 'Cơm chiên dương châu rất ngon, nhân viên công ty rất thích!';

-- Sentiments
INSERT INTO sentiments (ReviewId, SentimentLabel, Confidence)
SELECT Id, 'positive', 0.95 FROM reviews WHERE Comment LIKE '%rất ngon, thịt mềm%';
INSERT INTO sentiments (ReviewId, SentimentLabel, Confidence)
SELECT Id, 'positive', 0.72 FROM reviews WHERE Comment LIKE '%hơi mặn%';
INSERT INTO sentiments (ReviewId, SentimentLabel, Confidence)
SELECT Id, 'positive', 0.98 FROM reviews WHERE Comment LIKE '%nhân viên công ty%';

-- Complaints
INSERT INTO complaints (UserId, OrderId, Title, Description, Status, AssignedTo, ResolvedAt)
SELECT (SELECT Id FROM users WHERE Username = 'truong_xyz'), @ord2, 'Giao hàng trễ', 'Đơn hàng cho trường XYZ bị giao trễ 30 phút.', 'in_progress', (SELECT Id FROM users WHERE Username = 'admin'), NULL;
INSERT INTO complaints (UserId, OrderId, Title, Description, Status, AssignedTo)
SELECT (SELECT Id FROM users WHERE Username = 'khach1'), @ord3, 'Thiếu món', 'Đặt 2 món nhưng chỉ nhận 1. Thiếu canh chua tôm.', 'new', NULL;

-- Chatbot logs
INSERT INTO chatbot_logs (UserId, Message, Response) VALUES
((SELECT Id FROM users WHERE Username = 'khach1'), 'Hôm nay có món gì ăn trưa?', 'Hôm nay thực đơn trưa gồm: Cơm sườn nướng, Rau muống xào tỏi, Canh chua tôm.'),
((SELECT Id FROM users WHERE Username = 'khach2'), 'Tôi muốn xem đánh giá món cơm gà', 'Cơm gà chiên mắm có đánh giá trung bình 4/5 sao.'),
(NULL, 'Giờ đặt cơm trưa là mấy giờ?', 'Bạn có thể đặt cơm trưa trước 9:00 sáng hàng ngày.');

-- Menu Suggestions (AI)
INSERT INTO menu_suggestions (WeekStart, SuggestionText, AlgorithmVersion, CreatedBy) VALUES
('2025-03-31', 'Dựa trên đánh giá tuần trước:\n- T2: Cơm sườn nướng (5★), Đậu hũ sốt cà chua\n- T3: Cơm gà chiên mắm, Khoai tây chiên\n- T4: Cơm chiên dương châu (5★), Rau muống xào tỏi\nLưu ý: Giảm nước mắm trong gà chiên.', 'SmartLunch-AI-v1.2', (SELECT Id FROM users WHERE Username = 'admin')),
('2025-04-07', 'Thực đơn tuần 2/4:\n- Tăng món chay (xu hướng +15%)\n- Bổ sung Bún bò Huế\n- Giữ Cơm sườn nướng (top 1)', 'SmartLunch-AI-v1.2', (SELECT Id FROM users WHERE Username = 'admin'));
