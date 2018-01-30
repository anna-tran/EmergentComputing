using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffindorSpawner : MonoBehaviour {
    public Griffindor griffindor;
	// Use this for initialization
	void Start () {
        griffindor = new Griffindor();
        for (int i = 0; i < 20; i++)
        {
            Griffindor g = Instantiate<Griffindor>(griffindor);
        }
    }
	
}
