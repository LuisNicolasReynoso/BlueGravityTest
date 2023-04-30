using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rig;
    [SerializeField]
    private float MovementSpeed;

    Animator anim;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        //Get Input
        float InputX = Input.GetAxis("Horizontal");
        float InputY = Input.GetAxis("Vertical");

        //Calculate Direction
        Vector2 Direction = GetDirectionInput(InputX, InputY);


        //Adapt direction values to animation values
        float animationX = Direction.normalized.x;
        float animationY = Direction.normalized.y;

        if(animationX != 0 && animationY != 0)
        {
            animationX = 0;

            if(animationY >0)
            {
                animationY = 1;
            }
            else if(animationY<0)
            {
                animationY = -1;
            }    
        }

        //Apply force in the direction calculated and set animation parameters
        if (Direction != Vector2.zero)
        {
            anim.SetBool("Moving", true);
            anim.SetFloat("X", animationX);
            anim.SetFloat("Y", animationY);
            rig.velocity = Direction * MovementSpeed;
        }
        else        {
            rig.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }

        
    }

    public void StepSFX() //Called from animation
    {
        AudioManager.Instance.PlayRandomSound(5);
    }



    Vector2 GetDirectionInput(float X, float Y)
    {
        Vector2 Direction = new Vector2(0, 0);

        if (X != 0)
        {  
            Direction += Vector2.right * X;
        }

        if (Y != 0)
        {   
            Direction += Vector2.up * Y;
        }


        return Direction;
    }
}
