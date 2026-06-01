using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MapTransitionZone : MonoBehaviour
{
    [SerializeField] public string zoneId;
    [SerializeField] public MapData targetMap;
    [SerializeField] public string targetZoneId;
    [SerializeField] private Transform spawnPoint;

    private static readonly Dictionary<string, MapTransitionZone> Registry = new();

    public Vector2 SpawnPosition => spawnPoint != null
        ? (Vector2)spawnPoint.position
        : (Vector2)transform.position;

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(zoneId))
            Registry[zoneId] = this;
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(zoneId) && Registry.TryGetValue(zoneId, out var z) && z == this)
            Registry.Remove(zoneId);
    }

    public static bool TryGet(string id, out MapTransitionZone zone)
        => Registry.TryGetValue(id, out zone);

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TriggerEnter");
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player");
        if (targetMap == null || string.IsNullOrEmpty(targetZoneId)) return;
        Debug.Log("Not Empty");
        MapManager.Instance.StartTransition(targetMap, targetZoneId);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var col = GetComponent<Collider2D>();
        if (col != null) Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        }
    }
}
