using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneHandler : MonoBehaviour 
{
	void Start () 
	{
		SceneHandler.Init ();
	}

    public void ChangeScene(string sceneName){
        SceneHandler.SwitchScene(sceneName);
    }
}