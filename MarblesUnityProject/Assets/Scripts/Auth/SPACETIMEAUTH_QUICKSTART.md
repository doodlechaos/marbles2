# SpacetimeAuth for Unity WebGL - Quick Start

Get your Unity WebGL game authenticated with SpacetimeAuth in 5 minutes!

## 📋 Prerequisites

- Unity project with SpacetimeDB integration
- Game built for WebGL
- SpacetimeAuth project created at https://spacetimedb.com/spacetimeauth

## 🚀 5-Minute Setup

### Step 1: Configure SpacetimeAuth (2 minutes)

1. Go to https://spacetimedb.com/spacetimeauth
2. Create a new project (or use existing)
3. Go to **Clients** tab
4. Copy your **Client ID** (you'll need this in Step 2)
5. Add your game URL to **Redirect URIs**:
   - Local testing: `http://localhost:8080`
   - Production: `https://yourgame.com`

### Step 2: Add AuthManager to Unity (2 minutes)

1. Open your main scene (the one with your STDB manager)
2. Create empty GameObject: **GameObject → Create Empty**
3. Name it "AuthManager"
4. Add component: **AuthManager** script
5. In Inspector, set:
   - **Client ID**: Paste from Step 1
   - **Authority**: Keep default `https://auth.spacetimedb.com/oidc`
   - **Scopes**: Keep default `openid profile email`

### Step 3: Add Login UI (1 minute)

Choose one option:

#### Option A: Auto-generated UI (Easiest)
1. Create UI Canvas: **GameObject → UI → Canvas**
2. Add component: **SimpleLoginUI** script
3. Leave "Create UI Automatically" checked
4. Done! UI will be created at runtime

#### Option B: Custom UI
1. Create your own Login/Logout buttons
2. Create a script that calls:
   ```csharp
   authManager.StartLogin();  // For login button
   authManager.Logout();       // For logout button
   ```

### Step 4: Test It! (30 seconds)

1. **File → Build Settings → WebGL**
2. Click **Build and Run**
3. Browser opens with your game
4. Click "Login" → redirects to SpacetimeAuth
5. Choose login method (Google, GitHub, etc.)
6. Redirects back to your game → You're authenticated! 🎉

## ✅ Verification

After login, check Unity console. You should see:
```
[AuthManager] Starting interactive login...
[AuthManager] Detected OAuth callback, processing...
[AuthManager] Exchanging authorization code for tokens...
[AuthManager] Authentication successful! User: user_xxxxx
```

Your STDB connection will automatically use the authenticated token!

## 📝 What Just Happened?

1. ✅ AuthManager added OAuth 2.0 + OIDC authentication
2. ✅ Your existing `STDB.cs` automatically uses the SpacetimeAuth ID token
3. ✅ User identity is passed to all SpacetimeDB reducer calls
4. ✅ Session persists across page refreshes

## 🎮 Next Steps

### Access User Info in Unity

```csharp
AuthManager authManager = FindObjectOfType<AuthManager>();

if (authManager.IsAuthenticated())
{
    Debug.Log($"User: {authManager.userProfile.name}");
    Debug.Log($"Email: {authManager.userProfile.email}");
    Debug.Log($"ID: {authManager.userProfile.sub}");
}
```

### Use User Identity in SpacetimeDB Reducers

```csharp
[SpacetimeDB.Reducer]
public static void MyGameAction(ReducerContext ctx, int score)
{
    // ctx.Sender is the authenticated user's ID (from SpacetimeAuth)
    var userId = ctx.Sender;
    
    Debug.Log($"User {userId} scored {score} points!");
    
    // Save user-specific data
    ctx.Db.UserScores.Insert(new UserScore
    {
        UserId = userId,
        Score = score
    });
}
```

### Handle Login Events

```csharp
void Start()
{
    AuthManager authManager = FindObjectOfType<AuthManager>();
    
    authManager.OnAuthenticationSuccess += () => {
        Debug.Log("Login successful!");
        // Load game, show main menu, etc.
    };
    
    authManager.OnAuthenticationError += (error) => {
        Debug.LogError($"Login failed: {error}");
        // Show error message to user
    };
    
    authManager.OnLogout += () => {
        Debug.Log("User logged out");
        // Return to login screen
    };
}
```

## 🎨 Customize Login Page

In your SpacetimeAuth dashboard:
1. Go to **Customization** tab
2. Change colors, logo, and theme
3. Enable/disable login providers (Google, GitHub, Discord, etc.)
4. Changes apply immediately!

## 🐛 Troubleshooting

### "Client ID not configured"
→ Set the Client ID in AuthManager Inspector

### Login redirects but nothing happens
→ Check that redirect URI in SpacetimeAuth matches your game URL exactly

### "State verification failed"
→ Clear browser cache and PlayerPrefs, try again

### Connection fails after authentication
→ Ensure your SpacetimeDB server accepts SpacetimeAuth tokens

### Works in Editor but not WebGL
→ Build to WebGL; some auth features only work in actual builds

## 📚 Complete Documentation

- **AUTH_SETUP_README.md** - Detailed AuthManager documentation
- **STDB_INTEGRATION_GUIDE.md** - Integration with your STDB connection
- **AuthExample.cs** - Full example with SpacetimeDB connection
- **SimpleLoginUI.cs** - Example UI implementation

## 🔐 Security Notes

- ✅ Client secret not needed (PKCE flow)
- ✅ Tokens stored securely in PlayerPrefs
- ✅ State parameter prevents CSRF
- ✅ ID token validated by SpacetimeDB
- ⚠️ Only works over HTTPS in production (not localhost)

## 🎯 Production Checklist

Before going live:

- [ ] Configure production redirect URI in SpacetimeAuth
- [ ] Set production SpacetimeDB server URL
- [ ] Test login flow on production URL
- [ ] Customize SpacetimeAuth branding
- [ ] Configure desired login providers
- [ ] Test token expiration handling
- [ ] Verify user data privacy in reducers

## 💡 Pro Tips

1. **Auto-login**: Call `authManager.StartLogin()` in Start() for automatic redirect
2. **Session persistence**: Tokens are saved; users stay logged in across refreshes
3. **Multiple environments**: Create separate SpacetimeAuth projects for dev/staging/prod
4. **Custom claims**: Request additional scopes for more user data
5. **Role-based access**: Use SpacetimeAuth roles for permissions

## 🆘 Need Help?

- SpacetimeDB Discord: https://discord.gg/spacetimedb
- SpacetimeAuth Docs: https://spacetimedb.com/docs/spacetimeauth
- GitHub Issues: https://github.com/clockworklabs/SpacetimeDB/issues

## 📄 License

This integration code is provided as part of your SpacetimeDB project.

---

**You're all set!** Your Unity WebGL game now has enterprise-grade authentication. 🎉

