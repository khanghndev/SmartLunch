-- Phản hồi của quản lý cho đánh giá khách hàng
-- An toàn chạy nhiều lần (run_sql.bat update)

DROP PROCEDURE IF EXISTS smartlunch_add_reviews_manager_reply;
DELIMITER //
CREATE PROCEDURE smartlunch_add_reviews_manager_reply()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'reviews'
          AND COLUMN_NAME = 'ManagerReply'
    ) THEN
        ALTER TABLE reviews
            ADD COLUMN ManagerReply TEXT NULL COMMENT 'Nội dung trả lời từ CSKH/Quản lý' AFTER Comment;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'reviews'
          AND COLUMN_NAME = 'RepliedAt'
    ) THEN
        ALTER TABLE reviews
            ADD COLUMN RepliedAt DATETIME NULL AFTER ManagerReply;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'reviews'
          AND COLUMN_NAME = 'RepliedByUserId'
    ) THEN
        ALTER TABLE reviews
            ADD COLUMN RepliedByUserId INT NULL AFTER RepliedAt;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'reviews'
          AND CONSTRAINT_NAME = 'FK_reviews_replied_by'
    ) THEN
        ALTER TABLE reviews
            ADD CONSTRAINT FK_reviews_replied_by FOREIGN KEY (RepliedByUserId) REFERENCES users(Id) ON DELETE SET NULL;
    END IF;
END //
DELIMITER ;

CALL smartlunch_add_reviews_manager_reply();
DROP PROCEDURE IF EXISTS smartlunch_add_reviews_manager_reply;
