using System;
using System.Collections.Generic;
using Colyseus.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatRoomHandler : MonoBehaviour
{
    public static ChatRoomHandler _instance;

    private Button _exitBtn;

    private TMP_Text _chatRoomInfoText;
    
    private GameObject _chatAreaScrollView;
    private Transform  _chatAreaContent;
    private GameObject _chatTextPrefab;
    
    private GameObject _userAreaScrollView;
    private Transform  _userAreaContent;
    private GameObject _userTextPrefab;

    private TMP_InputField _chattingTextInput;
    private Button _submitBtn;

    private void InitializeUIElements()
    {
        // Btn
        _exitBtn = GameObject.Find("BtnExit").GetComponent<Button>();
        _exitBtn.onClick.AddListener(OnExit);
        
        // Text
        _chatRoomInfoText = GameObject.Find("TextChatRoomInfo").GetComponent<TMP_Text>();
        
        // Chatting Area
        _chatAreaScrollView = GameObject.Find("ChatAreaScrollView");
        _chatAreaContent = _chatAreaScrollView?.transform.Find("Viewport/Content");
        _chatTextPrefab = Resources.Load<GameObject>("ChatMessagePrefab");
        
        // User List
        _userAreaScrollView = GameObject.Find("UserListScrollView");
        _userAreaContent = _userAreaScrollView?.transform.Find("Viewport/Content");
        _userTextPrefab = Resources.Load<GameObject>("UserListPrefab");
        
        // Chatting Text Input
        _chattingTextInput = GameObject.Find("InputChatText").GetComponent<TMP_InputField>();
        _chattingTextInput.onSubmit.AddListener(SendChatMessage);
        // SubmitBtn
        _submitBtn = GameObject.Find("BtnSubmitMessage").GetComponent<Button>();
        _submitBtn.onClick.AddListener(SendChatMessage);
    }
    
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
        if (NetworkManager.Instance.ChatRoom != null)
        {
            UpdateChatRoomState(NetworkManager.Instance.ChatRoom.State);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ChatRoom")
        {
            InitializeUIElements();
            if (NetworkManager.Instance.Lobby != null)
            {
                UpdateChatRoomState(NetworkManager.Instance.ChatRoom.State);
            }
        }
    }
    
    public static void OnStateChange(ChatRoomState state, bool isFirstState)
    {
        _instance.UpdateChatRoomState(state);
    }

    private void UpdateChatRoomState(ChatRoomState state)
    {
        UpdateChatRoomInfo(state);
        UpdateChatRoomUsers(state.players);
        NetworkManager.Instance.ChatRoom.OnMessage<Dictionary<string, object>>("CHAT_ECHO", OnReceivingMessage);
    }

    private void UpdateChatRoomInfo(ChatRoomState state)
    {
        _chatRoomInfoText.text = $"[{state.roomOwner}] {state.roomName}";
    }

    private void UpdateChatRoomUsers(MapSchema<Player> players)
    {
        if (_userAreaContent == null) return;

        foreach (Transform child in _userAreaContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Player player in players.Values)
        {
            GameObject chatRoomItem = Instantiate(_userTextPrefab, _userAreaContent);
            TMP_Text chatRoomText = chatRoomItem.GetComponentInChildren<TMP_Text>();
            chatRoomText.text = $"{player.id}";
        }
        
    }
    
    private void OnExit()
    {
        NetworkManager.Instance.DisConnectChatRoom();
        SceneManager.LoadSceneAsync("Lobby");
        NetworkManager.Instance.Reinitialize();
    }
    
    
    private void SendChatMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            var chatMessage = new Dictionary<string, object>
            {
                { "id", NetworkManager.Instance.CurrentPlayerId },
                { "message", message }
            };
            NetworkManager.Instance.ChatRoom.Send("CHAT_MESSAGE" ,chatMessage);
            _chattingTextInput.text = ""; // Clear the input field after sending the message
        }
    }
    
    private void SendChatMessage()
    {
        if (_chattingTextInput.text != null || _chattingTextInput.text != "")
        {
            var chatMessage = new Dictionary<string, object>
            {
                { "id", NetworkManager.Instance.CurrentPlayerId },
                { "message", _chattingTextInput.text }
            };
            NetworkManager.Instance.ChatRoom.Send("CHAT_MESSAGE", chatMessage);
            _chattingTextInput.text = ""; // Clear the input field after sending the message
        }
    }

    private void OnReceivingMessage(Dictionary<string, object> message)
    {
        message.TryGetValue("id", out object id);
        message.TryGetValue("message", out object chat);
        Debug.Log($"{id} : {chat}");
        GameObject chatList = Instantiate(_chatTextPrefab, _chatAreaContent);
        TMP_Text messenger = chatList.transform.Find("ID").GetComponent<TMP_Text>();
        TMP_Text chatMessage = chatList.transform.Find("Message").GetComponent<TMP_Text>();

        messenger.text = id + " : ";
        chatMessage.text = chat.ToString();

    }
    
    
    
}
