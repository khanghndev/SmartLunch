# Dish seed assets (cloud-only)

Ảnh món ăn seed **không lưu trong repo**. File nằm trên **Appwrite Storage** (`dishes/seed/...` hoặc `users/{id}/dish-image/...`).

## Nguồn chuẩn (reference)

File dump HeidiSQL: `Data/Scripts/reference/dish_reference.sql`  
(Copy từ export DB thực tế — mỗi món một `dishes.ImageUrl` đúng như production.)

## Khi chạy seed DB (`run_sql.bat update`)

`04_seed_data/19_seed_dish_images.sql` (idempotent):

1. `UPDATE dishes.ImageUrl` theo tên món từ reference
2. `DELETE` toàn bộ `dish_images` của món có ảnh (không nhân bản khi chạy lại)
3. `INSERT IGNORE media_files` theo `(Bucket, ObjectName)`
4. `INSERT` **một** `dish_images` cover / món

## Sinh lại seed từ dump

```bash
cd SmartLunch-Backend-Service
# Cập nhật reference/dish_reference.sql từ export mới (nếu cần)
python scratch/generate_dish_image_seed_from_dump.py
```

Tùy chọn:

```bash
python scratch/generate_dish_image_seed_from_dump.py --input "C:/path/to/dish.sql"
```

## Upload ảnh seed mới lên Appwrite (tùy chọn)

```bash
python scratch/seed_dish_images.py
```

Sau đó export DB hoặc cập nhật `dish_reference.sql` rồi chạy generator ở trên.
