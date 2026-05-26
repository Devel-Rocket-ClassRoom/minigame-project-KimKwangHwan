using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Witch : MonoBehaviour
{
    [SerializeField] List<BossPattern> patterns;
    private BossContext ctx;
    private BossPattern lastPattern;
    private int currentPhase = 2;
    [SerializeField] private Animator animator;
    [SerializeField] private BossAnimEvents animEvents;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Vector2 hitboxOffset;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundWidth;
    [SerializeField] private float groundHeight;
    public bool IsGrounded => Physics2D.OverlapBox(
        groundCheck.position, new Vector2(groundWidth, groundHeight), 0f, groundLayer);

    public float Facing => transform.localScale.x;
    private void Awake()
    {
        ctx = new BossContext
        {
            bossTransform = transform,
            playerTransform = player,
            animator = animator,
            animEvents = animEvents,
            hitbox = hitbox,
            muzzle = muzzle,
            currentPhase = currentPhase
        };
    }
    private void Start()
    {
        StartCoroutine(BehaviorLoop());
    }
    private IEnumerator BehaviorLoop()
    {
        while (true)
        {
            var pattern = patterns[Random.Range(0, patterns.Count)];
            //var pattern = patterns[0];
            yield return StartCoroutine(ExecutePattern(pattern));
        }

    }
    private IEnumerator ExecutePattern(BossPattern pattern)
    {
        lastPattern = pattern;
        pattern.lastUsedTime = Time.time;

        yield return StartCoroutine(pattern.Execute(ctx));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.transform.position, new Vector3(groundWidth, groundHeight, 1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)hitbox.transform.position + hitboxOffset, hitboxSize);
    }
}