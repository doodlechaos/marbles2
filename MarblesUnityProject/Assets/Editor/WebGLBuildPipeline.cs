using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom WebGL build pipeline that exports directly to the SvelteKit static folder
/// and automatically removes the Unity-generated index.html to avoid routing conflicts.
/// </summary>
public static class WebGLBuildPipeline
{
    // Output path relative to the Unity project root
    private const string RelativeOutputPath = "../apps/web/marbles-web-client/static";

    [MenuItem("Build/Build WebGL for SvelteKit %#w")]
    public static void BuildWebGL()
    {
        // Get absolute output path
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string outputPath = Path.GetFullPath(Path.Combine(projectRoot, RelativeOutputPath));

        Debug.Log($"[WebGLBuildPipeline] Building WebGL to: {outputPath}");

        // Get scenes from build settings
        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("[WebGLBuildPipeline] No scenes found in build settings!");
            return;
        }

        // Configure build options
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        // Perform the build
        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[WebGLBuildPipeline] Build succeeded! Output: {outputPath}");

            // Clean up the index.html that Unity generates
            CleanupIndexHtml(outputPath);

            Debug.Log("[WebGLBuildPipeline] Build complete and cleaned up!");
        }
        else
        {
            Debug.LogError(
                $"[WebGLBuildPipeline] Build failed with {report.summary.totalErrors} error(s)"
            );
        }
    }

    [MenuItem("Build/Build WebGL for SvelteKit (Development) %#&w")]
    public static void BuildWebGLDevelopment()
    {
        // Get absolute output path
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string outputPath = Path.GetFullPath(Path.Combine(projectRoot, RelativeOutputPath));

        Debug.Log($"[WebGLBuildPipeline] Building WebGL (Development) to: {outputPath}");

        // Get scenes from build settings
        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("[WebGLBuildPipeline] No scenes found in build settings!");
            return;
        }

        // Configure build options with development flag
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.Development | BuildOptions.ConnectWithProfiler,
        };

        // Perform the build
        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[WebGLBuildPipeline] Development build succeeded! Output: {outputPath}");

            // Clean up the index.html that Unity generates
            CleanupIndexHtml(outputPath);

            Debug.Log("[WebGLBuildPipeline] Development build complete and cleaned up!");
        }
        else
        {
            Debug.LogError(
                $"[WebGLBuildPipeline] Development build failed with {report.summary.totalErrors} error(s)"
            );
        }
    }

    /// <summary>
    /// Removes the Unity-generated index.html file that conflicts with SvelteKit routing.
    /// </summary>
    private static void CleanupIndexHtml(string outputPath)
    {
        string indexHtmlPath = Path.Combine(outputPath, "index.html");

        if (File.Exists(indexHtmlPath))
        {
            File.Delete(indexHtmlPath);
            Debug.Log($"[WebGLBuildPipeline] Deleted: {indexHtmlPath}");
        }
        else
        {
            Debug.LogWarning($"[WebGLBuildPipeline] index.html not found at: {indexHtmlPath}");
        }
    }

    /// <summary>
    /// Gets all enabled scenes from the build settings.
    /// </summary>
    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && !string.IsNullOrEmpty(scene.path))
            {
                scenes.Add(scene.path);
            }
        }

        return scenes.ToArray();
    }

    /// <summary>
    /// Opens the output folder in the file explorer.
    /// </summary>
    [MenuItem("Build/Open WebGL Output Folder")]
    public static void OpenOutputFolder()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string outputPath = Path.GetFullPath(Path.Combine(projectRoot, RelativeOutputPath));

        if (Directory.Exists(outputPath))
        {
            EditorUtility.RevealInFinder(outputPath);
        }
        else
        {
            Debug.LogWarning($"[WebGLBuildPipeline] Output folder does not exist: {outputPath}");
        }
    }
}
