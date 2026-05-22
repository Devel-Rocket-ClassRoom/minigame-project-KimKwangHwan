using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D RB { get { return rb; } }
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask oneWayPlatformLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float groundWidth;
    [SerializeField] private float groundHeight;
    [SerializeField] private float wallRadius;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallJumpHorizontalPower;
    [SerializeField] private float wallJumpLockTime = 0.2f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float wallCheckHeight = 0.5f;
    [SerializeField] private float wallStickSpeed = 2f;

    private Vector2 origin;
    private bool isGrounded;
    private bool wasGrounded;
    private float wallJumpLockTimer;
    private bool canWallClimbing;
    private bool isTouchingWall;
    private float moveInput;
    [SerializeField] float groundAccel = 120f;   // 크게 → 지면은 거의 즉시
    [SerializeField] float groundDecel = 120f;
    [SerializeField] float airAccel = 45f;    // 작게 → 공중은 점진적
    [SerializeField] float airDecel = 30f;
    public bool SuppressHorizontalControl { get; set; }
    private bool isOnOneWayPlatform;
    public bool IsOnOneWayPlatform() => isOnOneWayPlatform;
    [SerializeField] private float dropThroughDuration = 0.3f;
    private Coroutine CoDropThrough;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrounded = true;
        wasGrounded = true;
        canWallClimbing = false;
    }

    public void MoveHorizontal(float x)
    {
        //if (wallJumpLockTimer > 0f) return;
        moveInput = x;
    }
    public void MoveStop()
    {
        //if (wallJumpLockTimer > 0f) return;
        moveInput = 0f;
    }

    public void SetHorizontalVelocity(float vx)
    {
        rb.linearVelocity = new Vector2(vx, rb.linearVelocityY);
    }

    public bool IsGrounded() => isGrounded;
    public bool IsLanded() => isGrounded && !wasGrounded;
    public bool InAir() => !isGrounded;
    public bool ClimbCheck() => canWallClimbing;
    public bool IsTouchingWall() => isTouchingWall;
    public float GetYVelocity() => rb.linearVelocityY;

    // 수직 점프 — 카운트 안 셈. 시키면 뛸 뿐.
    public void JumpVertical()
    {
        rb.linearVelocityY = 0f;
        rb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
        wallJumpLockTimer = 0f;
    }

    // 대각 벽 점프 — 벽 방향을 인자로 받음 (+1 = 벽이 오른쪽)
    public void WallJump(float wallSide)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(-wallSide * wallJumpHorizontalPower, jumpPower),
                    ForceMode2D.Impulse);
        wallJumpLockTimer = wallJumpLockTime;
    }
    public void WallStick(float wallSide)
    {
        // 벽 쪽으로 약하게 눌러 양방향 감지가 안 풀리게
        rb.linearVelocityX = wallSide * wallStickSpeed;

        // 자유낙하 대신 일정 슬라이드 속도로 하강 (위로 솟는 속도는 그대로 둠)
        if (rb.linearVelocityY < -wallSlideSpeed)
            rb.linearVelocityY = -wallSlideSpeed;
    }

    private void FixedUpdate()
    {
        if (wallJumpLockTimer > 0f)
            wallJumpLockTimer -= Time.fixedDeltaTime;

        wasGrounded = isGrounded;
        var ground = Physics2D.OverlapBox(groundCheck.transform.position,
                                          new Vector2(groundWidth, groundHeight),
                                          0f, groundLayer);
        isGrounded = ground != null;
        var onOneWayPlatform = Physics2D.OverlapBox(groundCheck.transform.position,
                                          new Vector2(groundWidth, groundHeight),
                                          0f, oneWayPlatformLayer);
        isOnOneWayPlatform = onOneWayPlatform != null;
        origin = (Vector2)transform.position + Vector2.up * wallCheckHeight;
        bool right = Physics2D.OverlapCircle(origin + Vector2.right * wallCheckDistance,
                                             wallRadius, wallLayer) != null;
        bool left = Physics2D.OverlapCircle(origin + Vector2.left * wallCheckDistance,
                                            wallRadius, wallLayer) != null;
        isTouchingWall = left || right;

        canWallClimbing = isTouchingWall && InAir();

        if (wallJumpLockTimer <= 0f && !SuppressHorizontalControl)
        {
            float target = moveInput * moveSpeed;
            bool accelerating = Mathf.Abs(target) > 0.01f;

            float rate = isGrounded
                ? (accelerating ? groundAccel : groundDecel)
                : (accelerating ? airAccel : airDecel);

            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, target, rate * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.transform.position, new Vector3(groundWidth, groundHeight, 1f));

        Gizmos.color = Color.cyan;
        Vector2 o = (Vector2)transform.position + Vector2.up * wallCheckHeight;
        Gizmos.DrawWireSphere(o + Vector2.right * wallCheckDistance, wallRadius);
        Gizmos.DrawWireSphere(o + Vector2.left * wallCheckDistance, wallRadius);
    }
}