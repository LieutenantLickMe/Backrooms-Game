using UnityEngine;

public class generator_1 : MonoBehaviour {


	[Header("Tile settings")]

	bool instantGen = false;
	bool autoFill = true;
	bool singleFill = false;
	bool randomFill = false;
	bool fillHoles = false;
	bool autoDel = false;

	[Space(10)]
	bool drawSpecific = false;
	Vector2Int drawPos = Vector2Int.zero;
	bool drawSeal = false;
	bool drawConnections = false;
	bool drawOpen = false;
	bool drawManilaCentre = false;
	bool drawID = false;


	[Space(20)]
	bool useStage0 = true;
	bool useStage1 = true;
	bool useStage2 = true;
	bool useStage3 = true;
	bool useStage4 = true;
	bool useStage5 = false;
	int stage;

	[Space(20)]

	public bool useSeed;
	public int seed;

	[Space(20)]

	bool surroundPlayer = true;
	public GameObject player;

	[Space(20)]

	int xTiles = 24;
	int yTiles = 24;
	bool lockXAndYTiles = false;

	[Space(20)]
	[Range(0f, 1f)]
	float manilaChance = 0.001f;

	[Space(20)]
	bool useSamplingMethod = true;
	int openExpand = 4;
	[Range(0f, 1f)]
	float openFrequency = 0.017f;
	[Range(0f, 10f)]
	float openSize = 3.43f;
	[Range(0f, 1f)]
	float openChance = 0.148f;
	[Range(0f, 1f)]
	float elevatorPassChance = 0.03f;
	[Range(0f, 1f)]
	float forcePlaceChance = 0.001f;

	[Space(20)]
	[Range(0f, 1f)]
	float overrideChance = 0.01f;

	int numTiles = 0;

	float distanceLimit = 50;
	public bool generate = true;

	int i;
	int n;

	int xOffset = 0;
	int yOffset = 0;

	int xCheck = 0;
	int yCheck = 0;

	int xDel = 0;
	int yDel = 0;

	int randPersist;
	int coordSeed;


	bool begin = true;
	bool done;
	bool placeClear;
	bool placed;


	int fillCrop = 0;

	bool placeElevator;
	

	GameObject[,] tileObj = new GameObject[48, 48];
	int[,] tile = new int[48, 48];
	int[,] tileR = new int[48, 48];
	int[,] tileU = new int[48, 48];
	int[,] tileL = new int[48, 48];
	int[,] tileD = new int[48, 48];
	bool[,] tileSeal = new bool[48, 48];
	bool[,] tileSealDone = new bool[48, 48];
	bool[,] manilaState = new bool[48, 48];


	int currR = 1;
	int currU = 1;
	int currL = 1;
	int currD = 1;

	int loopLimit = 200;
	int loops;



	GameObject newTile;
	public GameObject level;

	[Header("Prefabs")]

	public GameObject elevatorPref;
	public GameObject ruld1;
	public GameObject r2;
	public GameObject u3;
	public GameObject l4;
	public GameObject d5;
	public GameObject ru6;
	public GameObject rl7;
	public GameObject rd8;
	public GameObject ul9;
	public GameObject ud10;
	public GameObject ld11;
	public GameObject rul12;
	public GameObject rud13;
	public GameObject rld14;
	public GameObject uld15;
	public GameObject manila16;

	
	[Header("Disabled tiles")]
	
	bool ruldEnabled = true;
	bool rEnabled = false;
	bool uEnabled = false;
	bool lEnabled = false;
	bool dEnabled = false;
	bool ruEnabled = true;
	bool rlEnabled = true;
	bool rdEnabled = true;
	bool ulEnabled = true;
	bool udEnabled = true;
	bool ldEnabled = true;
	bool rulEnabled = true;
	bool rudEnabled = true;
	bool rldEnabled = true;
	bool uldEnabled = true;


	bool ruldOver = true;
	bool rOver = false;
	bool uOver = false;
	bool lOver = false;
	bool dOver = false;
	bool ruOver = true;
	bool rlOver = true;
	bool rdOver = true;
	bool ulOver = true;
	bool udOver = true;
	bool ldOver = true;
	bool rulOver = true;
	bool rudOver = true;
	bool rldOver = true;
	bool uldOver = true;

	

	bool clampEdges = false;

	[Space(20)]
	public int updateRate;
	int updateCount = 0;
	public int quadrant;
	float maxDist;


	float getRandXY(int xpos, int ypos) {

		randPersist = Random.Range(0, 2147483647);

		Random.InitState(coordSeed);

		Random.InitState(Random.Range((int)((float)2147483647 * 0.35), (int)((float)2147483647 * 0.65)) + xpos);
		Random.InitState(Random.Range((int)((float)2147483647 * 0.35), (int)((float)2147483647 * 0.65)) + ypos);

		float randomOut;
		randomOut = Random.Range(0f, 1f);

		Random.InitState(randPersist);

		return (randomOut);

	}

	bool placeEmpty(int xpos, int ypos, int xoff, int yoff) {

		int j;
		int k;

		float expandcoefficient;

		if (useSamplingMethod == false) {
			if (getRandXY((Mathf.FloorToInt((float)(xpos + yoff) / openSize) * (int)openSize), (Mathf.FloorToInt((float)(ypos + xoff) / openSize) * (int)openSize)) < openFrequency) {
				return (true);
			}
		}
		else {

			expandcoefficient = getRandXY((Mathf.FloorToInt((float)(xpos + yoff) / openSize) * (int)openSize), (Mathf.FloorToInt((float)(ypos + xoff) / openSize) * (int)openSize));

			for (j = 0; j < (int)(1f * (float)openExpand); j += 1) {
				for (k = 0; k < (int)(1f * (float)openExpand); k += 1) {
					if (getRandXY(xpos + yoff + j + 999999, ypos + xoff + k + 999999) < openFrequency) {
						return (true);
					}
				}
			}
		}
		
		return (false);

		
	}

	bool ManilaCentre(int xpos, int ypos, int xoff, int yoff) {

		if (getRandXY(xpos + yoff + 999999, ypos + xoff + 999999) < manilaChance) {
			return (true);
		}

		return (false);

	}

	void Start() {

		numTiles = 0;



		if (useSeed == false) {
			seed = Random.Range(0, 2147483647);
		}
		Random.InitState(seed);
		coordSeed = Random.Range(0, 2147483647);
		

		if (lockXAndYTiles == true) {
			yTiles = xTiles;
		}

		
		for (i = 0; i < xTiles; i++) {
			for (n = 0; n < yTiles; n ++) {

				tile[i, n] = 0;
				tileR[i, n] = 1;
				tileU[i, n] = 1;
				tileL[i, n] = 1;
				tileD[i, n] = 1;
				tileSeal[i, n] = false;
				tileSealDone[i, n] = false;
				manilaState[i, n] = false;

			}
		}


		



	}

	void RemoveTile(int i, int n) {

		Destroy(tileObj[i, n]);

		tile[i, n] = 0;
		tileR[i, n] = 1;
		tileU[i, n] = 1;
		tileL[i, n] = 1;
		tileD[i, n] = 1;
		tileSeal[i, n] = false;
		tileSealDone[i, n] = false;

		numTiles -= 1;

	}

	void OnDrawGizmos() {
		/*
		if (drawSpecific == true) {
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(new Vector3((float)(drawPos.x + yOffset - xTiles / 2) * 8f, 0, (float)(drawPos.y + xOffset - yTiles / 2) * 8f), 2f);
			Gizmos.color = Color.blue;
			if (tileObj[drawPos.x, drawPos.y] != null) {
				Gizmos.DrawSphere(tileObj[drawPos.x, drawPos.y].transform.position + new Vector3(0f, -2f, 0f), 2f);
			}

		}

		for (i = 0; i < xTiles; i++) {
			for (n = 0; n < yTiles; n++) {

				if (drawID == true) {
					Gizmos.color = Color.HSVToRGB((float)tile[i, n] / 17f, 1f, 1f);
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f, 0, (float)(n + xOffset - yTiles / 2) * 8f), 2f);
				}

				if (drawManilaCentre == true) {
					Gizmos.color = Color.red;
					if (ManilaCentre(i,n, xOffset, yOffset) == true) {
						Gizmos.color = Color.green;
					}
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f, 0, (float)(n + xOffset - yTiles / 2) * 8f), 2f);
				}
				
				if (drawSeal == true) {
					Gizmos.color = Color.red;
					if (tileSealDone[i, n] == true) {
						Gizmos.color = Color.green;
					}

					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f, 0, (float)(n + xOffset - yTiles / 2) * 8f), 2f);
				}
				if (drawConnections == true) {

					
					if (tileR[i, n] == 0) {
						Gizmos.color = Color.red;
					}
					if (tileR[i, n] == 1) {
						Gizmos.color = Color.yellow;
					}
					if (tileR[i, n] == 2) {
						Gizmos.color = Color.green;
					}
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f + 3f, 0, (float)(n + xOffset - yTiles / 2) * 8f), 1f);




					if (tileU[i, n] == 0) {
						Gizmos.color = Color.red;
					}
					if (tileU[i, n] == 1) {
						Gizmos.color = Color.yellow;
					}
					if (tileU[i, n] == 2) {
						Gizmos.color = Color.green;
					}
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f, 0, (float)(n + xOffset - yTiles / 2) * 8f + 3f), 1f);


					if (tileL[i, n] == 0) {
						Gizmos.color = Color.red;
					}
					if (tileL[i, n] == 1) {
						Gizmos.color = Color.yellow;
					}
					if (tileL[i, n] == 2) {
						Gizmos.color = Color.green;
					}
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f - 3f, 0, (float)(n + xOffset - yTiles / 2) * 8f), 1f);



					if (tileD[i, n] == 0) {
						Gizmos.color = Color.red;
					}
					if (tileD[i, n] == 1) {
						Gizmos.color = Color.yellow;
					}
					if (tileD[i, n] == 2) {
						Gizmos.color = Color.green;
					}
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f, 0, (float)(n + xOffset - yTiles / 2) * 8f - 3f), 1f);

				}
				if (drawOpen == true) {
					Gizmos.color = Color.red;
					if (placeEmpty(i, n, xOffset, yOffset) == true) {
						Gizmos.color = Color.green;
					}
					Gizmos.DrawSphere(new Vector3((float)(i + yOffset - xTiles / 2) * 8f, 0, (float)(n + xOffset - yTiles / 2) * 8f - 3f), 1f);
				}
			}
		}
		*/
	}

	void Update() {


		if (player == null) {

			player = GameObject.FindGameObjectWithTag("Player");

		}

		updateCount += 1;

		if (generate == true && updateCount > updateRate) {
			updateCount = 0;

			if (Mathf.Abs((float)xOffset) > distanceLimit || Mathf.Abs((float)yOffset) > distanceLimit) {

				generate = false;

				if (Mathf.Abs((float)xOffset) > Mathf.Abs((float)yOffset)) {

					if ((float)xOffset > 0) {

						quadrant = 0;

					}
					else {

						quadrant = 2;

					}

				}
				else {

					if ((float)yOffset > 0) {

						quadrant = 1;

					}
					else {

						quadrant = 3;

					}

				}

				for (i = 0; i < xTiles; i++) {

					RemoveTile(i, 0);
					RemoveTile(i, yTiles - 1);

				}
				for (i = 0; i < yTiles; i++) {

					RemoveTile(0, i);
					RemoveTile(xTiles - 1, i);

				}


			}

			for (i = 0; i < xTiles; i++) {
				for (n = 0; n < yTiles; n++) {
					if (tileObj[i, n] == null && manilaState[i, n] == false) {
						tile[i, n] = 0;
						tileR[i, n] = 1;
						tileU[i, n] = 1;
						tileL[i, n] = 1;
						tileD[i, n] = 1;
						tileSeal[i, n] = false;
						tileSealDone[i, n] = false;
						manilaState[i, n] = false;

					}
				}
			}
			placed = false;

			#region Array initiation

			if (begin == true) {
				begin = false;

				for (i = 0; i < xTiles; i++) {
					for (n = 0; n < yTiles; n++) {

						tile[i, n] = 0;
						tileR[i, n] = 1;
						tileU[i, n] = 1;
						tileL[i, n] = 1;
						tileD[i, n] = 1;
						tileSeal[i, n] = false;
						tileSealDone[i, n] = false;

					}
				}

			}

			#endregion

			#region Manila room - 0

			if (useStage0 == true) {
				for (i = 1; i < xTiles - 1; i += 1) {
					for (n = 1; n < yTiles - 1; n += 1) {
						if (manilaState[i, n] == false) {
							if (ManilaCentre(i, n, xOffset, yOffset) == true) {
								PlaceManilla(i, n);




							}
						}
					}
				}

			}

			#endregion

			#region large room gen - 1

			if (useStage1 == true) {
				if (autoFill == true || singleFill == true) {


					for (i = 0; i < xTiles; i++) {
						for (n = 0; n < yTiles; n++) {

							if (tile[i, n] == 0 && tileSealDone[i, n] == false) {

								if (placeEmpty(i, n, xOffset, yOffset) == true) {

									//								Debug.Log("Created a room");

									placeRULD(i, n);

									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;


								}
							}



						}
					}


				}



			}



			#endregion

			#region Large room sealing - 2

			if (useStage2 == true) {

				for (i = 0; i < xTiles; i++) {
					for (n = 0; n < yTiles; n++) {

						if (tileSealDone[i, n] == false) {

							if (placeEmpty(i, n, xOffset, yOffset) == true && tile[i, n] == 1) {


								if (placeEmpty(i - 1, n, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeRUD(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;


								}

								if (placeEmpty(i + 1, n, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeULD(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;

								}

								if (placeEmpty(i, n - 1, xOffset, yOffset) == false) {



									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeRUL(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;

								}

								if (placeEmpty(i, n + 1, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeRLD(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;

								}



							}

							if (placeEmpty(i, n, xOffset, yOffset) == true && tile[i, n] == 12) {

								if (placeEmpty(i - 1, n, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeRU(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;

								}

								if (placeEmpty(i + 1, n, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeUL(i, n);

									tileSeal[i, n] = true;

								}



							}

							if (placeEmpty(i, n, xOffset, yOffset) == true && tile[i, n] == 14) {

								if (placeEmpty(i - 1, n, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeRD(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;

								}

								if (placeEmpty(i + 1, n, xOffset, yOffset) == false) {


									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeLD(i, n);

									tileSeal[i, n] = true;
									tileSealDone[i, n] = false;

								}


							}
						}

					}
				}
			}

			#endregion

			#region Large room opening - 3

			if (useStage3 == true) {

				if (autoFill == true || singleFill == true) {
					for (i = 0; i < xTiles; i++) {
						for (n = 0; n < yTiles; n++) {



							if (tileSeal[i, n] == true && tileSealDone[i, n] == false) {

								if (Random.Range(0f, 1f) < openChance) {

									Destroy(tileObj[i, n]);
									tile[i, n] = 0;
									tileR[i, n] = 1;
									tileU[i, n] = 1;
									tileL[i, n] = 1;
									tileD[i, n] = 1;
									tileSeal[i, n] = false;
									tileSealDone[i, n] = false;

									placeRULD(i, n);


								}
								tileSealDone[i, n] = true;
							}
						}
					}
				}
			}

			#endregion

			#region Filler Generation - 4
			if (useStage4 == true) {
				if (autoFill == true || singleFill == true) {

					done = false;

					for (i = 2; i < xTiles - 2; i++) {
						for (n = 2; n < yTiles - 2; n++) {


							if (done == false) {

								if (randomFill == false) {
									xCheck = i;
									yCheck = n;
								}
								else {
									xCheck = Random.Range(0, xTiles);
									yCheck = Random.Range(0, yTiles);
								}



								if (tile[xCheck, yCheck] == 0) {

									loops = 0;
									placeClear = false;

									while (placeClear == false && loops < loopLimit) {

										loops += 1;

										placeClear = true;

										tile[xCheck, yCheck] = Random.Range(1, 16);




										switch (tile[xCheck, yCheck]) {

										#region Current

										case 1:

											ruldOver = ruldEnabled;


											if (ruldEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												ruldOver = true;
											}

											placeClear = (placeClear && ruldOver);

											currR = 2;
											currU = 2;
											currL = 2;
											currD = 2;

											break;


										case 2:

											rOver = rEnabled;


											if (rEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												rOver = true;
											}

											placeClear = (placeClear && rOver);

											placeElevator = true;

											if (Random.Range(0f, 1f) >= elevatorPassChance) {

												placeElevator = false;
												Debug.Log("Failed an elevator");

											}



											currR = 2;
											currU = 0;
											currL = 0;
											currD = 0;

											break;

										case 3:

											uOver = uEnabled;

											if (uEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												uOver = true;
											}

											placeClear = (placeClear && uOver);

											currR = 0;
											currU = 2;
											currL = 0;
											currD = 0;

											break;

										case 4:

											lOver = lEnabled;

											if (lEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												lOver = true;
											}

											placeClear = (placeClear && lOver);

											currR = 0;
											currU = 0;
											currL = 2;
											currD = 0;

											break;

										case 5:

											dOver = dEnabled;

											if (dEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												dOver = true;
											}

											placeClear = (placeClear && dOver);

											currR = 0;
											currU = 0;
											currL = 0;
											currD = 2;

											break;

										case 6:

											ruOver = ruEnabled;

											if (ruEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												ruOver = true;
											}

											placeClear = (placeClear && ruOver);

											currR = 2;
											currU = 2;
											currL = 0;
											currD = 0;

											break;

										case 7:

											rlOver = rlEnabled;

											if (rlEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												rlOver = true;
											}

											placeClear = (placeClear && rlOver);

											currR = 2;
											currU = 0;
											currL = 2;
											currD = 0;

											break;

										case 8:

											rdOver = rdEnabled;

											if (rdEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												rdOver = true;
											}

											placeClear = (placeClear && rdOver);

											currR = 2;
											currU = 0;
											currL = 0;
											currD = 2;

											break;

										case 9:

											ulOver = ulEnabled;

											if (ulEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												ulOver = true;
											}

											placeClear = (placeClear && ulOver);

											currR = 0;
											currU = 2;
											currL = 2;
											currD = 0;

											break;

										case 10:

											udOver = udEnabled;

											if (udEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												udOver = true;
											}

											placeClear = (placeClear && udOver);

											currR = 0;
											currU = 2;
											currL = 0;
											currD = 2;

											break;

										case 11:

											ldOver = ldEnabled;

											if (ldEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												ldOver = true;
											}

											placeClear = (placeClear && ldOver);

											currR = 0;
											currU = 0;
											currL = 2;
											currD = 2;

											break;

										case 12:

											rulOver = rulEnabled;

											if (rulEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												rulOver = true;
											}

											placeClear = (placeClear && rulOver);

											currR = 2;
											currU = 2;
											currL = 2;
											currD = 0;

											break;

										case 13:

											rudOver = rudEnabled;

											if (rudEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												rudOver = true;
											}

											placeClear = (placeClear && rudOver);

											currR = 2;
											currU = 2;
											currL = 0;
											currD = 2;

											break;

										case 14:

											rldOver = rldEnabled;

											if (rldEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												rldOver = true;
											}

											placeClear = (placeClear && rldOver);

											currR = 2;
											currU = 0;
											currL = 2;
											currD = 2;

											break;

										case 15:

											uldOver = uldEnabled;

											if (uldEnabled == false && Random.Range(0f, 1f) <= overrideChance) {
												uldOver = true;
											}

											placeClear = (placeClear && uldOver);

											currR = 0;
											currU = 2;
											currL = 2;
											currD = 2;

											break;

											#endregion

										}

										#region Tile failing
										if (yCheck > 0) {
											if (currD == 0 && tileU[xCheck, yCheck - 1] == 2) {
												placeClear = false;
												//will never close off an upwards path
											}
											if (currD == 2 && tileU[xCheck, yCheck - 1] == 0) {
												placeClear = false;
												//will never open up an upwards path
											}
										}
										if (xCheck > 0) {
											if (currL == 0 && tileR[xCheck - 1, yCheck] == 2) {
												placeClear = false;
												//will never close off a rightwards path
											}
											if (currL == 2 && tileR[xCheck - 1, yCheck] == 0) {
												placeClear = false;
												//will never open up a rightwards path
											}
										}
										if (yCheck < yTiles - 1) {
											if (currU == 0 && tileD[xCheck, yCheck + 1] == 2) {
												placeClear = false;
												//will never close off an downwards path
											}
											if (currU == 2 && tileD[xCheck, yCheck + 1] == 0) {
												placeClear = false;
												//will never open up an downwards path
											}
										}
										if (xCheck < xTiles - 1) {
											if (currR == 0 && tileL[xCheck + 1, yCheck] == 2) {
												placeClear = false;
												//will never close off a leftwards path
											}
											if (currR == 2 && tileL[xCheck + 1, yCheck] == 0) {
												placeClear = false;
												//will never open up a leftwards path
											}
										}

										if (Random.Range(0f, 1f) <= forcePlaceChance) {

											placeClear = true;

										}

										#endregion
									}

									if (loops == loopLimit) {

										//									Debug.Log("Loop limit reached");

										tile[xCheck, yCheck] = 1;


									}

									if (placeClear == true) {

										switch (tile[xCheck, yCheck]) {



										#region Instantiation

										case 1:

											tile[xCheck, yCheck] = 0;

											placeRULD(xCheck, yCheck);


											break;

										case 2:

											tile[xCheck, yCheck] = 0;

											placeR(xCheck, yCheck);

											break;

										case 3:

											tile[xCheck, yCheck] = 0;

											placeU(xCheck, yCheck);

											break;

										case 4:

											tile[xCheck, yCheck] = 0;

											placeL(xCheck, yCheck);

											break;

										case 5:

											tile[xCheck, yCheck] = 0;

											placeD(xCheck, yCheck);

											break;

										case 6:

											tile[xCheck, yCheck] = 0;

											placeRU(xCheck, yCheck);

											break;

										case 7:

											tile[xCheck, yCheck] = 0;

											placeRL(xCheck, yCheck);

											break;

										case 8:

											tile[xCheck, yCheck] = 0;

											placeRD(xCheck, yCheck);

											break;

										case 9:

											tile[xCheck, yCheck] = 0;

											placeUL(xCheck, yCheck);

											break;

										case 10:

											tile[xCheck, yCheck] = 0;

											placeUD(xCheck, yCheck);

											break;

										case 11:

											tile[xCheck, yCheck] = 0;

											placeLD(xCheck, yCheck);

											break;

										case 12:

											tile[xCheck, yCheck] = 0;

											placeRUL(xCheck, yCheck);

											break;

										case 13:

											tile[xCheck, yCheck] = 0;

											placeRUD(xCheck, yCheck);

											break;

										case 14:

											tile[xCheck, yCheck] = 0;

											placeRLD(xCheck, yCheck);

											break;

										case 15:

											tile[xCheck, yCheck] = 0;

											placeULD(xCheck, yCheck);

											break;

											#endregion


										}

									}
									if (instantGen == false) {
										done = true;
									}
								}
							}

						}
					}

				}
			}

			#endregion

			#region Manila planting - 5

			if (useStage5 == true) {

				for (i = 0; i < xTiles; i += 1) {
					for (n = 0; n < yTiles; n += 1) {
						if (Input.GetKeyDown(KeyCode.P) == true) {
							if (manilaState[i, n] == false) {

								if (i != 0) {
									if (manilaState[i - 1, n] == true) {

										RemoveTile(i, n);
										tileSeal[i, n] = false;
										tileSealDone[i, n] = false;
										Debug.Log("Trying to delete");


									}
								}

								if (i != xTiles - 1) {
									if (manilaState[i + 1, n] == true) {

										RemoveTile(i, n);
										tileSeal[i, n] = false;
										tileSealDone[i, n] = false;
										Debug.Log("Trying to delete");

									}
								}

								if (n != 0) {
									if (manilaState[i, n - 1] == true) {

										RemoveTile(i, n);
										tileSeal[i, n] = false;
										tileSealDone[i, n] = false;
										Debug.Log("Trying to delete");

									}
								}

								if (n != yTiles - 1) {
									if (manilaState[i, n + 1] == true) {

										RemoveTile(i, n);
										tileSeal[i, n] = false;
										tileSealDone[i, n] = false;
										Debug.Log("Trying to delete");

									}
								}

							}
						}
					}
				}

			}

			#endregion

			#region hole filling

			if (fillHoles == true && placed == false) {
				for (i = 0; i < xTiles; i++) {
					for (n = 0; n < yTiles; n++) {

						if (tileObj[i, n] == null) {

							tile[i, n] = 0;
							tileR[i, n] = 1;
							tileU[i, n] = 1;
							tileL[i, n] = 1;
							tileD[i, n] = 1;
							tileSeal[i, n] = false;
							tileSealDone[i, n] = false;
						}

					}
				}
			}

			#endregion

			#region Autodeletion

			if (autoDel == true) {

				xDel = Random.Range(0, xTiles);
				yDel = Random.Range(0, yTiles);


				Destroy(tileObj[xDel, yDel]);
				tile[xDel, yDel] = 0;
				tileR[xDel, yDel] = 1;
				tileU[xDel, yDel] = 1;
				tileL[xDel, yDel] = 1;
				tileD[xDel, yDel] = 1;

			}

			#endregion

			#region Moving

			if (xOffset < Mathf.Round(player.transform.position.z / 8f)) {

				xOffset += 1;


				for (i = 0; i < xTiles; i++) {


					Destroy(tileObj[i, 0]);
					tile[i, 0] = 0;
					tileR[i, 0] = 1;
					tileU[i, 0] = 1;
					tileL[i, 0] = 1;
					tileD[i, 0] = 1;
					tileSeal[i, 0] = false;
					tileSealDone[i, 0] = false;
					manilaState[i, 0] = false;

				}



				for (i = 0; i < xTiles; i++) {
					for (n = 0; n < yTiles - 1; n++) {

						tileObj[i, n] = tileObj[i, n + 1];
						tile[i, n] = tile[i, n + 1];
						tileR[i, n] = tileR[i, n + 1];
						tileU[i, n] = tileU[i, n + 1];
						tileL[i, n] = tileL[i, n + 1];
						tileD[i, n] = tileD[i, n + 1];
						tileSeal[i, n] = tileSeal[i, n + 1];
						tileSealDone[i, n] = tileSealDone[i, n + 1];
						manilaState[i, n] = manilaState[i, n + 1];

					}
				}

				for (i = 0; i < xTiles; i++) {

					tileObj[i, yTiles - 1] = null;
					tile[i, yTiles - 1] = 0;
					tileR[i, yTiles - 1] = 1;
					tileU[i, yTiles - 1] = 1;
					tileL[i, yTiles - 1] = 1;
					tileD[i, yTiles - 1] = 1;
					tileSeal[i, yTiles - 1] = false;
					tileSealDone[i, yTiles - 1] = false;
					manilaState[i, yTiles - 1] = false;

				}


			}



			if (yOffset < Mathf.Round(player.transform.position.x / 8f)) {

				yOffset += 1;

				for (i = 0; i < yTiles; i++) {


					Destroy(tileObj[0, i]);
					tile[0, i] = 0;
					tileR[0, i] = 1;
					tileU[0, i] = 1;
					tileL[0, i] = 1;
					tileD[0, i] = 1;
					tileSeal[0, i] = false;
					tileSealDone[0, i] = false;
					manilaState[0, i] = false;

				}



				for (i = 0; i < xTiles - 1; i++) {
					for (n = 0; n < yTiles; n++) {

						tileObj[i, n] = tileObj[i + 1, n];
						tile[i, n] = tile[i + 1, n];
						tileR[i, n] = tileR[i + 1, n];
						tileU[i, n] = tileU[i + 1, n];
						tileL[i, n] = tileL[i + 1, n];
						tileD[i, n] = tileD[i + 1, n];
						tileSeal[i, n] = tileSeal[i + 1, n];
						tileSealDone[i, n] = tileSealDone[i + 1, n];
						manilaState[i, n] = manilaState[i + 1, n];

					}
				}

				for (i = 0; i < yTiles; i++) {

					tileObj[xTiles - 1, i] = null;
					tile[xTiles - 1, i] = 0;
					tileR[xTiles - 1, i] = 1;
					tileU[xTiles - 1, i] = 1;
					tileL[xTiles - 1, i] = 1;
					tileD[xTiles - 1, i] = 1;
					tileSeal[xTiles - 1, i] = false;
					tileSealDone[xTiles - 1, i] = false;
					manilaState[xTiles - 1, i] = false;

				}


			}



			if (xOffset > Mathf.Round(player.transform.position.z / 8f)) {

				xOffset -= 1;


				for (i = 0; i < xTiles; i++) {

					Destroy(tileObj[i, yTiles - 1]);
					tile[i, yTiles - 1] = 0;
					tileR[i, yTiles - 1] = 1;
					tileU[i, yTiles - 1] = 1;
					tileL[i, yTiles - 1] = 1;
					tileD[i, yTiles - 1] = 1;
					tileSeal[i, yTiles - 1] = false;
					tileSealDone[i, yTiles - 1] = false;
					manilaState[i, yTiles - 1] = false;

				}



				for (i = 0; i < xTiles; i++) {
					for (n = yTiles - 1; n > 0; n--) {

						tileObj[i, n] = tileObj[i, n - 1];
						tile[i, n] = tile[i, n - 1];
						tileR[i, n] = tileR[i, n - 1];
						tileU[i, n] = tileU[i, n - 1];
						tileL[i, n] = tileL[i, n - 1];
						tileD[i, n] = tileD[i, n - 1];
						tileSeal[i, n] = tileSeal[i, n - 1];
						tileSealDone[i, n] = tileSealDone[i, n - 1];
						manilaState[i, n] = manilaState[i, n - 1];

					}
				}

				for (i = 0; i < xTiles; i++) {

					tileObj[i, 0] = null;
					tile[i, 0] = 0;
					tileR[i, 0] = 1;
					tileU[i, 0] = 1;
					tileL[i, 0] = 1;
					tileD[i, 0] = 1;
					tileSeal[i, 0] = false;
					tileSealDone[i, 0] = false;
					manilaState[i, 0] = false;

				}


			}



			if (yOffset > Mathf.Round(player.transform.position.x / 8f)) {

				yOffset -= 1;

				for (i = 0; i < yTiles; i++) {

					Destroy(tileObj[xTiles - 1, i]);
					tile[xTiles - 1, i] = 0;
					tileR[xTiles - 1, i] = 1;
					tileU[xTiles - 1, i] = 1;
					tileL[xTiles - 1, i] = 1;
					tileD[xTiles - 1, i] = 1;
					tileSeal[xTiles - 1, i] = false;
					tileSealDone[xTiles - 1, i] = false;
					manilaState[xTiles - 1, i] = false;

				}



				for (i = xTiles - 1; i > 0; i--) {
					for (n = 0; n < yTiles; n++) {

						tileObj[i, n] = tileObj[i - 1, n];
						tile[i, n] = tile[i - 1, n];
						tileR[i, n] = tileR[i - 1, n];
						tileU[i, n] = tileU[i - 1, n];
						tileL[i, n] = tileL[i - 1, n];
						tileD[i, n] = tileD[i - 1, n];
						tileSeal[i, n] = tileSeal[i - 1, n];
						tileSealDone[i, n] = tileSealDone[i - 1, n];
						manilaState[i, n] = manilaState[i - 1, n];

					}
				}

				for (i = 0; i < yTiles; i++) {

					tileObj[0, i] = null;
					tile[0, i] = 0;
					tileR[0, i] = 1;
					tileU[0, i] = 1;
					tileL[0, i] = 1;
					tileD[0, i] = 1;
					tileSeal[0, i] = false;
					tileSealDone[0, i] = false;
					manilaState[0, i] = false;

				}


			}
			#endregion

			singleFill = false;
		}
	}

	#region Instantiation methods
	void placeRULD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 1;
			tileObj[xpos, ypos] = Instantiate(ruld1, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), ruld1.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 2;

			placed = true;

}

	}

	void placeR(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			

			numTiles += 1;

			tile[xpos, ypos] = 2;

			if (placeElevator == true) {

				tileObj[xpos, ypos] = Instantiate(elevatorPref, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), elevatorPref.transform.rotation);
				tileObj[xpos, ypos].transform.parent = level.transform;
			}
			else {

				tileObj[xpos, ypos] = Instantiate(r2, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), r2.transform.rotation);
				tileObj[xpos, ypos].transform.parent = level.transform;

			}

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeU(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 3;
			tileObj[xpos, ypos] = Instantiate(u3, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), u3.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeL(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 4;
			tileObj[xpos, ypos] = Instantiate(l4, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), l4.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 5;
			tileObj[xpos, ypos] = Instantiate(d5, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), d5.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}

	void placeRU(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 6;
			tileObj[xpos, ypos] = Instantiate(ru6, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), ru6.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeRL(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 7;
			tileObj[xpos, ypos] = Instantiate(rl7, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), rl7.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeRD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 8;
			tileObj[xpos, ypos] = Instantiate(rd8, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), rd8.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}

	void placeUL(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 9;
			tileObj[xpos, ypos] = Instantiate(ul9, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), ul9.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeUD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 10;
			tileObj[xpos, ypos] = Instantiate(ud10, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), ud10.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}

	void placeLD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 11;
			tileObj[xpos, ypos] = Instantiate(ld11, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), ld11.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}

	void placeRUL(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 12;
			tileObj[xpos, ypos] = Instantiate(rul12, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), rul12.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 0;

			placed = true;
		}

	}

	void placeRUD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 13;
			tileObj[xpos, ypos] = Instantiate(rud13, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), rud13.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 0;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}

	void placeRLD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 14;
			tileObj[xpos, ypos] = Instantiate(rld14, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), rld14.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 0;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}


	void placeULD(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			tile[xpos, ypos] = 15;
			tileObj[xpos, ypos] = Instantiate(uld15, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), uld15.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 0;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 2;

			placed = true;
		}

	}

	void PlaceManilla(int xpos, int ypos) {

		if (tile[xpos, ypos] == 0) {

			numTiles += 1;

			manilaState[i - 1, n - 1] = true;
			manilaState[i, n - 1] = true;
			manilaState[i + 1, n - 1] = true;
			manilaState[i - 1, n] = true;
			manilaState[i, n] = true;
			manilaState[i + 1, n] = true;
			manilaState[i - 1, n + 1] = true;
			manilaState[i, n + 1] = true;
			manilaState[i + 1, n + 1] = true;
			
			tile[xpos - 1, ypos - 1] = 16;
			tile[xpos, ypos - 1] = 16;
			tile[xpos + 1, ypos - 1] = 16;
			tile[xpos - 1, ypos] = 16;
			tile[xpos, ypos] = 16;
			tile[xpos + 1, ypos] = 16;
			tile[xpos - 1, ypos + 1] = 16;
			tile[xpos, ypos + 1] = 16;
			tile[xpos + 1, ypos + 1] = 16;
			
			tileObj[xpos, ypos] = Instantiate(manila16, new Vector3((float)(xpos + yOffset - xTiles / 2) * 8f, 0, (float)(ypos + xOffset - yTiles / 2) * 8f), manila16.transform.rotation);
			tileObj[xpos, ypos].transform.parent = level.transform;

			tileR[xpos, ypos] = 2;
			tileU[xpos, ypos] = 2;
			tileL[xpos, ypos] = 2;
			tileD[xpos, ypos] = 2;

			tileR[xpos - 1, ypos - 1] = 0;
			tileU[xpos - 1, ypos - 1] = 0;
			tileL[xpos - 1, ypos - 1] = 0;
			tileD[xpos - 1, ypos - 1] = 0;

			tileR[xpos, ypos - 1] = 0;
			tileU[xpos, ypos - 1] = 0;
			tileL[xpos, ypos - 1] = 0;
			tileD[xpos, ypos - 1] = 0;

			tileR[xpos + 1, ypos - 1] = 0;
			tileU[xpos + 1, ypos - 1] = 0;
			tileL[xpos + 1, ypos - 1] = 0;
			tileD[xpos + 1, ypos - 1] = 0;

			tileR[xpos - 1, ypos] = 0;
			tileU[xpos - 1, ypos] = 0;
			tileL[xpos - 1, ypos] = 0;
			tileD[xpos - 1, ypos] = 0;

			tileR[xpos + 1, ypos] = 0;
			tileU[xpos + 1, ypos] = 0;
			tileL[xpos + 1, ypos] = 0;
			tileD[xpos + 1, ypos] = 0;

			tileR[xpos - 1, ypos + 1] = 0;
			tileU[xpos - 1, ypos + 1] = 0;
			tileL[xpos - 1, ypos + 1] = 0;
			tileD[xpos - 1, ypos + 1] = 0;

			tileR[xpos, ypos + 1] = 0;
			tileU[xpos, ypos + 1] = 2;
			tileL[xpos, ypos + 1] = 0;
			tileD[xpos, ypos + 1] = 0;

			tileR[xpos + 1, ypos + 1] = 0;
			tileU[xpos + 1, ypos + 1] = 0;
			tileL[xpos + 1, ypos + 1] = 0;
			tileD[xpos + 1, ypos + 1] = 0;


			placed = true;
		}

	}

	#endregion
}