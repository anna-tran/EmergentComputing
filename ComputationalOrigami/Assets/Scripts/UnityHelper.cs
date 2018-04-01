using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityHelper : MonoBehaviour {
	public static System.Random rand = new System.Random ();


    public static bool IntersectsPocket(TransformEdge e, Pocket p, EdgeType et)
    {
        Vector3 s1 = e.start.position;
        Vector3 e1 = e.end.position;
        Vector3 s2 = p.edge1.end.position;
        Vector3 e2 = p.edge2.end.position;
        float m1 = GetSlope(s1.x, s1.z, e1.x, e1.z);
        float m2 = GetSlope(s2.x, s2.z, e2.x, e2.z);

        if (ApproxSameFloat(m1, m2))
        {
            //			print ("m1 " + m1 + "  m2 " + m2);
            return false;
        }
        float intx, intz;
        if (Double.IsInfinity(m1))
        {
            float b2 = GetB(e2.x, e2.z, m2);
            intx = e1.x;
            intz = (m2 * intx) + b2;
        }
        else if (Double.IsInfinity(m2))
        {
            float b1 = GetB(e1.x, e1.z, m1);
            intx = e2.x;
            intz = (m1 * intx) + b1;
        }
        else
        {
            float b1 = GetB(e1.x, e1.z, m1);
            float b2 = GetB(e2.x, e2.z, m2);
            intx = (b2 - b1) / (m1 - m2);
            intz = (m1 * intx) + b1;
        }
        // if not the same as the ends of the pocket
        if (!ApproxSameFloat(intx, s2.x)
            && !ApproxSameFloat(intx, e2.x)
            && InBetweenExcl(s2.x, e2.x, intx))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public static void RandomlyPosition(Transform orig, Transform zone) {
		Vector3 zoneSize = zone.GetComponent<MeshRenderer> ().bounds.size;
		float xRange = zoneSize.x-5;
		float yRange = 1;
		float zRange = zoneSize.z-5;
		do {
			float newX = (float) rand.NextDouble() * xRange;
			SetV3Value(orig,"x",newX-(xRange/2));
		} while (orig.position.x >= xRange / 2 || orig.position.x <= -xRange / 2);

		SetV3Value(orig,"y", (float) rand.NextDouble() * yRange);

		do {
			float newZ = (float) rand.NextDouble() * zRange;
			SetV3Value(orig,"z",newZ-(zRange/2));
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
		return Vector3.SqrMagnitude(a - b) < 0.0001f;
	}

	public static bool V3ApproxEqual(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.0003f;
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

}