using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb2d;
    private float currentHorizontalSpeed = 0;
    private float coyoteTimeCounter = 0;
    private bool isHovering = false;
    private float hoverCounter = 0;

    public float horizontalAcceleration = 0;
    public float maxHorizontalSpeed = 0;
    public float jumpForce = 0;
    public float hoverDuration = 0;
    public float jumpHoldForce = 0;
    public float jumpHoldAcceleration = 0;
    private float currentJumpHoldForce = 0;
    public float gravityForce = -9;
    public float fallMultiplier;

    public void Initialize(Rigidbody2D rb)
    {
        if (rb2d == null)
            rb2d = rb;

        currentJumpHoldForce = jumpHoldForce;
    }

    public void HandleMovement(FrameInput input, bool isGrounded)
    {
        if (input.moveInput.x != 0)
        {
            currentHorizontalSpeed += input.moveInput.x * horizontalAcceleration * Time.deltaTime;
            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -maxHorizontalSpeed, maxHorizontalSpeed);
        }
        else
        {
            currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, horizontalAcceleration * Time.deltaTime);
        }

        if (isGrounded)
        {
            coyoteTimeCounter = 0.2f;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        rb2d.velocity = new Vector2(currentHorizontalSpeed, rb2d.velocity.y);
    }

    public void HandleCeilingCollision(bool hitCeiling)
    {
        if (hitCeiling && rb2d.velocity.y > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        }
    }

    public void HandleJumping(bool isGrounded, FrameInput input)
    {
        if (input.jumpPressed)
        {
            Debug.Log($"HandleJumping called - isGrounded: {isGrounded}, coyoteTimeCounter: {coyoteTimeCounter}, jumpPressed: {input.jumpPressed}");
        }

        if ((isGrounded || coyoteTimeCounter > 0) && input.jumpPressed)
        {
            Debug.Log("Calling StartJump");
            StartJump();
        }
        else if (input.jumpHolded && rb2d.velocity.y > 0)
        {
            ContinueJump(input);
        }
        else if (isHovering && hoverCounter <= 0)
        {
            EndHover(isGrounded);
        }
        else if (!input.jumpHolded)
        {
            isHovering = false;
        }

        hoverCounter -= Time.deltaTime;
    }

    private void StartJump()
    {
        Debug.Log("StartJump called");
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        hoverCounter = hoverDuration;
        isHovering = false;
        coyoteTimeCounter = 0;
        currentJumpHoldForce = jumpHoldForce;
    }

    private void ContinueJump(FrameInput input)
    {
        if (input.jumpHolded && currentJumpHoldForce > 0)
        {
            rb2d.velocity += Vector2.up * currentJumpHoldForce * Time.deltaTime;
            currentJumpHoldForce -= jumpHoldAcceleration * Time.deltaTime;
        }
        else if (hoverCounter > 0 && !isHovering)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            isHovering = true;
        }
    }

    private void EndHover(bool isGrounded)
    {
        isHovering = false;
        ApplyGravity(isGrounded);
    }
    public void ApplyGravity(bool isGrounded)
    {
        if (!isGrounded && !isHovering)
        {
            float gravity = gravityForce * Time.deltaTime;
            if (rb2d.velocity.y < 0)
            {
                rb2d.velocity += Vector2.up * gravity * fallMultiplier;
            }
            else
            {
                rb2d.velocity += Vector2.up * gravity;
            }
        }
    }
}
