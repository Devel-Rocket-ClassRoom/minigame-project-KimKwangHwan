using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public GameObject SourcePrefab { get; set; }

    public void Despawn()
    {
        PoolManager.Instance.Despawn(SourcePrefab, gameObject);
    }
}