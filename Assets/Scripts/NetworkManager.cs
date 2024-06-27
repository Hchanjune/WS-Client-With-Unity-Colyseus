using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using DefaultNamespace;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public ColyseusClient Client { get; private set; }

    public ColyseusRoom<LobbyState> Lobby { get; private set; }
    public ColyseusRoom<ChatRoomState> ChatRoom { get; private set; }

    // 현재 로그인한 유저의 정보를 저장하는 필드
    public string CurrentPlayerId { get; private set; }

    // Room 초기화 완료 이벤트
    public event Action OnRoomInitialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하는 경우 중복 방지
        }
    }

    private void Start()
    {
        Client = new ColyseusClient("ws://localhost:3000");
    }

    public async Task<bool> ConnectToLobby(string playerId)
    {
        var options = new Dictionary<string, object>
        {
            { "id", playerId }
        };

        try
        {
            Lobby = await Client.JoinOrCreate<LobbyState>("Lobby", options);
            Lobby.OnStateChange += OnLobbyStateChange;
            CurrentPlayerId = playerId; // 현재 플레이어 ID 저장
            Debug.Log($"Joined lobby with id: {playerId}");

            // Room 초기화 완료 이벤트 발생
            OnRoomInitialized?.Invoke();

            Lobby.OnMessage<Dictionary<string, object>>("chat_room_created", OnChatRoomCreated);
            Lobby.OnMessage<Dictionary<string, object>>("error", OnError);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to join or create lobby: " + e.Message);
            return false;
        }
    }

    private void OnChatRoomCreated(Dictionary<string, object> message)
    {
        if (message.TryGetValue("chatRoomId", out object chatRoomId))
        {
            JoinChatRoom(chatRoomId.ToString());
        }
    }

    private void OnError(Dictionary<string, object> message)
    {
        if (message.TryGetValue("message", out object errorMessage))
        {
            Debug.LogError(errorMessage.ToString());
        }
    }

    public async Task<bool> CreateChatRoom(string roomName)
    {
        var options = new Dictionary<string, object>
        {
            { "id", CurrentPlayerId }
        };

        try
        {
            await Lobby.Send("create_chat_room", options);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send create chat room message: " + e.Message);
            return false;
        }
    }

    public async void JoinChatRoom(string roomId)
    {
        var options = new Dictionary<string, object>
        {
            { "id", CurrentPlayerId }
        };

        try
        {
            ChatRoom = await Client.JoinById<ChatRoomState>(roomId, options);
            Debug.Log($"Joined ChatRoom with id: {roomId}");
            ChatRoom.OnStateChange += OnChatRoomStateChange;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to join chat room: " + e.Message);
        }
    }

    public void DisconnectFromRoom()
    {
        if (Lobby != null)
        {
            Lobby.Leave();
            Lobby = null;
        }

        if (ChatRoom != null)
        {
            ChatRoom.Leave();
            ChatRoom = null;
        }

        CurrentPlayerId = null; // 플레이어 ID 초기화
        Debug.Log("Logged out");
    }

    private void OnLobbyStateChange(LobbyState state, bool isFirstState)
    {
        foreach (Player player in state.players.Values)
        {
            Debug.Log($"Lobby - Player {player.id} has score {player.score}");
        }
    }

    private void OnChatRoomStateChange(ChatRoomState state, bool isFirstState)
    {
        foreach (Player player in state.players.Values)
        {
            Debug.Log($"ChatRoom - Player {player.id} has score {player.score}");
        }
    }
}
