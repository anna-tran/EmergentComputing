using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityHelper : MonoBehaviour {
	public static void getNewRotation(Transform orig, string field, float value) {
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



	public static float getSlope(float x1, float z1, float x2, float z2) {
		return (z1 - z2) / (x1 - x2);
	}

	public static float getB (float x, float z, float m) {
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