using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriSquare : MonoBehaviour {

    Triangle[] children;

	// Use this for initialization
	void Start () {
        int num_children = gameObject.transform.childCount;
        children = new Triangle[num_children];
        int num_edges = 0;
        for (int i = 0; i < num_children; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            children[i] = child.GetComponent<Triangle>();
            num_edges = children[i].edges.Count;

            children[i].edges.Sort();
            
           
        }
        for (int i = 0; i < num_edges; i++)
        {
            Edge a = children[0].edges.ElementAt(i);
            Edge b = children[1].edges.ElementAt(i);
            if (a.Equals(b)) {
                //print(a);
            }
        }

 
        



    }
    
    void Update()
    {

        children[0].Rotate();
    }
    
}
