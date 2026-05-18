using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private GameObject groundCheck;
    [SerializeField]
    private GameObject wallCheck;
    [SerializeField]
    private LayerMask jumpLayer;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private float groundRadius;
    [SerializeField] 
    private float wallRadius;
    [SerializeField]
    private float wallSlideSpeed;

    private bool isGrounded;
    private bool wasGrounded;
    private int jumpCount;
    private bool isWallClimbing;
    private bool canWallClimbing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpCount = 0;
        isGrounded = true;
        wasGrounded = true;
        isWallClimbing = false;
        canWallClimbing = false;
    }
    public void MoveHorizontal(float x)
    {
        rb.linearVelocityX = x * moveSpeed;
    }
    public void MoveStop()
    {
        rb.linearVelocityX = 0f;
    }
    public bool IsGrounded() => isGrounded;
    public bool IsLanded() => isGrounded && !wasGrounded;
    public bool InAir() => !isGrounded;
    public bool CanJump() => jumpCount < 2;
    public int JumpCount => jumpCount;
    public bool ClimbCheck() => canWallClimbing;
    public void JumpReset() => jumpCount = 0;
    public bool WallClimbing() => isWallClimbing;
    public void WallClimb()
    {
        isWallClimbing = true;
    }
    public void SeperateToWall()
    {
        isWallClimbing = false;
    }

    public float GetYVelocity()
    {
        return rb.linearVelocityY;
    }

    public void JumpVertical()
    {
        rb.linearVelocityY = 0f;
        rb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
        jumpCount++;
    }
    private void FixedUpdate()
    {
        if (IsLanded())
        {
            JumpReset();
        }
        wasGrounded = isGrounded;
        var ground = Physics2D.OverlapCircle(groundCheck.transform.position, groundRadius, jumpLayer);
        isGrounded = ground != null;

        var wall = Physics2D.OverlapCircle(wallCheck.transform.position, wallRadius, wallLayer);
        canWallClimbing = wall != null && InAir();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundRadius);
        Gizmos.DrawWireSphere(wallCheck.transform.position, wallRadius);
    }
}