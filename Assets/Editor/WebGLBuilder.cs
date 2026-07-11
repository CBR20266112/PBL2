using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuilder
{
    private static readonly string[] Scenes = new[]
    {
        "Assets/Scenes/Title.unity",
        "Assets/Scenes/Main.unity"
    };

    [InitializeOnLoadMethod]
    private static void OnUnityEditorLoad()
    {
        var args = System.Environment.GetCommandLineArgs();
        if (System.Array.Exists(args, arg => arg == "-buildWebGLDocs"))
        {
            File.WriteAllText(Path.Combine(Application.dataPath, "..", "build_trigger.txt"), "triggered");
            BuildWebGLDocs();
        }
    }

    [MenuItem("Build/Build WebGL to Docs")]
    public static void BuildWebGLToDocsMenu()
    {
        BuildWebGLDocs();
    }

    public static void BuildWebGLDocs()
    {
        string outputPath = Path.Combine(Application.dataPath, "..", "docs");
        BuildWebGL(outputPath);
    }

    public static void BuildWebGL(string outputPath)
    {
        File.WriteAllText(Path.Combine(Application.dataPath, "..", "build_debug.txt"), $"BuildWebGL called at {DateTime.Now:O} to {outputPath}");
        outputPath = Path.GetFullPath(outputPath);
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        var report = BuildPipeline.BuildPlayer(Scenes, outputPath, BuildTarget.WebGL, BuildOptions.None);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new System.Exception($"WebGL build failed: {report.summary.result} ({report.summary.totalErrors} errors)");
        }

        File.WriteAllText(Path.Combine(outputPath, ".nojekyll"), string.Empty);
        AssetDatabase.Refresh();

        Debug.Log($"WebGL build completed successfully to '{outputPath}'");
    }
}
