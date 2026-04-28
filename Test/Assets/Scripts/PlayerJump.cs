using Fusion;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.Unicode;

public class PlayerJump : NetworkBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravity = -20f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Networked] private float VerticalVelocityNet { get; set; }
    [Networked] private NetworkBool IsGroundedNet { get; set; }
    [Networked] private int JumpTick { get; set; }

    public float VerticalVelocity => VerticalVelocityNet;
    public bool IsGrounded => IsGroundedNet;

    public void Tick(PlayerNetworkInput input, NetworkButtons previousButtons)
    {
        bool grounded = CheckGrounded();
        IsGroundedNet = grounded;

        if (grounded && VerticalVelocityNet < 0f)
        {
            VerticalVelocityNet = 0f;
        }

        if (grounded && input.Buttons.WasPressed(previousButtons, (int)PlayerInputButton.Jump))
        {
            VerticalVelocityNet = jumpForce;
            IsGroundedNet = false;
            JumpTick = Runner.Tick;
        }

        VerticalVelocityNet += gravity * Runner.DeltaTime;

        if (!(grounded && VerticalVelocityNet <= 0f))
        {
            Vector3 verticalMove = new Vector3(
                0f,
                VerticalVelocityNet * Runner.DeltaTime,
                0f
            );

            transform.position += verticalMove;
        }
    }

    private bool CheckGrounded()
    {
        Vector3 checkPosition = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.9f;

        return Physics.CheckSphere(
            checkPosition,
            groundCheckRadius,
            groundMask
        );
    }
}