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
    public void LaunchArc(Vector2 start, Vector2 target, float speed, float damage)
    {
        rb.position = start;
        this.damage = damage;
        rb.gravityScale = gravityScale;

        float g = Mathf.Abs(Physics2D.gravity.y) * gravityScale;
        float rawDx = target.x - start.x;
        float dx = Mathf.Abs(rawDx);
        float dy = target.y - start.y;

        // 거의 수직인 경우 그냥 위로 발사
        if (dx < 0.01f)
        {
            rb.linearVelocity = new Vector2(0f, speed);
            return;
        }

        float v2 = speed * speed;
        // 포물선 발사각 공식: 판별식이 0보다 작으면 사정거리 초과
        float disc = v2 * v2 - g * (g * dx * dx + 2f * dy * v2);
        if (disc < 0f)
        {
            // 사정거리 초과 시 45도(최대 사거리 각도)로 발사
            rb.linearVelocity = new Vector2(
                speed * Mathf.Cos(45f * Mathf.Deg2Rad) * Mathf.Sign(rawDx),
                speed * Mathf.Sin(45f * Mathf.Deg2Rad)
            );
            return;
        }

        // +sqrt → 고각(포물선), -sqrt → 저각(직선에 가까움)
        float tanTheta = (v2 + Mathf.Sqrt(disc)) / (g * dx);
        float theta = Mathf.Atan(tanTheta);

        rb.linearVelocity = new Vector2(
            speed * Mathf.Cos(theta) * Mathf.Sign(rawDx),
            speed * Mathf.Sin(theta)
        );
    }
}
