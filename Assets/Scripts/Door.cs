using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float openDuration = 1f;
    [SerializeField] private float openDistance;

    [SerializeField] private bool _isOpen;
    public AudioClip sfxClip;

    private CancellationTokenSource _doorCts;

    private void Awake()
    {
        SetOpenDistance();
    }

    private void OnEnable()
    {
        SetOpenDistance();
    }

    private void OnDestroy()
    {
        _doorCts?.Cancel();
        _doorCts?.Dispose();
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        SFXManager.Instance?.PlaySFX(sfxClip);
        _doorCts?.Cancel();
        _doorCts?.Dispose();
        _doorCts = new CancellationTokenSource();
        OpenRoutine(_doorCts.Token).Forget();
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;
        SFXManager.Instance?.PlaySFX(sfxClip);
        _doorCts?.Cancel();
        _doorCts?.Dispose();
        _doorCts = new CancellationTokenSource();
        CloseRoutine(_doorCts.Token).Forget();
    }

    public void OpenImmediately()
    {
        if (_isOpen) return;
        SetOpenDistance();
        _isOpen = true;
        _doorCts?.Cancel();
        _doorCts?.Dispose();
        _doorCts = null;
        transform.position += Vector3.up * openDistance;
    }

    private async UniTask OpenRoutine(CancellationToken ct)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * openDistance;
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / openDuration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            await UniTask.Yield(ct);
        }

        transform.position = targetPos;
    }

    private async UniTask CloseRoutine(CancellationToken ct)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos - Vector3.up * openDistance;
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / openDuration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            await UniTask.Yield(ct);
        }

        transform.position = targetPos;
    }

    private void SetOpenDistance()
    {
        if (openDistance == 0f)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                openDistance = sr.bounds.size.y;
        }
    }
}
