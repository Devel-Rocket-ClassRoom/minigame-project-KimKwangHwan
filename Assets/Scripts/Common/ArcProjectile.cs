using UnityEngine;

public class ArcProjectile : Projectile
{
    [SerializeField] private float gravityScale = 1f;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // start: 발사 위치, target: 착지 목표, speed: 발사 속력 (고정)
    // useLowArc: true면 저각(직선에 가까운) 궤도 사용
    public void LaunchArc(Vector2 start, Vector2 target, float speed, float damage, bool useLowArc = false)
    {
        rb.position = start;
        this.damage = damage;
        rb.gravityScale = gravityScale;

        float g = Mathf.Abs(Physics2D.gravity.y) * gravityScale;
        float rawDx = target.x - start.x;
        float dx = Mathf.Abs(rawDx);
        float dy = target.y - start.y;

        // 거의 수직인 경우
        if (dx < 0.01f)
        {
            rb.linearVelocity = new Vector2(0f, useLowArc ? -speed : speed);
            return;
        }

        float v2 = speed * speed;
        // 포물선 발사각 공식: 판별식이 0보다 작으면 사정거리 초과
        float disc = v2 * v2 - g * (g * dx * dx + 2f * dy * v2);
        if (disc < 0f)
        {
            float fallbackAngle = useLowArc ? 20f : 45f;
            rb.linearVelocity = new Vector2(
                speed * Mathf.Cos(fallbackAngle * Mathf.Deg2Rad) * Mathf.Sign(rawDx),
                speed * Mathf.Sin(fallbackAngle * Mathf.Deg2Rad)
            );
            return;
        }

        // +sqrt → 고각(포물선), -sqrt → 저각(직선에 가까움, dy<0이면 하향도 가능)
        float sqrtDisc = Mathf.Sqrt(disc);
        float tanTheta = useLowArc
            ? (v2 - sqrtDisc) / (g * dx)
            : (v2 + sqrtDisc) / (g * dx);
        float theta = Mathf.Atan(tanTheta);

        rb.linearVelocity = new Vector2(
            speed * Mathf.Cos(theta) * Mathf.Sign(rawDx),
            speed * Mathf.Sin(theta)
        );
    }
}
