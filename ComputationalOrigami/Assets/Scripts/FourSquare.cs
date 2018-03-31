using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FourSquare : MonoBehaviour {
	public static float PAPER_THICKNESS = 0.01f;
	private static int NUM_VERTICES = 8;

    // edges are at local positions, relative to the center
	public SortedList<EdgeType,List<TransformEdge>> edges { get; private set; }
	public List<Pocket> pockets { get; private set; }
	public Transform center { get; private set; }

	public FourSquare target { get; set; }
	public Transform insertionVertice { get; private set; }
	public Transform IVneighbor1 { get; private set;}
	public Transform IVneighbor2 { get; private set; }
	public bool rendMesh;
	int numFolds;

	// Use this for initialization
	void Awake () {
		center = transform.Find ("Center");
		edges = new SortedList<EdgeType,List<TransformEdge>>();
		pockets = new List<Pocket> ();
//		foreach (var type in Enum.GetValues(typeof(EdgeType))) {
//			EdgeType t = (EdgeType) Enum.ToObject(typeof(EdgeType), type);
//			edges.Add (t, new List<TransformEdge> ());
//		}
		
		numFolds = 0;
		insertionVertice = null;
		rendMesh = true;
		ChangeColor ();

	}

	void ChangeColor() {
		Color rand_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
		foreach (Renderer rend in transform.GetComponentsInChildren<Renderer>()) {
			rend.material.color = rand_color;
		}
	}

	public void insertEdge(TransformEdge e) {
		EdgeType et = e.edge_type;

		// if there is at least one edge, then we can create pockets
		if (pockets.Count > 0) {
			List<Pocket> toDiscard = new List<Pocket> ();
			List<Pocket> toAdd = new List<Pocket> ();
			foreach (Pocket p in pockets) {
				// if edge crosses with pocket in the x,z
				if (IntersectsPocket (e, p, et)) {
					// save the two edges of that pocket e1,e2
					TransformEdge e1 = p.edge1;
					TransformEdge e2 = p.edge2;
					// erase the pocket
					toDiscard.Add (p);
//					print ("Discarding pocket " + p.ToString());
					// create 2 new pockets e-e1,e-e2

					toAdd.Add (new Pocket (e, e1)); 
					toAdd.Add (new Pocket (e, e2));
//					print ("Adding 2 new pockets");
				}
			}
			pockets.RemoveAll (t => toDiscard.Contains(t));
			pockets.AddRange(toAdd);
//			print ("pocket count: " + pockets.Count);

		} else  {
			// create a pocket with edges not of this type
			foreach (EdgeType et1 in edges.Keys) {
				if (et1 != et) {
					foreach (TransformEdge e1 in edges[et1]) {
						pockets.Add (new Pocket (e, e1));
//						print ("Adding new pocket");
					}
				}
			}
//			print ("init pocket count: " + pockets.Count);


		}
		if (!edges.ContainsKey (et)) {
			edges.Add (et, new List<TransformEdge> ());
		}
		edges [et].Add (e);

	}


	private bool IntersectsPocket(TransformEdge e, Pocket p, EdgeType et) {
		Vector3 s1 = e.start.position;
		Vector3 e1 = e.end.position;
		Vector3 s2 = p.edge1.end.position;
		Vector3 e2 = p.edge2.end.position;
		float m1 = UnityHelper.getSlope (s1.x, s1.z, e1.x, e1.z);
		float m2 = UnityHelper.getSlope (s2.x, s2.z, e2.x, e2.z);

		if (UnityHelper.ApproxSameFloat (m1, m2)) {
//			print ("m1 " + m1 + "  m2 " + m2);
			return false;
		}
		float intx, intz;
		if (Double.IsInfinity (m1)) {
			float b2 = UnityHelper.getB (e2.x, e2.z, m2);
			intx = e1.x;
			intz = (m2 * intx) + b2;
		} else if (Double.IsInfinity (m2)) {
			float b1 = UnityHelper.getB (e1.x, e1.z, m1);
			intx = e2.x;
			intz = (m1 * intx) + b1;
		} else {
			float b1 = UnityHelper.getB (e1.x, e1.z, m1);
			float b2 = UnityHelper.getB (e2.x, e2.z, m2);
			intx = (b2 - b1) / (m1 - m2);
			intz = (m1 * intx) + b1;
		}
		// if not the same as the ends of the pocket
		if (!UnityHelper.ApproxSameFloat (intx, s2.x)
			&& !UnityHelper.ApproxSameFloat (intx, e2.x)
			&& UnityHelper.InBetweenExcl (s2.x, e2.x, intx)) {
			return true;
		} else {
			return false;
		}

	}


	void GeneratePockets() {
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
							pockets.Add (new Pocket (e1, e2));
//							print ("pocket \n" + e1.ToString () + "\n" + e2.ToString ());
						}
					}
				}
			}
		}

	}

	// vertices are labelled 'Vx' where x in range 1-8
	public Transform chooseInsertionVertice() {
		if (insertionVertice == null) {
			int vNum = (int) (UnityEngine.Random.Range(0,NUM_VERTICES-1));
//			insertionVertice = transform.Find("V" + vNum);
			insertionVertice = transform.Find("V7");
			getIVNeighbors ();
		}
//		print ("insertion vertice" + insertionVertice.name);
		return insertionVertice;
	}

	// get the connected vertices to find the fromDir vector
	private void getIVNeighbors() {
		int vNum = Int32.Parse(insertionVertice.name.Substring(1));
		print (vNum + "\n" + (((vNum - 1 + NUM_VERTICES) % NUM_VERTICES)) + "\n" + ((vNum + 1) % NUM_VERTICES));
	
		Transform vBefore = transform.Find("V" + ((vNum - 1 + NUM_VERTICES) % NUM_VERTICES));
		Transform vAfter = transform.Find("V" + ((vNum + 1) % NUM_VERTICES));

		List<Transform> diffVects = new List<Transform> ();
		diffVects.Add (vBefore);
		diffVects.Add (vAfter);
		diffVects.Add (center);


		float maxDistance = 0;
		Vector3 temp1, temp2;
		for (int i = 0; i < diffVects.Count; i++) {
			temp1 = diffVects[i].position;
			for (int j = i; j < diffVects.Count; j++) {
				temp2 = diffVects [j].position;
				if (Vector3.Distance (temp1, temp2) > maxDistance) {
					maxDistance = Vector3.Distance (temp1, temp2);
					IVneighbor1 = diffVects [i];
					IVneighbor2 = diffVects [j];
				}
			}
		}
		print (IVneighbor1 + "\n" + IVneighbor2);
	}

	public Vector3 getVectorIn() {
		Vector3 v1 = IVneighbor1.position - insertionVertice.position;
		Vector3 v2 = IVneighbor2.position - insertionVertice.position;
		print("IVs\n" + (v1+v2));
		Vector3 sum = v1 + v2;
		sum.x = -sum.x;
		sum.z = -sum.z;
		print ("fromDir " + sum);
		return sum;
	}

	public bool alignedWithTarget() {
		Vector3 v1 = IVneighbor1.position - target.pockets[0].edge1.end.position;
		Vector3 v2 = IVneighbor2.position - target.pockets[0].edge2.end.position;
		print ("n1-e1 " + v1 + "\nn2-e2 " + v2);

		return UnityHelper.V3ApproxEqual (v1, v2);
	}

	public void incNumFolds() {
		numFolds += 1;
	}

	
	// Update is called once per frame
	void Update () {
		if (rendMesh) {
			foreach (MeshRenderer mr in transform.GetComponentsInChildren<MeshRenderer>()) {
				mr.enabled = true;
			}
		} else {
			foreach (MeshRenderer mr in transform.GetComponentsInChildren<MeshRenderer>()) {
				mr.enabled = false;
			}
		}
//		foreach (List<TransformEdge> l in edges.Values) {
//			foreach (TransformEdge e in l) {
//				Debug.DrawLine (e.start.position, e.end.position, Color.black);
//			}
//		}

		foreach (var pocket in pockets) {
			Debug.DrawLine (pocket.edge1.start.position, pocket.edge1.end.position, pocket.color);
			Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.end.position, pocket.color);
//			Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.start.position + pocket.GetVectorIn(), Color.blue);
		}

	}

}
