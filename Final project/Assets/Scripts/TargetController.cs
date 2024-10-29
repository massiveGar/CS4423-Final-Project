using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    List<GameObject> visibleEnemies;
    List<GameObject> alreadyTargetedEnemies;
    //[SerializeField] GameController gameController;
    Player player;
    GameObject target;
    GameObject targetIndicator;

    void OnEnable() {
        PlayerInputHandler.OnTargetPressed += TargetEnemy;
    }

    void OnDisable() {
        PlayerInputHandler.OnTargetPressed -= TargetEnemy;
    }

    // Start is called before the first frame update
    void Start()
    {
        visibleEnemies = new List<GameObject>();
        alreadyTargetedEnemies = new List<GameObject>();
        player = GameController.Instance.GetPlayer();

        // Get the target indicator's GameObject and disable it for now 
        targetIndicator = transform.GetChild(0).gameObject;
        targetIndicator.SetActive(false);
    }

    public GameObject GetTarget() {
        return target;
    }

    public void AddVisibleEnemy(GameObject enemy) {
        visibleEnemies.Add(enemy);
    }

    public void RemoveInvisibleEnemy(GameObject enemy) {
        if(target == enemy) {
            target = null;
            targetIndicator.SetActive(false);
        }
        visibleEnemies.Remove(enemy);
        alreadyTargetedEnemies.Remove(enemy);
    }
    
    void SortEnemyList() { 
        Vector3 playerPosition = player.transform.position;

        // Sort based on distance from player
        visibleEnemies.Sort((a, b) => 
        {
            float distanceA = Vector3.Distance(a.transform.position, playerPosition);
            float distanceB = Vector3.Distance(b.transform.position, playerPosition);
            return distanceA.CompareTo(distanceB);
        });
    }

    public void TargetEnemy() {
        if(visibleEnemies.Count == 0) {
            if(alreadyTargetedEnemies.Count == 0) {
                return;
            }
            // Restart the cycle by adding the already targeted enemies back into the list
            visibleEnemies.AddRange(alreadyTargetedEnemies);
            alreadyTargetedEnemies.Clear();
        }
        SortEnemyList();

        // Logic to retarget when the nearest enemy is already targeted
        if(visibleEnemies.Count > 1 && target == visibleEnemies[0]) {
            alreadyTargetedEnemies.Add(target);
            visibleEnemies.Remove(target);
        }

        // Set the target to the closest enemy that has not been targeted in this cycle
        target = visibleEnemies[0];
        // Move the target indicator to the target
        targetIndicator.transform.SetParent(target.transform, worldPositionStays:false);
        //targetIndicator.transform.localPosition = new Vector3(0,-0.25f, 0);
        targetIndicator.SetActive(true);

        // When an enemy is targeted, remove it from the main list and add it to alreadyTargeted
        alreadyTargetedEnemies.Add(target);
        visibleEnemies.Remove(target);
    }
}
