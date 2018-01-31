using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Griffindor : MonoBehaviour {
    private GameObject game;

    // player's targets
    private GameObject snitch;
    private GameObject startPoint;

    // player's qualities
    public float velocity;
    public float acceleration;
    public float probTackle;

    private bool wasHit;
    private float playerDistance;
    private float personalRadius;
    private System.Random random;
    

    public Rigidbody body { get; private set; }

    // Called before Start()
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
        body.detectCollisions = true;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        game = GameObject.Find("Quidditch_Game");
        snitch = GameObject.Find("Snitch");
        startPoint = GameObject.Find("G_Start_Point");

        random = new System.Random();
        playerDistance = 10;
        personalRadius = 3;
        wasHit = false;

        velocity = 16.0f;
        acceleration = 20.0f;
        probTackle = 0.3f;

        body.velocity = transform.forward * velocity;
        body.position = getStartPosition();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (wasHit)
        {
            body.AddForce(Vector3.down * acceleration, ForceMode.Acceleration);
        }
        else
        {
            // The player will move towards the snitch
            transform.LookAt(snitch.transform);
            body.AddRelativeForce(Vector3.forward * acceleration, ForceMode.Acceleration);

            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            //Debug.DrawRay(transform.position, fwd * playerDistance);

            if (Physics.Raycast(transform.position, fwd, out hit, playerDistance))
            {
                if (hit.collider.gameObject.name.Contains("Player"))
                {
                    transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * acceleration, Space.World);
                }

            }
        }
        
            
    }



    private void OnCollisionEnter(Collision collision)
    {
        //print("collided with " + collision.gameObject.name);
        if (collision.gameObject.name == "Snitch")
        {
            game.SendMessage("pointForGriffindor");

        }
        else if (collision.gameObject.name == "S_player")
        {
            double aProb = random.NextDouble();
            if (aProb < probTackle)
            {
                collision.gameObject.SendMessage("wasHitByOpponent");
            }
        } else if (collision.gameObject.name == "Field")
        {
            transform.position = getStartPosition();
            transform.LookAt(snitch.transform);
            body.AddRelativeForce(Vector3.forward * acceleration, ForceMode.Acceleration);
            wasHit = false;

        }
    }

    private Vector3 getStartPosition()
    {
        
        return new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, startPoint.transform.position.z);
    }

    private void wasHitByOpponent()
    {
        wasHit = true;
    }

}
