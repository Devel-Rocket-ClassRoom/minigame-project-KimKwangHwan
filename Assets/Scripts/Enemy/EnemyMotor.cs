using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D RB => rb;
    [SerializeField] private float moveSpeed = 0.8f;
    private float moveInput;
    private bool externalControl;

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

    public void SuspendControl() => externalControl = true;
    public void ResumeControl() => externalControl = false;

    private void FixedUpdate()
    {
        if (externalControl) return;
        rb.linearVelocityX = moveInput * moveSpeed;
    }
}
