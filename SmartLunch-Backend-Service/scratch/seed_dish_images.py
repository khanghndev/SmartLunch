#!/usr/bin/env python3
"""Download dish cover images (Wikimedia Commons) and upload to Appwrite Storage."""

from __future__ import annotations

import hashlib
import json
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
from io import BytesIO
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "SmartLunch-Backend-Service-Infrastructure/Data/Assets/dishes/manifest.json"
ASSETS_DIR = ROOT / "SmartLunch-Backend-Service-Infrastructure/Data/Assets/dishes/images"
APPSETTINGS = ROOT / "SmartLunch-Backend-Service-API/appsettings.Development.json"

USER_AGENT = "SmartLunchSeed/1.0 (thesis demo)"
OBJECT_PREFIX = "dishes/seed"
BUCKET_ID = "69bfa6de000fdacda87d"


def load_config() -> dict:
    with APPSETTINGS.open(encoding="utf-8") as f:
        cfg = json.load(f)
    appwrite = cfg["Appwrite"]
    return {
        "endpoint": appwrite["Endpoint"].rstrip("/"),
        "project_id": appwrite["ProjectId"],
        "api_key": appwrite["ApiKey"],
        "bucket_id": appwrite.get("BucketId", BUCKET_ID),
    }


def to_file_id(object_name: str) -> str:
    digest = hashlib.sha256(object_name.strip().encode("utf-8")).hexdigest()
    return f"f_{digest[:34]}"


def http_json(url: str, headers: dict | None = None, retries: int = 6) -> dict:
    last_err: Exception | None = None
    for attempt in range(retries):
        try:
            req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT, **(headers or {})})
            with urllib.request.urlopen(req, timeout=30) as res:
                return json.loads(res.read().decode("utf-8"))
        except urllib.error.HTTPError as ex:
            last_err = ex
            if ex.code == 429 and attempt + 1 < retries:
                time.sleep(5 * (attempt + 1))
                continue
            raise
    raise last_err  # type: ignore[misc]


def fetch_wikimedia_image_url(search: str) -> str | None:
    time.sleep(3.0)
    for term in (search, " ".join(search.split()[:3]), search.split()[0]):
        if not term:
            continue
        try:
            params = urllib.parse.urlencode(
                {
                    "action": "query",
                    "generator": "search",
                    "gsrnamespace": "6",
                    "gsrsearch": term,
                    "gsrlimit": "5",
                    "prop": "imageinfo",
                    "iiprop": "url|mime",
                    "iiurlwidth": "960",
                    "format": "json",
                }
            )
            data = http_json(f"https://commons.wikimedia.org/w/api.php?{params}")
        except urllib.error.HTTPError as ex:
            if ex.code == 429:
                return None
            raise
        pages = data.get("query", {}).get("pages", {})
        for page in sorted(pages.values(), key=lambda p: p.get("index", 999)):
            infos = page.get("imageinfo") or []
            if not infos:
                continue
            info = infos[0]
            mime = info.get("mime", "")
            if not mime.startswith("image/"):
                continue
            return info.get("thumburl") or info.get("url")
    return None


def download_file(url: str, dest: Path) -> None:
    last_err: Exception | None = None
    for attempt in range(4):
        try:
            time.sleep(1.0)
            req = urllib.request.Request(url, headers={"User-Agent": USER_AGENT})
            with urllib.request.urlopen(req, timeout=60) as res:
                dest.write_bytes(res.read())
            return
        except urllib.error.HTTPError as ex:
            last_err = ex
            if ex.code == 429 and attempt + 1 < 4:
                time.sleep(2 ** attempt)
                continue
            raise
    raise last_err  # type: ignore[misc]


def generate_placeholder(dest: Path, title: str, slug: str) -> None:
    hue = int(hashlib.sha256(slug.encode()).hexdigest()[:6], 16) % 360
    img = Image.new("RGB", (960, 640), color=(40 + hue % 80, 90 + (hue // 3) % 80, 120 + (hue // 5) % 80))
    draw = ImageDraw.Draw(img)
    try:
        font = ImageFont.truetype("arial.ttf", 42)
    except OSError:
        font = ImageFont.load_default()
    draw.text((40, 280), title, fill=(255, 255, 255), font=font)
    buf = BytesIO()
    img.save(buf, format="JPEG", quality=85)
    dest.write_bytes(buf.getvalue())


def upload_to_appwrite(cfg: dict, object_name: str, payload: bytes, content_type: str) -> None:
    import uuid

    boundary = f"----SmartLunch{uuid.uuid4().hex}"
    file_id = to_file_id(object_name)
    filename = Path(object_name).name

    body = bytearray()
    for name, value in [
        ("fileId", file_id),
        ("permissions[]", 'read("any")'),
    ]:
        body.extend(f"--{boundary}\r\n".encode())
        body.extend(f'Content-Disposition: form-data; name="{name}"\r\n\r\n'.encode())
        body.extend(f"{value}\r\n".encode())

    body.extend(f"--{boundary}\r\n".encode())
    body.extend(
        f'Content-Disposition: form-data; name="file"; filename="{filename}"\r\n'.encode()
    )
    body.extend(f"Content-Type: {content_type}\r\n\r\n".encode())
    body.extend(payload)
    body.extend(f"\r\n--{boundary}--\r\n".encode())

    url = f"{cfg['endpoint']}/storage/buckets/{cfg['bucket_id']}/files"
    req = urllib.request.Request(
        url,
        data=bytes(body),
        method="POST",
        headers={
            "User-Agent": USER_AGENT,
            "Content-Type": f"multipart/form-data; boundary={boundary}",
            "X-Appwrite-Project": cfg["project_id"],
            "X-Appwrite-Key": cfg["api_key"],
        },
    )
    try:
        with urllib.request.urlopen(req, timeout=120) as res:
            res.read()
    except urllib.error.HTTPError as ex:
        if ex.code == 409:
            delete_url = f"{cfg['endpoint']}/storage/buckets/{cfg['bucket_id']}/files/{urllib.parse.quote(file_id, safe='')}"
            del_req = urllib.request.Request(
                delete_url,
                method="DELETE",
                headers={
                    "User-Agent": USER_AGENT,
                    "X-Appwrite-Project": cfg["project_id"],
                    "X-Appwrite-Key": cfg["api_key"],
                },
            )
            try:
                urllib.request.urlopen(del_req, timeout=30).read()
            except urllib.error.HTTPError:
                pass
            upload_to_appwrite(cfg, object_name, payload, content_type)
            return
        body_text = ex.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"Upload failed ({ex.code}): {body_text}") from ex


def log(msg: str) -> None:
    try:
        print(msg)
    except UnicodeEncodeError:
        print(msg.encode("ascii", errors="replace").decode("ascii"))


def public_view_url(cfg: dict, object_name: str) -> str:
    file_id = to_file_id(object_name)
    return (
        f"{cfg['endpoint']}/storage/buckets/{cfg['bucket_id']}/files/"
        f"{urllib.parse.quote(file_id, safe='')}/view?project={urllib.parse.quote(cfg['project_id'])}"
    )


def main() -> int:
    if not MANIFEST.exists():
        print(f"Missing manifest: {MANIFEST}", file=sys.stderr)
        return 1

    cfg = load_config()
    ASSETS_DIR.mkdir(parents=True, exist_ok=True)

    with MANIFEST.open(encoding="utf-8") as f:
        dishes = json.load(f)

    results = []
    for item in dishes:
        slug = item["slug"]
        search = item["search"]
        object_name = f"{OBJECT_PREFIX}/{slug}.jpg"
        local_path = ASSETS_DIR / f"{slug}.jpg"

        if not local_path.exists() or local_path.stat().st_size == 0:
            image_url = fetch_wikimedia_image_url(search)
            if image_url:
                log(f"[download] {item['name']} <- {image_url}")
                try:
                    download_file(image_url, local_path)
                except Exception as ex:
                    log(f"[download-fail] {slug}: {ex} -> placeholder")
                    generate_placeholder(local_path, item["name"], slug)
            else:
                log(f"[placeholder] {item['name']} - no Wikimedia match")
                generate_placeholder(local_path, item["name"], slug)

        payload = local_path.read_bytes()
        content_type = "image/jpeg"
        log(f"[upload] {object_name} ({len(payload)} bytes)")
        try:
            upload_to_appwrite(cfg, object_name, payload, content_type)
        except Exception as ex:
            log(f"[upload-fail] {object_name}: {ex}")
            continue
        view_url = public_view_url(cfg, object_name)
        results.append(
            {
                "name": item["name"],
                "slug": slug,
                "objectName": object_name,
                "sizeBytes": len(payload),
                "viewUrl": view_url,
            }
        )

    out = ROOT / "scratch" / "dish_image_upload_results.json"
    out.write_text(json.dumps(results, ensure_ascii=False, indent=2), encoding="utf-8")
    log(f"\nDone: {len(results)}/{len(dishes)} uploaded. Results -> {out}")
    return 0 if results else 1


if __name__ == "__main__":
    raise SystemExit(main())
