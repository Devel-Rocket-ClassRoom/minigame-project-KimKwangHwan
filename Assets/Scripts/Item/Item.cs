using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [SerializeField] protected TextMeshPro statUpText;
    [SerializeField] protected string upText;
    [SerializeField] protected float floatDistance = 1f;
    [SerializeField] protected float floatDuration = 1f;
    [SerializeField] protected float stat;
    [SerializeField] protected StatType statType;

    public event Action<PlayerController> OnCollected;
    private void Awake()
    {
        if (statUpText != null)
            statUpText.text = $"{upText} +{stat:F0}";
    }
    private void OnEnable()
    {
        OnCollected += AddStats;
    }
    private void OnDisable()
    {
        OnCollected -= AddStats;
    }

    public void Collect(PlayerController player)
    {
        OnCollected?.Invoke(player);
        if (statUpText != null)
            StartCoroutine(FloatAndFade());
        else
            Destroy(gameObject);
    }

    private IEnumerator FloatAndFade()
    {
        //Transform textTransform = statUpText.transform;
        Vector3 startPos  = transform.position;
        Vector3 targetPos = startPos + Vector3.up * floatDistance;
        Color startColor  = statUpText.color;
        Color spriteStartColor = GetComponent<SpriteRenderer>().color;
        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            statUpText.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            GetComponent<SpriteRenderer>().color = new Color(spriteStartColor.r, spriteStartColor.g, spriteStartColor.b, 1f - t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void AddStats(PlayerController player)
    {
        player.Stats.AllStats[statType].AddModifier(new StatModifier(stat, this));
    }
}
