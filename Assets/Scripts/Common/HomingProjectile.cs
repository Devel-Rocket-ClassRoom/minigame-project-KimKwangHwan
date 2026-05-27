using UnityEngine;

public class HomingProjectile : Projectile
{
    [Header("Homing")]
    [SerializeField] protected float turnRateDeg = 120f;
    protected Transform target;
    protected float currentSpeed;
    public virtual void LaunchHoming(Vector2 dir, float speed, float damage, float facing, Transform target)
    {
        base.Launch(dir, speed, damage, facing);
        currentSpeed = speed;
        this.target = target;
    }
    public override void Launch(Vector2 dir, float speed, float damage, float facing)
    {
        base.Launch(dir, speed, damage, facing);
        currentSpeed = speed;
        target = null;
    }

    protected virtual void FixedUpdate()
    {
        // 호밍 페이즈가 끝나거나 타겟이 사라지면 직진 — 회피 창구
        if (timer >= duration) return;
        if (target == null || !target.gameObject.activeInHierarchy) return;

        Vector2 currentDir = rb.linearVelocity.sqrMagnitude > 0.0001f
            ? rb.linearVelocity.normalized
            : Vector2.right;
        Vector2 desiredDir = ((Vector2)target.position - rb.position).normalized;

        // 핵심: 회전 속도 제한 — 한 프레임에 허용된 각도만큼만 desiredDir 쪽으로 회전
        float maxRad = turnRateDeg * Mathf.Deg2Rad * Time.fixedDeltaTime;
        Vector2 newDir = Vector3.RotateTowards(currentDir, desiredDir, maxRad, 0f);

        rb.linearVelocity = newDir * currentSpeed;

        // 진행 방향에 맞춰 스프라이트 회전
        float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }
}
