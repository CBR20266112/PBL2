import os
import argparse
import glob
import hashlib
import json
import time
from datetime import datetime

try:
    import numpy as np
    import cv2
    from PIL import Image
except ImportError:
    print("필수 라이브러리가 설치되어 있지 않습니다. 설치 명령어:")
    print("pip install pillow numpy opencv-python")
    exit(1)

# 처리에서 제외할 캐릭터 시트 파일명(또는 키워드/UUID 접두사) 목록
SKIP_FILES = [
    "6ec69027", "714a876a", "c2bd230b", "d1f644d5", "ee20bee0", 
    "character", "char_"
]

def should_skip_file(filename):
    lower_name = filename.lower()
    for skip_key in SKIP_FILES:
        if skip_key.lower() in lower_name:
            return True
    return False

def get_image_hash(image):
    return hashlib.md5(image.tobytes()).hexdigest()

def load_naming_map(map_path):
    if os.path.exists(map_path):
        try:
            with open(map_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            print(f"Warning: {map_path} 읽기 실패. 기본 명명 규칙을 사용합니다. ({e})")
    return {}

def log_message(log_path, message, to_console=True, dry_run=False):
    if to_console:
        print(message)
    log_dir = os.path.dirname(log_path)
    if not os.path.exists(log_dir) and log_dir != '':
        os.makedirs(log_dir, exist_ok=True)
    with open(log_path, 'a', encoding='utf-8') as f:
        f.write(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] {message}\n")

def skip_log(skipped_path, filename, reason):
    log_dir = os.path.dirname(skipped_path)
    if not os.path.exists(log_dir) and log_dir != '':
        os.makedirs(log_dir, exist_ok=True)
    with open(skipped_path, 'a', encoding='utf-8') as f:
        f.write(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] File: {filename} | Reason: {reason}\n")

def get_category_from_name(asset_name):
    prefix = asset_name.split("_")[0]
    category_map = {
        "UI": "UI",
        "Furniture": "Furniture",
        "Decoration": "Decoration",
        "TeaWare": "Tea",
        "Tea": "Tea",
        "TeaDrink": "Tea",
        "Ingredient": "Tea",
        "Prop": "Tea",
        "Background": "Background",
        "Environment": "Environment",
        "Floor": "Environment",
        "Wall": "Environment",
        "Window": "Environment",
        "Door": "Environment",
        "Structure": "Environment",
        "Shelf": "Furniture",
        "Effect": "Effects",
        "Animation": "Effects",
        "Anim": "Effects"
    }
    return category_map.get(prefix, "Uncategorized")

def process_image(img_path, output_base_dir, naming_map, seen_hashes, args, log_path, skipped_path, stats):
    filename = os.path.basename(img_path)
    base_name = os.path.splitext(filename)[0]
    
    if should_skip_file(filename):
        log_message(log_path, f"SKIPPED: {filename} (캐릭터 시트 등 제외 대상)", to_console=True)
        stats["skipped_png"] += 1
        return
        
    try:
        img = Image.open(img_path).convert("RGBA")
    except Exception as e:
        log_message(log_path, f"ERROR: 이미지를 열 수 없습니다. {filename} - {e}")
        stats["errors"] += 1
        return

    img_w, img_h = img.size
    
    np_img = np.array(img)
    alpha_channel = np_img[:, :, 3]
    
    # 2. 알파 채널 유무에 따른 마스크 생성 분기
    if np.min(alpha_channel) < 255:
        # 투명 배경(Alpha) 기반 추출
        _, mask = cv2.threshold(alpha_channel, 10, 255, cv2.THRESH_BINARY)
        kernel = np.ones((3,3), np.uint8)
        mask_opened = cv2.morphologyEx(mask, cv2.MORPH_OPEN, kernel, iterations=1)
        contours, _ = cv2.findContours(mask_opened, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    else:
        # 불투명(솔리드) 배경인 경우 Canny Edge 기반 추출
        gray = cv2.cvtColor(np_img, cv2.COLOR_RGBA2GRAY)
        blurred = cv2.GaussianBlur(gray, (5, 5), 0)
        edges = cv2.Canny(blurred, 30, 100)
        kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (15, 15))
        closed = cv2.morphologyEx(edges, cv2.MORPH_CLOSE, kernel)
        contours, _ = cv2.findContours(closed, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    
    log_message(log_path, f"\n=== Processing {filename} ({img_w}x{img_h}) - Found {len(contours)} contours ===")
    
    valid_count = 0
    padding = args.padding
    
    preview_img_cv = None
    if args.dry_run:
        preview_img_cv = cv2.cvtColor(np_img, cv2.COLOR_RGBA2BGRA)

    for i, contour in enumerate(contours):
        x, y, w, h = cv2.boundingRect(contour)
        
        if w < args.min_size or h < args.min_size:
            skip_log(skipped_path, filename, f"Too small (Contour {i}: {w}x{h})")
            stats["too_small"] += 1
            continue
            
        x1 = max(0, x - padding)
        y1 = max(0, y - padding)
        x2 = min(img_w, x + w + padding)
        y2 = min(img_h, y + h + padding)
        
        cropped_img = img.crop((x1, y1, x2, y2))
        
        img_hash = get_image_hash(cropped_img)
        if img_hash in seen_hashes:
            skip_log(skipped_path, filename, f"Duplicate asset detected (Contour {i})")
            stats["duplicates"] += 1
            continue
            
        seen_hashes.add(img_hash)
        valid_count += 1
        
        # 7. 파일명 및 카테고리(출력 폴더) 결정
        file_map = naming_map.get(filename, {})
        final_name = file_map.get(str(valid_count), f"Category_Unknown_{valid_count:02d}.png")
        if not final_name.endswith('.png'):
            final_name += '.png'
            
        if args.dry_run:
            cv2.rectangle(preview_img_cv, (x1, y1), (x2, y2), (0, 0, 255, 255), 2)
            log_message(log_path, f"[DRY-RUN] Would save: {final_name} ({x2-x1}x{y2-y1})", to_console=False)
        else:
            category_folder = get_category_from_name(final_name)
            out_dir = os.path.join(output_base_dir, category_folder)
            out_path = os.path.join(out_dir, final_name)
            
            if os.path.exists(out_path):
                log_message(log_path, f"[SKIP] Already exists: {out_path}", to_console=False)
                continue
                
            if not os.path.exists(out_dir):
                os.makedirs(out_dir, exist_ok=True)
            cropped_img.save(out_path, format="PNG")
            log_message(log_path, f"[SAVED] {out_path} ({x2-x1}x{y2-y1})", to_console=False)
            
    if args.dry_run and preview_img_cv is not None:
        preview_dir = os.path.join(output_base_dir, "Preview")
        if not os.path.exists(preview_dir):
            os.makedirs(preview_dir, exist_ok=True)
        preview_path = os.path.join(preview_dir, f"{base_name}_preview.png")
        cv2.imwrite(preview_path, preview_img_cv)
        log_message(log_path, f"[DRY-RUN] Preview saved: {preview_path}")

    stats["processed_png"] += 1
    stats["generated_assets"] += valid_count
    log_message(log_path, f"Finished {filename}: Extracted {valid_count} assets.")

def main():
    start_time = time.time()
    
    parser = argparse.ArgumentParser(description="Auto Crop transparent sprites for Tea Cafe Project.")
    parser.add_argument("--input", type=str, default="conceptArt", help="Input directory containing raw PNGs")
    parser.add_argument("--output", type=str, default="Cropped", help="Output directory for cropped assets")
    parser.add_argument("--min-size", type=int, default=15, help="Minimum width/height of asset (to remove noise)")
    parser.add_argument("--padding", type=int, default=2, help="Padding around cropped asset (pixels)")
    parser.add_argument("--dry-run", action="store_true", help="Run without saving files or creating asset folders")
    parser.add_argument("--map", type=str, default="naming_map.json", help="Path to JSON naming map")
    parser.add_argument("--file", type=str, help="Process a single specific PNG file name (e.g. 7fc5059d...png)")
    
    args = parser.parse_args()
    
    input_dir = os.path.abspath(args.input)
    output_dir = os.path.abspath(args.output)
    logs_dir = os.path.join(output_dir, "logs")
    
    if not os.path.exists(logs_dir):
        os.makedirs(logs_dir, exist_ok=True)
        
    log_path = os.path.join(logs_dir, "log.txt")
    skipped_path = os.path.join(logs_dir, "skipped.txt")
    
    stats = {
        "processed_png": 0,
        "skipped_png": 0,
        "generated_assets": 0,
        "duplicates": 0,
        "too_small": 0,
        "errors": 0
    }
    
    if args.dry_run:
        print("!!! DRY RUN MODE ACTIVE - No asset files or folders will be created !!!\n")
        
    log_message(log_path, "Starting Sprite Auto-Crop Script...", dry_run=args.dry_run)
    
    naming_map = load_naming_map(args.map)
    seen_hashes = set()
    
    png_files = glob.glob(os.path.join(input_dir, "*.png"))
    
    if args.file:
        png_files = [f for f in png_files if os.path.basename(f) == args.file]
        
    if not png_files:
        log_message(log_path, f"No PNG files found to process.", dry_run=args.dry_run)
        return

    for img_path in sorted(png_files):
        process_image(img_path, output_dir, naming_map, seen_hashes, args, log_path, skipped_path, stats)
        
    elapsed = time.time() - start_time
    
    # Save summary.json
    summary_path = os.path.join(logs_dir, "summary.json")
    with open(summary_path, 'w', encoding='utf-8') as f:
        json.dump(stats, f, indent=4)
        
    log_message(log_path, "Auto-Crop Process Completed.", dry_run=args.dry_run)
    
    # Final Summary Console Output
    print("\n===== SUMMARY =====")
    print(f"Processed PNG    : {stats['processed_png']}")
    print(f"Generated Assets : {stats['generated_assets']}")
    print(f"Skipped          : {stats['skipped_png']}")
    print(f"Duplicates       : {stats['duplicates']}")
    print(f"Too Small        : {stats['too_small']}")
    print(f"Errors           : {stats['errors']}")
    print(f"Elapsed Time     : {elapsed:.2f} seconds")
    print("===================")

if __name__ == "__main__":
    main()
