using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

	GameObject player;
	GameObject pointLight;
	public Material lightOn;
	public Material lightOff;
	MeshRenderer currentMat;
	bool lightState = true;

	LevelManager levelManager;
	public float distanceCutoff;
	float lightThreshold;

	public AudioSource buzz;
	public AudioSource flicker;

	public bool doSound;
	bool inRange;

	int checkCoolDown;
	public int coolDownTime;

	void Start() {

		checkCoolDown = Random.Range(0, coolDownTime);


		player = GameObject.FindGameObjectWithTag("Player");
		
		levelManager = GameObject.Find("Level generator").GetComponent<LevelManager>();

		pointLight = transform.GetChild(0).gameObject;
		currentMat = gameObject.GetComponent<MeshRenderer>();
		lightThreshold = Random.Range(0f, 1f);

		TurnLightOn();
	}

	
	void TurnLightOn() {
		pointLight.gameObject.SetActive(true);
		currentMat.material = lightOn;
		if (doSound == true) {
			buzz.volume = 0.1f;
			buzz.Play();
		}
	}

	void TurnLightOff() {
		pointLight.gameObject.SetActive(false);
		currentMat.material = lightOff;
		if (doSound == true) {
			buzz.volume = 0f;
			if ((player.transform.position - transform.position).magnitude < distanceCutoff) {
				flicker.Play();
			}
		}

		}

		void FixedUpdate() {

		if (doSound == true) {
			checkCoolDown -= 1;

			if (checkCoolDown == 0) {
				if ((player.transform.position - transform.position).magnitude > distanceCutoff && inRange == true) {

					buzz.Stop();
					inRange = false;

				}
				if ((player.transform.position - transform.position).magnitude > distanceCutoff && inRange == false) {

					buzz.Play();
					inRange = true;

				}
				checkCoolDown = coolDownTime;
			}
		}

		if (lightThreshold < levelManager.lightCutoff) {


			if (lightState == true) {
				if (Random.Range(0, 10000) == 0) {
					TurnLightOff();
					lightState = false;
				}
			}
			if (lightState == false) {
				if (Random.Range(0, 120) == 0) {
					TurnLightOn();
					lightState = true;
				}
			}
		}
		else {
			TurnLightOff();
			lightState = false;
		}

		
		
	}
}
