using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void NewGameButtonPressed() {
        MainController.Instance.NewGame();
    }
    public void LoadButtonPressed() {
        MainController.Instance.LoadTheGame();
    }
    public void OptionsButtonPressed() {
        MainController.Instance.ToggleOptions();
    }
    public void ExitButtonPressed() {
        MainController.Instance.Exit();
    }
}
