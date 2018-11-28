using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

	public BattleSystem battleSystem;
	public Image playerStamina;
	public Image fishStamina;
	public Text text;
	
	// Update is called once per frame
	void Update () {
		
		playerStamina.fillAmount = (float)battleSystem.playerStamina / (float)battleSystem.maxPlayerStamina;
		fishStamina.fillAmount = (float)battleSystem.fishStamina / (float)battleSystem.maxFishStamina;
		text.text = battleSystem.fishState.ToString();
	}
}
