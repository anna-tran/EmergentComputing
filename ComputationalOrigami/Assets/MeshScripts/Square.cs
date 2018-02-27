using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {
	Bounds sq_bounds;
	// Use this for initialization
	void Start () {
		sq_bounds = new Bounds ();
		foreach (Transform child in transform) {
			sq_bounds.Encapsulate (child.GetComponent<MeshRenderer> ().bounds);
			print (child.name + "\n" + child.GetComponent<MeshRenderer> ().bounds);
		}
		gameObject.AddComponent<MeshFilter> ();
		GetComponent<MeshFilter> ().mesh.bounds = sq_bounds;
	}
	

}
