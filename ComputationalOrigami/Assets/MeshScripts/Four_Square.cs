using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Four_Square : MonoBehaviour {
	public static float PAPER_THICKNESS = 0.01f;

    // values are local position
	List<FoldedEdge> edges;

	// Use this for initialization
	void Start () {
		edges = new List<FoldedEdge> ();
		foreach (Transform child in transform) {
			print (child.name + "\n" + child.GetComponent<MeshRenderer>().bounds);
		}
		print (transform.position);
//        RotateHorz();
        RotateVert();
        foreach (FoldedEdge e in edges)
        {
            print(e);
        }
	}

    void AssignLowest(ref float a, float b)
    {
        if (b < a)
        {
            a = b;
        }
    }

    void AssignHighest(ref float a, float b)
    {
        if (b > a)
        {
            a = b;
        }
    }




    void RotateHorz() {
        // direction = up
        // temporarily parent each child to a new GameObject
        GameObject c_parent = new GameObject();
        c_parent.transform.position = transform.position;

        List<Transform> children_to_group = new List<Transform>();
        float lowest = 0, highest = 0, height = 0;
		foreach (Transform child in transform)
			if (child.position.x < transform.position.x) {
				children_to_group.Add (child);
				float z_val = child.GetComponent<MeshRenderer> ().bounds.center.z - transform.position.z;
				AssignLowest (ref lowest, z_val);
				AssignHighest (ref highest, z_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				float y_val = child.GetComponent<MeshRenderer> ().bounds.center.y - transform.position.y;
				AssignHighest (ref height, y_val);
			}
                

        foreach (Transform child in children_to_group)
            child.parent = c_parent.transform;
		float topmost_y = height + PAPER_THICKNESS;
        c_parent.transform.Rotate(Vector3.forward, 180);
		c_parent.transform.Translate(new Vector3(0,-topmost_y,0));

        // reparent children to this object
        while (c_parent.transform.childCount > 0)
            c_parent.transform.GetChild(0).parent = transform;

        
        if (Mathf.Abs(lowest - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(0, 0, lowest);
			edges.Add(new FoldedEdge(v, transform.position));
        }
        if (Mathf.Abs(highest - 0.0f) > Mathf.Epsilon) {
			Vector3 v = new Vector3(0, 0, highest);
			edges.Add(new FoldedEdge(v, transform.position));
        }
    }

	void RotateVert() {
        // direction = up
        // temporarily parent each child to a new GameObject
        GameObject c_parent = new GameObject();
        c_parent.transform.position = transform.position;

        List<Transform> children_to_group = new List<Transform>();
        float lowest = 0, highest = 0, height = 0;
        foreach (Transform child in transform)
            if (child.position.z > transform.position.z)
            {
                children_to_group.Add(child);
                float x_val = child.GetComponent<MeshRenderer>().bounds.center.x - transform.position.x;
                AssignLowest(ref lowest, x_val);
                AssignHighest(ref highest, x_val);
			} else {
				// find the highest y coordinate of the children to be folded on
				float y_val = child.GetComponent<MeshRenderer> ().bounds.center.y - transform.position.y;
				AssignHighest (ref height, y_val);
			}
                

        foreach (Transform child in children_to_group)
            child.parent = c_parent.transform;
		float topmost_y = height + PAPER_THICKNESS;
        c_parent.transform.Rotate(Vector3.right, 180);
		c_parent.transform.Translate(new Vector3(0,-topmost_y,0));

        // reparent children to this object
        while (c_parent.transform.childCount > 0)
            c_parent.transform.GetChild(0).parent = transform;

        
        if (Mathf.Abs(lowest - 0.0f) > Mathf.Epsilon)
        {
			Vector3 v = new Vector3(lowest, 0, 0);
			edges.Add(new FoldedEdge(v, transform.position));
        }
        if (Mathf.Abs(highest - 0.0f) > Mathf.Epsilon)
        {
			Vector3 v = new Vector3(highest, 0, 0);
			edges.Add(new FoldedEdge(v, transform.position));
        }
    }

    void RotateDiag()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
