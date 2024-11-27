using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private variables
    private Rigidbody2D playerRB;
    private float accel;
    private float decel;
    private float playerGravity;
    private float jumpTimer; //When a jump started
    private bool isJumping; //flag to check if player is jumping
    private float coyoteFloat; //amount of time since player has been off the ground to compare against coyoteTime

    //public variables
    public float maxSpeed;
    public float accelTime;
    public float decelTime;
    public LayerMask lM;
    public float apexHeight;
    public float apexTime;
    public float terminalSpeed;
    public float coyoteTime;
    public float overlapRadius; //radius of the overlap circle for detecting ground

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //set up the Rigidbody2D for functions
        playerGravity = playerRB.gravityScale; //note the inital gravity when the program starts

        //set up acceleration and deceleration equations
        decel = maxSpeed / decelTime;
        accel = maxSpeed / accelTime;
    }


    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);

        
        /* Saving these for later if I need them
        //setting up lines to represent my raycasts for a better visual indicator while testing.
        Vector2 currentPos1 = transform.position;
        Vector2 endPos1 = new Vector2(currentPos1.x, currentPos1.y - 1.1f);
        Debug.DrawLine(currentPos1, endPos1, Color.red);

        Vector2 currentPos2 = (Vector2)transform.position + (Vector2.left * 0.3f);
        Vector2 endPos2 = new Vector2(currentPos2.x, currentPos2.y - 1.1f);
        Debug.DrawLine(currentPos2, endPos2, Color.red);

        Vector2 currentPos3 = (Vector2)transform.position + (Vector2.right * 0.3f);
        Vector2 endPos3 = new Vector2(currentPos3.x, currentPos3.y - 1.1f);
        Debug.DrawLine(currentPos3, endPos3, Color.red); */
        
        

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 playerVelocity = playerRB.velocity; //Determine the player's initial velocity

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            playerVelocity += accel * Vector2.left * Time.deltaTime; //Move left if the left arrow or A is pressed
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            playerVelocity += accel * Vector2.right * Time.deltaTime; //Move right if the right arrow or D is pressed
        } 
        if ((Input.GetKeyDown(KeyCode.Space) && !isJumping && (coyoteFloat <= coyoteTime || IsGrounded()))) //GetKeyDown rather than just GetKey because we don't want the player to be able to hold the spacebar and fly
                                                                                                          // Also checks that player hasn't been off the ground without a jump for longer than coyoteTime seconds
        {
            isJumping = true;
            playerRB.gravityScale = 0f; //set gravity to 0 while jumping
            float vertVelocity = (2 * apexHeight) / apexTime; //using the Initial Jump Velocity equation from Week 11's lecture notes.
            playerVelocity = new Vector2(playerVelocity.x, vertVelocity); //Jump
            jumpTimer = Time.time; //start recording the time that the jump has been happening

            if (!IsGrounded())
            {
                coyoteFloat = 0f;
            }
        }
        if (isJumping)
        {
            float timePassed = Time.time - jumpTimer; //record how much time has passed since player started jumping.

            if (timePassed >= apexTime) //if the player's been jumping longer than their apex time, they start to fall
            {
                isJumping = false;
                playerRB.gravityScale = playerGravity;
                               
            }
        }
        if (playerRB.velocity.y < 0 && playerRB.velocity.y < terminalSpeed) //if player is falling faster than terminalSpeed, they fall at terminalSpeed instead.
        {
            playerVelocity = new Vector2(playerRB.velocity.x, terminalSpeed);
        }

        playerRB.velocity = playerVelocity; //return to neutral
    }

    public bool IsWalking()
    {
        if (Mathf.Abs(playerRB.velocity.x) > 0.1f)
        {
            return true;
        } else
        {
            return false;
        }
                    
    }
    public bool IsGrounded()
    {
        //Set up three raycasts, on in the center of the sprite, one on the left, one on the right
        /*Vector2 rayStart1 = transform.position;
        Vector2 rayEnd1 = new Vector2(transform.position.x, transform.position.y - 0.7f);
        bool hitGround1 = Physics2D.Raycast(rayStart1, rayEnd1, 1.1f, lM);

        Vector2 rayStart2 = (Vector2)transform.position + (Vector2.left * 0.3f);
        Vector2 rayEnd2 = new Vector2(rayStart2.x, rayStart2.y - 0.7f);
        bool hitGround2 = Physics2D.Raycast(rayStart2, rayEnd2, 1.1f, lM);

        Vector2 rayStart3 = (Vector2)transform.position + (Vector2.right * 0.3f);
        Vector2 rayEnd3 = new Vector2(rayStart3.x, rayStart3.y - 0.7f);
        bool hitGround3 = Physics2D.Raycast(rayStart3, rayEnd3, 1.1f, lM);*/

        //Set up OverlapCircle
        bool hasGround = Physics2D.OverlapCircle(transform.position, overlapRadius, lM);
        coyoteFloat = 0f;
        if (coyoteFloat < coyoteTime)
        {
            coyoteFloat += Time.deltaTime;
        }
        if (hasGround)
        {
            return true;
        }
        else
        {
            return false;
        }



        /*if (hitGround1 == true || hitGround2 == true || hitGround3 == true)
        {
            return true;
        } else
        {
            return false;
        }*/


    }

    public FacingDirection GetFacingDirection()
    {
        if (playerRB.velocity.x > 0.1f)
        {
            return FacingDirection.right;
        } else
        {
            return FacingDirection.left;
        }
    }
}
