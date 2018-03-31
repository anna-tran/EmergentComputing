using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pocket {

	public TransformEdge edge1 { get; private set; }
	public TransformEdge edge2 { get; private set; }
	public float angle { get; private set; }
	public Color color {get; private set; }

	public bool filled { get; set; }

	public static float CalculateAngle(TransformEdge e1, TransformEdge e2) {
		Vector3 v1 = e1.end.position - e1.start.position;
		Vector3 v2 = e2.end.position - e2.start.position;
		float dot_v1v2 = Vector3.Dot (v1, v2);
		float magn_v1 = Vector3.Magnitude (v1);
		float magn_v2 = Vector3.Magnitude (v2);
		float angle = Mathf.Acos (dot_v1v2 / (magn_v1 * magn_v2));
		return angle;
	}

	public Pocket(TransformEdge e1, TransformEdge e2) {
		edge1 = e1;
		edge2 = e2;
		angle = Pocket.CalculateAngle (e1,e2);
		filled = false;
//		RandEdgeColor ();
		color = Color.red;
	}

	void RandEdgeColor() {
		color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
	}

	public Vector3 GetPocketCenter() {
		return edge1.start.position;
	}

	public Vector3 GetVectorIn() {
		Vector3 v1 = edge1.end.position - edge1.start.position;
		Vector3 v2 = edge2.end.position - edge2.start.position;
		return v1 + v2;
	}

	public Vector3 GetVectorPerp() {
		Vector3 vIn = GetVectorIn ();
		return (vIn);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Pocket))
		{
			return false;
		}

		Pocket p = (Pocket)obj;
		if (
			((edge1.Equals(p.edge1) && edge2.Equals(p.edge2))
				|| (edge2.Equals(p.edge1) && edge1.Equals(p.edge2)))
		)
		{
			return true;
		} else
		{
			return false;
		}
	}

	public override int GetHashCode()
	{
		unchecked // Overflow is fine, just wrap
		{
			int hash = 17;
			// Suitable nullity checks etc, of course :)
			hash = hash * 23 + edge1.GetHashCode();
			hash = hash * 23 + edge2.GetHashCode();
			return hash;
		}
	}

	public override string ToString()
	{
		return "edge1: " + edge1.ToString()
			+ "\nedge2: " + edge2.ToString()
			+ "\nangle: " + angle;
	}
}
