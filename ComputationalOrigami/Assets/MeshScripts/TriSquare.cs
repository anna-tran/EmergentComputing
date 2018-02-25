using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriSquare : MonoBehaviour
{

    public String cease_fold_parent { get; set; }
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

    
    public void FoldDiagonal(int direction)
    {
        Triangle t2 = tri2.GetComponent<Triangle>();
        Vector3 dir = tri2.position - tri1.position;
        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
        Debug.DrawRay(tri2.position, dir, Color.yellow);
        t2.directions[0] = perp;
        t2.directions[1] = -perp;
        t2.FoldDiagonal(direction);
    }

    public bool IsDiagFoldable()
    {
        return tri2.GetComponent<Triangle>().IsDiagFoldable();
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("Tri_Center") 
                && !other.transform.IsChildOf(transform)
                && col.bounds.Intersects(other.bounds)
            )
        {
            gameObject.AddComponent<SpringJoint>();
            GetComponent<SpringJoint>().connectedBody = other.GetComponent<Rigidbody>();
            Debug.Log(transform.name + " collided with " + other.GetComponentInParent<TriSquare>().name
                    + " -- " + other.name);
            gameObject.SendMessageUpwards(cease_fold_parent, false);
        }
    }



}
