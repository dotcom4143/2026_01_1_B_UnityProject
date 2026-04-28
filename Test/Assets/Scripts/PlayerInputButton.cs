using Fusion;
using UnityEngine;

public enum PlayerInputButton
{
    Fire = 0,
    Jump = 1,
    Pickup = 2
}

public struct PlayerNetworkInput : INetworkInput
{
    public Vector2 Move;
    public float CameraYaw;
    public NetworkButtons Buttons;
}