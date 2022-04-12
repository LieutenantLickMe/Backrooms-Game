using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Rendering;

public class ReflectionUpdate : MonoBehaviour {

	public int updateFrequency;
	int updateCount;

	Renderer meshRenderer;

	public ReflectionProbe reflection;

	void Start() {

		reflection = GetComponent<ReflectionProbe>();
		updateCount = Random.Range(0, updateFrequency);

		meshRenderer = GetComponent<Renderer>();

	}


	void FixedUpdate() {


		updateCount += 1;
		if (updateCount >= updateFrequency) {

			updateCount = 0;
//			HDAdditionalReflectionDataExtensions.RequestRenderNextUpdate(reflection);

		}

		if (meshRenderer.isVisible == true) {
			reflection.refreshMode = ReflectionProbeRefreshMode.EveryFrame;
		}
		else {
			reflection.refreshMode = ReflectionProbeRefreshMode.OnAwake;
		}

	}
}
