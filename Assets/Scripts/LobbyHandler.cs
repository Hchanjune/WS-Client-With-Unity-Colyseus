using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    private Player _player; 
    private Button _logoutBtn;
    private Button _chatRoomCreateBtn;
    private TMP_Text _usernameText;
    private TMP_Text _currentSessionIdText;

    public void Start()
    {
        // Initialize Player
        
        
        // Btn
        _logoutBtn = GameObject.Find("LogoutBtn").GetComponent<Button>();
        _logoutBtn.onClick.AddListener(LogOut);
        _chatRoomCreateBtn = GameObject.Find("BtnChatRoomCreate").GetComponent<Button>();
        _chatRoomCreateBtn.onClick.AddListener(CreateNewChatRoom);
        
        // Text
        _usernameText = GameObject.Find("TextUsername").GetComponent<TMP_Text>();
        _usernameText.text = NetworkManager.Instance.CurrentPlayerId;
        _currentSessionIdText = GameObject.Find("TextCurrentSessionId").GetComponent<TMP_Text>();
        _currentSessionIdText.text = NetworkManager.Instance.Lobby.SessionId;

        
        
    }
    
    private void LogOut()
    {
        NetworkManager.Instance.DisconnectFromRoom();
        SceneManager.LoadScene("Login");
    }

    private async void CreateNewChatRoom()
    {
        bool process = await NetworkManager.Instance.CreateChatRoom("New Chat Room");
        if (process)
        {
            Debug.Log("ChatRoom SuccessFully Created");
        }
        else
        {
            
        }
    }
    
}