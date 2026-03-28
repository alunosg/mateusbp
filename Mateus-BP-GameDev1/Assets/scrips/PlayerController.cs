using UnityEngine;
using UnityEngine.InputSystem;

public class Playercontroller : MonoBehaviour
{
    public Rigidbody2D rig;
    public Collider2D col;
    public float speed = 10;
    public float jumpForce = 10;
    public LayerMask floorLayer;
    public Animator anim;
    public bool grounded =false;

    private Vector2 moveInput;


    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        anim.SetBool("IsMoving", moveInput.x != 0);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.started && grounded)
        {
            anim.SetTrigger("jump");
            rig.linearVelocity = new(rig.linearVelocity.x, jumpForce);
        }
    }

    public void GroundCheck()
    {
        Vector2 leftPoint = new(col.bounds.min.x, col.bounds.max.y);
        Vector2 rightPoint = new(col.bounds.max.x, col.bounds.max.y);

        if (Physics2D.Raycast(leftPoint, Vector2.down, col.bounds.size.y * 1.1f, floorLayer) ||
           Physics2D.Raycast(rightPoint, Vector2.down, col.bounds.size.y * 1.1f, floorLayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        anim.SetBool("IsGrounded", grounded);
    }

    public void FixedUpdate()
    {
        GroundCheck();
        rig.linearVelocity = new Vector2(moveInput.x * speed, rig.linearVelocity.y);
    }

}
