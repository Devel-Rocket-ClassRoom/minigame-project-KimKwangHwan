using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float viewAngle = 120f;     // 시야각 (0이면 전방향)
    [SerializeField] private Transform eyePoint;          // 시선 원점 (없으면 transform)
    [SerializeField] private float detectionPlayerDist = 2f;
    [SerializeField] private float nearDist = 0.7f;

    [Header("Layers")]
    [SerializeField] private LayerMask targetMask;        // Player 레이어
    [SerializeField] private LayerMask obstacleMask;      // Wall/Ground 레이어

    [Header("Memory")]
    [SerializeField] private float loseSightDuration = 2f;  // 시야에서 사라진 뒤 추적 유지 시간
    [SerializeField] private Transform forceTarget;
    public Transform Target { get; private set; }
    public bool HasTarget => Target != null && _lostTimer < loseSightDuration;

    private float _lostTimer;

    private Vector2 Origin => eyePoint != null ? (Vector2)eyePoint.position : (Vector2)transform.position;
    // 적이 바라보는 방향. 스프라이트 flipX 또는 transform.localScale.x로 판정.
    //private Vector2 Facing => new Vector2(Mathf.Sign(transform.localScale.x), 0f);
    public float Facing
    {
        get { return transform.parent.transform.localScale.x > 0f ? 1f : -1f; }
    }

    private void Update()
    {
        if (TryPerceivePlayer(out Transform found))
        {
            Target = found;
            _lostTimer = 0f;
        }
        else if (Target != null)
        {
            _lostTimer += Time.deltaTime;
            if (_lostTimer >= loseSightDuration)
                Target = null;
        }
    }

    private bool TryPerceivePlayer(out Transform player)
    {
        player = null;
        bool inViewAngle = false;
        bool near = false;
        bool tooNear = false;
        // 1) 범위 내 플레이어 후보 검색
        Collider2D hit = Physics2D.OverlapCircle(Origin, detectionRange, targetMask);
        if (hit == null) return false;

        Vector2 toTarget = (Vector2)hit.transform.position - Origin;
        float dist = toTarget.magnitude;
        if (dist < detectionPlayerDist) { player = hit.transform; near = true; }
        if (dist < nearDist) tooNear = true;
        Vector2 dir = toTarget / dist;

        // 2) 시야각 체크 (viewAngle <= 0 이면 전방향 감지)
        float dot = 0f;
        if (viewAngle > 0f)
        {
            dot = Vector2.Dot(new Vector2(Facing, 0f), dir);
            float cosHalf = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
            if (dot >= cosHalf)
                inViewAngle = true;
        }

        // 3) 벽 차단 검사: 플레이어까지 광선을 쐈을 때 장애물(벽)에 먼저 맞으면 실패
        RaycastHit2D wall = Physics2D.Raycast(Origin, dir, dist, obstacleMask);
        if (wall.collider != null) return false;

        player = hit.transform;
        return tooNear || (near && dot > 0f) || inViewAngle;
    }
    public void SetTarget()
    {
        Target = forceTarget;
        _lostTimer = 0f;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = Origin;

        Gizmos.color = HasTarget ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(origin, detectionRange);

        if (viewAngle > 0f)
        {
            Vector2 f = new Vector2(Facing, 0f);
            Quaternion left = Quaternion.Euler(0, 0, viewAngle * 0.5f);
            Quaternion right = Quaternion.Euler(0, 0, -viewAngle * 0.5f);
            Gizmos.DrawLine(origin, origin + (Vector2)(left * f) * detectionRange);
            Gizmos.DrawLine(origin, origin + (Vector2)(right * f) * detectionRange);
        }

        if (Application.isPlaying && Target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, Target.position);
        }
    }
#endif
}