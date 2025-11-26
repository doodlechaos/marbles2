using System.IO;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

/// <summary>
/// Custom WebGL build pipeline that exports directly to the SvelteKit static folder
/// and automatically removes the Unity-generated index.html to avoid routing conflicts.
/// Uses settings from the Web.asset Build Profile.
/// </summary>
public static class WebGLBuildPipeline
{
    // Output path relative to the Unity project root
    private const string RelativeOutputPath = "../apps/web/marbles-web-client/static/unity-webgl";

    // Path to the Web Build Profile asset
    private const string BuildProfilePath = "Assets/Settings/Build Profiles/Web.asset";

    [MenuItem("Build/Build WebGL for SvelteKit %#w")]
    public static void BuildWebGL()
    {
        BuildWithProfile(development: false);
    }

    [MenuItem("Build/Build WebGL for SvelteKit (Development) %#&w")]
    public static void BuildWebGLDevelopment()
    {
        BuildWithProfile(development: true);
    }

    private static void BuildWithProfile(bool development)
    {
        // Load the Build Profile asset
        BuildProfile buildProfile = AssetDatabase.LoadAssetAtPath<BuildProfile>(BuildProfilePath);
        if (buildProfile == null)
        {
            Debug.LogError($"[WebGLBuildPipeline] Build Profile not found at: {BuildProfilePath}");
            return;
        }

        // Get absolute output path
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string outputPath = Path.GetFullPath(Path.Combine(projectRoot, RelativeOutputPath));

        string buildType = development ? "Development" : "Release";
        Debug.Log($"[WebGLBuildPipeline] Building WebGL ({buildType}) to: {outputPath}");
        Debug.Log($"[WebGLBuildPipeline] Using Build Profile: {BuildProfilePath}");

        // Get scenes from build settings (or profile if it overrides)
        string[] scenes = GetScenesFromProfile(buildProfile);
        if (scenes.Length == 0)
        {
            Debug.LogError("[WebGLBuildPipeline] No scenes found in build settings!");
            return;
        }

        // Get build options from the profile and override what we need
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = development
                ? BuildOptions.Development | BuildOptions.ConnectWithProfiler
                : BuildOptions.None,
        };

        // Perform the build
        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"[WebGLBuildPipeline] {buildType} build succeeded! Output: {outputPath}");

            // Clean up the index.html that Unity generates
            CleanupIndexHtml(outputPath);

            Debug.Log($"[WebGLBuildPipeline] {buildType} build complete and cleaned up!");
        }
        else
        {
            Debug.LogError(
                $"[WebGLBuildPipeline] {buildType} build failed with {report.summary.totalErrors} error(s)"
            );
        }
    }

    /// <summary>
    /// Gets scenes from the Build Profile if it overrides the global scene list,
    /// otherwise falls back to the Editor Build Settings scenes.
    /// </summary>
    private static string[] GetScenesFromProfile(BuildProfile profile)
    {
        // Check if the profile overrides the global scene list
        if (profile.scenes != null && profile.scenes.Length > 0)
        {
            var scenes = new System.Collections.Generic.List<string>();
            foreach (var scene in profile.scenes)
            {
                if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                {
                    scenes.Add(scene.path);
                }
            }
            if (scenes.Count > 0)
            {
                return scenes.ToArray();
            }
        }

        // Fall back to global build settings
        return GetEnabledScenes();
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

    /// <summary>
    /// Deletes all contents of the unity-webgl output folder.
    /// </summary>
    [MenuItem("Build/Clean WebGL Output")]
    public static void CleanWebGLOutput()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string outputPath = Path.GetFullPath(Path.Combine(projectRoot, RelativeOutputPath));

        if (!Directory.Exists(outputPath))
        {
            Debug.Log(
                $"[WebGLBuildPipeline] Output folder does not exist, nothing to clean: {outputPath}"
            );
            return;
        }

        // Delete all files in the directory
        foreach (string file in Directory.GetFiles(outputPath))
        {
            File.Delete(file);
        }

        // Delete all subdirectories
        foreach (string dir in Directory.GetDirectories(outputPath))
        {
            Directory.Delete(dir, recursive: true);
        }

        Debug.Log($"[WebGLBuildPipeline] Cleaned WebGL output folder: {outputPath}");
    }
}
