using UnityEngine;

/// <summary>
/// Helper class for managing SpacetimeDB session tokens.
/// This class ONLY manages the long-lived SpacetimeDB token.
/// OAuth/ID tokens are managed by AuthManager.
/// </summary>
public static class SessionToken
{
    private const string TOKEN_KEY = "spacetimedb_auth_token";

    /// <summary>
    /// Get the stored SpacetimeDB session token.
    /// Returns empty string if no token is stored.
    /// </summary>
    public static string Token => PlayerPrefs.GetString(TOKEN_KEY, "");

    /// <summary>
    /// Save a SpacetimeDB session token.
    /// </summary>
    public static void SaveToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            PlayerPrefs.SetString(TOKEN_KEY, token);
            PlayerPrefs.Save();
            Debug.Log("[SessionToken] SpacetimeDB token saved");
        }
    }

    /// <summary>
    /// Clear the stored SpacetimeDB session token.
    /// </summary>
    public static void ClearToken()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.Save();
        Debug.Log("[SessionToken] Token cleared");
    }

    /// <summary>
    /// Check if we have a stored SpacetimeDB session token.
    /// </summary>
    public static bool HasToken() => !string.IsNullOrEmpty(Token);
}
