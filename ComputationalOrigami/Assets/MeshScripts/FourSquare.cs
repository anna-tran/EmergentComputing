using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourSquare : MonoBehaviour {
	public static float PAPER_THICKNESS = 0.01f;

    // edges are at local positions, relative to the center
	List<TransformEdge> edges;
	Transform center;

	// Use this for initialization
	void Start () {
		center = gameObject.transform.Find ("Center");
		edges = new List<TransformEdge> ();
        RotateHorz();
        //RotateVert();
		foreach (TransformEdge e in edges) {
			print (e);
		}
	}

	void AssignLowest(ref float curr, float value)
    {
        if (value < curr)
        {
            curr = value;
        }
    }

	void AssignHighest(ref float curr, float value)
    {
        if (value > curr)
        {
            curr = value;
        }
    }




    void RotateHorz() {
        // direction = up
        // temporarily parent each child to a new GameObject
        GameObject c_parent = new GameObject();
        c_parent.transform.position = transform.position;

        List<Transform> children_to_group = new List<Transform>();
		float height_folded_on = 0, height_to_fold = 0;
		Transform t_lowest_z = center;
		Transform t_highest_z = center;

		Transform neg_z = transform.Find ("-Z");
		Transform z = transform.Find ("Z");
		if (t_lowest_z.position.z > neg_z.position.z) {
			t_lowest_z = neg_z;
		}
		if (t_highest_z.position.z < z.position.z) {
			t_highest_z = z;
		}

		foreach (Transform child in transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - transform.position.y;
			if (child.position.x < transform.position.x) {
				// group children left of the center
				children_to_group.Add (child);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}
		AddPaperThickness (ref height_to_fold, ref height_folded_on);

		ParentTo (children_to_group, ref c_parent);

		float topmost_y = height_folded_on + height_to_fold;
		c_parent.transform.Rotate(new Vector3(0,0,1),180);
		c_parent.transform.Translate(new Vector3(0,-topmost_y,0));

        // reparent children to this object
		ReparentFrom(ref c_parent);
        
		FlipEdges (EdgeType.HORZ);

		if (Mathf.Abs(t_lowest_z.position.z - 0) > Mathf.Epsilon) {
			edges.Add(new TransformEdge(center, t_lowest_z, EdgeType.HORZ));
        }
		if (Mathf.Abs(t_highest_z.position.z - 0) > Mathf.Epsilon) {
			edges.Add(new TransformEdge(center, t_highest_z, EdgeType.HORZ));
        }

    }
	/*
	void RotateVert() {
        // direction = up
        // temporarily parent each child to a new GameObject
        GameObject c_parent = new GameObject();
        c_parent.transform.position = transform.position;

        List<Transform> children_to_group = new List<Transform>();
		float lowest_x = 0, highest_x = 0, height_folded_on = 0, height_to_fold = 0;
		foreach (Transform child in transform) {
			float y_val = child.GetComponent<MeshRenderer> ().bounds.center.y - transform.position.y;
			if (child.position.z < transform.position.z) {
				// group children south of the center
				children_to_group.Add (child);
				float x_val = child.GetComponent<MeshRenderer> ().bounds.center.x - transform.position.x;
				AssignLowest (ref lowest_x, x_val);
				AssignHighest (ref highest_x, x_val);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}
		AddPaperThickness (ref height_to_fold, ref height_folded_on);

		ParentTo (children_to_group, ref c_parent);

		float topmost_y = height_folded_on + height_to_fold;
		c_parent.transform.Rotate(new Vector3(1,0,0),180);
		c_parent.transform.Translate(new Vector3(0,-topmost_y,0));


        // reparent children to this object
		ReparentFrom(ref c_parent);

		FlipEdges (EdgeType.VERT);

		if (Mathf.Abs(lowest_x - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(lowest_x, topmost_y/2, 0);
			edges.Add(new FoldedEdge(FoldedEdge.ORIGIN, v, EdgeType.VERT));
		}
		if (Mathf.Abs(highest_x - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(highest_x, topmost_y/2, 0);
			edges.Add(new FoldedEdge(FoldedEdge.ORIGIN, v, EdgeType.VERT));
		}



    }
*/
	void FlipVert(TransformEdge e) {
		if (e.start.position.z < center.position.z) {
			string str = "Before\n" + e.start.position;
			Vector3 vertVect = e.start.position;
			vertVect.z = -1 * vertVect.z;
			e.start.position = vertVect;
			print (str + "\nFlipped vert\n" + e.start.position);
		}
		if (e.end.position.z < center.position.z) {
			string str = "Before\n" + e.end.position;
			Vector3 vertVect = e.end.position;
			vertVect.z = -1 * vertVect.z;
			e.end.position = vertVect;
			print (str + "Flipped vert\n" + e.end.position);
		}
	}

	void FlipHorz(TransformEdge e) {
		if (e.start.position.x < center.position.x) {
			string str = "Before\n" + e.start.position;
			Vector3 vertVect = e.start.position;
			vertVect.x = -1 * vertVect.x;
			e.start.position = vertVect;
			print (str + "\nFlipped horz\n" + e.start.position);
		}
		if (e.end.position.x < center.position.x) {
			string str = "Before\n" + e.end.position;
			Vector3 vertVect = e.end.position;
			vertVect.x = -1 * vertVect.x;
			e.end.position = vertVect;
			print (str + "Flipped horz\n" + e.end.position);
		}
	}

	void FlipEdges(EdgeType toFlip) {
		foreach (TransformEdge e in edges) {
			if (toFlip == EdgeType.HORZ) {
				FlipHorz (e);
			} else if (toFlip == EdgeType.VERT) {
				FlipVert (e);
			}
		}
	}

	void RotateDiag()
	{

	}

	void AddPaperThickness(ref float height_to_fold, ref float height_folded_on) {
		height_to_fold += PAPER_THICKNESS / 2;        
		height_folded_on += PAPER_THICKNESS / 2;
		print ("height folded on: " + height_folded_on);
		print ("height to fold: " + height_to_fold);

		Vector3 vertVect = center.position;
		vertVect.y = (height_to_fold + height_folded_on) / 2;
		center.position = vertVect;
	}

	void ParentTo(List<Transform> children_to_group, ref GameObject c_parent) {
		foreach (Transform child in children_to_group)
			child.parent = c_parent.transform;
	}

	void ReparentFrom(ref GameObject c_parent) {
		while (c_parent.transform.childCount > 0)
			c_parent.transform.GetChild(0).parent = transform;
		Destroy (c_parent);
	}


	
	// Update is called once per frame
	void Update () {
		foreach (TransformEdge e in edges) {
			Debug.DrawLine (transform.position + e.start.position, transform.position + e.end.position, Color.red);
		}
	}
}

/*

public class FourSquare : MonoBehaviour {
	public static float PAPER_THICKNESS = 0.01f;

    // edges are at local positions, relative to the center
	List<FoldedEdge> edges;
	Vector3 center;

	// Use this for initialization
	void Start () {
		center = new Vector3 (0, 0, 0);
		edges = new List<FoldedEdge> ();
        RotateHorz();
        RotateVert();
		foreach (FoldedEdge e in edges) {
			print (e);
		}
	}

    void AssignLowest(ref float a, float b)
    {
        if (b < a)
        {
            a = b;
        }
    }

    void AssignHighest(ref float a, float b)
    {
        if (b > a)
        {
            a = b;
        }
    }




    void RotateHorz() {
        // direction = up
        // temporarily parent each child to a new GameObject
        GameObject c_parent = new GameObject();
        c_parent.transform.position = transform.position;

        List<Transform> children_to_group = new List<Transform>();
		float lowest_z = 0, highest_z = 0, height_folded_on = 0, height_to_fold = 0;
		foreach (Transform child in transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - transform.position.y;
			if (child.position.x < transform.position.x) {
				// group children left of the center
				children_to_group.Add (child);
				float z_val = child.GetComponent<MeshRenderer>().bounds.center.z - transform.position.z;
				AssignLowest (ref lowest_z, z_val);
				AssignHighest (ref highest_z, z_val);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}
		AddPaperThickness (ref height_to_fold, ref height_folded_on);

		ParentTo (children_to_group, ref c_parent);

		float topmost_y = height_folded_on + height_to_fold;
		c_parent.transform.Rotate(new Vector3(0,0,1),180);
		c_parent.transform.Translate(new Vector3(0,-topmost_y,0));

        // reparent children to this object
		ReparentFrom(ref c_parent);
        
		FlipEdges (EdgeType.HORZ);

        if (Mathf.Abs(lowest_z - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(0, topmost_y/2, lowest_z);
			edges.Add(new FoldedEdge(FoldedEdge.ORIGIN, v, EdgeType.HORZ));
        }
        if (Mathf.Abs(highest_z - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(0, topmost_y/2, highest_z);
			edges.Add(new FoldedEdge(FoldedEdge.ORIGIN, v, EdgeType.HORZ));
        }

    }

	void RotateVert() {
        // direction = up
        // temporarily parent each child to a new GameObject
        GameObject c_parent = new GameObject();
        c_parent.transform.position = transform.position;

        List<Transform> children_to_group = new List<Transform>();
		float lowest_x = 0, highest_x = 0, height_folded_on = 0, height_to_fold = 0;
		foreach (Transform child in transform) {
			float y_val = child.GetComponent<MeshRenderer> ().bounds.center.y - transform.position.y;
			if (child.position.z < transform.position.z) {
				// group children south of the center
				children_to_group.Add (child);
				float x_val = child.GetComponent<MeshRenderer> ().bounds.center.x - transform.position.x;
				AssignLowest (ref lowest_x, x_val);
				AssignHighest (ref highest_x, x_val);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}
		AddPaperThickness (ref height_to_fold, ref height_folded_on);

		ParentTo (children_to_group, ref c_parent);

		float topmost_y = height_folded_on + height_to_fold;
		c_parent.transform.Rotate(new Vector3(1,0,0),180);
		c_parent.transform.Translate(new Vector3(0,-topmost_y,0));


        // reparent children to this object
		ReparentFrom(ref c_parent);

		FlipEdges (EdgeType.VERT);

		if (Mathf.Abs(lowest_x - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(lowest_x, topmost_y/2, 0);
			edges.Add(new FoldedEdge(FoldedEdge.ORIGIN, v, EdgeType.VERT));
		}
		if (Mathf.Abs(highest_x - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(highest_x, topmost_y/2, 0);
			edges.Add(new FoldedEdge(FoldedEdge.ORIGIN, v, EdgeType.VERT));
		}



    }

	void FlipVert(FoldedEdge e) {
		if (!e.start.Equals(FoldedEdge.ORIGIN) && e.start.z < center.z) {
			string str = "Before\n" + e.start;
			Vector3 vertVect = e.start;
			vertVect.z = -1 * vertVect.z;
			e.start = vertVect;
			print (str + "\nFlipped vert\n" + e.start);
		}
		if (e.end.z < center.z) {
			string str = "Before\n" + e.end;
			Vector3 vertVect = e.end;
			vertVect.z = -1 * vertVect.z;
			e.end = vertVect;
			print (str + "Flipped vert\n" + e.end);
		}
	}

	void FlipHorz(FoldedEdge e) {
		if (!e.start.Equals(FoldedEdge.ORIGIN) && e.start.x < center.x) {
			string str = "Before\n" + e.start;
			Vector3 vertVect = e.start;
			vertVect.x = -1 * vertVect.x;
			e.start = vertVect;
			print (str + "\nFlipped horz\n" + e.start);
		}
		if (e.end.x < center.x) {
			string str = "Before\n" + e.end;
			Vector3 vertVect = e.end;
			vertVect.x = -1 * vertVect.x;
			e.end = vertVect;
			print (str + "Flipped horz\n" + e.end);
		}
	}

	void FlipEdges(EdgeType toFlip) {
		foreach (FoldedEdge e in edges) {
			if (toFlip == EdgeType.HORZ) {
				FlipHorz (e);
			} else if (toFlip == EdgeType.VERT) {
				FlipVert (e);
			}
		}
	}

	void RotateDiag()
	{

	}

	void AddPaperThickness(ref float height_to_fold, ref float height_folded_on) {
		height_to_fold += PAPER_THICKNESS / 2;        
		height_folded_on += PAPER_THICKNESS / 2;
		print ("height folded on: " + height_folded_on);
		print ("height to fold: " + height_to_fold);
		center.y = (height_to_fold + height_folded_on) / 2;
	}

	void ParentTo(List<Transform> children_to_group, ref GameObject c_parent) {
		foreach (Transform child in children_to_group)
			child.parent = c_parent.transform;
	}

	void ReparentFrom(ref GameObject c_parent) {
		while (c_parent.transform.childCount > 0)
			c_parent.transform.GetChild(0).parent = transform;
		Destroy (c_parent);
	}


	
	// Update is called once per frame
	void Update () {
		foreach (FoldedEdge e in edges) {
			Vector3 start = center;
			if (!e.start.Equals(FoldedEdge.ORIGIN)) {
				start = e.start;
			}
			Debug.DrawLine (transform.position + start, transform.position + e.end, Color.red);
		}
	}
}
*/