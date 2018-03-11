using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pocket {

	public TransformEdge edge1 { get; private set; }
	public TransformEdge edge2 { get; private set; }
	public float width { get; private set; }

	public bool filled { get; set; }

	public Pocket(TransformEdge e1, TransformEdge e2) {
		edge1 = e1;
		edge2 = e2;
		filled = false;

		CalculateWidth ();
	}

	private void CalculateWidth() {
		Vector3 v1 = edge1.end.position - edge1.start.position;
		Vector3 v2 = edge2.end.position - edge2.start.position;
		float x2 = Mathf.Pow (v1.x - v2.x, 2);
		float y2 = Mathf.Pow (v1.y - v2.y, 2);
		float z2 = Mathf.Pow (v1.z - v2.z, 2);
		width = Mathf.Sqrt (x2 + y2 + z2);
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
			+ "\nedge2: " + edge2.ToString();
	}
}
