using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDustEffect : MonoBehaviour
{
    //Variables that hold the different dust effect objects
    [SerializeField] private GameObject dustEffect;
    [SerializeField] private GameObject fallDustEffect;

    //Holds the ground layer value
    [SerializeField] private LayerMask GroundLayer;

    //Unity components
    private BoxCollider2D boxColliderComponent;
    private Rigidbody2D rigidbodyComponent;
    private SpriteRenderer spriteComponent;

    //Vector3 variables determine the position of each of the dust effects on the player
    private Vector3 dustPosition;

    //Counter used to measure the delay between each dust particle
    private float dustCounter;
    
    //Values that determine the time between each dust effect
    [SerializeField] private float dustCreationDelay;

    //Determines the speed at which dust will start to be created
    [SerializeField] private float dustVelocityDelay;

    private PlayerController playerController;
    [SerializeField] private float dustOffset;


    // Start is called before the first frame update
   private void Start()
    {
        playerController = GetComponent<PlayerController>();
        spriteComponent = GetComponent<SpriteRenderer>();
        boxColliderComponent = GetComponent<BoxCollider2D>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
       
    }
    private void Update()
    {
        //Counter is incremented with the current time
        dustCounter += Time.deltaTime;
        //Dust position is set to be just behind the player so that the dust appears whilst running
        dustPosition = new Vector3(transform.position.x, transform.position.y - dustOffset);

        //Allows floor and wall dust to be created if the player is still alive
        CreateDust();
     
    }


    private void CreateDust()
    {
        //checks if the player's movement exceeds the required velocity for dust to be generated, also checks if the player is on the ground
        if(Mathf.Abs(rigidbodyComponent.velocity.x) > dustVelocityDelay && IsGrounded())
        {
            //Checks if there has been enough time from the last dust particle
            if(dustCounter > dustCreationDelay)
            {
                //Creates the dust particle at the correct position and resets the counter
                Instantiate(dustEffect, dustPosition, Quaternion.identity);
                dustCounter = 0;
            }
            
        }
    }

    public void CreateJumpDust()
    {
       //Creates dust just below the player
       Instantiate(fallDustEffect, dustPosition, Quaternion.identity);
    }

    private bool IsGrounded()
    {
        //Returns true if the box cast overlaps within 0.1f of the ground layer
        return Physics2D.BoxCast(boxColliderComponent.bounds.center, boxColliderComponent.bounds.size, 0f, Vector2.down, .1f, GroundLayer);
    }

}
