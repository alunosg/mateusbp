using UnityEngine;
using UnityEngine.InputSystem;

public class Playercontroller : MonoBehaviour
{
    public Rigidbody2D rig;
    public Collider2D col;
    public float speed = 10;
    public float jumpForce = 10;
    public LayerMask floorLayer;

    private Vector2 moveInput;


    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Vector2 leftPoint = new(col.bounds.min.x, col.bounds.max.y);
        Vector2 rightPoint = new(col.bounds.max.x, col.bounds.max.y);

        if (Physics2D.Raycast(leftPoint, Vector2.down, col.bounds.size.y * 1.1f, floorLayer) ||
           Physics2D.Raycast(rightPoint, Vector2.down, col.bounds.size.y * 1.1f, floorLayer))
        {
            rig.linearVelocity = new(rig.linearVelocity.x, jumpForce);
        }
    }

    public void FixedUpdate()
    {
        rig.linearVelocity = new Vector2(moveInput.x * speed, rig.linearVelocity.y);
    }

}
