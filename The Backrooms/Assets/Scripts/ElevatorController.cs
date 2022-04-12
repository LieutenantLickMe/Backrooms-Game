using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ElevatorController : MonoBehaviour {

	public GameObject leftDoor;
	public GameObject rightDoor;
	public GameObject button1;
	public GameObject button2;
	public Light ceilingLight1;
	public Light ceilingLight2;
	public Light ceilingLight3;
	public Light ceilingLight4;



	public float doorSpeed;

	GameObject playerCamera;

	Camera mainCamera;

	bool outsideInteracable;
	bool doorsOpen;

	Vector3 leftDoorStart;
	Vector3 leftDoorTarget;

	Vector3 rightDoorStart;
	Vector3 rightDoorTarget;

	float doorLerp;

	Ray cameraRay;
	RaycastHit hit;

	void Start() {

		playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
		doorsOpen = false;

		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		leftDoorStart = leftDoor.transform.position;
		leftDoorTarget = leftDoorStart + new Vector3(0f ,0f, -1.4f);

		rightDoorStart = rightDoor.transform.position;
		rightDoorTarget = rightDoorStart + new Vector3(0f, 0f, 1.4f);

		doorLerp = 0f;

	}

	private void FixedUpdate() {
		
		if (doorsOpen == true) {

			doorLerp += doorSpeed;

		}
		else {

			doorLerp -= doorSpeed;

		}


	}

	void Update() {

		if (doorLerp > 0) {

			ceilingLight1.range = 25f;
			ceilingLight2.range = 25f;
			ceilingLight3.range = 25f;
			ceilingLight4.range = 25f;

		}
		else {

			ceilingLight1.range = 9f;
			ceilingLight2.range = 9f;
			ceilingLight3.range = 9f;
			ceilingLight4.range = 9f;

		}

		

		if (playerCamera != null) {

			cameraRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
			Physics.Raycast(cameraRay, out hit);

			if (hit.transform.gameObject == leftDoor || hit.transform.gameObject == rightDoor || hit.transform.gameObject == button1 || hit.transform.gameObject == button2) {

				outsideInteracable = true;

			}
			else {

				outsideInteracable = false;

			}

			if (outsideInteracable == true && Input.GetKeyDown(KeyCode.E) == true) {

				doorsOpen = doorsOpen != true;

				if (doorsOpen == true) {

					doorLerp = 0f;

				}
				else {

					doorLerp = 1f;

				}

			}

			leftDoor.transform.position = Vector3.Lerp(leftDoorStart, leftDoorTarget, Mathf.Clamp(doorLerp, 0f, 1f));
			rightDoor.transform.position = Vector3.Lerp(rightDoorStart, rightDoorTarget, Mathf.Clamp(doorLerp, 0f, 1f));

		}
	}

	

}
