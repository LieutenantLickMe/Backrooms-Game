using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour {

	public GameController gameController;

	public float fieldOfView;
	public float staminaDrainRate;

	[Space(20f)]

//	public bool[] graphicSetting = new bool[9];

	float[] fpsList = new float[32];
	float fpsSum;

	public TMP_Text fpsCounter;

	public GameObject[] sliderInput = new GameObject[2];
	TMP_InputField[] optionText = new TMP_InputField[2];
	Slider[] optionSlider = new Slider[2];
	bool changeField;
	int optionIndex;
	float sliderMin;
	float sliderMax;

	public Image crosshair;

	[Space(20f)]

	public Toggle enableJumping;
	public Toggle enableBlackouts;
	public Toggle ambientOcclusion;
	public Toggle bloom;
	public Toggle chromaticAbberation;
	public Toggle colorAdjustments;
	public Toggle depthOfField;
	public Toggle filmGrain;
	public Toggle fog;
	public Toggle motionBlur;
	public Toggle vignette;


	void Start() {

		for (int i = 0; i < 32; i += 1) {

			fpsList[i] = 0f;

		}

		for (int i = 0; i < 2; i += 1) {

			optionText[i] = sliderInput[i].GetComponentInChildren<TMP_InputField>();
			optionSlider[i] = sliderInput[i].GetComponentInChildren<Slider>();

		}


		LoadSettings();


		



	}

	void Update() {

		if (Input.GetKeyDown(KeyCode.Tab) == true) {
//			LoadSettings();
		}

		for (int i = 31; i > 0; i -= 1) {
			fpsList[i] = fpsList[i - 1];
		}
		fpsList[0] = 1f / Time.deltaTime;

		fpsSum = 0f;

		for (int i = 0; i < 32; i += 1) {

			fpsSum += fpsList[i];

		}

		fpsSum /= 32f;
		fpsSum = Mathf.Round(fpsSum);

		fpsCounter.text = "FPS: " + fpsSum.ToString();

		if (changeField == true && optionText[optionIndex].text != "") {
			optionSlider[optionIndex].value = (float.Parse(optionText[optionIndex].text) - sliderMin) / (sliderMax - sliderMin);
		}
		
		staminaDrainRate = float.Parse(optionText[0].text);
		fieldOfView = float.Parse(optionText[1].text);

//		gameController.cameraFOV = fieldOfView;
//		gameController.staminaDrain = staminaDrainRate;

		if (crosshair != null) {
			if (gameController.gameState == 1) {

				crosshair.enabled = true;

			}
			else {

				crosshair.enabled = false;

			}

		}

	}

	public void ResetDefaults() {

		optionText[0].text = "0.2";
		optionSlider[0].value = 0.1f;

		optionText[1].text = "70";
		optionSlider[1].value = 0.2f;

		enableJumping.isOn = true;
		enableBlackouts.isOn = true;
		ambientOcclusion.isOn = true;
		bloom.isOn = true;
		chromaticAbberation.isOn = true;
		colorAdjustments.isOn = true;
		depthOfField.isOn = false;
		filmGrain.isOn = false;
		fog.isOn = true;
		motionBlur.isOn = true;
		vignette.isOn = true;

	}

	public void LoadSettings() {

		optionText[0].text = PlayerPrefs.GetFloat("StaminaDrain").ToString();
		optionSlider[0].value = PlayerPrefs.GetFloat("StaminaDrain") / 2f;

		optionText[1].text = PlayerPrefs.GetFloat("Camera FOV").ToString();
		optionSlider[1].value = (PlayerPrefs.GetFloat("Camera FOV") - 60f) / 50f;

		enableJumping.isOn = IntToBool(PlayerPrefs.GetInt("Enable Jumping"));
		enableBlackouts.isOn = IntToBool(PlayerPrefs.GetInt("Enable Blackouts"));
		ambientOcclusion.isOn = IntToBool(PlayerPrefs.GetInt("Ambient Occlusion"));
		bloom.isOn = IntToBool(PlayerPrefs.GetInt("Bloom"));
		chromaticAbberation.isOn = IntToBool(PlayerPrefs.GetInt("Chromatic Abberation"));
		colorAdjustments.isOn = IntToBool(PlayerPrefs.GetInt("Color Adjustments"));
		depthOfField.isOn = IntToBool(PlayerPrefs.GetInt("Depth of Field"));
		filmGrain.isOn = IntToBool(PlayerPrefs.GetInt("Film Grain"));
		fog.isOn = IntToBool(PlayerPrefs.GetInt("Fog"));
		motionBlur.isOn = IntToBool(PlayerPrefs.GetInt("Motion Blur"));
		vignette.isOn = IntToBool(PlayerPrefs.GetInt("Vignette"));



	}

	public void OnfieldEnter(int index) {


		switch (index) {

		case 0:

			//Stamina drain

			sliderMin = 0f;
			sliderMax = 2f;

			break;

		case 1:

			//Fov slider

			sliderMin = 60f;
			sliderMax = 110f;

			break;

		}

		changeField = true;
		optionIndex = index;


	}
	public void OnfieldExit() {

		changeField = false;

	}
	public void onSlider(int index) {

		if (changeField == false) {

			switch (index) {

			case 0:

				//Stamina drain

				sliderMin = 0f;
				sliderMax = 2f;

				optionText[index].text = (Mathf.Round((optionSlider[index].value * (sliderMax - sliderMin) + sliderMin) * 100f) / 100f).ToString();

				break;

			case 1:

				//Fov slider

				sliderMin = 60f;
				sliderMax = 110f;

				optionText[index].text = Mathf.Round(optionSlider[index].value * (sliderMax - sliderMin) + sliderMin).ToString();

				break;

			}
		}

		

	}
	public void PlayGame() {

		gameController.GotoLevel();

	}

	public void ResumeGame() {

		gameController.UnPause();

	}

	public void QuitGame() {

		Debug.Log("Quit");
		Application.Quit();
	}

	public void ApplySettings() {

		gameController.staminaDrain = staminaDrainRate;
		gameController.enableJumping = enableJumping.isOn;
		gameController.enableBlackouts = enableBlackouts.isOn;
		gameController.cameraFOV = float.Parse(optionText[1].text);
		gameController.ambientOcclusionState = ambientOcclusion.isOn;
		gameController.bloomState = bloom.isOn;
		gameController.chromaticAberrationState = chromaticAbberation.isOn;
		gameController.colorAdjustmentsState = colorAdjustments.isOn;
		gameController.depthOfFieldState = depthOfField.isOn;
		gameController.filmGrainState = filmGrain.isOn;
		gameController.fogState = fog.isOn;
		gameController.motionBlurState = motionBlur.isOn;
		gameController.vignetteState = vignette.isOn;


		PlayerPrefs.SetFloat("Stamina Drain", staminaDrainRate);
		PlayerPrefs.SetInt("Enable Jumping", BoolToInt(enableJumping.isOn));
		PlayerPrefs.SetInt("Enable Blackouts", BoolToInt(enableBlackouts.isOn));
		PlayerPrefs.SetFloat("Camera FOV", float.Parse(optionText[1].text));
		PlayerPrefs.SetInt("Ambient Occlusion", BoolToInt(ambientOcclusion.isOn));
		PlayerPrefs.SetInt("Bloom", BoolToInt(bloom.isOn));
		PlayerPrefs.SetInt("Chromatic Abberation", BoolToInt(chromaticAbberation.isOn));
		PlayerPrefs.SetInt("Color Adjustments", BoolToInt(colorAdjustments.isOn));
		PlayerPrefs.SetInt("Depth of Field", BoolToInt(depthOfField.isOn));
		PlayerPrefs.SetInt("Film Grain", BoolToInt(filmGrain.isOn));
		PlayerPrefs.SetInt("Fog", BoolToInt(fog.isOn));
		PlayerPrefs.SetInt("Motion Blur", BoolToInt(motionBlur.isOn));
		PlayerPrefs.SetInt("Vignette", BoolToInt(vignette.isOn));

	}

	public void CancelSettings() {

		staminaDrainRate = gameController.staminaDrain;
		enableJumping.isOn = gameController.enableJumping;
		enableBlackouts.isOn = gameController.enableBlackouts;
		ambientOcclusion.isOn = gameController.ambientOcclusionState;
		bloom.isOn = gameController.bloomState;
		chromaticAbberation.isOn = gameController.chromaticAberrationState;
		colorAdjustments.isOn = gameController.colorAdjustmentsState;
		depthOfField.isOn = gameController.depthOfFieldState;
		filmGrain.isOn = gameController.filmGrainState;
		fog.isOn = gameController.fogState;
		motionBlur.isOn = gameController.motionBlurState;
		vignette.isOn = gameController.vignetteState;


	}
	
	int BoolToInt(bool arg) {

		if (arg == true) {
			return (1);
		}
		else {
			return (0);
		}

	}

	bool IntToBool(int arg) {

		if (arg == 1) {
			return (true);
		}
		else {
			return (false);
		}

	}

}
