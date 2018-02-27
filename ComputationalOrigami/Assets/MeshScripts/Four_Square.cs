using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Four_Square : MonoBehaviour {
	List<FoldedEdge> edges;

	// Use this for initialization
	void Start () {
		edges = new List<FoldedEdge> ();
		foreach (Transform child in transform) {
			print (child.name + "\n" + child.localPosition);
		}
		print (transform.position);
		RotateVert ();
		RotateHorz ();
	}

	void InitializeChildrenPosition() {
		float sqrt2 = Mathf.Sqrt (2) / 2;
		float neg_sqrt2 = -sqrt2;

		transform.GetChild (0).position = transform.position + new Vector3 (neg_sqrt2, 0, neg_sqrt2);
		transform.GetChild (1).position = transform.position + new Vector3 (sqrt2, 0, neg_sqrt2);
		transform.GetChild (2).position = transform.position + new Vector3 (neg_sqrt2, 0, sqrt2);
		transform.GetChild (3).position = transform.position + new Vector3 (sqrt2, 0, sqrt2);
	}

	void RotateHorz() {
		Vector3 right_translate = new Vector3 (-Mathf.Sqrt (2), -0.01f, 0);
																// change this to height of object I'm folding towards
		foreach (Transform child in transform) {
			if (child.position.x < transform.position.x) {
				child.Rotate (Vector3.forward,180);
				child.Translate (right_translate);
				print (child.name + "\n" + child.GetComponent<MeshFilter>().mesh.bounds);
			}
		}
	}

	void RotateVert() {
		Vector3 forwards_translate = new Vector3 (0, -0.01f, -Mathf.Sqrt (2));
		foreach (Transform child in transform) {
			if (child.position.z < transform.position.z) {
				child.Rotate (Vector3.right,180);
				child.Translate (forwards_translate);
				print (child.name + "\n" + child.GetComponent<MeshFilter>().mesh.bounds);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
