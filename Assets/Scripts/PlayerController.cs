using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private variables
    private Rigidbody2D playerRB;
    private float accel;
    private float decel;

    //public variables
    public float maxSpeed;
    public float accelTime;
    public float decelTime;
    public LayerMask lM;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //set up the Rigidbody2D for functions

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

        Vector2 currentPos = transform.position;
        Vector2 endPos = new Vector2(currentPos.x, currentPos.y - 0.7f);
        Debug.DrawLine(currentPos, endPos, Color.red);

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
        } if (Input.GetKey(KeyCode.Space))
        {
            playerVelocity += accel * Vector2.up * Time.deltaTime;
        }




        playerRB.velocity = playerVelocity;
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
        Vector2 rayStart = transform.position;
        Vector2 rayEnd = new Vector2(transform.position.x, transform.position.y - 0.7f);
        bool hitGround = Physics2D.Raycast(rayStart, rayEnd, 1.1f, lM);

        if (hitGround == true)
        {
            return true;
        } else
        {
            return false;
        }
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
