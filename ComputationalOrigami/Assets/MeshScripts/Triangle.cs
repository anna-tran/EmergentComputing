using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Triangle : MonoBehaviour {
    Mesh mesh;
    Transform[] pivots;
    Vector3 direction;
    public List<Edge> edges { get; set; }
    bool rotatable;
    

    private void Awake()
    {

        edges = new List<Edge>();
        mesh = GetComponent<MeshFilter>().mesh;
        int[] tris = mesh.triangles;
        HashSet<HashSet<int>> uniqueEdges = new HashSet<HashSet<int>>();
        for (int i = 0; i < tris.Length; i += 3)
        {
            HashSet<int> verticeSet = new HashSet<int> { tris[i], tris[i + 1], tris[i + 2] };
            if (!uniqueEdges.Contains(verticeSet))
            {
                uniqueEdges.Add(verticeSet);
            }
            
        }

        foreach (HashSet<int> verticeSet in uniqueEdges)
        {
            int[] vertices = new int[3];
            verticeSet.CopyTo(vertices);
            int v1 = vertices[0];
            int v2 = vertices[1];
            int v3 = vertices[2];

            edges.Add(new Edge(v1, v2));
            edges.Add(new Edge(v1, v3));
            edges.Add(new Edge(v2, v3));
        }
    }



    void Start()
    {
        rotatable = true;
        int num_children = gameObject.transform.childCount;
        pivots = new Transform[num_children];
        for (int i = 0; i < num_children; i++)
        {
            Transform pivot = gameObject.transform.GetChild(i).transform;
            pivots[i] = pivot;
        }


        direction = transform.TransformDirection(new Vector3(-0.5f, 0, -0.5f));
    }

    public void Rotate()
    {
        if (rotatable)
        {
            float degreesPerSecond = 50.0f;
            transform.RotateAround(pivots[0].position, direction,degreesPerSecond * Time.deltaTime);
        } 
       
    }


    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }

        string colls = gameObject.name + "\n";
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            ContactPoint contact = collision.contacts[i];
            Debug.DrawRay(contact.point, contact.normal, Color.red);
            if (Math.Abs(contact.point.x) < 0.01f &&
                Math.Abs(contact.point.y) < 0.01f &&
                Math.Abs(contact.point.z) < 0.01f)
                
            {              
                rotatable = false;
            }
        }

    }
}
