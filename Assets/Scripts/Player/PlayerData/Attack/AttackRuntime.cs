using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public sealed class AttackRuntime
{
    public IMotor Motor;
    public Hitbox Hitbox;
    public Transform MuzzleOrigin;
    public float Facing;
}