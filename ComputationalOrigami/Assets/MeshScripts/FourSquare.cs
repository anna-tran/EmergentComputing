using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FourSquare : MonoBehaviour {
	public static float PAPER_THICKNESS = 0.01f;

    // edges are at local positions, relative to the center
	SortedList<EdgeType,List<TransformEdge>> edges;
	List<Pocket> pockets;
	Transform center;


	// Use this for initialization
	void Start () {
		center = gameObject.transform.Find ("Center");
		edges = new SortedList<EdgeType,List<TransformEdge>>();
		pockets = new List<Pocket> ();
		foreach (var type in Enum.GetValues(typeof(EdgeType))) {
			EdgeType t = (EdgeType) Enum.ToObject(typeof(EdgeType), type);
			edges.Add (t, new List<TransformEdge> ());
		}

        RotateHorz();
        RotateVert();
		foreach (var pocket in pockets) {
			print (pocket);
		}
	}

	void GeneratePockets() {
		var edge_enums = Enum.GetValues (typeof(EdgeType));
		for (int i = 0; i < edge_enums.Length; i++) {
			EdgeType t1 = (EdgeType) Enum.ToObject(typeof(EdgeType), edge_enums[i]);

			// only match with the unseen edge types
			for (int j = i + 1; j < edge_enums.Length; j++) {
				EdgeType t2 = (EdgeType) Enum.ToObject(typeof(EdgeType), edge_enums[j]);

				foreach (TransformEdge e1 in edges[t1]) {
					foreach (TransformEdge e2 in edges[t2]) {
						pockets.Add (new Pocket (e1, e2));
					}
				}
			}
		}
	}


    void RotateHorz() {
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
		foreach (Transform t in new List<Transform>{center, t_lowest_z, t_highest_z}) {
			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
		}

		ParentTo (children_to_group, c_parent);
		DoVisualFold (c_parent.transform, new Vector3 (0, 0, 1), height_folded_on, height_to_fold);
		ReparentFrom (ref c_parent);
        
		FlipEdges (EdgeType.HORZ);

		if (Mathf.Abs(t_lowest_z.position.z - 0) > Mathf.Epsilon) {
			//edges.Add(new TransformEdge(center, t_lowest_z, EdgeType.HORZ));
			edges[EdgeType.HORZ].Add(new TransformEdge(center, t_lowest_z, EdgeType.HORZ));
        }
		if (Mathf.Abs(t_highest_z.position.z - 0) > Mathf.Epsilon) {
			//edges.Add(new TransformEdge(center, t_highest_z, EdgeType.HORZ));
			edges[EdgeType.HORZ].Add(new TransformEdge(center, t_highest_z, EdgeType.HORZ));
        }

    }


	void RotateVert() {
		// temporarily parent each child to a new GameObject
		GameObject c_parent = new GameObject();
		c_parent.transform.position = transform.position;

		List<Transform> children_to_group = new List<Transform>();
		float height_folded_on = 0, height_to_fold = 0;
		Transform t_lowest_x = center;
		Transform t_highest_x = center;

		Transform neg_x = transform.Find ("-X");
		Transform x = transform.Find ("X");
		if (t_lowest_x.position.x > neg_x.position.x) {
			t_lowest_x = neg_x;
		}
		if (t_highest_x.position.x < x.position.x) {
			t_highest_x = x;
		}

		foreach (Transform child in transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - transform.position.y;
			if (child.position.z < transform.position.z) {
				// group children left of the center
				children_to_group.Add (child);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}
		AddPaperThickness (ref height_to_fold, ref height_folded_on);
		foreach (Transform t in new List<Transform>{center, t_lowest_x, t_highest_x}) {
			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
		}

		ParentTo (children_to_group, c_parent);
		DoVisualFold (c_parent.transform, new Vector3 (1, 0, 0), height_folded_on, height_to_fold);
		ReparentFrom (ref c_parent);

		FlipEdges (EdgeType.VERT);

		if (Mathf.Abs(t_lowest_x.position.x - 0) > Mathf.Epsilon) {
			//edges.Add(new TransformEdge(center, t_lowest_x, EdgeType.VERT));
			edges[EdgeType.VERT].Add(new TransformEdge(center, t_lowest_x, EdgeType.VERT));
		}
		if (Mathf.Abs(t_highest_x.position.x - 0) > Mathf.Epsilon) {
			//edges.Add(new TransformEdge(center, t_highest_x, EdgeType.VERT));
			edges[EdgeType.VERT].Add(new TransformEdge(center, t_highest_x, EdgeType.VERT));
		}

	}


	void AssignLowest(ref float curr, float value){
		if (value < curr) {
			curr = value;
		}
	}

	void AssignHighest(ref float curr, float value)	{
		if (value > curr) {
			curr = value;
		}
	}
	void AddPaperThickness(ref float height_to_fold, ref float height_folded_on) {
		height_to_fold += PAPER_THICKNESS / 2;        
		height_folded_on += PAPER_THICKNESS / 2;
		print ("height folded on: " + height_folded_on);
		print ("height to fold: " + height_to_fold);
	}

	void ParentTo(List<Transform> children_to_group, GameObject c_parent) {
		foreach (Transform child in children_to_group)
			child.parent = c_parent.transform;
	}

	void ReparentFrom(ref GameObject c_parent) {
		while (c_parent.transform.childCount > 0)
			c_parent.transform.GetChild(0).parent = transform;
		Destroy (c_parent);
	}

	void DoVisualFold(Transform c_parent_transform, Vector3 rotate_vector, 
						float height_folded_on, float height_to_fold) {
		float topmost_y = height_folded_on + height_to_fold;
		c_parent_transform.Rotate(rotate_vector,180);
		c_parent_transform.Translate(new Vector3(0,-topmost_y,0));
	}

	void FlipVert(TransformEdge e) {
		if (e.start.position.z < center.position.z) {
			string str = "Before\n" + e.start.position;
			SetVector3Value (e.start, "z", -1 * e.start.position.z);
			print (str + "\nFlipped vert\n" + e.start.position);
		}
		if (e.end.position.z < center.position.z) {
			string str = "Before\n" + e.end.position;
			SetVector3Value (e.end, "z", -1 * e.end.position.z);
			print (str + "Flipped vert\n" + e.end.position);
		}
	}

	void FlipHorz(TransformEdge e) {
		if (e.start.position.x < center.position.x) {
			string str = "Before\n" + e.start.position;
			SetVector3Value (e.start, "x", -1 * e.start.position.x);
			print (str + "\nFlipped horz\n" + e.start.position);
		}
		if (e.end.position.x < center.position.x) {
			string str = "Before\n" + e.end.position;
			SetVector3Value (e.start, "x", -1 * e.start.position.x);
			print (str + "Flipped horz\n" + e.end.position);
		}
	}

	void FlipEdges(EdgeType type_to_flip) {
		foreach (TransformEdge e in edges[type_to_flip]) {
			if (type_to_flip == EdgeType.HORZ) {
				FlipHorz (e);
			} else if (type_to_flip == EdgeType.VERT) {
				FlipVert (e);
			}
		}
	}

	void SetVector3Value(Transform orig, string field, float value) {
		Vector3 vect = orig.position;
		if (field.ToLower().Equals ("x"))
			vect.x = value;
		else if (field.ToLower().Equals ("y"))
			vect.y = value;
		else if (field.ToLower().Equals ("z"))
			vect.z = value;
		orig.position = vect;
	}



	
	// Update is called once per frame
	void Update () {
		foreach (var type in Enum.GetValues(typeof(EdgeType))) {
			EdgeType t = (EdgeType) Enum.ToObject(typeof(EdgeType), type);
			foreach (TransformEdge e in edges[t]) {
				Debug.DrawLine (e.start.position, e.end.position, Color.red);
			}
		}
	}
}
