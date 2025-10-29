using System;
using UnityEngine;

public class Box : MonoBehaviour, IGrabeable
{

    [SerializeField] private float health;
    public void Grab()
    {
        Debug.Log("Panchitada Historica");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
