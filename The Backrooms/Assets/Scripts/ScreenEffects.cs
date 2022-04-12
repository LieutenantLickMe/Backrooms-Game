using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ScreenEffects : MonoBehaviour {

	public PlayerController playerController;
	
	Volume volume;
	public VolumeProfile volumeProfile;
	Vignette vignette;

	void Start() {
		volumeProfile.TryGet<Vignette>(out vignette);
    }


	void Update() {

		vignette.intensity.value = (100f - playerController.stamina) * (0.2f / 100f) + 0.2f;

	}
}
