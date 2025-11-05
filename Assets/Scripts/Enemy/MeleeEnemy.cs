using UnityEngine;

public class MeleeEnemy : Enemy, IDamageable
{
    void Start()
    {
        Attack();
    }
    public override void Attack()
    {
        base.Attack();
    }
    
    void IDamageable.TakeDamage(float damage)
    {
        Debug.Log("Me Cago");
    }
}
