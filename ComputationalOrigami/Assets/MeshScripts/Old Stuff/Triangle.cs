﻿using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Triangle : MonoBehaviour {

    public static int UP = 0;
    public static int DOWN = 1;

    Collider col;
    Rigidbody rb;
    // up, down
    public Vector3[] directions { get; set; }
    Transform[] pivots;
    
    bool diag_foldable;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = transform.GetComponent<Collider>();

        diag_foldable = true;
        directions = new Vector3[2];
        directions[0] = transform.TransformDirection(new Vector3(1, 0, 1));
        directions[1] = transform.TransformDirection(new Vector3(-1, 0, -1));

        pivots = new Transform[2];
        pivots[0] = transform.Find("Pivot_Hyp_Up").transform;
        pivots[1] = transform.Find("Pivot_Hyp_Down").transform;
    }



    private void addForce()
    {
        if (gameObject.name.Contains("2"))
        {
            rb.AddForce(Vector3.up * 3, ForceMode.Acceleration);
        }
            
    }

    public void FoldDiagonal(int direction_index)
    {
        if (diag_foldable)
        {
           
            Vector3 dir = directions[direction_index];
            float degreesPerSecond = 70.0f;
            Debug.DrawRay(pivots[direction_index].position, dir, Color.red);
            transform.RotateAround(pivots[direction_index].position, dir,degreesPerSecond * Time.deltaTime);
            
        } 
       
    }

    void OnCollisionEnter(Collision collision)
    {


        if (collision.collider.name.Contains("Corner_In")
            && !collision.collider.transform.IsChildOf(transform))
        {
            gameObject.AddComponent<FixedJoint>();
            GetComponent<FixedJoint>().connectedBody = collision.collider.GetComponent<Rigidbody>();
            //Debug.Log(transform.name + " collided with " + other.GetComponentInParent<TriSquare>().name
            //        + " -- " + other.name);
            diag_foldable = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.name.Contains("Corner_In") )
        {
            gameObject.AddComponent<FixedJoint>();
            GetComponent<FixedJoint>().connectedBody = other.GetComponent<Rigidbody>();
            
            GameObject[] vertices = GameObject.FindGameObjectsWithTag("Vertice");
            GameObject[] touching_vertices = new GameObject[2];
            int i = 0;
            foreach(GameObject v in vertices)
            {
                if (v.GetComponent<Renderer>().bounds.Intersects(GetComponent<Renderer>().bounds) && !v.name.Contains("Vertice5"))
                {
                    touching_vertices[i] = v;
                    i++;
                }

            }
            Edge e = new Edge(touching_vertices[0], touching_vertices[1]);
            gameObject.SendMessageUpwards("AddEdgeToParent", e);

            diag_foldable = false;
        }
    }

    public bool IsDiagFoldable()
    {
        return diag_foldable;
    }

    
}