# Dish seed assets (cloud-only)

Ảnh món ăn seed **không lưu trong repo**. File JPEG nằm trên **Appwrite Storage** với object key:

`dishes/seed/{slug}.jpg` (và `{slug}-1.jpg` … `{slug}-3.jpg` cho gallery).

## Khi chạy seed DB

Script `04_seed_data/19_seed_dish_images.sql` sẽ:

1. Ghi `media_files` (Bucket = Appwrite bucket id, `ObjectName` = `dishes/seed/...`)
2. Gắn `dish_images` (cover + gallery)
3. Cập nhật `dishes.ImageUrl` = object key ảnh cover

Không cần thư mục `images/` local.

## Upload ảnh mới (tùy chọn)

```bash
cd SmartLunch-Backend-Service
python scratch/seed_dish_images.py   # tải tạm → upload Appwrite → ghi dish_image_upload_results.json
python scratch/generate_image_sql.py # sinh lại 19_seed_dish_images.sql
```

`manifest.json` giữ metadata (tên, slug, từ khóa tìm ảnh). Thư mục `images/` chỉ là cache tạm khi chạy script upload.
