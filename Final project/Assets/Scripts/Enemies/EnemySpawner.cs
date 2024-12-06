using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float spawnTime = 5f;
    [SerializeField] int spawnLimit = 10;
    
    // Rank and level of spawned enemy
    [Header("Enemy Stats")]
    [SerializeField] int rank = 1;
    [SerializeField] int level = 0;
    [SerializeField] int patrolWidth = 1;
    [SerializeField] int patrolHeight = 1;
    [SerializeField] GameObject enemyPrefab;

    public int counter = 0;

    void Start() {
        SpawnEnemy();
        counter++;
        SpawnEnemies();
    }

    // Spawn an enemy using the spawner's settings and the enemyPrefab
    void SpawnEnemy() {
        Vector3 spawnOffset = new Vector3(Random.Range(-patrolWidth,patrolWidth), Random.Range(-patrolHeight, patrolHeight), 0);
        Vector3 spawnPos = transform.position + spawnOffset;

        GameObject spawnedMob = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        spawnedMob.transform.SetParent(transform);
        DefaultEnemyAI spawnedAI = spawnedMob.GetComponent<DefaultEnemyAI>();
        spawnedAI.SetCL(rank, level);   // Set the enemy's CL
        spawnedAI.SetPatrolArea(patrolWidth, patrolHeight); // Set the enemy's patrol area
    }

    // Decrement spawned mob counter when one dies
    public void SpawnedMobDied() {
        counter--;
    }


    // Infinite coroutine, spawn enemies if counter < spawnLimit
    void SpawnEnemies() {
        StartCoroutine(SpawnEnemiesRoutine());
        IEnumerator SpawnEnemiesRoutine() {
            // Spawn enemies, wait until one dies if spawnLimit is reached
            while(true) {
                yield return new WaitForSeconds(spawnTime);
                if(counter >= spawnLimit) {
                    yield return new WaitUntil(() => counter < spawnLimit == true);
                }
                SpawnEnemy();
                counter++;
            }
        }
    }

}
