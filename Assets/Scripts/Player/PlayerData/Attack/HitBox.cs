using UnityEngine;

public sealed class Hitbox : MonoBehaviour
{
    public void Enable(float damage, Vector2 offset, Vector2 size,
                       float knockback, float facing)
    {
        // TODO: 콜라이더 위치/크기 세팅 후 활성화.
        //       같은 스윙 내 중복 타격 방지(이미 맞은 대상 HashSet)는 필수.
    }

    public void Disable()
    {
        // TODO: 콜라이더 비활성 + 타격 대상 set 클리어
    }
}
