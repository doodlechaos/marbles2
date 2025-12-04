using System;
using System.Collections.Generic;
using FPMathLib;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;

public class GameTileExporter : EditorWindow
{
    private RenderPrefabRegistry cachedPrefabRegistry;

    private const string PREF_SERVER_URL = "GameTileExporter_ServerUrl";
    private const string PREF_MODULE_NAME = "GameTileExporter_ModuleName";

    private const string DEFAULT_SERVER_URL = "http://127.0.0.1:3000";
    private const string DEFAULT_MODULE_NAME = "marbles2";

    private string serverUrl;
    private string moduleName;

    [MenuItem("Window/GameCore/GameTileExporter")]
    public static void ShowWindow()
    {
        GetWindow<GameTileExporter>("Game Tile Exporter");
    }

    void OnEnable()
    {
        serverUrl = EditorPrefs.GetString(PREF_SERVER_URL, DEFAULT_SERVER_URL);
        moduleName = EditorPrefs.GetString(PREF_MODULE_NAME, DEFAULT_MODULE_NAME);
    }

    void OnGUI()
    {
        GUILayout.Label("Game Tile Export Tools", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        serverUrl = EditorGUILayout.TextField("Server URL", serverUrl);
        moduleName = EditorGUILayout.TextField("Module Name", moduleName);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(PREF_SERVER_URL, serverUrl);
            EditorPrefs.SetString(PREF_MODULE_NAME, moduleName);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Export and Upload Levels to SpacetimeDB"))
        {
            ExportAndUploadLevelsToSpacetimeDBAsync();
        }
    }

    private async void ExportAndUploadLevelsToSpacetimeDBAsync()
    {
        DbConnection adminConn = null;

        try
        {
            if (!CachePrefabRegistry())
            {
                Debug.LogError(
                    "Could not find RenderPrefabRegistry asset. Please create one via Assets > Create > Marbles > Render Prefab Registry."
                );
                return;
            }

            Debug.Log(
                $"Found RenderPrefabRegistry with {cachedPrefabRegistry.Count} render prefabs configured."
            );

            Debug.Log("Creating temporary admin connection...");
            adminConn = await STDB.GetTempAdminConnection();
            Debug.Log("Admin connection established!");

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
            int uploadedCount = 0;

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab == null)
                {
                    Debug.LogWarning($"Failed to load prefab at {assetPath}");
                    continue;
                }

                GameTileAuthBase gameTileAuth = prefab.GetComponent<GameTileAuthBase>();
                if (gameTileAuth != null)
                {
                    // Convert the prefab to runtime GameTile data
                    GameTileBase gameTile = GameTileConverter.ConvertToGameTile(
                        prefab,
                        cachedPrefabRegistry
                    );

                    if (gameTile == null)
                    {
                        Debug.LogWarning($"Failed to convert {prefab.name}");
                        continue;
                    }

                    // Serialize the entire GameTileBase with MemoryPack
                    byte[] gameTileBinary = MemoryPackSerializer.Serialize<GameTileBase>(gameTile);

                    string tileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

                    // Upload to SpacetimeDB
                    UploadGameTileToSpacetimeDB(
                        adminConn,
                        guid,
                        tileName,
                        gameTileAuth,
                        gameTileBinary
                    );

                    Debug.Log(
                        $"✓ Uploaded: {tileName} (GUID: {guid}, Size: {gameTileBinary.Length} bytes)"
                    );
                    uploadedCount++;
                }
            }

            Debug.Log($"Upload complete! {uploadedCount} level(s) uploaded successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during export/upload: {ex}");
        }
        finally
        {
            if (adminConn != null)
            {
                Debug.Log("Disconnecting temporary admin connection...");
                adminConn.Disconnect();
            }
        }
    }

    private bool CachePrefabRegistry()
    {
        // Find the RenderPrefabRegistry asset in the project
        string[] guids = AssetDatabase.FindAssets("t:RenderPrefabRegistry");

        if (guids.Length == 0)
        {
            return false;
        }

        if (guids.Length > 1)
        {
            Debug.LogWarning("Multiple RenderPrefabRegistry assets found. Using the first one.");
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        cachedPrefabRegistry = AssetDatabase.LoadAssetAtPath<RenderPrefabRegistry>(path);

        return cachedPrefabRegistry != null;
    }

    private void UploadGameTileToSpacetimeDB(
        DbConnection conn,
        string unityPrefabGUID,
        string tileName,
        GameTileAuthBase gameTileAuth,
        byte[] gameTileBinary
    )
    {
        var gameTileData = new GameTile
        {
            UnityPrefabGuid = unityPrefabGUID,
            TileName = tileName,
            Rarity = (int)gameTileAuth.AppearanceFrequency,
            MinAuctionSpots = gameTileAuth.MinAuctionSpots,
            MaxAuctionSpots = gameTileAuth.MaxAuctionSpots,
            MaxRaffleDraws = gameTileAuth.MaxRaffleDraws,
            GameTileBinary = new List<byte>(gameTileBinary),
        };

        conn.Reducers.UpsertGameTile(gameTileData);
    }
}
