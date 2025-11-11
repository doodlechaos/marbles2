using SpacetimeDB.Types;
using SpacetimeDB;
using System;
using UnityEngine;
using GameCoreLib;
using com.cyborgAssets.inspectorButtonPro;
using System.IO;
using MemoryPack;


public class GameManager : MonoBehaviour
{
    const string SERVER_URL = "http://127.0.0.1:3000";
    const string MODULE_NAME = "marbles2";

    public static event Action OnConnected;
    public static event Action OnSubscriptionApplied;

    public static GameManager Instance { get; private set; }
    public static Identity LocalIdentity { get; private set; }
    public static DbConnection Conn { get; private set; }

    public GameCore GameCore = new GameCore();

    public string Game1JSON_PATH;
    
    public RuntimeRenderer RuntimeRenderer; 

    private void Awake()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;
    }
    private void Start()
    {
        Instance = this;
        Application.targetFrameRate = 60;

        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection.Builder()
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
    }

    [ProButton]
    public void LoadTile1()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;

        string json = File.ReadAllText(Game1JSON_PATH);

        GameCore.GameTile1.Load(json);
        Debug.Log("Done Loading Tile1 Test");
        
        // Automatically render the loaded tile if renderer is assigned
        if (RuntimeRenderer != null)
        {
            RuntimeRenderer.RenderGameTile(GameCore.GameTile1);
            Debug.Log("Rendered Tile1");
        }
    }

    [ProButton]
    public void RenderTile1()
    {
        if (RuntimeRenderer != null)
        {
            RuntimeRenderer.RenderGameTile(GameCore.GameTile1);
            Debug.Log("Rendered Tile1");
        }
        else
        {
            Debug.LogError("RuntimeRenderer not assigned!");
        }
    }

    [ProButton]
    public void ClearRendering()
    {
        if (RuntimeRenderer != null)
        {
            RuntimeRenderer.ClearRendering();
            Debug.Log("Cleared rendering");
        }
    }

    [ProButton]
    public void StepPhysics()
    {
        GameCore.GameTile1.Step();
        Debug.Log("Stepped physics simulation");
    }

    [ProButton]
    public void StepPhysicsMultiple()
    {
        for (int i = 0; i < 60; i++)
        {
            GameCore.GameTile1.Step();
        }
        Debug.Log("Stepped physics simulation 60 times (1 second)");
    }

    // Called when we connect to SpacetimeDB and receive our client identity
    void HandleConnect(DbConnection _conn, Identity identity, string token)
    {
        Debug.Log("Connected.");
        AuthToken.SaveToken(token);
        LocalIdentity = identity;

        OnConnected?.Invoke();

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(HandleSubscriptionApplied)
            .SubscribeToAllTables();
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

    private void HandleSubscriptionApplied(SubscriptionEventContext ctx)
    {
        Debug.Log("Subscription applied!");
        OnSubscriptionApplied?.Invoke();
    }

    public static bool IsConnected()
    {
        return Conn != null && Conn.IsActive;
    }

    public void Disconnect()
    {
        Conn.Disconnect();
        Conn = null;
    }

    public void CallTestMethod()
    {
        Conn.Reducers.TestReducer();
        Debug.Log("Calling test reducer"); 
    }

    [ProButton]
    public void TestSerializeGameCore()
    {
        byte[] data = MemoryPackSerializer.Serialize(GameCore, new MemoryPackSerializerOptions { });
        //Write this to file
        string tempDir = "Temp";
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }
        string filePath = Path.Combine(tempDir, "GameCore.bin");
        File.WriteAllBytes(filePath, data);
        Debug.Log($"Serialized GameCore and wrote to {filePath}");
    }
    [ProButton]
    public void TestDeserializeGameCore()
    {
        string filePath = "Temp/GameCore.bin";
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }
        byte[] data = File.ReadAllBytes(filePath);
        GameCore = MemoryPackSerializer.Deserialize<GameCore>(data, new MemoryPackSerializerOptions { });
        Debug.Log("Deserialized GameCore successfully");
        // For testing, you can assign it back or compare, but for now, just log success
        // Example: GameCore = deserialized; (uncomment if needed)
    }
}