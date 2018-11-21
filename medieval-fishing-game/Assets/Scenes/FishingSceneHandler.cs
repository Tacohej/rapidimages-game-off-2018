using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            print("Escape Pressed!");
            SceneHandler.SwitchScene("MainMenuScene");
        }
    }
}
