using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlytherinSpawner : MonoBehaviour {
    public Rigidbody s_player;
    public Transform spawner;

    // Use this for initialization
    void Start()
    {
        // Currently there is one Slytherin player on the field
        // To create a team of 20, instantiate 19 more players
        for (int i = 0; i < 19; i++)
        {
            StartCoroutine(delaySpawn());
            Rigidbody s_copy;
            s_copy = Instantiate(s_player, spawner.position, spawner.rotation) as Rigidbody;
        }
    }

    IEnumerator delaySpawn()
    {
        yield return new WaitForSecondsRealtime(2);
    }
}
