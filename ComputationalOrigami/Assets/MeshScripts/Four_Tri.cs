using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Four_Tri : MonoBehaviour {
    static int UP = 0;
    static int DOWN = 1;
    // 1    P2  2 
    // P1       P3
    // 4    P4  3
    TriSquare[] trisquares;
    HashSet<Edge> edges;
    List<GameObject> vertices;

    // 0 to 3 UP
    // 4 to 7 DOWN
    Transform[] pivots;
    int[] tris_to_fold;
    bool horz_rotatable, vert_rotatable;

    // Use this for initialization
    void Start () {
        trisquares = new TriSquare[4];
        for (int i = 1; i <= 4; i++) {
            trisquares[i - 1] = gameObject.transform.Find("TriSquare " + i).GetComponent<TriSquare>();
        }
        pivots = new Transform[8];
        for (int i = 1; i <= 4; i++) {
            pivots[i - 1] = gameObject.transform.Find("Pivot" + i + "_Up");
        }
        for (int i = 5; i <= 8; i++) {
            pivots[i - 1] = gameObject.transform.Find("Pivot" + (i-4) + "_Down");
        }
        vertices = new List<GameObject>();
        for (int i = 0; i < 5; i++) {
            vertices.Add(GameObject.Find("Vertice" + (i+1)));
        }

        horz_rotatable = true;
        vert_rotatable = true;

        edges = new HashSet<Edge>();
    }



    void InitializeEdges(HashSet<Edge> edges)
    {
        // 1 connects to 2, 4, 5
        edges.Add(new Edge(vertices[0], vertices[1]));
        edges.Add(new Edge(vertices[0], vertices[3]));
        edges.Add(new Edge(vertices[0], vertices[4]));

        // 2 connects to 3, 5
        edges.Add(new Edge(vertices[1], vertices[2]));
        edges.Add(new Edge(vertices[1], vertices[4]));

        // 3 connects to 4, 5
        edges.Add(new Edge(vertices[2], vertices[3]));
        edges.Add(new Edge(vertices[2], vertices[4]));

        // 4 connects to 5
        edges.Add(new Edge(vertices[3], vertices[4]));
    }

    void FoldHorizontal(int direction)
    {
        
        
        for (int i = 0; i < 4; i++)
            trisquares[i].cease_fold_parent = "StopHorzRotation";
        Vector3 z_dir;
        if (direction == UP)
        {
            z_dir = Vector3.back;
        }
        else //if (direction == DOWN)
        {
            z_dir = Vector3.forward;
        }
        if (horz_rotatable)
        {
            int degreesPerSecond = 60;
            Debug.DrawRay(pivots[1 + (4 * direction)].position, Vector3.back.normalized, Color.red);
            trisquares[0].transform.RotateAround(pivots[1 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
            trisquares[3].transform.RotateAround(pivots[3 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
        }
        
    }

    void FoldVertical(int direction)
    {
        for (int i = 0; i < 4; i++)
            trisquares[i].cease_fold_parent = "StopVertRotation";
        Vector3 z_dir;
        if (direction == UP)
        {
            z_dir = Vector3.left;
        }
        else //if (direction == DOWN)
        {
            z_dir = Vector3.right;
        }
        if (vert_rotatable)
        {
            int degreesPerSecond = 60;
            Debug.DrawRay(pivots[1 + (4 * direction)].position, Vector3.back.normalized, Color.red);
            trisquares[0].transform.RotateAround(pivots[0 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
            trisquares[1].transform.RotateAround(pivots[2 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
        }

    }

    void FoldDiagonal(TriSquare a_trisquare, int direction)
    {
        a_trisquare.FoldDiagonal(direction);
    }

	
	// Update is called once per frame
	void Update () {
        // assume that non-rotatable means that we've already folded on them
        if (trisquares[0].IsDiagFoldable())
        {
            FoldDiagonal(trisquares[0], UP);
        } else if (horz_rotatable)
        {
            FoldHorizontal(UP);
        } 
        string edges_str = "";
        if (edges.Count > 0)
        {
            foreach (Edge e in edges)
            {
                edges_str += e.ToString() + "\n";
            }
            print(edges_str);
        }
        
        //FoldHorizontal(UP);
	}
    


    public void StopHorzRotation()
    {
        horz_rotatable = false;
        edges.Add(new Edge(vertices[1], vertices[4]));
        edges.Add(new Edge(vertices[3], vertices[4]));
    }

    public void StopVertRotation()
    {
        vert_rotatable = false;
        edges.Add(new Edge(vertices[0], vertices[4]));
        edges.Add(new Edge(vertices[2], vertices[4]));
    }

    public void AddEdge(Edge e)
    {
        edges.Add(e);
    }

    void AddSpringJoint(GameObject go1, GameObject go2) 
    {
        go1.AddComponent<SpringJoint>();
        GetComponent<SpringJoint>().connectedBody = go2.GetComponent<Rigidbody>();
    }

}

