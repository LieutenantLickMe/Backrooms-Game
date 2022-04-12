using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generator_2 : MonoBehaviour {

	public bool useSeed;
	public int seed;
	public bool useTest;

	[Space(20)]

	public int levelTiles;
	public int maxJoinFails;
	int maxRecordedTiles = 100;
	int maxRecordedjoins = 300;

	[Space(20)]

	public bool generateSingle;
	public bool autoGenerate;
	public int autoGenerateRate;
	int autoGenerateTimer;

	[Space(20)]

	public GameObject[] levelTile = new GameObject[3];


	public GameObject[] recordedTile = new GameObject[100];
	public GameObject[] recordedJoin = new GameObject[300];

//	public GameObject[] joiner = new GameObject[2];

//	make sure maxRecordedTiles and the size of these arrays are the same!!!

	public int tiles;

	int chosenTile;

	GameObject joinCheck;
	joiner joinerComp;
	GameObject tryJoin;
	GameObject tryChild;

	GameObject newTile;
	int newJoiner;
	
	float joinType;
	bool placed;

	public int loopLimit;
	int loops;

	float offsetAng;

	public GameObject tester;
	GameObject newTest;


//	You should really make sure the joints generated first are connected up to other tiles first
//	This happens anyway be default because im a smart boy but just double check :-)
//	Could store ready joiners in another array??

	void Start() {

		if (useSeed == true) {
			Random.InitState(seed);
		}
		else {

			seed = Random.Range(0, 2147483647);
			Random.InitState(seed);

		}

		for (int i = 0; i < maxRecordedTiles; i += 1) {

			recordedTile[i] = null;

		}

		tiles = 0;
	}


	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1) == true) {
			newTile = Instantiate(GetChildWithTag(levelTile[0].transform, "Bounding").gameObject, GetChildWithTag(levelTile[0].transform, "Bounding").position, GetChildWithTag(levelTile[0].transform, "Bounding").rotation);
			
			newTest = Instantiate(tester, levelTile[0].transform.GetChild(1).position, Quaternion.identity);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) == true) {
			newTest.transform.parent = newTile.transform;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) == true) {
			newTile.transform.rotation = Quaternion.Euler(newTile.transform.rotation.eulerAngles + new Vector3(0f, -90f, 0f));
		}
		if (Input.GetKeyDown(KeyCode.Alpha4) == true) {
			newTile.transform.position += (recordedJoin[0].transform.position - newTest.transform.position);
		}



		if (autoGenerate == true) {
			autoGenerateTimer += 1;

			if (autoGenerateTimer >= autoGenerateRate) {

				generateSingle = true;
				autoGenerateTimer = 0;

			}

		}

		if (Input.GetKeyDown(KeyCode.Return) == true || generateSingle == true) {

			generateSingle = false;
			Debug.Log("------------------------------");

//			Debug.Log("Attempting to place a tile");

			if (tiles == 0) {

				PlaceTileFirst();

			}
			else {

				PlaceTile();

			}


		}

	}

	int AddToTileList(GameObject obj) {

		for (var i__ = 0; i__ < maxRecordedTiles; i__ += 1) {

			if (recordedTile[i__] == null) {

				recordedTile[i__] = obj;
				i__ = maxRecordedTiles + 1;
				return (i__);
				

			}

		}

		return (maxRecordedTiles + 1);

	}

	int AddToJoinerList(GameObject obj) {

		for (int J_ = 0; J_ < maxRecordedjoins; J_ += 1) {

			if (recordedJoin[J_] == null) {

				recordedJoin[J_] = joinCheck;

//				Adds all the children with the tag to the joiner array

				J_ = maxRecordedjoins + 1;

				return (J_);

			}

		}

		return (maxRecordedjoins + 1);

	}

	void PlaceTile() {

//		Debug.Log("Trying to connect a tile");

		for (int i = 0; i < maxRecordedjoins; i += 1) {


//			Loop through all the the recorded joints

			if (recordedJoin[i] != null) {

				joinerComp = recordedJoin[i].GetComponent<joiner>();

//				Get the joiner script component from the found joint

				if (joinerComp.done == false && joinerComp.fails <= maxJoinFails) {

					if (joinerComp.deadEnd == false) {

						joinType = joinerComp.joinType;

						placed = false;

						loops = 0;

						while (placed == false && loops < loopLimit) {

							tryJoin = levelTile[Random.Range(0, levelTiles)];

//							Select a random tile to try and place
//							May take multiple tries to find a tile with valid joiner pieces with more laggy with more possible pieces.
//							Maybe have lists with tiles with the connector pieces to save time

							for (int n = 0; n < tryJoin.transform.childCount; n += 1) {


//								loop through all children of tile to find the joiner piece

								tryChild = tryJoin.transform.GetChild(n).gameObject;
								if (tryChild.tag == "Joiner") {


//									One potential issue with this is it will always choose the first valid joiner so it may not be that random. Solve this later probably!! :)
//									test child piece for joinType and orientation
//									&& tryChild.GetComponent<joiner>().orientation + joinerComp.orientation == Vector2.zero

									if (tryChild.GetComponent<joiner>().joinType == joinType) {

										offsetAng = -Vector2.SignedAngle(tryChild.GetComponent<joiner>().orientation, -joinerComp.orientation);

										newTile = tryJoin;
										newJoiner = n;

//										I got no idea if this works or if this is gonna work so im sorry if this is a piece of shit, im tired and cant be fucking bothered writing decent code okay???????


										if (useTest == true) {

											
											TestFit(newTile, newJoiner, i, offsetAng);

										}
										else {

											ConnectTile(newTile, newJoiner, i, offsetAng);
//											joinerComp.done = true;

										}






										i = maxRecordedjoins + 1;

										n = tryJoin.transform.childCount + 1;

										placed = true;

									}

								}

							}

							loops += 1;


						}
						if (loops >= loopLimit) {

//							Debug.Log("Reached loop limit");
							joinerComp.deadEnd = true;
							joinerComp.done = true;

							Debug.Log("Could not find joiner piece");

						}

					}

					if (joinerComp.deadEnd == true) {

						joinerComp.done = true;
//						Add some code spawning in a dead end thingo

					}

				}
			}

		}

	}

	Vector3 GetRotatedPos(Vector3 originalPos, Vector3 originPos, float rotationAngle) {

		float originalAngle = Vector2.SignedAngle(new Vector2(1f, 0f), new Vector2(originalPos.x - originPos.x, originalPos.z - originPos.z));
		float originalDistance = Vector2.Distance(new Vector2(0f, 0f), new Vector2(originalPos.x - originPos.x, originalPos.z - originPos.z));

		return (new Vector3(Mathf.Cos(Mathf.Deg2Rad * (originalAngle - rotationAngle)) * originalDistance + originPos.x, originalPos.y, Mathf.Sin(Mathf.Deg2Rad * (originalAngle - rotationAngle)) * originalDistance + originPos.z));

		

	}

	Transform GetChildWithTag(Transform transform, string tag) {

		for (var i__ = 0; i__ < transform.childCount; i__ += 1) {

			if (transform.GetChild(i__).tag == tag) {

				return (transform.GetChild(i__));

			}

		}

		return (transform);

	}

	void TestFit(GameObject obj, int childNum, int recordedJoint, float offsetAngle) {

//		obj is the tile prefab to place
//		childNum is child-index of the joiner peice to use within the tile prefab
//		recordedJoint is pre-existing joint peice in the main map to try to connect to
//		offsetAngle is the rotation of the new prefab

		GameObject newTile;
		TileInside boundingComp;

		newTile = Instantiate(GetChildWithTag(obj.transform, "Bounding").gameObject, GetChildWithTag(obj.transform, "Bounding").position, GetChildWithTag(obj.transform, "Bounding").rotation);
		GameObject newTest;
		newTest = new GameObject();
		newTest.transform.position = obj.transform.GetChild(childNum).position;
//		newTest = Instantiate(tester, obj.transform.GetChild(childNum).position, Quaternion.identity);
		newTest.transform.parent = newTile.transform;
		newTile.transform.rotation = Quaternion.Euler(newTile.transform.rotation.eulerAngles + new Vector3(0f, offsetAngle, 0f));
		newTile.transform.position += (recordedJoin[recordedJoint].transform.position - newTest.transform.position);

//		Hell yeah this work, okay but it can be sped up to maybe same frame idk. also replace the tester prefab for an empty transform could be cool. okay love you bye <3 <3 <3

		boundingComp = newTile.GetComponent<TileInside>();
		boundingComp.compute = true;
		boundingComp.levelTile = obj;
		boundingComp.childNum = childNum;
		boundingComp.recordedJoint = recordedJoint;
		boundingComp.offsetAngle = offsetAngle;
		boundingComp.genComp = this;


	}


	public void ConnectTile(GameObject obj, int childNum, int recordedJoint, float offsetAngle) {

//		obj is the tile prefab to place
//		childNum is child-index of the joiner peice to use within the tile prefab
//		recordedJoint is pre-existing joint peice in the main map to try to connect to
//		offsetAngle is the rotation of the new prefab

		GameObject newTile;

//		Debug.Log(offsetAngle);

		newTile = Instantiate(obj, Vector3.zero, Quaternion.Euler(obj.transform.rotation.eulerAngles + new Vector3(0f, offsetAngle, 0f)));
		newTile.transform.position -= newTile.transform.GetChild(childNum).position - recordedJoin[recordedJoint].transform.position;
		newTile.transform.GetChild(childNum).GetComponent<joiner>().done = true;

		recordedJoin[recordedJoint].GetComponent<joiner>().done = true;
		tiles += 1;

		AddToTileList(newTile);

		

		
//		this block of code is to change the orientation variable of the joiner peices on the new tile depending on the offset angle

		for (int j_ = 0; j_ < newTile.transform.childCount; j_ += 1) {

			Transform childNum_ = newTile.transform.GetChild(j_);

			if (childNum_.tag == "Joiner") {


				Vector2 inOrientation_ = childNum_.GetComponent<joiner>().orientation;
				Vector3 outOrientation = GetRotatedPos(new Vector3(inOrientation_.x, 0f, inOrientation_.y), Vector3.zero, offsetAngle);


				childNum_.GetComponent<joiner>().orientation = new Vector2(outOrientation.x, outOrientation.z);


			}



		}

		

//		this bit of code is to add the joiner peices of the new tile to the list of joiner peices		

		for (int n_ = 0; n_ < newTile.transform.childCount; n_ += 1) {

			joinCheck = newTile.transform.GetChild(n_).gameObject;

//			Loop over all the children of the place tile and gets its object refrence;

			if (joinCheck.tag == "Joiner") {

//				Check if the child is a joiner piece

				AddToJoinerList(joinCheck);

			}

		}





	}

	void PlaceTileFirst() {

		for (int i_ = 0; i_ < maxRecordedTiles; i_ += 1) {

			if (recordedTile[i_] == null) {

//				Find an empty spot in this array ^

				chosenTile = Random.Range(0, levelTiles);

				recordedTile[i_] = Instantiate(levelTile[chosenTile], Vector3.zero, levelTile[chosenTile].transform.rotation);
				GetChildWithTag(recordedTile[i_].transform, "Bounding").gameObject.GetComponent<TileInside>().compute = false;
				tiles += 1;

//				Add the tile GameObject to the empty spot in the tile array


//				Debug.Log("Placed");

				for (int n_ = 0; n_ < recordedTile[i_].transform.childCount; n_ += 1) {

					joinCheck = recordedTile[i_].transform.GetChild(n_).gameObject;

//					Loop over all the children of the place tile and gets its object refrence;

					if (joinCheck.tag == "Joiner") {

//						Check if the child is a joiner piece

						for (int J_ = 0; J_ < maxRecordedjoins; J_ += 1) {

							if (recordedJoin[J_] == null) {

								recordedJoin[J_] = joinCheck;

//								Adds all the children with the tag to the joiner array

								J_ = maxRecordedjoins + 1;

							}

						}

					}

				}

				i_ = maxRecordedTiles + 1;

			}


		}

		
		

	}

	public void IncrementJoinerFails(int joinerID) {

		recordedJoin[joinerID].GetComponent<joiner>().fails += 1;

	}

}
