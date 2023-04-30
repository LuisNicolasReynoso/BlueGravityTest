using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance = null;

    public List<Clip> Clips = new List<Clip>();
 


    AudioSource Source;
    void Awake() //Create Singleton
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this)
            Destroy(gameObject);
        


    }

    // Start is called before the first frame update
    void Start()
    {
        Source = this.GetComponent<AudioSource>();
    }

    public void PlaySound(int ID)
    {
        Clip clip = SearchClipByID(ID);

        Source.volume = clip.Volume;
        Source.PlayOneShot(clip.AudioClip[0]);
    }

    public void PlayRandomSound(int ID)
    {
        Clip clip = SearchClipByID(ID);

        Source.volume = clip.Volume;
        Source.PlayOneShot(clip.AudioClip[Random.Range(0, clip.AudioClip.Length)]);
    }

    Clip SearchClipByID(int ID)
    {
        Clip clip = Clips[0];
        foreach(Clip C in Clips)
        {
            if(C.ID == ID)
            {
                clip = C;
            }
        }

        return clip;
    }

   
}
