using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float maxHp;
    public float moveSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public float dashSpeed;
}
