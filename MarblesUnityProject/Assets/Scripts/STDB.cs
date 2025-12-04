using System;
using System.Linq;
using System.Runtime.InteropServices; //Must keep for WebGL builds
using System.Text;
using System.Threading.Tasks;
using SpacetimeDB;
using SpacetimeDB.Types;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// STDB is the source of truth for SpacetimeDB connections and table subscriptions.
/// It does NOT know about OAuth flows - use events to communicate auth state changes.
/// </summary>
public class STDB : MonoBehaviour
{
    // Fallback values for non-WebGL builds (Editor, standalone)
    const string DEFAULT_SERVER_URL = "http://127.0.0.1:3000";
    const string DEFAULT_MODULE_NAME = "marbles2";

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetSpacetimeDBHost();

    [DllImport("__Internal")]
    private static extern string GetSpacetimeDBModuleName();

    private static string ServerUrl => GetSpacetimeDBHost();
    private static string ModuleName => GetSpacetimeDBModuleName();
#else
    private static string ServerUrl => DEFAULT_SERVER_URL;
    private static string ModuleName => DEFAULT_MODULE_NAME;
#endif

    /// <summary>
    /// Gets the profile picture API URL using the consolidated WebGLBrowser utility.
    /// </summary>
    private static string GetProfilePictureApiUrl() =>
        WebGLBrowser.GetApiUrl("/api/profile-picture");

    public static DbConnection Conn { get; private set; }

    [SerializeField]
    private Synchronizer _synchronizer;

    [SerializeField]
    private AuthManager _authManager; // Soft dependency - only used to read profile picture URL

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private string _tokenConnectedWith;

    /// <summary>
    /// Tracks whether we've already attempted to clear an invalid token and retry.
    /// Prevents infinite reconnection loops.
    /// </summary>
    private bool _hasAttemptedTokenClear = false;

    [SerializeField]
    private BidDisplayPanel _bidDisplayPanel;

    [SerializeField]
    public TextMeshProUGUI _marbleCountText;

    [SerializeField]
    public TextMeshProUGUI _pointsText;

    public string ConnectedIdentity = "";

    /// <summary>
    /// Reference to the connection we registered callbacks on.
    /// Used for proper cleanup when connection changes.
    /// </summary>
    private DbConnection _callbackConnection;

    // Events
    /// <summary>Raised when connection is established</summary>
    public event Action OnSTDBConnect;

    /// <summary>Raised when disconnected</summary>
    public event Action OnSTDBDisconnect;

    /// <summary>Raised when subscriptions are applied and ready</summary>
    public event Action OnSubscriptionsReady;

    /// <summary>Raised when an authentication error occurs (e.g., invalid token)</summary>
    public event Action<Exception> OnSTDBConnectError;

    /// <summary>
    /// Initialize and connect to SpacetimeDB.
    /// Uses SessionToken.Token if available (which may be the STDB token or ID token).
    /// Automatically disconnects any existing connection first.
    /// </summary>
    public void InitStdbConnection()
    {
        // Disconnect any existing connection to avoid orphaned sessions on the server
        Disconnect();

        // Log the resolved config (useful for debugging WebGL builds)
        Debug.Log($"[STDB] Connecting to SpacetimeDB - Host: {ServerUrl}, Module: {ModuleName}");

        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection
            .Builder()
            .OnConnect(HandleConnect)
            .OnConnectError(HandleConnectError)
            .OnDisconnect(HandleDisconnect)
            .WithUri(ServerUrl)
            .WithModuleName(ModuleName);

        _tokenConnectedWith = GetConnectionToken();

        // Debug: Log token status before connecting
        bool hasToken = !string.IsNullOrEmpty(_tokenConnectedWith);
        Debug.Log($"[STDB] HasToken = {hasToken}");
        if (hasToken && _tokenConnectedWith.Length > 50)
        {
            Debug.Log($"[STDB] Token preview: {_tokenConnectedWith.Substring(0, 50)}...");
        }

        // If we have a token, use it to authenticate the connection
        if (hasToken)
        {
            Debug.Log("[STDB] Connecting WITH token");
            builder = builder.WithToken(_tokenConnectedWith);
        }
        else
        {
            Debug.Log("[STDB] Connecting ANONYMOUSLY WITHOUT token");
        }

        // Building the connection will establish a connection to the SpacetimeDB server
        Conn = builder.Build();

        CreateTableCallbacks(Conn);

        Debug.Log($"[STDB] Connection initiated");
    }

    /// <summary>
    /// Gets the token to use for connection.
    /// Prioritizes stored STDB token, then falls back to OAuth ID token.
    /// </summary>
    private string GetConnectionToken()
    {
        // First check for stored SpacetimeDB session token
        if (SessionToken.HasToken())
        {
            return SessionToken.Token;
        }

        // Fall back to OAuth ID token for initial authentication
        if (_authManager != null && _authManager.HasIdToken())
        {
            return _authManager.GetIdToken();
        }

        return "";
    }

    private void CreateTableCallbacks(DbConnection conn)
    {
        Debug.Log("[STDB] Initializing table callbacks...");

        // Store reference for cleanup
        _callbackConnection = conn;

        // Set callbacks on all handlers
        _bidDisplayPanel.SetCallbacks(conn);
        _uiManager.SetCallbacks(conn);
        _synchronizer.SetCallbacks(conn);

        // Register STDB's own callbacks using method references
        conn.Db.MyAccount.OnInsert += OnMyAccountInsert;
        conn.Db.MyAccount.OnUpdate += OnMyAccountUpdate;

        Debug.Log("[STDB] All table callbacks initialized");
    }

    // Callback method handlers for STDB's own callbacks
    private void OnMyAccountInsert(EventContext ctx, Account account)
    {
        UpdateAccountStats(account);
    }

    private void OnMyAccountUpdate(EventContext ctx, Account oldAccount, Account newAccount)
    {
        UpdateAccountStats(newAccount);
    }

    /// <summary>
    /// Cleanup all registered callbacks from all handlers.
    /// Call this before disconnecting or when changing connections.
    /// Safe to call multiple times.
    /// </summary>
    private void CleanupAllCallbacks()
    {
        if (_callbackConnection == null)
            return;

        Debug.Log("[STDB] Cleaning up all callbacks...");

        // Cleanup all handler callbacks (pass the connection they registered on)
        _synchronizer.CleanupCallbacks(_callbackConnection);
        _uiManager.CleanupCallbacks(_callbackConnection);
        _bidDisplayPanel.CleanupCallbacks(_callbackConnection);

        // Cleanup STDB's own callbacks
        _callbackConnection.Db.MyAccount.OnInsert -= OnMyAccountInsert;
        _callbackConnection.Db.MyAccount.OnUpdate -= OnMyAccountUpdate;
        _callbackConnection = null;

        Debug.Log("[STDB] All callbacks cleaned up");
    }

    private void UpdateAccountStats(Account account)
    {
        Debug.Log(
            $"[STDB] Updating account stats: {account.Marbles} marbles, {account.Points} points"
        );
        _marbleCountText.SetText($"Marbles: {account.Marbles}");
        _pointsText.SetText($"Points: {account.Points}");
    }

    void CheckIfNeedsToUploadProfilePicture(SubscriptionEventContext ctx)
    {
        Account localAccount = ctx.Db.MyAccount.Iter().FirstOrDefault();

        // Only process profile picture upload for the LOCAL identity's account
        if (localAccount == null)
        {
            Debug.Log("[STDB] LocalIdentity not set yet, skipping pfp check");
            return;
        }

        AccountCustomization localAccountCustomization = ctx.Db.AccountCustomization.AccountId.Find(
            localAccount.Id
        );

        if (localAccountCustomization == null)
        {
            Debug.LogError("Local Account Customization is null");
            return;
        }

        // Soft dependency on AuthManager - only for reading profile picture URL
        string pictureUrl = _authManager?.userProfile?.picture;

        if (localAccountCustomization.PfpVersion == 0 && !string.IsNullOrEmpty(pictureUrl))
        {
            Debug.Log($"[STDB] Account has no profile picture, uploading from OAuth: {pictureUrl}");
            StartCoroutine(
                UploadProfilePictureFromUrl(pictureUrl, SessionToken.Token, localAccount.Identity)
            );
        }
        else
        {
            Debug.Log(
                $"Local Account {localAccount} {localAccountCustomization} doesn't need to upload profile picture from auth"
            );
        }
    }

    // Called when we connect to SpacetimeDB and receive our client identity
    void HandleConnect(DbConnection _conn, Identity identity, string token)
    {
        // Reset the token clear flag on successful connection
        _hasAttemptedTokenClear = false;

        ConnectedIdentity = identity.ToString();
        Debug.Log($"[STDB] HandleConnect called!");
        Debug.Log($"[STDB] Received identity: {identity}");
        Debug.Log(
            $"[STDB] Received token (first 50 chars): {(token?.Length > 50 ? token.Substring(0, 50) : token)}..."
        );
        Debug.Log(
            $"[STDB] Previous token we connected with: {(_tokenConnectedWith?.Length > 50 ? _tokenConnectedWith.Substring(0, 50) : _tokenConnectedWith)}..."
        );

        // Save the SpacetimeDB session token for future connections
        SessionToken.SaveToken(token);

        // Notify listeners that connection is established
        OnSTDBConnect?.Invoke();

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(
                (SubscriptionEventContext ctx) =>
                {
                    Debug.Log("Subscription applied!");
                    CheckIfNeedsToUploadProfilePicture(ctx);

                    // Notify listeners that we're fully ready
                    OnSubscriptionsReady?.Invoke();
                }
            )
            .OnError(
                (ErrorContext ctx, Exception ex) =>
                {
                    Debug.LogError($"Subscription error: {ex}");
                }
            )
            .Subscribe(
                new string[]
                {
                    "SELECT * FROM AuthFrame",
                    "SELECT * FROM MyAccount",
                    "SELECT * FROM MySessionKind",
                    "SELECT * FROM AccountCustomization",
                    "SELECT * FROM BaseCfg",
                    "SELECT * FROM AccountBid",
                    "SELECT * FROM BiddingStateS",
                    "SELECT * FROM BidTimeS",
                }
            );
    }

    void HandleConnectError(Exception ex)
    {
        Debug.LogError($"[STDB] Connection error: {ex}");

        // Check if this is an authentication error (401 Unauthorized / invalid token)
        bool isAuthError = IsAuthenticationError(ex);

        if (isAuthError && !_hasAttemptedTokenClear && SessionToken.HasToken())
        {
            Debug.LogWarning(
                "[STDB] Detected authentication error with stored token. Clearing token and reconnecting..."
            );

            // Mark that we've attempted to clear the token to prevent infinite loops
            _hasAttemptedTokenClear = true;

            // Clear the invalid SpacetimeDB session token (our responsibility)
            SessionToken.ClearToken();

            // Notify listeners about the auth error so they can clear OAuth credentials if needed
            OnSTDBConnectError?.Invoke(ex);

            // Reconnect without the invalid token (will connect anonymously)
            Debug.Log("[STDB] Reconnecting after clearing invalid token...");
            InitStdbConnection();
        }
        else if (isAuthError && _hasAttemptedTokenClear)
        {
            Debug.LogError(
                "[STDB] Authentication error persists after clearing token. User may need to log in again."
            );
            // Notify listeners so they can show login UI
            OnSTDBConnectError?.Invoke(ex);
        }
    }

    /// <summary>
    /// Checks if the exception indicates an authentication/authorization error.
    /// This includes 401 Unauthorized responses and token verification failures.
    /// </summary>
    private bool IsAuthenticationError(Exception ex)
    {
        if (ex == null)
            return false;

        string message = ex.ToString().ToLowerInvariant();

        // Specific auth failure patterns (not just mentioned in help text)
        if (
            message.Contains("failed to verify token")
            || message.Contains("invalid token")
            || message.Contains("token expired")
            || message.Contains("authentication failed")
            || message.Contains("401")
            || message.Contains("unauthorized")
            || message.Contains("failed to verify token")
        )
        {
            return true;
        }

        // In WebGL, auth failures often come as generic "Failed to connect WebSocket"
        // The actual 401 is only visible in browser console (JavaScript layer)
        // If we have a token and get this error, it's likely a bad token
#if UNITY_WEBGL && !UNITY_EDITOR
        if (message.Contains("failed to connect websocket"))
        {
            Debug.Log(
                "[STDB] WebGL connection failure detected - may be auth-related if token is present"
            );
            return true;
        }
#endif

        return false;
    }

    /// <summary>
    /// Disconnect from SpacetimeDB.
    /// </summary>
    public void Disconnect()
    {
        // Cleanup ALL callbacks BEFORE disconnecting to unregister them
        // and prevent handlers from running with stale state while we're reconnecting
        CleanupAllCallbacks();

        if (Conn != null)
        {
            Debug.Log($"[STDB] Disconnect() called. IsActive={Conn.IsActive}");
            try
            {
                Conn.Disconnect();
                Debug.Log("[STDB] Disconnected from SpacetimeDB.");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[STDB] Error during disconnect: {ex.Message}");
            }
            Conn = null; // Clear reference
        }
        else
        {
            Debug.LogWarning("[STDB] No connection to disconnect from.");
        }
    }

    void HandleDisconnect(DbConnection _conn, Exception ex)
    {
        Debug.Log("[STDB] Disconnected.");
        if (ex != null)
        {
            Debug.LogException(ex);
        }
        OnSTDBDisconnect?.Invoke();
    }

    /// <summary>
    /// Uploads a profile picture to the R2 bucket by providing an image URL.
    /// The server will download the image from the URL and store it.
    /// </summary>
    private System.Collections.IEnumerator UploadProfilePictureFromUrl(
        string imageUrl,
        string authToken,
        Identity identity
    )
    {
        string apiUrl = GetProfilePictureApiUrl();
        string identityHex = identity.ToString(); // SpacetimeDB Identity.ToString() returns hex
        Debug.Log(
            $"[STDB] Starting profile picture upload from URL: {imageUrl} to API: {apiUrl} for identity: {identityHex}"
        );

        // Create JSON payload with imageUrl and identity
        string jsonPayload = $"{{\"imageUrl\":\"{imageUrl}\",\"identity\":\"{identityHex}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {authToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(
                    $"[STDB] Profile picture uploaded successfully: {request.downloadHandler.text}"
                );
            }
            else
            {
                Debug.LogError(
                    $"[STDB] Failed to upload profile picture: {request.error} - {request.downloadHandler.text}"
                );
            }
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Creates a temporary admin connection for editor scripts to use.
    /// The caller is responsible for calling Disconnect() on the returned connection.
    /// </summary>
    public static Task<DbConnection> GetTempAdminConnection()
    {
        // Read admin token from secrets.json
        string secretsJson = System.IO.File.ReadAllText("Assets/secrets.json");
        var secrets =
            Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<
                string,
                string
            >>(secretsJson);

        if (
            !secrets.TryGetValue("adminToken", out string adminToken)
            || string.IsNullOrEmpty(adminToken)
        )
        {
            throw new Exception("Admin token not found in Assets/secrets.json");
        }

        var tcs = new TaskCompletionSource<DbConnection>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );

        DbConnection tempConn = null;

        tempConn = DbConnection
            .Builder()
            .OnConnect(
                (conn, identity, token) =>
                {
                    Debug.Log("Temp admin connection established");
                    tcs.TrySetResult(conn);
                }
            )
            .OnConnectError(
                (ex) =>
                {
                    Debug.LogError($"Temp admin connection error: {ex}");
                    tcs.TrySetException(ex);
                }
            )
            .OnDisconnect(
                (ctx, ex) =>
                {
                    if (!tcs.Task.IsCompleted)
                    {
                        tcs.TrySetException(
                            ex
                                ?? new Exception(
                                    "Temp admin connection disconnected before connect."
                                )
                        );
                    }
                }
            )
            .WithUri(DEFAULT_SERVER_URL)
            .WithModuleName(DEFAULT_MODULE_NAME)
            .WithToken(adminToken)
            .Build();

        double startTime = EditorApplication.timeSinceStartup;
        const double timeoutSeconds = 10.0;

        void Pump()
        {
            // If already done (success or failure), stop pumping
            if (tcs.Task.IsCompleted)
            {
                EditorApplication.update -= Pump;
                Debug.Log("Pumping temp admin connection stopped (task completed)");
                return;
            }

            // Advance the connection so callbacks can run
            Debug.Log(
                $"Pumping temp admin connection advancing frame (IsActive={tempConn.IsActive})"
            );
            tempConn.FrameTick();

            // Optional: timeout so we don't hang forever if the server is down
            if (EditorApplication.timeSinceStartup - startTime > timeoutSeconds)
            {
                EditorApplication.update -= Pump;
                tempConn.Disconnect();
                if (!tcs.Task.IsCompleted)
                {
                    tcs.TrySetException(new TimeoutException("Timed out connecting temp admin."));
                }
                Debug.Log("Pumping temp admin connection stopped (timeout)");
            }
        }

        // Start pumping on the editor main thread
        EditorApplication.update += Pump;
        Debug.Log("Pumping temp admin connection");

        return tcs.Task;
    }
#endif
}
