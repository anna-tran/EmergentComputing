using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FourSquare : MonoBehaviour {
	public static float PAPER_THICKNESS = 0.01f;

    // edges are at local positions, relative to the center
	public SortedList<EdgeType,List<TransformEdge>> edges { get; private set; }
	public List<Pocket> pockets { get; private set; }
	public Transform center { get; private set; }

	public FourSquare target { get; set; }

	// Use this for initialization
	void Awake () {
		center = transform.Find ("Center");
		edges = new SortedList<EdgeType,List<TransformEdge>>();
		pockets = new List<Pocket> ();
		foreach (var type in Enum.GetValues(typeof(EdgeType))) {
			EdgeType t = (EdgeType) Enum.ToObject(typeof(EdgeType), type);
			edges.Add (t, new List<TransformEdge> ());
		}
		ChangeColor ();

	}

	void ChangeColor() {
		Color rand_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
		foreach (Renderer rend in transform.GetComponentsInChildren<Renderer>()) {
			rend.material.color = rand_color;
		}
	}

	public void GeneratePockets() {
		var edge_enums = Enum.GetValues (typeof(EdgeType));
		for (int i = 0; i < edge_enums.Length; i++) {
			EdgeType t1 = (EdgeType) Enum.ToObject(typeof(EdgeType), edge_enums.GetValue(i));

			// only match with the unseen edge types
			for (int j = i + 1; j < edge_enums.Length; j++) {
				EdgeType t2 = (EdgeType) Enum.ToObject(typeof(EdgeType), edge_enums.GetValue(j));

				foreach (TransformEdge e1 in edges[t1]) {
					foreach (TransformEdge e2 in edges[t2]) {

						// add pocket only if folded edges are not on the same x or z plane
						float angle = Pocket.CalculateAngle (e1,e2);
						if (Mathf.PI - angle > Mathf.Epsilon) {
							pockets.Add (new Pocket (e1, e2, angle));
							print ("pocket \n" + e1.ToString () + "\n" + e2.ToString ());
						}
					}
				}
			}
		}

	}


	
	// Update is called once per frame
	void Update () {
		foreach (var type in Enum.GetValues(typeof(EdgeType))) {
			EdgeType t = (EdgeType) Enum.ToObject(typeof(EdgeType), type);
			foreach (TransformEdge e in edges[t]) {
				Debug.DrawLine (e.start.position, e.end.position, Color.red);
			}
		}

		foreach (var pocket in pockets) {
			Vector3 v1 = pocket.edge1.end.position - pocket.edge1.start.position;
			Vector3 v2 = pocket.edge2.end.position - pocket.edge2.start.position;
			Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.start.position + v1 + v2, Color.blue);
		}

	}
}
