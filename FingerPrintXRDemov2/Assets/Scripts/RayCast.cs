/*
  * Basic raycasting, detects what the tip of the needle is hitting as it is inserted
  * Code written by Jasmin E. Palmer
  * Email: jasminp@stanford.edu
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class RayCast : MonoBehaviour
{
    public GameObject[] hapticTargets;
    public bool isNeedleInPigPhantom;
    public bool isNeedleInSpine;

    // Raycast will only detect hits with this mask
    public LayerMask hapticTargetMask;

    // Start is called before the first frame update
    void Start()
    {
        print("Raycast Start");
        // Contains the names of each game object the needle can detect
        hapticTargets = new GameObject[] {
            GameObject.Find("PigPhantom"),
            GameObject.Find("Spineitk")
            //GameObject.Find("Muscle"),
            //GameObject.Find("Male_Skeletal_Intervertabral_Discs_Geo"),
            //GameObject.Find("Nervous_Brain_Stem_Geo"),
            //GameObject.Find("Skeleton")
            };

        hapticTargetMask = LayerMask.GetMask("PhantomAnatomy");

        // Get the Renderer component from the new cube
        var renderer = GetComponent<Renderer>();

        // Create a new RGBA color using the Color constructor and store it in a variable
        Color customColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);//new Color(0.4f, 0.9f, 0.7f, 1.0f);

        // Call SetColor using the shader property name "_Color" and setting the color to the custom color you created
        renderer.material.SetColor("_Color", customColor);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward); // Ray out the negative y-axis
        RaycastHit hitInfo;
        // Does the ray intersect desired objects
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, hapticTargetMask))
        {
            print(hitInfo.collider.gameObject.name);
            print(hitInfo.distance);

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
            //if (hitInfo.collider.gameObject.name == hapticTargets[0].name)
            if (hitInfo.rigidbody.gameObject.name == hapticTargets[0].name)
            {
                //Combine skin and muscle feedback but use skin mesh collider
                isNeedleInPigPhantom = true;
                isNeedleInSpine = false;
                print("PigPhantom  ENTER");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
            }
            //if (hitInfo.collider.gameObject.name == hapticTargets[1].name)
            if (hitInfo.rigidbody.gameObject.name == hapticTargets[1].name)
            {
                //Combine skin and muscle feedback but use skin mesh collider
                isNeedleInPigPhantom = false;
                isNeedleInSpine = true;
                print("Spine  ENTER");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.green);
            //UnityEngine.Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.green);
            Debug.Log("Did not Hit");
        }
    }
}

