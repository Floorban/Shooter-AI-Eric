using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSAgents : MonoBehaviour
{
    [Header("Attributes")]
    public int maxHealth = 100;
    public int health;
    public float moveSpeed;
    public Rigidbody rb;

    Vector3 startPos;

    void Awake()
    {
        health = maxHealth;
        startPos = transform.position;
    }
    public void GetShot(int dmg, ShooterAI shooter)
    {
        TakeDamage(dmg, shooter);
    }
    public void TakeDamage(int damageAmount, ShooterAI shooter)
    {
        health -= damageAmount;
        if (health <= 0)
            Die(shooter);
    }
    public void ApplyDamage(int dmg)
    {
        health -= dmg;
    }
    public void Die(ShooterAI shooter)
    {
        shooter.RegisterKill();
        Destroy(gameObject);
        Respawn();
    }
    public void Respawn()
    {
        health = maxHealth;
        transform.position = startPos;
    }
}
