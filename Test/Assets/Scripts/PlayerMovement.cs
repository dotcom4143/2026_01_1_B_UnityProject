using Fusion;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.Unicode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Networked] private float MoveAmountNet { get; set; }

    public float MoveAmount => MoveAmountNet;

    public void Tick(PlayerNetworkInput input)
    {
        Quaternion cameraYawRotation = Quaternion.Euler(0f, input.CameraYaw, 0f);

        Vector3 forward = cameraYawRotation * Vector3.forward;
        Vector3 right = cameraYawRotation * Vector3.right;

        Vector3 move = forward * input.Move.y + right * input.Move.x;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        MoveAmountNet = move.magnitude;

        Vector3 horizontalMove = move * moveSpeed * Runner.DeltaTime;

        transform.position += horizontalMove;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Runner.DeltaTime
            );
        }
    }
}