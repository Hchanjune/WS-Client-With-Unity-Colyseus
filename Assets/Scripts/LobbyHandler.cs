using System;
using Colyseus.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    private static LobbyHandler _instance;

    private Player _player;
    private Button _logoutBtn;
    private Button _chatRoomCreateBtn;

    private TMP_Text _usernameText;
    private TMP_Text _currentSessionIdText;
    private TMP_Text _currentUserCountText;

    private GameObject _chatRoomListScrollView;
    private Transform _chatRoomListScrollContent;
    private GameObject _chatRoomItemButtonPrefab;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        InitializeUIElements();
        if (NetworkManager.Instance.Lobby != null)
        {
            UpdateLobbyState(NetworkManager.Instance.Lobby.State);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            InitializeUIElements();
            if (NetworkManager.Instance.Lobby != null)
            {
                UpdateLobbyState(NetworkManager.Instance.Lobby.State);
            }
        }
    }

    private void InitializeUIElements()
    {
        // Btn
        _logoutBtn = GameObject.Find("LogoutBtn")?.GetComponent<Button>();
        _chatRoomCreateBtn = GameObject.Find("BtnChatRoomCreate")?.GetComponent<Button>();

        if (_logoutBtn != null)
            _logoutBtn.onClick.AddListener(LogOut);

        if (_chatRoomCreateBtn != null)
            _chatRoomCreateBtn.onClick.AddListener(CreateNewChatRoom);

        // Text
        _usernameText = GameObject.Find("TextUsername")?.GetComponent<TMP_Text>();
        _currentSessionIdText = GameObject.Find("TextSessionID")?.GetComponent<TMP_Text>();
        _currentUserCountText = GameObject.Find("TextCurrentUserCount")?.GetComponent<TMP_Text>();

        if (_usernameText != null)
            _usernameText.text = NetworkManager.Instance.CurrentPlayerId;

        if (_currentSessionIdText != null)
            _currentSessionIdText.text = NetworkManager.Instance.Lobby.SessionId;

        // View
        _chatRoomListScrollView = GameObject.Find("ChatRoomListScrollView");
        _chatRoomListScrollContent = _chatRoomListScrollView?.transform.Find("Viewport/Content");

        // Prefab
        _chatRoomItemButtonPrefab = Resources.Load<GameObject>("ChatRoomItemButtonPrefab");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LogOut()
    {
        NetworkManager.Instance.DisconnectAll();
        SceneManager.LoadScene("Login");
    }

    private async void CreateNewChatRoom()
    {
        bool process = await NetworkManager.Instance.CreateChatRoom("New Chat Room");
        if (process)
        {
            Debug.Log("ChatRoom Successfully Created");
        }
        else
        {
            Debug.LogError("Failed to create chat room");
        }
    }

    public static void OnStateChange(LobbyState state, bool isFirstState)
    {
        _instance.UpdateLobbyState(state);
    }

    private void UpdateLobbyState(LobbyState state)
    {
        UpdateCurrentUserCount(state.players);
        UpdateChatRoomList(state.chatRoomList);
    }

    private void UpdateCurrentUserCount(MapSchema<Player> players)
    {
        if (_currentUserCountText != null)
            _currentUserCountText.text = players.Count.ToString();
    }

    private void UpdateChatRoomList(MapSchema<LobbyChatRoomListState> chatRoomList)
    {
        if (_chatRoomListScrollContent == null)
            return;

        foreach (Transform child in _chatRoomListScrollContent)
        {
            Destroy(child.gameObject);
        }

        foreach (LobbyChatRoomListState chatRoom in chatRoomList.Values)
        {
            GameObject chatRoomItem = Instantiate(_chatRoomItemButtonPrefab, _chatRoomListScrollContent);
            Button chatRoomButton = chatRoomItem.GetComponent<Button>();
            TMP_Text chatRoomText = chatRoomItem.GetComponentInChildren<TMP_Text>();
            chatRoomText.text = $"[{chatRoom.roomOwner}] - {chatRoom.roomName}";

            chatRoomButton.onClick.AddListener(() =>
            {
                Debug.Log($"Button for room {chatRoom.roomId} clicked");
                NetworkManager.Instance.JoinChatRoom(chatRoom.roomId);
            });
        }
    }
}
