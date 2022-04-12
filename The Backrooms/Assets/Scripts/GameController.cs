using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class GameController : MonoBehaviour {

	#region Scenes

	public string mainMenu;
	public string level;

	public Scene level0;

	[Space(20)]

	#endregion

	#region Prefabs

	public GameObject playerPrefab;
	public GameObject menuPrefab;
	public GameObject volumePrefab;

	#endregion

	#region Gamestate

	[Space(20)]
	public int gameState;

	// *gameState*
	// 0 - main menu
	// 1 - in level
	// 2 - paused in level

	#endregion

	#region Object refrences

	

	bool havePlayer;
	public GameObject player;
	public Camera playerCamera;
	public GameObject gui;
	public GameObject guiContent;

	public PlayerController playerController;

	public VolumeProfile volumeProfile;

	public LevelManager levelManager;

	Scene currentScene;

	#endregion

	#region Settings

	[Space(20)]
	public float cameraFOV = 70;

	public bool ambientOcclusionState;
	AmbientOcclusion ambientOcclusion;
	public bool bloomState;
	Bloom bloom;
	public bool chromaticAberrationState;
	ChromaticAberration chromaticAberration;
	public bool colorAdjustmentsState;
	ColorAdjustments colorAdjustments;
	public bool depthOfFieldState;
	DepthOfField depthOfField;
	public bool filmGrainState;
	FilmGrain filmGrain;
	public bool fogState;
	Fog fog;
	public bool motionBlurState;
	MotionBlur motionBlur;
	public bool vignetteState;
	Vignette vignette;

	public float staminaDrain;
	public bool enableJumping;
	public bool enableBlackouts;

	#endregion


	

	public void GotoLevel() {

		SceneManager.LoadScene(level);
		gameState = 1;
		Cursor.lockState = CursorLockMode.Locked;

	}

	public void Pause() {
		gameState = 2;
		guiContent.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
	}

	public void UnPause() {

		gameState = 1;
		guiContent.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;

	}



	void Start() {

		DontDestroyOnLoad(gameObject);
		havePlayer = false;

	}

	void FixedUpdate() {

		


	}

	void Update() {

		#region Getting Object Refrences

		if (gameState == 1) {

			if (havePlayer == false && SceneManager.GetActiveScene() == SceneManager.GetSceneByName(level)) {

				player = Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f));
				gui = Instantiate(menuPrefab).transform.GetChild(0).gameObject;
				volumeProfile = Instantiate(volumePrefab).transform.GetChild(0).GetComponent<Volume>().profile;

				havePlayer = true;

				currentScene = SceneManager.GetActiveScene();

				gui = GameObject.FindGameObjectWithTag("GUI");
				player = GameObject.FindGameObjectWithTag("Player");
				playerController = player.GetComponent<PlayerController>();
				playerController.gameController = gameObject.GetComponent<GameController>();
				gui.GetComponent<MainMenuController>().gameController = gameObject.GetComponent<GameController>();
				guiContent = gui.transform.GetChild(0).gameObject;
				playerCamera = player.GetComponentInChildren<Camera>();
//				levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

//				volumeProfile = GameObject.FindGameObjectWithTag("Volume").GetComponent<Volume>().profile;

				volumeProfile.TryGet<AmbientOcclusion>(out ambientOcclusion);
				volumeProfile.TryGet<Bloom>(out bloom);
				volumeProfile.TryGet<ChromaticAberration>(out chromaticAberration);
				volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments);
				volumeProfile.TryGet<DepthOfField>(out depthOfField);
				volumeProfile.TryGet<FilmGrain>(out filmGrain);
				volumeProfile.TryGet<Fog>(out fog);
				volumeProfile.TryGet<MotionBlur>(out motionBlur);
				volumeProfile.TryGet<Vignette>(out vignette);

			}

			if (SceneManager.GetActiveScene() != currentScene) {

				havePlayer = false;

			}


		}

		#endregion

		#region Volume updating

		if (gameState == 0) {

		}

		if (havePlayer == true) {
			if (gameState == 1 || gameState == 2) {
				playerCamera.fieldOfView = cameraFOV;

				ambientOcclusion.active = ambientOcclusionState;
				bloom.active = bloomState;

				chromaticAberration.active = chromaticAberrationState;
				colorAdjustments.active = colorAdjustmentsState;
				depthOfField.active = depthOfFieldState;
				filmGrain.active = filmGrainState;
				fog.active = fogState;

				motionBlur.active = motionBlurState;
				vignette.active = vignetteState;


			}
		}

		#endregion

		#region Pause / Unpause

		if (Input.GetKeyDown(KeyCode.Escape) == true) {
			if (gameState == 1) {
				Pause();
			}
			else {
				UnPause();
			}
		}

		#endregion

	}
}
