using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrigamiFolder {

	public static void RotateHorz(FourSquare square) {
		// temporarily parent each child to a new GameObject

		GameObject c_parent = new GameObject();
		c_parent.transform.position = square.transform.position;

		List<Transform> children_to_group = new List<Transform>();
		float height_folded_on = 0, height_to_fold = 0;
		Transform t_lowest_z = square.center;
		Transform t_highest_z = square.center;

		Transform neg_z = square.transform.Find ("V3");	// -Z
		Transform z = square.transform.Find ("V7");		// Z
		if (t_lowest_z.position.z > neg_z.position.z) {
			t_lowest_z = neg_z;
		}
		if (t_highest_z.position.z < z.position.z) {
			t_highest_z = z;
		}

		foreach (Transform child in square.transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
			if (child.position.x < square.transform.position.x) {
				// group children left of the center
				children_to_group.Add (child);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}

		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
		foreach (Transform t in new List<Transform>{square.center, t_lowest_z, t_highest_z}) {
			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
		}

		ParentTo (children_to_group, c_parent);
		DoVisualFold (c_parent.transform, new Vector3 (0, 0, 1), height_folded_on, height_to_fold);
		ReparentFrom (ref c_parent, square);

		FlipEdges (square, EdgeType.HORZ);

		if (Mathf.Abs(t_lowest_z.position.z - 0) > Mathf.Epsilon) {
			TransformEdge te = new TransformEdge (square.center, t_lowest_z, EdgeType.HORZ);
			square.edges[EdgeType.HORZ].Add(te);
		}
		if (Mathf.Abs(t_highest_z.position.z - 0) > Mathf.Epsilon) {
			TransformEdge te = new TransformEdge (square.center, t_highest_z, EdgeType.HORZ);
			square.edges[EdgeType.HORZ].Add(new TransformEdge(square.center, t_highest_z, EdgeType.HORZ));
		}
		square.numFolds += 1;

	}


	public static void RotateVert(FourSquare square) {
		// temporarily parent each child to a new GameObject
		GameObject c_parent = new GameObject();
		c_parent.transform.position = square.transform.position;

		List<Transform> children_to_group = new List<Transform>();
		float height_folded_on = 0, height_to_fold = 0;
		Transform t_lowest_x = square.center;
		Transform t_highest_x = square.center;

		Transform neg_x = square.transform.Find ("V5");	// -X
		Transform x = square.transform.Find ("V1");		// X
		if (t_lowest_x.position.x > neg_x.position.x) {
			t_lowest_x = neg_x;
		}
		if (t_highest_x.position.x < x.position.x) {
			t_highest_x = x;
		}

		foreach (Transform child in square.transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
			if (child.position.z < square.transform.position.z) {
				// group children left of the center
				children_to_group.Add (child);
				AssignHighest (ref height_to_fold, y_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				AssignHighest (ref height_folded_on, y_val);
			}
		}
		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
		foreach (Transform t in new List<Transform>{square.center, t_lowest_x, t_highest_x}) {
			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
		}

		ParentTo (children_to_group, c_parent);
		DoVisualFold (c_parent.transform, new Vector3 (1, 0, 0), height_folded_on, height_to_fold);
		ReparentFrom (ref c_parent, square);

		FlipEdges (square, EdgeType.VERT);

		if (Mathf.Abs(t_lowest_x.position.x - 0) > Mathf.Epsilon) {
			square.edges[EdgeType.VERT].Add(new TransformEdge(square.center, t_lowest_x, EdgeType.VERT));
		}
		if (Mathf.Abs(t_highest_x.position.x - 0) > Mathf.Epsilon) {
			square.edges[EdgeType.VERT].Add(new TransformEdge(square.center, t_highest_x, EdgeType.VERT));
		}

		square.numFolds += 1;

	}


	static void AssignLowest(ref float curr, float value){
		if (value < curr) {
			curr = value;
		}
	}

	static void AssignHighest(ref float curr, float value)	{
		if (value > curr) {
			curr = value;
		}
	}
	static void AddPaperThickness(ref float height_to_fold, ref float height_folded_on, float PAPER_THICKNESS) {
		height_to_fold += PAPER_THICKNESS / 2;        
		height_folded_on += PAPER_THICKNESS / 2;
//		Debug.Log ("height folded on: " + height_folded_on);
//		Debug.Log ("height to fold: " + height_to_fold);
	}

	static void ParentTo(List<Transform> children_to_group, GameObject c_parent) {
		foreach (Transform child in children_to_group)
			child.parent = c_parent.transform;
	}

	static void ReparentFrom(ref GameObject c_parent, FourSquare square) {
		while (c_parent.transform.childCount > 0)
			c_parent.transform.GetChild(0).parent = square.transform;
		GameObject.Destroy (c_parent);
	}

	static void DoVisualFold(Transform c_parent_transform, Vector3 rotate_vector, 
		float height_folded_on, float height_to_fold) {
		float topmost_y = height_folded_on + height_to_fold;
//		Debug.Log ("topmost y: " + topmost_y);
//		Debug.Log ("transform position " + c_parent_transform.position);
		c_parent_transform.Rotate(rotate_vector,180);
		c_parent_transform.Translate(new Vector3(0,-topmost_y,0));
	}

	static void FlipVert(TransformEdge e, FourSquare square) {
		if (e.start.position.z < square.center.position.z) {
			string str = "Before\n" + e.start.position;
			SetVector3Value (e.start, "z", -1 * e.start.position.z);
//			Debug.Log (str + "\nFlipped vert\n" + e.start.position);
		}
		if (e.end.position.z < square.center.position.z) {
			string str = "Before\n" + e.end.position;
			SetVector3Value (e.end, "z", -1 * e.end.position.z);
//			Debug.Log (str + "Flipped vert\n" + e.end.position);
		}
	}

	static void FlipHorz(TransformEdge e, FourSquare square) {
		if (e.start.position.x < square.center.position.x) {
			string str = "Before\n" + e.start.position;
			SetVector3Value (e.start, "x", -1 * e.start.position.x);
//			Debug.Log (str + "\nFlipped horz\n" + e.start.position);
		}
		if (e.end.position.x < square.center.position.x) {
			string str = "Before\n" + e.end.position;
			SetVector3Value (e.start, "x", -1 * e.start.position.x);
//			Debug.Log (str + "Flipped horz\n" + e.end.position);
		}
	}

	static void FlipEdges(FourSquare square, EdgeType type_to_flip) {
		foreach (TransformEdge e in square.edges[type_to_flip]) {
			if (type_to_flip == EdgeType.HORZ) {
				FlipHorz (e, square);
			} else if (type_to_flip == EdgeType.VERT) {
				FlipVert (e, square);
			}
		}
	}

	static void SetVector3Value(Transform orig, string field, float value) {
		Vector3 vect = orig.position;
		if (field.ToLower().Equals ("x"))
			vect.x = value;
		else if (field.ToLower().Equals ("y"))
			vect.y = value;
		else if (field.ToLower().Equals ("z"))
			vect.z = value;
		orig.position = vect;
	}

}
