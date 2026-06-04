using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BossRoomData WitchBossRoom;
}

[System.Serializable]
public struct BossRoomData
{
    public BossRoom bossRoom;
    public EnemyHealth boss;
    public Door door;
}