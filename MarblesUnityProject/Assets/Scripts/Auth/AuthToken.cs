using UnityEngine;

/// <summary>
/// Helper class for managing SpacetimeDB authentication tokens
/// This class can work with both traditional SpacetimeDB tokens and SpacetimeAuth ID tokens
/// </summary>
public static class AuthToken
{
    private const string TOKEN_KEY = "spacetimedb_auth_token";

    /// <summary>
    /// Get the current authentication token
    /// First checks for SpacetimeAuth ID token, then falls back to stored SpacetimeDB token
    /// </summary>
    public static string Token
    {
        get
        {
            // First, try to get ID token from AuthManager if it exists and is authenticated
            AuthManager authManager = Object.FindObjectOfType<AuthManager>();
            if (authManager != null && authManager.IsAuthenticated())
            {
                string idToken = authManager.GetIdToken();
                if (!string.IsNullOrEmpty(idToken))
                {
                    return idToken;
                }
            }

            // Fall back to stored SpacetimeDB token
            return PlayerPrefs.GetString(TOKEN_KEY, "");
        }
    }

    /// <summary>
    /// Save a SpacetimeDB token (not SpacetimeAuth ID token)
    /// ID tokens from SpacetimeAuth are managed by AuthManager
    /// </summary>
    public static void SaveToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            PlayerPrefs.SetString(TOKEN_KEY, token);
            PlayerPrefs.Save();
            Debug.Log("[AuthToken] SpacetimeDB token saved");
        }
    }

    /// <summary>
    /// Clear the stored token
    /// </summary>
    public static void ClearToken()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.Save();
        Debug.Log("[AuthToken] Token cleared");
    }

    /// <summary>
    /// Check if we have any authentication token available
    /// </summary>
    public static bool HasToken()
    {
        return !string.IsNullOrEmpty(Token);
    }
}
