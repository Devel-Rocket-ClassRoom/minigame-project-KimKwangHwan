using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D RB => rb;
    private float moveSpeed = 0.8f;
    private float moveInput;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveHorizontal(float x)
    {
        moveInput = x;
    }

    public void MoveStop()
    {
        moveInput = 0f;
    }

    private void FixedUpdate()
    {
        rb.linearVelocityX = moveInput * moveSpeed;
    }
}
