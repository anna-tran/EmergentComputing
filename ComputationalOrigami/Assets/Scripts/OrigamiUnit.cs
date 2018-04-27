using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * Class to model a modular origami unit.
 * Its function is to find some target pocket to insert itself into.
 * The choice of pocket and the corner (vertex) from which the unit will insert itself
 * is dependent on the number of folds it has.
 */
public class OrigamiUnit : MonoBehaviour {
    public static float PAPER_THICKNESS = 0.03f;
    public static int NUM_VERTICES = 8;

   
	public SortedList<EdgeType, List<TransformEdge>> edges { get; private set; }	 	// edges are at local positions, relative to the center
    public List<Pocket> pockets { get; private set; }									// pockets formed by unit's folds
	public Transform center { get; private set; }										// center (vertex) of unit
	public Color defaultColor { get; private set; }										// default color of unit

    public Pocket targetP { get; set; }						// target pocket to insert into
    // iv   insertion vertex
	private Transform iv;									// insertion vertex (vertex at which the unit will be inserted into the pocket)
	private Transform ivNeighbor1;							// neighbours are used to help calculate the self rotation vector
	private Transform ivNeighbor2;							
    public bool rendMesh;									// flag to render the unit's mesh or not
    public int stage { get; private set; }					// stage of the unit
    public Vector3 targetRotationV3 { get; private set; }	// vector to rotate around target pocket
    public Vector3 selfRotationV3 { get; private set; }		// vector to rotate unit itself to face the pocket
	public int numFolds { get; private set; }				// number of folds of the unit

	
	void Awake () {
		center = transform.Find ("Center");
		defaultColor = transform.Find ("Tri1-1").GetComponent<Renderer> ().material.color;
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

		// set unique color to more easily distinguish from other units
		ChangeColor ();
		// the bounds of this unit encompasses all the bounds of its children
		CollectChildBounds ();

	}

	/*
	* Reset unit stage to 0.
	*/
	public void ResetStage() {
		stage = 0;
	}

	/*
	 * Reset the target pocket to null.
	 */
	public void ResetTargetPocket() {
		targetP = null;
	}
	/*
	 * Reset the insertion vertex to null.
	 */
	public void ResetIV() {
		iv = null;
	}

	/*
	 * Reset the color to the default color.
	 */
	public void ResetColor() {
		foreach (Renderer rend in transform.GetComponentsInChildren<Renderer>()) {
			rend.material.color = defaultColor;
		}
	}

	/*
	 * Move to the next stage.
	 */
    public void MoveToNextStage()
    {
        stage++;
    }

	/*
	 * Must increment the fold count each time a new fold has been applied to the unit.
	 */
	public void AddFold() {
		numFolds++;
	}

	/*
	 * Collect the child bounds into the unit's own mesh bounds
	*/
	void CollectChildBounds() {
		gameObject.AddComponent<MeshFilter> ();
		Bounds bounds = transform.GetComponent<MeshFilter> ().mesh.bounds;
		foreach(MeshFilter mr in transform.GetComponentsInChildren<MeshFilter>())
		{
			bounds.Encapsulate(mr.mesh.bounds);
		}
		transform.GetComponent<MeshFilter> ().mesh.bounds = bounds;
	}

	/*
	 * Change the unit's color to a random color.
	 */
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
		
		if (!edges.ContainsKey (et)) {
			edges.Add (et, new List<TransformEdge> ());
		}
		edges [et].Add (e);

	}

	/*
	 * Get the insertion vertex. If the vertex has not been initialized,
	 * initialize it depending on the number of folds it has.
	 */
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

	/*
	 * Get the first insertion vertex neighbour.
	 * If it is null, initialize the insertion vertex before initializing the neigbours.
	 */
	public Transform GetIVNeighbour1() {
		if (ivNeighbor1 == null) {
			GetIV ();
			GetIVNeighbours ();
		}
		return ivNeighbor1;
	}

	/*
	 * Get the second insertion vertex neighbour.
	 * If it is null, initialize the insertion vertex before initializing the neigbours.
	 */
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
	 * This vertex is assumed to be the furthest of all vertice from the center.
	 */
	private void ChooseEdgeIV() {
		List<Tuple<float,Transform>> vertDistances = new List<Tuple<float,Transform>> ();
		for (int i = 0; i < NUM_VERTICES; i++) {
			Transform vertex = transform.Find ("V" + i);
			float distFromCenter = (vertex.localPosition - center.localPosition).sqrMagnitude;
			vertDistances.Add (new Tuple<float,Transform> (distFromCenter, vertex));
		}
		vertDistances.Sort(delegate(Tuple<float, Transform> x, Tuple<float, Transform> y) {
			if (x.first > y.first)
				return -1;
			else if (x.first == y.first)
				return 0;
			else
				return 1;
		});
		iv = vertDistances [0].second;
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

	/*
	 * Get the directional vector of where the insertion vertex is currently facing.
	 */
	public Vector3 GetAlignmentV3() {
		Vector3 v1 = ivNeighbor1.position - iv.position;
		Vector3 v2 = ivNeighbor2.position - iv.position;
		Vector3 sum = v1 + v2;
		sum.x = -sum.x;
		sum.y = -sum.y;
		sum.z = -sum.z;
		return sum;
	}


	/*
	 * Calculate the vector to rotate around so that the unit will face the target pocket's center.
	 */
    public void CalcSelfRotationV3()
    {
        Vector3 pointToTarget = targetP.pCenter.position - iv.position;
		Vector3 pointToCenter = GetAlignmentV3();
        selfRotationV3 = Vector3.Cross(pointToTarget, pointToCenter);
    }

	/*
	 * Calculate the vector to rotate around the target pocket.
	 */
    public void CalcTargetRotationV3()
    {
		// perpendicular vector to rotate around
        targetRotationV3 = Vector3.Cross(
			iv.position - targetP.pCenter.position,
            targetP.GetVectorIn());

    }

	/*
		Disable the unit so that Update() no longer runs.
	 */
	public void Disable() {
		enabled = false;

	}



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
		// Draw a line between the ends of the pockets if they are not yet filled
		foreach (var pocket in pockets) {
			if (!pocket.filled) {
				Debug.DrawLine (pocket.edge2.end.position, pocket.edge1.end.position, pocket.color);
			}
		}


	}



}
