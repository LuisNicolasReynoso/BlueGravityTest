using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpell : MonoBehaviour
{
    // Start is called before the first frame update

    Transform Character;
    Rigidbody2D rig;

    float distanceTreshold = 1f;

    public Transform FireSprite;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        Character = GameManager.Instance.Player.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(Character.position, this.transform.position);
        Vector3 direction = this.transform.position - Character.transform.position;


        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        FireSprite.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (distance > distanceTreshold)
        {
            
            //FireSprite.LookAt(Character.transform);
           
            

            

            rig.AddForce(direction.normalized * (-5 * distance), ForceMode2D.Force);
        }
       
     
    }
}
