using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float acceleration;
	public float sprintAcceleration;
	public float friction;
	public float lookSens;
	public float jumpSpeed;
	public float gravity;

	bool onGround;
	Ray groundRay;
	RaycastHit groundHit;

	[Range(0f, 100f)]
	public float stamina;
	bool sprint;
	int moveState;
	public float staminaCooldown;
	public float staminaMaxCooldown;

	[Space(10)]
	public float bobSpeed;
	public float bobAmplitude;
	[Range(0f, 1f)]
	public float bobSmooth;

	float currentAcceleration;

	float mouseXInput = 0f;
	float mouseYInput = 0f;

	float mouseXSmooth = 0f;
	float mouseYSmooth = 0f;

	float mouseXSmoothSpeed = 0f;
	float mouseYSmoothSpeed = 0f;

	[Range(0f, 1f)]
	public float mouseSmooth = 0.5f;
	[Range(0f, 1f)]
	public float mouseDecay = 0.9f;
	[Range(0f, 1f)]
	public float mouseSmoothDecay = 0.9f;


	float camRotX = 0f;
	float camRotY = 0f;
	float camRotZ = 0f;

	float diagScale = 0f;

	bool keyForward = false;
	bool keyLeft = false;
	bool keyBack = false;
	bool keyRight = false;

	float time = 0f;

	public Camera playerCam;
	public Light flashLight;
	bool flashLightOn = false;

	public GameController gameController;

	float playerHVelocity;
	float playerHVeclocitySmooth;

	float eConst = 2.71828182846f;

	Vector2 camBob;

	Rigidbody rb;
	void Start() {
		rb = GetComponent<Rigidbody>();

//		Cursor.lockState = CursorLockMode.Locked;

	}

	Vector2 CameraBob(float t) {

		return (new Vector2(Mathf.Cos(t / 2f), Mathf.Sin(t) / 2f + 0.5f));
		
	}

	float AmpVel(float x) {

		return (0.2f / (1f + Mathf.Pow(eConst, -1f * (x - 3.9f))));
	}

	void Update() {

		mouseXInput = Input.GetAxis("Mouse X");
		mouseYInput = Input.GetAxis("Mouse Y");

		
		if (Input.GetMouseButtonDown(0) == true) {
//			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Cursor.lockState == CursorLockMode.Locked) {
			camRotX -= mouseYInput;
			camRotY += mouseXInput;
		}

		

		camRotX = Mathf.Clamp(camRotX, -90f, 90f);
		camRotZ = 0f;

		playerCam.transform.localRotation = Quaternion.Euler(camRotX, 0f, 0f);
		transform.rotation = Quaternion.Euler(0f, camRotY, camRotZ);

		if (Input.GetKeyDown(KeyCode.F) == true) {
			if (flashLightOn == true) {
				flashLightOn = false;
				flashLight.enabled = false;
			}
			else {
				flashLightOn = true;
				flashLight.enabled = true;
			}
		}

	}

    void FixedUpdate() {

		mouseXSmoothSpeed += mouseXInput * mouseSmooth;
		mouseYSmoothSpeed += mouseYInput * mouseSmooth;

		mouseXSmooth += mouseXSmoothSpeed;
		mouseYSmooth += mouseYSmoothSpeed;

		mouseXSmoothSpeed *= mouseDecay;
		mouseYSmoothSpeed *= mouseDecay;

		mouseXSmooth *= mouseSmoothDecay;
		mouseYSmooth *= mouseSmoothDecay;

		flashLight.transform.localRotation = Quaternion.Euler(-mouseYSmooth * 10f - 5.72f, mouseXSmooth * 10f - 2f, 0f);

		#region Gravity

		rb.velocity += Vector3.down * gravity;

		#endregion

		#region Ground detection

		groundRay = new Ray(transform.position, Vector3.down);
		Physics.Raycast(groundRay, out groundHit);
		if (groundHit.distance < 5f) {
			onGround = true;
		}
		else {
			onGround = false;
		}

		#endregion

		#region Movestate

		moveState = 0;

		keyForward = (Input.GetKey(KeyCode.W) == true);
		keyLeft = (Input.GetKey(KeyCode.A) == true);
		keyBack = (Input.GetKey(KeyCode.S) == true);
		keyRight = (Input.GetKey(KeyCode.D) == true);

		if (onGround == true) {
			if (keyForward == true || keyLeft == true || keyBack == true || keyRight == true) {

				moveState = 1;

			}
		}

		if (Input.GetKey(KeyCode.LeftShift) == true && stamina > 0.5f && moveState == 1) {

			moveState = 2;
			
		}

		if (moveState == 2) {
			currentAcceleration = sprintAcceleration;
		}
		else {
			currentAcceleration = acceleration;
		}

		if (onGround == false) {
			currentAcceleration *= 0.05f;
		}


		#endregion

		#region Horizontal movement

		

		diagScale = 0f;

		

		if ((keyForward ^ keyBack) & (keyLeft ^ keyRight)) {
			diagScale = Mathf.Pow(2f, -0.5f);
		}
		else {
			diagScale = 1f;
		}

		if (keyForward == true) {
			rb.velocity = new Vector3(rb.velocity.x + Mathf.Sin(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale, rb.velocity.y, rb.velocity.z + Mathf.Cos(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale);
		}
		if (keyLeft == true) {
			rb.velocity = new Vector3(rb.velocity.x - Mathf.Cos(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale, rb.velocity.y, rb.velocity.z + Mathf.Sin(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale);
		}
		if (keyBack == true) {
			rb.velocity = new Vector3(rb.velocity.x - Mathf.Sin(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale, rb.velocity.y, rb.velocity.z - Mathf.Cos(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale);
		}
		if (keyRight == true) {
			rb.velocity = new Vector3(rb.velocity.x + Mathf.Cos(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale, rb.velocity.y, rb.velocity.z - Mathf.Sin(Mathf.Deg2Rad * camRotY) * currentAcceleration * diagScale);
		}
		if (Input.GetKeyDown(KeyCode.V) == true && onGround == true && gameController.enableJumping == true) {
			rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
		}

		if (onGround == true) {
			rb.velocity = new Vector3(rb.velocity.x * friction, rb.velocity.y, rb.velocity.z * friction);
		}

		if (onGround == true) {
			playerHVelocity = Mathf.Pow(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f), 0.5f);
		}
		else {
			playerHVelocity = 0f;
		}

		#endregion

		#region Stamina



		if (moveState == 2) {
			stamina -= 0.05f * gameController.staminaDrain;
			stamina -= 0.05f * gameController.staminaDrain;
			staminaMaxCooldown = ((100f - stamina) + 100f) / 2f;
			staminaCooldown = staminaMaxCooldown;
		}
		if (moveState == 1) {
			if (staminaCooldown <= 0f) {
				stamina += (0.08f * stamina / 100f) + 0.005f;
			}
			staminaCooldown -= 0.1f * gameController.staminaDrain;
		}
		if (moveState == 0) {
			if (staminaCooldown <= 0f) {
				stamina += (0.12f * stamina / 100f) + 0.01f;
			}
			staminaCooldown -= 0.1f;
		}

		stamina = Mathf.Clamp(stamina, 0f, 100f);

		#endregion

		#region Camera bobbing

		playerHVeclocitySmooth += (playerHVelocity - playerHVeclocitySmooth) * bobSmooth;

		time += playerHVelocity * bobSpeed / 100f;

		camBob = CameraBob(time);

		playerCam.transform.localPosition = new Vector3(camBob.x * bobAmplitude * AmpVel(playerHVeclocitySmooth), camBob.y * bobAmplitude * AmpVel(playerHVeclocitySmooth) + 0.7f, 0f);

		#endregion


	}
}
