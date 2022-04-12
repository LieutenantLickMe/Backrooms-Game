using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOffset : MonoBehaviour {

	public Vector2 offset;
	public Vector2 magnitude;

	void Start() {

		transform.position += new Vector3(offset.x * Mathf.Round(Random.Range(-magnitude.x, magnitude.x)), 0f, offset.y * Mathf.Round(Random.Range(-magnitude.y, magnitude.y)));

	}


    void Update() {
		
	}
}
