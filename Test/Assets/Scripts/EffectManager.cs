using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject muzzleEffect;

    public GameObject HitEffect => hitEffect;
    public GameObject MuzzleEffect => muzzleEffect;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayLocalEffect(GameObject prefab, Vector3 position, Vector3 normal)
    {
        if (prefab == null)
            return;

        Quaternion rotation = normal.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(normal)
            : Quaternion.identity;

        GameObject effect = Instantiate(prefab, position, rotation);
        Destroy(effect, 2f);
    }

    public void PlayWorldEffect(GameObject prefab, Vector3 position, Vector3 normal)
    {
        PlayLocalEffect(prefab, position, normal);
    }
}