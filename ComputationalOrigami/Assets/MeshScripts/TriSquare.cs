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
    /*
    void Update()
    {

        tri2.Rotate(Triangle.UP);
    }
    */

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Corner_Out") && !collision.transform.IsChildOf(transform))
        {
            print(transform.name +  " collided with " + collision.collider.GetComponentInParent<TriSquare>().name);
            print(collision.contacts);
            gameObject.SendMessageUpwards("SetHorizontalRotation", false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        
    }

}
