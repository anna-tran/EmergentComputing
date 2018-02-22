using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriSquare : MonoBehaviour {

    Triangle tri1;
    Triangle tri2;

    // Use this for initialization
    void Start()
    {
        tri1 = gameObject.transform.Find("Triangle 1").GetComponent<Triangle>();
        tri2 = gameObject.transform.Find("Triangle 2").GetComponent<Triangle>();
    }
/*
    void Update()
    {
        
        if (gameObject.name.Contains("1"))
            tri2.Rotate(Triangle.UP);
    }
    */
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Corner_Out") && !collision.transform.IsChildOf(transform))
        {
            gameObject.SendMessageUpwards("SetHorizontalRotation", false);
        }

    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name.Equals("Corner_Out") && !collision.transform.IsChildOf(transform))
        {
            gameObject.SendMessageUpwards("SetHorizontalRotation", false);
        }

    }

}
