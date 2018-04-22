using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityHelper : MonoBehaviour {
	public static System.Random rand = new System.Random ();
	public static float EDGE_POCKET_DISTANCE = 0.06f;

	public static bool CanFitPocket(FourSquare unit, Pocket p)
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

	public static bool CorrectTargetPocket(FourSquare unit, Pocket p) {
		// end1			end of pocket edge 1
		// end2			end of pocket edge 2
		// fartherEnd	the edge of the pocket that is farther away from the center
		//				along the y axis
		Vector3 end1, end2, fartherEnd;
		switch (unit.numFolds) {
		case 0:
		case 1:
			return true;

		case 2:
			end1 = p.edge1.end.localPosition - p.pCenter.localPosition;
			end2 = p.edge2.end.localPosition - p.pCenter.localPosition;
			fartherEnd = (Math.Abs (end1.y) > Math.Abs (end2.y)) ? end1 : end2;
//			print (unit.name + " pocket distance " + fartherEnd.y);
			return Math.Abs(fartherEnd.y) < EDGE_POCKET_DISTANCE;

		case 3:
		case 4:
			end1 = p.edge1.end.localPosition - p.pCenter.localPosition;
			end2 = p.edge2.end.localPosition - p.pCenter.localPosition;
			fartherEnd = (Math.Abs (end1.y) > Math.Abs (end2.y)) ? end1 : end2;
			print (unit.name + " pocket distance " + fartherEnd.y);
			return Math.Abs(fartherEnd.y) > EDGE_POCKET_DISTANCE;
			
		default:
			return true;
		}

	}

	public static bool CorrectOverlap(FourSquare unit) {
		switch (unit.numFolds) {
		case 0:
		case 1:
			return true;
		case 2:
			return true;
		case 3:
		case 4:
			Plane pl = new Plane (unit.GetIV ().position, unit.targetP.edge1.end.position, unit.targetP.edge2.end.position);
			Transform targetParent = unit.targetP.pCenter.parent;
			int numTargetPos = 0;
			int numUnitPos = 0;
			for (int i = 0; i < targetParent.childCount; i++) {
				if (pl.GetSide (targetParent.GetChild (i).position)) {
					numTargetPos++;
					targetParent.GetChild (i).GetComponent<Renderer> ().material.color = Color.blue;
				}
			}
			Debug.Log (unit.name + "  numTargetPos: " + numTargetPos);
			Color rand_color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
			for (int i = 0; i < unit.transform.childCount; i++) {
				if (pl.GetSide (unit.transform.GetChild (i).position)) {
					unit.transform.GetChild (i).GetComponent<Renderer> ().material.color = rand_color;
					numUnitPos++;
				}
			}
			Debug.Log (unit.name + "  numUnitPos: " + numUnitPos);

			float targetPosRatio = numTargetPos / (float)targetParent.childCount;
			float unitPosRatio = numUnitPos / (float)unit.transform.childCount;
			return (
			    (GreaterEqualFloat (targetPosRatio, 0.5f) && (unitPosRatio < 0.5f)) ||
			    (GreaterEqualFloat (unitPosRatio, 0.5f) && (targetPosRatio < 0.5f))
			);
		default:
			return true;
		}

	}

    public static bool IntersectsPocket(TransformEdge e, Pocket p, EdgeType et)
    {
        Vector3 s1 = e.start.position;
        Vector3 e1 = e.end.position;
        Vector3 s2 = p.edge1.end.position;
        Vector3 e2 = p.edge2.end.position;
        float m1 = GetSlope(s1.x, s1.z, e1.x, e1.z);
        float m2 = GetSlope(s2.x, s2.z, e2.x, e2.z);

        if (ApproxSameFloat(m1, m2)) {
            //			print ("m1 " + m1 + "  m2 " + m2);
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

    public static void RandomlyPosition(Transform orig, Transform zone) {
		Vector3 zoneSize = zone.GetComponent<MeshRenderer> ().bounds.size;
		float xRange = zoneSize.x;
		float yRange = 5;
		float zRange = zoneSize.z;
		do {
			float newX = (float) rand.NextDouble() * xRange + 3;
			int neg = rand.NextDouble() > 0.5f? -1 : 1;
			SetV3Value(orig,"x",neg*newX);
		} while (orig.position.x >= xRange / 2 || orig.position.x <= -xRange / 2);

		SetV3Value(orig,"y", (float) rand.NextDouble() * yRange + 2);

		do {
			float newZ = (float) rand.NextDouble() * zRange + 3;
			int neg = rand.NextDouble() > 0.5f? -1 : 1;
			SetV3Value(orig,"z",neg*newZ);
		} while (orig.position.z >= zRange / 2 || orig.position.z <= -zRange / 2);
	}

	public static void RandomlyRotate(Transform orig) {
		GetNewRotation (orig, "x", (float) rand.NextDouble () * 360);	
		GetNewRotation (orig, "y", (float) rand.NextDouble () * 360);	
		GetNewRotation (orig, "z", (float) rand.NextDouble () * 360);	
	}

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



	public static void SetV3Value(Transform orig, string field, float value) {
		Vector3 vect = orig.position;
		if (field.ToLower().Equals ("x"))
			vect.x = value;
		else if (field.ToLower().Equals ("y"))
			vect.y = value;
		else if (field.ToLower().Equals ("z"))
			vect.z = value;
		orig.position = vect;
	}

	public static bool V3Equal(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.000007f;
	}

	public static bool V3ApproxEqual(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.00025f;
	}

	public static Vector3 GetOppositeV3(Vector3 v) {
		Vector3 oppV3 = new Vector3 ();
		oppV3.x = -v.x;
		oppV3.y = -v.y;
		oppV3.z = -v.z;
		return oppV3;
	}

	public static bool V3AbsEqual(Vector3 a, Vector3 b) {
		return (V3Equal (a, b) || V3Equal (GetOppositeV3 (a), b));
	}

	public static Vector3 acuteAngle(Vector3 a) {
		Vector3 angle = new Vector3 (a.x, a.y, a.z);
		if (angle.x > 180.0f) {
			angle.x = 360 - angle.x;
		}
		if (angle.y > 180.0f) {
			angle.y = 360 - angle.y;
		}
		if (angle.z > 180.0f) {
			angle.z = 360 - angle.z;
		}
		return angle;
	}

	public static bool GreaterEqualFloat(float a, float b) {
		return (ApproxSameFloat (a, b) || a > b);
	}

	public static bool LessThanEqualFloat(float a, float b) {
		return (ApproxSameFloat (a, b) || a < b);
	}

	public static bool ApproxSameFloat(float a, float b) {
		return Math.Abs(a - b) < 0.0001f;
	}

	public static bool V3WithinDec(Vector3 source, Vector3 target, float dec) {
		return 
			FloatWithinDec (source.x, target.x, dec)
			&& FloatWithinDec (source.y, target.y, dec)
			&& FloatWithinDec (source.z, target.z, dec);
	}

	public static bool FloatWithinDec(float a, float b, float dec) {
		return Math.Abs(a - b) < Math.Pow (10, dec);
	}



	public static float GetSlope(float x1, float z1, float x2, float z2) {
		return (z1 - z2) / (x1 - x2);
	}

	public static float GetB (float x, float z, float m) {
		return z - (m * x);
	}

	public static float PointDistance(float x1, float z1, float x2, float z2) {
		return (float) Math.Sqrt (Math.Pow(x1-x2,2) + Math.Pow(z1-z2,2));
	}

	public static bool InBetweenExcl(float v1, float v2, float vToCheck) {
		float left, right;
		left = Math.Min (v1, v2);
		right = Math.Max (v1, v2);
		return vToCheck > left && vToCheck < right;
	}

	// taken from 
	// https://answers.unity.com/questions/532297/rotate-a-vector-around-a-certain-point.html

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler(angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

}