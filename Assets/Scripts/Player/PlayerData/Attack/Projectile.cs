using UnityEngine;

public sealed class Projectile : MonoBehaviour
{
    public void Launch(Vector2 dir, float speed, float damage, float facing)
    {
        // TODO: rb.velocity = dir * speed; 데미지/수명/소유자 세팅
    }
}