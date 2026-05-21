using UnityEngine;

public interface IMotor
{
    public Rigidbody2D RB { get; }
    public void MoveHorizontal(float x);
    public void MoveStop();
}
