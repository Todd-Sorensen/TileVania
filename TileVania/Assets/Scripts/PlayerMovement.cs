using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private CapsuleCollider2D feetCapsuleCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    private Vector2 moveInput;
    private bool hasHorizontalSpeed, hasVerticalSpeed;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Climbing")]
    [SerializeField] private float climbingSpeed = 10f;
    
    private float initialGravityScale;

    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialGravityScale = rb.gravityScale;  
    }

    void Update()
    {
        hasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        hasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;

        Run();
        FlipSprite();
        Climbing();
    }

    private void Climbing()
    {
        // Check if player can climb
        if (feetCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) 
        {
            // Climbing movement
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, moveInput.y * climbingSpeed);

            // Climbing animations
            animator.SetBool("isClimbable", true);
            animator.SetBool("isClimbing", hasVerticalSpeed);
        }
        else 
        {
            // Reset gravity
            rb.gravityScale = initialGravityScale;

            animator.SetBool("isClimbing", false);
            animator.SetBool("isClimbable", false);
        }
    }

    void FlipSprite()
    {
        if(hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), transform.localScale.y);
        }
    }

    private void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        animator.SetBool("isRunning", hasHorizontalSpeed);
    }

    #region Input Messages

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {

        if (!feetCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        if(value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpForce);
        }
    }

    #endregion
}
