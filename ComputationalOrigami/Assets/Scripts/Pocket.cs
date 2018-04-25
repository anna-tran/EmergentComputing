using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pocket {

	public TransformEdge edge1 { get; private set; }
	public TransformEdge edge2 { get; private set; }
	public float angle { get; private set; }
	public Color color {get; private set; }
    public Transform pCenter { get; private set; }

	public bool filled { get; set; }

	public static float CalculateAngle(TransformEdge e1, TransformEdge e2) {
		float angle = Vector3.Angle(
			e1.end.position - e1.start.position, 
			e2.end.position - e2.start.position);
		return angle;
	}



	public Pocket(TransformEdge e1, TransformEdge e2) {
		edge1 = e1;
		edge2 = e2;
		angle = CalculateAngle (e1,e2);
        // 'start' of either edge is the center
        pCenter = edge1.start;
		filled = false;
		color = Color.red;
	}

	void RandEdgeColor() {
		color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
	}

	public Vector3 GetCenterPosition() {
		return pCenter.position;
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

	public Plane GetPocketPlane() {
		return new Plane (pCenter.position, edge1.end.position, edge2.end.position);
	}

	// intersection of a pocket only if there are at least 4 vertices on each side of
	// the pocket line
	public bool OverlappedBy(Transform other) {

		Plane plane = GetPocketPlane ();
		int numPos = 0;
		int numNeg = 0;
		foreach (Transform child in other.GetComponentsInChildren<Transform>()) {
			if (child.name.Contains("Tri")) {
				if (plane.GetSide (child.position))
					numPos++;
				else
					numNeg++;
			}
		}
		return numPos >= 3 && numNeg >= 3;
	}

	/*
	 * Check if pocket is positioned in a way that another unit cannot fill it
	 */
	public bool InaccessibleDueTo(Transform other) {
		Collider[] end1Overlaps = Physics.OverlapSphere (edge1.end.position, OrigamiUnit.PAPER_THICKNESS / 4);		
		Collider[] end2Overlaps = Physics.OverlapSphere (edge2.end.position, OrigamiUnit.PAPER_THICKNESS / 4);		
		foreach (Collider col in end1Overlaps) {
			if (col.transform.parent.Equals (other) && col.transform.name.Contains("Tri"))
				return true;
		}
		foreach (Collider col in end2Overlaps) {
			if (col.transform.parent.Equals (other) && col.transform.name.Contains("Tri"))
				return true;
		}
		return false;
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
		return "edge1: " + edge1.end.name
		+ "\nedge2: " + edge2.end.name;
	}
}
