using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    private int s_score;
    private int g_score;
    
	// Use this for initialization
	void Start () {
        s_score = 0;
        g_score = 0;
    }
	

    void pointForSlytherin()
    {
        s_score++;
        printScores();
    }

    void pointForGriffindor()
    {
        g_score++;
        printScores();
    }

    void printScores()
    {

        print("Score\nSlytherin: " + s_score + "\tGriffindor: " + g_score);
    }
}
