using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [Header("SpacetimeAuth Configuration")]
    [Tooltip("Your SpacetimeAuth Client ID")]
    public string clientId = "";

    [Tooltip("SpacetimeAuth authority URL")]
    public string authority = "https://auth.spacetimedb.com/oidc";

    [Tooltip("Scopes to request (space-separated)")]
    public string scopes = "openid profile email";

    [Header("Runtime State")]
    public bool isAuthenticated = false;

    public Toggle isAuthenticatedVisualToggle;
    public string idToken = null;
    public UserProfile userProfile = null;

    // Events
    public event Action OnAuthenticationSuccess;
    public event Action<string> OnAuthenticationError;
    public event Action OnLogout;

    // Private state
    private string codeVerifier;
    private string state;
    private string redirectUri;

    private const string STORAGE_KEY_ID_TOKEN = "spacetimeauth_id_token";
    private const string STORAGE_KEY_USER_PROFILE = "spacetimeauth_user_profile";

    [Serializable]
    public class UserProfile
    {
        public string sub;
        public string email;
        public string name;
        public string preferred_username;
        public string picture;
    }

    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public string id_token;
        public string token_type;
        public int expires_in;
        public string scope;
    }

    [Serializable]
    private class ErrorResponse
    {
        public string error;
        public string error_description;
    }

    void Awake()
    {
        // Set redirect URI based on current page (without query parameters)
        redirectUri = GetCurrentPageUrlWithoutQuery();

        Debug.Log("redirectUri: " + redirectUri);

        // Try to restore previous session
        RestoreSession();
    }

    void Start()
    {
        // Check if we're returning from an OAuth callback
        CheckForOAuthCallback();
    }

    void Update()
    {
        isAuthenticatedVisualToggle.isOn = isAuthenticated;
    }

    /// <summary>
    /// Start the interactive login flow by redirecting to SpacetimeAuth
    /// </summary>
    public void StartLogin()
    {
        if (string.IsNullOrEmpty(clientId))
        {
            Debug.LogError("[AuthManager] Client ID is not configured!");
            OnAuthenticationError?.Invoke("Client ID not configured");
            return;
        }

        Debug.Log("[AuthManager] Starting interactive login...");

        // Generate PKCE challenge
        codeVerifier = GenerateCodeVerifier();
        string codeChallenge = GenerateCodeChallenge(codeVerifier);

        // Generate state for CSRF protection
        state = GenerateRandomString(32);

        // Store PKCE and state in session storage for when we return
        PlayerPrefs.SetString("pkce_verifier", codeVerifier);
        PlayerPrefs.SetString("oauth_state", state);
        PlayerPrefs.Save();

        // Build authorization URL
        string authUrl = BuildAuthorizationUrl(codeChallenge, state);

        Debug.Log($"[AuthManager] Redirecting to: {authUrl}");

        // Redirect to authorization endpoint
        RedirectToUrl(authUrl);
    }

    /// <summary>
    /// Logout and clear session
    /// </summary>
    public void Logout()
    {
        Debug.Log("[AuthManager] Logging out...");

        // Clear tokens and profile
        idToken = null;
        userProfile = null;
        isAuthenticated = false;

        // Clear stored session
        PlayerPrefs.DeleteKey(STORAGE_KEY_ID_TOKEN);
        PlayerPrefs.DeleteKey(STORAGE_KEY_USER_PROFILE);
        PlayerPrefs.DeleteKey("pkce_verifier");
        PlayerPrefs.DeleteKey("oauth_state");
        PlayerPrefs.Save();

        OnLogout?.Invoke();

        // Perform local logout by refreshing the page (clears any in-memory state)
        // SpacetimeAuth doesn't support a server-side logout endpoint,
        // so we just clear local session and refresh
        string currentPage = GetCurrentPageUrlWithoutQuery();
        RedirectToUrl(currentPage);
    }

    /// <summary>
    /// Get the current ID token for use with SpacetimeDB
    /// </summary>
    public string GetIdToken()
    {
        return idToken;
    }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        return isAuthenticated && !string.IsNullOrEmpty(idToken);
    }

    #region Private Methods

    private void CheckForOAuthCallback()
    {
        string currentUrl = GetCurrentPageUrl();

        Debug.Log("currentUrl: " + currentUrl);

        // Check if URL contains OAuth callback parameters
        if (currentUrl.Contains("?code=") || currentUrl.Contains("&code="))
        {
            Debug.Log("[AuthManager] Detected OAuth callback, processing...");
            ProcessOAuthCallback(currentUrl);
        }
    }

    private void ProcessOAuthCallback(string url)
    {
        Dictionary<string, string> queryParams = ParseQueryString(url);

        // Check for error response
        if (queryParams.ContainsKey("error"))
        {
            string error = queryParams["error"];
            string errorDescription = queryParams.ContainsKey("error_description")
                ? queryParams["error_description"]
                : "Unknown error";

            Debug.LogError($"[AuthManager] OAuth error: {error} - {errorDescription}");
            OnAuthenticationError?.Invoke($"{error}: {errorDescription}");

            // Clean up URL
            CleanUrlParameters();
            return;
        }

        // Get authorization code and state
        if (!queryParams.ContainsKey("code"))
        {
            Debug.LogError("[AuthManager] No authorization code found in callback");
            return;
        }

        string code = queryParams["code"];
        string returnedState = queryParams.ContainsKey("state") ? queryParams["state"] : "";

        // Verify state matches (CSRF protection)
        string expectedState = PlayerPrefs.GetString("oauth_state", "");
        if (returnedState != expectedState)
        {
            Debug.LogError("[AuthManager] State mismatch! Possible CSRF attack.");
            OnAuthenticationError?.Invoke("State verification failed");
            CleanUrlParameters();
            return;
        }

        // Retrieve stored code verifier
        codeVerifier = PlayerPrefs.GetString("pkce_verifier", "");
        if (string.IsNullOrEmpty(codeVerifier))
        {
            Debug.LogError("[AuthManager] Code verifier not found!");
            OnAuthenticationError?.Invoke("PKCE verifier missing");
            CleanUrlParameters();
            return;
        }

        // Exchange code for tokens
        StartCoroutine(ExchangeCodeForTokens(code));
    }

    private IEnumerator ExchangeCodeForTokens(string code)
    {
        Debug.Log("[AuthManager] Exchanging authorization code for tokens...");
        Debug.Log($"[AuthManager] Token endpoint: {authority}/token");
        Debug.Log($"[AuthManager] Redirect URI: {redirectUri}");
        Debug.Log($"[AuthManager] Client ID: {clientId}");

        string tokenEndpoint = $"{authority}/token";

        // Build form data
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "authorization_code");
        form.AddField("code", code);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("client_id", clientId);
        form.AddField("code_verifier", codeVerifier);

        using (UnityWebRequest request = UnityWebRequest.Post(tokenEndpoint, form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = $"Token exchange failed: {request.error}";

                // Try to parse error response
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    try
                    {
                        ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(
                            request.downloadHandler.text
                        );
                        errorMessage = $"{errorResponse.error}: {errorResponse.error_description}";
                    }
                    catch
                    {
                        errorMessage += $"\n{request.downloadHandler.text}";
                    }
                }

                Debug.LogError($"[AuthManager] {errorMessage}");
                OnAuthenticationError?.Invoke(errorMessage);
                CleanUrlParameters();
                yield break;
            }

            // Parse token response
            string responseText = request.downloadHandler.text;
            Debug.Log($"[AuthManager] Received token response");

            try
            {
                TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseText);

                if (string.IsNullOrEmpty(tokenResponse.id_token))
                {
                    Debug.LogError("[AuthManager] No ID token in response!");
                    OnAuthenticationError?.Invoke("No ID token received");
                    CleanUrlParameters();
                    yield break;
                }

                // Store tokens
                idToken = tokenResponse.id_token;

                // Parse and store user profile from ID token
                userProfile = ParseIdToken(idToken);

                // Mark as authenticated
                isAuthenticated = true;

                // Persist session
                SaveSession();

                Debug.Log($"[AuthManager] Authentication successful! User: {userProfile?.sub}");

                // Clean URL and notify success
                CleanUrlParameters();
                OnAuthenticationSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AuthManager] Failed to parse token response: {ex.Message}");
                OnAuthenticationError?.Invoke($"Token parsing failed: {ex.Message}");
                CleanUrlParameters();
            }
        }
    }

    private UserProfile ParseIdToken(string token)
    {
        try
        {
            // JWT is in format: header.payload.signature
            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                Debug.LogError("[AuthManager] Invalid JWT format");
                return null;
            }

            // Decode the payload (base64url encoded)
            string payload = parts[1];

            // Base64Url decode
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
            }

            byte[] payloadBytes = Convert.FromBase64String(payload);
            string payloadJson = Encoding.UTF8.GetString(payloadBytes);

            Debug.Log($"[AuthManager] ID Token payload: {payloadJson}");

            // Parse JSON
            UserProfile profile = JsonUtility.FromJson<UserProfile>(payloadJson);
            return profile;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Failed to parse ID token: {ex.Message}");
            return null;
        }
    }

    private void SaveSession()
    {
        PlayerPrefs.SetString(STORAGE_KEY_ID_TOKEN, idToken);
        if (userProfile != null)
        {
            PlayerPrefs.SetString(STORAGE_KEY_USER_PROFILE, JsonUtility.ToJson(userProfile));
        }
        PlayerPrefs.Save();
    }

    private void RestoreSession()
    {
        if (PlayerPrefs.HasKey(STORAGE_KEY_ID_TOKEN))
        {
            idToken = PlayerPrefs.GetString(STORAGE_KEY_ID_TOKEN);

            if (PlayerPrefs.HasKey(STORAGE_KEY_USER_PROFILE))
            {
                string profileJson = PlayerPrefs.GetString(STORAGE_KEY_USER_PROFILE);
                userProfile = JsonUtility.FromJson<UserProfile>(profileJson);
            }

            // TODO: Validate token hasn't expired
            // For now, just mark as authenticated if token exists
            if (!string.IsNullOrEmpty(idToken))
            {
                isAuthenticated = true;
                Debug.Log($"[AuthManager] Restored session for user: {userProfile?.sub}");
            }
        }
    }

    private string BuildAuthorizationUrl(string codeChallenge, string state)
    {
        string authEndpoint = $"{authority}/auth";

        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", clientId },
            { "redirect_uri", redirectUri },
            { "post_logout_redirect_uri", redirectUri },
            { "scope", scopes },
            { "state", state },
            { "code_challenge", codeChallenge },
            { "code_challenge_method", "S256" },
        };

        StringBuilder urlBuilder = new StringBuilder(authEndpoint);
        urlBuilder.Append("?");

        bool first = true;
        foreach (var kvp in queryParams)
        {
            if (!first)
                urlBuilder.Append("&");
            urlBuilder.Append(Uri.EscapeDataString(kvp.Key));
            urlBuilder.Append("=");
            urlBuilder.Append(Uri.EscapeDataString(kvp.Value));
            first = false;
        }

        return urlBuilder.ToString();
    }

    private string GenerateCodeVerifier()
    {
        // Generate a random 43-128 character string for PKCE
        return GenerateRandomString(128);
    }

    private string GenerateCodeChallenge(string verifier)
    {
        // SHA256 hash of the verifier
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            return Base64UrlEncode(hash);
        }
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        StringBuilder result = new StringBuilder(length);

        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            foreach (byte b in randomBytes)
            {
                result.Append(chars[b % chars.Length]);
            }
        }

        return result.ToString();
    }

    private string Base64UrlEncode(byte[] input)
    {
        string base64 = Convert.ToBase64String(input);
        // Convert base64 to base64url
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    private Dictionary<string, string> ParseQueryString(string url)
    {
        var result = new Dictionary<string, string>();

        int queryStart = url.IndexOf('?');
        if (queryStart < 0)
            return result;

        string query = url.Substring(queryStart + 1);
        string[] pairs = query.Split('&');

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split('=');
            if (parts.Length == 2)
            {
                string key = Uri.UnescapeDataString(parts[0]);
                string value = Uri.UnescapeDataString(parts[1]);
                result[key] = value;
            }
        }

        return result;
    }

    private string GetCurrentPageUrl()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetPageUrl();
#else
        return "http://localhost:8080"; // Fallback for editor testing
#endif
    }

    private string GetCurrentPageUrlWithoutQuery()
    {
        string fullUrl = GetCurrentPageUrl();

        // Remove query parameters and hash fragments
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

    private void RedirectToUrl(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"window.location.href = '{url}';");
#else
        Debug.Log($"[AuthManager] Would redirect to: {url}");
        Application.OpenURL(url);
#endif
    }

    private void CleanUrlParameters()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Clean the URL by removing query parameters (but keep the same page)
        Application.ExternalEval(
            "window.history.replaceState({}, document.title, window.location.pathname);"
        );
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetPageUrl();
#endif

    #endregion
}

