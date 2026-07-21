#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 10_ART/Cropped 폴더(카테고리별 서브폴더)에서 PNG를
/// Assets/Sprites 대응 폴더로 복사하고 Sprite Import 설정을 적용합니다.
///
/// 소스 → 대상 폴더 매핑:
///   10_ART/Cropped/Background/   → Assets/Sprites/Background/
///   10_ART/Cropped/Decoration/   → Assets/Sprites/Decoration/
///   10_ART/Cropped/Effects/      → Assets/Sprites/Effects/
///   10_ART/Cropped/Environment/  → Assets/Sprites/Environment/
///   10_ART/Cropped/Furniture/    → Assets/Sprites/Furniture/
///   10_ART/Cropped/Tea/          → Assets/Sprites/Tea/
///   10_ART/Cropped/UI/           → Assets/Sprites/UI/
///   10_ART/Cropped/Uncategorized/→ Assets/Sprites/Uncategorized/
///   10_ART/Cropped/*.png (루트)  → Assets/Sprites/Uncategorized/
///
/// Character 에셋은 별도 작업(10_ART/character → Assets/Sprites/Character/)에서 처리합니다.
/// </summary>
public static class ArtAssetImporter
{
    // -------------------------------------------------------------------------
    // 경로 설정
    // -------------------------------------------------------------------------
    private const string CroppedRoot     = "../10_ART/Cropped";
    private const string SpritesRoot     = "Assets/Sprites";
    private const string ManifestPath    = "Assets/Sprites/sprite_import_manifest.json";

    /// <summary>Cropped 서브폴더명 → Assets/Sprites 서브폴더명 매핑</summary>
    private static readonly Dictionary<string, string> FolderMap = new Dictionary<string, string>
    {
        { "Background",    "Background"    },
        { "Decoration",    "Decoration"    },
        { "Effects",       "Effects"       },
        { "Environment",   "Environment"   },
        { "Furniture",     "Furniture"     },
        { "Tea",           "Tea"           },
        { "UI",            "UI"            },
        { "Uncategorized", "Uncategorized" },
    };

    // -------------------------------------------------------------------------
    // 메뉴 항목 — Art > Import Cropped Assets
    // -------------------------------------------------------------------------
    [MenuItem("Tea Cafe/Import Art Assets (10_ART/Cropped → Sprites)")]
    public static void ImportCroppedAssets()
    {
        string projectRoot   = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string croppedFolder = Path.GetFullPath(Path.Combine(projectRoot, CroppedRoot));

        if (!Directory.Exists(croppedFolder))
        {
            Debug.LogError($"[ArtAssetImporter] 소스 폴더를 찾을 수 없습니다: {croppedFolder}");
            return;
        }

        EnsureDirectory(SpritesRoot);

        var manifest      = LoadManifest();
        int importedCount = 0;
        int skippedCount  = 0;

        // ── 1. 카테고리 서브폴더 처리 ──────────────────────────────────────
        foreach (var kv in FolderMap)
        {
            string srcFolder  = Path.Combine(croppedFolder, kv.Key);
            string destFolder = $"{SpritesRoot}/{kv.Value}";

            if (!Directory.Exists(srcFolder)) continue;

            EnsureDirectory(destFolder);

            foreach (string srcFile in Directory.GetFiles(srcFolder, "*.png", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(srcFile);
                string destPath = $"{destFolder}/{fileName}";

                if (IsAlreadyImported(manifest, fileName, destPath) && File.Exists(destPath))
                {
                    skippedCount++;
                    continue;
                }

                CopyAndImport(srcFile, destPath, kv.Value, manifest);
                importedCount++;
            }
        }

        // ── 2. Cropped/Uncategorized 폴더 → 파일명 접두사 기반 재분류 ───────
        // Effects_, Furniture_ 등의 접두사가 있으면 올바른 카테고리 폴더로 라우팅.
        // 접두사 매핑이 없는 파일만 Uncategorized 로 보냄.
        string uncatSrc  = Path.Combine(croppedFolder, "Uncategorized");
        string uncatDest = $"{SpritesRoot}/Uncategorized";
        EnsureDirectory(uncatDest);

        if (Directory.Exists(uncatSrc))
        {
            foreach (string srcFile in Directory.GetFiles(uncatSrc, "*.png", SearchOption.TopDirectoryOnly))
            {
                string fileName   = Path.GetFileName(srcFile);
                string category   = ResolveDestinationFolder(fileName);   // 접두사 기반 분류
                string targetDir  = $"{SpritesRoot}/{category}";
                string destPath   = $"{targetDir}/{fileName}";

                EnsureDirectory(targetDir);

                if (IsAlreadyImported(manifest, fileName, destPath) && File.Exists(destPath))
                {
                    skippedCount++;
                    continue;
                }

                CopyAndImport(srcFile, destPath, category, manifest);
                importedCount++;
            }
        }

        // ── 3. Cropped 루트 직속 PNG → 파일명 접두사 기반 재분류 ─────────────
        foreach (string srcFile in Directory.GetFiles(croppedFolder, "*.png", SearchOption.TopDirectoryOnly))
        {
            string fileName  = Path.GetFileName(srcFile);
            string category  = ResolveDestinationFolder(fileName);
            string targetDir = $"{SpritesRoot}/{category}";
            string destPath  = $"{targetDir}/{fileName}";

            EnsureDirectory(targetDir);

            if (IsAlreadyImported(manifest, fileName, destPath) && File.Exists(destPath))
            {
                skippedCount++;
                continue;
            }

            CopyAndImport(srcFile, destPath, category, manifest);
            importedCount++;
        }

        SaveManifest(manifest);
        AssetDatabase.Refresh();

        string summary = $"임포트: {importedCount}개  /  스킵: {skippedCount}개  /  총: {importedCount + skippedCount}개";
        Debug.Log($"[ArtAssetImporter] 완료 — {summary}");
        EditorUtility.DisplayDialog("Art Asset Import 완료", summary + $"\n\n매니페스트: {ManifestPath}", "확인");
    }

    // -------------------------------------------------------------------------
    // 내부 유틸리티
    // -------------------------------------------------------------------------

    private static void CopyAndImport(string srcFile, string destPath, string category, List<ManifestEntry> manifest)
    {
        string fileName = Path.GetFileName(srcFile);
        File.Copy(srcFile, destPath, overwrite: true);

        string assetPath = destPath.Replace("\\", "/");
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        ConfigureSpriteImportSettings(assetPath);

        manifest.RemoveAll(e => e.sourceFileName == fileName);
        manifest.Add(new ManifestEntry
        {
            sourceFileName = fileName,
            targetPath     = destPath,
            category       = category,
            importedAt     = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });

        Debug.Log($"[ArtAssetImporter] {category}/{fileName}");
    }

    private static bool IsAlreadyImported(List<ManifestEntry> manifest, string fileName, string destPath)
        => manifest.Exists(e => e.sourceFileName == fileName && e.targetPath == destPath);

    /// <summary>
    /// 파일명 접두사를 분석하여 Assets/Sprites 하위 대상 폴더명을 반환합니다.
    /// 일치하는 접두사가 없으면 "Uncategorized" 를 반환합니다.
    /// </summary>
    private static string ResolveDestinationFolder(string fileName)
    {
        string lower = fileName.ToLowerInvariant();

        // Effects 계열 (가장 먼저 체크 — 이전 누락 원인 해소)
        if (lower.StartsWith("effects_")) return "Effects";

        // 배경 / 환경
        if (lower.StartsWith("background_")) return "Background";
        if (lower.StartsWith("environment_") || lower.StartsWith("floor_") ||
            lower.StartsWith("wall_")         || lower.StartsWith("window_") ||
            lower.StartsWith("door_")         || lower.StartsWith("structure_") ||
            lower.StartsWith("base_"))         return "Environment";

        // 가구
        if (lower.StartsWith("furniture_") || lower.StartsWith("shelf_")) return "Furniture";

        // 장식
        if (lower.StartsWith("decoration_")) return "Decoration";

        // 차 / 음료 / 재료
        if (lower.StartsWith("tea_")        || lower.StartsWith("teaware_") ||
            lower.StartsWith("teadrink_")   || lower.StartsWith("ingredient_") ||
            lower.StartsWith("prop_")       || lower.StartsWith("animation_")) return "Tea";

        // UI
        if (lower.StartsWith("ui_")) return "UI";

        // 캐릭터
        if (lower.StartsWith("customer_") || lower.StartsWith("cat_") ||
            lower.StartsWith("player_")   || lower.StartsWith("npc_") ||
            lower.StartsWith("animal_"))   return "Character/Customers";

        return "Uncategorized";
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.ImportAsset(path);
        }
    }

    /// <summary>
    /// Sprite 임포트 설정 (프로젝트 표준)
    ///   - TextureType  : Sprite (2D and UI)
    ///   - SpriteMode   : Single
    ///   - PPU          : 100
    ///   - FilterMode   : Bilinear
    ///   - Mipmap       : OFF  (2D 게임 표준 — 메모리 절약)
    ///   - PhysicsShape : OFF  (UI/장식용 에셋은 물리 불필요)
    ///   - Compression  : Uncompressed (개발 단계; 빌드 시 플랫폼별 변경)
    /// </summary>
    private static void ConfigureSpriteImportSettings(string assetPath)
    {
        if (!(AssetImporter.GetAtPath(assetPath) is TextureImporter importer)) return;

        importer.textureType                          = TextureImporterType.Sprite;
        importer.spriteImportMode                     = SpriteImportMode.Single;
        importer.spritePivot                          = new Vector2(0.5f, 0.5f);
        importer.spritePixelsPerUnit                  = 100f;
        importer.filterMode                           = FilterMode.Bilinear;
        importer.mipmapEnabled                        = false;
        importer.alphaSource                          = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency                  = true;
        importer.textureCompression                   = TextureImporterCompression.Uncompressed;

        importer.SaveAndReimport();
    }

    /// <summary>
    /// 손님 캐릭터 정보를 담은 CustomerData ScriptableObject를 디스크에 자동 생성하고
    /// 임포트된 스프라이트를 자동으로 할당합니다.
    /// </summary>
    [MenuItem("Tea Cafe/Create Customer ScriptableObjects")]
    public static void CreateCustomerAssets()
    {
        string folderPath = "Assets/Resources/ScriptableObjects/Customers";
        EnsureDirectory(folderPath);

        // 5가지 기본 손님 정보와 스프라이트 파일명 매핑
        var customersInfo = new[]
        {
            new { name = "Luna",    type = "luna",    tea = "yuzu",   temp = 0, steep = 0, personality = "수줍음, 예민함", story = "과제 때문에 밤 늦게 방문",       sprite = "customer_6ec69027" },
            new { name = "Hyuntae", type = "hyuntae", tea = "chai",   temp = 2, steep = 2, personality = "활발함, 자신감", story = "회의 사이 잠깐 방문",           sprite = "customer_714a876a" },
            new { name = "Wei",     type = "wei",     tea = "puerh",  temp = 1, steep = 2, personality = "차분함, 건강 관심", story = "건강식 영양사",               sprite = "customer_c2bd230b" },
            new { name = "Sakura",  type = "sakura",  tea = "matcha", temp = 1, steep = 0, personality = "활기찬, 사교적", story = "SNS 크리에이터, 사진 촬영", sprite = "customer_d1f644d5" },
            new { name = "Denu",    type = "denu",    tea = "chai",   temp = 2, steep = 2, personality = "침착함, 지혜로움", story = "은퇴한 차 수집가, 신비로움",     sprite = "customer_ee20bee0" }
        };

        foreach (var info in customersInfo)
        {
            string assetPath = $"{folderPath}/Customer_{info.name}.asset";
            CustomerData data = AssetDatabase.LoadAssetAtPath<CustomerData>(assetPath);
            bool isNew = false;

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<CustomerData>();
                isNew = true;
            }

            data.customerName = info.name;
            data.characterType = info.type;
            data.preferredTea = info.tea;
            data.preferredTemperature = info.temp;
            data.preferredSteepTime = info.steep;
            data.personality = info.personality;
            data.storyBit = info.story;

            // 스프라이트 로드 및 연결
            string spritePath = $"Assets/Sprites/Character/Customers/{info.sprite}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
            {
                data.characterSprite = sprite;
            }
            else
            {
                Debug.LogWarning($"[ArtAssetImporter] 스프라이트를 찾을 수 없습니다: {spritePath}");
            }

            if (isNew)
            {
                AssetDatabase.CreateAsset(data, assetPath);
                Debug.Log($"[ArtAssetImporter] 생성됨: {assetPath}");
            }
            else
            {
                EditorUtility.SetDirty(data);
                Debug.Log($"[ArtAssetImporter] 갱신됨: {assetPath}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ArtAssetImporter] 모든 손님 ScriptableObject 생성/업데이트 완료.");
    }

    // -------------------------------------------------------------------------
    // 매니페스트 (JSON)
    // -------------------------------------------------------------------------

    private static List<ManifestEntry> LoadManifest()
    {
        if (!File.Exists(ManifestPath)) return new List<ManifestEntry>();
        var root = JsonUtility.FromJson<ManifestRoot>(File.ReadAllText(ManifestPath));
        return root?.assets != null ? new List<ManifestEntry>(root.assets) : new List<ManifestEntry>();
    }

    private static void SaveManifest(List<ManifestEntry> entries)
    {
        string json = JsonUtility.ToJson(new ManifestRoot { assets = entries.ToArray() }, prettyPrint: true);
        File.WriteAllText(ManifestPath, json);
        AssetDatabase.ImportAsset(ManifestPath, ImportAssetOptions.ForceUpdate);
    }

    [System.Serializable] private class ManifestRoot   { public ManifestEntry[] assets; }

    [System.Serializable]
    private class ManifestEntry
    {
        public string sourceFileName;
        public string targetPath;
        public string category;
        public string importedAt;
    }
}

/// <summary>
/// Assets/Sprites/ 폴더에 추가되는 모든 텍스처를 자동으로 Sprite 표준 설정으로 임포트하는 포스트프로세서.
/// </summary>
public class ArtAssetPostprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        // Assets/Sprites/ 하위에 있는 이미지만 대상으로 필터링
        if (assetPath.StartsWith("Assets/Sprites/", StringComparison.OrdinalIgnoreCase))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            if (importer == null) return;

            importer.textureType                          = TextureImporterType.Sprite;
            importer.spriteImportMode                     = SpriteImportMode.Single;
            importer.spritePivot                          = new Vector2(0.5f, 0.5f);
            importer.spritePixelsPerUnit                  = 100f;
            importer.filterMode                           = FilterMode.Bilinear;
            importer.mipmapEnabled                        = false;
            importer.alphaSource                          = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency                  = true;
            importer.textureCompression                   = TextureImporterCompression.Uncompressed;
        }
    }
}

/// <summary>
/// 에디터 컴파일 완료 시 손님 ScriptableObject 데이터를 자동으로 최초 1회 생성해 주는 클래스.
/// </summary>
[InitializeOnLoad]
public static class CustomerAssetAutoGenerator
{
    static CustomerAssetAutoGenerator()
    {
        string folderPath = "Assets/Resources/ScriptableObjects/Customers";
        string checkPath = $"{folderPath}/Customer_Luna.asset";
        
        if (!File.Exists(checkPath))
        {
            EditorApplication.delayCall += () => {
                ArtAssetImporter.CreateCustomerAssets();
            };
        }
    }
}
#endif