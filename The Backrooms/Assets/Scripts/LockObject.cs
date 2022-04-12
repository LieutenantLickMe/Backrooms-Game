using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockObject : MonoBehaviour {


	public GameObject obj;
	public bool xFollow;
	public bool yFollow;
	public bool zFollow;
	public bool matchWithGen;
	generator_1 genScript;

	Vector3 check;

	Vector3 start;
	Vector3 targetOffset;

	// Start is called before the first frame update
	void Start() {

		start = transform.position;
		

    }

	// Update is called once per frame
    void Update() {

		if (matchWithGen == true) {
			if (genScript == null) {

				genScript = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<generator_1>();

			}
			else {

				if (genScript.generate == false) {

					xFollow = false;
					yFollow = false;
					zFollow = false;
					start = transform.position;

				}

			}
		}

		if (obj == null) {

			obj = GameObject.FindGameObjectWithTag("Player");
			if (obj != null) {
				targetOffset = obj.transform.position;
			}

		}

		if (obj != null) {

			check = new Vector3(0f, 0f, 0f);

			targetOffset = obj.transform.position;

			if (xFollow == true) {
				check += new Vector3(1f, 0f, 0f);
			}
			if (yFollow == true) {
				check += new Vector3(0f, 0f, 0f);
			}
			if (zFollow == true) {
				check += new Vector3(0f, 0f, 1f);
			}

			transform.position = start + new Vector3(targetOffset.x * check.x, targetOffset.y * check.y, targetOffset.z * check.z);

		}

	}
}
