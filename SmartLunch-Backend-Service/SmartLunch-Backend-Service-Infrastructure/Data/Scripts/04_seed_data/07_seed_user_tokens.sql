-- =====================================================
-- Seed: User Tokens
-- =====================================================
INSERT INTO user_tokens (UserId, AccessToken, RefreshToken, IssuedAt, ExpiresAt, IsActive)
SELECT Id, 'sample_access_token_khach1', 'sample_refresh_token_khach1', DATE_SUB(NOW(), INTERVAL 1 HOUR), DATE_ADD(NOW(), INTERVAL 23 HOUR), 1
FROM users WHERE Username = 'khach1';

INSERT INTO user_tokens (UserId, AccessToken, RefreshToken, IssuedAt, ExpiresAt, IsActive)
SELECT Id, 'expired_token_khach2', 'expired_refresh_khach2', DATE_SUB(NOW(), INTERVAL 2 DAY), DATE_SUB(NOW(), INTERVAL 1 DAY), 0
FROM users WHERE Username = 'khach2';
