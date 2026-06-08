using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BGMZone : MonoBehaviour
{
    [SerializeField] private AudioClip _bgmClip;
    [SerializeField] private bool _crossfade = true;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SFXManager.Instance?.PlayBGM(_bgmClip, _crossfade);
    }
}
