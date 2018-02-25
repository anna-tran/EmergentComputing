using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Four_Tri : MonoBehaviour {
    static int UP = 0;
    static int DOWN = 1;
    // 1    P2  2 
    // P1       P3
    // 4    P4  3
    TriSquare[] trisquares;

    // 0 to 3 UP
    // 4 to 7 DOWN
    Transform[] pivots;
    int[] tris_to_fold;
    bool horz_rotatable, vert_rotatable;

    // Use this for initialization
    void Start () {
        trisquares = new TriSquare[4];
        for (int i = 1; i <= 4; i++) {
            trisquares[i - 1] = gameObject.transform.Find("TriSquare " + i).GetComponent<TriSquare>();
            if (trisquares[i-1] == null)
            {
                print("null at trisquares index " + i);
            }
        }

        pivots = new Transform[8];
        for (int i = 1; i <= 4; i++) {
            pivots[i - 1] = gameObject.transform.Find("Pivot" + i + "_Up");
            if (pivots[i - 1] == null)
            {
                print("null at pivots index " + i);
            }
        }
        for (int i = 5; i <= 8; i++) {
            pivots[i - 1] = gameObject.transform.Find("Pivot" + (i-4) + "_Down");
            if (pivots[i - 1] == null)
            {
                print("null at pivots index " + i);
            }
        }

        horz_rotatable = true;
        vert_rotatable = true;

        AddSpringJoint(trisquares[0].gameObject, trisquares[1].gameObject);
        AddSpringJoint(trisquares[1].gameObject, trisquares[2].gameObject);
        AddSpringJoint(trisquares[2].gameObject, trisquares[3].gameObject);
        AddSpringJoint(trisquares[3].gameObject, trisquares[0].gameObject);
        gameObject.AddComponent<Collider>();
    }

    void FoldHorizontal(int direction)
    {
        for (int i = 0; i < 4; i++)
            trisquares[i].cease_fold_parent = "SetHorizontalRotation";
        Vector3 z_dir;
        if (direction == UP)
        {
            z_dir = Vector3.back;
        }
        else //if (direction == DOWN)
        {
            z_dir = Vector3.forward;
        }
        if (horz_rotatable)
        {
            int degreesPerSecond = 60;
            Debug.DrawRay(pivots[1 + (4 * direction)].position, Vector3.back.normalized, Color.red);
            trisquares[0].transform.RotateAround(pivots[1 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
            trisquares[3].transform.RotateAround(pivots[3 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
        }
        
    }

    void FoldVertical(int direction)
    {
        for (int i = 0; i < 4; i++)
            trisquares[i].cease_fold_parent = "SetVerticalRotation";
        Vector3 z_dir;
        if (direction == UP)
        {
            z_dir = Vector3.left;
        }
        else //if (direction == DOWN)
        {
            z_dir = Vector3.right;
        }
        if (vert_rotatable)
        {
            int degreesPerSecond = 60;
            Debug.DrawRay(pivots[1 + (4 * direction)].position, Vector3.back.normalized, Color.red);
            trisquares[0].transform.RotateAround(pivots[0 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
            trisquares[1].transform.RotateAround(pivots[2 + (4 * direction)].position, z_dir, degreesPerSecond * Time.deltaTime);
        }

    }

    void FoldDiagonal(TriSquare a_trisquare, int direction)
    {
        a_trisquare.FoldDiagonal(direction);
    }

	
	// Update is called once per frame
	void Update () {
        if (vert_rotatable)
        {
            FoldVertical(UP);
        } else
        {
            print(GetComponent<Collider>().bounds.size);
        }
         
        //FoldHorizontal(UP);
	}
    


    public void SetHorizontalRotation(bool rotatable)
    {
        horz_rotatable = rotatable;
    }

    public void SetVerticalRotation(bool rotatable)
    {
        vert_rotatable = rotatable;
    }

    void AddSpringJoint(GameObject go1, GameObject go2) 
    {
        go1.AddComponent<SpringJoint>();
        GetComponent<SpringJoint>().connectedBody = go2.GetComponent<Rigidbody>();
    }

}

