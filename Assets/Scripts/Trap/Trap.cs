using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float knockbackForceX = 25f;
    [SerializeField] private float knockbackForceY = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer.value) == 0) return;
        if (!other.TryGetComponent<HurtBox>(out var hurtBox)) return;

        var player = other.GetComponentInParent<PlayerController>();
        hurtBox.ReceiveHit(damage);

        if (player == null) return;

        float dir = -player.Facing;
        Vector2 force = new Vector2(dir * knockbackForceX, knockbackForceY);
        player.Motor.RB.linearVelocity = Vector2.zero;
        player.Motor.RB.AddForce(force, ForceMode2D.Impulse);

        player.Motor.SuppressHorizontalControl = true;
        StartCoroutine(ReleaseKnockback(player));
    }

    private IEnumerator ReleaseKnockback(PlayerController player)
    {
        yield return new WaitForSeconds(player.hurtDuration);
        if (player != null)
            player.Motor.SuppressHorizontalControl = false;
    }
}
