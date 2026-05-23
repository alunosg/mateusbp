using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyStates { Patrol, Chase, Attack, Hit, Death }
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
        if (locked) return;

        anim.SetBool("IsWalking", Mathf.Abs(rig.linearVelocity.x) > 0.1f);

        if (Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y * 1.1f, floorLayers))
        {
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", true);
        }

        switch (state)
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
            case EnemyStates.Hit:
                HitState();
                break;
            case EnemyStates.Death:
                DeathState();
                break;
        }
    }

    private void PatrolState()
    {
        rig.linearVelocity = new Vector2(direction * speed, rig.linearVelocity.y);

        Vector2 point = new(
            col.bounds.center.x + col.bounds.extents.x * direction,
            col.bounds.center.y);

        if (!Physics2D.Raycast(point, Vector2.down, col.bounds.extents.y * 1.1f, floorLayers))
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
                rig.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    private void ChaseState()
    {
        SetDirection();
        rig.linearVelocity = new Vector2(direction * speed, rig.linearVelocity.y);
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > chaseDistance)
        {
            state = EnemyStates.Patrol;
            rig.bodyType = RigidbodyType2D.Dynamic;
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
                rig.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                state = EnemyStates.Chase;
                rig.bodyType = RigidbodyType2D.Dynamic;
            }
        }
        else
        {
            EnterAttack();
        }
    }

    private void HitState()
    {
        state = EnemyStates.Patrol;
        rig.bodyType = RigidbodyType2D.Dynamic;
    }

    private void DeathState()
    {

    }

    private void SetDirection()
    {
        direction = (player.transform.position - transform.position).normalized.x;
        if (direction > 0) direction = 1;
        else if (direction < 0) direction = -1;
        transform.localScale = new Vector2(direction, 1);
    }

    private void EnterAttack()
    {
        SetDirection();
        state = EnemyStates.Attack;
        rig.bodyType = RigidbodyType2D.Kinematic;
        anim.SetTrigger("Attack");
        anim.SetBool("IsWalking", false);
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
        rig.bodyType = RigidbodyType2D.Kinematic;
        CancelInvoke(nameof(Unlock));
        if (hitFX) Instantiate(hitFX, transform.position, transform.rotation);

        if (hp > 0)
        {
            state = EnemyStates.Hit;
            anim.SetTrigger("Hit");
            Invoke(nameof(Unlock), hitDuration);
            rig.linearVelocity = (transform.position - pos).normalized * push;
        }
        else
        {
            state = EnemyStates.Death;
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
