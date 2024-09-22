/*
  * Serial Communication from Unity to microcontroller controlled haptic devices 
  * Code written by Jasmin E. Palmer
  * Email: jasminp@standford.edu
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Diagnostics;

using Debug = UnityEngine.Debug;
using static System.Net.Mime.MediaTypeNames;
using System;

public class SerialCommsArduino : MonoBehaviour
{
    GameObject toolTip, toolTip1, toolTip2;

    //Set the port and the baud rate to 9600
    public string portName = "COM9";
    //public int baudRate = 115200;
    public int baudRate = 9600;
    SerialPort stream;

    private float lastTime = 0.0f;
    private float currentTime = 0.0f;

    public static string[] arduinoDataVals;
    public static float[] unityDataVals;

    public int expectedUnityEntries;

    private List<string[]> arduinoDataList;
    private List<float[]> unityDataList;

    private string oldMessage = "0.00 0.00 0.00 0.00 0.00\n";

    //public bool toolTip1MoveBool;
    //public bool toolTip2MoveBool;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start Serial Comms");
        //initialize lists
        arduinoDataList = new List<string[]>();
        unityDataList = new List<float[]>();

        //Start the hand behavior/game logic as disabled until serial comms is up
        toolTip = GameObject.Find("ToolTip1");
        //toolTip1 = GameObject.Find("ToolTip1");
        //toolTip2 = GameObject.Find("ToolTip2");
        print("ToolTips found");

        //Define and open serial port       
        stream = new SerialPort(portName, baudRate);
        stream.Open();
        // stream.DiscardInBuffer();
        // stream.DiscardOutBuffer();

        Debug.Log("<color=green>Serial Communication Established</color>");

        //Serial Port Read and Write Timeouts
        stream.ReadTimeout = 5;
        stream.WriteTimeout = 10;

        //Enable Game Logic
        //GetComponent<HapticController>().enabled = true;
        Debug.Log("<color=blue>Haptic Controller Enabled</color>");

        //writeSerial("0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00");
        writeSerial("0.00");
        stream.Close();
        // readSerial();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        toolTip1MoveBool = false;
        toolTip2MoveBool = false;

        if (toolTip1.transform.hasChanged)
        {
            toolTip1MoveBool = true;
        }
        if (toolTip2.transform.hasChanged)
        {
            toolTip2MoveBool = true;
        }

        //toolTip1MoveBool = toolTip1.transform.hasChanged;
        //toolTip2MoveBool = toolTip2.transform.hasChanged;

        */

        //Debug.Log("SerialComms.cs");
        if (stream.IsOpen || !stream.IsOpen)  // 
        {
            currentTime = Time.time;

            if (currentTime - lastTime > 0.01f)
            {
                /*
                string message;
                // Tool Tip1
                // Get force from finger proxy
                Vector3 force1 = toolTip1.GetComponent<FingerProxy>().force;
                //float torque = 0.0f; // index.GetComponent<FingerProxy>().torqueMag;
                force1.y *= -1;  // Changed direction to consider  Unity's LHC 
                float forceMag1 = force1.magnitude;

                // Tool tip2
                // Get force from finger proxy
                Vector3 force2 = toolTip2.GetComponent<FingerProxy>().force;
                //float torque = 0.0f; // index.GetComponent<FingerProxy>().torqueMag;
                force2.y *= -1;  // Changed direction to consider  Unity's LHC 
                float forceMag2 = force2.magnitude;

                //if (forceMag1 <= 0.1f && forceMag2 >= 0.0f)
                if (forceMag1 == 0.0f && forceMag2 >= 0.0f)
                {
                    //Send forces for tool 2 if tool 1 is inactive
                    message = forceMag2.ToString("0.00");
                    print("tool1 inactive -- tool 2 active");
                }
                //else if (forceMag2 <= 0.1f && forceMag1 >= 0.0f)
                else if (forceMag2 == 0.0f && forceMag1 >= 0.0f)
                {
                    //Send forces for tool 2 if tool 1 is inactive
                    message = forceMag1.ToString("0.00");
                    print("tool2 inactive -- tool 1 active");
                }
                else if (forceMag1 == 0.0f && forceMag2 == 0.0f)
                {
                    message = "0.00";
                    print("Both tools inactive");
                }
                else
                {
                    message = "0.00";
                    print("help");
                }
                */
                // Get force from finger proxy
                Vector3 force = toolTip.GetComponent<FingerProxy>().force;
                //float torque = 0.0f; // index.GetComponent<FingerProxy>().torqueMag;
                force.y *= -1;  // Changed direction to consider  Unity's LHC 
                float forceMag = force.magnitude;

                // Message for Regular hoxels
                //string message = index_force.x.ToString("0.00") + " " + index_force.y.ToString("0.00") + " " + index_force.z.ToString("0.00") + " " + index_magF.ToString("0.00") + " " + index_shear.ToString("0.00") + " " + index_torque.ToString("0.000") + "\n";
                // message = message + " " + thumb_force.x.ToString("0.00") + " " + thumb_force.y.ToString("0.00") + " " + thumb_force.z.ToString("0.00") + " " + thumb_magF.ToString("0.00") + " " + thumb_shear.ToString("0.00") + '\n';
                string message = forceMag.ToString("0.00");


                // Check to see if the old message is the same
                // If so dont sent the new message to avoid semaphore issue
                if (oldMessage != message)
                {
                    // Open stream
                    stream.Open();
                    stream.DiscardInBuffer();
                    stream.DiscardOutBuffer();
                    //Write to Arudino via serial

                    writeSerial(message);
                    lastTime = currentTime;

                    // Close stream to avoid semaphore error --- check later on Jasmin's PCs
                    stream.Close();
                    oldMessage = message;
                }

                //Read the serial data that came from arduino
                // readSerial();

                //Debug.Log("Back from Arduino");
                // lastTime = currentTime;

            }
        }
    }

    public void writeSerial(string message)
    {
        if (stream.IsOpen)
        {
            //read stuff
            try
            {
                stream.Write(message);
                Debug.Log("MESSAGE: " + message);
            }
            catch (IOException e)
            {
                //time out exception
                Debug.Log("Failed MESSAGE: " + message);
                print(e);
                print(Time.time);
            }
        }

    }

    public void readSerial()
    {
        if (stream.IsOpen)
        {
            try
            {
                //read stuff
                string arduinoMessage = stream.ReadLine();
                Debug.Log("arduinoMessage: " + arduinoMessage);
                arduinoDataVals = arduinoMessage.Split(',');
            }
            catch (System.TimeoutException)
            {
                //time out exception
                //Do Nothing
            }
        }
    }

    private void OnApplicationQuit()
    {
        //Close Serial Stream
        Debug.Log("<color=blue>GOODBYE</color>");
        stream.Close();

        /*Shut down the application*/
        //UnityEditor.EditorApplication.isPlaying = false;

        //Ignored in editor, used in build
        UnityEngine.Application.Quit();
    }
}
