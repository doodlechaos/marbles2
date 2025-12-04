using System;
using System.Collections.Generic;
using System.IO;
using com.cyborgAssets.inspectorButtonPro;
using GameCoreLib;
using MemoryPack;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

/// <summary>
/// GameManager serves as the bootstrap for the game.
/// It wires together AuthManager and STDB via events, keeping their responsibilities separate.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    [SerializeField]
    public GameCore GameCore = new GameCore();

    public string Game1JSON_PATH;

    [Header("Rendering")]
    [Tooltip("Renderer for GameTile1 (player 1's tile)")]
    public TileRenderer GameTile1Renderer;

    [Tooltip("Renderer for GameTile2 (player 2's tile)")]
    public TileRenderer GameTile2Renderer;

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

        // Wire up events between AuthManager and STDB
        WireAuthAndStdbEvents();

        // Initialize AuthManager and try to restore session
        _authManager.InitAndTryRestoreSession();
    }

    /// <summary>
    /// Wires events between AuthManager and STDB so they communicate via events
    /// rather than direct method calls. This keeps responsibilities clean:
    /// - AuthManager owns OAuth/login state
    /// - STDB owns SpacetimeDB connection
    /// </summary>
    private void WireAuthAndStdbEvents()
    {
        // When auth succeeds (either via login or session restore callback), connect to STDB
        _authManager.OnAuthenticationSuccess += HandleAuthSuccess;

        // When logout happens, disconnect STDB
        _authManager.OnLogout += HandleLogout;

        // When STDB hits an auth error, ask AuthManager to clear OAuth credentials
        _stdb.OnSTDBConnectError += HandleStdbAuthError;
    }

    private void HandleAuthSuccess()
    {
        Debug.Log("[GameManager] Auth success, connecting to STDB...");
        _stdb.InitStdbConnection();
    }

    private void HandleLogout()
    {
        Debug.Log("[GameManager] Logout, reconnecting as anonymous...");
        // InitStdbConnection handles disconnecting any existing connection internally
        // Tokens are already cleared by AuthManager, so this will connect anonymously
        _stdb.InitStdbConnection();
    }

    private void HandleStdbAuthError(Exception ex)
    {
        Debug.LogWarning($"[GameManager] STDB auth error: {ex}. Clearing OAuth credentials.");
        _authManager.ClearAllCredentials();
        // Note: STDB will automatically reconnect anonymously after clearing its token
    }

    private void Start()
    {
        // Check for OAuth callback (WebGL only - this might trigger OnAuthenticationSuccess)
        _authManager.CheckForOAuthCallback();

        // If user already has a stored SpacetimeDB token or ID token, connect now
        // (If CheckForOAuthCallback triggered auth success, this is redundant but safe)
        _stdb.InitStdbConnection();

        // Wire up renderers to their respective GameTiles
        UpdateRenderers();
    }

    private void Update()
    {
        // Keep renderers synchronized with the GameCore tiles
        // (tiles may change due to network updates, game state changes, etc.)
        UpdateRenderers();
    }

    /// <summary>
    /// Update renderers with the current GameCore tiles.
    /// This is called every frame to keep them in sync.
    /// </summary>
    private void UpdateRenderers()
    {
        if (GameTile1Renderer != null)
        {
            GameTile1Renderer.Render(GameCore?.GameTile1);
            GameTile1Renderer.PhysicsSim = GameCore?.GameTile1?.Sim;
        }
        if (GameTile2Renderer != null)
        {
            GameTile2Renderer.Render(GameCore?.GameTile2);
            GameTile2Renderer.PhysicsSim = GameCore?.GameTile2?.Sim;
        }
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        if (_authManager != null)
        {
            _authManager.OnAuthenticationSuccess -= HandleAuthSuccess;
            _authManager.OnLogout -= HandleLogout;
        }
        if (_stdb != null)
        {
            _stdb.OnSTDBConnectError -= HandleStdbAuthError;
        }
    }
}
