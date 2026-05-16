using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyStates { Patrol, Chase, Attack }
    public EnemyStates state = EnemyStates.Patrol;

    public Animator anim;
    public Rigidbody2D rig;
    public Collider2D col;
    public float speed = 5;
    public LayerMask floorLayers;

    private float direction = 1;
    private PlayerController player;

    public float chaseDistance = 5;
    public float attackDistance = 1;

    public float attackDuration = 2;
    public float hitDuration = 3;
    public float deathDuration = 3;

    public Transform attackPoint;
    public GameObject attackFX, attackHitFX, hitFX, deathFX;

    public float hp = 1;
    private float maxHp;

    private bool locked = false;

    private void Start()
    {
        maxHp = hp;
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        anim.SetBool("IsWalking", Mathf.Abs(rig.linearVelocity.x) > 0.1f);

        if (Physics2D.Raycast(transform.position, Vector2.down, col.bounds.extents.y * 1.1f, floorLayers))
        {
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", true);
        }

        if (locked) return;

        switch(state)
        {
            case EnemyStates.Patrol:
                PatrolState();
                break;
            case EnemyStates.Chase:
                ChaseState();
                break;
            case EnemyStates.Attack:
                AttackState();
                break;
        }
    }

    private void PatrolState()
    {
        rig.linearVelocity = new Vector2(direction * speed, rig.linearVelocity.y);

        Vector2 point = new(
            col.bounds.center.x + col.bounds.extents.x * direction,
            col.bounds.center.y);

        if(!Physics2D.Raycast(point, Vector2.down, col.bounds.extents.y * 1.1f, floorLayers))
        {
            direction *= -1;
            transform.localScale = new Vector2(direction, 1);
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < chaseDistance)
        {
            if (distance < attackDistance)
            {
                EnterAttack();
            }
            else
            {
                state = EnemyStates.Chase;
            }
        }
    }

    private void ChaseState()
    {
        direction = (player.transform.position - transform.position).normalized.x;
        transform.localScale = new Vector2(direction, 1);

        rig.linearVelocity = new Vector2(direction * speed, rig.linearVelocity.y);

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > chaseDistance)
        {
            state = EnemyStates.Patrol;
        }
        else if (distance < attackDistance)
        {
            EnterAttack();
        }
    }

    private void AttackState()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > attackDistance)
        {
            if (distance > chaseDistance)
            {
                state = EnemyStates.Patrol;
            }
            else
            {
                state = EnemyStates.Chase;
            }
        }
        else
        {
            EnterAttack();
        }
    }

    private void EnterAttack()
    {
        direction = (player.transform.position - transform.position).normalized.x;
        transform.localScale = new Vector2(direction, 1);

        state = EnemyStates.Attack;
        anim.SetTrigger("Attack");
        if (attackFX) Instantiate(attackFX, attackPoint.position, attackPoint.rotation);
        locked = true;
        CancelInvoke(nameof(Unlock));
        Invoke(nameof(Unlock), attackDuration);
    }

    private void Unlock() => locked = false;

    public void GetHit(float damage, float push, Vector3 pos)
    {
        hp -= damage;
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
            Invoke(nameof(AutoDestroy), deathDuration);
            rig.linearVelocity = Vector2.zero;
            if (deathFX) Instantiate(deathFX, transform.position, transform.rotation);
        }
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }
}
