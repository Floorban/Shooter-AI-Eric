using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] List<GameObject> enemies;
    int numberOfEnemies;
    [SerializeField] int maxNumberOfEnemies;

/*    [Header("Player Performance")]
    [SerializeField] float dmgInTotal;
    [SerializeField] float dps;
    [SerializeField] float playerSpeed;
    [SerializeField] int onPlayerKilled;*/

    [Header("Spawning")]
    [SerializeField] GameObject enemyPrefab;
    //[SerializeField] GameObject propsPrefab;
    [Range(1f, 10f)]
    [SerializeField] float spawnConstrains = 5f;
    [SerializeField] Vector3 notSpawnArea;
    [Range(0, 3f)]
    [SerializeField] float spawnRadius = 1.5f;
    [SerializeField] float spawnDelay;
    [SerializeField] Transform playerTransform;

/*    [Header("Enemy Attributes")]
    [SerializeField] int maxHealth;
    [SerializeField] float g_DmgFactor;
    [SerializeField] float speed, chasingSpeed;*/
    void OnEnable()
    {
        Actions.EnemyOnKilled += TrackingPlayerPerformance;
    }
    void OnDisable()
    {
        Actions.EnemyOnKilled -= TrackingPlayerPerformance;
    }
    void Start()
    {
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        SetSpawnArea(spawnConstrains);
        //TrackingPlayerPerformance();
    }
    void TrackingPlayerPerformance()
    {
        
    }

    void SetSpawnArea(float radius)
    {
        notSpawnArea = new Vector3(radius, 0, radius);
    }

    void SpawnEnemyAgent()
    {
        Vector3 position = GetRandomPosition();

        position += playerTransform.position;

        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.GetComponent<EnemyAgent>().centerPoint = position;
        newEnemy.transform.position = position;
        RegisterAgent(newEnemy);
    }
    public void SpawnEnemyAgents()
    {
        CancelAgent();

        for (int i = 0; i < maxNumberOfEnemies; i++)
        {
            SpawnEnemyAgent();
        }
    }
    IEnumerator SpawnEnemies()
    {
        while (numberOfEnemies < maxNumberOfEnemies)
        {
            //Vector3 position = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), 0, Random.Range(-spawnArea.z, spawnArea.z));
            Vector3 position = GetRandomPosition();

            position += playerTransform.position;

            GameObject newEnemy = Instantiate(enemyPrefab);
            newEnemy.GetComponent<EnemyAgent>().centerPoint = position;
            newEnemy.transform.position = position;
            RegisterAgent(newEnemy);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
    Vector3 GetRandomPosition()
    {
        /// To prevent the enemy appears right near the player
        Vector3 position = new Vector3();
        float f = Random.value > 0.5f ? -spawnRadius : spawnRadius;
        if (Random.value > 0.5f)
        {
            position.x = Random.Range(-notSpawnArea.x, notSpawnArea.x);
            position.z = Random.Range(spawnConstrains, notSpawnArea.z * f);
        }
        else
        {
            position.z = Random.Range(-notSpawnArea.z, notSpawnArea.z);
            position.x = Random.Range(spawnConstrains, notSpawnArea.x * f);
        }
        position.y = 0;
        return position;
    }
    void RegisterAgent(GameObject enemy)
    {
        enemies.Add(enemy);
        numberOfEnemies++;
    }

    void CancelAgent()
    {
        foreach (GameObject enemy in enemies)
            Destroy(enemy);
    }
    void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        //Handles.DrawWireCube(playerTransform.position, notSpawnArea);
        Handles.DrawWireArc(playerTransform.position, Vector3.up, Vector3.forward, 360f, spawnConstrains);
        Handles.DrawWireArc(playerTransform.position, Vector3.up, Vector3.forward, 360f, spawnConstrains * spawnRadius);
    }
}
