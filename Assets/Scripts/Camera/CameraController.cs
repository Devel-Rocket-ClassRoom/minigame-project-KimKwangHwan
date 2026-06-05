using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 followOffset = new Vector2(0f, 2f);

    private Camera _cam;
    private Rect _bounds;
    private Vector3 _velocity;

    private void Awake()
    {
        Instance = this;
        _cam = GetComponent<Camera>();
    }

    public void SetBounds(Rect bounds)
    {
        _bounds = bounds;
        SnapToPlayer();
    }

    public void SnapToPlayer()
    {
        if (!PlayerManager.Instance.HasPlayer) return;
        _velocity = Vector3.zero;
        transform.position = ClampedPosition(PlayerManager.Instance.Current.transform.position);
    }

    private void LateUpdate()
    {
        if (!PlayerManager.Instance.HasPlayer) return;

        Vector3 dest = ClampedPosition(PlayerManager.Instance.Current.transform.position);
        transform.position = Vector3.SmoothDamp(transform.position, dest, ref _velocity, smoothTime);
    }

    private Vector3 ClampedPosition(Vector2 target)
    {
        float halfH = _cam.orthographicSize;
        float halfW = halfH * _cam.aspect;

        target += followOffset;

        float x = Mathf.Clamp(target.x, _bounds.xMin + halfW, _bounds.xMax - halfW);
        float y = Mathf.Clamp(target.y, _bounds.yMin + halfH, _bounds.yMax - halfH);

        return new Vector3(x, y, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}
