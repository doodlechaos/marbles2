using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            bool pumping = true;
            EditorApplication.CallbackFunction pumpAction = () =>
            {
                if (pumping && adminConn != null)
                {
                    adminConn.FrameTick();
                }
            };
            EditorApplication.update += pumpAction;

            try
            {
                Task<int> gameTask = ExportGameTilesAsync(adminConn);
                Task<int> throneTask = ExportThroneTilesAsync(adminConn);

                await Task.WhenAll(gameTask, throneTask);

                int gameTileCount = await gameTask;
                int throneTileCount = await throneTask;

                Debug.Log(
                    $"Upload complete! {gameTileCount} game tile(s) and {throneTileCount} throne tile(s) uploaded successfully"
                );
            }
            finally
            {
                pumping = false;
                EditorApplication.update -= pumpAction;
            }
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

            bool pumping = true;
            EditorApplication.CallbackFunction pumpAction = () =>
            {
                if (pumping && adminConn != null)
                {
                    adminConn.FrameTick();
                }
            };
            EditorApplication.update += pumpAction;

            try
            {
                int gameTileCount = await ExportGameTilesAsync(adminConn);

                Debug.Log($"Upload complete! {gameTileCount} game tile(s) uploaded successfully");
            }
            finally
            {
                pumping = false;
                EditorApplication.update -= pumpAction;
            }
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

            bool pumping = true;
            EditorApplication.CallbackFunction pumpAction = () =>
            {
                if (pumping && adminConn != null)
                {
                    adminConn.FrameTick();
                }
            };
            EditorApplication.update += pumpAction;

            try
            {
                int throneTileCount = await ExportThroneTilesAsync(adminConn);

                Debug.Log(
                    $"Upload complete! {throneTileCount} throne tile(s) uploaded successfully"
                );
            }
            finally
            {
                pumping = false;
                EditorApplication.update -= pumpAction;
            }
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

    private async Task<int> ExportGameTilesAsync(DbConnection adminConn)
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        List<Task> uploadTasks = new List<Task>();
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
                Task uploadTask = UploadGameTileToSpacetimeDBAsync(
                    adminConn,
                    guid,
                    tileName,
                    gameTileAuth,
                    gameTileBinary
                );
                uploadTasks.Add(uploadTask);

                uploadedCount++;
            }
        }

        await Task.WhenAll(uploadTasks);
        return uploadedCount;
    }

    private async Task<int> ExportThroneTilesAsync(DbConnection adminConn)
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        List<Task> uploadTasks = new List<Task>();
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
                Task uploadTask = UploadThroneTileToSpacetimeDBAsync(
                    adminConn,
                    guid,
                    tileName,
                    throneTileBinary
                );
                uploadTasks.Add(uploadTask);

                uploadedCount++;
            }
        }

        await Task.WhenAll(uploadTasks);
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

    private async Task UploadGameTileToSpacetimeDBAsync(
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
            GameTileBinary = new List<byte>(gameTileBinary),
        };

        var tcs = new TaskCompletionSource<bool>();

        RemoteReducers.UpsertGameTileHandler handler = null;
        handler = (ctx, gameTileData) =>
        {
            if (gameTileData.UnityPrefabGuid == unityPrefabGUID)
            {
                if (ctx.Event.Status is SpacetimeDB.Status.Committed)
                {
                    Debug.Log(
                        $"✓ Uploaded GameTile: {tileName} (GUID: {unityPrefabGUID}, Size: {gameTileBinary.Length} bytes)"
                    );
                    tcs.SetResult(true);
                }
                else
                {
                    string errorMsg = ctx.Event.Status is SpacetimeDB.Status.Failed failed
                        ? failed.ToString()
                        : "Out of energy";
                    Debug.LogError($"Failed to upload GameTile {tileName}: {errorMsg}");
                    tcs.SetException(new Exception(errorMsg));
                }
                conn.Reducers.OnUpsertGameTile -= handler;
            }
        };

        conn.Reducers.OnUpsertGameTile += handler;

        conn.Reducers.UpsertGameTile(gameTileData);

        var timeoutTask = Task.Delay(10000)
            .ContinueWith(_ => tcs.TrySetException(new TimeoutException("Upload timed out")));

        await tcs.Task;
    }

    private async Task UploadThroneTileToSpacetimeDBAsync(
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

        var tcs = new TaskCompletionSource<bool>();

        RemoteReducers.UpsertThroneTileHandler handler = null;
        handler = (ctx, throneTileData) =>
        {
            if (throneTileData.UnityPrefabGuid == unityPrefabGUID)
            {
                if (ctx.Event.Status is SpacetimeDB.Status.Committed)
                {
                    Debug.Log(
                        $"✓ Uploaded ThroneTile: {tileName} (GUID: {unityPrefabGUID}, Size: {throneTileBinary.Length} bytes)"
                    );
                    tcs.SetResult(true);
                }
                else
                {
                    string errorMsg = ctx.Event.Status is SpacetimeDB.Status.Failed failed
                        ? failed.ToString()
                        : "Out of energy";
                    Debug.LogError($"Failed to upload ThroneTile {tileName}: {errorMsg}");
                    tcs.SetException(new Exception(errorMsg));
                }
                conn.Reducers.OnUpsertThroneTile -= handler;
            }
        };

        conn.Reducers.OnUpsertThroneTile += handler;

        conn.Reducers.UpsertThroneTile(throneTileData);

        var timeoutTask = Task.Delay(10000)
            .ContinueWith(_ => tcs.TrySetException(new TimeoutException("Upload timed out")));

        await tcs.Task;
    }
}
