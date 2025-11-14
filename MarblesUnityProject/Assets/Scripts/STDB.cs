using System;
using System.Threading.Tasks;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;

public class STDB : MonoBehaviour
{
    const string SERVER_URL = "http://127.0.0.1:3000";
    const string MODULE_NAME = "marbles2";

    public static Identity LocalIdentity { get; private set; }
    public static DbConnection Conn { get; private set; }

    [SerializeField]
    private GameObject _synchronizer;

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
