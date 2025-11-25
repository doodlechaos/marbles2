/* using UnityEngine;
using UnityEngine.UI;
using SpacetimeDB;

/// <summary>
/// Example script showing how to use AuthManager with SpacetimeDB
/// Attach this to a GameObject in your scene along with AuthManager
/// </summary>
public class AuthExample : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AuthManager authManager;
    
    [Header("UI Elements (Optional)")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text userInfoText;
    
    [Header("SpacetimeDB Settings")]
    [SerializeField] private string spacetimeDbHost = "wss://testnet.spacetimedb.com";
    [SerializeField] private string spacetimeDbName = "your-database-name";
    
    private DbConnection dbConnection;

    void Start()
    {
        // Find AuthManager if not assigned
        if (authManager == null)
        {
            authManager = FindObjectOfType<AuthManager>();
        }
        
        if (authManager == null)
        {
            Debug.LogError("[AuthExample] AuthManager not found! Please add it to the scene.");
            return;
        }
        
        // Subscribe to auth events
        authManager.OnAuthenticationSuccess += HandleAuthSuccess;
        authManager.OnAuthenticationError += HandleAuthError;
        authManager.OnLogout += HandleLogout;
        
        // Setup UI buttons
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }
        
        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(OnLogoutClicked);
        }
        
        // Update UI based on current auth state
        UpdateUI();
        
        // If already authenticated, connect to SpacetimeDB
        if (authManager.IsAuthenticated())
        {
            ConnectToSpacetimeDB();
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (authManager != null)
        {
            authManager.OnAuthenticationSuccess -= HandleAuthSuccess;
            authManager.OnAuthenticationError -= HandleAuthError;
            authManager.OnLogout -= HandleLogout;
        }
        
        // Disconnect from SpacetimeDB
        DisconnectFromSpacetimeDB();
    }

    private void OnLoginClicked()
    {
        Debug.Log("[AuthExample] Login button clicked");
        authManager.StartLogin();
    }

    private void OnLogoutClicked()
    {
        Debug.Log("[AuthExample] Logout button clicked");
        
        // Disconnect from SpacetimeDB before logging out
        DisconnectFromSpacetimeDB();
        
        authManager.Logout();
    }

    private void HandleAuthSuccess()
    {
        Debug.Log("[AuthExample] Authentication successful!");
        UpdateUI();
        ConnectToSpacetimeDB();
    }

    private void HandleAuthError(string error)
    {
        Debug.LogError($"[AuthExample] Authentication error: {error}");
        UpdateUI();
        
        if (statusText != null)
        {
            statusText.text = $"Auth Error: {error}";
        }
    }

    private void HandleLogout()
    {
        Debug.Log("[AuthExample] Logged out");
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool isAuthenticated = authManager.IsAuthenticated();
        
        if (loginButton != null)
        {
            loginButton.gameObject.SetActive(!isAuthenticated);
        }
        
        if (logoutButton != null)
        {
            logoutButton.gameObject.SetActive(isAuthenticated);
        }
        
        if (statusText != null)
        {
            statusText.text = isAuthenticated ? "Authenticated" : "Not Authenticated";
        }
        
        if (userInfoText != null && isAuthenticated && authManager.userProfile != null)
        {
            var profile = authManager.userProfile;
            userInfoText.text = $"User: {profile.name ?? profile.preferred_username ?? profile.email}\nID: {profile.sub}";
        }
        else if (userInfoText != null)
        {
            userInfoText.text = "Not logged in";
        }
    }

    private void ConnectToSpacetimeDB()
    {
        string idToken = authManager.GetIdToken();
        
        if (string.IsNullOrEmpty(idToken))
        {
            Debug.LogError("[AuthExample] Cannot connect to SpacetimeDB: No ID token");
            return;
        }
        
        Debug.Log("[AuthExample] Connecting to SpacetimeDB with authenticated token...");
        
        try
        {
            // Create connection to SpacetimeDB using the ID token
            dbConnection = DbConnection.Builder()
                .WithUri(spacetimeDbHost)
                .WithModuleName(spacetimeDbName)
                .WithToken(idToken) // Pass the ID token here
                .OnConnect(OnSpacetimeDbConnected)
                .OnConnectError(OnSpacetimeDbConnectError)
                .OnDisconnect(OnSpacetimeDbDisconnected)
                .Build();
            
            dbConnection.Connect();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[AuthExample] Failed to create SpacetimeDB connection: {ex.Message}");
        }
    }

    private void DisconnectFromSpacetimeDB()
    {
        if (dbConnection != null)
        {
            Debug.Log("[AuthExample] Disconnecting from SpacetimeDB...");
            dbConnection.Disconnect();
            dbConnection = null;
        }
    }

    private void OnSpacetimeDbConnected(DbConnection conn, string identity, string token)
    {
        Debug.Log($"[AuthExample] Connected to SpacetimeDB! Identity: {identity}");
        
        if (statusText != null)
        {
            statusText.text = "Connected to SpacetimeDB";
        }
    }

    private void OnSpacetimeDbConnectError(DbConnection conn, string error)
    {
        Debug.LogError($"[AuthExample] SpacetimeDB connection error: {error}");
        
        if (statusText != null)
        {
            statusText.text = $"DB Connection Error: {error}";
        }
    }

    private void OnSpacetimeDbDisconnected(DbConnection conn)
    {
        Debug.Log("[AuthExample] Disconnected from SpacetimeDB");
        
        if (statusText != null && !authManager.IsAuthenticated())
        {
            statusText.text = "Disconnected";
        }
    }
}

 */