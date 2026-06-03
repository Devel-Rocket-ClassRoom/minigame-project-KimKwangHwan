using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    public Animator Animator { get { return animator; } }
    public float Facing { get { return Mathf.Sign(transform.localScale.x); } }
    [SerializeField]
    protected EnemyMotor enemyMotor;
    [SerializeField]
    protected EnemyCombat enemyCombat;
    [SerializeField]
    protected EnemyHealth enemyHealth;
    public EnemyHealth Health { get { return enemyHealth; } }
    public EnemyMotor Motor { get { return enemyMotor; } }
    protected float moveDirection;
    protected EnemyStateMachine stateMachine;
    private SpriteRenderer _renderers;
    private Color _originalColor;
    private Coroutine _blinkCoroutine;
    private MaterialPropertyBlock _mpb;
    private static readonly int FlashID = Shader.PropertyToID("_FlashAmount");
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        moveDirection = 1f;
        enemyHealth.OnDamaged += GetHurt;
        _renderers = animator.GetComponent<SpriteRenderer>();
        _originalColor = new Color();
        _originalColor = _renderers.color;
        _mpb = new MaterialPropertyBlock();
    }
    private void SetFlash(float amount)
    {
        _renderers.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashID, amount);
        _renderers.SetPropertyBlock(_mpb);
    }
    public virtual void AllFlip(float x)
    {
        if (x != 0f)
        {
            if (moveDirection * x < 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            moveDirection = x > 0f ? 1f : -1f;
        }
    }
    protected virtual void Update()
    {
        stateMachine.CurrentState.Update();
    }
    protected virtual void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
    protected virtual void GetHurt(float damage)
    {
        StartBlink();
    }

    public void StartBlink(float interval = 0.1f, int count = 2)
    {
        //if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        //_blinkCoroutine = StartCoroutine(BlinkRoutine(interval, count));
        if (_blinkCoroutine == null)
            _originalColor = _renderers.color;
        else
        {
            StopCoroutine(_blinkCoroutine);
            _renderers.color = _originalColor;
        }
        _blinkCoroutine = StartCoroutine(BlinkRoutine(interval, count));
    }

    private IEnumerator BlinkRoutine(float interval, int count)
    {
        var wait = new WaitForSeconds(interval);
        for (int b = 0; b < count; b++)
        {
            SetFlash(1f);
            yield return wait;
            SetFlash(0f);
            yield return wait;
        }
        _blinkCoroutine = null;
    }

    public void StartFadeAndDestroy(float delay = 0.3f, float duration = 0.8f)
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
            _renderers.color = _originalColor;
        }
        StartCoroutine(FadeAndDestroyRoutine(delay, duration));
    }

    private IEnumerator FadeAndDestroyRoutine(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color c = _originalColor;
            c.a = alpha;
            _renderers.color = c;
            yield return null;
        }
        Destroy(gameObject);
    }
}
