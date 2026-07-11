#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 10_ART/conceptArt 폴더에서 PNG/JPG 파일을 Unity 프로젝트의 Resources/Art 폴더로 복사하고 스프라이트로 설정합니다.
/// </summary>
public static class ArtAssetImporter
{
    private const string SourceRelativePath = "../10_ART/conceptArt";
    private const string DestinationPath = "Assets/Resources/Art";
    private const string ManifestFileName = "art_design_manifest.json";

    private static readonly Dictionary<string, string> ExplicitRenameMap = new Dictionary<string, string>
    {
        // Add known source filename mappings here if you want stable stable names for specific concept sheets.
    };

    [MenuItem("Art/Import 10_ART Concept Art")]
    public static void ImportArtDesignAssets()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string sourceFolder = Path.GetFullPath(Path.Combine(projectRoot, SourceRelativePath));

        if (!Directory.Exists(sourceFolder))
        {
            Debug.LogError($"ArtAssetImporter: Source folder not found: {sourceFolder}");
            return;
        }

        if (!Directory.Exists(DestinationPath))
        {
            Directory.CreateDirectory(DestinationPath);
            AssetDatabase.ImportAsset(DestinationPath);
        }

        string[] sourceFiles = Directory.GetFiles(sourceFolder, "*.png", SearchOption.TopDirectoryOnly);
        sourceFiles = Combine(sourceFiles, Directory.GetFiles(sourceFolder, "*.jpg", SearchOption.TopDirectoryOnly));

        if (sourceFiles.Length == 0)
        {
            Debug.LogWarning($"ArtAssetImporter: No PNG/JPG files found in {sourceFolder}");
            return;
        }

        var manifestEntries = LoadManifest();
        int fileIndex = 1;

        foreach (string sourceFile in sourceFiles)
        {
            string sourceFileName = Path.GetFileName(sourceFile);
            string importedFileName = ResolveImportedFileName(sourceFileName, fileIndex);
            fileIndex++;

            if (manifestEntries.Exists(entry => entry.sourceFileName == sourceFileName && entry.importedFileName == importedFileName))
            {
                Debug.Log($"ArtAssetImporter: Skipping already imported source file {sourceFileName}");
                continue;
            }

            string destFile = Path.Combine(DestinationPath, importedFileName);
            File.Copy(sourceFile, destFile, true);
            Debug.Log($"ArtAssetImporter: Copied {sourceFileName} to {destFile}");

            string assetPath = destFile.Replace("\\", "/");
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            ConfigureTextureImporter(assetPath);

            manifestEntries.Add(new ManifestEntry
            {
                sourceFileName = sourceFileName,
                importedFileName = importedFileName,
                category = "Uncategorized",
                notes = string.Empty
            });
        }

        SaveManifest(manifestEntries);
        AssetDatabase.Refresh();
        Debug.Log($"ArtAssetImporter: Imported {sourceFiles.Length} concept art asset(s) from 10_ART.");
    }

    private static string[] Combine(string[] first, string[] second)
    {
        var combined = new string[first.Length + second.Length];
        Array.Copy(first, combined, first.Length);
        Array.Copy(second, 0, combined, first.Length, second.Length);
        return combined;
    }

    private static string ResolveImportedFileName(string sourceFileName, int index)
    {
        if (ExplicitRenameMap.TryGetValue(sourceFileName, out string mappedName))
        {
            return mappedName;
        }

        string extension = Path.GetExtension(sourceFileName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFileName);

        if (Guid.TryParse(fileNameWithoutExtension, out _))
        {
            return $"Art_Concept_{index:D2}{extension}";
        }

        return sourceFileName;
    }

    private static List<ManifestEntry> LoadManifest()
    {
        string manifestPath = Path.Combine(DestinationPath, ManifestFileName);
        if (!File.Exists(manifestPath))
        {
            return new List<ManifestEntry>();
        }

        string manifestJson = File.ReadAllText(manifestPath);
        var manifestRoot = JsonUtility.FromJson<ManifestRoot>(manifestJson);
        return manifestRoot?.assets != null ? new List<ManifestEntry>(manifestRoot.assets) : new List<ManifestEntry>();
    }

    private static void SaveManifest(List<ManifestEntry> manifestEntries)
    {
        string manifestPath = Path.Combine(DestinationPath, ManifestFileName);
        string manifestJson = JsonUtility.ToJson(new ManifestRoot { assets = manifestEntries.ToArray() }, true);
        File.WriteAllText(manifestPath, manifestJson);
        AssetDatabase.ImportAsset(manifestPath, ImportAssetOptions.ForceUpdate);
        Debug.Log($"ArtAssetImporter: Manifest saved to {manifestPath}");
    }

    [System.Serializable]
    private class ManifestRoot
    {
        public ManifestEntry[] assets;
    }

    [System.Serializable]
    private class ManifestEntry
    {
        public string sourceFileName;
        public string importedFileName;
        public string category;
        public string notes;
    }

    private static void ConfigureTextureImporter(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        importer.SaveAndReimport();
        Debug.Log($"ArtAssetImporter: Configured sprite import settings for {assetPath}");
    }
}
#endif