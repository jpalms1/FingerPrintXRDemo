using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerInteraction : MonoBehaviour
{
    public GameObject hapticCube;
    public Rigidbody hapticCubeRb; // Assign the cube's Rigidbody in the inspector
    public float interactionForceMultiplier = 10f; // Tune this multiplier to control force strength
    public PhysicMaterial gripMaterial; // Physic material with proper friction

    private Rigidbody fingerRb;
    private bool isTouchingCube = false;
    private Vector3 contactPoint;
    Vector3 force;
    void Start()
    {
        hapticCube = GameObject.Find("HapticCube");
        hapticCubeRb = hapticCube.GetComponent<Rigidbody>();

        fingerRb = GetComponent<Rigidbody>();

        // Set the friction properties of the sphere
        if (gripMaterial != null)
        {
            Collider col = GetComponent<Collider>();
            col.material = gripMaterial;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the sphere is in contact with the cube
        if (collision.gameObject == hapticCube)
        {
            //isTouchingCube = true;
            contactPoint = collision.contacts[0].point;
        }
    }

    void FixedUpdate()
    {
        // Get the force from FingerProxy.cs
        force = transform.parent.gameObject.GetComponent<FingerProxy>().force;
                    
        if (Mathf.Sqrt(force.sqrMagnitude) >= 0.1f)
        {
            isTouchingCube = true;
        }
        else
        {
            isTouchingCube = false;
        }

        if (isTouchingCube)
        {
            
            ApplyForceToCube();
        }
    }

    void ApplyForceToCube()
    {
        // Calculate the interaction force based on the finger's movement and contact point
        //Vector3 fingerVelocity = fingerRb.velocity;
        //Vector3 forceDirection = (hapticCube.transform.position - contactPoint).normalized;

        // Compute the force based on the velocity and a multiplier (tune to your needs)
        //Vector3 interactionForce = forceDirection * fingerVelocity.magnitude * interactionForceMultiplier;

        // Apply the force at the contact point on the cube's Rigidbody
        hapticCubeRb.AddForceAtPosition(force, contactPoint);
    }
}
