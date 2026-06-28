using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Vector2 center;
    [SerializeField] private Vector2 size;
    [SerializeField] private Door[] entranceDoors;
    [SerializeField] private BoxCollider2D encounter;
    public Vector2 Min => (Vector2)transform.position + center - size * 0.5f;
    public Vector2 Max => (Vector2)transform.position + center + size * 0.5f;

    public Vector2 RandomPoint()
    {
        return new Vector2(
            Random.Range(Min.x, Max.x),
            Random.Range(Min.y, Max.y)
        );
    }

    public Vector2 RandomPointAwayFrom(Vector2 from, float minDistance, float maxDistance, int maxTries = 20)
    {
        for (int i = 0; i < maxTries; i++)
        {
            var p = RandomPoint();
            float dist = Vector2.Distance(p, from);
            if (dist >= minDistance && dist <= maxDistance)
                return p;
        }
        Vector2 dir = ((Vector2)transform.position - from).normalized;
        return (Vector2)transform.position + dir * minDistance;
    }

    public event System.Action OnPlayerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter?.Invoke();
            CloseDoors();
            encounter.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)transform.position + center, size);
    }

    public void OpenDoors()
    {
        foreach (var door in entranceDoors)
        {
            door.Open();
        }
    }

    public void CloseDoors()
    {
        foreach (var door in entranceDoors)
        {
            door.Close();
        }
    }
}
