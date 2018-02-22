using UnityEngine;

/**
 * Directed movement for the Snitch in the Quidditch game
 */
public class GoldenSnitch: MonoBehaviour {
    // player's qualities
    [Range(1.0f, 16.0f)]
    public float velocity;
    [Range(1.0f, 20.0f)]
    public float acceleration;
    private System.Random random;

    private Vector3 currentDirection;
    private GameObject field;
    private GameObject ceiling;


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

        velocity = 16.0f;              // Initial Velocity
        acceleration = 20.0f;          // Initial Acceleration force

        body.velocity = Vector3.ClampMagnitude(body.velocity, 20);
        
        random = new System.Random();
        currentDirection = new Vector3();

        // Have snitch start moving in a random direction
        changeToRandomDirection();
        body.velocity = transform.forward * velocity;
        body.AddForce(currentDirection * acceleration, ForceMode.VelocityChange);

        field = GameObject.Find("Field");
        ceiling = GameObject.Find("Wall5");
    }
    
    // Update is called once per frame to check if snitch should change directions
    void FixedUpdate () {
            changeToRandomDirection();
            body.AddForce(currentDirection * acceleration, ForceMode.Acceleration);
    }

    // Set new direction for snitch
    void changeToRandomDirection()
    {
        currentDirection[0] = (float) random.NextDouble();
        currentDirection[1] = ((random.NextDouble() > 0.5f) ? -1 : 1 );
        currentDirection[2] = (float) random.NextDouble();
        currentDirection = currentDirection.normalized;
        //print("direction : " + currentDirection.ToString());
    }


    // If snitch collides with the playing field or tries to exit the boundaries 
    // of the field, then have the snitch move in a different direction upon collision
    void OnCollisionEnter(Collision c)
    {
        // if snitch collides with the field or a wall boundary, bounce off in a different direction
        if (c.gameObject.name == "Field" 
            || c.gameObject.name.Contains("Wall"))
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

        // if snitch collides with a player then respawn in random location
        } else if (c.gameObject.name.Contains("Player"))
        {
            float x = (float) random.NextDouble() * field.transform.position.x;
            float z = (float) random.NextDouble() * field.transform.position.z;
            float y = (float) random.NextDouble() * ceiling.transform.position.y;

            transform.position = new Vector3(x, y, z);

        }
    }

}
