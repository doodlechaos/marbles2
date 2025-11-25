# Integrating AuthManager with STDB Connection

This guide shows how to integrate SpacetimeAuth authentication with your existing STDB connection manager.

## Quick Integration

Your current `STDB.cs` already uses the `AuthToken` class, which has been updated to automatically use SpacetimeAuth ID tokens when available. This means **the integration is mostly automatic**!

## How It Works

1. The `AuthToken.Token` property now checks for SpacetimeAuth ID tokens first
2. If `AuthManager` is present and user is authenticated, it returns the ID token
3. Otherwise, it falls back to the traditional SpacetimeDB token stored in PlayerPrefs
4. Your existing `STDB.cs` code continues to work without changes

## For WebGL with SpacetimeAuth

If you want to require SpacetimeAuth for WebGL builds, update your `STDB.cs` like this:

### Option 1: Require Auth for WebGL Only

```csharp
private void Awake()
{
#if UNITY_WEBGL && !UNITY_EDITOR
    // For WebGL builds, wait for SpacetimeAuth
    AuthManager authManager = FindObjectOfType<AuthManager>();
    if (authManager == null)
    {
        Debug.LogError("AuthManager required for WebGL builds!");
        return;
    }
    
    if (!authManager.IsAuthenticated())
    {
        Debug.Log("Waiting for authentication...");
        authManager.OnAuthenticationSuccess += InitializeConnection;
        return;
    }
#endif
    
    InitializeConnection();
}

private void InitializeConnection()
{
    var builder = DbConnection
        .Builder()
        .OnConnect(HandleConnect)
        .OnConnectError(HandleConnectError)
        .OnDisconnect(HandleDisconnect)
        .WithUri(SERVER_URL)
        .WithModuleName(MODULE_NAME);

    // AuthToken.Token automatically uses SpacetimeAuth ID token if available
    if (AuthToken.Token != "")
    {
        builder = builder.WithToken(AuthToken.Token);
    }

    Conn = builder.Build();
    Debug.Log("Building stdb connection");
}
```

### Option 2: Always Require SpacetimeAuth

```csharp
[SerializeField]
private AuthManager authManager;

private void Awake()
{
    if (authManager == null)
    {
        authManager = FindObjectOfType<AuthManager>();
    }
    
    if (authManager == null)
    {
        Debug.LogError("AuthManager not found! Please add it to the scene.");
        return;
    }
    
    // Subscribe to auth events
    authManager.OnAuthenticationSuccess += InitializeConnection;
    authManager.OnAuthenticationError += HandleAuthError;
    
    // If already authenticated, connect immediately
    if (authManager.IsAuthenticated())
    {
        InitializeConnection();
    }
    else
    {
        Debug.Log("Waiting for authentication...");
        // Optionally auto-start login
        // authManager.StartLogin();
    }
}

private void HandleAuthError(string error)
{
    Debug.LogError($"Authentication failed: {error}");
    // Handle auth error - show UI, retry, etc.
}

private void InitializeConnection()
{
    string idToken = authManager.GetIdToken();
    
    if (string.IsNullOrEmpty(idToken))
    {
        Debug.LogError("Cannot connect: No ID token available");
        return;
    }
    
    var builder = DbConnection
        .Builder()
        .OnConnect(HandleConnect)
        .OnConnectError(HandleConnectError)
        .OnDisconnect(HandleDisconnect)
        .WithUri(SERVER_URL)
        .WithModuleName(MODULE_NAME)
        .WithToken(idToken);  // Use SpacetimeAuth ID token
    
    Conn = builder.Build();
    Debug.Log("Building stdb connection with authenticated token");
}
```

### Option 3: No Changes (Automatic Fallback)

Keep your existing `STDB.cs` exactly as-is! The updated `AuthToken` class automatically:
- Uses SpacetimeAuth ID token when available
- Falls back to traditional token if not using SpacetimeAuth
- Works in both Editor and WebGL builds

This is the simplest approach if you want to support both authenticated and unauthenticated modes.

## Configuration for Production

When deploying to production with SpacetimeAuth:

1. Update `SERVER_URL` in `STDB.cs` to your production SpacetimeDB server
2. Make sure your SpacetimeDB module is configured to accept SpacetimeAuth tokens
3. Configure your SpacetimeAuth redirect URIs to match your production URL

### Example Production Config

```csharp
#if UNITY_WEBGL && !UNITY_EDITOR
    const string SERVER_URL = "wss://testnet.spacetimedb.com";
#else
    const string SERVER_URL = "http://127.0.0.1:3000";
#endif
```

## Handling Re-authentication

If the user's token expires or they logout, you need to handle reconnection:

```csharp
void HandleConnectError(Exception ex)
{
    Debug.LogError($"Connection error: {ex}");
    
    // Check if it's an authentication error
    if (ex.Message.Contains("unauthorized") || ex.Message.Contains("authentication"))
    {
        AuthManager authManager = FindObjectOfType<AuthManager>();
        if (authManager != null)
        {
            Debug.Log("Authentication error - redirecting to login");
            authManager.StartLogin();
        }
    }
}
```

## Testing

### Local Testing (Editor)
Your existing setup continues to work with local SpacetimeDB server.

### WebGL Testing with SpacetimeAuth
1. Build for WebGL
2. Serve from a URL configured in SpacetimeAuth
3. User will be redirected to login
4. After authentication, connection is established with ID token

## Server-Side Configuration

On your SpacetimeDB module, you'll have access to the authenticated user identity:

```csharp
[SpacetimeDB.Reducer]
public static void MyReducer(ReducerContext ctx)
{
    // ctx.Sender contains the authenticated user's identity
    // This will be the 'sub' claim from the SpacetimeAuth ID token
    var userId = ctx.Sender;
    
    Debug.Log($"Reducer called by user: {userId}");
    
    // You can use this to enforce permissions, track user actions, etc.
}
```

### Example: User-Specific Data

```csharp
[SpacetimeDB.Reducer]
public static void SaveUserData(ReducerContext ctx, string data)
{
    var userId = ctx.Sender;
    
    // Save data associated with the authenticated user
    var userData = new UserData
    {
        UserId = userId,
        Data = data,
        Timestamp = DateTime.UtcNow
    };
    
    ctx.Db.UserData.Insert(userData);
}
```

## Common Issues

### "No AuthManager found"
- Make sure you have an AuthManager component in your scene
- Add it to a persistent GameObject (don't destroy on load if needed)

### Connection works in Editor but not in WebGL
- Check that SpacetimeAuth redirect URIs are configured correctly
- Verify the CLIENT_ID is set in AuthManager
- Check browser console for JavaScript errors

### "Unauthorized" errors on connection
- Ensure your SpacetimeDB module is configured to accept SpacetimeAuth tokens
- Verify the token hasn't expired
- Check that the authority URL is correct in AuthManager

## Summary

The integration is designed to be **automatic and backwards-compatible**:

1. ✅ **No STDB.cs changes required** - AuthToken class handles everything
2. ✅ **Works in Editor** - Falls back to traditional tokens for local dev
3. ✅ **Works in WebGL** - Automatically uses SpacetimeAuth ID tokens
4. ✅ **Optional explicit control** - Use the examples above if you want more control

Just add the AuthManager to your scene and configure it - the rest happens automatically!

