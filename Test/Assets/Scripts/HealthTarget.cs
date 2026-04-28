using Fusion;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthTarget : NetworkBehaviour
{
    [SerializeField] private int maxHp = 5;

    [Networked] public int HP { get; private set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            HP = maxHp;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority)
            return;

        HP -= damage;

        Debug.Log($"{name} HP : {HP}");

        if (HP <= 0)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        HP = maxHp;
        transform.position = Vector3.zero;

        Debug.Log($"{name} 리스폰");
    }
}