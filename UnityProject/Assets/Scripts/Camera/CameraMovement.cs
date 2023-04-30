using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    [Space(10)]
    [Header("Movement")]
    [HideInInspector]
    public GameObject cameraTarget;
    [HideInInspector]
    public float smoothTime = 0.1f;
  
    Vector2 velocity;

    [HideInInspector]
    public bool ShowMovement;

    private Transform thisTransform;
    Camera thisCamera;

    [Space(10)]
    [Header("Base Parameters")]

    [HideInInspector]
    public float baseOffset = 5;
    [HideInInspector]
    public bool ShowPos;
    [HideInInspector]
    public float BaseAngle = 50;



    [Space(10)]
    [Header("Zoom Values")]

    [HideInInspector]
    public float zoomInOffset = 10;
    [HideInInspector]
    public float ZoomSteps = 10;
    [HideInInspector]
    public bool ZoomingIn;

    [HideInInspector]
    public bool ShowZoom; 
    
  
    [Header("Debug Only")]
    [Space(30)]
    [HideInInspector]
    public float CurrentOffset;
    [HideInInspector]
    public float ZoomPercentage;
    [HideInInspector]
    public bool ShowDebug;   


 
    void Start()
    {
       
        thisTransform = transform;       
        thisCamera = this.GetComponent<Camera>();        
        CurrentOffset = baseOffset;
       

        if (cameraTarget != null)
        {
            this.transform.position = cameraTarget.transform.position;
        }
        else
        {
            cameraTarget = GameManager.Instance.Player;
        }

       
    }

    private void Update()
    {
        if (cameraTarget != null)
        {
            Zoom();
           
        }     

    }


    void FixedUpdate() //Normal camera movement is done in fixedUpdate to prevent jitter
    {
        if (cameraTarget != null)
        {
            MoveCamera();
        }

    }
    

    void MoveCamera() //Follow character smoothly
    {
        float offset = CurrentOffset;        
        Vector3 TargetPos = cameraTarget.transform.position;     

        //Lerp character position and camera position
        Vector3 cameraPos = Vector3.Lerp(transform.position, TargetPos, smoothTime);

        //Only apply X and Y movement
        transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);

        //Apply Current zoom values
        thisCamera.orthographicSize = offset;
       
    }


    void Zoom()
    {
        if (Input.GetAxis("Zoom") != 0)
        {
            if (Input.GetAxis("Zoom") > 0)
            {
                ZoomOut();
              
            }
            else
            {
                ZoomIn();
            }

        }

    }

    void ZoomIn()
    {
        //If zoom direction changes, stop the CalculateZoom coroutine and start a new one in the new direction
        if (!ZoomingIn) 
        {
            ZoomingIn = true;
            StopAllCoroutines();
        }
       

        StartCoroutine(CalculateZoom(zoomInOffset, true));

    }


    void ZoomOut()
    {
        //If zoom direction changes, stop the CalculateZoom coroutine and start a new one in the new direction
        if (ZoomingIn)
        {
            ZoomingIn = false;
            StopAllCoroutines();
        }

        StartCoroutine(CalculateZoom(baseOffset, false));
    }

    IEnumerator CalculateZoom(float Size, bool ZoomIn)
    {
        //Use a certain number of steps to make zooming smoothly using mouse wheel
        int steps = (int)ZoomSteps;       

        while (steps > 0)
        {
            //Calculate the diference between current zoom level and the maximum/minimum allowed zoom
            float Delta = Mathf.Abs(Size-CurrentOffset);

            if (Delta > .01f)
            {
                if(ZoomIn)
                {
                    CurrentOffset = CurrentOffset + .01f;
                }
                else
                {
                    CurrentOffset = CurrentOffset - .01f;
                }
                
            }
            else
            {
                CurrentOffset = Size;
            }


            calcualteZoomPercentage();


           
            steps = steps - 1;
            yield return new WaitForSeconds(0.016f);
        }

    }

    void calcualteZoomPercentage()
    {
        //Calculate how much zoomed is the camera, in a percentage from 0% to 100%, used for debugging
        ZoomPercentage = map(CurrentOffset, baseOffset, zoomInOffset, 0, 100);

    }


    float map(float s, float a1, float a2, float b1, float b2) //Transform a range of values into a new range of values based on a parameter
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}