-- =====================================================
-- Seed: Media Files
-- =====================================================
INSERT INTO media_files (OwnerUserId, Bucket, ObjectName, OriginalFileName, ContentType, SizeBytes, MediaType, IsPublic)
SELECT Id, 'smartlunch-storage', 'avatars/admin_avatar.jpg', 'admin_avatar.jpg', 'image/jpeg', 52480, 'image', 1
FROM users WHERE Username = 'admin';

INSERT INTO media_files (OwnerUserId, Bucket, ObjectName, OriginalFileName, ContentType, SizeBytes, MediaType, IsPublic)
SELECT Id, 'smartlunch-storage', 'menu/banner_weekly.jpg', 'banner_weekly.jpg', 'image/jpeg', 204800, 'image', 1
FROM users WHERE Username = 'admin';
