﻿using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Triangle : MonoBehaviour {

    public static int UP = 0;
    public static int DOWN = 1;

    // up, down
    Vector3[] directions;
    Transform[] pivots;
    
    bool diag_foldable;

    Transform other;


    void Start()
    {
        diag_foldable = true;
        directions = new Vector3[2];
        directions[0] = transform.TransformDirection(new Vector3(1, 0, 1));
        directions[1] = transform.TransformDirection(new Vector3(-1, 0, -1));

        pivots = new Transform[2];
        pivots[0] = transform.Find("Pivot_Hyp_Up").transform;
        pivots[1] = transform.Find("Pivot_Hyp_Down").transform;

    }




    public void FoldDiagonal(int direction_index)
    {
        if (diag_foldable)
        {
            Vector3 dir = directions[direction_index];
            float degreesPerSecond = 50.0f;
            Debug.DrawRay(pivots[direction_index].position, dir, Color.red);
            transform.RotateAround(pivots[direction_index].position, dir,degreesPerSecond * Time.deltaTime);
        } 
       
    }

    void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.name.Contains("Corner_In") && !collision.transform.IsChildOf(transform))
        {
            gameObject.AddComponent<HingeJoint>();
            gameObject.GetComponent<HingeJoint>().connectedBody = collision.rigidbody;
            print(collision.gameObject.name);
            diag_foldable = false;
        }
    }

    public bool IsDiagFoldable()
    {
        return diag_foldable;
    }

    
}
