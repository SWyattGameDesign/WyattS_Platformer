using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceForce; // How hard the trampoline bounces the player
    public float minimumBounce;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D playerRB = collision.collider.GetComponent<Rigidbody2D>(); //read the player's rigidbody so that the trampoline can interact with it.
        if (playerRB != null )
        {
            float bounceLaunch = Mathf.Abs(playerRB.velocity.y) * bounceForce; //Multiply the vertical motion of the player by the bounceForce

            if (bounceLaunch < minimumBounce)
            {
                bounceLaunch = minimumBounce;
            }

            playerRB.velocity = new Vector2(playerRB.velocity.x, bounceLaunch);
        }
    }
}
