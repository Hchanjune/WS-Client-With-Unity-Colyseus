using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{
    private Dictionary<string, Button> _buttons;
    private TMP_InputField _usernameInput;
    private TMP_InputField _passwordInput;
    private Button _loginBtn;
    private Button _logoutBtn;
    private Button _quitBtn;
    private TMP_Text _statusMessage;

    private void Awake()
    {
        Application.runInBackground = true;
        
        _buttons = new Dictionary<string, Button>();
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            _buttons[button.name] = button;
            button.onClick.AddListener(() => OnButtonClicked(button.name));
        }

        // InputField
        _usernameInput = GameObject.Find("InputUsername").GetComponent<TMP_InputField>();
        _usernameInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Username";
        _passwordInput = GameObject.Find("InputPassword").GetComponent<TMP_InputField>();
        _passwordInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Password";
        _passwordInput.contentType = TMP_InputField.ContentType.Password;
        
        // Buttons
        _loginBtn = GetButtonByName("LoginBtn");
        _logoutBtn = GetButtonByName("LogoutBtn");
        _quitBtn = GetButtonByName("QuitBtn");
        
        // Status Message Text
        _statusMessage = GameObject.Find("StatusMessage").GetComponent<TMP_Text>();

        // Debug Log
        if (_usernameInput == null) Debug.LogError("InputUsername TMP_InputField not found!");
        if (_loginBtn == null) Debug.LogError("LoginBtn Button not found!");
        if (_logoutBtn == null) Debug.LogError("LogoutBtn Button not found!");
        
        // Initial View
        _loginBtn.gameObject.SetActive(true);
        _logoutBtn.gameObject.SetActive(false);
        _statusMessage.gameObject.SetActive(false);
    }

    private void OnButtonClicked(string buttonName)
    {
        //Debug.Log(buttonName + " clicked!");
        switch (buttonName)
        {
            case "LoginBtn":
                HandleLogin();
                break;
            case "LogoutBtn":
                HandleLogout();
                break;
            case "QuitBtn":
                HandleQuit();
                break;
        }
    }

    private Button GetButtonByName(string name)
    {
        if (_buttons.TryGetValue(name, out Button button))
        {
            return button;
        }
        else
        {
            Debug.LogWarning("Button with name " + name + " not found");
            return null;
        }
    }

    private async void HandleLogin()
    {
        // 입력된 아이디 가져오기 전에 null 확인
        if (_usernameInput == null || _usernameInput.text.Trim().Equals(""))
        {
            _statusMessage.text = "Check Your Username Again Please";
            _statusMessage.color = Color.red;
            _statusMessage.gameObject.SetActive(true);
            return;
        }

        string playerId = _usernameInput.text; // 입력된 아이디 가져오기
        bool success = await NetworkManager.Instance.ConnectToLobby(playerId);

        if (success)
        {
            //Debug.Log("Successfully connected to the room");
            // 로그인 버튼을 숨기고 로그아웃 버튼을 보이게 설정
            _loginBtn.gameObject.SetActive(false);
            _logoutBtn.gameObject.SetActive(true);
            // Username Interactive false
            _usernameInput.interactable = false;

            SceneManager.LoadScene("Lobby");
        }
        else
        {
            _statusMessage.text = "Server Network Unavailable";
            _statusMessage.color = Color.red;
            _statusMessage.gameObject.SetActive(true);
        }
    }

    private void HandleLogout()
    {
        NetworkManager.Instance.DisconnectAll();

        // 로그아웃 버튼을 숨기고 로그인 버튼을 보이게 설정
        _loginBtn.gameObject.SetActive(true);
        _logoutBtn.gameObject.SetActive(false);
        // Username Interactive true
        _usernameInput.interactable = true;
    }
    
    
    private void HandleQuit()
    {
        //Debug.Log("Quitting the game...");

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
}
