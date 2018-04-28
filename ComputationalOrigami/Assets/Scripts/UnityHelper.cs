using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class containing general helper methods.
 */
public class UnityHelper : MonoBehaviour {
	public static System.Random rand = new System.Random ();

	public static bool Encapsulates (Bounds b1, Bounds b2) {
		Vector3 b1Extents = b1.center + b1.extents;
		Vector3 b2Extents = b2.center + b2.extents;
		print (b1Extents + "\n" + b2Extents);
		return b1Extents.x > b2Extents.x
		&& b1Extents.y > b2Extents.y
		&& b1Extents.z > b2Extents.z;
	}

	/*
	 * Shuffle a list containint objects of type T
	 * source : https://stackoverflow.com/questions/273313/randomize-a-listt
	 */
	public static void Shuffle<T>(ref List<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rand.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	/*
	 * Checks whether an origami unit can fit in a pocket p given its insertion vertex
	 */
	public static bool CanFitPocket(OrigamiUnit unit, Pocket p)
	{
		Transform iv = unit.GetIV ();
		Transform ivn1 = unit.GetIVNeighbour1 ();
		Transform ivn2 = unit.GetIVNeighbour2 ();
		float angleU = Vector3.Angle (
			ivn1.position - iv.position,
			ivn2.position - iv.position);
		float angleP = p.angle;
		return angleU < angleP || ApproxSameFloat (angleP, angleU);
	}

	/*
	 * Checks whether the pocket p matches the criteria for an origami unit
	 * with a specified IV.
	 */
	public static bool CorrectTargetPocket(OrigamiUnit unit, Pocket p) {
		// end1			end of pocket edge 1
		// end2			end of pocket edge 2
		// fartherEnd	the edge of the pocket that is farther away from the center
		//				along the y axis
		Vector3 end1, end2, fartherEnd;
		switch (unit.numFolds) {
		case 0:
		case 1:
			return true;

		// distance between the center vertex and one endpoint of the pocket
		// must be within the EDGE_POCKET_DISTANCE
		case 2:
			end1 = p.edge1.end.localPosition - p.pCenter.localPosition;
			end2 = p.edge2.end.localPosition - p.pCenter.localPosition;
			fartherEnd = (Math.Abs (end1.y) > Math.Abs (end2.y)) ? end1 : end2;
			return Math.Abs(fartherEnd.y) < Pocket.EDGE_POCKET_DISTANCE;

		// distance between the center vertex and one endpoint of the pocket
		// must be greater than the EDGE_POCKET_DISTANCE
		case 3:
		case 4:
			end1 = p.edge1.end.localPosition - p.pCenter.localPosition;
			end2 = p.edge2.end.localPosition - p.pCenter.localPosition;
			fartherEnd = (Math.Abs (end1.y) > Math.Abs (end2.y)) ? end1 : end2;
			return Math.Abs(fartherEnd.y) > Pocket.EDGE_POCKET_DISTANCE;
			
		default:
			return true;
		}

	}

	/*
	 * Checks whether the origami unit has the correct alignment before inserting itself
	 * into the target pocket.
	 */
	public static bool CorrectOverlap(OrigamiUnit unit) {
		switch (unit.numFolds) {
		case 0:
		case 1:
			return true;
		case 2:
			return true;
		case 3:
		case 4:
			// most of the unit's volume will be on one side of the IV
			// the majority of the volume needs to not overlap with the target unit
			Plane pl = new Plane(unit.GetIV().position, unit.targetP.edge1.end.position, unit.targetP.edge2.end.position);
			Transform targetParent = unit.targetP.pCenter.parent;
			int numPos = 0;
			int numNeg = 0;
			for (int i = 0; i < targetParent.childCount; i++) {
				if (pl.GetSide (targetParent.GetChild (i).position)) {
					numPos++;
				} else {
					numNeg++;
				}
			}
			for (int i = 0; i < unit.transform.childCount; i++) {
				if (pl.GetSide (unit.transform.GetChild (i).position)) {
					numPos++;
				} else {
					numNeg++;
				}
			}
			return (Math.Abs(numNeg-numPos) < unit.transform.childCount/2);
		default:
			return true;
		}

	}


	/*
	 * Checks whether the insertion of a new edge cuts into an already existant pocket.
	 * Each edge is represented as a line with a specified start and end point.
	 * Intersection occurs if these two lines cross.
	 */
	public static bool IntersectsPocket(TransformEdge e, Pocket p, EdgeType et)
	{
		Vector3 s1 = e.start.position;
		Vector3 e1 = e.end.position;
		Vector3 s2 = p.edge1.end.position;
		Vector3 e2 = p.edge2.end.position;
		float m1 = GetSlope(s1.x, s1.z, e1.x, e1.z);
		float m2 = GetSlope(s2.x, s2.z, e2.x, e2.z);

		if (ApproxSameFloat(m1, m2)) {
			return false;
		}
		float intx, intz;
		if (Double.IsInfinity(m1)) {
			float b2 = GetB(e2.x, e2.z, m2);
			intx = e1.x;
			intz = (m2 * intx) + b2;
		}
		else if (Double.IsInfinity(m2)) {
			float b1 = GetB(e1.x, e1.z, m1);
			intx = e2.x;
			intz = (m1 * intx) + b1;
		}
		else {
			float b1 = GetB(e1.x, e1.z, m1);
			float b2 = GetB(e2.x, e2.z, m2);
			intx = (b2 - b1) / (m1 - m2);
			intz = (m1 * intx) + b1;
		}
		// if not the same as the ends of the pocket
		if (!ApproxSameFloat(intx, s2.x)
			&& !ApproxSameFloat(intx, e2.x)
			&& InBetweenExcl(s2.x, e2.x, intx)) {
			return true;
		}
		else {
			return false;
		}

	}

	/*
	 * Get the slope of a line, given two points on the line.
	 */
	public static float GetSlope(float x1, float z1, float x2, float z2) {
		return (z1 - z2) / (x1 - x2);
	}

	/*
	 * Get the constant b in z = mx + b, where m is the slope
	 */
	public static float GetB (float x, float z, float m) {
		return z - (m * x);
	}

	/*
	 * Find the distance between two points
	 */
	public static float PointDistance(float x1, float z1, float x2, float z2) {
		return (float) Math.Sqrt (Math.Pow(x1-x2,2) + Math.Pow(z1-z2,2));
	}

	/*
	 * Checks whether a float 'vToCheck' is exclusively in between floats v1 and v2
	 */
	public static bool InBetweenExcl(float v1, float v2, float vToCheck) {
		float left, right;
		left = Math.Min (v1, v2);
		right = Math.Max (v1, v2);
		return vToCheck > left && vToCheck < right;
	}


	/*
	 * Compute the plane formed by a given vertex of an origami unit and the child triangle
	 * the vertex is embedded in
	 */
	public static Plane GetPlaneOfVertex(OrigamiUnit unit, Transform vertex) {
		Collider[] overlaps = Physics.OverlapSphere (vertex.position, OrigamiUnit.PAPER_THICKNESS / 4);
		Transform overlappingTri = null;
		foreach (Collider col in overlaps) {
			if (col.name.Contains ("Tri"))
				overlappingTri = col.transform;
		}
		Vector3 v1 = vertex.position;
		Vector3 v2 = overlappingTri.Find ("Core").position;
		Vector3 v3 = overlappingTri.position;
		return new Plane (v1, v2, v3);


	}

	/*
	 * Randomly position the origami unit within the zone.
	 */
    public static void RandomlyPosition(Transform orig, Transform zone) {
		Vector3 zoneSize = zone.GetComponent<MeshRenderer> ().bounds.size;
		float xRange = zoneSize.x;
		float yRange = 5;
		float zRange = zoneSize.z;
		do {
			float newX = (float) rand.NextDouble() * xRange + 3;
			int neg = rand.NextDouble() > 0.5f? -1 : 1;
			SetPositionValue(orig,"x",neg*newX);
		} while (orig.position.x >= xRange / 2 || orig.position.x <= -xRange / 2);

		SetPositionValue(orig,"y", (float) rand.NextDouble() * yRange + 2);

		do {
			float newZ = (float) rand.NextDouble() * zRange + 3;
			int neg = rand.NextDouble() > 0.5f? -1 : 1;
			SetPositionValue(orig,"z",neg*newZ);
		} while (orig.position.z >= zRange / 2 || orig.position.z <= -zRange / 2);
	}

	/*
	 * Randomly rotate the given Transform
	 */
	public static void RandomlyRotate(Transform orig) {
		GetNewRotation (orig, "x", (float) rand.NextDouble () * 360);	
		GetNewRotation (orig, "y", (float) rand.NextDouble () * 360);	
		GetNewRotation (orig, "z", (float) rand.NextDouble () * 360);	
	}

	/*
	 * Change a rotation field of a transform given the new rotation value
	 */
	public static void GetNewRotation(Transform orig, string field, float value) {
		Vector3 rotationVector = orig.rotation.eulerAngles;
		if (field == "x") {
			rotationVector.x = value;
		} else if (field == "y") {
			rotationVector.y = value;
		} else if (field == "z") {
			rotationVector.z = value;
		}

		orig.rotation = Quaternion.Euler(rotationVector);

	}


	/*
	 * Change a value of the given Transform's position to the new value 
	 */
	public static void SetPositionValue(Transform orig, string field, float value) {
		Vector3 vect = orig.position;
		if (field.ToLower().Equals ("x"))
			vect.x = value;
		else if (field.ToLower().Equals ("y"))
			vect.y = value;
		else if (field.ToLower().Equals ("z"))
			vect.z = value;
		orig.position = vect;
	}

	/*
	 * Check if two vectors are approximately equal by comparing their magnitudes
	 */
	public static bool V3EqualMagn(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.00001f;
	}

	/*
	 * Check if two vectors are close to each other in magnitude
	 */
	public static bool V3ApproxEqualMagn(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.005f;
	}

	/*
	 * Check if each field of two vectors is the same, within 3 decimal places
	 */
	public static bool V3EqualFields(Vector3 a, Vector3 b) {
		return V3WithinDec (a, b, -3);
	}

	/*
	 * Get the vector facing the opposite direction of a given vector
	 */
	public static Vector3 GetOppositeV3(Vector3 v) {
		Vector3 oppV3 = new Vector3 ();
		oppV3.x = -v.x;
		oppV3.y = -v.y;
		oppV3.z = -v.z;
		return oppV3;
	}

	/*
	 * Check that two planes are approximately equal using their normal vectors
	 */
	public static bool ApproxEqualPlane(Plane a, Plane b) {
		return (V3ApproxEqualMagn (a.normal, b.normal) 
			|| V3ApproxEqualMagn (GetOppositeV3 (a.normal), b.normal));
	}
		
	/*
	 * Check whether two floats are approximately the same
	 */
	public static bool ApproxSameFloat(float a, float b) {
		return Math.Abs(a - b) < 0.0001f;
	}
		
	/*
	 * Check that each field of two vectors are within 'dec' decimal places of each other
	 */
	public static bool V3WithinDec(Vector3 source, Vector3 target, float dec) {
		return 
			FloatWithinDec (source.x, target.x, dec)
			&& FloatWithinDec (source.y, target.y, dec)
			&& FloatWithinDec (source.z, target.z, dec);
	}

	/*
	 * Check that two float values are within 'dec' decimal places of each other
	 */
	public static bool FloatWithinDec(float a, float b, float dec) {
		return Math.Abs(a - b) < Math.Pow (10, dec);
	}

	/*
	 * For debugging purposes.
	 * Return a string with more specific values for each field of a vector.
	 */
	public static String LogV3(Vector3 v) {
		string output = "(";
		output += v.x + ",";
		output += v.y + ",";
		output += v.z + ",";
		output += ")";
		return output;
	}

}