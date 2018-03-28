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

	public static void setV3Value(Transform orig, string field, float value) {
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
		return (a - b) < 0.0001f;
	}

}