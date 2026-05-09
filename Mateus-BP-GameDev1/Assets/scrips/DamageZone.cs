using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damage = 1;
    public float push = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().GetHit(damage, push, transform.position);
        }
    }
}
