using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class ShooterAI : Agent
{
    GameDirector director;
    [SerializeField] Transform firePoint;
    public int minStepsBetweenShots = 10;
    public int dmg = 100;
    [SerializeField] bool canShot = true;
    int stepsUntilCanShot = 0;

    Vector3 startPos;
    Rigidbody rb;
    [SerializeField] GameObject enemy;

    public int n_steps_per_run = 5000;
    int epoch;
    private void Awake()
    {
        director = FindObjectOfType<GameDirector>();
    }
    private void Start()
    {
        startPos = this.transform.position;
        rb = GetComponent<Rigidbody>();
     
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("episode starts!");
        director.SpawnEnemyAgents();
        epoch = 0;
/*        enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.transform.position);
        enemy.GetComponent<EnemyAgent>().state = EnemyAgent.EnemyState.Follow;
        enemy.transform.localPosition = new Vector3(Random.Range(-5, 5f), 1f, Random.Range(-5, 5f));
        this.transform.localPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));*/
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
        canShot = true;
    }
    private void FixedUpdate()
    {
        if (!canShot)
        {
            stepsUntilCanShot--;

            if (stepsUntilCanShot <= 0)
            {
                canShot = true;
            }
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        epoch += 1;
        if (epoch > n_steps_per_run)
        {
            AddReward(-1f);
            EndEpisode();
        }

        if (actions.ContinuousActions[0] >= 1)
        {
            Shoot();
        }

        rb.velocity = transform.forward * actions.ContinuousActions[1] * 5f;
        this.transform.Rotate(0f, actions.ContinuousActions[2] * 2, 0f);
    }
    void Shoot()
    {
        if (!canShot)
            return;

        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        Vector3 direction = transform.forward;
        Debug.Log("shhoot");
        Debug.DrawRay(firePoint.position, direction, Color.red, 20f);

        if (Physics.Raycast(firePoint.position, direction, out var hit, 200f, layerMask))
        {
            hit.transform.GetComponent<FPSAgents>().GetShot(dmg, this);
        }else
        {
            AddReward(-0.00001f);
        }

        canShot = false;
        stepsUntilCanShot = minStepsBetweenShots;
    }
    public void RegisterKill()
    {
        Debug.Log("I killed it!");
        AddReward(10f);
        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                AddReward(-10f);
                EndEpisode();
                break;
            case "Enemy":
                AddReward(-10f);         
                EndEpisode();
                break;
        }
    }
}
