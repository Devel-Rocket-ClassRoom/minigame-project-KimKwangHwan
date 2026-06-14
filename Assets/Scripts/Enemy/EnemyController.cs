using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
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
    private CancellationTokenSource blinkCts;
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
        //if (_blinkCoroutine == null)
        //    _originalColor = _renderers.color;
        //else
        //{
        //    StopCoroutine(_blinkCoroutine);
        //    _renderers.color = _originalColor;
        //}
        //_blinkCoroutine = StartCoroutine(BlinkRoutine(interval, count));
        blinkCts?.Cancel();
        blinkCts?.Dispose();

        blinkCts = new CancellationTokenSource();

        BlinkRoutine(interval, count, blinkCts.Token).Forget();
    }

    private async UniTask BlinkRoutine(float interval, int count, CancellationToken ct)
    {
        int wait = (int)(interval * 1000);
        for (int b = 0; b < count; b++)
        {
            SetFlash(1f);
            await UniTask.Delay(wait, cancellationToken: ct);
            SetFlash(0f);
            await UniTask.Delay(wait, cancellationToken: ct);
        }
    }

    public void StartFadeAndDestroy(float delay = 0.3f, float duration = 0.8f)
    {
        //if (_blinkCoroutine != null)
        //{
        //    StopCoroutine(_blinkCoroutine);
        //    _blinkCoroutine = null;
        //    _renderers.color = _originalColor;
        //}
        blinkCts?.Cancel();
        blinkCts?.Dispose();
        SetFlash(0f);
        FadeAndDestroyRoutine(delay, duration).Forget();
    }

    private async UniTask FadeAndDestroyRoutine(float delay, float duration)
    {
        await UniTask.Delay((int)(delay * 1000));
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color c = _originalColor;
            c.a = alpha;
            _renderers.color = c;
            await UniTask.Yield();
        }
        Destroy(gameObject);
    }
}
