using UnityEngine;

/**
 * Directed movement for the Snitch in the Quidditch game
 */
public class GoldenSnitch: MonoBehaviour {
    private float velocity;
    private Vector3 currentDirection;
    private float acceleration;
    System.Random random;
    int frames = 0;          // counter for how often snitch will change directions
    int maxFrames = 15;      // number of frames before snitch will change directions

    public Rigidbody body;

    // Called before Start()
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Initialize variables
    void Start() {
        // Have snitch detect collisions (e.g. with boundaries of the playing field)
        body.detectCollisions = true;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        velocity = 16.0f;              // Max Velocity
        acceleration = 20.0f;          // Max Acceleration force
        
        random = new System.Random();
        currentDirection = new Vector3();

        // Have snitch start moving in a random direction
        changeToRandomDirection();
        body.velocity = transform.forward * velocity;
        body.AddForce(currentDirection * acceleration, ForceMode.VelocityChange);


    }
    
    // Update is called once per frame to check if snitch should change directions
    void Update () {
        if (frames == maxFrames)
        {
            changeToRandomDirection();
            body.AddForce(currentDirection * acceleration, ForceMode.VelocityChange);
            frames = 0;
        }
        frames++;
    }

    // Set new direction for snitch
    void changeToRandomDirection()
    {
        currentDirection[0] = (float) random.NextDouble();
        currentDirection[1] = (float) random.NextDouble();
        currentDirection[2] = (float) random.NextDouble();
        currentDirection = currentDirection.normalized;
        //print("direction : " + currentDirection.ToString());
    }


    // If snitch collides with the playing field or a player,
    // or tries to exit the boundaries of the field, then
    // have the snitch move in a different direction upon collision
    void OnCollisionEnter(Collision c)
    {
        
        if (c.gameObject.name == "Field" 
            || c.gameObject.name.Contains("Wall") 
            || c.gameObject.name.Contains("Player"))
        {
            
            int newY = random.Next(-1, 1); 

            // Get the initial direction
            Vector3 newDirection = transform.position - c.transform.position;
            // Change to the opposite (Vector3) direction and normalize it
            newDirection = newDirection.normalized;

            // If the snitch hits the field and the new direction is towards the field
            // then inverse it
            newDirection[1] = newY;
            if (newDirection[1] == 0 && transform.position[2] == 0)
            {
                newDirection[1] = 1;
            }

            // Push the snitch back from the boundary/field
            body.AddForce(newDirection * acceleration, ForceMode.VelocityChange);
        }
    }

}
