using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Triangle : MonoBehaviour {

    public static int UP = 0;
    public static int DOWN = 1;

    // up, down
    Vector3[] directions;
    Transform[] pivots;
    
    bool rotatable;

    Transform other;


    void Start()
    {
        rotatable = true;
        directions = new Vector3[2];
        directions[0] = transform.TransformDirection(new Vector3(1, 0, 1));
        directions[1] = transform.TransformDirection(new Vector3(-1, 0, -1));

        pivots = new Transform[2];
        pivots[0] = transform.Find("Pivot_Hyp_Up").transform;
        pivots[1] = transform.Find("Pivot_Hyp_Down").transform;

        other = GameObject.Find("Sphere").transform;


    }




    public void Rotate(int direction_index)
    {
        if (rotatable)
        {
            Vector3 dir = directions[direction_index];
            float degreesPerSecond = 50.0f;
            Debug.DrawRay(pivots[direction_index].position, dir, Color.red);
            transform.RotateAround(pivots[direction_index].position, dir,degreesPerSecond * Time.deltaTime);
        } 
       
    }

    void OnCollisionEnter(Collision collision)
    {
        
        

        if (collision.gameObject.name.Equals("Corner_In") && !collision.transform.IsChildOf(transform))
        {
            print(collision.gameObject.name);
            rotatable = false;
        }
    }


    void OnCollisionStay(Collision collision)
    {
        

        if (collision.gameObject.name.Equals("Corner_In") && !collision.transform.IsChildOf(transform))
        {
            rotatable = false;
        }

    }
    
}
