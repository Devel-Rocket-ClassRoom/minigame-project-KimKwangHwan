using System.Collections;
using UnityEngine;
using SaveDataV = SaveDataV1;

public class GameInitializer : Singleton<GameInitializer>
{
    [SerializeField] private PlayerController player;
    [SerializeField] private bool autoLoadOnStart = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        _ = Instance;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (!autoLoadOnStart) return;

        SaveDataV data = SaveManager.Instance.LoadLastUsed();
        ApplyData(data);
    }

    /// <summary>로드 UI 등 외부에서 특정 슬롯을 명시적으로 로드할 때 호출.</summary>
    public void ApplyLoad(int slot)
    {
        SaveDataV data = SaveManager.Instance.Load(slot);
        ApplyData(data);
    }

    private void ApplyData(SaveDataV data)
    {
        //if (data == null || !data.IsValid())
        if (data == null)
        {
            // 세이브 없음 → 씬 기본 시작 위치 유지
            return;
        }
        StartCoroutine(ApplyPositionNextFrame(data.GetPosition()));
    }

    /// <summary>
    /// PlayerMotor.Awake 의 Rigidbody2D 초기화가 끝난 뒤 물리와 동기화하여
    /// 위치를 적용해야 하므로 WaitForFixedUpdate 후에 반영한다.
    /// 잔류 속도를 0 으로 만든 뒤 rb.position 으로 텔레포트.
    /// </summary>
    private IEnumerator ApplyPositionNextFrame(Vector2 pos)
    {
        Debug.Log($"[1] 코루틴 진입, 목표 pos={pos}");
        yield return new WaitForFixedUpdate();

        if (player == null)
        {
            Debug.LogError("[2] player 가 null");
            yield break;
        }
        Debug.Log($"[2] player={player.name} id={player.GetInstanceID()} 활성={player.gameObject.activeInHierarchy}");
        Debug.Log($"[3] 대입 전 위치={player.transform.position}");

        //player.Motor.WarpTo(pos);
        
        player.transform.position = pos;

        Debug.Log($"[4] 대입 직후 위치={player.transform.position}");
        yield return null;
        Debug.Log($"[5] 1프레임 후 위치={player.transform.position}");
    }
}