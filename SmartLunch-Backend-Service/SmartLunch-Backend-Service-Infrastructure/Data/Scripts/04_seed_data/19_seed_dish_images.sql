-- =====================================================
-- Seed: Dish cover images (media_files + dish_images)
-- ObjectName: dishes/seed/{slug}.jpg on Appwrite bucket 69bfa6de000fdacda87d
-- Run scratch/seed_dish_images.py first to upload JPEG files to Appwrite.
-- BE resolves public URL at runtime from ObjectName + IsPublic=1.
-- =====================================================

SET @bucket := '69bfa6de000fdacda87d';

INSERT INTO media_files (OwnerUserId, Bucket, ObjectName, OriginalFileName, ContentType, SizeBytes, MediaType, IsPublic)
SELECT u.Id, @bucket, v.ObjectName, v.OriginalFileName, 'image/jpeg', 120000, 'image', 1
FROM users u
CROSS JOIN (
    SELECT 'dishes/seed/com-suon-nuong.jpg' AS ObjectName, 'com-suon-nuong.jpg' AS OriginalFileName UNION ALL
    SELECT 'dishes/seed/com-ga-chien-mam.jpg', 'com-ga-chien-mam.jpg' UNION ALL
    SELECT 'dishes/seed/com-chien-duong-chau.jpg', 'com-chien-duong-chau.jpg' UNION ALL
    SELECT 'dishes/seed/thit-heo-kho-trung.jpg', 'thit-heo-kho-trung.jpg' UNION ALL
    SELECT 'dishes/seed/ga-kho-gung.jpg', 'ga-kho-gung.jpg' UNION ALL
    SELECT 'dishes/seed/ca-loc-kho-to.jpg', 'ca-loc-kho-to.jpg' UNION ALL
    SELECT 'dishes/seed/thit-bo-xao-hanh-tay.jpg', 'thit-bo-xao-hanh-tay.jpg' UNION ALL
    SELECT 'dishes/seed/com-ca-basa-kho-to.jpg', 'com-ca-basa-kho-to.jpg' UNION ALL
    SELECT 'dishes/seed/thit-heo-xao-ca-chua.jpg', 'thit-heo-xao-ca-chua.jpg' UNION ALL
    SELECT 'dishes/seed/ga-nuong-sa.jpg', 'ga-nuong-sa.jpg' UNION ALL
    SELECT 'dishes/seed/tom-rang-me.jpg', 'tom-rang-me.jpg' UNION ALL
    SELECT 'dishes/seed/ca-basa-chien-gion.jpg', 'ca-basa-chien-gion.jpg' UNION ALL
    SELECT 'dishes/seed/bo-xao-ca-chua.jpg', 'bo-xao-ca-chua.jpg' UNION ALL
    SELECT 'dishes/seed/com-rang-thit-ga.jpg', 'com-rang-thit-ga.jpg' UNION ALL
    SELECT 'dishes/seed/thit-heo-xao-sa-ot.jpg', 'thit-heo-xao-sa-ot.jpg' UNION ALL
    SELECT 'dishes/seed/ga-hap-gung.jpg', 'ga-hap-gung.jpg' UNION ALL
    SELECT 'dishes/seed/canh-chua-tom.jpg', 'canh-chua-tom.jpg' UNION ALL
    SELECT 'dishes/seed/canh-bi-do-thit-bam.jpg', 'canh-bi-do-thit-bam.jpg' UNION ALL
    SELECT 'dishes/seed/canh-cai-ngot-thit-bam.jpg', 'canh-cai-ngot-thit-bam.jpg' UNION ALL
    SELECT 'dishes/seed/canh-kho-qua-nhoi-thit.jpg', 'canh-kho-qua-nhoi-thit.jpg' UNION ALL
    SELECT 'dishes/seed/canh-mong-toi-tom.jpg', 'canh-mong-toi-tom.jpg' UNION ALL
    SELECT 'dishes/seed/canh-rau-cu-thit-bam.jpg', 'canh-rau-cu-thit-bam.jpg' UNION ALL
    SELECT 'dishes/seed/canh-ca-chua-trung.jpg', 'canh-ca-chua-trung.jpg' UNION ALL
    SELECT 'dishes/seed/canh-dau-hu-ca-chua.jpg', 'canh-dau-hu-ca-chua.jpg' UNION ALL
    SELECT 'dishes/seed/canh-khoai-tay-thit-bam.jpg', 'canh-khoai-tay-thit-bam.jpg' UNION ALL
    SELECT 'dishes/seed/canh-cai-ngot-ca-chua.jpg', 'canh-cai-ngot-ca-chua.jpg' UNION ALL
    SELECT 'dishes/seed/canh-kho-qua-ca-chua.jpg', 'canh-kho-qua-ca-chua.jpg' UNION ALL
    SELECT 'dishes/seed/rau-muong-xao-toi.jpg', 'rau-muong-xao-toi.jpg' UNION ALL
    SELECT 'dishes/seed/dau-hu-sot-ca-chua.jpg', 'dau-hu-sot-ca-chua.jpg' UNION ALL
    SELECT 'dishes/seed/cai-ngot-xao-toi.jpg', 'cai-ngot-xao-toi.jpg' UNION ALL
    SELECT 'dishes/seed/dau-cove-xao-thit.jpg', 'dau-cove-xao-thit.jpg' UNION ALL
    SELECT 'dishes/seed/bi-do-xao-tom.jpg', 'bi-do-xao-tom.jpg' UNION ALL
    SELECT 'dishes/seed/ca-rot-xao-trung.jpg', 'ca-rot-xao-trung.jpg' UNION ALL
    SELECT 'dishes/seed/khoai-tay-xao-thit.jpg', 'khoai-tay-xao-thit.jpg' UNION ALL
    SELECT 'dishes/seed/bi-do-xao-toi.jpg', 'bi-do-xao-toi.jpg' UNION ALL
    SELECT 'dishes/seed/cai-ngot-luoc.jpg', 'cai-ngot-luoc.jpg' UNION ALL
    SELECT 'dishes/seed/ca-chua-xao-trung.jpg', 'ca-chua-xao-trung.jpg' UNION ALL
    SELECT 'dishes/seed/trung-chien.jpg', 'trung-chien.jpg' UNION ALL
    SELECT 'dishes/seed/khoai-tay-chien.jpg', 'khoai-tay-chien.jpg' UNION ALL
    SELECT 'dishes/seed/dau-hu-chien-gion.jpg', 'dau-hu-chien-gion.jpg' UNION ALL
    SELECT 'dishes/seed/trung-luoc.jpg', 'trung-luoc.jpg' UNION ALL
    SELECT 'dishes/seed/dua-leo-tron.jpg', 'dua-leo-tron.jpg' UNION ALL
    SELECT 'dishes/seed/salad-dua-leo-ca-rot.jpg', 'salad-dua-leo-ca-rot.jpg' UNION ALL
    SELECT 'dishes/seed/sot-ca-chua-cham.jpg', 'sot-ca-chua-cham.jpg' UNION ALL
    SELECT 'dishes/seed/khoai-tay-luoc.jpg', 'khoai-tay-luoc.jpg' UNION ALL
    SELECT 'dishes/seed/ca-rot-muoi-chua.jpg', 'ca-rot-muoi-chua.jpg' UNION ALL
    SELECT 'dishes/seed/che-bi-do.jpg', 'che-bi-do.jpg' UNION ALL
    SELECT 'dishes/seed/trai-cay-dam.jpg', 'trai-cay-dam.jpg' UNION ALL
    SELECT 'dishes/seed/pho-bo.jpg', 'pho-bo.jpg' UNION ALL
    SELECT 'dishes/seed/banh-canh-cua.jpg', 'banh-canh-cua.jpg'
) AS v
WHERE u.Username = 'admin';

INSERT INTO dish_images (DishId, MediaFileId, Role, SortOrder)
SELECT d.Id, m.Id, 'cover', 0
FROM dishes d
INNER JOIN media_files m ON m.ObjectName = CASE d.Name
    WHEN 'Cơm sườn nướng' THEN 'dishes/seed/com-suon-nuong.jpg'
    WHEN 'Cơm gà chiên mắm' THEN 'dishes/seed/com-ga-chien-mam.jpg'
    WHEN 'Cơm chiên dương châu' THEN 'dishes/seed/com-chien-duong-chau.jpg'
    WHEN 'Thịt heo kho trứng' THEN 'dishes/seed/thit-heo-kho-trung.jpg'
    WHEN 'Gà kho gừng' THEN 'dishes/seed/ga-kho-gung.jpg'
    WHEN 'Cá lóc kho tộ' THEN 'dishes/seed/ca-loc-kho-to.jpg'
    WHEN 'Thịt bò xào hành tây' THEN 'dishes/seed/thit-bo-xao-hanh-tay.jpg'
    WHEN 'Cơm cá basa kho tộ' THEN 'dishes/seed/com-ca-basa-kho-to.jpg'
    WHEN 'Thịt heo xào cà chua' THEN 'dishes/seed/thit-heo-xao-ca-chua.jpg'
    WHEN 'Gà nướng sả' THEN 'dishes/seed/ga-nuong-sa.jpg'
    WHEN 'Tôm rang me' THEN 'dishes/seed/tom-rang-me.jpg'
    WHEN 'Cá basa chiên giòn' THEN 'dishes/seed/ca-basa-chien-gion.jpg'
    WHEN 'Bò xào cà chua' THEN 'dishes/seed/bo-xao-ca-chua.jpg'
    WHEN 'Cơm rang thịt gà' THEN 'dishes/seed/com-rang-thit-ga.jpg'
    WHEN 'Thịt heo xào sả ớt' THEN 'dishes/seed/thit-heo-xao-sa-ot.jpg'
    WHEN 'Gà hấp gừng' THEN 'dishes/seed/ga-hap-gung.jpg'
    WHEN 'Canh chua tôm' THEN 'dishes/seed/canh-chua-tom.jpg'
    WHEN 'Canh bí đỏ thịt bằm' THEN 'dishes/seed/canh-bi-do-thit-bam.jpg'
    WHEN 'Canh cải ngọt thịt bằm' THEN 'dishes/seed/canh-cai-ngot-thit-bam.jpg'
    WHEN 'Canh khổ qua nhồi thịt' THEN 'dishes/seed/canh-kho-qua-nhoi-thit.jpg'
    WHEN 'Canh mồng tơi tôm' THEN 'dishes/seed/canh-mong-toi-tom.jpg'
    WHEN 'Canh rau củ thịt bằm' THEN 'dishes/seed/canh-rau-cu-thit-bam.jpg'
    WHEN 'Canh cà chua trứng' THEN 'dishes/seed/canh-ca-chua-trung.jpg'
    WHEN 'Canh đậu hũ cà chua' THEN 'dishes/seed/canh-dau-hu-ca-chua.jpg'
    WHEN 'Canh khoai tây thịt bằm' THEN 'dishes/seed/canh-khoai-tay-thit-bam.jpg'
    WHEN 'Canh cải ngọt cà chua' THEN 'dishes/seed/canh-cai-ngot-ca-chua.jpg'
    WHEN 'Canh khổ qua cà chua' THEN 'dishes/seed/canh-kho-qua-ca-chua.jpg'
    WHEN 'Rau muống xào tỏi' THEN 'dishes/seed/rau-muong-xao-toi.jpg'
    WHEN 'Đậu hũ sốt cà chua' THEN 'dishes/seed/dau-hu-sot-ca-chua.jpg'
    WHEN 'Cải ngọt xào tỏi' THEN 'dishes/seed/cai-ngot-xao-toi.jpg'
    WHEN 'Đậu cove xào thịt' THEN 'dishes/seed/dau-cove-xao-thit.jpg'
    WHEN 'Bí đỏ xào tôm' THEN 'dishes/seed/bi-do-xao-tom.jpg'
    WHEN 'Cà rốt xào trứng' THEN 'dishes/seed/ca-rot-xao-trung.jpg'
    WHEN 'Khoai tây xào thịt' THEN 'dishes/seed/khoai-tay-xao-thit.jpg'
    WHEN 'Bí đỏ xào tỏi' THEN 'dishes/seed/bi-do-xao-toi.jpg'
    WHEN 'Cải ngọt luộc' THEN 'dishes/seed/cai-ngot-luoc.jpg'
    WHEN 'Cà chua xào trứng' THEN 'dishes/seed/ca-chua-xao-trung.jpg'
    WHEN 'Trứng chiên' THEN 'dishes/seed/trung-chien.jpg'
    WHEN 'Khoai tây chiên' THEN 'dishes/seed/khoai-tay-chien.jpg'
    WHEN 'Đậu hũ chiên giòn' THEN 'dishes/seed/dau-hu-chien-gion.jpg'
    WHEN 'Trứng luộc' THEN 'dishes/seed/trung-luoc.jpg'
    WHEN 'Dưa leo trộn' THEN 'dishes/seed/dua-leo-tron.jpg'
    WHEN 'Salad dưa leo cà rốt' THEN 'dishes/seed/salad-dua-leo-ca-rot.jpg'
    WHEN 'Sốt cà chua chấm' THEN 'dishes/seed/sot-ca-chua-cham.jpg'
    WHEN 'Khoai tây luộc' THEN 'dishes/seed/khoai-tay-luoc.jpg'
    WHEN 'Cà rốt muối chua' THEN 'dishes/seed/ca-rot-muoi-chua.jpg'
    WHEN 'Chè bí đỏ' THEN 'dishes/seed/che-bi-do.jpg'
    WHEN 'Trái cây dầm' THEN 'dishes/seed/trai-cay-dam.jpg'
    WHEN 'Phở bò' THEN 'dishes/seed/pho-bo.jpg'
    WHEN 'Bánh canh cua' THEN 'dishes/seed/banh-canh-cua.jpg'
END;
