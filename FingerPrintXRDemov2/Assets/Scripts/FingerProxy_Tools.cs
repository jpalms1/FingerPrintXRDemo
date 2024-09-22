/*
  * Script to apply finger proxy algorithm
  * Re-written by Jasmin E. Palmer for MARSS 2024 Hackathon
  * 
  * Code further modified by Jasmin E. Palmer for FingerPrint VR Demo
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using System.Collections.Specialized;  // Use this namespace for TextMeshPro


public class FingerProxy_Tools : MonoBehaviour
{
    // Set to the proxy object from unity
    GameObject proxyGO;
    public Transform proxyToPlace;

    // Parent object the user will hold
    public Transform hapticObject;

    // Masks for detecting what can and cannot be hit by raycast
    public LayerMask mask, fingerMask;

    // Variables to keep track of distance, force, torque, and angles
    public Vector3 distance, force, normalForce;
    public Vector3 torque;
    public float forceMag;
    public float torqueMag;
    float oldAngle, angleChange; // Previous angle value (absolute), delta angle

    // enteredMesh keeps track of if the finger just entered this frame
    // isFingerInMesh keeps track of if the finger is in the mesh
    bool enteredMesh;
    public bool isFingerInMesh;

    // Constants for calculating torque  -- from Papers in Talise's GDrive --  "Char. of Tissue Stiffness", "Mechanical Properties and Young's Mod"
    float fingerRadius = 0.002f;
    float skinFriction = 0.46f;
    float skinThickness = 0.02f;
    float skinTwistRadius = 0.01f;
    // Skin Stiffness of 266.3 N/m
    float skinStiffness = 266.3f;
    // 4.2 * 10^5 N/m^2
    float skinElasticity = 4.2f * (float)Math.Pow(10, 5);

    //float halfProxyLength =  0.01525; //  distance from orgin of finger proxy to tip of the finger proxy, i.e. : origin finger gameobject to tip to finger gameobject
    public float halfProxyLength; // distance from orgin of finger proxy to tip of the finger proxy, i.e. : origin finger gameobject to tip to finger gameobject

    // Adjustment accounts for origin at center of object, not at tip, must be set based on size of finger
    //Vector3 adjustment = new Vector3(0, 0, halfProxyLength);
    Vector3 adjustment;
    Vector3 raycastDir;

    public float hitPointToHIPDistance;
    public float hitPointToProxyDistance;
    public Vector3 hipToHitPointVector;
    public Vector3 proxyToHitPointVector;
    public Vector3 hitPointNormal;

    //public float dotResult;

    RaycastHit hitInfo;
    RaycastHit hitInfo2;

    public float anatomyStiffness = 0.5f;

    GameObject textGO;
    Renderer hipRenderer;
    Renderer proxyRenderer;

    public GameObject player;

    public string hapticLabel;


    // Start is called before the first frame update
    void Start()
    {
        halfProxyLength = 0.5f * transform.localScale.z;
        adjustment = new Vector3(0, 0, halfProxyLength);


        // Set the transform just in case it's not done in the Inspector:
        //proxyGO = this.gameObject.transform.GetChild(0);
        proxyToPlace = this.gameObject.transform.GetChild(0).GetComponent<Transform>().transform;
        hapticObject = this.transform.parent.gameObject.transform;

        mask = LayerMask.GetMask("PhantomAnatomy");
        fingerMask = LayerMask.GetMask("HapticTools");

        // For writing collisions to canvas
        textGO = GameObject.Find("Current Anatomy Text");

        // Make object invisible
        /*
        hipRenderer = GetComponent<Renderer>();
        hipRenderer.enabled = false;
        proxyRenderer = this.gameObject.transform.GetChild(0).GetComponent<Renderer>();
        proxyRenderer.enabled = false;
        */
        player = GameObject.Find("Player");


        hapticLabel = "Haptic Anatomy";

    }

    // Update is called once per frame
    void Update()
    {
        // Forward facing ray
        raycastDir = transform.forward;
        Ray ray = new Ray(transform.position, raycastDir);

        // Backward facing ray
        Vector3 rayStartPosition = new Vector3(0, 0, 0);
        // Draw the ray from a starting point far away
        // Raycast only detects hits from the outside of the mesh so the ray must start from the opposite side
        rayStartPosition += (transform.position + raycastDir);
        //Ray ray2 = new Ray(rayStartPosition, -raycastDir);
        Vector3 raycastDir2 = -raycastDir;
        Ray ray2 = new Ray(transform.position, raycastDir2);

        hitPointNormal = hitInfo.normal;
        UnityEngine.Debug.DrawLine(proxyToPlace.transform.position, proxyToPlace.transform.position + hitPointNormal, Color.cyan);
        UnityEngine.Debug.DrawLine(transform.position, transform.position + hitPointNormal, Color.gray);

        hitPointToHIPDistance = Vector3.Distance(hitInfo.point, transform.position);
        hitPointToProxyDistance = Vector3.Distance(hitInfo.point, proxyToPlace.transform.position);

        hipToHitPointVector = hitInfo.point - transform.position;
        proxyToHitPointVector = hitInfo.point - proxyToPlace.transform.position;

        //dotResult = Vector3.Dot(hipToHitPointVector, hitPointNormal);

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask))
        {
            /*
            //if (Vector3.Distance(hitInfo.point, transform.position) <= halfProxyLength)
            if (hitPointToProxyDistance <= halfProxyLength) //(Vector3.Distance(hitInfo.point, transform.position) <= halfProxyLength)
            {
                isFingerInMesh = true;
                //print(hitPointToProxyDistance);

                UnityEngine.Debug.DrawLine(ray.origin, ray.origin + raycastDir, Color.green);
            }
            else
            {
                isFingerInMesh = false;
                UnityEngine.Debug.DrawLine(ray.origin, ray.origin + raycastDir, Color.red);
            }
            */
        }

        //// Detect a hit from the backwards ray // ray from orgin of finger and outwards toward the skin
        if (Physics.Raycast(ray2, out hitInfo2, 100, mask))
        {
            print("ACTUALLY INSIDE: " + hitInfo2.collider.name);

            isFingerInMesh = true;
            UnityEngine.Debug.DrawLine(ray2.origin, ray2.origin + raycastDir2, Color.blue);// print("blue");

            writeToCanvas(hitInfo2.collider.name);

            //playAudio(hitInfo2.collider);
        }
        else
        {
            print("NO CONTACT");
            isFingerInMesh = false;
            //stopAudio();
        }

        if (isFingerInMesh)
        {
            //print("in the skin");
            //UnityEngine.Debug.DrawLine(ray2.origin, ray2.origin + hitInfo2.point, Color.blue);// print("blue");

            // Finding the angle for Torque Rendering:
            // Set the initial angle for when the finger entered the skin and current angle
            if (enteredMesh == false)
            {
                enteredMesh = true;
                oldAngle = transform.eulerAngles.z;
                angleChange = 0;
            }
            else
            {
                float angleDelta = transform.eulerAngles.z - oldAngle;
                if (angleDelta > 100)  // Set a lower threshold due to being within the Update function (1 frame)
                {
                    angleDelta -= 360;

                }
                else if (angleDelta < -100)
                {
                    angleDelta += 360;
                }
                oldAngle = transform.eulerAngles.z;
                angleChange += angleDelta;
                // print(angleChange);
            }

            // The rest of finger proxy
            // Finger is inside the skin so move proxy to hit point
            proxyToPlace.position = hitInfo2.point;

            // Calculate force using stiffness of objects being manipulated
            distance = (transform.position + adjustment) - proxyToPlace.position;
            force = anatomyStiffness * distance;

            forceMag = force.magnitude;

            // Calculate torque
            // Torque from normal force wrt contact surface only (see paper)
            normalForce = (Vector3.Dot(force, hitInfo2.normal)) * hitInfo2.normal;
            torque = (2 * skinFriction * fingerRadius / 3) * normalForce;

            // Account for any rotation
            // Given a 100 MPa for skin 100 MPa * pi * r^4 / 2 = k
            float skinTorsionalConstant = skinElasticity * 2 * (float)Math.PI * 0.4f * skinThickness * fingerRadius * skinTwistRadius;
            float rotationalTorque = skinTorsionalConstant * angleChange * (float)Math.PI / 180;

            torqueMag = rotationalTorque + torque.z;
        }
        else
        {
            // Finger is outside the skin
            //print("outside the skin ~~ 1");
            proxyToPlace.position = transform.position;
            distance = (transform.position) - proxyToPlace.position;
            //force = skinStiffness * distance;
            force = Vector3.zero;
            forceMag = 0.0f;
            torque *= 0.0f;
            torqueMag = 0.0f;
            enteredMesh = false;
        }
    }

    void writeToCanvas(string anatomyText) // Not robustly done -- will be a problem if you change the mesh names
    {
        TMP_Text newText = textGO.GetComponent<TMP_Text>(); // Fetch the TMP_Text component

        // Modify the text to be more presentable
        if (anatomyText == "mesenteric_a_Mesh")
        {
            anatomyText = "Mesenteric Artery";
        }
        else if (anatomyText == "mesenteric")
        {
            anatomyText = "Mesenteric Vein";
        }
        else if (anatomyText == "01_a_mesenterica inferior_Mesh")
        {
            anatomyText = "Mesenterica Inferior Artery";
        }
        else if (anatomyText == "02_v_cavaMesh")
        {
            anatomyText = "V. Cava";
        }
        else if (anatomyText == "03_a_iliac_branch_Mesh")
        {
            anatomyText = "Iliac Branch Artery";
        }
        else if (anatomyText == "04_ureter_left_Mesh")
        {
            anatomyText = "Left Ureter";
        }
        else if (anatomyText == "05_tumor_Mesh")
        {
            anatomyText = "Tumor";
        }
        else if (anatomyText == "06_reter_right_Mesh")
        {
            anatomyText = "Right Ureter";
        }
        else if (anatomyText == "07_sphincter_Mesh")
        {
            anatomyText = "Sphincter";
        }
        else if (anatomyText == "08_sacrum_Mesh")
        {
            anatomyText = "Sacrum";
        }
        else if (anatomyText == "SpineitkMesh")
        {
            anatomyText = "Spine";
        }
        else
        {
            anatomyText = " ";
        }

        newText.text = anatomyText; // Update the text

    }



}
