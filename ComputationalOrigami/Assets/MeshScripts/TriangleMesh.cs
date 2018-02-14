using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TriangleMesh : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    // Use this for initialization
    void Start () {
        MakeMeshData();
        CreateMesh();

	}

    void MakeMeshData()
    {
        // create an array of vertices
        vertices = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,0)
        };
        // create an array of integers
        // order for Unity to read indices of 'vertices' array
        triangles = new int[] {
            0, 1, 2,    // top face
            0, 2, 1     // bottom face
        };
    }

    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
