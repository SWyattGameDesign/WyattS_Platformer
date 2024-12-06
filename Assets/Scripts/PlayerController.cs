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
    private bool isJumping = false; //flag to check if player is jumping
    private float coyoteFloat; //amount of time since player has been off the ground to compare against coyoteTime
    private float jumpBufferTime = 0.2f; // How much time to buffer the jump input
    private float jumpBufferCount = 0f;
    private bool isDashing; //check if player is dashing
    private float dashBufferTime = 0.2f; // How much to buffer the dash input
    private float dashBufferCount = 0f; 

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
    public int health = 10;
    public float dashSpeed; //Speed of the dash
    public float dashDuration; //how long the dash should go on for
    public float dashTime; //How long the dash has been going

    public enum FacingDirection
    {
        left, right
    }

    public FacingDirection currentFacingDirection = FacingDirection.right;

    public enum CharacterState
    {
        idle, walk, jump, die
    }

    public CharacterState currentCharacterState = CharacterState.idle;
    public CharacterState previousCharacterState = CharacterState.idle;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //set up the Rigidbody2D for functions
        playerGravity = playerRB.gravityScale; //note the inital gravity when the program starts

        //set up acceleration and deceleration equations
        decel = maxSpeed / decelTime;
        accel = maxSpeed / accelTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCount = jumpBufferTime;
        }
        if (jumpBufferCount > 0)
        {
            jumpBufferCount -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashBufferCount = dashBufferTime;
        }
        if (dashBufferCount > 0)
        {
            jumpBufferCount -= Time.deltaTime;
        }

            previousCharacterState = currentCharacterState;
        switch (currentCharacterState)
        {
            case CharacterState.die:

                break;
            case CharacterState.jump:
                if (IsGrounded())
                {

                    if (IsWalking())
                    {
                        currentCharacterState = CharacterState.walk;
                    }
                    else
                    {
                        currentCharacterState = CharacterState.idle;
                    }
                }
                break;
            case CharacterState.walk:
                //Are we walking
                if (!IsWalking())
                {
                    currentCharacterState = CharacterState.idle;
                }
                //Are we jumping
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }

                break;
            case CharacterState.idle:
                //Are we walking
                if (IsWalking())
                {
                    currentCharacterState = CharacterState.walk;
                }
                //Are we jumping
                if (!IsGrounded())
                {
                    currentCharacterState = CharacterState.jump;
                }
                break;
        }

    }


    private void FixedUpdate()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2();

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            playerInput += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            playerInput += Vector2.right;
        }


        MovementUpdate(playerInput);

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        Vector2 playerVelocity = playerRB.velocity; //Determine the player's initial velocity

        if (playerInput.x != 0)
        {
            playerVelocity += playerInput * accel * Time.fixedDeltaTime;
        }
        else
        {
            playerVelocity = new Vector2(0, playerVelocity.y);
        }
        if (dashBufferCount > 0 && !isDashing)
        {
            isDashing = true; //Player has started dashing
            dashBufferCount = 0f; //Reset Dash buffer
            dashTime = dashDuration; //start recording the time of the dash
            playerRB.gravityScale = 0f; //Turn off player gravity
            playerVelocity = new Vector2((playerVelocity.x * dashSpeed), playerVelocity.y); //Make the player rush

            
            if (isDashing)
            {
                dashTime -= dashDuration;

                if (dashTime <= 0f)
                {
                    isDashing = false;
                    playerRB.gravityScale = playerGravity; //restore the gravity of the player
                   if (playerVelocity.x > maxSpeed)
                    {
                        playerVelocity.x = maxSpeed; //put player back to their normal max speed rather than the dash speed.
                    }
                }
            }
        }
        if (jumpBufferCount > 0 && !isJumping && (coyoteFloat <= coyoteTime || IsGrounded())) //GetKeyDown rather than just GetKey because we don't want the player to be able to hold the spacebar and fly
                                                                                                            // Also checks that player hasn't been off the ground without a jump for longer than coyoteTime seconds
        {
            isJumping = true;
            jumpBufferCount = 0f; //reset the jump buffer
            playerRB.gravityScale = 0f; //set gravity to 0 while jumping
            float vertVelocity = (2 * apexHeight) / apexTime; //using the Initial Jump Velocity equation from Week 11's lecture notes.
            playerVelocity = new Vector2(playerVelocity.x, vertVelocity); //Jump
            jumpTimer = Time.time; //start recording the time that the jump has been happening

        }
        if (isJumping)
        {

            float timePassed = Time.time - jumpTimer; //record how much time has passed since player started jumping.

            if (Input.GetKeyUp(KeyCode.Space) && timePassed < apexTime)
            {
                isJumping = false; //turn off the jumping flag
                playerRB.gravityScale = playerGravity; //reset gravity
                if (playerVelocity.y > 0)
                {
                    playerVelocity.y *= 0.5f; //reduce player's upward velocity
                }
            }
            

            if (timePassed >= apexTime) //if the player's been jumping longer than their apex time, they start to fall
            {
                isJumping = false;
                playerRB.gravityScale = playerGravity;
               
            }
        }
        if (playerRB.velocity.y < 0 && playerRB.velocity.y < terminalSpeed) //if player is falling faster than terminalSpeed, they fall at terminalSpeed instead.
        {
            playerVelocity = new Vector2(playerRB.velocity.x, terminalSpeed); //Player falls at terminal speed
            //Debug.Log("Player is falling at " + playerVelocity.y); //Test to make sure player's downward velocity doesn't exceed terminalSpeed
        }
        if (!IsGrounded() && !isJumping)
        {
            isJumping = false;
        }


        playerRB.velocity = playerVelocity; //return to neutral
    }

    public bool IsWalking()
    {
        if (Mathf.Abs(playerRB.velocity.x) > 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool IsGrounded()
    {
        coyoteFloat = 0f;
        //Set up OverlapCircle
        bool hasGround = Physics2D.OverlapCircle(transform.position, overlapRadius, lM);

        if (coyoteFloat < coyoteTime)
        {
            coyoteFloat += Time.deltaTime;
        }

        if (hasGround)
        {
            //Debug.Log("Player is Grounded"); //Check to see if player is grounded (Debug purposes)
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public void OnDeathAnimationComplete()
    {
        gameObject.SetActive(false);
    }

    public FacingDirection GetFacingDirection()
    {
        if (playerRB.velocity.x > 0f)
        {
            currentFacingDirection = FacingDirection.right;
        }
        else if (playerRB.velocity.x < 0f)
        {
            currentFacingDirection = FacingDirection.left;
        }
        return currentFacingDirection;
    }
}
