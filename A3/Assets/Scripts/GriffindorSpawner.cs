using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffindorSpawner : MonoBehaviour {
    public Rigidbody g_player;
    public Transform spawner;

	// Use this for initialization
	void Start () {
        // Currently there is one Griffindor player on the field
        // To create a team of 20, instantiate 19 more players
        for (int i = 0; i < 19; i++)
        {
            delaySpawn();
            Rigidbody g_copy;
            g_copy = Instantiate(g_player, spawner.position, spawner.rotation) as Rigidbody;
        }
    }

    IEnumerator delaySpawn()
    {
        yield return new WaitForSecondsRealtime(2);
    }
}
