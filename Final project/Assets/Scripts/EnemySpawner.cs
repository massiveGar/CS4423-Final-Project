using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float timer = 0;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnTime = 5f;
    [SerializeField] float spawnRadius = 25;
    [SerializeField] int spawnLimit = 10;   
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy();
        SpawnEnemies();
    }

    void SpawnEnemy() {
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), 0) * spawnRadius;
        GameObject spawnedMob = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        spawnedMob.transform.SetParent(transform);
    }

    public void SpawnedMobDied() {
        counter--;
    }

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
