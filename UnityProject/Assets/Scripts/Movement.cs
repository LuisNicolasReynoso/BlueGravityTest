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
        float InputX = Input.GetAxis("Horizontal");
        float InputY = Input.GetAxis("Vertical");

        Vector2 Direction = GetDirectionInput(InputX, InputY);
       

        if (Direction != Vector2.zero)
        {
            anim.SetBool("Moving", true);
            anim.SetFloat("X", Direction.normalized.x);
            anim.SetFloat("Y", Direction.normalized.y);
            rig.velocity = Direction * MovementSpeed;
        }
        else
        {
            anim.SetBool("Moving", false);
        }

        
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
