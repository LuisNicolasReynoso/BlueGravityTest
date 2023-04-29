using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform position;

    public GameObject messagePref;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            GameObject NewMessage = (GameObject)Instantiate(messagePref);
            NewMessage.transform.SetParent(this.transform);

            Message message = NewMessage.GetComponent<Message>();
            message.SetMessage("Hello", position);
        }
    }
}
