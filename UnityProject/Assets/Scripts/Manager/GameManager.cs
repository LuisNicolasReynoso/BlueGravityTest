using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;

    public static GameManager Instance = null;

    public MessageManager messageManager;

    
    void Awake() //Create Singleton
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);


    }

    
}
