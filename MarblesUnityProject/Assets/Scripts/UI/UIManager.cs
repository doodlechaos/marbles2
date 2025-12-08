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

    // Currency display elements
    private Button _marblesButton;
    private Label _marbleCountLabel;
    private Label _pointsCountLabel;

    // Profile modal elements
    private VisualElement _profileModal;
    private Button _logoutButton;

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

        // Setup Marbles button
        _marblesButton = _root.Q<Button>("btn-marbles");
        _marbleCountLabel = _root.Q<Label>("marble-count");
        _marblesButton.clicked += OnMarblesClicked;

        // Setup Points display
        _pointsCountLabel = _root.Q<Label>("points-count");

        // Setup Profile modal
        _profileModal = _root.Q<VisualElement>("profile-modal");
        _logoutButton = _root.Q<Button>("btn-logout");

        if (_logoutButton != null)
        {
            _logoutButton.clicked += OnLogoutButtonClicked;
        }

        // New: close modal when clicking on the dimmed background
        if (_profileModal != null)
        {
            // Click on background closes modal
            _profileModal.RegisterCallback<ClickEvent>(OnProfileModalBackgroundClicked);

            // Make clicks inside the card NOT close it
            var modalContent = _profileModal.Q<VisualElement>(className: "profile-modal-content");
            if (modalContent != null)
            {
                modalContent.RegisterCallback<ClickEvent>(evt =>
                {
                    evt.StopPropagation(); // prevent bubble to overlay
                });
            }
        }

        HideProfileModal();
    }

    public void SetCallbacks(DbConnection conn)
    {
        conn.Db.MySessionKind.OnInsert += OnMySessionKindInsert;
        conn.Db.MySessionKind.OnUpdate += OnMySessionKindUpdate;
        conn.Db.MySessionKind.OnDelete += OnMySessionKindDelete;

        // Subscribe to account updates for currency display
        conn.Db.MyAccount.OnInsert += OnMyAccountInsert;
        conn.Db.MyAccount.OnUpdate += OnMyAccountUpdate;

        Debug.Log("[UIManager] Callbacks registered");
    }

    public void CleanupCallbacks(DbConnection conn)
    {
        conn.Db.MySessionKind.OnInsert -= OnMySessionKindInsert;
        conn.Db.MySessionKind.OnUpdate -= OnMySessionKindUpdate;
        conn.Db.MySessionKind.OnDelete -= OnMySessionKindDelete;

        conn.Db.MyAccount.OnInsert -= OnMyAccountInsert;
        conn.Db.MyAccount.OnUpdate -= OnMyAccountUpdate;

        Debug.Log("[UIManager] Callbacks cleaned up");
    }

    // Callback method handlers for session kind
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

    // Callback method handlers for account (currency display)
    private void OnMyAccountInsert(EventContext ctx, Account account)
    {
        UpdateCurrencyDisplay(account);
    }

    private void OnMyAccountUpdate(EventContext ctx, Account oldAccount, Account newAccount)
    {
        UpdateCurrencyDisplay(newAccount);
    }

    private void UpdateCurrencyDisplay(Account account)
    {
        if (_marbleCountLabel != null)
        {
            _marbleCountLabel.text = FormatCurrencyCount(account.Marbles);
        }

        if (_pointsCountLabel != null)
        {
            _pointsCountLabel.text = FormatCurrencyCount(account.Points);
        }

        Debug.Log(
            $"[UIManager] Currency updated: {account.Marbles} marbles, {account.Points} points"
        );
    }

    private string FormatCurrencyCount(uint value)
    {
        if (value >= 1_000_000)
        {
            return $"{value / 1_000_000f:0.#}M";
        }
        else if (value >= 1_000)
        {
            return $"{value / 1_000f:0.#}K";
        }
        return value.ToString();
    }

    void OnDestroy()
    {
        // Cleanup STDB callbacks if connection still exists
        if (STDB.Conn != null)
            CleanupCallbacks(STDB.Conn);

        if (_authManager != null)
            _authManager.OnLogout -= OnLogout;

        if (_logoutButton != null)
            _logoutButton.clicked -= OnLogoutButtonClicked;
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
        Debug.Log("[UIManager] Profile button clicked - opening profile modal");
        ShowProfileModal();
    }

    private void OnLogoutButtonClicked()
    {
        Debug.Log("[UIManager] Logout button clicked from profile modal");

        if (_authManager != null)
        {
            _authManager.Logout();
        }
        else
        {
            Debug.LogError("[UIManager] AuthManager not assigned!");
        }

        HideProfileModal();
    }

    private void OnMarblesClicked()
    {
        Debug.Log("[UIManager] Marbles button clicked - TODO: Open purchase modal");
        // TODO: Open modal for purchasing marbles
    }

    private void OnLogout()
    {
        Debug.Log("[UIManager] Logout event received");
        _loadedProfilePictureUrl = null;
        HideProfileModal();
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

    private void ShowProfileModal()
    {
        if (_profileModal != null)
        {
            _profileModal.style.display = DisplayStyle.Flex;
            _profileModal.BringToFront();
        }
    }

    private void HideProfileModal()
    {
        if (_profileModal != null)
        {
            _profileModal.style.display = DisplayStyle.None;
        }
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

    private void OnProfileModalBackgroundClicked(ClickEvent evt)
    {
        Debug.Log("[UIManager] Clicked outside profile modal, closing it");
        HideProfileModal();
    }
}
