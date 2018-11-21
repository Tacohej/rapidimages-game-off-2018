using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
//using UnityEditor;

/// <summary>
/// Class containing methods for changing and exiting the current scene.
/// </summary>
public class SceneHandler
{
	private static bool alreadyInitialized = false;
	private static Stack sceneStack;
	private static AssetBundle myLoadedAssetBundle;
	private static string[] scenePaths;
	private static ArrayList scenePathsList;

	public static void Init()
	{
		if (!alreadyInitialized)
		{
			scenePathsList = new ArrayList ();
			sceneStack = new Stack ();

			scenePathsList.Add("SampleScene");
			scenePathsList.Add("MainMenuScene");
		}
	}

	public static void SwitchScene(string sceneName)
	{
		if (scenePathsList.Contains (sceneName))
		{
			sceneStack.Push (SceneManager.GetActiveScene().name);
			SceneManager.LoadScene (sceneName);
		}
	}

	public static void GoBackToScene(string sceneName)
	{
		//Pop stack until a matching scene is found.
		while (!sceneName.Equals ((string)sceneStack.Peek ()) && sceneStack.Count > 1)
		{
			sceneStack.Pop ();
		}
		string newSceneName = (string)sceneStack.Pop ();
		if (scenePathsList.Contains(newSceneName))
		{
			SceneManager.LoadScene(newSceneName);
		}
	}
}