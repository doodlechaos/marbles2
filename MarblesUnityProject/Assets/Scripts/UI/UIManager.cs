using System.Collections;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField]
    private AuthManager _authManager;

    private Button _loginButton;
    private Button _profileButton;
    private Image _profilePicture;
    private VisualElement _root;

    private string _loadedProfilePictureUrl = null;

    void Awake()
    {
        _root = _uiDocument.rootVisualElement;

        // Setup Discord button
        var discordButton = _root.Q<Button>("btn-discord");
        discordButton.clicked += () =>
        {
            Application.OpenURL("https://discord.gg/A2nZCk2FtA");
        };

        // Setup Login button
        _loginButton = _root.Q<Button>("btn-login");
        _loginButton.clicked += OnLoginClicked;

        // Setup Profile button
        _profileButton = _root.Q<Button>("btn-profile");
        _profilePicture = _root.Q<Image>("profile-picture");
        _profileButton.clicked += OnProfileClicked;

        // Subscribe to auth logout event to clear profile picture
        if (_authManager != null)
        {
            _authManager.OnLogout += OnLogout;
        }

        // Initial state - show login until we hear from server
        ShowLoginButton();
    }

    public void SetCallbacks(DbConnection conn)
    {
        conn.Db.MySessionKind.OnInsert += OnMySessionKindInsert;
        conn.Db.MySessionKind.OnUpdate += OnMySessionKindUpdate;
        conn.Db.MySessionKind.OnDelete += OnMySessionKindDelete;
        Debug.Log("[UIManager] Callbacks registered");
    }

    public void CleanupCallbacks(DbConnection conn)
    {
        conn.Db.MySessionKind.OnInsert -= OnMySessionKindInsert;
        conn.Db.MySessionKind.OnUpdate -= OnMySessionKindUpdate;
        conn.Db.MySessionKind.OnDelete -= OnMySessionKindDelete;
        Debug.Log("[UIManager] Callbacks cleaned up");
    }

    // Callback method handlers
    private void OnMySessionKindInsert(EventContext ctx, MySessionKindContainer mySessionKind)
    {
        UpdateAuthUI(mySessionKind.Kind);
    }

    private void OnMySessionKindUpdate(
        EventContext ctx,
        MySessionKindContainer oldMySessionKind,
        MySessionKindContainer newMySessionKind
    )
    {
        UpdateAuthUI(newMySessionKind.Kind);
    }

    private void OnMySessionKindDelete(EventContext ctx, MySessionKindContainer mySessionKind)
    {
        UpdateAuthUI(mySessionKind.Kind);
    }

    void OnDestroy()
    {
        // Cleanup STDB callbacks if connection still exists
        if (STDB.Conn != null)
            CleanupCallbacks(STDB.Conn);

        if (_authManager != null)
            _authManager.OnLogout -= OnLogout;
    }

    private void OnLoginClicked()
    {
        Debug.Log("[UIManager] Login button clicked");
        if (_authManager != null)
        {
            _authManager.StartLogin();
        }
        else
        {
            Debug.LogError("[UIManager] AuthManager not assigned!");
        }
    }

    private void OnProfileClicked()
    {
        Debug.Log("[UIManager] Profile button clicked");
        // TODO: Open profile menu/dropdown
        // For now, just logout for testing
        if (_authManager != null)
        {
            _authManager.Logout();
        }
    }

    private void OnLogout()
    {
        Debug.Log("[UIManager] Logout event received");
        _loadedProfilePictureUrl = null;
        ShowLoginButton();
    }

    private void UpdateAuthUI(SessionKind kind)
    {
        if (kind == SessionKind.SpacetimeAuth)
        {
            Debug.Log("[UIManager] User authenticated via SpacetimeAuth, showing profile button");
            ShowProfileButton();

            // Load profile picture from OAuth profile if available
            string pictureUrl = _authManager?.userProfile?.picture;
            if (!string.IsNullOrEmpty(pictureUrl) && pictureUrl != _loadedProfilePictureUrl)
            {
                StartCoroutine(LoadProfilePicture(pictureUrl));
            }
        }
        else
        {
            Debug.Log($"[UIManager] Session kind is {kind}, showing login button");
            ShowLoginButton();
        }
    }

    private void ShowLoginButton()
    {
        _loginButton.style.display = DisplayStyle.Flex;
        _profileButton.style.display = DisplayStyle.None;
    }

    private void ShowProfileButton()
    {
        _loginButton.style.display = DisplayStyle.None;
        _profileButton.style.display = DisplayStyle.Flex;
    }

    private IEnumerator LoadProfilePicture(string url)
    {
        Debug.Log($"[UIManager] Loading profile picture from: {url}");

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                _profilePicture.image = texture;
                _loadedProfilePictureUrl = url;
                Debug.Log("[UIManager] Profile picture loaded successfully");
            }
            else
            {
                Debug.LogError($"[UIManager] Failed to load profile picture: {request.error}");
                // Keep showing profile button but without image
            }
        }
    }
}
