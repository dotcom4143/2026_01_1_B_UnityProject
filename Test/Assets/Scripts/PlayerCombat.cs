using Fusion;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.Unicode;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCombat : NetworkBehaviour
{
    [Header("Combat")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireDistance = 20f;
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField] private LayerMask hitMask;

    [Networked] private TickTimer FireCooldown { get; set; }

    public void Tick(PlayerNetworkInput input)
    {
        if (
            input.Buttons.IsSet((int)PlayerInputButton.Fire) &&
            FireCooldown.ExpiredOrNotRunning(Runner)
        )
        {
            FireLagCompensated();

            FireCooldown = TickTimer.CreateFromSeconds(Runner, fireInterval);
        }
    }

    private void FireLagCompensated()
    {
        if (!Object.HasStateAuthority)
            return;

        Vector3 origin = firePoint != null
        ? firePoint.position
        : transform.position + Vector3.up * 0.5f;

        Vector3 direction = transform.forward;

        if (Runner.LagCompensation.Raycast(
        origin,
        direction,
        fireDistance,
        Object.InputAuthority,
        out LagCompensatedHit hit,
        hitMask))
        {
            Debug.Log($"LagComp Hit : {hit.Hitbox.name}");

            RPC_PlayHitEffect(hit.Point, hit.Normal);

            Hitbox hitbox = hit.Hitbox;

            if (hitbox == null)
                return;

            HealthTarget target = hitbox.GetComponentInParent<HealthTarget>();

            if (target != null)
            {
                target.TakeDamage(1);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayHitEffect(Vector3 position, Vector3 normal)
    {
        if (EffectManager.Instance == null)
            return;

        EffectManager.Instance.PlayWorldEffect(
            EffectManager.Instance.HitEffect,
            position,
            normal
        );
    }
}