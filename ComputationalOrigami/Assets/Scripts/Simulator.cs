using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour {
	float ROTATION_SPEED = 80;

	public int InitUnits;
	public FourSquare square;
	Vector3 targetDir, fromDir, pocketV, rotatePerp, perp;

	int stage;

	Transform point;


	void Start() {
		Transform zone = GameObject.Find ("Zone").transform;
		InitUnits = 5;
		FourSquare squareCopy;
		for (int i = 0; i < InitUnits; i++) {
			squareCopy = Instantiate (square, transform.position, transform.rotation) as FourSquare;
			OrigamiFolder.RandomlyFold (squareCopy);
			UnityHelper.RandomlyRotate (squareCopy.transform);
			UnityHelper.RandomlyPosition (squareCopy.transform, zone);
		}


	}
	// Use this for initialization
	void tart () {
		square = GameObject.Find ("4Square").GetComponent<FourSquare> ();
		FourSquare squareCopy;
		squareCopy = Instantiate (square, transform.position, transform.rotation) as FourSquare;
//		OrigamiFolder.RotateHorz (squareCopy);
//		OrigamiFolder.RotateVert (squareCopy);
//		squareCopy.transform.Rotate (Vector3.up,180.0f);

		print (square.center.name);
//		OrigamiFolder.RotateHorz (square);
//		OrigamiFolder.RotateVert (square);
		FourSquare[] units = GameObject.FindObjectsOfType<FourSquare> ();
		foreach (FourSquare unit in units) {
			if (!unit.Equals (square)) {
				square.target = unit;
				break;
			}
		}


		UnityHelper.GetNewRotation(squareCopy.transform, "x", -3);
		UnityHelper.GetNewRotation(squareCopy.transform, "y", 20);
		UnityHelper.GetNewRotation(squareCopy.transform, "z", -14);

//		UnityHelper.setV3Value (squareCopy.transform, "y", 0.5f);
//		UnityHelper.getNewRotation(square.transform, "x", -10);


		point = square.chooseInsertionVertice();


		targetDir = square.target.center.transform.position - square.transform.position;
										// modify this to use a random vertice
		fromDir = point.position - square.transform.position;
		square.transform.rotation = Quaternion.FromToRotation (fromDir, targetDir);
		point.LookAt (square.target.center.transform);


		// perp - vector to rotate around
		perp = Vector3.Cross (square.transform.position - square.target.center.transform.position, 
			square.target.pockets [0].GetVectorIn () - square.target.center.transform.position);


		Vector3 pToT = square.target.pockets [0].GetPocketCenter () - point.position;
		Vector3 pToC = square.IVneighbor1.position - square.IVneighbor2.position;
		rotatePerp = Vector3.Cross (pToT, pToC);
		print ("rotate perp " + rotatePerp);
		Vector3 v1 = square.target.pockets [0].GetVectorIn ();
		Vector3 v2 = square.target.center.transform.position - point.position;
		print ("vector in " + v1);
		print ("p to c " + v2);
		stage = 0;
	}

	void pdate() {
		Debug.DrawLine (square.target.pockets [0].GetPocketCenter(),
			square.target.pockets [0].GetPocketCenter() + perp, 
			Color.green);
		Debug.DrawLine (point.position, square.target.center.position, Color.cyan);
		Debug.DrawLine (square.transform.position, square.transform.position + fromDir, Color.magenta);
		Debug.DrawLine (square.transform.position, square.transform.position + targetDir, Color.magenta);
	}

	// WORKING!!
	void Udate() {
		Debug.DrawLine (point.position, point.position + rotatePerp, Color.cyan);
		Debug.DrawLine (point.position, point.position + square.getVectorIn(), Color.magenta);
		if (stage == 0) {
			Vector3 v1 = square.getVectorIn ();
			v1.y = 0;
			v1.Normalize ();
			Vector3 v2 = square.target.center.position - point.position;
			v2.y = 0;
			v2.Normalize ();
			if (!UnityHelper.V3Equal (v1, v2)) {

				print ("alignv3 " + v1 + "\nvectIn " + v2);
				square.transform.RotateAround (point.position, rotatePerp, 40 * Time.deltaTime);
			} else {
				stage = 1;
			}
		} else if (stage == 1) {
			Vector3 normRotating = (point.position - square.target.center.transform.position);
			normRotating.Normalize ();
			Vector3 normStill = (square.target.pockets [0].GetVectorIn ());
			normStill.Normalize ();

			if (!UnityHelper.V3Equal(normRotating,normStill)) {
				print (normRotating + "\n" + normStill);
				square.transform.RotateAround (square.target.center.position, perp, ROTATION_SPEED * Time.deltaTime);
			} else {
				stage = 2;
			}
	
		} else if (stage == 2) {
			Vector3 acute1 = UnityHelper.acuteAngle (square.transform.eulerAngles);
			Vector3 acute2 = UnityHelper.acuteAngle (square.target.center.eulerAngles);
			if (square.alignedWithTarget()) {//(Math.Abs(acute1.x - acute2.x) < 0.3f && Math.Abs(acute1.z - acute2.z) < 1.5f) {
				stage = 3;
			} else {
				print (acute1 + "\n" + acute2);
				square.transform.RotateAround(point.position, square.target.pockets [0].GetVectorIn (), ROTATION_SPEED * Time.deltaTime);
			} 
		} else if (stage == 3) {
			if ((point.position - square.target.center.position).magnitude > 0.25f) {
				Vector3 pointToCenter = square.transform.position - point.position;
//				square.transform.Translate (square.target.center.position - point.position * Time.deltaTime, Space.Self);
				square.transform.position = Vector3.MoveTowards(square.transform.position, square.target.center.position + pointToCenter, 1.0f * Time.deltaTime);
			} else {
				stage = 4;
			}
			
		}


		point.LookAt (square.target.center.transform);

//		square.transform.RotateAround (square.target.center.position, square.target.pockets [0].GetVectorPerp (), 20);
		Debug.DrawLine (square.target.pockets [0].GetPocketCenter(),
			square.target.pockets [0].GetPocketCenter() - perp, 
			Color.green);
		Debug.DrawLine (point.position, square.target.center.position, Color.cyan);
		
	}



}
