using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrigamiFolder {

	public static void FoldSquare(EdgeType et, FourSquare square) {
		// temporarily parent each child to a new GameObject

		GameObject c_parent = new GameObject();
		c_parent.transform.position = square.transform.position;

		List<Transform> children_to_group = new List<Transform>();
		List<Transform> children_to_fold_on = new List<Transform>();

		float height_folded_on = 0, height_to_fold = 0;
		Transform lowBound = getLowBound (et, square);
		Transform highBound = getHighBound (et, square);

		if (shouldUpdateLB(et,lowBound,square)) {
			lowBound = square.center;
		}
		if (shouldUpdateHB(et,highBound,square)) {
			highBound = square.center;
		}
		Debug.Log (et);
		foreach (Transform child in square.transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
			if (shouldGroupChild(et,child,square)) {
				// group children left of the center
				children_to_group.Add (child);
//				Debug.Log ("added to group " + child.name);
				if (child.tag.ToLower() != "vertice")
					AssignHighest (ref height_to_fold, y_val);
			} else {
				children_to_fold_on.Add (child);
			}
		}

		if (children_to_group.Count == 0 
			|| (children_to_group.Count == 1 
				&& children_to_group[0].Equals(square.center)))
			return;

		height_folded_on = getHeightFoldedOn (children_to_group, children_to_fold_on, square, et);

		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
		foreach (Transform t in new List<Transform>{square.center, lowBound, highBound}) {
			UnityHelper.SetV3Value (t, "y", (height_to_fold + height_folded_on) / 2.0f);
		}


		if (!lowBound.Equals(square.center)) {
			TransformEdge te = new TransformEdge (square.center, lowBound, et);
			square.insertEdge(te);
		}
		if (!highBound.Equals(square.center)) {
			TransformEdge te = new TransformEdge (square.center, highBound, et);
			square.insertEdge(te);
		}

		ParentTo (children_to_group, c_parent);
		DoVisualFold (c_parent.transform, getFoldVector(et), height_folded_on, height_to_fold);
		ReparentFrom (ref c_parent, square);
	}

	static Transform getLowBound(EdgeType et, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return square.transform.Find ("V3");
		case EdgeType.VERT:
			return square.transform.Find ("V5");
		case EdgeType.DIAG_RIGHT:
			return square.transform.Find ("V6");
		case EdgeType.DIAG_LEFT:
			return square.transform.Find ("V0");
		default:
			return null;
		}
	}

	static Transform getHighBound(EdgeType et, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return square.transform.Find ("V7");
		case EdgeType.VERT:
			return square.transform.Find ("V1");
		case EdgeType.DIAG_RIGHT:
			return square.transform.Find ("V2");
		case EdgeType.DIAG_LEFT:
			return square.transform.Find ("V4");
		default:
			return null;
		}
	}

	static bool shouldUpdateLB(EdgeType et, Transform bound, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return bound.position.z > square.center.position.z;
		case EdgeType.VERT:
			return bound.position.x > square.center.position.x;
		case EdgeType.DIAG_RIGHT:
		case EdgeType.DIAG_LEFT:			
			float distCenter =  distanceFromFunction (et, square.center.position);
			float distB = distanceFromFunction (et, bound.GetComponent<MeshRenderer> ().bounds.center);
			return !UnityHelper.ApproxSameFloat(distB,distCenter);
		default:
			return false;
		}
	}

	static bool shouldUpdateHB(EdgeType et, Transform bound, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return bound.position.z < square.center.position.z;
		case EdgeType.VERT:
			return bound.position.x < square.center.position.x;
		case EdgeType.DIAG_RIGHT:
		case EdgeType.DIAG_LEFT:			
			float distCenter =  distanceFromFunction (et, square.center.position);
			float distB = distanceFromFunction (et, bound.GetComponent<MeshRenderer> ().bounds.center);
			return !UnityHelper.ApproxSameFloat(distB,distCenter);
		default:
			return false;
		}
	}

	static bool shouldGroupChild(EdgeType et, Transform child, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return child.position.x < square.transform.position.x;
		case EdgeType.VERT:
			return child.position.z < square.transform.position.z;
		case EdgeType.DIAG_RIGHT:
			MeshRenderer childR = child.GetComponent<MeshRenderer> ();
			return UnityHelper.ApproxSameFloat (childR.bounds.center.z, negf (childR.bounds.center.x))
				|| childR.bounds.center.z < negf (childR.bounds.center.x);
		case EdgeType.DIAG_LEFT:
			MeshRenderer childL = child.GetComponent<MeshRenderer> ();
			return UnityHelper.ApproxSameFloat (childL.bounds.center.z, f (childL.bounds.center.x))
				|| childL.bounds.center.z < f (childL.bounds.center.x);
		default:
			return false;
		}
	}


	static Vector3 getFoldVector(EdgeType et) {
		switch (et) {
		case EdgeType.HORZ:
			return new Vector3 (0, 0, 1);
		case EdgeType.VERT:
			return new Vector3 (1, 0, 0);
		case EdgeType.DIAG_RIGHT:
			return new Vector3 (1, 0, -1);
		case EdgeType.DIAG_LEFT:
			return new Vector3 (1, 0, 1);
		default:
			return Vector3.zero;
		}
	}

	static void template(EdgeType et, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			break;
		case EdgeType.VERT:
			break;
		case EdgeType.DIAG_RIGHT:
			break;
		case EdgeType.DIAG_LEFT:
			break;
		default:
			break;
		}
	}
//
//	public static void RotateHorz(FourSquare square) {
//		// temporarily parent each child to a new GameObject
//
//		GameObject c_parent = new GameObject();
//		c_parent.transform.position = square.transform.position;
//
//		List<Transform> children_to_group = new List<Transform>();
//		List<Transform> children_to_fold_on = new List<Transform>();
//
//		float height_folded_on = 0, height_to_fold = 0;
//		Transform t_lowest_z = square.center;
//		Transform t_highest_z = square.center;
//
//		Transform neg_z = square.transform.Find ("V3");	// -Z
//		Transform z = square.transform.Find ("V7");		// Z
//		if (t_lowest_z.position.z > neg_z.position.z) {
//			t_lowest_z = neg_z;
//		}
//		if (t_highest_z.position.z < z.position.z) {
//			t_highest_z = z;
//		}
//
//		foreach (Transform child in square.transform) {
//			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
//			if (child.position.x < square.transform.position.x) {
//				// group children left of the center
//				children_to_group.Add (child);
//				if (child.tag.ToLower() != "vertice")
//					AssignHighest (ref height_to_fold, y_val);
//			} else {
//				children_to_fold_on.Add (child);
////				 find the highest y coordinate of the children to be folded on
////				if (child.tag.ToLower() != "vertice")
////					AssignHighest (ref height_folded_on, y_val);
//			}
//		}
//		height_folded_on = getHeightFoldedOn (children_to_group, children_to_fold_on, square, EdgeType.HORZ);
//
//		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
//		foreach (Transform t in new List<Transform>{square.center, t_lowest_z, t_highest_z}) {
//			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
//		}
//
//
//		if (!t_lowest_z.Equals(square.center)) {
//			TransformEdge te = new TransformEdge (square.center, t_lowest_z, EdgeType.HORZ);
//			square.insertEdge(te);
//		}
//		if (!t_highest_z.Equals(square.center)) {
//			TransformEdge te = new TransformEdge (square.center, t_highest_z, EdgeType.HORZ);
//			square.insertEdge(te);
//		}
//
//		ParentTo (children_to_group, c_parent);
//		DoVisualFold (c_parent.transform, new Vector3 (0, 0, 1), height_folded_on, height_to_fold);
//		ReparentFrom (ref c_parent, square);
//		square.incNumFolds ();
//	}
//
//
//	public static void RotateVert(FourSquare square) {
//		// temporarily parent each child to a new GameObject
//		GameObject c_parent = new GameObject();
//		c_parent.transform.position = square.transform.position;
//
//		List<Transform> children_to_group = new List<Transform>();
//		List<Transform> children_to_fold_on = new List<Transform>();
//		float height_folded_on = 0, height_to_fold = 0;
//		Transform t_lowest_x = square.center;
//		Transform t_highest_x = square.center;
//
//		Transform neg_x = square.transform.Find ("V5");	// -X
//		Transform x = square.transform.Find ("V1");		// X
//		if (t_lowest_x.position.x > neg_x.position.x) {
//			t_lowest_x = neg_x;
//		}
//		if (t_highest_x.position.x < x.position.x) {
//			t_highest_x = x;
//		}
//
//		foreach (Transform child in square.transform) {
//			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
//			if (child.position.z < square.transform.position.z) {
//				// group children left of the center
//				children_to_group.Add (child);
//				if (child.tag.ToLower() != "vertice")
//					AssignHighest (ref height_to_fold, y_val);
//			} else {
//				children_to_fold_on.Add (child);
//				// find the highest y coordinate of the children to be folded on
////				if (child.tag.ToLower() != "vertice")
////					AssignHighest (ref height_folded_on, y_val);
//				
//			}
//		}
//		height_folded_on = getHeightFoldedOn (children_to_group, children_to_fold_on, square, EdgeType.VERT);
//		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
//		foreach (Transform t in new List<Transform>{square.center, t_lowest_x, t_highest_x}) {
//			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
//		}
//
//
//
//		if (!t_lowest_x.Equals(square.center)) {
//			TransformEdge te = new TransformEdge (square.center, t_lowest_x, EdgeType.VERT);
//			square.insertEdge(te);
//		}
//		if (!t_highest_x.Equals(square.center)) {
//			TransformEdge te = new TransformEdge (square.center, t_highest_x, EdgeType.VERT);
//			square.insertEdge(te);
//		}
//
//		ParentTo (children_to_group, c_parent);
//		DoVisualFold (c_parent.transform, new Vector3 (1, 0, 0), height_folded_on, height_to_fold);
//		ReparentFrom (ref c_parent, square);
//		square.incNumFolds ();
//
//	}
//
//	public static void RotateDiagRight(FourSquare square) {
//		// temporarily parent each child to a new GameObject
//		GameObject c_parent = new GameObject();
//		c_parent.transform.position = square.transform.position;
//
//		List<Transform> children_to_group = new List<Transform>();
//		List<Transform> children_to_fold_on = new List<Transform>();
//		float height_folded_on = 0, height_to_fold = 0;
//		Transform top_left = square.transform.Find ("V6");		// -X  Y
//		Transform bottom_right = square.transform.Find ("V2");		//  X -Y
//
//		float distCenter =  distanceFromNegF (square.center.position);
//		float distTL = distanceFromNegF (top_left.GetComponent<MeshRenderer> ().bounds.center);
//		float distBR = distanceFromNegF (bottom_right.GetComponent<MeshRenderer> ().bounds.center);
//		if (!UnityHelper.ApproxSameFloat(distTL,distCenter)
//			&& distTL > distCenter) {
//			top_left = square.center;
//		}
//		if (!UnityHelper.ApproxSameFloat(distBR,distCenter)
//			&& distBR > distCenter) {
//			bottom_right = square.center;
//		}
//
//		foreach (Transform child in square.transform) {
//			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
//			MeshRenderer childR = child.GetComponent<MeshRenderer> ();
//			if (UnityHelper.ApproxSameFloat(childR.bounds.center.z, negf(childR.bounds.center.x)) 
//				|| childR.bounds.center.z < negf(childR.bounds.center.x)) {
//				// group children left-down of the center
//				children_to_group.Add (child);
//				if (child.tag.ToLower() != "vertice")
//					AssignHighest (ref height_to_fold, y_val);
//			} else {
//				children_to_fold_on.Add (child);
//				// find the highest y coordinate of the children to be folded on
////				if (child.tag.ToLower() != "vertice")
////					AssignHighest (ref height_folded_on, y_val);
//			}
//		}
//		height_folded_on = getHeightFoldedOn (children_to_group, children_to_fold_on, square, EdgeType.DIAG_RIGHT);
//		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
//		foreach (Transform t in new List<Transform>{square.center, top_left, bottom_right}) {
//			SetVector3Value(t, "y", (height_to_fold + height_folded_on) / 2.0f);
//		}
//			
//		if (!top_left.Equals(square.center)) {
//			TransformEdge te = new TransformEdge (square.center, top_left, EdgeType.DIAG_RIGHT);
//			square.insertEdge(te);
//		}
//		if (!bottom_right.Equals(square.center)) {
//			TransformEdge te = new TransformEdge (square.center, bottom_right, EdgeType.DIAG_RIGHT);
//			square.insertEdge(te);
//		}
//		ParentTo (children_to_group, c_parent);
//		DoVisualFold (c_parent.transform, new Vector3 (1, 0, -1), height_folded_on, height_to_fold);
//		ReparentFrom (ref c_parent, square);
//		square.incNumFolds ();
//
//	}
//


	static float getHeightFoldedOn(List<Transform> children_to_group, List<Transform> children_to_fold_on, FourSquare square, EdgeType et) {
		float height_folded_on = 0;;
		List<Transform> child_copy = children_to_fold_on;
		foreach (Transform cg in children_to_group) {
			Vector3 opp = getOppositePosition (cg, et);

			foreach (Transform cfo in children_to_fold_on) {
				float y_val = cfo.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
				// loop until found the child on the opposite side
				if (UnityHelper.V3WithinDec (opp, cfo.GetComponent<MeshRenderer>().bounds.center, -1)
					&& cfo.tag.ToLower() != "vertice") {
					AssignHighest (ref height_folded_on, y_val);
				}


			}
				 
		}
		return height_folded_on;

	}

	static Vector3 getOppositePosition(Transform t, EdgeType et) {
		Vector3 other = t.GetComponent<MeshRenderer>().bounds.center;
		if (et == EdgeType.HORZ) {
			other.x = -other.x;
		} else if (et == EdgeType.VERT) {
			other.y = -other.y;
		} else if (et == EdgeType.DIAG_RIGHT) {
			float tempx = other.x;
			other.x = -other.z;
			other.z = -tempx;
		} else if (et == EdgeType.DIAG_LEFT) {
			float tempx = other.x;
			other.x = other.z;
			other.z = tempx;
		}
		return other;

	}

	static float negf(float x) {
		return -x;
	}

	static float f(float x) {
		return x;
	}

	static float distanceFromFunction(EdgeType et, Vector3 v) {
		switch (et) {
		case EdgeType.DIAG_RIGHT:
			float negf_z = negf (v.x);
			return Math.Abs (negf_z - v.z);
		case EdgeType.DIAG_LEFT:
			float f_z = f (v.x);
			return Math.Abs (f_z - v.z);
		default:
			return 0;
		}
	}

	static float distanceFromNegF(Vector3 v) {
		float f_z = negf (v.x);
		return Math.Abs (f_z - v.z);
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
			UnityHelper.SetV3Value (e.start, "z", -1 * e.start.position.z);
//			Debug.Log (str + "\nFlipped vert\n" + e.start.position);
		}
		if (e.end.position.z < square.center.position.z) {
			string str = "Before\n" + e.end.position;
			UnityHelper.SetV3Value (e.end, "z", -1 * e.end.position.z);
//			Debug.Log (str + "Flipped vert\n" + e.end.position);
		}
	}

	static void FlipHorz(TransformEdge e, FourSquare square) {
		if (e.start.position.x < square.center.position.x) {
			string str = "Before\n" + e.start.position;
			UnityHelper.SetV3Value (e.start, "x", -1 * e.start.position.x);
//			Debug.Log (str + "\nFlipped horz\n" + e.start.position);
		}
		if (e.end.position.x < square.center.position.x) {
			string str = "Before\n" + e.end.position;
			UnityHelper.SetV3Value (e.start, "x", -1 * e.start.position.x);
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


}
