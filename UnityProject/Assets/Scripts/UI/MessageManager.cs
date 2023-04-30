using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform position;

    public GameObject messagePref;

    Message characterMessage;
    Message npcMessage;
    void Start()
    {
        
    }
   
 

    public void SpawnMessage(string text, Transform position, bool character)
    {
        if(character)
        {
            if(characterMessage == null)
            {
                characterMessage = CreateMessage(text, position);
            }
        }
        else
        {
            if(npcMessage == null)
            {
                npcMessage = CreateMessage(text, position);
            }
        }
       
    }

    Message CreateMessage(string text, Transform position)
    {
        GameObject NewMessage = (GameObject)Instantiate(messagePref);
        NewMessage.transform.SetParent(this.transform);

        Message message = NewMessage.GetComponent<Message>();
        message.SetMessage(text, position);

        return message;
    }
}
