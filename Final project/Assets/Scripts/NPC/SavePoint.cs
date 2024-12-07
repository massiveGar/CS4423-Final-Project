using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {
    bool playerIsClose = false;

    void OnBecameVisible() {
        GameController.OnInteract += InteractWithPlayer;
    }
    void OnBecameInvisible() {
        GameController.OnInteract -= InteractWithPlayer;
    }

    // Trigger interaction logic if player is nearby
    void InteractWithPlayer() {
        if(!playerIsClose) {
            return;
        }
        MainController.Instance.SaveTheGame();
    }

    // Detect when the player is nearby
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            playerIsClose = true;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            playerIsClose = false;
        }
    }
}
