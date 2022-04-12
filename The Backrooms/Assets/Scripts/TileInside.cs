using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInside : MonoBehaviour {

	public bool placedInside = false;
	bool collisionCheck;
	bool result;
	bool done;
	bool ready;
	bool respond;


	public GameObject levelTile;
	public int childNum;
	public int recordedJoint;
	public float offsetAngle;

	public generator_2 genComp;

	public bool compute;

	public Vector3 pos;
	public Quaternion rot;

	int frame;
	int frame2;

	void Start() {

		result = false;
		done = false;
		collisionCheck = true;
		ready = false;
		respond = false;

		frame = 0;
		frame2 = 0;

	}


	private void OnCollisionStay(Collision collision) {

		if (collision.gameObject.tag == "Bounding" && compute == false) {

			Debug.Log("Failed :(");

		}

		if (compute == true) {
			if (done == false) {
				if (collision.gameObject.tag == "Bounding") {

//					Debug.Log("Collided with bounding");
					result = true;

				}

				if (collision.gameObject.tag == "Collision dummy") {

//					Debug.Log("Collided with dummy");
					ready = true;

				}
			}
		}

	}


	void FixedUpdate() {


		if (ready == true && done == false && compute == true) {

			done = true;

			Debug.Log("Result: " + result);

			if (result == false) {

				genComp.ConnectTile(levelTile, childNum, recordedJoint, offsetAngle);
				Destroy(gameObject);

			}
			else {

				genComp.IncrementJoinerFails(recordedJoint);
				Destroy(gameObject);

			}

		}

	}
}