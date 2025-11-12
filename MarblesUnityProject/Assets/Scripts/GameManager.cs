using System;
using System.Collections.Generic;
using System.IO;
using com.cyborgAssets.inspectorButtonPro;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const string SERVER_URL = "http://127.0.0.1:3000";
    const string MODULE_NAME = "marbles2";

    public static event Action OnConnected;
    public static event Action OnSubscriptionApplied;

    public static GameManager Inst { get; private set; }
    public static Identity LocalIdentity { get; private set; }
    public static DbConnection Conn { get; private set; }

    public GameCore GameCore = new GameCore();

    [SerializeField]
    private bool _forceStepping;

    public string Game1JSON_PATH;

    public GameCoreRenderer GameCoreRenderer;

    private void Awake()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;
    }

    private void Start()
    {
        Inst = this;
        Application.targetFrameRate = 60;

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
    }

    private void FixedUpdate()
    {
        if (_forceStepping)
        {
            GameCore.Step(new List<InputEvent>());
        }
    }

    [ProButton]
    public void LoadTile1()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;

        string json = File.ReadAllText(Game1JSON_PATH);

        GameCore.GameTile1.Load(json, GameCore);
        Debug.Log("Done Loading Tile1 Test");

        // Automatically render the loaded tile if renderer is assigned
        if (GameCoreRenderer != null)
        {
            GameCoreRenderer.UpdateRendering();
            Debug.Log("Rendered Tile1");
        }
    }

    [ProButton]
    public void StepPhysics()
    {
        Debug.Log("GameTile1 Bodies: " + GameCore.GameTile1.Sim.Bodies.Count);
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
        Conn.SubscriptionBuilder().OnApplied(HandleSubscriptionApplied).SubscribeToAllTables();
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
        Debug.Log($"Serialized GameCore hash: {GameCore.GetDeterministicHashHex()}");
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
        GameCore = MemoryPackSerializer.Deserialize<GameCore>(
            data,
            new MemoryPackSerializerOptions { }
        );
        Debug.Log("Deserialized GameCore successfully");
        Debug.Log($"Deserialized GameCore hash: {GameCore.GetDeterministicHashHex()}");

        /*         // Re-render to sync with the new GameCore
                if (GameCoreRenderer != null)
                {
                    GameCoreRenderer.UpdateRendering();
                    Debug.Log("Re-rendered both tiles after deserialization");
                } */
    }
}
