using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossEncounterManager : MonoBehaviour
{
    [SerializeField] private BossRoom bossRoom;
    [SerializeField] private Witch witch;
    [SerializeField] private BossHUD bossHUD;
    [SerializeField] private CanvasGroup bossNamePanel;

    [SerializeField] private float cutsceneZoomSize = 3f;
    [SerializeField] private BossClearUI bossClearUI;

    private void Start()
    {
        bossHUD.gameObject.SetActive(false);
        bossRoom.OnPlayerEnter += () => PlayIntroAsync().Forget();
        witch.Health.OnDead += () => PlayDefeatAsync().Forget();
    }

    private async UniTask PlayIntroAsync()
    {
        var player = PlayerManager.Instance?.Current;
        if (player != null) player.InputLocked = true;

        bossHUD.Bind(witch.Health);
        CameraController.Instance.BeginCutscene(witch.transform, cutsceneZoomSize);

        await NameFadeSequenceAsync();

        CameraController.Instance.EndCutscene();

        await bossHUD.Show();

        if (player != null) player.InputLocked = false;
        witch.StartFight();
    }

    private async UniTask NameFadeSequenceAsync()
    {
        await FadeCanvasGroup(bossNamePanel, 0f, 1f, 0.4f);
        await UniTask.Delay(3000, ignoreTimeScale: true);
        await FadeCanvasGroup(bossNamePanel, 1f, 0f, 0.4f);
    }

    private async UniTask PlayDefeatAsync()
    {
        Time.timeScale = 0.3f;
        await UniTask.Delay(500, ignoreTimeScale: true);
        Time.timeScale = 1f;

        await FadeController.Instance.FlashWhite(0.3f);

        await UniTask.Delay(1500);

        await bossHUD.Hide();
        bossRoom.OpenDoors();

        var currentMap = MapManager.Instance?.CurrentMap;
        if (currentMap != null)
            SFXManager.Instance?.PlayBGM(currentMap.bgmClip, true);

        var player = PlayerManager.Instance?.Current;
        if (player != null) player.InputLocked = true;
        bossClearUI?.Show();
    }

    private async UniTask FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        cg.alpha = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            await UniTask.Yield();
        }
        cg.alpha = to;
    }
}
