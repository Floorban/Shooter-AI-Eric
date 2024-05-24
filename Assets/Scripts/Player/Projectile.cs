using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public int dmg = 10;
    float destroyTime = 2f;
    float elapsedTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        OptimizeProjectiles();
    }
    void OptimizeProjectiles()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= destroyTime) 
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Wall":
                Destroy(gameObject);
                break;
            case "Enemy":
                other.gameObject.GetComponent<FPSAgents>().ApplyDamage(dmg);
                other.gameObject.GetComponent<EnemyAgent>().isHit = true;
                Destroy(gameObject);
                break;
        }
    }
}
