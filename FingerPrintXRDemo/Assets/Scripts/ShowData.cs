using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static System.Net.Mime.MediaTypeNames;


public class ShowData : MonoBehaviour
{
    public TMP_Text indexForceText, thumbForceText, torqueText, torqueAngleText, forceTitle, torqueTitle, torqueAngleTitle; // These need to be linked in the editor

    GameObject index, thumb, ftInfo;

    // Start is called before the first frame update
    void Start()
    {
        // Start the hand behavior/game logic as disabled until serial comms is up
        index = GameObject.Find("Index");
        thumb = GameObject.Find("Thumb");
        ftInfo = GameObject.Find("FTMainInfo");

        // Optionally, you can find the TMP components here if not linked in the inspector:
        indexForceText = GameObject.Find("IndexForceTextObject").GetComponent<TMP_Text>();
        thumbForceText = GameObject.Find("ThumbForceTextObject").GetComponent<TMP_Text>();

        torqueText = GameObject.Find("TorqueTextObject").GetComponent<TMP_Text>();
        torqueAngleText = GameObject.Find("TorqueAngleTextObject").GetComponent<TMP_Text>();
        
        
        forceTitle = GameObject.Find("ForceTitleObject").GetComponent<TMP_Text>();
        torqueTitle = GameObject.Find("TorqueTitleObject").GetComponent<TMP_Text>();
        torqueAngleTitle = GameObject.Find("TorqueAngleTitleObject").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 indexForce = index.GetComponent<FingerProxy>().force;
        Vector3 thumbForce = thumb.GetComponent<FingerProxy>().force;
        float indexTorque = index.GetComponent<FingerProxy>().torqueMag;
        float thumbTorque = thumb.GetComponent<FingerProxy>().torqueMag;
        float indexTorqueAngle = index.GetComponent<FingerProxy>().angleChange;
        float thumbTorqueAngle = thumb.GetComponent<FingerProxy>().angleChange;

        indexForceText.text = "Index:\nX: " + indexForce.x.ToString("0.00") + " N\nY: " + indexForce.y.ToString("0.00") + " N\nZ: " + indexForce.z.ToString("0.00") + " N";
        thumbForceText.text = "Thumb:\nX: " + thumbForce.x.ToString("0.00") + " N\nY: " + thumbForce.y.ToString("0.00") + " N\nZ: " + thumbForce.z.ToString("0.00") + " N";
        torqueText.text = "Index: " + indexTorque.ToString("0.00") + " Nm\nThumb: " + thumbTorque.ToString("0.00") + " Nm";
        torqueAngleText.text = "Index: " + indexTorqueAngle.ToString("0.00") + " deg\nThumb: " + thumbTorqueAngle.ToString("0.00") + " deg";

        TMP_Text newText = ftInfo.GetComponent<TMP_Text>(); // Fetch the TMP_Text component
        newText.text = forceTitle + indexForceText.text + thumbForceText.text + torqueTitle + torqueText.text + torqueAngleTitle + torqueAngleText.text; // Update the text on the panel
    }
}
