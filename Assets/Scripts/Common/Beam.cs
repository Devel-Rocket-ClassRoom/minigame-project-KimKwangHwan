using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Beam : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private float damage;
    private BeamAnimEvent animEvents;
    private Animator animator;
    private Collider2D hitbox;

    //public BeamAnimEvent AnimEvents => animEvents;
    private void Awake()
    {
        animEvents = GetComponent<BeamAnimEvent>();
        hitbox = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        hitbox.enabled = false;
    }

    public void BeamEnable()
    {
        hitbox.enabled = true;
    }
    public void BeamDisable()
    {
        animator.SetTrigger("Stop");
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitPrefab != null)
            Instantiate(hitPrefab, transform.position, Quaternion.identity);
        if (((1 << other.gameObject.layer) & hitLayer.value) != 0)
        {
            if (other.TryGetComponent<HurtBox>(out var hurtbox))
            {
                hurtbox.ReceiveHit(damage);
            }
        }
    }
    public IEnumerator WaitForAnimEvent(string eventName)
    {
        bool fired = false;
        System.Action handler = () => fired = true;
        animEvents.Subscribe(eventName, handler);
        yield return new WaitUntil(() => fired);
        animEvents.Unsubscribe(eventName, handler);
    }
}