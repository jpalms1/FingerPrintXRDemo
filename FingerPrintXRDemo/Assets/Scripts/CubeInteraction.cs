using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CubeInteraction : MonoBehaviour
{
    private Rigidbody cubeRb;

    void Start()
    {
        cubeRb = GetComponent<Rigidbody>();

        // Optional: Set a custom mass or inertia tensor to better match the real-world physics
        cubeRb.mass = 1f;
        cubeRb.inertiaTensor = new Vector3(1f, 1f, 1f);
    }

    void FixedUpdate()
    {
        // Apply gravity and check if any of the forces from the fingers are lifting it
        if (cubeRb.velocity.magnitude > 0)
        {
           // UnityEngine.Debug.Log("Cube is being manipulated by the fingers!");
        }
    }
}

