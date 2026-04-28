using Fusion;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.Unicode;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class PickableBox : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Networked] private NetworkBool IsHeld { get; set; }
    [Networked] private PlayerRef Holder { get; set; }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!IsHeld)
            return;

        if (!Runner.TryGetPlayerObject(Holder, out NetworkObject playerObject))
            return;

        SimplePlayer player = playerObject.GetComponent<SimplePlayer>();

        if (player == null)
            return;

        rb.isKinematic = true;
        transform.position = player.HoldPointPosition;
        transform.rotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);
    }

    public void PickUp(PlayerRef holder)
    {
        if (!Object.HasStateAuthority)
            return;

        Holder = holder;
        IsHeld = true;

        if (rb == null)
            return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void Drop(Vector3 impulse)
    {
        if (!Object.HasStateAuthority)
            return;

        IsHeld = false;
        Holder = default;

        if (rb == null)
            return;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(impulse, ForceMode.VelocityChange);
    }
}