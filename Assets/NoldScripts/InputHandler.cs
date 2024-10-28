using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public FrameInput ProcessInput(Transform groundChecker, Transform ceilingChecker, ref bool isGrounded, ref bool hitCeiling)
    {
        FrameInput input;
       isGrounded = IsGrounded(groundChecker);
       hitCeiling = IsHitCeiling(ceilingChecker);

        input = new FrameInput
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            jumpPressed = Input.GetKeyDown(KeyCode.Space),
            jumpHolded = Input.GetKey(KeyCode.Space)
        };

      return input;
    }

    public bool IsGrounded(Transform groundChecker)
    {
        return Physics2D.OverlapCircleAll(groundChecker.position, 0.2f)
            .Any(collider => collider.gameObject != gameObject);
    }

    public bool IsHitCeiling(Transform ceilingChecker)
    {
        return Physics2D.OverlapCircleAll(ceilingChecker.position, 0.2f)
            .Any(collider => collider.gameObject != gameObject);
    }
}
public class FrameInput
{
    public bool jumpPressed { get; set; }
    public bool jumpHolded { get; set; }
    public Vector2 moveInput { get; set; }

    public FrameInput()
    {
        jumpPressed = false;
        jumpHolded = false;
        moveInput = Vector2.zero;
    }
}

