using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriSquare : MonoBehaviour
{
    BoxCollider col;
    Triangle tri1;
    Triangle tri2;

    // Use this for initialization
    void Start()
    {
        tri1 = gameObject.transform.Find("Triangle 1").GetComponent<Triangle>();
        tri2 = gameObject.transform.Find("Triangle 2").GetComponent<Triangle>();

        col = transform.GetComponent<Collider>() as BoxCollider;
        col.size = GetComponent<MeshFilter>().mesh.bounds.size;

    }
    

    public void FoldDiagonal()
    {
        tri2.FoldDiagonal(Triangle.UP);
    }

    public bool IsDiagFoldable()
    {
        return tri2.IsDiagFoldable();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        print(transform.name + " touching " + collision.collider.GetComponentInParent<TriSquare>().name
                + " -- " + collision.collider.name);
        if (collision.gameObject.name.Contains("Corner") 
            && !collision.transform.IsChildOf(transform)
            && GetComponent<Collider>().bounds.Intersects(collision.collider.bounds))
        {
            gameObject.AddComponent<HingeJoint>();
            gameObject.GetComponent<HingeJoint>().connectedBody = collision.rigidbody;
            print(transform.name +  " collided with " + collision.collider.GetComponentInParent<TriSquare>().name
                + " -- " + collision.collider.name);
            gameObject.SendMessageUpwards("SetHorizontalRotation", false);
        }
    }



}
