using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour {
	public FourSquare square;
	Vector3 targetDir, fromDir;

	// Use this for initialization
	void Start () {
		square = GameObject.Find ("4Square").GetComponent<FourSquare> ();
		FourSquare squareCopy;
		squareCopy = Instantiate (square, transform.position, transform.rotation) as FourSquare;
		OrigamiFolder.RotateHorz (squareCopy);
		OrigamiFolder.RotateVert (squareCopy);
		squareCopy.GeneratePockets ();
		squareCopy.transform.Rotate (Vector3.up,180.0f);

		print (square.center.name);
		OrigamiFolder.RotateHorz (square);
		square.GeneratePockets ();
		FourSquare[] units = GameObject.FindObjectsOfType<FourSquare> ();
		foreach (FourSquare unit in units) {
			if (!unit.Equals (square)) {
				square.target = unit;
				break;
			}
		}
		Debug.Log (square.target.name);


//		targetDir = square.target.center.transform.position - square.transform.position;
		Pocket pocket = squareCopy.pockets[0];
		Vector3 v1 = pocket.edge1.end.position - pocket.edge1.start.position;
		Vector3 v2 = pocket.edge2.end.position - pocket.edge2.start.position;
		targetDir = pocket.edge2.start.position + v1 + v2;
										// modify this to use a random vertice
		fromDir = square.transform.Find ("-X-Z").position - square.transform.position;
		square.transform.rotation = Quaternion.FromToRotation (fromDir, targetDir);
		square.transform.Find ("-X-Z").LookAt (square.target.center.transform);

	}



	void Update() {
		
		Debug.DrawRay (square.transform.position, targetDir, Color.cyan);
//		square.transform.Translate (targetDir * Time.deltaTime);

//		Vector3 targetDir = square.target.transform.position - square.transform.position;
//		float step = 1.0f * Time.deltaTime;
//		Vector3 newDir = Vector3.RotateTowards (square.transform.forward, targetDir, step, 0.0f);
//		Debug.DrawRay (square.transform.position, newDir, Color.cyan);
//		square.transform.rotation = Quaternion.LookRotation (newDir);
	}
//		square.transform.Find("-XZ").LookAt (square.target.center.transform);
//
//		Vector3 dir = square.target.transform.position - square.transform.position;
//		var rot = Quaternion.FromToRotation (dir, -square.transform.position);
//
//		square.transform.position += square.transform.forward * Time.deltaTime;
//	}



}
