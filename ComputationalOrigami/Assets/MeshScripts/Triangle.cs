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
    


    void Start()
    {
        rotatable = true;
        directions = new Vector3[2];
        directions[0] = transform.TransformDirection(new Vector3(0, 1, 0));
        directions[1] = transform.TransformDirection(new Vector3(-0.5f, 0, -0.5f));

        pivots = new Transform[2];
        pivots[0] = gameObject.transform.Find("Pivot_Hyp_Up");
        pivots[1] = gameObject.transform.Find("Pivot_Hyp_Down");
        
    }

    public void Update()
    {
        if (rotatable)
        {
            float degreesPerSecond = 50.0f;
            Debug.DrawRay(transform.position, directions[0], Color.blue);
            transform.RotateAround(pivots[0].position, directions[0], degreesPerSecond * Time.deltaTime);
        }

    }


    public void Rotate(int direction_index)
    {
        if (rotatable)
        {

            float degreesPerSecond = 50.0f;
            Debug.DrawRay(pivots[direction_index].position, directions[direction_index], Color.red);
            transform.RotateAround(pivots[direction_index].position, directions[direction_index],degreesPerSecond * Time.deltaTime);
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
            print(collision.gameObject.name);
            rotatable = false;
        }

    }
    
}
