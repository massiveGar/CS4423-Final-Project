using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    //[SerializeField] GameController gameController;
    [SerializeField] Player player;
    public static event Action OnTargetPressed;
    public static event Action OnInventoryPressed;
    public static event Action OnInteractPressed;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) {
            //GameController.Instance.TargetEnemy();
            OnTargetPressed?.Invoke();
        }

        for (int i = 1; i <= 5; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
                GameController.Instance.UseRing(i);
            }
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            OnInventoryPressed?.Invoke();
            //GameController.Instance.ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            OnInteractPressed?.Invoke();
        }
    }

    void FixedUpdate() {
        Vector3 movement = Vector3.zero;
        if(Input.GetKey(KeyCode.W)) {
            movement += new Vector3(0,5,0);
        }
        if(Input.GetKey(KeyCode.A)) {
            movement += new Vector3(-5,0,0);
        }
        if(Input.GetKey(KeyCode.S)) {
            movement += new Vector3(0,-5,0);
        }
        if(Input.GetKey(KeyCode.D)) {
            movement += new Vector3(5,0,0);
        }

        player.Move(movement);
    }
}
