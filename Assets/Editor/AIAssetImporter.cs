#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 10_ART/Cropped 폴더에서 PNG 파일을 Unity 프로젝트의 Assets/Sprites 폴더로
/// 카테고리별로 임포트하고 Sprite 설정을 자동 적용합니다.
///
/// 대상 폴더 구조:
///   Assets/Sprites/
///     Background/
///     Decoration/
///     Effects/
///     Environment/
///     Furniture/
///     Tea/
///     UI/
///     Character/
///       OrangeCat/
///       Customers/
///       NPC/
///       Animals/
///
/// 카테고리 매핑 규칙:
///   파일명 접두사(naming_map.json 기준)를 파싱하여 대상 하위 폴더를 결정합니다.
///   접두사가 일치하지 않으면 Uncategorized/ 로 이동합니다.
/// </summary>
public static class ArtAssetImporter
{
    // -------------------------------------------------------------------------
    // 경로 설정
    // -------------------------------------------------------------------------

    /// <summary>10_ART/Cropped 폴더 (Art Pipeline 출력 결과)</summary>
    private const string SourceRelativePath = "../10_ART/Cropped";

    /// <summary>Unity 프로젝트 내 Sprites 루트 폴더</summary>
    private const string SpritesRoot = "Assets/Sprites";

    /// <summary>분류되지 않은 파일의 임시 대상 폴더</summary>
    private const string UncategorizedFolder = "Assets/Sprites/Uncategorized";

    /// <summary>매니페스트 저장 경로</summary>
    private const string ManifestPath = "Assets/Sprites/sprite_import_manifest.json";

    // -------------------------------------------------------------------------
    // 카테고리 매핑 (파일명 접두사 → Sprites 하위 폴더)
    // naming_map.json 의 카테고리 규칙을 반영
    // -------------------------------------------------------------------------
    private static readonly Dictionary<string, string> PrefixToFolder = new Dictionary<string, string>
    {
        // 배경 / 환경
        { "bg_",          "Background"             },
        { "env_",         "Environment"            },
        { "floor_",       "Environment"            },
        { "wall_",        "Environment"            },
        { "tile_",        "Environment"            },

        // 가구
        { "furniture_",   "Furniture"              },
        { "chair_",       "Furniture"              },
        { "table_",       "Furniture"              },
        { "shelf_",       "Furniture"              },
        { "cabinet_",     "Furniture"              },

        // 차 / 음료 / 재료
        { "tea_",         "Tea"                    },
        { "drink_",       "Tea"                    },
        { "ingredient_",  "Tea"                    },
        { "cup_",         "Tea"                    },
        { "pot_",         "Tea"                    },

        // 장식
        { "deco_",        "Decoration"             },
        { "decoration_",  "Decoration"             },
        { "prop_",        "Decoration"             },
        { "plant_",       "Decoration"             },
        { "lantern_",     "Decoration"             },

        // UI
        { "ui_",          "UI"                     },
        { "icon_",        "UI"                     },
        { "btn_",         "UI"                     },
        { "button_",      "UI"                     },
        { "panel_",       "UI"                     },
        { "frame_",       "UI"                     },
        { "badge_",       "UI"                     },

        // 이펙트
        { "fx_",          "Effects"                },
        { "effect_",      "Effects"                },
        { "particle_",    "Effects"                },

        // 캐릭터 — 주인공 고양이
        { "cat_",         "Character/OrangeCat"    },
        { "player_",      "Character/OrangeCat"    },
        { "orangecat_",   "Character/OrangeCat"    },

        // 캐릭터 — 손님
        { "customer_",    "Character/Customers"    },
        { "guest_",       "Character/Customers"    },

        // 캐릭터 — NPC
        { "npc_",         "Character/NPC"          },

        // 캐릭터 — 동물 (국가 대표 동물)
        { "animal_",      "Character/Animals"      },
        { "lion_",        "Character/Animals"      },
        { "panda_",       "Character/Animals"      },
        { "fox_",         "Character/Animals"      },
        { "buffalo_",     "Character/Animals"      },
        { "marmot_",      "Character/Animals"      },
    };

    // -------------------------------------------------------------------------
    // 메뉴 항목
    // -------------------------------------------------------------------------

    [MenuItem("Tea Cafe/Import Art Assets (10_ART → Sprites)")]
    public static void ImportArtAssets()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string sourceFolder = Path.GetFullPath(Path.Combine(projectRoot, SourceRelativePath));

        if (!Directory.Exists(sourceFolder))
        {
            Debug.LogError($"[ArtAssetImporter] 소스 폴더를 찾을 수 없습니다: {sourceFolder}");
            return;
        }

        // 루트 및 Uncategorized 폴더 보장
        EnsureDirectory(SpritesRoot);
        EnsureDirectory(UncategorizedFolder);

        // 카테고리 폴더 모두 보장
        foreach (var folder in PrefixToFolder.Values)
        {
            EnsureDirectory($"{SpritesRoot}/{folder}");
        }

        string[] sourceFiles = Directory.GetFiles(sourceFolder, "*.png", SearchOption.TopDirectoryOnly);

        if (sourceFiles.Length == 0)
        {
            Debug.LogWarning($"[ArtAssetImporter] PNG 파일 없음: {sourceFolder}");
            return;
        }

        var manifest = LoadManifest();
        int importedCount = 0;
        int skippedCount = 0;

        foreach (string sourceFile in sourceFiles)
        {
            string fileName = Path.GetFileName(sourceFile);
            string targetFolder = ResolveTargetFolder(fileName);
            string destPath = $"{SpritesRoot}/{targetFolder}/{fileName}";

            // 이미 임포트된 파일은 건너뜀 (소스 파일 크기 변경 시 재임포트)
            bool alreadyImported = manifest.Exists(e =>
                e.sourceFileName == fileName &&
                e.targetPath == destPath);

            if (alreadyImported && File.Exists(destPath))
            {
                skippedCount++;
                continue;
            }

            File.Copy(sourceFile, destPath, overwrite: true);
            string assetPath = destPath.Replace("\\", "/");
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            ConfigureSpriteImportSettings(assetPath);

            manifest.RemoveAll(e => e.sourceFileName == fileName); // 중복 제거 후 갱신
            manifest.Add(new ManifestEntry
            {
                sourceFileName = fileName,
                targetPath = destPath,
                category = targetFolder,
                importedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            importedCount++;
            Debug.Log($"[ArtAssetImporter] {fileName} → Sprites/{targetFolder}/");
        }

        SaveManifest(manifest);
        AssetDatabase.Refresh();

        Debug.Log($"[ArtAssetImporter] 완료: 임포트 {importedCount}개, 스킵 {skippedCount}개 (총 {sourceFiles.Length}개)");
        EditorUtility.DisplayDialog(
            "Art Asset Import 완료",
            $"임포트: {importedCount}개\n스킵(이미 임포트됨): {skippedCount}개\n총 파일: {sourceFiles.Length}개\n\n매니페스트: {ManifestPath}",
            "확인"
        );
    }

    // -------------------------------------------------------------------------
    // 내부 유틸리티
    // -------------------------------------------------------------------------

    /// <summary>파일명 접두사로 대상 하위 폴더를 결정합니다.</summary>
    private static string ResolveTargetFolder(string fileName)
    {
        string lower = fileName.ToLowerInvariant();
        foreach (var kv in PrefixToFolder)
        {
            if (lower.StartsWith(kv.Key))
                return kv.Value;
        }
        return "Uncategorized";
    }

    /// <summary>해당 경로가 없으면 폴더를 생성하고 AssetDatabase에 등록합니다.</summary>
    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.ImportAsset(path);
        }
    }

    /// <summary>
    /// Sprite 임포트 설정을 프로젝트 표준에 맞게 적용합니다.
    ///   - Texture Type  : Sprite (2D and UI)
    ///   - Sprite Mode   : Single
    ///   - PPU           : 100
    ///   - Filter Mode   : Bilinear
    ///   - Mipmap        : 비활성화 (2D 게임 표준)
    ///   - Physics Shape : 비활성화 (UI/장식용 에셋)
    ///   - Compression   : Uncompressed (초기값; 빌드 단계에서 플랫폼별 변경 예정)
    /// </summary>
    private static void ConfigureSpriteImportSettings(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null) return;

        importer.textureType        = TextureImporterType.Sprite;
        importer.spriteImportMode   = SpriteImportMode.Single;
        importer.spritePivot        = new Vector2(0.5f, 0.5f);
        importer.spritePixelsPerUnit = 100f;
        importer.filterMode         = FilterMode.Bilinear;
        importer.mipmapEnabled      = false;
        importer.alphaSource        = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        // Physics Shape 생성 비활성화 (불필요한 연산 방지)
        importer.spriteGenerateFallbackPhysicsShape = false;

        importer.SaveAndReimport();
    }

    // -------------------------------------------------------------------------
    // 매니페스트 (JSON) 관련
    // -------------------------------------------------------------------------

    private static List<ManifestEntry> LoadManifest()
    {
        if (!File.Exists(ManifestPath))
            return new List<ManifestEntry>();

        string json = File.ReadAllText(ManifestPath);
        ManifestRoot root = JsonUtility.FromJson<ManifestRoot>(json);
        return root?.assets != null
            ? new List<ManifestEntry>(root.assets)
            : new List<ManifestEntry>();
    }

    private static void SaveManifest(List<ManifestEntry> entries)
    {
        string json = JsonUtility.ToJson(new ManifestRoot { assets = entries.ToArray() }, prettyPrint: true);
        File.WriteAllText(ManifestPath, json);
        AssetDatabase.ImportAsset(ManifestPath, ImportAssetOptions.ForceUpdate);
        Debug.Log($"[ArtAssetImporter] 매니페스트 저장: {ManifestPath}");
    }

    // -------------------------------------------------------------------------
    // 직렬화 클래스
    // -------------------------------------------------------------------------

    [System.Serializable]
    private class ManifestRoot
    {
        public ManifestEntry[] assets;
    }

    [System.Serializable]
    private class ManifestEntry
    {
        public string sourceFileName;
        public string targetPath;
        public string category;
        public string importedAt;
    }
}
#endif