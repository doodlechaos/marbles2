using System;
using System.Collections.Generic;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB.Types;
using UnityEditor;
using UnityEngine;

public class TileExporter : EditorWindow
{
    private RenderPrefabRegistry cachedPrefabRegistry;

    private const string PREF_SERVER_URL = "TileExporter_ServerUrl";
    private const string PREF_MODULE_NAME = "TileExporter_ModuleName";

    private const string DEFAULT_SERVER_URL = "http://127.0.0.1:3000";
    private const string DEFAULT_MODULE_NAME = "marbles2";

    private string serverUrl;
    private string moduleName;

    [MenuItem("Window/GameCore/Tile Exporter")]
    public static void ShowWindow()
    {
        GetWindow<TileExporter>("Tile Exporter");
    }

    void OnEnable()
    {
        serverUrl = EditorPrefs.GetString(PREF_SERVER_URL, DEFAULT_SERVER_URL);
        moduleName = EditorPrefs.GetString(PREF_MODULE_NAME, DEFAULT_MODULE_NAME);
    }

    void OnGUI()
    {
        GUILayout.Label("Tile Export Tools", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        serverUrl = EditorGUILayout.TextField("Server URL", serverUrl);
        moduleName = EditorGUILayout.TextField("Module Name", moduleName);
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString(PREF_SERVER_URL, serverUrl);
            EditorPrefs.SetString(PREF_MODULE_NAME, moduleName);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Export and Upload All Tiles to SpacetimeDB"))
        {
            ExportAndUploadAllTilesToSpacetimeDBAsync();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        if (GUILayout.Button("Export Game Tiles Only"))
        {
            ExportAndUploadGameTilesToSpacetimeDBAsync();
        }

        if (GUILayout.Button("Export Throne Tiles Only"))
        {
            ExportAndUploadThroneTilesToSpacetimeDBAsync();
        }
    }

    private async void ExportAndUploadAllTilesToSpacetimeDBAsync()
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

            int gameTileCount = ExportGameTiles(adminConn);
            int throneTileCount = ExportThroneTiles(adminConn);

            Debug.Log(
                $"Upload complete! {gameTileCount} game tile(s) and {throneTileCount} throne tile(s) uploaded successfully"
            );
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

    private async void ExportAndUploadGameTilesToSpacetimeDBAsync()
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

            Debug.Log("Creating temporary admin connection...");
            adminConn = await STDB.GetTempAdminConnection();
            Debug.Log("Admin connection established!");

            int gameTileCount = ExportGameTiles(adminConn);

            Debug.Log($"Upload complete! {gameTileCount} game tile(s) uploaded successfully");
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

    private async void ExportAndUploadThroneTilesToSpacetimeDBAsync()
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

            Debug.Log("Creating temporary admin connection...");
            adminConn = await STDB.GetTempAdminConnection();
            Debug.Log("Admin connection established!");

            int throneTileCount = ExportThroneTiles(adminConn);

            Debug.Log($"Upload complete! {throneTileCount} throne tile(s) uploaded successfully");
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

    private int ExportGameTiles(DbConnection adminConn)
    {
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
                    $"✓ Uploaded GameTile: {tileName} (GUID: {guid}, Size: {gameTileBinary.Length} bytes)"
                );
                uploadedCount++;
            }
        }

        return uploadedCount;
    }

    private int ExportThroneTiles(DbConnection adminConn)
    {
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

            ThroneTileAuthBase throneTileAuth = prefab.GetComponent<ThroneTileAuthBase>();
            if (throneTileAuth != null)
            {
                // Convert the prefab to runtime ThroneTile data
                ThroneTile throneTile = ThroneTileConverter.ConvertToThroneTile(
                    prefab,
                    cachedPrefabRegistry
                );

                if (throneTile == null)
                {
                    Debug.LogWarning($"Failed to convert {prefab.name}");
                    continue;
                }

                // Serialize the ThroneTile with MemoryPack
                byte[] throneTileBinary = MemoryPackSerializer.Serialize<ThroneTile>(throneTile);

                string tileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

                // Upload to SpacetimeDB
                UploadThroneTileToSpacetimeDB(adminConn, guid, tileName, throneTileBinary);

                Debug.Log(
                    $"✓ Uploaded ThroneTile: {tileName} (GUID: {guid}, Size: {throneTileBinary.Length} bytes)"
                );
                uploadedCount++;
            }
        }

        return uploadedCount;
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
        var gameTileData = new GameTileData
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

    private void UploadThroneTileToSpacetimeDB(
        DbConnection conn,
        string unityPrefabGUID,
        string tileName,
        byte[] throneTileBinary
    )
    {
        var throneTileData = new ThroneTileData
        {
            UnityPrefabGuid = unityPrefabGUID,
            TileName = tileName,
            ThroneTileBinary = new List<byte>(throneTileBinary),
        };

        conn.Reducers.UpsertThroneTile(throneTileData);
    }
}
