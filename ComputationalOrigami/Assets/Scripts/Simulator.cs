using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour {
	public FourSquare square;
	public FourSquare spawner;

	// Use this for initialization
	void Start () {
		FourSquare squareCopy;
		squareCopy = Instantiate (square, transform.position, transform.rotation) as FourSquare;
		OrigamiFolder.RotateHorz (squareCopy);
		OrigamiFolder.RotateVert (squareCopy);
		squareCopy.GeneratePockets ();

		FourSquare[] units = GameObject.FindObjectsOfType<FourSquare> ();
		foreach (FourSquare unit in units) {
			if (!unit.Equals (square)) {
				square.target = unit;
				break;
			}
		}
		Debug.Log (square.name);
		square.transform.Find("-X").LookAt (square.target.transform.Find ("Center"));
//		square.transform.Translate (Vector3.right);

	}


}
