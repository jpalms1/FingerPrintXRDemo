using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedback : MonoBehaviour
{


    // Audio 
    public GameObject player;

    public AudioSource audioSource;
    public AudioClip lowBeepAudio;
    public AudioClip highBeepAudio;
    public float maxDepth = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        audioSource = player.GetComponent<AudioSource>();
        lowBeepAudio = Resources.Load<AudioClip>("Audio/lowBeep");
        highBeepAudio = Resources.Load<AudioClip>("Audio/highBeep");
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    void playAudio(Collider hapticObject)
    {
        //UnityEngine.Debug.Log(" Collision detected with: " + hapticLabel);
        if (hapticObject.tag == "Haptic Anatomy High Beep")
        {
            if (!audioSource.isPlaying)
            {
                UnityEngine.Debug.Log("Playing HIGH sound.");
                //audioSource.Play();
                audioSource.PlayOneShot(highBeepAudio, 0.7f);
            }
        }        
        if (hapticObject.tag == "Haptic Anatomy Low Beep")
        {
            if (!audioSource.isPlaying)
            {
                UnityEngine.Debug.Log("Playing HIGH sound.");
                //audioSource.Play();
                audioSource.PlayOneShot(lowBeepAudio, 0.7f);
            }
        }

    }

    void stopAudio()
    {
        //if (collision.gameObject.tag == hapticLabel)
        //{
        //UnityEngine.Debug.Log("Exiting collision with: " + hitInfo2.collider.name);
        //audioSource.Stop();
        //}
    }
    
}
