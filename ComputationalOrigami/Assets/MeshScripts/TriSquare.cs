using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriSquare : MonoBehaviour {

    GameObject triangle1;
    GameObject triangle2;

	// Use this for initialization
	void Start () {
        triangle1 = this.gameObject.transform.GetChild(0).gameObject;
        triangle2 = this.gameObject.transform.GetChild(1).gameObject;

        triangle1.transform.Rotate(Vector3.right);
        print(triangle1.name);
    }

    void Update()
    {
        // Rotate the object around its local X axis at 1 degree per second
        triangle1.transform.Rotate(Vector3.right * Time.deltaTime);

        // ...also rotate around the World's Y axis
        triangle1.transform.Rotate(Vector3.up * Time.deltaTime, Space.World);
    }
}
