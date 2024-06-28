using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatRoomHandler : MonoBehaviour
{

    private Button _exitBtn;

    private TMP_Text _chatRoomInfoText;
    
    private GameObject _chatAreaScrollView;
    private Transform  _chatAreaContent;
    private GameObject _chatTextPrefab;
    
    private GameObject _userAreaScrollView;
    private Transform  _userAreaContent;
    private GameObject _userTextPrefab;


    private void Start()
    {
        
        // Btn
        _exitBtn = GameObject.Find("BtnExit").GetComponent<Button>();
        _exitBtn.onClick.AddListener(OnExit);
        
        // Text
        _chatRoomInfoText = GameObject.Find("TextChatRoomInfo").GetComponent<TMP_Text>();
        
    }

    private void Update()
    {
        _chatRoomInfoText.text = $"[{NetworkManager.Instance.ChatRoom.State.roomOwner}] {NetworkManager.Instance.ChatRoom.State.roomName}";
    }
    
    private void OnExit()
    {
        NetworkManager.Instance.DisConnectChatRoom();
        SceneManager.LoadSceneAsync("Lobby");
        NetworkManager.Instance.Reinitialize();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
