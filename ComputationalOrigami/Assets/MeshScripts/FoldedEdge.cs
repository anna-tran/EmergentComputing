using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldedEdge {
	float vertice1;
	float vertice2;

	public FoldedEdge() {}

	public FoldedEdge(float v1, float v2)
	{
		vertice1 = v1;
		vertice2 = v2;
	}

	public float GetVertice1()
	{
		return vertice1;
	}

	public float GetVertice2()
	{
		return vertice2;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is FoldedEdge))
		{
			return false;
		}

		FoldedEdge e = (FoldedEdge)obj;
		if ((Mathf.Approximately(vertice1,e.GetVertice1()) && Mathf.Approximately(vertice2,e.GetVertice2()))
			|| (Mathf.Approximately(vertice2,e.GetVertice1()) && Mathf.Approximately(vertice1,e.GetVertice2()))
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
			hash = hash * 23 + (int) vertice1.GetHashCode();
			hash = hash * 23 + (int) vertice2.GetHashCode();
			return hash;
		}
	}

	public override string ToString()
	{
		return "vertice 1: " + vertice1
			+ "\nvertice 2: " + vertice2
			+ "\n";
	}

}
