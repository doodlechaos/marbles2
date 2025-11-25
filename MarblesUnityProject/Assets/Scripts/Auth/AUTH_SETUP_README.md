# SpacetimeAuth Integration for Unity WebGL

This guide explains how to use the `AuthManager` to integrate SpacetimeAuth OAuth authentication into your Unity WebGL game.

## Overview

The `AuthManager` provides a complete OAuth 2.0 + OpenID Connect (OIDC) authentication flow using the Authorization Code flow with PKCE (Proof Key for Code Exchange). This is the recommended approach for browser-based applications as it doesn't require a client secret.

## Files Included

- **AuthManager.cs** - Main authentication manager component
- **AuthPlugin.jslib** - JavaScript plugin for WebGL browser integration
- **AuthExample.cs** - Example script showing how to use AuthManager with SpacetimeDB

## Setup Instructions

### 1. Create a SpacetimeAuth Project

1. Go to https://spacetimedb.com/spacetimeauth
2. Click "New Project" to create a new project
3. Navigate to the "Clients" tab
4. Copy your **Client ID** (you'll need this in step 3)

### 2. Configure Redirect URIs

In your SpacetimeAuth client settings, add your game's URL as an allowed redirect URI:

- For local development: `http://localhost:8080` (or whatever port you use)
- For production: `https://yourgame.com` (your actual game URL)

**Important:** The redirect URI must exactly match where your WebGL build is hosted. The AuthManager automatically uses the current page URL as the redirect URI.

### 3. Configure AuthManager in Unity

1. Create an empty GameObject in your scene (e.g., "AuthManager")
2. Add the `AuthManager` component to it
3. In the Inspector, set:
   - **Client ID**: Your SpacetimeAuth client ID
   - **Authority**: `https://auth.spacetimedb.com/oidc` (default, usually don't change)
   - **Scopes**: `openid profile email` (default, can customize)

### 4. Use AuthManager in Your Code

#### Basic Usage

```csharp
public class MyGameManager : MonoBehaviour
{
    private AuthManager authManager;
    
    void Start()
    {
        authManager = FindObjectOfType<AuthManager>();
        
        // Subscribe to events
        authManager.OnAuthenticationSuccess += OnAuthSuccess;
        authManager.OnAuthenticationError += OnAuthError;
        
        // Check if already authenticated
        if (!authManager.IsAuthenticated())
        {
            // Show login UI or automatically redirect
            authManager.StartLogin();
        }
        else
        {
            // User is already logged in
            ConnectToSpacetimeDB();
        }
    }
    
    void OnAuthSuccess()
    {
        Debug.Log("User logged in!");
        ConnectToSpacetimeDB();
    }
    
    void OnAuthError(string error)
    {
        Debug.LogError($"Auth failed: {error}");
    }
    
    void ConnectToSpacetimeDB()
    {
        string idToken = authManager.GetIdToken();
        
        // Use the ID token with SpacetimeDB
        DbConnection.Builder()
            .WithUri("wss://testnet.spacetimedb.com")
            .WithModuleName("your-database")
            .WithToken(idToken)  // Pass the ID token here!
            .Build()
            .Connect();
    }
}
```

#### Accessing User Information

```csharp
if (authManager.IsAuthenticated())
{
    var profile = authManager.userProfile;
    Debug.Log($"Welcome {profile.name}!");
    Debug.Log($"Email: {profile.email}");
    Debug.Log($"User ID: {profile.sub}");
    Debug.Log($"Username: {profile.preferred_username}");
}
```

#### Logout

```csharp
authManager.Logout();
```

### 5. Testing in Unity Editor

The AuthManager includes fallback behavior for testing in the Unity Editor:
- It will use `http://localhost:8080` as the redirect URI
- Login will open in your default browser instead of redirecting the window
- You'll need to manually copy callback parameters back (not ideal for testing)

**Recommendation:** Test authentication in an actual WebGL build for the best experience.

### 6. Build for WebGL

1. Go to **File > Build Settings**
2. Select **WebGL** platform
3. Click **Build and Run**
4. Make sure your build is served from the same URL you configured in SpacetimeAuth redirect URIs

## How It Works

### Authentication Flow

1. User clicks login → `authManager.StartLogin()` is called
2. AuthManager generates PKCE challenge and state (for security)
3. Browser redirects to SpacetimeAuth login page
4. User logs in with their chosen provider (Google, GitHub, Discord, etc.)
5. SpacetimeAuth redirects back to your game with an authorization code
6. AuthManager automatically detects the callback and exchanges code for tokens
7. ID token is stored and made available to your game
8. Your game connects to SpacetimeDB using the ID token

### Security Features

- **PKCE (Proof Key for Code Exchange)**: Protects against authorization code interception
- **State Parameter**: Prevents CSRF attacks
- **Token Storage**: Tokens are persisted in PlayerPrefs for session continuity
- **No Client Secret**: Safe for browser environments

## SpacetimeDB Integration

The ID token from SpacetimeAuth is used to authenticate with your SpacetimeDB module:

```csharp
DbConnection.Builder()
    .WithToken(authManager.GetIdToken())
    .Build();
```

### Accessing User Identity in Reducers

In your SpacetimeDB module reducers, you can access the authenticated user's identity:

```csharp
[SpacetimeDB.Reducer]
public static void MyReducer(ReducerContext ctx)
{
    // ctx.Sender contains the user's identity (sub claim from the ID token)
    var userId = ctx.Sender;
    
    // You can also access claims from the token
    // The identity will match the "sub" claim from the ID token
}
```

## API Reference

### AuthManager Methods

- `void StartLogin()` - Initiates the OAuth login flow
- `void Logout()` - Logs out the user and clears session
- `string GetIdToken()` - Returns the current ID token (for SpacetimeDB)
- `bool IsAuthenticated()` - Returns true if user is authenticated

### AuthManager Properties

- `bool isAuthenticated` - Current authentication state
- `string idToken` - The ID token (if authenticated)
- `UserProfile userProfile` - User information from ID token

### AuthManager Events

- `event Action OnAuthenticationSuccess` - Fired when login succeeds
- `event Action<string> OnAuthenticationError` - Fired when login fails
- `event Action OnLogout` - Fired when user logs out

### UserProfile Class

```csharp
public class UserProfile
{
    public string sub;                  // Unique user ID
    public string email;                // User's email
    public string name;                 // Full name
    public string preferred_username;   // Username
    public string picture;              // Profile picture URL
}
```

## Common Issues

### "Client ID not configured"
- Make sure you've set the Client ID in the AuthManager Inspector

### "State verification failed"
- This usually means the authentication flow was interrupted
- Try clearing PlayerPrefs and logging in again
- Check browser console for errors

### "No authorization code found in callback"
- Check that your redirect URI is correctly configured in SpacetimeAuth
- Verify the URL matches exactly (including protocol, port, and path)

### Tokens not persisting between sessions
- PlayerPrefs should automatically persist tokens
- For WebGL, make sure browser cookies/storage isn't disabled

### SpacetimeDB connection fails with authenticated token
- Verify your SpacetimeDB module is configured to accept SpacetimeAuth tokens
- Check that the token isn't expired
- Look at SpacetimeDB server logs for specific error messages

## Example Project Structure

```
YourUnityProject/
├── Assets/
│   ├── Scripts/
│   │   ├── AuthManager.cs           ← Main auth component
│   │   ├── AuthExample.cs           ← Usage example
│   │   └── AUTH_SETUP_README.md     ← This file
│   └── Plugins/
│       └── WebGL/
│           └── AuthPlugin.jslib     ← Browser integration
```

## Advanced Usage

### Customizing Scopes

You can request different scopes based on your needs:

```csharp
authManager.scopes = "openid profile email roles";
```

Available scopes:
- `openid` (required) - Basic user ID
- `profile` - Name, username, picture, etc.
- `email` - Email address
- Custom scopes defined in your SpacetimeAuth project

### Token Expiration

Currently, the AuthManager doesn't automatically handle token expiration. You should:

1. Monitor SpacetimeDB connection errors
2. If you get an authentication error, call `authManager.StartLogin()` again
3. Or implement token refresh logic (requires refresh tokens from token endpoint)

### Multiple Authentication Providers

SpacetimeAuth supports multiple identity providers (Google, GitHub, Discord, Twitch, Kick). Users choose their provider on the SpacetimeAuth login page. Your game doesn't need to do anything special - just call `StartLogin()`.

## Support

For issues with:
- **SpacetimeAuth**: Report at https://github.com/clockworklabs/SpacetimeDB/issues
- **This integration**: Check Unity console logs and browser console for errors

## License

This code is provided as part of your SpacetimeDB integration.

