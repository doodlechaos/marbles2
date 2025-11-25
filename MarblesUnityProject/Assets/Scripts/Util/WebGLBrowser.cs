using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Utility class for WebGL browser interactions.
/// Consolidates all JavaScript interop for browser-related functionality.
/// </summary>
public static class WebGLBrowser
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetPageUrl();
#endif

    /// <summary>
    /// Gets the current page URL. Returns a fallback URL in editor/standalone builds.
    /// </summary>
    public static string GetCurrentPageUrl(string fallback = "http://localhost:8080")
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
    /// </summary>
    public static string GetCurrentPageUrlWithoutQuery(string fallback = "http://localhost:8080")
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
    /// Example: "https://example.com" or "http://localhost:5173"
    /// </summary>
    public static string GetCurrentPageOrigin(string fallback = "http://localhost:5173")
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
    /// </summary>
    public static string GetApiUrl(string path, string fallbackOrigin = "http://localhost:5173")
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
    /// In editor/standalone, opens the URL in the default browser.
    /// </summary>
    public static void RedirectTo(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"window.location.href = '{url}';");
#else
        Debug.Log($"[WebGLBrowser] Would redirect to: {url}");
        Application.OpenURL(url);
#endif
    }

    /// <summary>
    /// Cleans the current URL by removing query parameters and hash fragments.
    /// Uses history.replaceState to avoid adding a new history entry.
    /// </summary>
    public static void CleanUrlParameters()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval(
            "window.history.replaceState({}, document.title, window.location.pathname);"
        );
#endif
    }
}

