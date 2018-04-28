using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Performs folds on an OrigamiUnit in 4 different ways, in the order of: 
 * 		Horizontal Fold
 * 		Vertical Fold
 * 		Right Diagonal Fold
 * 		Left Diagonal Fold
 */
public class OrigamiFolder : MonoBehaviour{
	// list of temporary parent objects to be cleaned up after performing a fold
	List<GameObject> tempParents;

	void Start() {
		tempParents = new List<GameObject> ();
	}

	/*
	 * Given a list of probabilities of folds, execute the fold if it is chosen in the
	 * order of
	 * 		Horizontal Fold
	 * 		Vertical Fold
	 * 		Right Diagonal Fold
	 * 		Left Diagonal Fold
	 * In this order, there are 2^4 = 16 choices but only 5 distinct structures
	 */
	public void RandomlyFold(OrigamiUnit unit,
		float probHorzFold,
		float probVertFold,
		float probDiagRightFold,
		float probDiagLeftFold) {

		if (UnityHelper.rand.NextDouble () < probHorzFold)
			FoldSquare (EdgeType.HORZ, unit);
		if (UnityHelper.rand.NextDouble () < probVertFold)
			FoldSquare (EdgeType.VERT, unit);
		if (UnityHelper.rand.NextDouble () < probDiagRightFold)
			FoldSquare (EdgeType.DIAG_RIGHT, unit);
		if (UnityHelper.rand.NextDouble () < probDiagLeftFold)
			FoldSquare (EdgeType.DIAG_LEFT, unit);

	}

	/*
	 * Execute a fold given the edge the origami unit will be folded along
	 */
	public void FoldSquare(EdgeType et, OrigamiUnit unit) {
		// temporarily parent children which will be folded to a new GameObject
		GameObject tempParent = new GameObject();
		tempParent.name = "parent";
		tempParents.Add (tempParent);

		tempParent.transform.position = unit.transform.position;

		List<Transform> children_to_group = new List<Transform>();
		List<Transform> children_to_fold_on = new List<Transform>();

		float  heightFoldedOn = 0,  heightToFold = 0;
		Transform lowBound = GetLowBound (et, unit);
		Transform highBound = GetHighBound (et, unit);

		// get the vertice of the lower and higher bound of the fold line
		if (ShouldUpdateLB(et,lowBound,unit)) {
			lowBound = unit.center;
		}
		if (ShouldUpdateHB(et,highBound,unit)) {
			highBound = unit.center;
		}
		// group the triangle of the square unit to be folded on, and to fold over
		foreach (Transform child in unit.transform) {
			if (ShouldGroupChild(et,child,unit)) {
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
				&& children_to_group[0].Equals(unit.center)))
			return;

		// get the height of the unit to translate along the y-axis 
		Tuple<float,float> heights = GetHeightFoldedOn (children_to_group, children_to_fold_on, unit, et);
		 heightFoldedOn = heights.first;
		 heightToFold = heights.second;

		AddPaperThickness (ref  heightToFold, ref  heightFoldedOn, OrigamiUnit.PAPER_THICKNESS);
		foreach (Transform t in new List<Transform>{unit.center, lowBound, highBound}) {
			UnityHelper.SetPositionValue (t, "y", ( heightToFold +  heightFoldedOn) / 2.0f);
		}

		// insert an edge from the center to the upper/lower bound only if the bounding vertice was not the center
		if (!lowBound.Equals(unit.center)) {
			TransformEdge te = new TransformEdge (unit.center, lowBound, et);
			unit.InsertEdge(te);
		}
		if (!highBound.Equals(unit.center)) {
			TransformEdge te = new TransformEdge (unit.center, highBound, et);
			unit.InsertEdge(te);
		}

		// remove the temporary parent and reparent to original object
		ParentTo (children_to_group, tempParent);
		DoVisualFold (tempParent.transform, GetFoldVector(et),  heightFoldedOn,  heightToFold);
		ReparentFrom (ref tempParent, unit);
		GameObject.DestroyImmediate (tempParent);

		unit.AddFold ();
	}

	/*
	 * Get the default lower bound (vertex) of the edge
	 */
	private Transform GetLowBound(EdgeType et, OrigamiUnit unit) {
		switch (et) {
		case EdgeType.HORZ:
			return unit.transform.Find ("V3");
		case EdgeType.VERT:
			return unit.transform.Find ("V5");
		case EdgeType.DIAG_RIGHT:
			return unit.transform.Find ("V6");
		case EdgeType.DIAG_LEFT:
			return unit.transform.Find ("V0");
		default:
			return null;
		}
	}

	/*
	 * Get the default higher bound (vertex) of the edge
	 */
	private Transform GetHighBound(EdgeType et, OrigamiUnit unit) {
		switch (et) {
		case EdgeType.HORZ:
			return unit.transform.Find ("V7");
		case EdgeType.VERT:
			return unit.transform.Find ("V1");
		case EdgeType.DIAG_RIGHT:
			return unit.transform.Find ("V2");
		case EdgeType.DIAG_LEFT:
			return unit.transform.Find ("V4");
		default:
			return null;
		}
	}

	/*
	 * Check if the current lower bound is on the other side of the unit's center
	 * if true, the lower bound should be updated to a different vertex
	 */
	private bool ShouldUpdateLB(EdgeType et, Transform bound, OrigamiUnit unit) {
		switch (et) {
		case EdgeType.HORZ:
			return bound.position.z > unit.center.position.z;
		case EdgeType.VERT:
			return bound.position.x > unit.center.position.x;
		case EdgeType.DIAG_RIGHT:
		case EdgeType.DIAG_LEFT:			
			float distCenter =  DistanceFromFunction (et, unit.center.position);
			float distB = DistanceFromFunction (et, bound.GetComponent<MeshRenderer> ().bounds.center);
			return !UnityHelper.ApproxSameFloat(distB,distCenter);
		default:
			return false;
		}
	}

	/*
	 * Check if the current higher bound is on the other side of the unit's center
	 * if true, the higher bound should be updated to a different vertex
	 */
	private bool ShouldUpdateHB(EdgeType et, Transform bound, OrigamiUnit unit) {
		switch (et) {
		case EdgeType.HORZ:
			return bound.position.z < unit.center.position.z;
		case EdgeType.VERT:
			return bound.position.x < unit.center.position.x;
		case EdgeType.DIAG_RIGHT:
		case EdgeType.DIAG_LEFT:			
			float distCenter =  DistanceFromFunction (et, unit.center.position);
			float distB = DistanceFromFunction (et, bound.GetComponent<MeshRenderer> ().bounds.center);
			return !UnityHelper.ApproxSameFloat(distB,distCenter);
		default:
			return false;
		}
	}

	/*
	 * Check if the child's position is on the side of the unit to be folded, given the edge type
	 * if true, the child should be grouped
	 */
	bool ShouldGroupChild(EdgeType et, Transform child, OrigamiUnit unit) {
		switch (et) {
		case EdgeType.HORZ:
			return child.position.x < unit.transform.position.x;
		case EdgeType.VERT:
			return child.position.z < unit.transform.position.z;
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

	/*
	 * Get the vector to fold along given the edge type
	 */
	private Vector3 GetFoldVector(EdgeType et) {
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
    
	/*
	 * Get the height of the children to fold on and the children to fold
	 */
	private Tuple<float,float> GetHeightFoldedOn(List<Transform> children_to_group, List<Transform> children_to_fold_on, OrigamiUnit unit, EdgeType et) {
		float  heightFoldedOn = 0;
		float  heightToFold = 0;
		// for each child to group, find the height to fold on
		foreach (Transform cg in children_to_group) {
			if (cg.tag.ToLower () != "vertice") {
				Vector3 opp = GetOppositePosition (cg, et);

				// loop and find triangle children that would be folded on
				foreach (Transform cfo in children_to_fold_on) {
					float cfo_y_val = cfo.GetComponent<MeshRenderer> ().bounds.center.y - unit.transform.position.y;
					if (cfo.tag.ToLower () != "vertice" && UnityHelper.V3WithinDec (opp, cfo.Find ("Core").position, -1)) {
						// assign highest height for pieces to fold on and pieces to fold
						float cg_y_val = cg.GetComponent<MeshRenderer> ().bounds.center.y - unit.transform.position.y;
						AssignHighest (ref  heightFoldedOn, cfo_y_val);
						AssignHighest (ref  heightToFold, cg_y_val);
					}



				}
			}
				 
		}
		return new Tuple<float,float>( heightFoldedOn, heightToFold);

	}

	/*
	 * Get the opposite position of the Transform relative to the edge type
	 */
	private Vector3 GetOppositePosition(Transform t, EdgeType et) {
		Vector3 other = t.Find ("Core").position; 
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

	/*
	 * Get the point rests on the right diagonal edge, given the x coordinate
	 */
	private float Negf(float x) {
		return -x;
	}

	/*
	 * Get the point rests on the left diagonal edge, given the x coordinate
	 */
	private float f(float x) {
		return x;
	}

	/*
	 * Distance of a point (a Vector3 in this case) from an edge
	 */
	private float DistanceFromFunction(EdgeType et, Vector3 v) {
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

	/*
	 * Assign a new value to the referenced float only if it is higher
	 */
	private void AssignHighest(ref float curr, float value)	{
		if (value > curr) {
			curr = value;
		}
	}

	/*
	 * Add the PAPER_THICKNESS to the height to fold and height to fold on
	 */
	private void AddPaperThickness(ref float  heightToFold, ref float  heightFoldedOn, float PAPER_THICKNESS) {
		 heightToFold += PAPER_THICKNESS / 2;        
		 heightFoldedOn += PAPER_THICKNESS / 2;
	}

	/*
	 * Parent all children in list to the temporary parent
	 */
	private void ParentTo(List<Transform> children_to_group, GameObject tempParent) {
		foreach (Transform child in children_to_group)
			child.parent = tempParent.transform;
	}

	/*
	 * Parent all children of a temporary parent to a given Transform
	 */
	private void ReparentFrom(ref GameObject tempParent, OrigamiUnit unit) {
		while (tempParent.transform.childCount > 0)
			tempParent.transform.GetChild(0).parent = unit.transform;
		
	}

	/*
	 * Perform a fold visually using a temporary parent, without the animation 
	 */
	private void DoVisualFold(Transform tempParentTransform, Vector3 rotate_vector, 
		float  heightFoldedOn, float  heightToFold) {
		float topmost_y =  heightFoldedOn +  heightToFold;
		tempParentTransform.Rotate(rotate_vector,180);
		tempParentTransform.Translate(new Vector3(0,-topmost_y,0));
	}
    
	void Update() {
		tempParents.ForEach (p => GameObject.Destroy (p));
	}

}
