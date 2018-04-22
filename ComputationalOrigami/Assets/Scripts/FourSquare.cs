using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FourSquare : MonoBehaviour {
    public static float PAPER_THICKNESS = 0.03f;
    private static int NUM_VERTICES = 8;

    // edges are at local positions, relative to the center
    public SortedList<EdgeType, List<TransformEdge>> edges { get; private set; }
    public List<Pocket> pockets { get; private set; }
    public Transform center { get; private set; }

    public Pocket targetP { get; set; }
    // iv   insertion vertex
	private Transform iv;
	private Transform ivNeighbor1;
	private Transform ivNeighbor2;
    public bool rendMesh;
    public int stage { get; private set; }
    public Vector3 targetRotationV3 { get; private set; }
    public Vector3 selfRotationV3 { get; private set; }
	public int numFolds { get; private set; }

	// Use this for initialization
	void Awake () {
		center = transform.Find ("Center");
		edges = new SortedList<EdgeType,List<TransformEdge>>();
		pockets = new List<Pocket> ();
		
		iv = null;
		ivNeighbor1 = null;
		ivNeighbor2 = null;
		rendMesh = true;
        selfRotationV3 = Vector3.zero;
        targetRotationV3 = Vector3.zero;
        stage = 0;
		numFolds = 0;
		targetP = null;

//		ChangeColor ();
		CollectChildBounds ();

		// only uncomment for sandbox
//		OrigamiFolder.FoldSquare (EdgeType.HORZ, this);
//		OrigamiFolder.FoldSquare (EdgeType.VERT, this);
//		OrigamiFolder.FoldSquare (EdgeType.DIAG_RIGHT, this);
//		OrigamiFolder.FoldSquare (EdgeType.DIAG_LEFT, this);




	}

	public void ResetStage() {
		stage = 0;
	}

	public void ResetTargetPocket() {
		targetP = null;
	}

	public void ResetIV() {
		iv = null;
	}

    public void MoveToNextStage()
    {
        stage++;
    }

	public void AddFold() {
		numFolds++;
	}

	void CollectChildBounds() {
		gameObject.AddComponent<MeshFilter> ();
		Bounds bounds = transform.GetComponent<MeshFilter> ().mesh.bounds;
		foreach(MeshFilter mr in transform.GetComponentsInChildren<MeshFilter>())
		{
			bounds.Encapsulate(mr.mesh.bounds);
		}
		transform.GetComponent<MeshFilter> ().mesh.bounds = bounds;
	}

	void ChangeColor() {
		Color rand_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
		foreach (Renderer rend in transform.GetComponentsInChildren<Renderer>()) {
			rend.material.color = rand_color;
		}
	}

    /*
     * Fold on a new edge, meaning that we must go through the list of pockets
     * again to see if a new pocket had been created as a result of the fold.
     */
    public void InsertEdge(TransformEdge e) {
		EdgeType et = e.edge_type;

		// if there is at least one edge, then we can create pockets
		if (pockets.Count > 0) {
			List<Pocket> toDiscard = new List<Pocket> ();
			HashSet<Pocket> toAdd = new HashSet<Pocket> ();
			foreach (Pocket p in pockets) {
				// if edge crosses with pocket in the x,z
				if (UnityHelper.IntersectsPocket (e, p, et)) {
					// save the two edges of that pocket e1,e2
					TransformEdge e1 = p.edge1;
					TransformEdge e2 = p.edge2;
					// erase the pocket
					toDiscard.Add (p);

//					print ("Discarding pocket " + p.ToString());
                    // create 2 new pockets e-e1,e-e2
                    Pocket p1 = new Pocket(e, e1);
                    Pocket p2 = new Pocket(e, e2);
                    toAdd.Add (p1); 
					toAdd.Add (p2);
				}
			}
			pockets.RemoveAll (t => toDiscard.Contains(t));
			pockets.AddRange(toAdd);


		} else  {
			// create a pocket with edges not of this type
			foreach (EdgeType et1 in edges.Keys) {
				if (et1 != et) {
					foreach (TransformEdge e1 in edges[et1]) {
                        Pocket p = new Pocket(e, e1);
						pockets.Add (p);
					}
				}
			}



		}
//		string output = "";
//		foreach (Pocket p in pockets) {
//			output += p.ToString () + "\n";
//		}
//		print(output);
		if (!edges.ContainsKey (et)) {
			edges.Add (et, new List<TransformEdge> ());
		}
		edges [et].Add (e);

	}

	public Transform GetIV() {
		if (iv == null) {
			switch (numFolds) {
			case 0:
			case 1:
				ChooseRandomIV ();
				break;
			case 2:
				ChooseCenterIV ();
				break;
			case 3:
			case 4:
				ChooseEdgeIV ();
				break;
			default:
				break;

			}

		}
		return iv;
	}

	public Transform GetIVNeighbour1() {
		if (ivNeighbor1 == null) {
			GetIV ();
			GetIVNeighbours ();
		}
		return ivNeighbor1;
	}

	public Transform GetIVNeighbour2() {
		if (ivNeighbor2 == null) {
			GetIV ();
			GetIVNeighbours ();
		}
		return ivNeighbor2;
	}

	/*
	 * Set iv to the center vertice and the neighbours to be the edges of
	 * the first pocket in the pocket list.
	 */
	private void ChooseCenterIV() {
		iv = center;	
	}

	/* 
	 * Set iv to a random vertex, excluding the center vertex 
	 * Vertices are labelled 'Vx' where x in range 1-8
	 */
	private void ChooseRandomIV() {
		int vNum = (int)(UnityEngine.Random.Range (0, NUM_VERTICES));
		iv = transform.Find ("V" + vNum);
	}

	/*
	 * Set iv to a vertex on the outside of the unit (not in between vertices).
	 * ASSUMPTION: a vertex with the least number of colliders is on the edge
	 */
	private void ChooseEdgeIV() {
		List<Tuple<int,Transform>> vColliders = new List<Tuple<int,Transform>> ();
		for (int i = 0; i < NUM_VERTICES; i++) {
			Transform vertice = transform.Find ("V" + i);
			Collider[] cols = Physics.OverlapSphere (vertice.position, FourSquare.PAPER_THICKNESS);
			vColliders.Add (new Tuple<int,Transform> (cols.Length, vertice));
		}
		vColliders.Sort(delegate(Tuple<int, Transform> x, Tuple<int, Transform> y) {
			if (x.first < y.first)
				return -1;
			else if (x.first == y.first)
				return 0;
			else
				return 1;
		});
		iv = vColliders [0].second;
	}

    

	// vertices are labelled 'Vx' where x in range 1-8
	public Transform ChooseInsertionVertice(float probCenterInsertion) {
		int lowerBound = 0;
		int higherBound = 0;
		if (UnityHelper.ApproxSameFloat (probCenterInsertion, 1)) {
			lowerBound = -1;
		} else {
			int centerChoice = (int) ( (NUM_VERTICES * probCenterInsertion) / (1 - probCenterInsertion));
			lowerBound = -centerChoice;
			higherBound = NUM_VERTICES;
		}

		while (iv == null) {
			int vNum = (int) (UnityEngine.Random.Range(lowerBound,higherBound));
			// -1 means center
			if (vNum < 0 && pockets.Count > 0) {
				iv = center;
				Pocket p = pockets [0];
				ivNeighbor1 = p.edge1.end;
				ivNeighbor2 = p.edge2.end;
			} else if (vNum >= 0) {
				iv = transform.Find("V" + vNum);
				GetIVNeighbours ();
			}

		}
//		print (ivNeighbor1.name + "\n" + ivNeighbor2.name);

		return iv;
	}
		
	/*
	 * Get neighbouring vertices for the insertion vertex, in order to determine
	 * if the unit can fit into the pocket using this vertex and for finding the alignment
	 * plane.
	 * 
	 * If the insertion vertex is the center vertex, assert that there is at least one pocket.
	 */
	private void GetIVNeighbours() {
		if (iv.Equals(center)) {
			Debug.Assert (pockets.Count > 0, "Can only get neighbors for center iv if pocket count > 0 !");
			Pocket p = pockets [0];
			ivNeighbor1 = p.edge1.end;
			ivNeighbor2 = p.edge2.end;
		} else {
			int vNum = Int32.Parse (iv.name.Substring (1));
	
			Transform vBefore = transform.Find ("V" + ((vNum - 1 + NUM_VERTICES) % NUM_VERTICES));
			Transform vAfter = transform.Find ("V" + ((vNum + 1) % NUM_VERTICES));

			List<Transform> diffVects = new List<Transform> ();
			diffVects.Add (vBefore);
			diffVects.Add (vAfter);
			diffVects.Add (center);

			// the neighbouring vertices of Vi are the vertices (between Vi-1, Vi+1, and Center) that
			// are the furthest away of Vi
			// important because if Vi-1, or Vi+1 are touching Vi, then the alignment plane will be
			// calculated incorrectly
			float maxDistance = 0;
			Vector3 temp1, temp2;
			for (int i = 0; i < diffVects.Count; i++) {
				temp1 = diffVects [i].position;
				for (int j = i; j < diffVects.Count; j++) {
					temp2 = diffVects [j].position;
					if (Vector3.Distance (temp1, temp2) > maxDistance) {
						maxDistance = Vector3.Distance (temp1, temp2);
						ivNeighbor1 = diffVects [i];
						ivNeighbor2 = diffVects [j];
					}
				}
			}
		}
	}

	public Vector3 GetAlignmentV3() {
		Vector3 v1 = ivNeighbor1.position - iv.position;
		Vector3 v2 = ivNeighbor2.position - iv.position;
		Vector3 sum = v1 + v2;
		sum.x = -sum.x;
		sum.y = -sum.y;
		sum.z = -sum.z;
		return sum;
	}




    public void CalcSelfRotationV3()
    {
        Vector3 pointToTarget = targetP.pCenter.position - iv.position;
		Vector3 pointToCenter = GetAlignmentV3();
        selfRotationV3 = Vector3.Cross(pointToTarget, pointToCenter);
    }

    public void CalcTargetRotationV3()
    {
		// perpendicular vector to rotate around
        targetRotationV3 = Vector3.Cross(
			iv.position - targetP.pCenter.position,
            targetP.GetVectorIn());

    }


	public void Disable() {
		enabled = false;

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

		foreach (var pocket in pockets) {
			if (!pocket.filled) {
				Debug.DrawLine (pocket.edge1.start.position, pocket.edge1.end.position, pocket.color);
				Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.end.position, pocket.color);
	//			Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.start.position + pocket.GetVectorIn(), Color.blue);
			}
		}
//		print ((transform.Find ("V0").localPosition - center.localPosition).y);


	}

}
