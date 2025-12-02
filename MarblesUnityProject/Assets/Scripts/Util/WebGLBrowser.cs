using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Utility class for WebGL browser interactions.
/// Consolidates all JavaScript interop for browser-related functionality.
///
/// Fallback URLs are used in Editor/Standalone builds:
/// - GetCurrentPageUrl: Falls back based on build type
/// - In Editor: Not used for OAuth (loopback server is used instead)
/// - In WebGL: Gets actual browser URL via JavaScript interop
/// </summary>
public static class WebGLBrowser
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetPageUrl();
#endif

    // Default fallback for local Svelte dev server
    private const string DEFAULT_LOCAL_GAME_URL = "http://localhost:5173/game";
    private const string DEFAULT_LOCAL_ORIGIN = "http://localhost:5173";

    /// <summary>
    /// Gets the current page URL. Returns a fallback URL in editor/standalone builds.
    /// In WebGL, this returns the actual browser URL including query parameters.
    /// </summary>
    public static string GetCurrentPageUrl(string fallback = DEFAULT_LOCAL_GAME_URL)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            string url = GetPageUrl();
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[WebGLBrowser] Failed to get page URL: {ex.Message}");
        }
#endif
        return fallback;
    }

    /// <summary>
    /// Gets the current page URL without query parameters or hash fragments.
    /// This is used as the OAuth redirect URI for WebGL builds.
    /// </summary>
    public static string GetCurrentPageUrlWithoutQuery(string fallback = DEFAULT_LOCAL_GAME_URL)
    {
        string fullUrl = GetCurrentPageUrl(fallback);

        int queryIndex = fullUrl.IndexOf('?');
        int hashIndex = fullUrl.IndexOf('#');

        int cutoffIndex = -1;
        if (queryIndex >= 0 && hashIndex >= 0)
        {
            cutoffIndex = Math.Min(queryIndex, hashIndex);
        }
        else if (queryIndex >= 0)
        {
            cutoffIndex = queryIndex;
        }
        else if (hashIndex >= 0)
        {
            cutoffIndex = hashIndex;
        }

        if (cutoffIndex >= 0)
        {
            return fullUrl.Substring(0, cutoffIndex);
        }

        return fullUrl;
    }

    /// <summary>
    /// Gets the origin (scheme + authority) of the current page.
    /// Example: "https://marbles.live" or "http://localhost:5173"
    /// </summary>
    public static string GetCurrentPageOrigin(string fallback = DEFAULT_LOCAL_ORIGIN)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            string pageUrl = GetPageUrl();
            if (!string.IsNullOrEmpty(pageUrl))
            {
                Uri uri = new Uri(pageUrl);
                return $"{uri.Scheme}://{uri.Authority}";
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[WebGLBrowser] Failed to get page origin: {ex.Message}");
        }
#endif
        return fallback;
    }

    /// <summary>
    /// Builds an API URL relative to the current page origin.
    /// Useful for making API calls to the same server hosting the game.
    /// </summary>
    public static string GetApiUrl(string path, string fallbackOrigin = DEFAULT_LOCAL_ORIGIN)
    {
        string origin = GetCurrentPageOrigin(fallbackOrigin);
        // Ensure path starts with /
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }
        return origin + path;
    }

    /// <summary>
    /// Redirects the browser to the specified URL.
    /// In WebGL: Navigates the entire page (Unity will be unloaded!)
    /// In Editor/Standalone: Opens URL in system default browser
    /// </summary>
    public static void RedirectTo(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"window.location.href = '{EscapeJsString(url)}';");
#else
        Debug.Log($"[WebGLBrowser] Opening URL in browser: {url}");
        Application.OpenURL(url);
#endif
    }

    /// <summary>
    /// Cleans the current URL by removing query parameters and hash fragments.
    /// Uses history.replaceState to avoid adding a new history entry.
    /// Only has effect in WebGL builds.
    /// </summary>
    public static void CleanUrlParameters()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval(
            "window.history.replaceState({}, document.title, window.location.pathname);"
        );
#else
        Debug.Log("[WebGLBrowser] CleanUrlParameters called - no-op in Editor/Standalone");
#endif
    }

    /// <summary>
    /// Escapes a string for safe use in JavaScript.
    /// </summary>
    private static string EscapeJsString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return str.Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
}
