using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffindorScript : MonoBehaviour {
    private GameObject game;

    // player's targets
    private GameObject snitch;
    private Vector3 snitchPosition;

    private GameObject startPoint;
    private Vector3 startPosition;

    // player's qualities
    public float velocity;
    public float acceleration;
    public float probTackle;

    float playerDistance;
    private System.Random random;

    public Rigidbody body;

    // Called before Start()
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
        body.detectCollisions = true;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        game = GameObject.Find("Game");
        snitch = GameObject.Find("Snitch");
        startPoint = GameObject.Find("G_Start_Point");

        velocity = 16.0f;
        acceleration = 20.0f;
        probTackle = 0.3f;

        body.velocity = transform.forward * velocity;
        body.position = getStartPosition();
        random = new System.Random();
        playerDistance = 10;
    }
	
	// Update is called once per frame
	void Update () {
        // The player will move towards the snitch
        transform.LookAt(snitch.transform);
        body.AddRelativeForce(Vector3.forward*acceleration, ForceMode.Acceleration);

        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        Debug.DrawRay(transform.position, fwd * playerDistance);

        if (Physics.Raycast(transform.position, fwd, out hit, playerDistance))
        {
            if (hit.collider.gameObject.name.Contains("Player"))
            {
                transform.Rotate(new Vector3(0, -0.7f, 0) * Time.deltaTime * acceleration, Space.World);
                print("Griffindor: There is a player in front of me!");
            }
            
        }
            
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("collided with " + collision.gameObject.name);
        if (collision.gameObject.name == "Snitch")
        {
            game.SendMessage("pointForGriffindor");
        } else if (collision.gameObject.name == "S_player")
        {

        } else if (collision.gameObject.name == "Field")
        {
            transform.position = getStartPosition();
            print("Griffindor collided with field");
            transform.LookAt(snitch.transform);
            body.AddRelativeForce(Vector3.forward * acceleration, ForceMode.Acceleration);

        }
    }
    /*
    private void OnCollisionExit(Collision collision)
    {
        //print("collided with " + collision.gameObject.name);
        if (collision.gameObject.name == "Field")
        {
            transform.position = getStartPosition();
            print(transform.position.ToString());
        }
    }
    */
    private Vector3 getStartPosition()
    {
        
        return new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, startPoint.transform.position.z);
    }

}
