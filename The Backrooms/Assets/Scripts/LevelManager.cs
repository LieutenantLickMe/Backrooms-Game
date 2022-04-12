using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	[Range(0f, 1f)]
	public float lightCutoff;

	public int lightState = 1;

	public int timeToBlackout = 72000;
	bool blackout;

	void Start() {
	
	}

	
    void FixedUpdate() {

		timeToBlackout -= 1;
		if (timeToBlackout == 0) {
			if (blackout == false) {

				timeToBlackout = Random.Range(28800, 57600);
				lightState = 0;
				blackout = true;

			}
			else {

				timeToBlackout = Random.Range(57600, 72000);
				lightState = 2;
				blackout = false;

			}

		}

//		if (Input.GetKeyDown(KeyCode.K) == true) {
//			lightState = 0;
//		}
//		if (Input.GetKeyDown(KeyCode.L) == true) {
//			lightState = 2;
//		}
		if (lightState == 0) {
			lightCutoff -= 0.0008f;
			if (lightCutoff <= 0f) {
				lightState = 1;
			}
		}
		if (lightState == 2) {
			lightCutoff += 0.0002f;
			if (lightCutoff >= 1f) {
				lightState = 1;
			}
		}

		lightCutoff = Mathf.Clamp(lightCutoff, 0f, 1f);
	}
}
