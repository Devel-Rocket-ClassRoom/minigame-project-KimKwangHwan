using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D RB => rb;
    [SerializeField] private float moveSpeed = 0.8f;
    private float moveInput;
    private bool externalControl;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float wallCheckDist = 0.3f;
    [SerializeField] private float cliffCheckDown = 0.5f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform cliffCheck;
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

    public bool IsBlocked(float dir)
    {
        if (dir == 0f) return false;
        if (Physics2D.Raycast(wallCheck.position, Vector2.right * dir, wallCheckDist, groundMask).collider != null)
        {
            return true;
        }
        if (Physics2D.Raycast((Vector2)cliffCheck.position, Vector2.down, cliffCheckDown, groundMask).collider == null)
        {
            return true;
        }
        return false;
    }
}
