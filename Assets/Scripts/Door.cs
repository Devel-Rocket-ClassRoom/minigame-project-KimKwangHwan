using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float openDuration = 1f;
    [SerializeField] private float openDistance;

    [SerializeField] private bool _isOpen;
    public AudioClip sfxClip;

    private void Awake()
    {
        SetOpenDistance();
    }

    private void OnEnable()
    {
        SetOpenDistance();
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        SFXManager.Instance?.PlaySFX(sfxClip);
        StartCoroutine(OpenRoutine());
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;
        SFXManager.Instance?.PlaySFX(sfxClip);
        StartCoroutine(CloseRoutine());
    }

    public void OpenImmediately()
    {
        if (_isOpen) return;
        SetOpenDistance();
        _isOpen = true;
        StopAllCoroutines();
        transform.position += Vector3.up * openDistance;
    }

    private IEnumerator OpenRoutine()
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
            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator CloseRoutine()
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
            yield return null;
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
