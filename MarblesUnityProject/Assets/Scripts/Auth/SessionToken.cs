using UnityEngine;

/// <summary>
/// Helper class for managing SpacetimeDB authentication tokens
/// This class can work with both traditional SpacetimeDB tokens and SpacetimeAuth ID tokens
/// </summary>
public static class SessionToken
{
    private const string TOKEN_KEY = "spacetimedb_auth_token";

    /// <summary>
    /// Get the current authentication token
    /// Prioritizes stored SpacetimeDB session token (long-lived), then falls back to OAuth ID token (short-lived)
    /// </summary>
    public static string Token
    {
        get
        {
            // First, check for stored SpacetimeDB session token (this has no expiration)
            string storedToken = PlayerPrefs.GetString(TOKEN_KEY, "");
            if (!string.IsNullOrEmpty(storedToken))
            {
                return storedToken;
            }

            // Fall back to OAuth ID token for initial authentication
            // This token has a short expiration (~60 seconds) and should only be used once
            AuthManager authManager = Object.FindFirstObjectByType<AuthManager>();
            if (authManager != null && authManager.IsAuthenticated())
            {
                string idToken = authManager.GetIdToken();
                if (!string.IsNullOrEmpty(idToken))
                {
                    return idToken;
                }
            }

            return "";
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
