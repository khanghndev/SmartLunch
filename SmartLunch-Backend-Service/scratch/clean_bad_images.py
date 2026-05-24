import pathlib
import hashlib
from collections import defaultdict

ROOT = pathlib.Path(__file__).resolve().parents[1]
ASSETS_DIR = ROOT / "SmartLunch-Backend-Service-Infrastructure/Data/Assets/dishes/images"

def main():
    if not ASSETS_DIR.exists():
        print(f"Directory not found: {ASSETS_DIR}")
        return 1

    files = list(ASSETS_DIR.glob("*.jpg"))
    hash_map = defaultdict(list)

    for p in files:
        base = p.name.replace(".jpg", "")
        for s in ["-1", "-2", "-3"]:
            if base.endswith(s):
                base = base[:-len(s)]
                break
        h = hashlib.md5(p.read_bytes()).hexdigest()
        hash_map[h].append((p, base))

    deleted = 0
    for h, items in hash_map.items():
        unique_slugs = set(slug for _, slug in items)
        if len(unique_slugs) > 1:
            for p, _ in items:
                if p.exists():
                    p.unlink()
                    print(f"[deleted duplicate] {p.name}")
                    deleted += 1
        else:
            for p, _ in items:
                if p.exists() and p.stat().st_size < 25000:
                    p.unlink()
                    print(f"[deleted placeholder] {p.name}")
                    deleted += 1

    print(f"Cleanup done. Deleted total {deleted} files.")
    return 0

if __name__ == "__main__":
    main()
