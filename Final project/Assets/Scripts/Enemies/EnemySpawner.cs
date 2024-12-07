using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int spawnerID = -1;    // Set in inspector, used for saving/loading
    [Header("Spawn Settings")]
    [SerializeField] float spawnTime = 5f;
    [SerializeField] int spawnLimit = 10;
    [SerializeField] int hardSpawnLimit = -1;
    List<DefaultEnemyAI> enemies;
    Vector3 defaultSpawnPos;
    
    // Rank and level of spawned enemy
    [Header("Enemy Stats")]
    [SerializeField] int rank = 1;
    [SerializeField] int level = 0;
    [SerializeField] int patrolWidth = 1;
    [SerializeField] int patrolHeight = 1;
    [SerializeField] GameObject enemyPrefab;
    int totalSpawned = 0;

    public int counter = 0;

    void OnEnable() {
        GameController.OnSave += SaveEnemies;
        GameController.OnLoad += LoadEnemies;
    }
    void OnDisable() {
        GameController.OnSave -= SaveEnemies;
        GameController.OnLoad -= LoadEnemies;
    }

    void Start() {
        enemies = new List<DefaultEnemyAI>();

        defaultSpawnPos = transform.position;

        foreach(Transform child in transform) {
            enemies.Add(child.GetComponent<DefaultEnemyAI>());
            counter++;
        }

        SpawnEnemies(); // Start the coroutine for spawning enemies
    }

    // Spawn an enemy using the spawner's settings and the enemyPrefab
    void SpawnEnemy(Vector3 spawnPos) {
        if(spawnPos == defaultSpawnPos) {
            Vector3 spawnOffset = new Vector3(Random.Range(-patrolWidth,patrolWidth), Random.Range(-patrolHeight, patrolHeight), 0);
            spawnPos += spawnOffset;
        }

        GameObject spawnedMob = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        spawnedMob.transform.SetParent(transform);
        DefaultEnemyAI spawnedAI = spawnedMob.GetComponent<DefaultEnemyAI>();
        spawnedAI.SetID(counter);
        spawnedAI.SetCL(rank, level);   // Set the enemy's CL
        spawnedAI.SetPatrolArea(patrolWidth, patrolHeight); // Set the enemy's patrol area

        enemies.Add(spawnedAI);
        totalSpawned++;
        counter++;
    }

    // Decrement spawned mob counter when one dies and remove it from the list
    // Then, refactor the mob IDs in the list
    public void SpawnedMobDied(int enemyID) {
        enemies.RemoveAt(enemyID);
        int newID = 0;
        foreach(DefaultEnemyAI enemy in enemies) {
            enemy.SetID(newID);
            newID++;
        }
        counter--;
    }

    // Infinite coroutine, spawn enemies if counter < spawnLimit
    void SpawnEnemies() {
        StartCoroutine(SpawnEnemiesRoutine());
        IEnumerator SpawnEnemiesRoutine() {
            // Spawn enemies, wait until one dies if spawnLimit is reached
            // Spawn the first enemy without delay.
            while(true) {
                if(hardSpawnLimit != -1 && totalSpawned >= hardSpawnLimit) {
                    break;
                }

                if(counter >= spawnLimit) {
                    yield return new WaitUntil(() => counter < spawnLimit == true);
                }

                yield return new WaitForSeconds(spawnTime);

                SpawnEnemy(defaultSpawnPos);
            }
        }
    }

    // Save/load total spawned and the health and location of each currently spawned enemy
    public void SaveEnemies() {
        NDSaveLoad.SaveInt(Constants.nd_TotalSpawned + spawnerID, totalSpawned);
        for(int i = 0; i < enemies.Count; i++) {
            NDSaveLoad.SaveInt(Constants.nd_EnemyHealth + spawnerID + ":" + i, enemies[i].GetCurrentHealth());
            NDSaveLoad.SaveVector3(Constants.nd_EnemyPos + spawnerID + ":" + i, enemies[i].transform.position);
        }
    }
    public void LoadEnemies() {
        for(int i = 0; i < spawnLimit; i++) {
            // Technically this would not work if the enemy was at exactly 0,0 but it's fine for now
            // Get the saved position of the enemy and spawn it there
            Vector3 spawnPos = NDSaveLoad.LoadVector3(Constants.nd_EnemyPos + spawnerID + ":" + i, Vector3.zero);
            if(spawnPos == Vector3.zero) {
                continue;
            }
            SpawnEnemy(spawnPos);
            
            // Load the enemy's health, counter is incremented during spawn so we -1
            enemies[counter-1].SetCurrentHealth(NDSaveLoad.LoadInt(Constants.nd_EnemyHealth + spawnerID + ":" + i, enemies[counter-1].GetCurrentHealth()));
        }
    }
}
