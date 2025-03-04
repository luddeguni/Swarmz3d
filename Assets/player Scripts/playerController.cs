using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BasicPlayerController : MonoBehaviour
{
    /* 
    Create a variable called 'rb' that will represent the 
    rigid body of this object.
    */
    private Rigidbody rb;

    // Create a public variable for the cameraTarget object
    public GameObject cameraTarget;
    /* 
    You will need to set the cameraTarget object in Unity. 
    The direction this object is facing will be used to determine
    the direction of forces we will apply to our player.
    */
    public float movementSpeed;
    /* 
    Creates a public variable that will be used to set 
    the movement intensity (from within Unity)
    */


    public int health = 100;
    public int mana = 20;
    public int atkSpeed = 1;
    public int projectiles = 1;
    public int crittChance = 0;

    void Start()
    {
        // make our rb variable equal the rigid body component
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        /* 
    	Establish some directions 
    	based on the cameraTarget object's orientation 
    	*/
        var ForwardDirection = cameraTarget.transform.forward;
        var RightDirection = cameraTarget.transform.right;

        // Move Forwards
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = ForwardDirection * movementSpeed;
            /* You may want to try using velocity rather than force.
            This allows for a more responsive control of the movement
            possibly better suited to first person controls, eg: */
            //rb.velocity = ForwardDirection * movementIntensity;
        }
        // Move Backwards
        if (Input.GetKey(KeyCode.S))
        {
            // Adding a negative to the direction reverses it
            rb.velocity = -ForwardDirection * movementSpeed;
        }
        // Move Rightwards (eg Strafe. *We are using A & D to swivel)
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = RightDirection * movementSpeed;
        }
        // Move Leftwards
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = -RightDirection * movementSpeed;
        }
    }
}