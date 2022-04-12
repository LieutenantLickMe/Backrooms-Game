using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

//	public Text stam;
	public Image stamCooldown;
	public Image stamProg;

	public PlayerController playerController;

	void Start() {
//		stam = GetComponent<Text>();
	}

	
	void Update() {

//		stam.text = Mathf.Round(playerController.stamina).ToString();
		stamCooldown.fillAmount = playerController.staminaCooldown / playerController.staminaMaxCooldown;
		stamProg.fillAmount = playerController.stamina / 100f;


	}
}
