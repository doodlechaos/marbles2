using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.Net;
using System.Threading;
#endif

/// <summary>
/// AuthManager is the source of truth for OAuth login state and ID tokens.
/// It handles the OAuth/PKCE dance for both WebGL and Editor.
/// It does NOT know about SpacetimeDB connections directly - use events to wire up.
/// </summary>
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
    public string idToken = null;
    public UserProfile userProfile = null;

    // Events
    /// <summary>Raised when authentication succeeds (either via login or session restore)</summary>
    public event Action OnAuthenticationSuccess;

    /// <summary>Raised when user logs out</summary>
    public event Action OnLogout;

    /// <summary>Raised when authentication fails with an error message</summary>
    public event Action<string> OnAuthenticationError;

    // Private state
    private string codeVerifier;
    private string state;
    private string redirectUri;

    private const string STORAGE_KEY_ID_TOKEN = "spacetimeauth_id_token";
    private const string STORAGE_KEY_USER_PROFILE = "spacetimeauth_user_profile";

#if UNITY_EDITOR
    // Editor-only: Loopback server for OAuth callback
    private HttpListener httpListener;
    private Thread listenerThread;
    private const int LOOPBACK_PORT = 8400;
    private const string LOOPBACK_CALLBACK_PATH = "/editor-callback";
    private volatile bool isListening = false;
#endif

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

    public void InitAndTryRestoreSession()
    {
#if UNITY_EDITOR
        // Ensure main thread dispatcher exists for Editor OAuth callback
        UnityMainThreadDispatcher.EnsureExists();
#endif

        // Set redirect URI based on current page (without query parameters)
        // For WebGL: this will be the actual page URL
        // For Editor: this will be overridden in StartEditorLogin()
        redirectUri = GetCurrentPageUrlWithoutQuery();

        Debug.Log($"[AuthManager] Initial redirect URI: {redirectUri}");

        // Try to restore previous session
        RestoreSession();
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        StopLoopbackListener();
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        StopLoopbackListener();
#endif
    }

    /// <summary>
    /// Start the interactive login flow.
    /// In Editor: Opens system browser and uses loopback server for callback.
    /// In WebGL: Redirects entire page to auth provider.
    /// </summary>
    public void StartLogin()
    {
        if (string.IsNullOrEmpty(clientId))
        {
            Debug.LogError("[AuthManager] Client ID is not configured!");
            OnAuthenticationError?.Invoke("Client ID not configured");
            return;
        }

#if UNITY_EDITOR
        StartEditorLogin();
#else
        StartWebGLLogin();
#endif
    }

    /// <summary>
    /// Logout and clear all OAuth session data.
    /// Listeners should disconnect from STDB and reconnect as anonymous.
    /// </summary>
    public void Logout()
    {
        Debug.Log("[AuthManager] Logging out...");

        // Clear in-memory tokens and profile
        idToken = null;
        userProfile = null;

        // Clear stored OAuth session
        PlayerPrefs.DeleteKey(STORAGE_KEY_ID_TOKEN);
        PlayerPrefs.DeleteKey(STORAGE_KEY_USER_PROFILE);
        PlayerPrefs.DeleteKey("pkce_verifier");
        PlayerPrefs.DeleteKey("oauth_state");
        PlayerPrefs.Save();

        // Clear SpacetimeDB token as well (owned by SessionToken but we clear on logout)
        SessionToken.ClearToken();

        // Notify listeners - they should disconnect and reconnect as anonymous
        OnLogout?.Invoke();

        Debug.Log("[AuthManager] Logout complete");
    }

    /// <summary>
    /// Get the current ID token for use with SpacetimeDB
    /// </summary>
    public string GetIdToken()
    {
        return idToken;
    }

    /// <summary>
    /// Check if user has a local OAuth ID token.
    /// Note: For authoritative auth state, use the server's session kind.
    /// </summary>
    public bool HasIdToken()
    {
        return !string.IsNullOrEmpty(idToken);
    }

    /// <summary>
    /// Check for OAuth callback in URL (WebGL only - called on page load)
    /// </summary>
    public void CheckForOAuthCallback()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string currentUrl = GetCurrentPageUrl();

        Debug.Log($"[AuthManager] Checking URL for OAuth callback: {currentUrl}");

        // Check if URL contains OAuth callback parameters
        if (currentUrl.Contains("?code=") || currentUrl.Contains("&code="))
        {
            Debug.Log("[AuthManager] Detected OAuth callback, processing...");
            ProcessOAuthCallback(currentUrl);
        }
#else
        Debug.Log(
            "[AuthManager] CheckForOAuthCallback called - skipping in Editor (using loopback server)"
        );
#endif
    }

    /// <summary>
    /// Clears all stored credentials including in-memory tokens.
    /// Used for recovery from invalid token errors.
    /// </summary>
    public void ClearAllCredentials()
    {
        // Clear in-memory state
        idToken = null;
        userProfile = null;

        // Clear all PlayerPrefs (includes both SpacetimeDB token and OAuth tokens)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[AuthManager] All credentials cleared (memory + storage)");
    }

    #region WebGL OAuth Flow

    /// <summary>
    /// WebGL login: Redirects the browser page to auth provider
    /// </summary>
    private void StartWebGLLogin()
    {
        Debug.Log("[AuthManager] Starting WebGL login (page redirect)...");

        // Generate PKCE challenge
        codeVerifier = GenerateCodeVerifier();
        string codeChallenge = GenerateCodeChallenge(codeVerifier);

        // Generate state for CSRF protection
        state = GenerateRandomString(32);

        // Store PKCE and state - these persist across page reload via IndexedDB (WebGL) or registry (Editor)
        PlayerPrefs.SetString("pkce_verifier", codeVerifier);
        PlayerPrefs.SetString("oauth_state", state);
        PlayerPrefs.Save();

        // Redirect URI is the current page (set in InitAndTryRestoreSession)
        Debug.Log($"[AuthManager] Using redirect URI: {redirectUri}");

        // Build authorization URL
        string authUrl = BuildAuthorizationUrl(codeChallenge, state);

        Debug.Log($"[AuthManager] Redirecting to: {authUrl}");

        // Redirect entire page to authorization endpoint
        // Unity will be completely unloaded and reloaded after auth!
        RedirectToUrl(authUrl);
    }

    #endregion

    #region Editor OAuth Flow (Loopback Server)

#if UNITY_EDITOR
    /// <summary>
    /// Editor login: Opens system browser and starts local HTTP server for callback
    /// </summary>
    private void StartEditorLogin()
    {
        Debug.Log("[AuthManager] Starting Editor login (loopback server)...");

        // Stop any existing listener
        StopLoopbackListener();

        // Set redirect URI to loopback with fixed port
        redirectUri = $"http://127.0.0.1:{LOOPBACK_PORT}{LOOPBACK_CALLBACK_PATH}";
        Debug.Log($"[AuthManager] Using loopback redirect URI: {redirectUri}");

        // Generate PKCE challenge
        codeVerifier = GenerateCodeVerifier();
        string codeChallenge = GenerateCodeChallenge(codeVerifier);

        // Generate state for CSRF protection
        state = GenerateRandomString(32);

        // Start loopback listener before opening browser
        if (!StartLoopbackListener())
        {
            Debug.LogError("[AuthManager] Failed to start loopback listener");
            OnAuthenticationError?.Invoke(
                $"Failed to start OAuth callback server on port {LOOPBACK_PORT}. Is another instance running?"
            );
            return;
        }

        // Build and open auth URL in system browser
        string authUrl = BuildAuthorizationUrl(codeChallenge, state);
        Debug.Log($"[AuthManager] Opening browser for auth: {authUrl}");
        Application.OpenURL(authUrl);
    }

    private bool StartLoopbackListener()
    {
        try
        {
            httpListener = new HttpListener();
            // Listen on the specific port and path
            httpListener.Prefixes.Add($"http://127.0.0.1:{LOOPBACK_PORT}{LOOPBACK_CALLBACK_PATH}/");
            httpListener.Start();
            isListening = true;

            listenerThread = new Thread(LoopbackListenerThread)
            {
                IsBackground = true,
                Name = "OAuth Loopback Listener",
            };
            listenerThread.Start();

            Debug.Log(
                $"[AuthManager] Loopback listener started at http://127.0.0.1:{LOOPBACK_PORT}{LOOPBACK_CALLBACK_PATH}"
            );
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Failed to start loopback listener: {ex.Message}");
            return false;
        }
    }

    private void StopLoopbackListener()
    {
        isListening = false;

        if (httpListener != null)
        {
            try
            {
                httpListener.Stop();
                httpListener.Close();
            }
            catch { }
            httpListener = null;
        }

        if (listenerThread != null && listenerThread.IsAlive)
        {
            try
            {
                listenerThread.Join(1000); // Wait up to 1 second
                if (listenerThread.IsAlive)
                {
                    listenerThread.Abort();
                }
            }
            catch { }
            listenerThread = null;
        }
    }

    private void LoopbackListenerThread()
    {
        try
        {
            while (isListening && httpListener != null && httpListener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    // This blocks until a request comes in or the listener is stopped
                    context = httpListener.GetContext();
                }
                catch (HttpListenerException)
                {
                    // Listener was stopped
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                var request = context.Request;
                var response = context.Response;

                Debug.Log($"[AuthManager] Received callback: {request.Url}");

                // Send a nice response to the browser
                string responseHtml =
                    @"
<!DOCTYPE html>
<html>
<head>
    <title>Authentication Complete</title>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            color: #fff;
        }
        .container {
            text-align: center;
            padding: 40px;
            background: rgba(255,255,255,0.1);
            border-radius: 16px;
            backdrop-filter: blur(10px);
        }
        .checkmark {
            font-size: 64px;
            margin-bottom: 20px;
        }
        h1 { margin: 0 0 16px 0; font-weight: 600; }
        p { margin: 0; opacity: 0.8; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='checkmark'>✓</div>
        <h1>Authentication Complete</h1>
        <p>You can close this window and return to Unity.</p>
    </div>
</body>
</html>";

                byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);
                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();

                // Extract the callback URL and dispatch to main thread
                string callbackUrl = request.Url.ToString();

                // Dispatch processing to Unity's main thread
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    ProcessEditorOAuthCallback(callbackUrl);
                });

                // Stop listening after receiving callback
                break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AuthManager] Loopback listener error: {ex.Message}");
        }
        finally
        {
            isListening = false;
        }
    }

    /// <summary>
    /// Process OAuth callback received via loopback server (Editor only)
    /// </summary>
    private void ProcessEditorOAuthCallback(string callbackUrl)
    {
        Debug.Log($"[AuthManager] Processing Editor OAuth callback: {callbackUrl}");

        // Stop the listener
        StopLoopbackListener();

        // Process the callback (reuse the same logic as WebGL)
        ProcessOAuthCallback(callbackUrl);
    }
#endif

    #endregion

    #region Shared OAuth Processing

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

            // Clean up URL (WebGL only)
            CleanUrlParameters();
            return;
        }

        // Get authorization code and state
        if (!queryParams.ContainsKey("code"))
        {
            Debug.LogError("[AuthManager] No authorization code found in callback");
            OnAuthenticationError?.Invoke("No authorization code in callback");
            return;
        }

        string code = queryParams["code"];
        string returnedState = queryParams.ContainsKey("state") ? queryParams["state"] : "";

#if UNITY_EDITOR
        // In Editor, state is in memory (same Unity instance)
        string expectedState = state;
#else
        // In WebGL, state was saved before page redirect
        string expectedState = PlayerPrefs.GetString("oauth_state", "");
#endif

        // Verify state matches (CSRF protection)
        if (returnedState != expectedState)
        {
            Debug.LogError(
                $"[AuthManager] State mismatch! Expected: {expectedState}, Got: {returnedState}"
            );
            OnAuthenticationError?.Invoke("State verification failed - possible CSRF attack");
            CleanUrlParameters();
            return;
        }

#if !UNITY_EDITOR
        // In WebGL, retrieve stored code verifier (saved before page redirect)
        codeVerifier = PlayerPrefs.GetString("pkce_verifier", "");
        if (string.IsNullOrEmpty(codeVerifier))
        {
            Debug.LogError("[AuthManager] Code verifier not found in PlayerPrefs!");
            OnAuthenticationError?.Invoke("PKCE verifier missing - please try logging in again");
            CleanUrlParameters();
            return;
        }
#endif

        Debug.Log("[AuthManager] OAuth callback validated, exchanging code for tokens...");

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

                // Clear any existing SpacetimeDB token so we authenticate fresh with the new ID token
                SessionToken.ClearToken();

                // Persist OAuth session
                SaveSession();

                Debug.Log($"[AuthManager] Authentication successful! User: {userProfile?.sub}");

                // Clean URL (WebGL only) and notify success
                CleanUrlParameters();

                // Notify listeners - STDB will connect via this event
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

    #endregion

    #region Token & Session Management

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
            if (!string.IsNullOrEmpty(idToken))
            {
                Debug.Log($"[AuthManager] Restored session for user: {userProfile?.sub}");
            }
        }
    }

    #endregion

    #region URL Building & Parsing

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

    #endregion

    #region PKCE Helpers

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

    #endregion

    #region Browser Helpers

    private string GetCurrentPageUrl() => WebGLBrowser.GetCurrentPageUrl();

    private string GetCurrentPageUrlWithoutQuery() => WebGLBrowser.GetCurrentPageUrlWithoutQuery();

    private void RedirectToUrl(string url) => WebGLBrowser.RedirectTo(url);

    private void CleanUrlParameters() => WebGLBrowser.CleanUrlParameters();

    #endregion
}
