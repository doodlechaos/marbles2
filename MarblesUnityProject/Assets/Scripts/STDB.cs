using System;
using System.Text;
using System.Threading.Tasks;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class STDB : MonoBehaviour
{
    const string SERVER_URL = "http://127.0.0.1:3000";
    const string MODULE_NAME = "marbles2";
    const string PROFILE_PICTURE_API_URL = "http://127.0.0.1:5173/api/profile-picture";

    public static Identity LocalIdentity { get; private set; }
    public static DbConnection Conn { get; private set; }

    [SerializeField]
    private GameObject _synchronizer;

    [SerializeField]
    private AuthManager _authManager;

    private void Awake()
    {
        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection
            .Builder()
            .OnConnect(HandleConnect)
            .OnConnectError(HandleConnectError)
            .OnDisconnect(HandleDisconnect)
            .WithUri(SERVER_URL)
            .WithModuleName(MODULE_NAME);

        // If the user has a SpacetimeDB auth token stored in the Unity PlayerPrefs,
        // we can use it to authenticate the connection.
        if (AuthToken.Token != "")
        {
            builder = builder.WithToken(AuthToken.Token);
        }

        // Building the connection will establish a connection to the SpacetimeDB
        // server.
        Conn = builder.Build();
        Debug.Log("Building stdb connection");
    }

    // Called when we connect to SpacetimeDB and receive our client identity
    void HandleConnect(DbConnection _conn, Identity identity, string token)
    {
        Debug.Log("Connected. Token: " + token);
        AuthToken.SaveToken(token);
        LocalIdentity = identity;

        _synchronizer.SetActive(true);

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(
                (SubscriptionEventContext ctx) =>
                {
                    Debug.Log("Subscription applied!");
                    // Here we should have the account and account customization for the local account loaded.
                    // If the pfp version of the local account is 0, and the user profile in the auth manager
                    // has a picture url, call the api endpoint to upload the picture to the cloudflare r2 bucket
                    Account localAccount = ctx.Db.Account.Identity.Find(identity);
                    AccountCustomization localAccountCustomization =
                        ctx.Db.AccountCustomization.AccountId.Find(localAccount.Id);
                    if (
                        localAccountCustomization.PfpVersion == 0
                        && !string.IsNullOrEmpty(_authManager.userProfile?.picture)
                    )
                    {
                        Debug.Log(
                            $"[STDB] Account has no profile picture, uploading from OAuth: {_authManager.userProfile.picture}"
                        );
                        StartCoroutine(
                            UploadProfilePictureFromUrl(_authManager.userProfile.picture, token)
                        );
                    }
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
        string authToken
    )
    {
        Debug.Log($"[STDB] Starting profile picture upload from URL: {imageUrl}");

        // Create JSON payload
        string jsonPayload = $"{{\"imageUrl\":\"{imageUrl}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        using (
            UnityWebRequest request = new UnityWebRequest(
                PROFILE_PICTURE_API_URL,
                UnityWebRequest.kHttpVerbPOST
            )
        )
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
            .WithUri(SERVER_URL)
            .WithModuleName(MODULE_NAME)
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
