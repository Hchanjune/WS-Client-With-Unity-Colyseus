using System;
using System.Collections;
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

    private Button _readyBtn;

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
        
        // ReadyBtn
        _readyBtn = GameObject.Find("BtnReady").GetComponent<Button>();
        _readyBtn.interactable = true;
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
            NetworkManager.Instance.ChatRoom.OnMessage<Dictionary<string, object>>("START", OnStart);
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
        UpdateChatRoomUsers(state.chatRoomPlayers);
        NetworkManager.Instance.ChatRoom.OnMessage<Dictionary<string, object>>("CHAT_ECHO", OnReceivingMessage);
        NetworkManager.Instance.ChatRoom.OnMessage<Dictionary<string, object>>("PLAYER_NOT_READY", OnPlayerNotReady);
    }

    private void UpdateChatRoomInfo(ChatRoomState state)
    {
        _chatRoomInfoText.text = $"[{state.roomOwner}] {state.roomName}";
    }
    

    private void UpdateChatRoomUsers(MapSchema<ChatRoomPlayer> players)
    {
        if (_userAreaContent == null) return;

        foreach (Transform child in _userAreaContent)
        {
            Destroy(child.gameObject);
        }
        
        // Usernames
        foreach (ChatRoomPlayer player in players.Values)
        {
            GameObject chatRoomUserItem = Instantiate(_userTextPrefab, _userAreaContent);
            TMP_Text chatRoomUserName = chatRoomUserItem.GetComponentInChildren<TMP_Text>();
            if (player.isOwner)
            {
                chatRoomUserName.text = $"[Owner]{player.id}";
                chatRoomUserName.color = Color.yellow;
            }
            else
            {
                chatRoomUserName.text = $"{player.id}";
                chatRoomUserName.color = player.isReady ? Color.green : Color.blue;
            }

            if (player.id.Equals(NetworkManager.Instance.CurrentPlayerId))
            {
                ChatRoomPlayer currentPlayer = player;
                // ReadyBtn
                if (currentPlayer.isOwner)
                {
                    _readyBtn.GetComponentInChildren<TMP_Text>().text = "시작";
                    _readyBtn.onClick.RemoveAllListeners();
                    _readyBtn.onClick.AddListener(StartRequest);
                    
                }
                else
                {
                    _readyBtn.GetComponentInChildren<TMP_Text>().text = currentPlayer.isReady ? "준비 취소" : "준비";
                    _readyBtn.onClick.RemoveAllListeners();
                    _readyBtn.onClick.AddListener(() => OnReady(currentPlayer.isReady));
                }
                _readyBtn.interactable = true;
            }
        }
    }
    
    private void OnExit()
    {
        NetworkManager.Instance.DisConnectChatRoom();
        SceneManager.LoadSceneAsync("Lobby");
        NetworkManager.Instance.Reinitialize();
    }

    private void OnReady(bool currentReadyState)
    {
        NetworkManager.Instance.ChatRoom.Send("PLAYER_READY_STATUS_CHANGE", new {
            id = NetworkManager.Instance.CurrentPlayerId,
            isReady = !currentReadyState
        });
    }

    private void OnPlayerNotReady(Dictionary<string, object> message)
    {
        _readyBtn.interactable = true;
    }

    private void StartRequest()
    {
        NetworkManager.Instance.ChatRoom.Send("START_REQUEST");
        _readyBtn.interactable = false;
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
            _chattingTextInput.Select();
            _chattingTextInput.ActivateInputField();
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
            _chattingTextInput.Select();
            _chattingTextInput.ActivateInputField();
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

        StartCoroutine(ScrollToBottom(_chatAreaScrollView.GetComponent<ScrollRect>()));
    }

    private void OnStart(Dictionary<string, object> message)
    {
        SceneManager.LoadScene("PlayGround");
    }

    private IEnumerator ScrollToBottom(ScrollRect scrollRect)
    {
        // Wait for the end of the frame to ensure the new message is added and the canvas is updated
        yield return new WaitForEndOfFrame();
        
        // Force the canvas to update
        Canvas.ForceUpdateCanvases();
        
        // Set the scroll position to the bottom
        scrollRect.verticalNormalizedPosition = 0;
    }
    
    
    
}
