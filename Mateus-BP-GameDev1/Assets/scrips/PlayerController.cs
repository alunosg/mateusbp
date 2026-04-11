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

    public float attackDuration = 1.0f;

    public float airAttackDuration = 1.0f;

    public GameObject attackFX, attackHitFX, hitFX, deathFX;

    public Transform attackPoint; 
    
    private bool grounded =false;

    private bool locked  =false;


    private Vector2 moveInput;


    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        anim.SetBool("IsMoving", moveInput.x != 0);
        transform.localScale = new(
            moveInput.x > 0 ? 1 : moveInput.x < 0 ? -1 : transform.localScale.x, 1);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(!locked && context.started && grounded)
        {
            anim.SetTrigger("jump");
            rig.linearVelocity = new(rig.linearVelocity.x, jumpForce);
        }
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (!locked && context.started)
        {
            
            anim.SetTrigger("Attack");
            locked = true;
            if (attackFX) Instantiate(attackFX, attackPoint.position, attackPoint.rotation);
            Invoke(nameof(Unlock), grounded ? attackDuration : airAttackDuration);
        }
    }

    private void Unlock() => locked =false;
       
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
        rig.linearVelocity = new Vector2(
            locked && grounded ? 0 : speed * moveInput.x, 
            rig.linearVelocity.y);
    }

}
