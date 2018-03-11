using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEdge {

	public Transform start { get; private set; }
	public Transform end { get; private set; }
	public EdgeType edge_type { get; private set; }


	public TransformEdge(Transform v_start, Transform v_end, EdgeType e_t)
	{
		start = v_start;
		end = v_end;
		edge_type = e_t;
	}


	public override bool Equals(object obj)
	{
		if (!(obj is TransformEdge))
		{
			return false;
		}

		TransformEdge e = (TransformEdge)obj;
		if (
			((start.Equals(e.start) && end.Equals(e.end))
				|| (start.Equals(e.end) && end.Equals(e.start)))
			&& edge_type == e.edge_type
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
			hash = hash * 23 + start.GetHashCode();
			hash = hash * 23 + end.GetHashCode();
			hash = hash * 23 + edge_type.GetHashCode ();
			return hash;
		}
	}

	public override string ToString()
	{
		return "start v: " + start
			+ "\nend v: " + end
			+ "\ntype: " + edge_type;
	}
}
