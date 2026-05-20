using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private float lifetime;
    private float elapsed;
    private Color startColor;

    public void Init(Sprite sprite, Vector3 position, Vector3 scale, Color color, float life)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        transform.position = position;
        transform.localScale = scale;
        startColor = color;
        sr.color = color;
        lifetime = life;
        elapsed = 0f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;

        // 알파 페이드
        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t);
        sr.color = c;

        if (t >= 1f) Destroy(gameObject);
    }
}