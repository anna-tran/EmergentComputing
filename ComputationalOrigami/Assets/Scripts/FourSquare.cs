using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FourSquare : MonoBehaviour {
    public static float PAPER_THICKNESS = 0.01f;
    private static int NUM_VERTICES = 8;

    private GameObject simulator;

    // edges are at local positions, relative to the center
    public SortedList<EdgeType, List<TransformEdge>> edges { get; private set; }
    public List<Pocket> pockets { get; private set; }
    public Transform center { get; private set; }

    public Pocket targetP { get; set; }
    // iv   insertion vertex
    public Transform iv { get; set; }
    public Transform ivNeighbor1 { get; private set; }
    public Transform ivNeighbor2 { get; private set; }
    public bool rendMesh;
    public int stage { get; private set; }
    public Vector3 targetRotationV3 { get; private set; }
    public Vector3 selfRotationV3 { get; private set; }
	public Bounds bounds {get;private set;}
	public Quaternion lookRot;

	// Use this for initialization
	void Awake () {
        simulator = GameObject.Find("Simulator");
		center = transform.Find ("Center");
		edges = new SortedList<EdgeType,List<TransformEdge>>();
		pockets = new List<Pocket> ();
		
		iv = null;
		rendMesh = true;
        selfRotationV3 = Vector3.zero;
        targetRotationV3 = Vector3.zero;
        stage = 0;
		targetP = null;

		ChangeColor ();
		CollectChildBounds ();
//		OrigamiFolder.FoldSquare (EdgeType.DIAG_RIGHT, this);

//		OrigamiFolder.FoldSquare (EdgeType.DIAG_LEFT, this);
//		OrigamiFolder.FoldSquare (EdgeType.HORZ, this);


	}

	public void ResetStage() {
		stage = 0;
	}

    public void MoveToNextStage()
    {
        stage++;
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

    // every time we add a pocket, notify the simulator
    public void InsertEdge(TransformEdge e) {
		EdgeType et = e.edge_type;

		// if there is at least one edge, then we can create pockets
		if (pockets.Count > 0) {
			List<Pocket> toDiscard = new List<Pocket> ();
			List<Pocket> toAdd = new List<Pocket> ();
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
                        Pocket p = new Pocket(e, e1);
						pockets.Add (p);
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
			Debug.Log ("lower bound insertion " + lowerBound + "\n" +
			"high bound insertion " + higherBound);
			int vNum = (int) (UnityEngine.Random.Range(lowerBound,higherBound));
			// -1 means center
			if (vNum < 0 && pockets.Count > 0) {
				iv = center;
				Pocket p = pockets [0];
				ivNeighbor1 = p.edge1.end;
				ivNeighbor2 = p.edge2.end;
			} else if (vNum >= 0) {
				iv = transform.Find("V" + vNum);
				GetIVNeighbors ();
			}

		}
//		print ("insertion vertice" + insertionVertice.name);
		return iv;
	}

	// get the connected vertices to find the fromDir vector
	private void GetIVNeighbors() {
		int vNum = Int32.Parse(iv.name.Substring(1));
//		print (vNum + "\n" + (((vNum - 1 + NUM_VERTICES) % NUM_VERTICES)) + "\n" + ((vNum + 1) % NUM_VERTICES));
	
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
					ivNeighbor1 = diffVects [i];
					ivNeighbor2 = diffVects [j];
				}
			}
		}
//		print (ivNeighbor1 + "\n" + ivNeighbor2);
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

	public bool AlignedWithTarget() {
		Vector3 v11 = targetP.edge1.end.position - ivNeighbor1.position;
		Vector3 v12 = iv.position - ivNeighbor1.position;
		Vector3 v21 = targetP.edge2.end.position - ivNeighbor2.position;
		Vector3 v22 = iv.position - ivNeighbor2.position;
		Plane plane1 = new Plane (iv.position, ivNeighbor1.position, ivNeighbor2.position);
		Plane plane2 = targetP.GetPocketPlane ();
			print ("plane1 " + plane1.normal + "\nplane2 " + plane2.normal);
		return UnityHelper.V3ApproxEqual (plane1.normal, plane2.normal);
	}


    public Vector3 GetTargetRotationV3()
    {
        if (targetRotationV3.Equals(Vector3.zero))
        {
            CalcTargetRotationV3();
        }
        return targetRotationV3;
    }

    public void CalcSelfRotationV3()
    {
        Vector3 pointToTarget = targetP.pCenter.position - iv.position;
//        Vector3 pointToCenter = ivNeighbor1.position - ivNeighbor2.position;
		Vector3 pointToCenter = GetAlignmentV3();
        selfRotationV3 = Vector3.Cross(pointToTarget, pointToCenter);
    }

    public void CalcTargetRotationV3()
    {
//        Vector3 targetDir = targetP.pCenter.position - transform.position;
//		Vector3 fromDir;
//		if (iv.Equals (center))
//			fromDir = ivNeighbor1.position - transform.position;
//		else
//        	fromDir = iv.position - transform.position;
//        transform.rotation = Quaternion.FromToRotation(fromDir, targetDir);
//        iv.LookAt(targetP.pCenter);


        // perpendicular vector to rotate around
        targetRotationV3 = Vector3.Cross(
			iv.position - targetP.pCenter.position,
            targetP.GetVectorIn() - targetP.pCenter.position);

    }

    public void Kill()
    {
        stage = -2;
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
			Debug.DrawLine (pocket.edge1.start.position, pocket.edge1.end.position, pocket.color);
			Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.end.position, pocket.color);
//			Debug.DrawLine (pocket.edge2.start.position, pocket.edge2.start.position + pocket.GetVectorIn(), Color.blue);
		}

	}

}
