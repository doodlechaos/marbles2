using System;
using System.Linq;
using System.Runtime.InteropServices; //Must keep for WebGL builds
using System.Text;
using System.Threading.Tasks;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

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
    private GameObject _synchronizer;

    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private string _tokenConnectedWith;

    [SerializeField]
    private BidDisplayPanel _bidDisplayPanel;

    public void InitStdbConnection()
    {
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

        _tokenConnectedWith = SessionToken.Token;

        // Debug: Log token status before connecting
        bool hasToken = SessionToken.HasToken();
        string currentToken = SessionToken.Token;
        Debug.Log($"[STDB] SessionToken.HasToken() = {hasToken}");
        Debug.Log(
            $"[STDB] SessionToken.Token is null? {currentToken == null}, empty? {string.IsNullOrEmpty(currentToken)}"
        );
        if (!string.IsNullOrEmpty(currentToken) && currentToken.Length > 50)
        {
            Debug.Log($"[STDB] Token preview: {currentToken.Substring(0, 50)}...");
        }

        // If the user has a SpacetimeDB auth token stored in the Unity PlayerPrefs,
        // we can use it to authenticate the connection.
        if (hasToken)
        {
            Debug.Log("[STDB] Connecting WITH existing token");
            builder = builder.WithToken(currentToken);
        }
        else
        {
            Debug.Log("[STDB] Connecting WITHOUT token (will get new identity)");
        }

        // Building the connection will establish a connection to the SpacetimeDB
        // server.
        Conn = builder.Build();

        CreateTableCallbacks(Conn);

        Debug.Log($"[STDB] Connection initiated");
    }

    private void CreateTableCallbacks(DbConnection conn)
    {
        Debug.Log("Initialized table callbacks.");
        _bidDisplayPanel.SetCallbacks(conn);
    }

    void CheckIfNeedsToUploadProfilePicture(SubscriptionEventContext ctx)
    {
        Account localAccount = ctx.Db.Account.Iter().FirstOrDefault();

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

        if (
            localAccountCustomization.PfpVersion == 0
            && !string.IsNullOrEmpty(_authManager.userProfile?.picture)
        )
        {
            Debug.Log(
                $"[STDB] Account has no profile picture, uploading from OAuth: {_authManager.userProfile.picture}"
            );
            StartCoroutine(
                UploadProfilePictureFromUrl(
                    _authManager.userProfile?.picture,
                    SessionToken.Token,
                    localAccount.Identity
                )
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
        Debug.Log($"[STDB] HandleConnect called!");
        Debug.Log($"[STDB] Received identity: {identity}");
        Debug.Log(
            $"[STDB] Received token (first 50 chars): {(token?.Length > 50 ? token.Substring(0, 50) : token)}..."
        );
        Debug.Log(
            $"[STDB] Previous token we connected with: {(_tokenConnectedWith?.Length > 50 ? _tokenConnectedWith.Substring(0, 50) : _tokenConnectedWith)}..."
        );

        SessionToken.SaveToken(token);

        _synchronizer.SetActive(true);

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(
                (SubscriptionEventContext ctx) =>
                {
                    Debug.Log("Subscription applied!");
                    CheckIfNeedsToUploadProfilePicture(ctx);
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
                    "SELECT * FROM Account",
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
        Debug.LogError($"Connection error: {ex}");
    }

    void HandleDisconnect(DbConnection _conn, Exception ex)
    {
        Debug.Log("Disconnected.");
        if (ex != null)
        {
            Debug.LogException(ex);
        }
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
        Conn.Disconnect();
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
