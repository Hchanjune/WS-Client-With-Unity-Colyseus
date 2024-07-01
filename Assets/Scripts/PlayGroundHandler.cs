using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

public class PlayGroundHandler : MonoBehaviour
{
    public GameObject playerPrefab;
    private Dictionary<string, GameObject> playerUnits = new Dictionary<string, GameObject>();

    void Start()
    {
        InitializePlayers();
        RegisterUnitSyncEvents();
    }

    private void InitializePlayers()
    {
        string currentPlayerId = NetworkManager.Instance.CurrentPlayerId;
        MapSchema<ChatRoomPlayer> players = NetworkManager.Instance.ChatRoom.State.chatRoomPlayers;

        int index = 0;
        foreach (ChatRoomPlayer player in players.Values)
        {
            string playerId = player.id;
            bool isCurrentPlayer = playerId == currentPlayerId;

            GameObject playerUnit = Instantiate(playerPrefab, GetSpawnPosition(index), Quaternion.identity);
            playerUnit.name = playerId;
            playerUnits.Add(playerId, playerUnit);

            if (isCurrentPlayer)
            {
                // 현재 플레이어의 유닛에 특별한 처리를 할 수 있습니다.
                playerUnit.GetComponentInChildren<Renderer>().material.color = Color.red;
                playerUnit.AddComponent<Unit>(); // Unit 스크립트를 현재 플레이어에만 추가
            }

            index++;
        }
    }

    private void RegisterUnitSyncEvents()
    {
        NetworkManager.Instance.ChatRoom.OnMessage<Dictionary<string, object>>("UNIT_POSITION", OnUpdatePosition);
    }

    private void OnUpdatePosition(Dictionary<string, object> message)
    {
        string playerId = message["id"].ToString();
        if (playerId != NetworkManager.Instance.CurrentPlayerId)
        {
            Vector3 position = new Vector3(
                float.Parse(message["transformX"].ToString()),
                float.Parse(message["transformY"].ToString()),
                float.Parse(message["transformZ"].ToString())
            );

            Quaternion rotation = new Quaternion(
                float.Parse(message["rotationX"].ToString()),
                float.Parse(message["rotationY"].ToString()),
                float.Parse(message["rotationZ"].ToString()),
                float.Parse(message["rotationW"].ToString())
            );

            if (playerUnits.ContainsKey(playerId))
            {
                playerUnits[playerId].GetComponent<Unit>().UpdatePosition(position, rotation);
            }
        }
    }

    private Vector3 GetSpawnPosition(int index)
    {
        float x = index * 2.0f;
        return new Vector3(x, 5, 0);
    }
}
