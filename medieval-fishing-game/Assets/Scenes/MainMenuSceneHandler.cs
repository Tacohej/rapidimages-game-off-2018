using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneHandler : MonoBehaviour 
{
	void Start () 
	{
		SceneHandler.Init ();
		GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayMusicFile("Music/Unsafe Waters-04");
	}

    public void ChangeScene(string sceneName){
        SceneHandler.SwitchScene(sceneName);
    }
}