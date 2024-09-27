/*
  * Script to apply finger proxy algorithm
  * Code written by Talise Baker-Matsuoka as part of the NSF REU 2024 program
  * Mentored by Jasmin E. Palmer
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;


public class FingerProxy : MonoBehaviour
{
    // Set to the proxy object from unity
    GameObject proxyGO;
    public Transform proxyToPlace;

    // Haptic Interaction point object
    public Transform hapticObject;

    // Masks for detecting what can and cannot be hit by raycast
    public LayerMask mask, fingerMask;

    // Variables to keep track of distance, force, torque, and angles
    public Vector3 distance, force, normalForce;
    public Vector3 torque;
    public float forceMag;
    public float torqueMag;
    public float oldAngle, angleChange; // Previous angle value (absolute), delta angle

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
        hapticObject = this.gameObject.transform;

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
        raycastDir = -transform.forward;
        Ray ray = new Ray(transform.position, raycastDir);

        // Backward facing ray
        Vector3 rayStartPosition = new Vector3(0, 0, 0);
        // Draw the ray from a starting point far away
        // Raycast only detects hits from the outside of the mesh so the ray must start from the opposite side
        rayStartPosition += (transform.position + transform.forward);
        Ray ray2 = new Ray(rayStartPosition, raycastDir);
        

        // Count the number of hits to determine if inside or outside the skin
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 100, mask);
        if (hits.Length == 0)
        {
            isFingerInMesh = true;
        }
        else if (Vector3.Distance(hits[0].point, transform.position) <= 0.01525) // 0.01525 =  distance from orgin of finger object to tip to finger object
        {
            isFingerInMesh = true;
        }
        else
        {
            isFingerInMesh = false;
        }

        // Detect a hit from the backwards ray // ray from orgina of finger and outwards toward the skin
        RaycastHit hit;
        if (Physics.Raycast(ray2, out hit, 100, mask))
        {
            if (isFingerInMesh)
            {
                // UnityEngine.Debug.DrawLine(ray2.origin, transform.position, Color.red);
                
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
                proxyToPlace.position = hit.point;

                // Calculate force using skin stiffness
                distance = (transform.position + adjustment) - proxyToPlace.position;
                force = skinStiffness * distance;
                forceMag = force.magnitude;


                // Calculate torque
                // Torque from normal force only (see paper)
                normalForce = (Vector3.Dot(force, hit.normal)) * hit.normal;
                torque = (2 * skinFriction * fingerRadius / 3) * normalForce;

                // Account for any rotation
                // Given a 100 MPa for skin 100 MPa * pi * r^4 / 2 = k
                float skinTorsionalConstant = skinElasticity * 2 * (float)Math.PI * 0.4f * skinThickness * fingerRadius * skinTwistRadius;
                float rotationalTorque =  skinTorsionalConstant * angleChange * (float)Math.PI / 180;

                torqueMag = rotationalTorque + torque.z;
            }
            else
            {
                // Finger is outside the skin
                proxyToPlace.position = transform.position;
                distance = (transform.position) - proxyToPlace.position;
                force = skinStiffness * distance;
                forceMag = 0.0f;
                torque *= 0.0f;
                torqueMag = 0.0f;
                enteredMesh = false;
            }
        }
        else
        {
            // Finger is outside the skin
            proxyToPlace.position = transform.position;
            distance = transform.position - proxyToPlace.position;
            force = skinStiffness * distance;
            torque *= 0;
            torqueMag = 0;
            enteredMesh = false;
        }
        //print(distance);
    }
}
