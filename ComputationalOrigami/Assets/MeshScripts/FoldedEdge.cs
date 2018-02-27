using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldedEdge {
	Vector3 vertice1;
    Vector3 vertice2;

	public FoldedEdge() {}

	public FoldedEdge(Vector3 v1, Vector3 v2)
	{
		vertice1 = v1;
		vertice2 = v2;
	}

	public Vector3 GetVertice1()
	{
		return vertice1;
	}

	public Vector3 GetVertice2()
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
		if ((vertice1.Equals(e.GetVertice1()) && vertice2.Equals(e.GetVertice2()))
            || (vertice1.Equals(e.GetVertice2()) && vertice2.Equals(e.GetVertice1()))
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
			hash = hash * 23 + vertice1.GetHashCode();
			hash = hash * 23 + vertice2.GetHashCode();
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
