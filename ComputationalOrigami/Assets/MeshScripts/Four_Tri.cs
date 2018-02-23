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

    }

    void FoldHorizontal(int direction)
    {
        if (horz_rotatable)
        {
            int degreesPerSecond = 20;
            Debug.DrawRay(pivots[1 + (4 * direction)].position, Vector3.back.normalized, Color.red);
            trisquares[0].transform.RotateAround(pivots[1 + (4 * direction)].position, Vector3.back, degreesPerSecond * Time.deltaTime);
            trisquares[3].transform.RotateAround(pivots[3 + (4 * direction)].position, Vector3.back, degreesPerSecond * Time.deltaTime);
        }
        
    }

    void FoldVertical()
    {

    }

	
	// Update is called once per frame
	void Update () {
        //FoldHorizontal(UP);
	}



    public void SetHorizontalRotation(bool rotatable)
    {
        horz_rotatable = rotatable;
    }


}

