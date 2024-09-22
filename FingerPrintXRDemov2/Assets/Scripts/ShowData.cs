using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class ShowData : MonoBehaviour
{
    public TMP_Text forceText, torqueText; // These need to be linked in the editor

    GameObject index, thumb, ftInfo;

    // Start is called before the first frame update
    void Start()
    {
        // Start the hand behavior/game logic as disabled until serial comms is up
        index = GameObject.Find("Index");
        thumb = GameObject.Find("Thumb");
        ftInfo = GameObject.Find("FTMainInfo");

        // Optionally, you can find the TMP components here if not linked in the inspector:
        forceText = GameObject.Find("ForceTextObject").GetComponent<TMP_Text>();
        torqueText = GameObject.Find("TorqueTextObject").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 index_force = index.GetComponent<FingerProxy>().force;
        Vector3 thumb_force = thumb.GetComponent<FingerProxy>().force;
        float index_torque = index.GetComponent<FingerProxy>().torqueMag;
        float thumb_torque = thumb.GetComponent<FingerProxy>().torqueMag;

        forceText.text = "Index Force: " + index_force + "\nThumb Force: " + thumb_force;
        torqueText.text = "Index Torque: " + index_torque + "\nThumb Torque: " + thumb_torque;

        TMP_Text newText = ftInfo.GetComponent<TMP_Text>(); // Fetch the TMP_Text component
        newText.text = forceText.text + "\n" + torqueText.text; // Update the text on the panel
    }
}
