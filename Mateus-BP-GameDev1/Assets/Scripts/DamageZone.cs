using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damage = 1;
    public float push = 5;

    public bool hitPlayer = true;

    public bool hitEnemy = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(hitPlayer && collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().GetHit(damage, push, transform.position);
        }
        else if(hitEnemy && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().GetHit(damage, push, transform.position);
        }
    }
}
