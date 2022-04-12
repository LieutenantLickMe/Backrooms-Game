using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DepthField : MonoBehaviour {

	RaycastHit hit;
	Ray viewRay;

	public GameObject volumeObject;

	Volume volume;
	public VolumeProfile prof;

	DepthOfField dof;

	public float width;
	public float blend;
	public float smooth;

	float distSmooth;

    void Start() {

		volume = volumeObject.GetComponent<Volume>();
		DepthOfField tmp;
		if (prof.TryGet<DepthOfField>(out tmp) == true) {
			dof = tmp;
		}

	}



	void Update() {

		viewRay = new Ray(transform.position, transform.forward);
		Physics.Raycast(viewRay, out hit);

//		Debug.Log(hit.distance);

		if (distSmooth > hit.distance && Mathf.Abs(distSmooth - hit.distance) > smooth) {
			distSmooth -= smooth;
		}
		if (distSmooth < hit.distance && Mathf.Abs(distSmooth - hit.distance) > smooth) {
			distSmooth += smooth;
		}
		if (Mathf.Abs(distSmooth - hit.distance) < smooth) {
			distSmooth = hit.distance;
		}

		if (Input.GetKey(KeyCode.L) == true || true) {

			dof.nearFocusStart.value = distSmooth - width / 2f;
			dof.nearFocusEnd.value = (distSmooth - width / 2f) - blend;


			dof.farFocusStart.value = distSmooth + width / 2f;
			dof.farFocusEnd.value = (distSmooth + width / 2f) * blend;

			dof.focusDistance.value = distSmooth;

		}

		

//		dof.farFocusStart.overrideState = true;

//		dof.farFocusStart.Override(hit.distance);

		



	}
}
