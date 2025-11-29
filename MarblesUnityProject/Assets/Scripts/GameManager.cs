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
    public static GameManager Inst { get; private set; }

    [SerializeField]
    public GameCore GameCore = new GameCore();

    public string Game1JSON_PATH;

    public GameCoreRenderer GameCoreRenderer;

    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private STDB _stdb;

    [SerializeField]
    private bool _stdbInitialized = false;

    private void Awake()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;

        Inst = this;
        Application.targetFrameRate = 60;

        _authManager.InitAndTryRestoreSession();
        _authManager.OnAuthenticationSuccess += OnAuthSuccess;
    }

    private void Start()
    {
        _authManager.CheckForOAuthCallback();
        TryInitStdbConnection();
    }

    private void OnDestroy()
    {
        _authManager.OnAuthenticationSuccess -= OnAuthSuccess;
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
    }

    private void OnAuthSuccess()
    {
        // We only establish the SpacetimeDB connection once we know the auth token,
        // otherwise the connection uses a fresh local identity that won't match the
        // authenticated account (and profile picture upload never triggers).
        TryInitStdbConnection();
    }

    private void TryInitStdbConnection()
    {
        if (_stdbInitialized)
        {
            return;
        }

        // Only connect once we have a token (either a restored SpacetimeDB session
        // token or the newly acquired SpacetimeAuth ID token).
        if (!SessionToken.HasToken())
        {
            return;
        }

        _stdb.InitStdbConnection();
        _stdbInitialized = true;
    }
}
