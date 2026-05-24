import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SQL_FILE = ROOT / "SmartLunch-Backend-Service-Infrastructure/Data/Scripts/04_seed_data/14_seed_dishes.sql"
MANIFEST_FILE = ROOT / "SmartLunch-Backend-Service-Infrastructure/Data/Assets/dishes/manifest.json"

def to_slug(text: str) -> str:
    # Basic slugify for Vietnamese
    import unicodedata
    text = unicodedata.normalize('NFKD', text).encode('ascii', 'ignore').decode('utf-8')
    text = re.sub(r'[^\w\s-]', '', text).strip().lower()
    return re.sub(r'[-\s]+', '-', text)

def main():
    content = SQL_FILE.read_text(encoding="utf-8")
    
    # Pattern: ('Name', 'NameEnglish', 'Description', ...)
    pattern = r"\(\s*'([^']+)'\s*,\s*'([^']+)'\s*,\s*'([^']*)'\s*,"
    matches = re.findall(pattern, content)
    
    dishes = []
    seen = set()
    for name, name_en, desc in matches:
        if name in seen:
            continue
        seen.add(name)
        slug = to_slug(name)
        search = f"{name_en.lower()} vietnamese food"
        dishes.append({
            "name": name,
            "slug": slug,
            "search": search
        })
        
    print(f"Parsed {len(dishes)} dishes from SQL.")
    MANIFEST_FILE.write_text(json.dumps(dishes, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Updated manifest.json with all {len(dishes)} dishes.")

if __name__ == "__main__":
    main()
