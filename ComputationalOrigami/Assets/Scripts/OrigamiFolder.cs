using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrigamiFolder : MonoBehaviour{

	public static void RandomlyFold(FourSquare square,
		float probHorzFold,
		float probVertFold,
		float probDiagRightFold,
		float probDiagLeftFold) {

		//TODO: Randomize order of folds


		EdgeType[] edgeTypes = (EdgeType[]) Enum.GetValues(typeof(EdgeType));
		UnityHelper.ShuffleEdgeTypes (ref edgeTypes);
		int i = 0;
		do {
			EdgeType et = edgeTypes[i];
			switch (et) {
			case EdgeType.HORZ:
				if (UnityHelper.rand.NextDouble () < probHorzFold)
					FoldSquare (EdgeType.HORZ, square);
				break;
			case EdgeType.VERT:
				if (UnityHelper.rand.NextDouble () < probVertFold)
					FoldSquare (EdgeType.VERT, square);
				break;
			case EdgeType.DIAG_LEFT:
				if (UnityHelper.rand.NextDouble () < probDiagLeftFold)
					FoldSquare (EdgeType.DIAG_LEFT, square);
				break;
			case EdgeType.DIAG_RIGHT:
				if (UnityHelper.rand.NextDouble () < probDiagRightFold)
					FoldSquare (EdgeType.DIAG_RIGHT, square);
				break;
			default:
				break;
			}
			i++;
		} while (i < edgeTypes.Length);


//
//
//		if (UnityHelper.rand.NextDouble () < probHorzFold)
//			FoldSquare (EdgeType.HORZ, square);
//		if (UnityHelper.rand.NextDouble () < probVertFold)
//			FoldSquare (EdgeType.VERT, square);
//		if (UnityHelper.rand.NextDouble () < probDiagRightFold)
//			FoldSquare (EdgeType.DIAG_RIGHT, square);
//		if (UnityHelper.rand.NextDouble () < probDiagLeftFold)
//			FoldSquare (EdgeType.DIAG_LEFT, square);
//		
//		var edge_enums = Enum.GetValues (typeof(EdgeType));
//
//		int numFolds = UnityHelper.rand.Next (minFolds, edge_enums.Length);
//
//		// get a random set of folds
//		HashSet<int> set = new HashSet<int> ();
//		for (int i = 0; i < numFolds; i++) {
//			set.Add (UnityHelper.rand.Next (0, edge_enums.Length-1));
//		}
//
//		foreach (int i in set) {
//			EdgeType et = (EdgeType) Enum.ToObject(typeof(EdgeType), edge_enums.GetValue(i));
//			Debug.Log ("folding as " + et.ToString ());
//			FoldSquare (et, square);
//		}

	}


	public static void FoldSquare(EdgeType et, FourSquare square) {
		// temporarily parent each child to a new GameObject

		GameObject c_parent = new GameObject();
		c_parent.transform.position = square.transform.position;

		List<Transform> children_to_group = new List<Transform>();
		List<Transform> children_to_fold_on = new List<Transform>();

		float height_folded_on = 0, height_to_fold = 0;
		Transform lowBound = GetLowBound (et, square);
		Transform highBound = GetHighBound (et, square);

		// get the vertice of the lower and higher bound of the fold line
		if (ShouldUpdateLB(et,lowBound,square)) {
			lowBound = square.center;
		}
		if (ShouldUpdateHB(et,highBound,square)) {
			highBound = square.center;
		}
		// group the triangle of the square unit to be folded on, and to fold over
		foreach (Transform child in square.transform) {
			float y_val = child.GetComponent<MeshRenderer>().bounds.center.y - square.transform.position.y;
			if (ShouldGroupChild(et,child,square)) {
				// group children on folding side
				children_to_group.Add (child);
			} else {
				// group children on folded on side
				children_to_fold_on.Add (child);
			}
		}

		// if there's no child on the side to fold, skip doing any fold
		if (children_to_group.Count == 0 
			|| (children_to_group.Count == 1 
				&& children_to_group[0].Equals(square.center)))
			return;

		// get the height of the unit to translate along the y-axis 
		Tuple<float,float> heights = GetHeightFoldedOn (children_to_group, children_to_fold_on, square, et);
		height_folded_on = heights.first;
		height_to_fold = heights.second;

		AddPaperThickness (ref height_to_fold, ref height_folded_on, FourSquare.PAPER_THICKNESS);
		foreach (Transform t in new List<Transform>{square.center, lowBound, highBound}) {
			UnityHelper.SetV3Value (t, "y", (height_to_fold + height_folded_on) / 2.0f);
		}

		// insert an edge from the center to the upper/lower bound only if the bounding vertice was not the center
		if (!lowBound.Equals(square.center)) {
			TransformEdge te = new TransformEdge (square.center, lowBound, et);
			square.InsertEdge(te);
		}
		if (!highBound.Equals(square.center)) {
			TransformEdge te = new TransformEdge (square.center, highBound, et);
			square.InsertEdge(te);
		}

		// remove the temporary parent and reparent to original object
		ParentTo (children_to_group, c_parent);
		DoVisualFold (c_parent.transform, GetFoldVector(et), height_folded_on, height_to_fold);
		ReparentFrom (ref c_parent, square);
		Destroy (c_parent);
	}

	static Transform GetLowBound(EdgeType et, FourSquare square) {
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

	static Transform GetHighBound(EdgeType et, FourSquare square) {
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

	static bool ShouldUpdateLB(EdgeType et, Transform bound, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return bound.position.z > square.center.position.z;
		case EdgeType.VERT:
			return bound.position.x > square.center.position.x;
		case EdgeType.DIAG_RIGHT:
		case EdgeType.DIAG_LEFT:			
			float distCenter =  DistanceFromFunction (et, square.center.position);
			float distB = DistanceFromFunction (et, bound.GetComponent<MeshRenderer> ().bounds.center);
			return !UnityHelper.ApproxSameFloat(distB,distCenter);
		default:
			return false;
		}
	}

	static bool ShouldUpdateHB(EdgeType et, Transform bound, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return bound.position.z < square.center.position.z;
		case EdgeType.VERT:
			return bound.position.x < square.center.position.x;
		case EdgeType.DIAG_RIGHT:
		case EdgeType.DIAG_LEFT:			
			float distCenter =  DistanceFromFunction (et, square.center.position);
			float distB = DistanceFromFunction (et, bound.GetComponent<MeshRenderer> ().bounds.center);
			return !UnityHelper.ApproxSameFloat(distB,distCenter);
		default:
			return false;
		}
	}

	static bool ShouldGroupChild(EdgeType et, Transform child, FourSquare square) {
		switch (et) {
		case EdgeType.HORZ:
			return child.position.x < square.transform.position.x;
		case EdgeType.VERT:
			return child.position.z < square.transform.position.z;
		case EdgeType.DIAG_RIGHT:
			MeshRenderer childR = child.GetComponent<MeshRenderer> ();
			return UnityHelper.ApproxSameFloat (childR.bounds.center.z, Negf (childR.bounds.center.x))
				|| childR.bounds.center.z < Negf (childR.bounds.center.x);
		case EdgeType.DIAG_LEFT:
			MeshRenderer childL = child.GetComponent<MeshRenderer> ();
			return UnityHelper.ApproxSameFloat (childL.bounds.center.z, f (childL.bounds.center.x))
				|| childL.bounds.center.z < f (childL.bounds.center.x);
		default:
			return false;
		}
	}


	static Vector3 GetFoldVector(EdgeType et) {
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
    

	static Tuple<float,float> GetHeightFoldedOn(List<Transform> children_to_group, List<Transform> children_to_fold_on, FourSquare square, EdgeType et) {
		float height_folded_on = 0;
		float height_to_fold = 0;
		List<Transform> child_copy = children_to_fold_on;
//		Debug.Log ("Getting heights for " + et.ToString ());
		// for each child to group, find 
		foreach (Transform cg in children_to_group) {
			if (cg.tag.ToLower () != "vertice") {
				Vector3 opp = GetOppositePosition (cg, et);

				// loop and find triangle children that would be folded on
				foreach (Transform cfo in children_to_fold_on) {
					float cfo_y_val = cfo.GetComponent<MeshRenderer> ().bounds.center.y - square.transform.position.y;

//					if (UnityHelper.V3WithinDec (opp, cfo.GetComponent<MeshRenderer> ().bounds.center, -1)
					if (cfo.tag.ToLower () != "vertice" && UnityHelper.V3WithinDec (opp, cfo.Find ("Core").position, -1)) {
						// assign highest height for pieces to fold on and pieces to fold
						float cg_y_val = cg.GetComponent<MeshRenderer> ().bounds.center.y - square.transform.position.y;
						AssignHighest (ref height_folded_on, cfo_y_val);
						AssignHighest (ref height_to_fold, cg_y_val);
//						Debug.Log (et + "\n" + cg.name + " " + cg.Find ("Core").position + "\n" +
//						cfo.name + " " + cfo.Find ("Core").position);
//
//					} else if (cfo.tag.ToLower () != "vertice") {
//						Debug.Log (et + "\n" + cg.name + " " + cg.Find("Core").position + "\n" +
//							cfo.name + " " + cfo.Find("Core").position);
					}



				}
			}
				 
		}
		return new Tuple<float,float>(height_folded_on,height_to_fold);

	}

	static Vector3 GetOppositePosition(Transform t, EdgeType et) {
		Vector3 other = t.Find ("Core").position; //.GetComponent<MeshRenderer>().bounds.center;
		if (et == EdgeType.HORZ) {
			other.x = -other.x;
		} else if (et == EdgeType.VERT) {
			other.z = -other.z;
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

	static float Negf(float x) {
		return -x;
	}

	static float f(float x) {
		return x;
	}

	static float DistanceFromFunction(EdgeType et, Vector3 v) {
		switch (et) {
		case EdgeType.DIAG_RIGHT:
			float negf_z = Negf (v.x);
			return Math.Abs (negf_z - v.z);
		case EdgeType.DIAG_LEFT:
			float f_z = f (v.x);
			return Math.Abs (f_z - v.z);
		default:
			return 0;
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
		
	}

	static void DoVisualFold(Transform c_parent_transform, Vector3 rotate_vector, 
		float height_folded_on, float height_to_fold) {
		float topmost_y = height_folded_on + height_to_fold;
//		Debug.Log ("topmost y: " + topmost_y);
//		Debug.Log ("transform position " + c_parent_transform.position);
		c_parent_transform.Rotate(rotate_vector,180);
		c_parent_transform.Translate(new Vector3(0,-topmost_y,0));
	}
    


}
