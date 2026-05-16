using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rig;
    public Collider2D col;
    public float speed = 10;
    public float jumpForce = 10;
    public float jumpDuration = 0.75f;
    public LayerMask floorLayer;
    public Animator anim;
    public float attackDuration = 1.0f;
    public float airAttackDuration = 1.0f;
    public float hitDuration = 0.25f;
    public float deathDuration = 2.0f;
    public GameObject attackFX, attackHitFX, hitFX, deathFX;
    public Transform attackPoint;

    private Vector2 moveInput;
    private bool grounded = false;
    private bool locked = false;

    private float maxHp;
    public float hp = 3;
    public Image hpBar;

    void Start()
    {
        maxHp = hp;
    }

    public void GetHit(float damage, float push, Vector3 pos)
    {
        hp -= damage;
        hpBar.fillAmount = Mathf.Max(0, hp / maxHp);
        locked = true;
        CancelInvoke(nameof(Unlock));
        if (hitFX) Instantiate(hitFX, transform.position, transform.rotation);

        if (hp > 0)
        {
            anim.SetTrigger("Hit");
            Invoke(nameof(Unlock), hitDuration);
            rig.linearVelocity = (transform.position - pos).normalized * push;
        }
        else
        {
            anim.SetTrigger("Death");
            Invoke(nameof(Reload), deathDuration);
            rig.linearVelocity = Vector2.zero;
        }
    }

    private void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        anim.SetBool("IsWalking", moveInput.x != 0);
        transform.localScale = new(
            moveInput.x > 0 ? 1 : moveInput.x < 0 ? -1 : transform.localScale.x, 1);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if(!locked && context.started && grounded)
        {
            anim.SetTrigger("Jump");
            rig.linearVelocity = new Vector2
            (
                rig.linearVelocity.x,
                jumpForce
            );
        }
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (!locked && context.started)
        {
            if(grounded) rig.linearVelocity = Vector2.zero;
            anim.SetTrigger("Attack");
            locked = true;
            if(attackFX) Instantiate(attackFX, attackPoint.position, attackPoint.rotation);
            Invoke(nameof(Unlock), grounded ? attackDuration : airAttackDuration);
        }
    }

    private void Unlock() => locked = false;

    private void DetectGround()
    {
        Vector2 pLeft = new Vector2(col.bounds.min.x, col.bounds.max.y);
        Vector2 pRight = new Vector2(col.bounds.max.x, col.bounds.max.y);

        if (Physics2D.Raycast(pLeft, Vector2.down, col.bounds.size.y * 1.1f, floorLayer) ||
            Physics2D.Raycast(pRight, Vector2.down, col.bounds.size.y * 1.1f, floorLayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        anim.SetBool("IsGrounded", grounded);
    }

    private void FixedUpdate()
    {
        DetectGround();

        if(!locked)
        {
            rig.linearVelocity = new Vector2
            (
                speed * moveInput.x,
                rig.linearVelocity.y
            );
        }
    }
}
