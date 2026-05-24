#!/usr/bin/env python3
"""Generate 20_seed_dish_seasonings.sql — gia vị theo cách chế biến, idempotent."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUTPUT = (
    ROOT
    / "SmartLunch-Backend-Service-Infrastructure/Data/Scripts/04_seed_data/20_seed_dish_seasonings.sql"
)

# (ingredient_name, quantity, unit)
PACKS: dict[str, list[tuple[str, float, str]]] = {
    "stewed": [
        ("Nước mắm", 0.02, "lít"),
        ("Đường cát trắng", 0.01, "kg"),
        ("Muối", 0.004, "kg"),
        ("Tiêu xay", 0.003, "kg"),
        ("Tỏi", 0.01, "kg"),
        ("Hành tím", 0.01, "kg"),
        ("Gừng", 0.01, "kg"),
        ("Dầu thực vật", 0.01, "lít"),
        ("Hạt nêm", 0.003, "kg"),
    ],
    "fried": [
        ("Dầu thực vật", 0.03, "lít"),
        ("Nước mắm", 0.015, "lít"),
        ("Nước tương", 0.01, "lít"),
        ("Tỏi", 0.01, "kg"),
        ("Tiêu xay", 0.003, "kg"),
        ("Muối", 0.003, "kg"),
        ("Hành phi", 0.005, "kg"),
    ],
    "stir_fried": [
        ("Dầu thực vật", 0.02, "lít"),
        ("Nước mắm", 0.015, "lít"),
        ("Nước tương", 0.01, "lít"),
        ("Tỏi", 0.01, "kg"),
        ("Hành tím", 0.008, "kg"),
        ("Đường cát trắng", 0.005, "kg"),
        ("Tiêu xay", 0.003, "kg"),
        ("Hạt nêm", 0.003, "kg"),
    ],
    "grilled": [
        ("Nước mắm", 0.02, "lít"),
        ("Đường cát trắng", 0.01, "kg"),
        ("Tỏi", 0.01, "kg"),
        ("Gừng", 0.01, "kg"),
        ("Sả băm", 0.005, "kg"),
        ("Dầu thực vật", 0.01, "lít"),
        ("Tiêu xay", 0.003, "kg"),
        ("Tương ớt", 0.005, "lít"),
    ],
    "steamed": [
        ("Nước mắm", 0.015, "lít"),
        ("Gừng", 0.015, "kg"),
        ("Hành tím", 0.01, "kg"),
        ("Tỏi", 0.005, "kg"),
        ("Tiêu xay", 0.002, "kg"),
        ("Hành lá", 0.005, "kg"),
    ],
    "boiled": [
        ("Muối", 0.005, "kg"),
        ("Tiêu xay", 0.002, "kg"),
        ("Hành tím", 0.005, "kg"),
        ("Gừng", 0.005, "kg"),
        ("Hạt nêm", 0.003, "kg"),
    ],
    "raw": [
        ("Muối", 0.003, "kg"),
        ("Đường cát trắng", 0.01, "kg"),
        ("Giấm ăn", 0.02, "lít"),
        ("Tỏi", 0.005, "kg"),
        ("Ớt hiểm", 0.002, "kg"),
    ],
}

DESSERT_PACK = [
    ("Đường cát trắng", 0.015, "kg"),
    ("Đường phèn", 0.01, "kg"),
]

DESSERT_NAMES = {
    "Chè bí đỏ",
    "Trái cây dầm",
    "Chè khoai tây đường cát",
    "Nước dừa tươi ngọt lành",
    "Cà rốt ngào đường",
    "Chè bí đỏ nước dừa ngọt",
}

NO_SEASONING_NAMES = {"Cơm trắng"}


def sql_escape(s: str) -> str:
    return s.replace("'", "''")


def emit_pack(
    lines: list[str],
    pack_name: str,
    items: list[tuple[str, float, str]],
    *,
    method_key: str | None = None,
    dish_names: set[str] | None = None,
    exclude_dessert: bool = False,
) -> None:
    for ing_name, qty, unit in items:
        ing = sql_escape(ing_name)
        lines.append(f"-- {pack_name}: {ing_name}")
        if dish_names:
            names_sql = ", ".join(f"'{sql_escape(n)}'" for n in sorted(dish_names))
            where_dish = f"d.Name IN ({names_sql})"
            dessert_join = ""
            dessert_filter = ""
        else:
            where_dish = "1=1"
            dessert_join = """
LEFT JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
LEFT JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'dessert'"""
            dessert_filter = "AND cat.Id IS NULL" if exclude_dessert else ""

        method_filter = ""
        if method_key:
            method_filter = f"AND cm.MethodKey = '{method_key}'"

        no_rice = "AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci"

        lines.append(f"""
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, {qty}, '{unit}'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = '{ing}' COLLATE utf8mb4_unicode_ci
{dessert_join}
WHERE {where_dish}
  {method_filter}
  {dessert_filter}
  {no_rice}
  AND NOT EXISTS (
      SELECT 1 FROM dish_ingredients di
      WHERE di.DishId = d.Id AND di.IngredientId = i.Id
  );""".strip())
        lines.append("")


def main() -> None:
    lines = [
        "-- =====================================================",
        "-- Seed: Gia vị / nước chấm cho định mức món (idempotent)",
        "-- Sinh bởi scratch/generate_dish_seasoning_bom.py",
        "-- Gắn theo cooking_methods.MethodKey; không nhân bản khi chạy lại.",
        "-- =====================================================",
        "",
        "USE SmartLunch;",
        "SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci;",
        "",
    ]

    for method_key, pack in PACKS.items():
        emit_pack(lines, method_key, pack, method_key=method_key, exclude_dessert=True)

    emit_pack(lines, "dessert", DESSERT_PACK, dish_names=DESSERT_NAMES)

    # Món kho cá: thêm mắm ruốc (đặc trưng Nam Bộ) nếu chưa có
    lines.append("-- Kho cá: mắm ruốc (Nam Bộ)")
    lines.append("""
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.003, 'kg'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId AND cm.MethodKey = 'stewed'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Mắm ruốc' COLLATE utf8mb4_unicode_ci
WHERE (d.Name LIKE '%cá %' OR d.Name LIKE 'Cá %' OR d.Name LIKE '%cá lóc%' OR d.Name LIKE '%Cá %')
  AND d.Name COLLATE utf8mb4_unicode_ci <> 'Cơm trắng' COLLATE utf8mb4_unicode_ci
  AND NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);""".strip())
    lines.append("")

    # Canh: thêm nước mắm nhẹ nếu chưa có (một số canh chay chỉ có muối)
    lines.append("-- Canh: nước mắm pha nhẹ")
    lines.append("""
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, 0.01, 'lít'
FROM dishes d
INNER JOIN cooking_methods cm ON cm.Id = d.CookingMethodId AND cm.MethodKey = 'boiled'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = 'Nước mắm' COLLATE utf8mb4_unicode_ci
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'soup'
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);""".strip())
    lines.append("")

    # Phở/bún: thêm gia vị nước dùng
    lines.append("-- Món nước (phở/bún): gia vị nước dùng")
    for ing_name, qty, unit in [
        ("Muối", 0.006, "kg"),
        ("Nước mắm", 0.015, "lít"),
        ("Gừng", 0.01, "kg"),
        ("Hành tím", 0.01, "kg"),
        ("Đường cát trắng", 0.005, "kg"),
        ("Tiêu xay", 0.002, "kg"),
        ("Sả băm", 0.005, "kg"),
    ]:
        ing = sql_escape(ing_name)
        lines.append(f"""
INSERT INTO dish_ingredients (DishId, IngredientId, Quantity, Unit)
SELECT d.Id, i.Id, {qty}, '{unit}'
FROM dishes d
INNER JOIN dish_dish_categories ddc ON ddc.DishId = d.Id
INNER JOIN dish_categories cat ON cat.Id = ddc.DishCategoryId AND cat.SlotKey = 'noodle_soup'
INNER JOIN ingredients i ON i.Name COLLATE utf8mb4_unicode_ci = '{ing}' COLLATE utf8mb4_unicode_ci
WHERE NOT EXISTS (SELECT 1 FROM dish_ingredients di WHERE di.DishId = d.Id AND di.IngredientId = i.Id);""".strip())

    lines.extend(["", "SELECT 'Dish seasoning BOM sync completed.' AS Status;", ""])
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    OUTPUT.write_text("\n".join(lines), encoding="utf-8")
    print(f"Wrote {OUTPUT}")


if __name__ == "__main__":
    main()
