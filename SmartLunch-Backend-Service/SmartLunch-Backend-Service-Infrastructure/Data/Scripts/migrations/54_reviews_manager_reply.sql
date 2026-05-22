-- Phản hồi của quản lý cho đánh giá khách hàng
ALTER TABLE reviews
    ADD COLUMN ManagerReply TEXT NULL COMMENT 'Nội dung trả lời từ CSKH/Quản lý' AFTER Comment,
    ADD COLUMN RepliedAt DATETIME NULL AFTER ManagerReply,
    ADD COLUMN RepliedByUserId INT NULL AFTER RepliedAt,
    ADD CONSTRAINT FK_reviews_replied_by FOREIGN KEY (RepliedByUserId) REFERENCES users(Id) ON DELETE SET NULL;
