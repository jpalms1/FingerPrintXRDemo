/*
  * Serial Communication from Unity to microcontroller controlled haptic devices 
  * Code written by Jasmin E. Palmer
  * Email: jasminp@standfor.edu
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

public class SerialCommsAdaFruit : MonoBehaviour
{
    GameObject index, thumb;

    //Set the port and the baud rate to 9600
    public string portName = "COM4";
    public int baudRate = 115200;
    //public int baudRate = 9600;
    SerialPort stream;

    private float lastTime = 0.0f;
    private float currentTime = 0.0f;

    public static string[] mcDataVals;
    //public static float[] unityDataVals;

    //public int expectedUnityEntries;

    private List<string[]> arduinoDataList;
    private List<float[]> unityDataList;

    private string oldMessage = "0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00\n";


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start Serial Comms");
        //initialize lists
        arduinoDataList = new List<string[]>();
        unityDataList = new List<float[]>();

        //Start the hand behavior/game logic as disabled until serial comms is up
        index = GameObject.Find("Index");
        thumb = GameObject.Find("Thumb");

        //Define and open serial port       
        stream = new SerialPort(portName, baudRate);
        //Serial Port Read and Write Timeouts
        stream.ReadTimeout = 10;//5;
        stream.WriteTimeout = 10;

        try
        {
            stream.Open();
            stream.DiscardInBuffer();
            stream.DiscardOutBuffer();
            Debug.Log("<color=green>Serial Communication Established</color>");

            //Enable Game Logic
            //GetComponent<HapticController>().enabled = true;
            Debug.Log("<color=blue>Haptic Controller Enabled</color>");
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
        writeSerial("0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.00 0.000\n");
        //stream.Close();
        readSerial();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("SerialComms.cs");
        currentTime = Time.time;

        if (stream.IsOpen)  // (stream.IsOpen || !stream.IsOpen) 
        {
            if (currentTime - lastTime > 0.1f)
            {
                // Get force from finger proxy
                Vector3 indexForce = index.GetComponent<FingerProxy>().force;
                Vector3 thumbForce = thumb.GetComponent<FingerProxy>().force;
                float indexTorque = index.GetComponent<FingerProxy>().torqueMag;
                float thumbTorque = thumb.GetComponent<FingerProxy>().torqueMag;
                float indexMagF = index.GetComponent<FingerProxy>().forceMag;
                float thumbMagF = thumb.GetComponent<FingerProxy>().forceMag;

                float indexShear = Mathf.Sqrt(indexForce.x * indexForce.x + indexForce.y * indexForce.y);
                float thumbShear = Mathf.Sqrt(thumbForce.x * thumbForce.x + thumbForce.y * thumbForce.y);
                indexForce.y *= -1;  // Changed direction to consider  Unity's LHC 
                thumbForce.x *= -1; // Changed direction to consider  Unity's LHC 
                thumbForce.y *= -1; // Changed direction to consider  Unity's LHC 

                // Message for Hoxels only
                string deviceMessage0 = indexForce.x.ToString("0.00") + " " + indexForce.y.ToString("0.00") + " " + indexForce.z.ToString("0.00") + " " + indexMagF.ToString("0.00") + " " + indexShear.ToString("0.00") + " " + "0.000";
                string deviceMessage1 = thumbForce.x.ToString("0.00") + " " + thumbForce.y.ToString("0.00") + " " + thumbForce.z.ToString("0.00") + " " + thumbMagF.ToString("0.00") + " " + thumbShear.ToString("0.00") + " " + "0.000";

                //message = message + thumbForce.x.ToString("0.00") + " " + thumbForce.y.ToString("0.00") + " " + thumbForce.z.ToString("0.00") + " " + thumbMagF.ToString("0.00") + " " + thumbShear.ToString("0.00") + "0.000" + "\n";
                /*
                // Message for Finger Prints only
                string message2 = indexForce.x.ToString("0.00") + " " + indexForce.y.ToString("0.00") + " " + indexForce.z.ToString("0.00") + " " + indexMagF.ToString("0.00") + " " + indexShear.ToString("0.00") + " " + indexTorque.ToString("0.000")+ "\n";
                //message2 = message2 + thumbForce.x.ToString("0.00") + " " + thumbForce.y.ToString("0.00") + " " + thumbForce.z.ToString("0.00") + " " + thumbMagF.ToString("0.00") + " " + thumbShear.ToString("0.00") + " " + thumbTorque.ToString("0.000") + "\n";

                // Message for Hoxels as device 0 and Finger Prints as device 1
                string message3 = indexForce.x.ToString("0.00") + " " + indexForce.y.ToString("0.00") + " " + indexForce.z.ToString("0.00") + " " + indexMagF.ToString("0.00") + " " + indexShear.ToString("0.00") + " " + "0.000" + "\n";
                //message3 = message3 + thumbForce.x.ToString("0.00") + " " + thumbForce.y.ToString("0.00") + " " + thumbForce.z.ToString("0.00") + " " + thumbMagF.ToString("0.00") + " " + thumbShear.ToString("0.00") + " " + thumbTorque.ToString("0.000") + "\n";
                */

                //string message = deviceMessage0+"\n";// + " " + deviceMessage1 + "\n";
                string message = deviceMessage0 + " " + deviceMessage1 + "\n";



                // Check to see if the old message is the same
                // If so dont sent the new message to avoid semaphore issue
                if (oldMessage != message)
                {
                    // Open stream
                    //stream.Open();
                    //stream.DiscardInBuffer();
                    //stream.DiscardOutBuffer();
                    //Write to Arudino via serial

                    //writeSerial(message);
                    oldMessage = message;
                    lastTime = currentTime;

                    // Close stream to avoid semaphore error --- check later on Jasmin's PCs
                    //stream.Close();
                }

                writeSerial(message);
                // Read the serial data that came from arduino
                readSerial();

                //Debug.Log("Back from MC");
                // lastTime = currentTime;

            }
        }
    }

    public void writeSerial(string message)
    {
        if (stream.IsOpen)
        {
            //write to serial
            try
            {
                stream.DiscardOutBuffer(); // Optional: Clear the output buffer
                stream.Write(message);
                Debug.Log("MESSAGE: " + message);
            }
            catch (IOException e)
            {
                //time out exception
                Debug.Log("Runtime: " + Time.time + " --- Failed MESSAGE: " + message + "---" + e);
                //print(e);
                //print("Runtime: " + Time.time);
            }
        }
        else
        {
            print("Tragedy: I have closed");
        }

    }

    public void readSerial()
    {
        if (stream.IsOpen && stream.BytesToRead > 0)
        {
            try
            {
                //read from serial
                stream.DiscardInBuffer(); // Optional: Clear the input buffer
                string mcMessage = stream.ReadLine();
                Debug.Log("mcMessage: " + mcMessage);
                mcDataVals = mcMessage.Split(' ');
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
        if (stream.IsOpen)
        {
            stream.Close();
        }

        /*Shut down the application*/
        //UnityEditor.EditorApplication.isPlaying = false;

        //Ignored in editor, used in build
        UnityEngine.Application.Quit();
    }
}
