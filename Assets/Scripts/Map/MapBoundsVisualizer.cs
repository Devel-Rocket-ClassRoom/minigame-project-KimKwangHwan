using UnityEngine;

public class MapBoundsVisualizer : MonoBehaviour
{
    [SerializeField] private MapData mapData;

    private void OnDrawGizmos()
    {
        if (mapData == null) return;

        Vector3 center = (Vector3)(Vector2)mapData.boundsCenter;
        Vector3 size   = (Vector3)(Vector2)mapData.boundsSize;

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.1f);
        Gizmos.DrawCube(center, size);

        Gizmos.color = new Color(0f, 1f, 0.5f, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}
