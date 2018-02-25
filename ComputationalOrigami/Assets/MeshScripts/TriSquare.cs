using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriSquare : MonoBehaviour
{
    Collider col;
    Transform tri1;
    Transform tri2;

    // Use this for initialization
    void Start()
    {
        tri1 = gameObject.transform.Find("Triangle 1");
        tri2 = gameObject.transform.Find("Triangle 2");

        col = transform.GetComponent<Collider>();
        
    }
    
    void Update()
    {
        
        col.bounds.Encapsulate(tri1.GetComponent<Collider>().bounds);
        col.bounds.Encapsulate(tri2.GetComponent<Collider>().bounds);
    }

    
    public void FoldDiagonal()
    {
        Triangle t2 = tri2.GetComponent<Triangle>();
        Vector3 dir = tri2.position - tri1.position;
        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
        Debug.DrawRay(tri2.position, dir, Color.yellow);
        t2.directions[0] = perp;
        t2.directions[1] = -perp;
        t2.FoldDiagonal(Triangle.UP);
    }

    public bool IsDiagFoldable()
    {
        return tri2.GetComponent<Triangle>().IsDiagFoldable();
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("Corner") 
            && !other.transform.IsChildOf(transform)
            && col.bounds.Intersects(other.bounds)
            )
        {
            Debug.Log(transform.name + " collided with " + other.GetComponentInParent<TriSquare>().name
                    + " -- " + other.name);
            gameObject.SendMessageUpwards("SetHorizontalRotation", false);
        }
    }



}
