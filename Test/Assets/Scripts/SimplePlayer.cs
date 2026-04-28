using Fusion;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerPickup))]
public class SimplePlayer : NetworkBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerCombat combat;
    private PlayerPickup pickup;
    private PlayerCameraController cameraController;

    [Networked] private NetworkButtons PreviousButtons { get; set; }

    public NetworkButtons PreviousInputButtons => PreviousButtons;

    public Vector3 HoldPointPosition => pickup.HoldPointPosition;

    public override void Spawned()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        combat = GetComponent<PlayerCombat>();
        pickup = GetComponent<PlayerPickup>();
        cameraController = GetComponent<PlayerCameraController>();

        if (cameraController != null)
        {
            cameraController.Setup(Object.HasInputAuthority);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput<PlayerNetworkInput>(out PlayerNetworkInput input))
            return;

        movement.Tick(input);
        jump.Tick(input, PreviousButtons);
        pickup.Tick(input, PreviousButtons);
        combat.Tick(input);

        PreviousButtons = input.Buttons;
    }

    public override void Render()
    {
        if (animator == null)
            return;

        if (movement != null)
        {
            animator.SetFloat("Speed", movement.MoveAmount);
        }

        if (jump != null)
        {
            animator.SetBool("Grounded", jump.IsGrounded);
            animator.SetBool("Jump", !jump.IsGrounded && jump.VerticalVelocity > 0.1f);
            animator.SetBool("FreeFall", !jump.IsGrounded && jump.VerticalVelocity <= 0.1f);
        }

        animator.SetFloat("MotionSpeed", 3f);
    }
}