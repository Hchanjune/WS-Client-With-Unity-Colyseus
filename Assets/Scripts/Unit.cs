using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float interpolationSpeed = 10f; // 보간 속도
    private Rigidbody rb;
    private string playerId;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerId = NetworkManager.Instance.CurrentPlayerId; // 플레이어 ID 설정
        if (playerId == gameObject.name) // 현재 플레이어만 서버에 위치 전송
        {
            StartCoroutine(SendPositionToServer());
        }
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (NetworkManager.Instance.CurrentPlayerId == gameObject.name)
        {
            Move();
        }
        else
        {
            InterpolatePosition();
        }
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ);

        if (move != Vector3.zero)
        {
            move = move.normalized * (moveSpeed * Time.deltaTime);
            Vector3 newPosition = rb.position + move;
            rb.MovePosition(newPosition);

            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, moveSpeed * 100 * Time.deltaTime));
        }
    }

    private void InterpolatePosition()
    {
        rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, interpolationSpeed * Time.deltaTime));
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, interpolationSpeed * Time.deltaTime));
    }

    private IEnumerator SendPositionToServer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f); // 0.05초마다 위치 전송

            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            NetworkManager.Instance.ChatRoom.Send("UPDATE_POSITION", new
            {
                id = playerId,
                transformX = position.x,
                transformY = position.y,
                transformZ = position.z,
                rotationX = rotation.x,
                rotationY = rotation.y,
                rotationZ = rotation.z,
                rotationW = rotation.w
            });
        }
    }

    public void UpdatePosition(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }
}
