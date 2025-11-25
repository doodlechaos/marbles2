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

    [SerializeField]
    private bool _forceStepping;

    public string Game1JSON_PATH;

    public GameCoreRenderer GameCoreRenderer;

    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private STDB _stdb;

    private void Awake()
    {
        GameCoreLib.Logger.Log = Debug.Log;
        GameCoreLib.Logger.Error = Debug.LogError;

        Inst = this;
        Application.targetFrameRate = 60;

        _authManager.InitAndTryRestoreSession();
    }

    private void Start()
    {
        _authManager.CheckForOAuthCallback();
        _stdb.InitStdbConnection();
    }

    private void FixedUpdate()
    {
        if (_forceStepping)
        {
            GameCore.Step(new List<InputEvent>());
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
