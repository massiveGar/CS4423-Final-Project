using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingAnimationController : MonoBehaviour
{
    Transform ringPosition;

    void Awake() {
        ringPosition = transform.Find("RingPosition");
    }

    // AimRingAtEnemy will be called when a ring that requires a target is cast
    public bool AimRingAtEnemy(GameObject target) {
        if(target == null) {
            return false;
        }
        Vector3 direction = target.transform.position - transform.position; // Get the difference between player and target position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // -180 to 180 degrees

        transform.rotation = Quaternion.Euler(0, 0, angle);
        return true;
    }
}
