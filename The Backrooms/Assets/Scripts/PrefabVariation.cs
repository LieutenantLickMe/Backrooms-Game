using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabVariation : MonoBehaviour {

	public GameObject variation1;
	public GameObject variation2;
	public GameObject variation3;
	public GameObject variation4;

	int rand;
	bool chosen = false;
	GameObject inst;

	void Start() {

		

		while (chosen == false) {

			rand = Random.Range(0, 4);

			if (rand == 0 && variation1 != null) {
				inst = Instantiate(variation1, transform.position, variation1.transform.rotation);
				inst.transform.parent = transform;
				chosen = true;
			}
			if (rand == 1 && variation2 != null) {
				inst = Instantiate(variation2, transform.position, variation2.transform.rotation);
				inst.transform.parent = transform;
				chosen = true;
			}
			if (rand == 2 && variation3 != null) {
				inst = Instantiate(variation3, transform.position, variation3.transform.rotation);
				inst.transform.parent = transform;
				chosen = true;
			}
			if (rand == 3 && variation4 != null) {
				inst = Instantiate(variation4, transform.position, variation4.transform.rotation);
				inst.transform.parent = transform;
				chosen = true;
			}

		}



	}

}
