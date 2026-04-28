using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPickup : NetworkBehaviour
{
    [Header("Pickup")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private float dropForce = 2f;
    [SerializeField] private LayerMask pickupMask;

    [Networked] private NetworkObject HeldObject { get; set; }

    public Vector3 HoldPointPosition
    {
        get
        {
            if (holdPoint != null)
                return holdPoint.position;

            return transform.position + transform.forward * 1.2f + Vector3.up * 1.2f;
        }
    }

    public void Tick(PlayerNetworkInput input, NetworkButtons previousButtons)
    {
        if (!input.Buttons.WasPressed(previousButtons, (int)PlayerInputButton.Pickup))
        {
            return;
        }

        if (!TryDrop())
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        if (!Object.HasStateAuthority)
            return;

        if (HeldObject != null)
            return;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * pickupDistance, Color.red, 2f);

        if (!Physics.Raycast(origin, direction, out RaycastHit hit, pickupDistance, pickupMask))
            return;

        PickableBox box = hit.collider.GetComponentInParent<PickableBox>();

        if (box == null)
            return;

        box.PickUp(Object.InputAuthority);
        HeldObject = box.Object;
    }

    private bool TryDrop()
    {
        if (!Object.HasStateAuthority)
            return false;

        if (HeldObject == null)
            return false;

        PickableBox box = HeldObject.GetComponent<PickableBox>();

        if (box == null)
            return false;

        box.Drop(transform.forward * dropForce);
        HeldObject = null;

        return true;
    }
}