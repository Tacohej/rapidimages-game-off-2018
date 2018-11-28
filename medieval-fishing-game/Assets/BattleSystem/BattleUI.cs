using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

	public BattleSystem battleSystem;
	public Image playerStamina;
	public Image fishStamina;

	public Image coolDown;
	public Image escapeTimer;

	public Text fishState;
	public Text playerState;
	
	// Update is called once per frame
	void Update () {
		
		escapeTimer.fillAmount = battleSystem.escapeTimer / battleSystem.fleeAfterMilliSec;
		coolDown.fillAmount = 1 - (float)battleSystem.coolDownTime / battleSystem.MAX_COOLDOWN;
		playerStamina.fillAmount = (float)battleSystem.playerStamina / (float)battleSystem.maxPlayerStamina;
		fishStamina.fillAmount = (float)battleSystem.fishStamina / (float)battleSystem.maxFishStamina;
		fishState.text = battleSystem.fishState.ToString();
		playerState.text = battleSystem.playerState.ToString();
	}
}
